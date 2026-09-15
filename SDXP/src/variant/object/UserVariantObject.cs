namespace SDSL;

public class UserVariantObject : VariantObject
{
	public UserVariantObject(
		VariantClass variantClass,
		VariantObject compoundBase)
	{
		ParentClass = variantClass;
		CompoundBase = compoundBase;
	}
	
	public override VariantClass ParentClass { get; }
	
	public VariantObject CompoundBase { get; }
	
	public Variant[] Fields { get; set; }

	public override string ToString()
	{
		return CompoundBase?.ToString() ?? base.ToString();
		
		if (!ParentClass.FunctionMap.TryGetValue("to_string", out Function function)
		    || function.IsStatic
		    || function.Signature.MinArgs != 0)
		{
			return base.ToString();
		}

		try
		{
			return function.MemberInvoke(this).ToString();
		}
		catch (Exception ex)
		{
			throw new RuntimeException(
				$"{ToSafeString()}.to_string()\n  --> {ex.Message}", ex);
		}
	}
}