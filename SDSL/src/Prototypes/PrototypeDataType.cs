namespace SDSL.Prototypes;

public class PrototypeDataType
{
    public PrototypeDataType(
        SourceLocation location,
        string pNamespace,
        string name)
    {
        Location = location;
        Namespace = pNamespace;
        Name = name;
    }
    
    public static readonly PrototypeDataType Any = new(SourceLocation.Invalid, null, "Any");

    public SourceLocation Location { get; }
    public string Namespace { get; }
    public string Name { get; }
    
    public override string ToString()
    {
        return $"{Namespace ?? "?"}::{Name}";
    }
}