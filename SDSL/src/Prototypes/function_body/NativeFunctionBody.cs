namespace SDSL.Prototypes;

public class NativeFunctionBody : FunctionBody
{
    public NativeFunctionBody(Func<SealValue, SealValue[], SealValue> func)
    {
        Func = func;
    }
    
    public Func<SealValue, SealValue[], SealValue> Func { get; }
    
    public override string ToString()
    {
        return $"NativeFunctionBody<{Func.Method.Name}>";
    }
}