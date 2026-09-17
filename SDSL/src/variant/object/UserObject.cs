namespace SDSL;

/// <summary>
/// Represents a user defined <see cref="VariantType.Object"/> object.
/// </summary>
public class UserObject : VariantObject
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UserObject"/> class from an associated <see cref="VariantClass"/>.
	/// </summary>
	/// <param name="objectClass">The class of the object.</param>
	public UserObject(VariantClass objectClass)
	{
		ObjectClass = objectClass;
	}
	
	public override VariantClass ObjectClass { get; }
	
	/// <summary>
	/// Gets or sets the user-defined fields stored in the current object.
	/// </summary>
	public Variant[] Fields { get; set; }
	
	/// <summary>
	/// Gets or sets the composite base of the object.
	/// </summary>
	/// <remarks>
	/// Composite bases are used when user-defined classes inherit from a native constructable class.
	/// </remarks>
	public VariantObject CompositeBase { get; set; }

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