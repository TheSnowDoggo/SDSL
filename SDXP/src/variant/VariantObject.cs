namespace SDSL;

public abstract class VariantObject : IEquatable<VariantObject>
{
	public abstract VariantClass Class { get; }

	public override string ToString()
	{
		return ToSafeString();
	}

	public virtual string ToSafeString()
	{
		return $"Object<{Class.Name}>";
	}

	public bool Equals(VariantObject other)
	{
		return this == other;
	}
}