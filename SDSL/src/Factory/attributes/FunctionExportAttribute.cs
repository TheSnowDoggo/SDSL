namespace SDSL.Factory;

[AttributeUsage(AttributeTargets.Method)]
public class FunctionExportAttribute : Attribute
{
	public FunctionExportAttribute(params string[] parameterTypes)
	{
		ParameterTypes = parameterTypes;
		MinArgs = parameterTypes.Length;
		MaxArgs = parameterTypes.Length;
	}

	public string[] ParameterTypes { get; }
	public int MinArgs { get; init; }
	public int MaxArgs { get; init; }
	public string Name { get; init; }
	public string[] ParametersNames { get; init; }
}