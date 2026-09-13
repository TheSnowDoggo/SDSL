namespace SDSL;

[AttributeUsage(AttributeTargets.Field)]
public class ConstantExportAttribute : Attribute
{
	public string Name { get; init; }
}