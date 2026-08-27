using System.Collections.Frozen;

namespace SDSL.Prototypes;

public class PrototypeClass
{
    private PrototypeNamespace[] _resolvedUsings;
    
    public PrototypeClass(
        PrototypeNamespace @namespace,
        string name,
        string[] usings)
    {
        Namespace = @namespace;
        Name = name;
        Usings = usings;
    }
    
    public PrototypeNamespace Namespace { get; }
    public string Name { get; }
    public string[] Usings { get; }

    public PrototypeAssembly Assembly => Namespace.Assembly;
    
    public PrototypeConstructor Constructor { get; set; }

    public Dictionary<string, PrototypeFunction> Functions { get; } = [];
    public Dictionary<string, PrototypeField> Fields { get; } = [];
    
    public SealClass Class { get; private set; }

    public void GenerateClass(
        FrozenDictionary<string, int> functionLookupTable,
        FrozenDictionary<string, int> fieldLookupTable)
    {
        Class = new SealClass(Namespace.Name, Name)
        {
            FunctionTable = functionLookupTable,
            FieldTable = fieldLookupTable,
        };
    }
    
    public PrototypeClass ResolveFullClass(
        SourceLocation error,
        string namespaceName,
        string className)
    {
        if (!Namespace.Assembly.Namespaces.TryGetValue(namespaceName, out PrototypeNamespace otherNamespace))
        {
            throw new LangException(error,
                $"Namespace {namespaceName} not found.");
        }

        if (!otherNamespace.Classes.TryGetValue(className, out PrototypeClass otherClass))
        {
            throw new LangException(error,
                $"Class {className} not found in namespace {namespaceName}.");
        }

        return otherClass;
    }
    
    public PrototypeClass ResolveImplicitClass(SourceLocation error, string className)
    {
        List<PrototypeClass> classes = GetMatchingImplicitClasses(error, className);

        return classes.Count switch
        {
            1 => classes[0],
            0 => throw new LangException(error,
                $"Failed to resolve class {className}."),
            _ => throw new LangException(error,
                $"Class {className} is ambigious between: [{string.Join(", ", classes)}].")
        };
    }

    public SealClass ResolveDataTypeClass(PrototypeDataType dataType)
    {
        if (dataType == PrototypeDataType.Any)
        {
            return null;
        }
        
        if (dataType.Namespace == null)
        {
            return ResolveImplicitClass(
                dataType.Location,
                dataType.Name
            ).Class;
        }
        else
        {
            return ResolveFullClass(
                dataType.Location,
                dataType.Namespace,
                dataType.Name
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
            throw new LangException(error,
                $"Class {className} is ambigious between: [{string.Join(", ", classes)}].");
        }
    }
    
    private List<PrototypeClass> GetMatchingImplicitClasses(SourceLocation error, string className)
    {
        if (_resolvedUsings == null)
        {
            ResolveUsings(error);
        }

        var classes = new List<PrototypeClass>();

        for (int i = 0; i < _resolvedUsings.Length; i++)
        {
            PrototypeNamespace @namespace = _resolvedUsings[i];
            
            if (@namespace.Classes.TryGetValue(className, out PrototypeClass otherClass))
            {
                classes.Add(otherClass);
            }
        }

        return classes;
    }
    
    public override string ToString()
    {
        return $"Class<{Namespace.Name}::{Name}>";
    }
    
    private void ResolveUsings(SourceLocation error)
    {
        PrototypeAssembly assembly = Namespace.Assembly;

        var namespaces = new HashSet<PrototypeNamespace>();
        
        namespaces.Add(Namespace);
        
        for (int i = 0; i < Usings.Length; i++)
        {
            string usingName = Usings[i];
            
            if (!assembly.Namespaces.TryGetValue(usingName, out PrototypeNamespace @namespace))
            {
                throw new LangException(error,
                    $"{ToString()} Failed to resolve namespace {usingName}.");
            }
            
            namespaces.Add(@namespace);
        }
        
        _resolvedUsings = namespaces.ToArray();
    }
}