namespace SDSL;

[AttributeUsage(AttributeTargets.Method)]
public class FunctionInfoAttribute : Attribute
{
	public FunctionInfoAttribute(params string[] argumentNames)
	{
		ArgumentNames = argumentNames;
	}
	
	public string[] ArgumentNames { get; }
}