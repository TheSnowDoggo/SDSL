namespace SDSL;

public class UserObject : VariantObject
{
	public UserObject(VariantClass parentClass)
	{
		ObjectClass = parentClass;
	}
	
	public override VariantClass ObjectClass { get; }
	
	public VariantObject CompositeBase { get; set; }
	
	public Variant[] Fields { get; set; }

	public override string ToStringVolatile()
	{
		if (!ObjectClass.FunctionMap.TryGetValue("to_string", out Function function))
		{
			return base.ToStringVolatile();
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

	public override bool ToBoolVolatile()
	{
		if (!ObjectClass.FunctionMap.TryGetValue("to_bool", out Function function))
		{
			return base.ToBoolVolatile();
		}

		try
		{
			return function.MemberInvoke(this).ToBool(false);
		}
		catch (Exception ex)
		{
			throw new RuntimeException(
				$"{ToString()}.to_bool()\n  --> {ex.Message}", ex);
		}
	}

	public override bool EqualsVolatile(VariantObject other)
	{
		if (!ObjectClass.FunctionMap.TryGetValue("equals", out Function function))
		{
			return base.EqualsVolatile(other);
		}

		try
		{
			return function.MemberInvoke(this, other).ToBool(false);
		}
		catch (Exception ex)
		{
			throw new RuntimeException(
				$"{ToString()}.to_bool()\n  --> {ex.Message}", ex);
		}
	}
}