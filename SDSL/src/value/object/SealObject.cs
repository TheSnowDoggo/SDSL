namespace SDSL;

public abstract class SealObject : IEquatable<SealObject>
{
    public abstract SealClass TypeClass { get; }
    
    public override string ToString()
        => $"Object<{TypeClass}>";
    
    public virtual bool Equals(SealObject other)
        => this == other;

    public virtual bool ToBool()
        => true;
}