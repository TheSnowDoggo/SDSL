namespace SDSL;

[AttributeUsage(AttributeTargets.Property)]
public class PropertyExportAttribute : Attribute
{
	public PropertyExportAttribute(string valueType = null)
	{
		ValueType = valueType;
	}
	
	public string ValueType { get; }
	
	public string Name { get; init; }
}