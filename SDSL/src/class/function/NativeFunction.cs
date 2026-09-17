namespace SDSL;

public delegate Variant NativeFunctionInvoke(Variant self, Variant[] args);

public class NativeFunction : Function
{
	private readonly NativeFunctionInvoke _invoke;
	
	public NativeFunction(
		string name,
		VariantClass parentClass,
		bool isStatic,
		FunctionSignature signature,
		NativeFunctionInvoke invoke)
	{
		Name = name;
		LocalClass = parentClass;
		IsStatic = isStatic;
		Signature = signature;
		_invoke = invoke;
	}

	public override VariantClass LocalClass { get; }

	protected override Variant Invoke(Variant self, Variant[] args)
	{
		return _invoke(self, args);
	}
}