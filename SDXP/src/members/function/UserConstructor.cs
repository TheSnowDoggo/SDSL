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

	public override VariantClass DeclaredClass => _userVariantClass;
	
	protected override Variant Invoke(Variant self, Variant[] args)
	{
		VariantObject compositeBase = CreateCompositeBase();
		
		var instance = new UserVariantObject(_userVariantClass, compositeBase);
		
		CreateFields(instance);
		
		_userFunction?.MemberInvoke(instance, args);
		
		return instance;
	}

	private VariantObject CreateCompositeBase()
	{
		NativeVariantClass compositeClass = _userVariantClass.CompositeClass;
		
		if (compositeClass == null)
		{
			return null;
		}

		Variant value;
		
		try
		{
			value = compositeClass.Constructor.StaticInvoke();
		}
		catch (Exception ex)
		{
			throw new RuntimeException(
				$"Failed to instantiate composite class {compositeClass}.\n  --> {ex.Message}", ex);
		}

		if (!value.IsAssignableTo(compositeClass))
		{
			throw new RuntimeException(_userFunction,
				$"Expected composite class object to be of type {compositeClass}, got {value.Class}.");
		}

		return value.AsVariantObject();
	}

	private void CreateFields(UserVariantObject userVariantObject)
	{
		UserInstanceProperty[] instanceProperties = _userVariantClass.InstanceFields;
		
		int length = instanceProperties.Length;

		if (length == 0)
		{
			userVariantObject.Fields = [];
			return;
		}
		
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

		userVariantObject.Fields = fields;
	}
}