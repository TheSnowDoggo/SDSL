namespace SDSL;

public abstract class VariantObject :
	IEquatable<VariantObject>,
	IComparable<VariantObject>
{
	public abstract VariantClass ParentClass { get; }

	public override string ToString()
	{
		return $"Object<{ParentClass.Name}>";
	}

	public virtual string ToUnsafeString()
	{
		return ToString();
	}

	public bool Equals(VariantObject other)
	{
		return this == other;
	}

	public virtual bool ToBool()
	{
		return true;
	}

	public int CompareTo(VariantObject other)
	{
		return 0;
	}
}