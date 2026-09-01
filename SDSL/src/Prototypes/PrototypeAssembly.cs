using System.Collections.Frozen;
using SDSL.Expressions;
using SDSL.Classes;
using SDSL.Functions;

namespace SDSL.Prototypes;

public class PrototypeAssembly
{
    public const string EntryPointName = "main";
    
    public PrototypeAssembly(string name)
    {
        Name = name;
    }
    
    public string Name { get; }

    public Dictionary<string, PrototypeNamespace> Namespaces { get; } = [];
    public HashSet<string> GlobalUsings { get; } = [];
    
    public PrototypeClass GlobalClass { get; private set; }
    
    public void GenerateAssembly()
    {
        AllocateAssembly();
        
        AssemblyGeneration();
    }
    
    public PrototypeNamespace GetOrCreateNamespace(string name)
    {
        if (Namespaces.TryGetValue(name, out PrototypeNamespace pNamespace))
        {
            return pNamespace;
        }

        pNamespace = new PrototypeNamespace(this, name);
        
        Namespaces.Add(name, pNamespace);

        return pNamespace;
    }
    
    public override string ToString()
    {
        return $"Assembly<{Name}>";
    }
    
    private IEnumerable<PrototypeClass> GetClasses()
    {
        foreach ((_, PrototypeNamespace pNamespace) in Namespaces)
        foreach ((_, PrototypeClass pClass) in pNamespace.Classes)
            yield return pClass;
    }
    
    private void ResolveUsings(PrototypeClass pClass)
    {
        var namespaces = new HashSet<PrototypeNamespace>();
        
        namespaces.Add(pClass.Namespace);
        
        for (int i = 0; i < pClass.UsingsNames.Length; i++)
        {
            string usingName = pClass.UsingsNames[i];

            if (!Namespaces.TryGetValue(usingName, out PrototypeNamespace pNamespace))
            {
                throw new InvalidOperationException(
                    $"{ToString()} Failed to resolve namespace {usingName}.");
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

    private void AllocateAssembly()
    {
        int staticFunctionCount = 0;
        int staticFieldCount = 0;

        foreach (PrototypeClass pClass in GetClasses())
        {
            // Resolving usings can be done as soon as all the prototype parsing is done
            ResolveUsings(pClass);
            
            ResolveBaseClass(pClass);

            foreach (PrototypeFunction pFunction in pClass.NativeFunctions)
            {
                pFunction.AssemblyLocation = staticFunctionCount++;
            }

            foreach (PrototypeField field in pClass.NativeFields)
            {
                if (field.IsStatic)
                {
                    field.AssemblyLocation = staticFieldCount++;
                }
            }
            
            if (pClass.Class == SealGlobal.Class)
            {
                GlobalClass = pClass;
            }
        }

        SealAssembly.Current = new SealAssembly(
            Name,
            new Function[staticFunctionCount],
            new Field[staticFieldCount]
        );
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

            foreach (PrototypeFunction pFunction in currentClass.NativeFunctions)
            {
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
            foreach (PrototypeField pField in baseClass.NativeFields)
            {
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

            foreach (PrototypeConstant pConstant in baseClass.NativeConstants)
            {
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
    
    private static void GenerateConstructor(PrototypeClass pClass)
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
                pConstructor
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

    private static void RegisterEntryPoint(UserFunction function)
    {
        if (!function.IsStatic
            || function.Name != EntryPointName)
        {
            return;
        }
        
        var assembly = SealAssembly.Current;

        if (assembly.EntryPoint != null)
        {
            throw new ParserException(function,
                $"Entry point has already been defined: {assembly.EntryPoint}.");
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
        
        assembly.EntryPoint = function;
    }

    private static Function GenerateFunction(PrototypeFunction pFunction)
    {
        switch (pFunction.Body)
        {
        case UserFunctionBody userFunctionBody:
            UserFunction userFunction = new UserFunctionParser(
                new TokenStream(userFunctionBody.Tokens),
                pFunction
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
    
    private static Expression ParseExpression(PrototypeClass pClass, ArraySegment<Token> tokens)
    {
        if (tokens.Count == 0)
            return null;
        
        return new ExpressionParser(
            new TokenStream(tokens),
            pClass,
            ExpressionParsingMode.Statement
        ).Parse();
    }

    private static void EvaluateStaticFields(Expression[] staticFieldExpressions)
    {
        Field[] staticFields = SealAssembly.Current.StaticFields;

        for (int i = 0; i < staticFields.Length; i++)
        {
            ref Field field = ref staticFields[i];
            
            Expression expression = staticFieldExpressions[i];
            
            field.Value = expression?.Evaluate(null) ?? SealClass.GetDefaultValue(field.Class);
        }
    }

    private void AssemblyGeneration()
    {
        SealAssembly assembly = SealAssembly.Current;

        var staticFieldExpressions = new Expression[assembly.StaticFields.Length];
        
        foreach (PrototypeClass pClass in GetClasses())
        {
            BuildClass(pClass);
            
            SealClass sClass = pClass.Class;
            
            GenerateConstructor(pClass);

            foreach ((_, PrototypeFunction pFunction) in pClass.Functions)
            {
                assembly.StaticFunctions[pFunction.AssemblyLocation] = GenerateFunction(pFunction);
            }
            
            var instanceFields = new FieldDefinition[sClass.FieldTable.Count];
            
            foreach ((_, PrototypeField pField) in pClass.Fields)
            {
                SealClass fieldClass = pField.Class.ResolveDataTypeSealClass(pField.DataType);
                Expression expression = ParseExpression(pClass, pField.Tokens);

                int location = pField.AssemblyLocation;
                
                if (pField.IsStatic)
                {
                    staticFieldExpressions[location] = expression;
                    
                    assembly.StaticFields[location] = new Field(
                        fieldClass,
                        pField.IsConst,
                        SealValue.Nil
                    );
                }
                else
                {
                    instanceFields[location] = new FieldDefinition(
                        fieldClass,
                        pField.IsConst,
                        expression
                    );
                }
            }
            
            sClass.InstanceFields = instanceFields;
        }

        EvaluateStaticFields(staticFieldExpressions);
    }
}