namespace SDSL;

public class UserConstructor : Function
{
	private readonly UserVariantClass _userVariantClass;
	
	public UserConstructor(
		UserVariantClass variantClass,
		Function function)
	{
		Name = "new";
		
		TypeClass = variantClass;
		_userVariantClass = variantClass;
		
		IsStatic = true;
		Signature = function?.Signature ?? FunctionSignature.Empty;
	}
	
	public override VariantClass TypeClass { get; }

	protected override Variant Invoke(Variant self, Variant[] args)
	{
		
		
		var instance = new UserVariantObject(TypeClass, fields);
	}

	private Variant CreateFields()
	{
		UserFieldInfo[] userFieldInfos = _userVariantClass.InstanceFields;
		
		int length = userFieldInfos.Length;
		
		Variant[] fields = new Variant[length];

		for (int i = 0; i < length; i++)
		{
			Variant defaultValue;
			
			
		}
	}
}