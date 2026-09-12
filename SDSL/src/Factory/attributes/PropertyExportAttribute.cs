namespace SDSL.Factory;

[AttributeUsage(AttributeTargets.Property)]
public class PropertyExportAttribute : Attribute
{
	public PropertyExportAttribute(string valueType)
	{
		ValueType = valueType;
	}
	
	public string ValueType { get; }
}