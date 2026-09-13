namespace SDSL;

public class FunctionSignature
{
	public FunctionSignature(
		FunctionArgument[] arguments,
		int minArgs,
		int maxArgs,
		VariantClass returnType)
	{
		Arguments = arguments;
		MinArgs = minArgs;
		MaxArgs = maxArgs;
		ReturnType = returnType;
	}
	
	public FunctionArgument[] Arguments { get; }
	public int MinArgs { get; }
	public int MaxArgs { get; }
	public VariantClass ReturnType { get; }
}