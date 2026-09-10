namespace SDSL;

public abstract class SealObject : IEquatable<SealObject>
{
    public abstract SealClass TypeClass { get; }

    public virtual string ToString(bool useRaw)
    {
        return $"Object<{TypeClass}>";
    }

    public override string ToString()
    {
        return ToString(false);
    }

    public virtual bool Equals(SealObject other)
    {
        return this == other;
    }

    public virtual bool ToBool()
    {
        return true;
    }
}