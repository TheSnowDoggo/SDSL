namespace SDSL;

public class NativeFunction : Function
{
    public NativeFunction(Func<SealValue, ReadOnlySpan<SealValue>, SealValue> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        Func = func;
    }
    
    public Func<SealValue, ReadOnlySpan<SealValue>, SealValue> Func { get; }
    
    protected override SealValue _Invoke(SealValue self, params ReadOnlySpan<SealValue> args)
    {
        return Func.Invoke(self, args);
    }
}