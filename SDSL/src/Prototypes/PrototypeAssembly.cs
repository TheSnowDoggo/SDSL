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
                foreach (PrototypeFunction function in @class.Functions.Values)
                    if (function.IsStatic)
                        function.AssemblyLocation = staticFunctionCount++;
                
                foreach (PrototypeField field in @class.Fields.Values)
                    if (field.IsStatic)
                        field.AssemblyLocation = staticFieldCount++;
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