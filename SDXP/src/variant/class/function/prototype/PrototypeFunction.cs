namespace SDSL;

public class PrototypeFunction : Function
{
	public PrototypeFunction(
		string name,
		VariantClass variantClass,
		bool isStatic,
		ArraySegment<Token> tokens)
	{
		Name = name;
		Class = variantClass;
		IsStatic = isStatic;
		Signature = FunctionSignature.Empty;
		Tokens = tokens;
	}
	
	public override VariantClass Class { get; }
	
	public ArraySegment<Token> Tokens { get; }
	
	protected override Variant Invoke(Variant self, Variant[] args)
	{
		throw new InvalidOperationException("Cannot invoke prototype function.");
	}
}