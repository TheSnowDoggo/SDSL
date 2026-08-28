namespace SDSL;

public abstract class SealObject
{
    public abstract SealClass Class { get; }
    
    public override string ToString()
    {
        return $"Object<{Class}>";
    }
}