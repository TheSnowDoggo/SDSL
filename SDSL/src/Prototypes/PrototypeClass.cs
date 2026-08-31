namespace SDSL.Prototypes;

public class PrototypeClass
{
    public PrototypeClass(
        PrototypeNamespace pNamespace,
        SealClass sClass)
    {
        Namespace = pNamespace;
        Class = sClass;
    }
    
    public PrototypeNamespace Namespace { get; }
    public SealClass Class { get; }
    
    public string Name => Class.Name;
    
    public string[] UsingsNames { get; init; } = [];
    public bool NoTerminators { get; init; }

    public PrototypeAssembly Assembly => Namespace.Assembly;
    
    public PrototypeNamespace[] Usings { get; set; }
    
    public PrototypeFunction Constructor { get; set; }

    public Dictionary<string, PrototypeFunction> Functions { get; } = [];
    public Dictionary<string, PrototypeField> Fields { get; } = [];
    public Dictionary<string, PrototypeConstant> Constants { get; } = [];

    public bool HasMember(string name)
    {
        return Functions.ContainsKey(name)
               || Fields.ContainsKey(name)
               || Constants.ContainsKey(name);
    }

    public PrototypeClass ResolveFullClass(
        SourceLocation error,
        string namespaceName,
        string className)
    {
        if (!Namespace.Assembly.Namespaces.TryGetValue(namespaceName, out PrototypeNamespace otherNamespace))
        {
            throw new ParserException(error,
                $"Namespace '{namespaceName}' not found.");
        }

        if (!otherNamespace.Classes.TryGetValue(className, out PrototypeClass otherClass))
        {
            throw new ParserException(error,
                $"Class '{className}' not found in namespace '{namespaceName}'.");
        }

        return otherClass;
    }
    
    public PrototypeClass ResolveImplicitClass(SourceLocation error, string className)
    {
        List<PrototypeClass> classes = GetMatchingImplicitClasses(error, className);

        return classes.Count switch
        {
            1 => classes[0],
            0 => throw new ParserException(error,
                $"Failed to resolve class {className}."),
            _ => throw new ParserException(error,
                $"Class {className} is ambigious between: [{string.Join(", ", classes)}].")
        };
    }

    public SealClass ResolveDataTypeClass(PrototypeDataType dataType)
    {
        if (dataType.Namespace == null
            && dataType.Name == "Any")
        {
            return null;
        }
        
        return ResolveSealClass(
            dataType.Location,
            dataType.Name,
            dataType.Namespace
        );
    }
    
    public SealClass ResolveSealClass(
        SourceLocation error,
        string name,
        string namespaceName = null)
    {
        if (namespaceName == null)
        {
            return ResolveImplicitClass(
                error,
                name
            ).Class;
        }
        else
        {
            return ResolveFullClass(
                error,
                namespaceName,
                name
            ).Class;
        }
    }
    
    public bool TryResolveImplicitClass(SourceLocation error,
        string className,
        out PrototypeClass prototypeClass)
    {
        List<PrototypeClass> classes = GetMatchingImplicitClasses(error, className);

        switch (classes.Count)
        {
        case 0:
            prototypeClass = null;
            return false;
        case 1:
            prototypeClass = classes[0];
            return true;
        default:
            throw new ParserException(error,
                $"Class {className} is ambigious between: [{string.Join(", ", classes)}].");
        }
    }
    
    private List<PrototypeClass> GetMatchingImplicitClasses(SourceLocation error, string className)
    {
        var classes = new List<PrototypeClass>();

        for (int i = 0; i < Usings.Length; i++)
        {
            PrototypeNamespace pNamespace = Usings[i];
            
            if (pNamespace.Classes.TryGetValue(className, out PrototypeClass otherClass))
                classes.Add(otherClass);
        }

        return classes;
    }
    
    public override string ToString()
    {
        return $"Class<{Namespace.Name}::{Name}>";
    }
}