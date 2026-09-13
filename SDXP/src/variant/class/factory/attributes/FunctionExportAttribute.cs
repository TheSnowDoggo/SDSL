namespace SDSL;

[AttributeUsage(AttributeTargets.Method)]
public class FunctionExportAttribute : Attribute
{
	public FunctionExportAttribute(params string[] argumentTypes)
	{
		ArgumentTypes = argumentTypes;
	}
	
	public string[] ArgumentTypes { get; }
	
	public int MinArgs { get; init; }
	public int MaxArgs { get; init; }
	
	public string Name { get; init; }
}