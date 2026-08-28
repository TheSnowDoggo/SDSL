namespace SDSL.Prototypes;

[AttributeUsage(AttributeTargets.Class)]
public class ClassExportAttribute : Attribute
{
    public ClassExportAttribute(
        string pNamespace,
        string name)
    {
        Namespace = pNamespace;
        Name = name;
    }
    
    public string Namespace { get; }
    public string Name { get; }
}