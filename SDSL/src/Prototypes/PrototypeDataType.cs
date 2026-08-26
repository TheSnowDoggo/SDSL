namespace SDSL.Prototypes;

public class PrototypeDataType
{
    public PrototypeDataType(
        SourceLocation location,
        string @namespace,
        string name)
    {
        Location = location;
        Namespace = @namespace;
        Name = name;
    }

    public SourceLocation Location { get; }
    public string Namespace { get; }
    public string Name { get; }

    public static readonly PrototypeDataType Any = new(SourceLocation.Empty, "global", "Any");
    
    public override string ToString()
    {
        return $"{Namespace ?? "?"}::{Name}";
    }
}