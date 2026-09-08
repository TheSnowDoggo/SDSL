using SDSL.Prototypes;

namespace SDSL.Classes;

[NativeClass]
public static class SealMath
{
    [ClassExport]
    public static readonly SealClass Class = SealClass.CreateGlobal("Math");
    
    [ConstantExport] public const double PI = Math.PI;
    [ConstantExport] public const double E = Math.E;
    [ConstantExport] public const double Tau = Math.Tau;

    [FunctionExport("fmod(x: Number, y: Number) -> Number")]
    public static SealValue _FMod(SealValue[] args)
    {
        return FMod(args[0].AsInt32(), args[1].AsInt32());
    }
    
    private static int FMod(int x, int y)
    {
        int rem = x % y;
        return rem >= 0 ? rem : rem + y;
    }
    
    [FunctionExport("floor(x: Number) -> Number")]
    public static SealValue _Floor(SealValue[] args)
        => Math.Floor(args[0].AsNumber());
    
    [FunctionExport("ceil() -> Number")]
    public static SealValue _Ceil(SealValue[] args)
        => Math.Ceiling(args[0].AsNumber());
    
    [FunctionExport("truncate(x: Number) -> Number")]
    public static SealValue _Truncate(SealValue[] args)
        => Math.Truncate(args[0].AsNumber());

    [FunctionExport("round(x: Number, digits: Number = ?) -> Number")]
    public static SealValue _Round(SealValue[] args) => args.Length switch
    {
        1 => Math.Round(args[0].AsNumber()),
        2 => Math.Round(args[0].AsNumber(), (int)args[1].AsNumber()),
        _ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}.")
    };

    [FunctionExport("log(x: Number, base: Number = ?) -> Number")]
    public static SealValue _Log(SealValue[] args) => args.Length switch
    {
        1 => Math.Log(args[0].AsNumber()),
        2 => Math.Log(args[0].AsNumber(), args[1].AsNumber()),
        _ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}.")
    };
    
    [FunctionExport("log2(x: Number) -> Number")]
    public static SealValue _Log2(SealValue[] args)
        => Math.Log2(args[0].AsNumber());
    
    [FunctionExport("log10(x: Number) -> Number")]
    public static SealValue _Log10(SealValue[] args)
        => Math.Log10(args[0].AsNumber());
    
    [FunctionExport("sin(x: Number) -> Number")]
    public static SealValue _Sin(SealValue[] args)
        => Math.Sin(args[0].AsNumber());
    
    [FunctionExport("cos(x: Number) -> Number")]
    public static SealValue _Cos(SealValue[] args)
        => Math.Cos(args[0].AsNumber());
    
    [FunctionExport("tan(x: Number) -> Number")]
    public static SealValue _Tan(SealValue[] args)
        => Math.Tan(args[0].AsNumber());
    
    [FunctionExport("asin(x: Number) -> Number")]
    public static SealValue _Asin(SealValue[] args)
        => Math.Asin(args[0].AsNumber());
    
    [FunctionExport("acos(x: Number) -> Number")]
    public static SealValue _Acos(SealValue[] args)
        => Math.Acos(args[0].AsNumber());
    
    [FunctionExport("atan(x: Number) -> Number")]
    public static SealValue _Atan(SealValue[] args)
        => Math.Atan(args[0].AsNumber());
    
    [FunctionExport("sqrt(x: Number) -> Number")]
    public static SealValue _Sqrt(SealValue[] args)
        => Math.Sqrt(args[0].AsNumber());
    
    [FunctionExport("cbrt(x: Number) -> Number")]
    public static SealValue _Cbrt(SealValue[] args)
        => Math.Cbrt(args[0].AsNumber());
    
    [FunctionExport("pow(x: Number, y: Number) -> Number")]
    public static SealValue _Pow(SealValue[] args)
        => Math.Pow(args[0].AsNumber(), args[1].AsNumber());
    
    [FunctionExport("exp(x: Number) -> Number")]
    public static SealValue _Exp(SealValue[] args)
        => Math.Exp(args[0].AsNumber());
    
    [FunctionExport("min(x: Number, y: Number) -> Number")]
    public static SealValue _Min(SealValue[] args)
        => Math.Min(args[0].AsNumber(), args[1].AsNumber());
    
    [FunctionExport("max(x: Number, y: Number) -> Number")]
    public static SealValue _Max(SealValue[] args)
        => Math.Max(args[0].AsNumber(), args[1].AsNumber());
    
    [FunctionExport("clamp(x: Number, min: Number, max: Number) -> Number")]
    public static SealValue Clamp(SealValue[] args)
        => Math.Clamp(args[0].AsNumber(), args[1].AsNumber(), args[2].AsNumber());

    [FunctionExport("is_integer(x: Number) -> Bool")]
    public static SealValue IsInteger(SealValue[] args)
        => double.IsInteger(args[0].AsNumber());
}