namespace SDSL.Factory;

[AttributeUsage(AttributeTargets.Field)]
public class ConstantExportAttribute : Attribute
{
    public string Name { get; init; }
}