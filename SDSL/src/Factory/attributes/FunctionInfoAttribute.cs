namespace SDSL.Factory;

[AttributeUsage(AttributeTargets.Method)]
public class FunctionInfoAttribute : Attribute
{
	public FunctionInfoAttribute(params string[] parameterNames)
	{
		ParameterNames = parameterNames;
	}
	
	public string[] ParameterNames { get; }
	public string ReturnType { get; init; }
	public string Description { get; init; }
}