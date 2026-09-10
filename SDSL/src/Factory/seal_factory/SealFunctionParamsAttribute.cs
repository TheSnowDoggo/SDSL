namespace SDSL.Factory;

[AttributeUsage(AttributeTargets.Method)]
public class SealFunctionParamsAttribute : Attribute
{
	public SealFunctionParamsAttribute(params string[] names)
	{
		Names = names;
	}
	
	public string[] Names { get; }
}