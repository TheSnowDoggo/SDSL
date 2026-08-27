using System.Collections.Frozen;
using SDSL.Expressions;

namespace SDSL.Prototypes;

public class PrototypeAssembly
{
    public PrototypeAssembly(string name)
    {
        Name = name;
    }
    
    public string Name { get; }

    public Dictionary<string, PrototypeNamespace> Namespaces { get; } = [];
    
    public SealAssembly Assembly { get; private set; }

    public HashSet<string> GlobalUsings { get; } = [];
    
    public PrototypeNamespace GetOrCreateNamespace(string name)
    {
        if (Namespaces.TryGetValue(name, out PrototypeNamespace @namespace))
        {
            return @namespace;
        }

        @namespace = new PrototypeNamespace(this, name);
        
        Namespaces.Add(name, @namespace);

        return @namespace;
    }

    public void GenerateAssembly()
    {
        (Function[] functions, Variable[] fields) = GlobalAllocate();
        
        var assembly = new SealAssembly(
            Name,
            functions,
            fields
        );
        
        foreach (PrototypeClass @class in GetClasses())
        {
            foreach ((_, PrototypeFunction prototypeFunction) in @class.Functions)
            {
                Function function;
                
                switch (prototypeFunction)
                {
                case UserPrototypeFunction userPrototypeFunction:
                    function = new UserFunctionParser(assembly, userPrototypeFunction)
                        .Parse();
                    break;
                case NativePrototypeFunction nativePrototypeFunction:
                    function = nativePrototypeFunction.GenerateFunction(assembly);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Prototype function is of unknown type: {prototypeFunction.GetType()}.");
                }

                functions[prototypeFunction.AssemblyLocation] = function;
            }
        }

        foreach (PrototypeClass @class in GetClasses())
        {
            foreach ((_, PrototypeField prototypeField) in @class.Fields)
            {
                if (!prototypeField.IsStatic)
                    continue;

                SealClass sealClass = @class.ResolveDataTypeClass(prototypeField.DataType);

                SealValue defaultValue;
                
                if (prototypeField.Tokens.Count == 0)
                {
                    defaultValue = sealClass?.GetDefaultValue() ?? SealValue.Nil;
                }
                else
                {
                    var stream = new TokenStream(prototypeField.Tokens);
                
                    Expression expression = new ExpressionParser(
                        stream,
                        ExpressionParsingMode.Statement,
                        @class
                    ).Parse();

                    defaultValue = expression.Evaluate(assembly, null);
                }

                fields[prototypeField.AssemblyLocation] = new Variable(
                    sealClass,
                    prototypeField.IsConst,
                    defaultValue
                );
            }
        }

        Assembly = assembly;
    }

    public IEnumerable<PrototypeClass> GetClasses()
    {
        foreach ((_, PrototypeNamespace @namespace) in Namespaces)
        {
            foreach ((_, PrototypeClass @class) in @namespace.Classes)
            {
                yield return @class;
            }
        }
    }
    
    public override string ToString()
    {
        return $"Assembly<{Name}>";
    }

    private (Function[], Variable[]) GlobalAllocate()
    {
        int staticFunctionCount = 0;
        int staticFieldCount = 0;

        foreach (PrototypeClass @class in GetClasses())
        {
            var functionLookupTable = new Dictionary<string, int>();
                
            // Both Static and Instance functions must be allocated
            foreach ((string functionName, PrototypeFunction function) in @class.Functions)
            {
                if (!function.IsStatic)
                    functionLookupTable.Add(functionName, staticFunctionCount);
                function.AssemblyLocation = staticFunctionCount++;
            }
                
            var fieldLookupTable = new Dictionary<string, int>();
                
            // Only Static fields are allocated an assembly location
            foreach ((string fieldName, PrototypeField field) in @class.Fields)
            {
                if (field.IsStatic)
                {
                    field.AssemblyLocation = staticFieldCount++;
                }
                else
                {
                    int location = fieldLookupTable.Count;
                    fieldLookupTable.Add(fieldName, location);
                }
            }

            SealClass sealClass = @class.Class;

            sealClass.FunctionTable = functionLookupTable.ToFrozenDictionary();
            sealClass.FieldTable = fieldLookupTable.ToFrozenDictionary();
        }

        return (new Function[staticFunctionCount], new Variable[staticFieldCount]);
    }
}