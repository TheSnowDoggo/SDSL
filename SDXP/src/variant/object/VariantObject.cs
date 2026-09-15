namespace SDSL;

public abstract class VariantObject :
	IEquatable<VariantObject>,
	IComparable<VariantObject>
{
	public abstract VariantClass ParentClass { get; }

	public override string ToString()
	{
		return ToSafeString();
	}

	public virtual string ToSafeString()
	{
		return $"Object<{ParentClass.Name}>";
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