using System.Collections.Frozen;

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
    public string FullName => $"{Namespace.Name}:{Class.Name}";
    
    public string[] UsingsNames { get; init; } = [];
    public bool NoTerminators { get; init; }
    public PrototypeDataType BaseClassDataType { get; init; }

    public PrototypeAssembly Assembly => Namespace.Assembly;
    
    public PrototypeNamespace[] Usings { get; set; }
    
    public PrototypeFunction Constructor { get; set; }
    
    public PrototypeFunction[] NativeFunctions { get; set; } = [];
    public PrototypeField[] NativeFields { get; set; } = [];
    public PrototypeConstant[] NativeConstants { get; set; } = [];
    
    public PrototypeClass BaseClass { get; set; }
    
    public FrozenDictionary<string, PrototypeFunction> Functions { get; set; }
    public FrozenDictionary<string, PrototypeField> Fields { get; set; }
    public FrozenDictionary<string, PrototypeConstant> Constants { get; set; }
    
    public PrototypeClass ResolveFullClass(
        SourceLocation error,
        string namespaceName,
        string className)
    {
        if (!Assembly.Namespaces.TryGetValue(namespaceName, out PrototypeNamespace otherNamespace))
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

    public PrototypeClass ResolveDataTypeClass(PrototypeDataType dataType)
    {
        if (dataType.Namespace == null
            && dataType.Name == "Any")
        {
            return null;
        }
        
        return ResolveClass(
            dataType.Location,
            dataType.Name,
            dataType.Namespace
        );
    }
    
    public SealClass ResolveDataTypeSealClass(PrototypeDataType dataType)
    {
        return ResolveDataTypeClass(dataType)?.Class;
    }
    
    public PrototypeClass ResolveClass(
        SourceLocation error,
        string name,
        string namespaceName = null)
    {
        if (namespaceName == null)
        {
            return ResolveImplicitClass(
                error,
                name
            );
        }
        else
        {
            return ResolveFullClass(
                error,
                namespaceName,
                name
            );
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