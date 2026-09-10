using SDSL.Functions;

namespace SDSL.Prototypes;

public class NativeFunctionBody : FunctionBody
{
    public NativeFunctionBody(NativeDelegate func)
    {
        Func = func;
    }
    
    public NativeDelegate Func { get; }
    
    public override string ToString()
    {
        return $"NativeFunctionBody<{Func.Method.Name}>";
    }
}