namespace SDSL;

public class FunctionSignature
{
	public FunctionSignature(
		FunctionArgument[] arguments,
		int minArgs,
		int maxArgs,
		string prototypeReturnType)
	{
		Arguments = arguments;
		MinArgs = minArgs;
		MaxArgs = maxArgs;
		PrototypeReturnType = prototypeReturnType;
	}

	public static FunctionSignature Empty { get; } = new FunctionSignature([], 0, 0, null);
	
	public FunctionArgument[] Arguments { get; }
	public int MinArgs { get; }
	public int MaxArgs { get; }
	
	public VariantClass ReturnType { get; set; }
	public string PrototypeReturnType { get; set; }
}