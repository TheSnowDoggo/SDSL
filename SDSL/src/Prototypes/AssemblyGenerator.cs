using System.Collections.Frozen;
using SDSL.Classes;
using SDSL.Expressions;
using SDSL.Functions;

namespace SDSL.Prototypes;

public class AssemblyGenerator
{
    public const string EntryPointName = "main";
    
    private readonly PrototypeAssembly _pAssembly;
    private SealAssembly _assembly;

    public AssemblyGenerator(PrototypeAssembly pAssembly)
    {
        _pAssembly = pAssembly;
    }
    
	public SealAssembly GenerateAssembly()
    {
        AllocateAssembly();

        BuildClasses();
        
        GenerateMembers();

        var assembly = _assembly;
        _assembly = null;
        
        return assembly;
    }
    
    private IEnumerable<PrototypeClass> GetClasses()
    {
        foreach (PrototypeNamespace pNamespace in _pAssembly.Namespaces.Values)
        {
            foreach (PrototypeClass pClass in pNamespace.Classes.Values)
            {
                yield return pClass;
            }
        }
    }
    
    private void AllocateAssembly()
    {
        int staticFunctionCount = 0;
        int staticFieldCount = 0;

        foreach (PrototypeClass pClass in GetClasses())
        {
            // Resolving usings can be done as soon as all the prototype parsing is done
            ResolveUsings(pClass);
            
            ResolveBaseClass(pClass);

            // Allocate functions
            
            List<PrototypeFunction> nativeFunctions = pClass.NativeFunctions;
            
            for (int i = 0; i < nativeFunctions.Count; i++)
            {
                nativeFunctions[i].AssemblyLocation = staticFunctionCount++;
            }

            // Alocate static fields
            
            List<PrototypeField> nativeFields = pClass.NativeFields;

            for (int i = 0; i < nativeFields.Count; i++)
            {
                PrototypeField pField = nativeFields[i];
                
                if (pField.IsStatic)
                {
                    pField.AssemblyLocation = staticFieldCount++;
                }
            }
            
            if (pClass.Class == SealGlobal.Class)
            {
                _pAssembly.GlobalClass = pClass;
            }
        }

        _assembly = new SealAssembly(
            _pAssembly.Name,
            new Function[staticFunctionCount],
            new Field[staticFieldCount]
        );
    }
    
    private void ResolveUsings(PrototypeClass pClass)
    {
        var namespaces = new HashSet<PrototypeNamespace>();
        
        namespaces.Add(pClass.Namespace);
        
        for (int i = 0; i < pClass.UsingsNames.Length; i++)
        {
            string usingName = pClass.UsingsNames[i];

            if (!_pAssembly.Namespaces.TryGetValue(usingName, out PrototypeNamespace pNamespace))
            {
                throw new ParserException(SourceLocation.Invalid, $"Failed to resolve namespace '{usingName}' for class {pClass}.");
            }
            
            namespaces.Add(pNamespace);
        }
        
        pClass.Usings = namespaces.ToArray();
    }

    private static void ResolveBaseClass(PrototypeClass pClass)
    {
        if (pClass.BaseClassDataType == null)
        {
            return;
        }

        PrototypeClass baseClass = pClass.ResolveDataTypeClass(pClass.BaseClassDataType);

        if (pClass == baseClass)
        {
            throw new ParserException(pClass.BaseClassDataType.Location,
                $"Class {pClass} cannot inherit from itself.");
        }

        pClass.BaseClass = baseClass;
    }
    
    private void BuildClasses()
    {
        foreach (PrototypeClass pClass in GetClasses())
        {
            pClass.Class.CurrentAssembly = _assembly;
            
            BuildClass(pClass);
        }
    }
    
    private static Stack<PrototypeClass> ImportBaseClasses(PrototypeClass pClass)
    {
        var buildStack = new Stack<PrototypeClass>();

        var functions = new Dictionary<string, PrototypeFunction>();
        
        var functionTable = new Dictionary<string, int>();
        
        var baseClasses = new HashSet<SealClass>();
        
        PrototypeClass currentClass = pClass;

        while (currentClass != null)
        {
            if (!baseClasses.Add(currentClass.Class))
            {
                throw new ParserException(pClass.BaseClassDataType.Location,
                    $"Class {pClass} has recursive base class {currentClass}.");
            }
            
            buildStack.Push(currentClass);

            List<PrototypeFunction> nativeFunctions = currentClass.NativeFunctions;

            for (int i = 0; i < nativeFunctions.Count; i++)
            {
                PrototypeFunction pFunction = nativeFunctions[i];
                
                if (!functions.TryAdd(pFunction.Name, pFunction))
                {
                    continue;
                }
                
                if (!pFunction.IsStatic)
                {
                    functionTable.Add(pFunction.Name, pFunction.AssemblyLocation);
                }
            }
            
            currentClass = currentClass.BaseClass;
        }

        pClass.Functions = functions.ToFrozenDictionary();
        
        pClass.Class.FunctionTable = functionTable.ToFrozenDictionary();
        
        pClass.Class.BaseClasses = baseClasses.ToFrozenSet();

        return buildStack;
    }
    
    private static void BuildClass(PrototypeClass pClass)
    {
        Stack<PrototypeClass> buildStack = ImportBaseClasses(pClass);

        var fields = new Dictionary<string, PrototypeField>();
        var constants = new Dictionary<string, PrototypeConstant>();
        
        var fieldTable = new Dictionary<string, int>();
        
        while (buildStack.TryPop(out PrototypeClass baseClass))
        {
            List<PrototypeField> nativeFields = baseClass.NativeFields;
            
            for (int i = 0; i < nativeFields.Count; i++)
            {
                PrototypeField pField = nativeFields[i];
                
                if (!fields.TryAdd(pField.Name, pField))
                {
                    throw new ParserException(pField.Location,
                        $"Class {baseClass} had duplicate field '{pField.Name}' defined in a base class or {pClass}.");
                }

                if (pField.IsStatic)
                { 
                    continue;
                }
                
                int location = fieldTable.Count;
                    
                fieldTable.Add(pField.Name, location);
                pField.AssemblyLocation = location;
            }

            List<PrototypeConstant> nativeConstants = baseClass.NativeConstants;
            
            for (int i = 0; i < nativeConstants.Count; i++)
            {
                PrototypeConstant pConstant = nativeConstants[i];
                
                if (!constants.TryAdd(pConstant.Name, pConstant))
                {
                    throw new ParserException(pConstant.Location,
                        $"Class {baseClass} had duplicate constant '{pConstant.Name}' defined in a base class or {pClass}.");
                }
            }
        }

        pClass.Fields = fields.ToFrozenDictionary();
        pClass.Constants = constants.ToFrozenDictionary();

        pClass.Class.FieldTable = fieldTable.ToFrozenDictionary();
    }
    
    private void GenerateMembers()
    {
        var staticFieldExpressions = new Expression[_assembly.StaticFields.Length];
        
        foreach (PrototypeClass pClass in GetClasses())
        {
            GenerateConstructor(pClass);
            
            List<PrototypeFunction> nativeFunctions = pClass.NativeFunctions;

            for (int i = 0; i < nativeFunctions.Count; i++)
            {
                PrototypeFunction pFunction = nativeFunctions[i];
                
                _assembly.StaticFunctions[pFunction.AssemblyLocation] = GenerateFunction(pFunction);
            }
            
            SealClass sClass = pClass.Class;
            
            var instanceFields = new FieldDefinition[sClass.FieldTable.Count];

            List<PrototypeField> nativeFields = pClass.NativeFields;
            
            for (int i = 0; i < nativeFields.Count; i++)
            {
                PrototypeField pField = nativeFields[i];
                
                SealClass fieldClass = pField.NativeClass.ResolveDataTypeSealClass(pField.DataType);
                
                Expression expression;
                
                if (pField.Tokens.Count == 0)
                {
                    expression = null;
                }
                else
                {
                    expression = new ExpressionParser(
                        new TokenStream(pField.Tokens),
                        _assembly,
                        ExpressionParsingMode.Statement,
                        pClass
                    ).Parse();
                }

                int location = pField.AssemblyLocation;
                
                if (pField.IsStatic)
                {
                    staticFieldExpressions[location] = expression;
                    
                    _assembly.StaticFields[location] = new Field(fieldClass, pField.IsConst, SealValue.Nil);
                }
                else
                {
                    instanceFields[location] = new FieldDefinition(fieldClass, pField.IsConst, expression);
                }
            }
            
            sClass.InstanceFields = instanceFields;
        }

        EvaluateStaticFields(staticFieldExpressions);
    }

    private void GenerateConstructor(PrototypeClass pClass)
    {
        SealClass sClass = pClass.Class;
        PrototypeFunction pConstructor = pClass.Constructor;
        
        if (pConstructor == null)
        {
            if (!sClass.GenerateConstructor
                || sClass.ValueType != ValueType.Object)
            {
                return;
            }

            sClass.Constructor = new UserConstructor(
                SourceLocation.Invalid,
                sClass,
                [], 0, 0,
                null
            );
            
            return;
        }
        
        switch (pConstructor.Body)
        {
            case UserFunctionBody userFunctionBody:
                UserFunction userFunction = new UserFunctionParser(
                    new TokenStream(userFunctionBody.Tokens),
                    pConstructor,
                    _assembly
                ).Parse();

                sClass.Constructor = new UserConstructor(
                    userFunction.Location,
                    userFunction.Class,
                    userFunction.Args,
                    userFunction.MinArgs,
                    userFunction.MaxArgs,
                    userFunction
                );
                
                break;
            case NativeFunctionBody nativeFunctionBody:
                sClass.Constructor = NativeFunction.Create(
                    pConstructor,
                    nativeFunctionBody.Func
                );
                
                break;
            default:
                throw new InvalidOperationException(
                    $"Prototype function body is unknown: {pConstructor.Body}.");
        }
    }

    private void RegisterEntryPoint(UserFunction function)
    {
        if (!function.IsStatic
            || function.Name != EntryPointName)
        {
            return;
        }
        
        if (_assembly.EntryPoint != null)
        {
            throw new ParserException(function,
                $"Entry point has already been defined: {_assembly.EntryPoint}.");
        }

        switch (function.MinArgs)
        {
        case 0:
            break;
        case 1:
            SealClass sClass = function.Args[0].Class;

            if (sClass != null && sClass != SealArray.Class)
            {
                throw new ParserException(function,
                    $"Entry point argument must allow {SealArray.Class}.");
            }
            
            break;
        default:
            throw new ParserException(function,
                "Entry point must take either 0 or 1 args.");
        }
        
        _assembly.EntryPoint = function;
    }

    private Function GenerateFunction(PrototypeFunction pFunction)
    {
        switch (pFunction.Body)
        {
        case UserFunctionBody userFunctionBody:
            UserFunction userFunction = new UserFunctionParser(
                new TokenStream(userFunctionBody.Tokens),
                pFunction,
                _assembly
            ).Parse();

            RegisterEntryPoint(userFunction);

            return userFunction;
        case NativeFunctionBody nativeFunctionBody:
            return NativeFunction.Create(
                pFunction,
                nativeFunctionBody.Func
            );
        default:
            throw new InvalidOperationException(
                $"Prototype function body is unknown: {pFunction.Body}.");
        }
    }

    private void EvaluateStaticFields(Expression[] staticFieldExpressions)
    {
        Field[] staticFields = _assembly.StaticFields;

        for (int i = 0; i < staticFields.Length; i++)
        {
            ref Field field = ref staticFields[i];
            
            Expression expression = staticFieldExpressions[i];

            SealValue defaultValue = expression != null
                ? expression.Evaluate(null)
                : SealClass.GetDefaultValue(field.Class);

            field.Value = defaultValue;
            
            SealClass sClass = field.Class == SealClass.Implicit
                ? defaultValue.Class
                : field.Class;
            
            field.Class = sClass;
        }
    }
}