namespace SDSL;

public class UserVariantObject : VariantObject
{
	public UserVariantObject(
		VariantClass variantClass)
	{
		ParentClass = variantClass;
	}
	
	public override VariantClass ParentClass { get; }
	
	public VariantObject CompositeBase { get; set; }
	
	public Variant[] Fields { get; set; }

	public override string ToUnsafeString()
	{
		if (!ParentClass.FunctionMap.TryGetValue("to_string", out Function function)
		    || function.IsStatic
		    || function.Signature.MinArgs != 0)
		{
			return base.ToString();
		}

		try
		{
			return function.MemberInvoke(this).AsString();
		}
		catch (Exception ex)
		{
			throw new RuntimeException(
				$"{ToString()}.to_string()\n  --> {ex.Message}", ex);
		}
	}
}