using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public static class SealMath
{
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        GlobalConfig.GlobalNamespace,
        "Math",
        ValueType.Object,
        true
    );
    
    [ConstantExport] public const double PI = Math.PI;
    [ConstantExport] public const double E = Math.E;
    [ConstantExport] public const double Tau = Math.Tau;
    
    [FunctionExport("floor(x: Number) -> Number")]
    public static SealValue Floor(SealValue[] args)
        => Math.Floor(args[0].AsNumber());
    
    [FunctionExport("ceil() -> Number")]
    public static SealValue Ceil(SealValue[] args)
        => Math.Ceiling(args[0].AsNumber());
    
    [FunctionExport("truncate(x: Number) -> Number")]
    public static SealValue Truncate(SealValue[] args)
        => Math.Truncate(args[0].AsNumber());

    [FunctionExport("round(x: Number, digits: Number = ?) -> Number")]
    public static SealValue Round(SealValue[] args) => args.Length switch
    {
        1 => Math.Round(args[0].AsNumber()),
        2 => Math.Round(args[0].AsNumber(), (int)args[1].AsNumber()),
        _ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}.")
    };

    [FunctionExport("log(x: Number, base: Number = ?) -> Number")]
    public static SealValue Log(SealValue[] args) => args.Length switch
    {
        1 => Math.Log(args[0].AsNumber()),
        2 => Math.Log(args[0].AsNumber(), args[1].AsNumber()),
        _ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}.")
    };
    
    [FunctionExport("log2(x: Number) -> Number")]
    public static SealValue Log2(SealValue[] args)
        => Math.Log2(args[0].AsNumber());
    
    [FunctionExport("log10(x: Number) -> Number")]
    public static SealValue Log10(SealValue[] args)
        => Math.Log10(args[0].AsNumber());
    
    [FunctionExport("sin(x: Number) -> Number")]
    public static SealValue Sin(SealValue[] args)
        => Math.Sin(args[0].AsNumber());
    
    [FunctionExport("cos(x: Number) -> Number")]
    public static SealValue Cos(SealValue[] args)
        => Math.Cos(args[0].AsNumber());
    
    [FunctionExport("tan(x: Number) -> Number")]
    public static SealValue Tan(SealValue[] args)
        => Math.Tan(args[0].AsNumber());
    
    [FunctionExport("asin(x: Number) -> Number")]
    public static SealValue Asin(SealValue[] args)
        => Math.Asin(args[0].AsNumber());
    
    [FunctionExport("acos(x: Number) -> Number")]
    public static SealValue Acos(SealValue[] args)
        => Math.Acos(args[0].AsNumber());
    
    [FunctionExport("atan(x: Number) -> Number")]
    public static SealValue Atan(SealValue[] args)
        => Math.Atan(args[0].AsNumber());
    
    [FunctionExport("sqrt(x: Number) -> Number")]
    public static SealValue Sqrt(SealValue[] args)
        => Math.Sqrt(args[0].AsNumber());
    
    [FunctionExport("cbrt(x: Number) -> Number")]
    public static SealValue Cbrt(SealValue[] args)
        => Math.Cbrt(args[0].AsNumber());
    
    [FunctionExport("pow(x: Number, y: Number) -> Number")]
    public static SealValue Pow(SealValue[] args)
        => Math.Pow(args[0].AsNumber(), args[1].AsNumber());
    
    [FunctionExport("exp(x: Number) -> Number")]
    public static SealValue Exp(SealValue[] args)
        => Math.Exp(args[0].AsNumber());
    
    [FunctionExport("min(x: Number, y: Number) -> Number")]
    public static SealValue Min(SealValue[] args)
        => Math.Min(args[0].AsNumber(), args[1].AsNumber());
    
    [FunctionExport("max(x: Number, y: Number) -> Number")]
    public static SealValue Max(SealValue[] args)
        => Math.Max(args[0].AsNumber(), args[1].AsNumber());
    
    [FunctionExport("clamp(x: Number, min: Number, max: Number) -> Number")]
    public static SealValue Clamp(SealValue[] args)
        => Math.Clamp(args[0].AsNumber(), args[1].AsNumber(), args[2].AsNumber());
}