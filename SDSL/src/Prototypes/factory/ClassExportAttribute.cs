namespace SDSL.Prototypes;

[AttributeUsage(AttributeTargets.Class)]
public class ClassExportAttribute : Attribute
{
    public ClassExportAttribute(
        string @namespace,
        string name)
    {
        Namespace = @namespace;
        Name = name;
    }
    
    public string Namespace { get; }
    public string Name { get; }
}