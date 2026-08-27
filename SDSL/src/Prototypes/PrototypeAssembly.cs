using System.Collections.Frozen;

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
        int staticFunctionCount = 0;
        int staticFieldCount = 0;
        
        foreach (PrototypeNamespace @namespace in Namespaces.Values)
        {
            foreach (PrototypeClass @class in @namespace.Classes.Values)
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
                
                @class.GenerateClass(
                    functionLookupTable.ToFrozenDictionary(),
                    fieldLookupTable.ToFrozenDictionary()
                );
            }
        }

        Assembly = new SealAssembly(
            Name,
            new Function[staticFunctionCount],
            new Variable[staticFieldCount]
        );
    }
    
    public override string ToString()
    {
        return $"Assembly<{Name}>";
    }
}