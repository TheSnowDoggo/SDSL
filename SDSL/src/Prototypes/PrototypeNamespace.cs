namespace SDSL.Prototypes;

public class PrototypeNamespace
{
    public PrototypeNamespace(
        PrototypeAssembly assembly,
        string name)
    {
        Assembly = assembly;
        Name = name;
    }
    
    public PrototypeAssembly Assembly { get; }
    public string Name { get; }

    public Dictionary<string, PrototypeClass> Classes { get; } = [];
    
    public override string ToString()
    {
        return $"Namespace<{Name}>";
    }
}