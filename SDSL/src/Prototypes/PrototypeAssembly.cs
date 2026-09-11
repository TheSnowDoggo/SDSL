namespace SDSL.Prototypes;

public class PrototypeAssembly
{
    public PrototypeAssembly(string name)
    {
        Name = name;
    }
    
    public string Name { get; }

    public Dictionary<string, PrototypeNamespace> Namespaces { get; } = [];
    public HashSet<string> GlobalUsings { get; init; } = [];
    
    public PrototypeClass GlobalClass { get; set; }
    
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

    public PrototypeClass CreateClass(SealClass sClass)
    {
        PrototypeNamespace pNamespace = GetOrCreateNamespace(sClass.Namespace);
        
        var pClass = new PrototypeClass(pNamespace, sClass);
        
        if (!pNamespace.Classes.TryAdd(pClass.Name, pClass))
        {
            throw new NativeFactoryException($"Namespace {pNamespace} already contains class with name '{sClass.Name}'.");
        }

        return pClass;
    }
    
    public override string ToString()
    {
        return $"Assembly<{Name}>";
    }
    
    
}