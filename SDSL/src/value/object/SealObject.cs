namespace SDSL;

public abstract class SealObject : IEquatable<SealObject>
{
    public abstract SealClass TypeClass { get; }

    public string ToString(bool useSaveValue)
    {
        return useSaveValue ? $"Object<{TypeClass}>" : ToString();
    }

    public override string ToString()
    {
        return $"Object<{TypeClass}>";
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