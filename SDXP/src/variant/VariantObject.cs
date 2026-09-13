namespace SDSL;

public abstract class VariantObject : IEquatable<VariantObject>
{
	public abstract VariantClass VClass { get; }

	public override string ToString()
	{
		return ToSafeString();
	}

	public virtual string ToSafeString()
	{
		return $"Object<{VClass.Name}>";
	}

	public bool Equals(VariantObject other)
	{
		return this == other;
	}
}