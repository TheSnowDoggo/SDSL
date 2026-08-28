using System.Collections.Frozen;
using SDSL.Expressions;

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
    
    public SealAssembly GenerateAssembly()
    {
        SealAssembly assembly = AllocateAssembly();
        
        GenerateFunctions(assembly);
        GenerateInstanceFields(assembly);
        GenerateStaticFields(assembly);

        return assembly;
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

    private SealAssembly AllocateAssembly()
    {
        int staticFunctionCount = 0;
        int staticFieldCount = 0;

        foreach (PrototypeClass pClass in GetClasses())
        {
            var functionLookupTable = new Dictionary<string, int>();
                
            // Both Static and Instance functions must be allocated
            foreach ((string functionName, PrototypeFunction function) in pClass.Functions)
            {
                if (!function.IsStatic)
                {
                    functionLookupTable.Add(functionName, staticFunctionCount);
                }
                
                function.AssemblyLocation = staticFunctionCount++;
            }
                
            var fieldLookupTable = new Dictionary<string, int>();
                
            // Only Static fields are allocated an assembly location
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

            sClass.FunctionTable = functionLookupTable.ToFrozenDictionary();
            sClass.FieldTable = fieldLookupTable.ToFrozenDictionary();
        }

        return new SealAssembly(
            Name,
            new Function[staticFunctionCount],
            new Variable[staticFieldCount]
        );
    }

    private void GenerateFunctions(SealAssembly assembly)
    {
        foreach (PrototypeClass pClass in GetClasses())
        {
            GenerateConstructor(assembly, pClass);
            
            foreach ((_, PrototypeFunction pFunction) in pClass.Functions)
            {
                GenerateFunction(assembly, pFunction);
            }
        }
    }

    private static void GenerateConstructor(SealAssembly assembly, PrototypeClass pClass)
    {
        SealClass sClass = pClass.Class;
        
        PrototypeFunction pConstructor = pClass.Constructor;
        
        if (pConstructor == null)
        {
            if (sClass.ValueType != SealValueType.Object)
            {
                return;
            }

            sClass.Constructor = new UserConstructor(null)
            {
                Assembly = assembly,
                Class = sClass,
                Args = [],
                MinArgs = 0,
                MaxArgs = 0,
                ReturnType = sClass,
                IsStatic = true
            };
            
            return;
        }
        
        switch (pConstructor.Body)
        {
        case UserFunctionBody userFunctionBody:
            UserFunction userFunction = new UserFunctionParser(
                assembly,
                pConstructor,
                new TokenStream(userFunctionBody.Tokens)
            ).Parse();

            sClass.Constructor = new UserConstructor(userFunction)
            {
                Assembly = assembly,
                Class = sClass,
                Name = "new",
                Args = userFunction.Args,
                MinArgs = userFunction.MinArgs,
                MaxArgs = userFunction.MaxArgs,
                ReturnType = sClass,
                IsStatic = true 
                // The user function is non-static but the constructor itself is static
            };
            
            break;
        case NativeFunctionBody nativeFunctionBody:
            sClass.Constructor = NativeFunction.Create(
                assembly,
                pConstructor,
                nativeFunctionBody.Func
            );
            break;
        default:
            throw new InvalidOperationException(
                $"Prototype function body is unknown: {pConstructor.Body}.");
        }
    }

    private static void GenerateFunction(SealAssembly assembly, PrototypeFunction pFunction)
    {
        Function function;
                
        switch (pFunction.Body)
        {
        case UserFunctionBody userFunctionBody:
            UserFunction userFunction = new UserFunctionParser(
                assembly,
                pFunction,
                new TokenStream(userFunctionBody.Tokens)
            ).Parse();

            if (userFunction.IsStatic
                && userFunction.Name == EntryPointName)
            {
                if (assembly.EntryPoint != null)
                    throw new LangException(pFunction.Location,
                        $"User defined entry point has already been defined: {assembly.EntryPoint}.");
                assembly.EntryPoint = userFunction;
            }
                
            function = userFunction;
                
            break;
        case NativeFunctionBody nativeFunctionBody:
            function = NativeFunction.Create(
                assembly,
                pFunction,
                nativeFunctionBody.Func
            );
            break;
        default:
            throw new InvalidOperationException(
                $"Prototype function body is unknown: {pFunction.Body}.");
        }

        assembly.Functions[pFunction.AssemblyLocation] = function;
    }
    
    private void GenerateInstanceFields(SealAssembly assembly)
    {
        foreach (PrototypeClass pClass in GetClasses())
        {
            SealClass sClass = pClass.Class;

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
    
    private void GenerateStaticFields(SealAssembly assembly)
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
                    : expression.Evaluate(assembly, null);

                assembly.Fields[pField.AssemblyLocation] = new Variable(
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
        ).Parse(false);
    }
}