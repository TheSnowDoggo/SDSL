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
        
        GenerateFunctionsAndInstanceFields();
        GenerateStaticFields();
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

    private void AllocateAssembly()
    {
        int staticFunctionCount = 0;
        int staticFieldCount = 0;

        foreach (PrototypeClass pClass in GetClasses())
        {
            // Resolving usings can be done as soon as all the prototype parsing is done
            ResolveUsings(pClass);
            
            // Only Static functions are allocated an assembly location
            foreach ((_, PrototypeFunction function) in pClass.Functions)
            {
                if (function.IsStatic)
                {
                    function.AssemblyLocation = staticFunctionCount++;
                }
            }
            
            var fieldLookupTable = new Dictionary<string, int>();
                
            // Static fields get allocated an assembly location
            // Instance fields are added to the field lookup table
            foreach ((string fieldName, PrototypeField field) in pClass.Fields)
            {
                if (field.IsStatic)
                {
                    field.AssemblyLocation = staticFieldCount++;
                }
                else
                {
                    int location = fieldLookupTable.Count;
                    
                    fieldLookupTable.Add(fieldName, location);

                    field.AssemblyLocation = location;
                }
            }

            SealClass sClass = pClass.Class;
            
            sClass.FieldTable = fieldLookupTable.ToFrozenDictionary();

            if (sClass == SealGlobal.Class)
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
    
    private void ResolveUsings(PrototypeClass pClass)
    {
        var namespaces = new HashSet<PrototypeNamespace>();
        
        namespaces.Add(pClass.Namespace);
        
        for (int i = 0; i < pClass.UsingsNames.Length; i++)
        {
            string usingName = pClass.UsingsNames[i];
            
            if (!Namespaces.TryGetValue(usingName, out PrototypeNamespace pNamespace))
                throw new InvalidOperationException(
                    $"{ToString()} Failed to resolve namespace {usingName}.");
            
            namespaces.Add(pNamespace);
        }
        
        pClass.Usings = namespaces.ToArray();
    }

    private void GenerateFunctionsAndInstanceFields()
    {
        SealAssembly assembly = SealAssembly.Current;
        
        foreach (PrototypeClass pClass in GetClasses())
        {
            SealClass sClass = pClass.Class;
            
            GenerateConstructor(pClass);

            var functionTable = new Dictionary<string, Function>();
            
            foreach ((_, PrototypeFunction pFunction) in pClass.Functions)
            {
                Function function = GenerateFunction(pFunction);
                
                if (pFunction.IsStatic)
                {
                    assembly.Functions[pFunction.AssemblyLocation] = function;
                }
                else
                {
                    functionTable.Add(pFunction.Name, function);
                }
            }
            
            sClass.FunctionTable = functionTable.ToFrozenDictionary();
            
            var instanceFields = new InstanceField[sClass.FieldTable.Count];
            
            foreach ((_, PrototypeField pField) in pClass.Fields)
            {
                if (pField.IsStatic)
                    continue;
                
                SealClass fieldClass = pField.Class.ResolveDataTypeClass(pField.DataType);
                Expression expression = ParseFieldExpression(pField);
                
                instanceFields[pField.AssemblyLocation] = new InstanceField(
                    fieldClass,
                    pField.IsConst,
                    expression
                );
            }
            
            sClass.InstanceFields = instanceFields;
        }
    }

    private static void GenerateConstructor(PrototypeClass pClass)
    {
        SealClass sClass = pClass.Class;
        PrototypeFunction pConstructor = pClass.Constructor;
        
        if (pConstructor == null)
        {
            if (sClass.IsNative
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
    
    private void GenerateStaticFields()
    {
        foreach (PrototypeClass pClass in GetClasses())
        {
            foreach ((_, PrototypeField pField) in pClass.Fields)
            {
                if (!pField.IsStatic)
                    continue;
                
                SealClass fieldClass = pField.Class.ResolveDataTypeClass(pField.DataType);

                Expression expression = ParseFieldExpression(pField);
                
                SealValue defaultValue = expression == null
                    ? SealClass.GetDefaultValue(fieldClass)
                    : expression.Evaluate(null);

                SealAssembly.Current.Fields[pField.AssemblyLocation] = new Field(
                    fieldClass,
                    pField.IsConst,
                    defaultValue
                );
            }
        }

    }
    
    private static Expression ParseFieldExpression(PrototypeField pField)
    {
        if (pField.Tokens.Count == 0)
            return null;
        
        return new ExpressionParser(
            new TokenStream(pField.Tokens),
            ExpressionParsingMode.Statement,
            pField.Class
        ).Parse();
    }
}