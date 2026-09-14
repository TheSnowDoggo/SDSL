namespace SDSL;

public class UserConstructor : Function
{
	private readonly UserVariantClass _userVariantClass;
	private readonly UserFunction _userFunction;
	
	public UserConstructor(
		UserVariantClass variantClass,
		UserFunction userFunction)
	{
		Name = "new";
		
		_userVariantClass = variantClass;
		
		IsStatic = true;
		Signature = userFunction?.Signature ?? FunctionSignature.Empty;

		_userFunction = userFunction;
	}

	public override VariantClass TypeClass => _userVariantClass;

	protected override Variant Invoke(Variant self, Variant[] args)
	{
		Variant[] fields = CreateFields();
		
		var instance = new UserVariantObject(_userVariantClass, fields);
		
		_userFunction?.MemberInvoke(instance, fields);
		
		return instance;
	}

	private Variant[] CreateFields()
	{
		UserInstanceProperty[] instanceProperties = _userVariantClass.InstanceFields;
		
		int length = instanceProperties.Length;
		
		Variant[] fields = new Variant[length];

		for (int i = 0; i < length; i++)
		{
			UserInstanceProperty property = instanceProperties[i];

			try
			{
				fields[i] = property.Expression?.Evaluate(null)
				    ?? VariantClass.GetDefaultValue(property.ValueClass);
			}
			catch (Exception ex)
			{
				throw new RuntimeException(_userFunction, 
					$"Failed to initialize field {property.FullName}.\n  --> {ex.Message}", ex);
			}
		}

		return fields;
	}
}