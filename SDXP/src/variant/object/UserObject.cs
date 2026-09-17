using SDSL.Native;

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
		if (!ObjectClass.FunctionMap.TryGetValue("to_string", out Function function)
		    || function.IsStatic
		    || function.Signature.MinArgs != 0
		    || !StringClass.Class.IsAssignableTo(function.Signature.ReturnType))
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

	public override bool ToBoolVolatile()
	{
		if (!ObjectClass.FunctionMap.TryGetValue("to_bool", out Function function)
		    || function.IsStatic
		    || function.Signature.MinArgs != 0
		    || !BoolClass.Class.IsAssignableTo(function.Signature.ReturnType))
		{
			return true;
		}

		try
		{
			return function.MemberInvoke(this).AsBool();
		}
		catch (Exception ex)
		{
			throw new RuntimeException(
				$"{ToString()}.to_bool()\n  --> {ex.Message}", ex);
		}
	}
}