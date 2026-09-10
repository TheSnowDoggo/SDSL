using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealMath
{
    public static readonly SealClass Class = SealClass.CreateGlobal("Math");
    
    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate(typeof(SealMath), pAssembly, Class);
    }
    
    [ConstantExport] public const double PI  = Math.PI;
    [ConstantExport] public const double E   = Math.E;
    [ConstantExport] public const double Tau = Math.Tau;

    [FunctionExport("Number", "Number")]
    public static SealValue fmod(SealValue[] args)
    {
        return FMod(args[0].AsInt32(), args[1].AsInt32());

        static double FMod(int x, int y)
        {
            int rem = x % y;
            return rem >= 0 ? rem : rem + y;
        }
    }

    [FunctionExport("Number")]
    public static SealValue floor(SealValue[] args)
    {
        return Math.Floor(args[0].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue ceil(SealValue[] args)
    {
        return Math.Ceiling(args[0].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue truncate(SealValue[] args)
    {
        return Math.Truncate(args[0].AsNumber());
    }

    [FunctionExport("Number", "Number", MinArgs = 1)]
    public static SealValue round(SealValue[] args) => args.Length switch
    {
        1 => Math.Round(args[0].AsNumber()),
        2 => Math.Round(args[0].AsNumber(), (int)args[1].AsNumber()),
        _ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}.")
    };

    [FunctionExport("Number", "Number", MinArgs = 1)]
    public static SealValue log(SealValue[] args) => args.Length switch
    {
        1 => Math.Log(args[0].AsNumber()),
        2 => Math.Log(args[0].AsNumber(), args[1].AsNumber()),
        _ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}.")
    };

    [FunctionExport("Number")]
    public static SealValue log2(SealValue[] args)
    {
        return Math.Log2(args[0].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue log10(SealValue[] args)
    {
        return Math.Log10(args[0].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue sin(SealValue[] args)
    {
        return Math.Sin(args[0].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue cos(SealValue[] args)
    {
        return Math.Cos(args[0].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue tan(SealValue[] args)
    {
        return Math.Tan(args[0].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue asin(SealValue[] args)
    {
        return Math.Asin(args[0].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue acos(SealValue[] args)
    {
        return Math.Acos(args[0].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue atan(SealValue[] args)
    {
        return Math.Atan(args[0].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue sqrt(SealValue[] args)
    {
        return Math.Sqrt(args[0].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue cbrt(SealValue[] args)
    {
        return Math.Cbrt(args[0].AsNumber());
    }

    [FunctionExport("Number", "Number")]
    public static SealValue pow(SealValue[] args)
    {
        return Math.Pow(args[0].AsNumber(), args[1].AsNumber());
    }

    [FunctionExport("Number")]
    public static SealValue exp(SealValue[] args)
    {
        return Math.Exp(args[0].AsNumber());
    }

    [FunctionExport("Number", "Number")]
    public static SealValue min(SealValue[] args)
    {
        return  Math.Min(args[0].AsNumber(), args[1].AsNumber());
    }

    [FunctionExport("Number", "Number")]
    public static SealValue max(SealValue[] args)
    {
        return Math.Max(args[0].AsNumber(), args[1].AsNumber());
    }

    [FunctionExport("Number", "Number", "Number")]
    public static SealValue clamp(SealValue[] args)
    {
        return Math.Clamp(args[0].AsNumber(), args[1].AsNumber(), args[2].AsNumber());
    }
    
    [FunctionExport("Number")]
    public static SealValue is_integer(SealValue[] args)
    {
        return double.IsInteger(args[0].AsNumber());
    }
}