namespace SDSL;

[AttributeUsage(AttributeTargets.Method)]
public class GetterFunctionExportAttribute : Attribute
{
	public GetterFunctionExportAttribute(string valueType = null)
	{
		ValueType = valueType;
	}
	
	public string ValueType { get; }
	
	public string Name { get; init; }
}