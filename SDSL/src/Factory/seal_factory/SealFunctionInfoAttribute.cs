namespace SDSL.Factory;

[AttributeUsage(AttributeTargets.Method)]
public class SealFunctionInfoAttribute : Attribute
{
	public SealFunctionInfoAttribute(params string[] parameterNames)
	{
		ParameterNames = parameterNames;
	}
	
	public string[] ParameterNames { get; }
	public string ReturnType { get; init; }
	public string Description { get; init; }
}