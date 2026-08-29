namespace SDSL;

public abstract class SealObject : IEquatable<SealObject>
{
    public abstract SealClass TypeClass { get; }
    
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