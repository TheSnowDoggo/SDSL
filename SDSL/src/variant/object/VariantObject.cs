namespace SDSL;

/// <summary>
/// Represents a <see cref="VariantType.Object"/> object.
/// </summary>
public abstract class VariantObject
{
	/// <summary>
	/// Gets the associated <see cref="VariantClass"/> of this object.
	/// </summary>
	public abstract VariantClass ObjectClass { get; }

	public override string ToString()
	{
		return $"Object<{ObjectClass.Name}>";
	}

	/// <summary>
	/// Returns a user-defined string representation of the current object
	/// </summary>
	/// <returns>A user-defined string that represents the current object.</returns>
	public virtual string ToStringVolatile()
	{
		return ToString();
	}

	/// <summary>
	/// Converts the variant to a boolean value using user-defined conversions.
	/// </summary>
	/// <returns>The converted boolean value.</returns>
	public virtual bool ToBoolVolatile()
	{
		return true;
	}
	
	/// <summary>
	/// Indicates whether the current object is equal to another object using user-defiend equality.
	/// </summary>
	/// <param name="other">The other variant to compare to.</param>
	/// <returns><see langword="true"/> if the current object is equal to the other object; otherwise, <see langword="false"/>.</returns>
	public virtual bool EqualsVolatile(VariantObject other)
	{
		return this == other;
	}
}