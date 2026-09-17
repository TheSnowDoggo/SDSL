namespace SDSL;

/// <summary>
/// Represents a <see cref="VariantType.Object"/> object.
/// </summary>
public abstract class VariantObject
{
	public abstract VariantClass ObjectClass { get; }

	public override string ToString()
	{
		return $"Object<{ObjectClass.Name}>";
	}

	public virtual string ToStringVolatile()
	{
		return ToString();
	}

	public virtual bool ToBoolVolatile()
	{
		return true;
	}
	
	public virtual bool EqualsVolatile(VariantObject other)
	{
		return this == other;
	}
}