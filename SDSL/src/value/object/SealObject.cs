namespace SDSL;

public abstract class SealObject
{
    public abstract SealClass TypeClass { get; }
    
    public override string ToString()
    {
        return $"Object<{TypeClass}>";
    }
}