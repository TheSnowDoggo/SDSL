namespace SDSL.Prototypes;

[AttributeUsage(AttributeTargets.Method)]
public class SealFunctionExportAttribute : Attribute
{
	public SealFunctionExportAttribute(params string[] types)
	{
		Types = types;
		MinArgs = types.Length;
		MaxArgs = types.Length;
	}

	public string[] Types { get; }
	public int MinArgs { get; init; }
	public int MaxArgs { get; init; }
	public string Name { get; init; }
}