namespace SDSL;

public delegate Variant NativeFunctionInvoke(Variant self, Variant[] args);

public class NativeFunction : Function
{
	private readonly NativeFunctionInvoke _invoke;
	
	public NativeFunction(
		string name,
		VariantClass variantClass,
		bool isStatic,
		FunctionSignature signature,
		NativeFunctionInvoke invoke)
	{
		Name = name;
		VClass = variantClass;
		IsStatic = isStatic;
		Signature = signature;
		_invoke = invoke;
	}

	public override VariantClass VClass { get; }

	protected override Variant Invoke(Variant self, Variant[] args)
	{
		return _invoke(self, args);
	}
}