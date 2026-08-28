namespace SDSL.Prototypes;

[AttributeUsage(AttributeTargets.Field)]
public class ConstantExportAttribute : Attribute
{
    public ConstantExportAttribute(string name = null)
    {
        Name = name;
    }
    
    public string Name { get; }
}