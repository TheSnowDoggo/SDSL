namespace SDSL;

public abstract class VariantObject :
	IComparable<VariantObject>
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

	public int CompareTo(VariantObject other)
	{
		return 0;
	}
}