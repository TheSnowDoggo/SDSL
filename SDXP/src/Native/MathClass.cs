namespace SDSL.Native;

[ClassExport]
public static class SealMath
{
    public static NativeClass Class { get; } = NativeClass.InheritObject("Math");
    
    public static void Generate(VariantAssembly assembly)
    {
        NativeClassFactory.GenerateClass(assembly, typeof(SealMath), Class);
    }
    
    [ConstantExport] public const double PI  = Math.PI;
    [ConstantExport] public const double E   = Math.E;
    [ConstantExport] public const double Tau = Math.Tau;

    [FunctionExport("Number", "Number")]
    public static Variant fmod(Variant[] args)
    {
        return FMod(args[0].AsInt32(), args[1].AsInt32());

        static double FMod(int x, int y)
        {
            int rem = x % y;
            return rem >= 0 ? rem : rem + y;
        }
    }

    [FunctionExport("Number")]
    public static Variant floor(Variant[] args)
    {
        return Math.Floor(args[0].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant ceil(Variant[] args)
    {
        return Math.Ceiling(args[0].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant truncate(Variant[] args)
    {
        return Math.Truncate(args[0].AsDouble());
    }

    [FunctionExport("Number", "Number", MinArgs = 1)]
    public static Variant round(Variant[] args) => args.Length switch
    {
        1 => Math.Round(args[0].AsDouble()),
        2 => Math.Round(args[0].AsDouble(), (int)args[1].AsDouble()),
        _ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}.")
    };

    [FunctionExport("Number")]
    public static Variant abs(Variant[] args)
    {
        return Math.Abs(args[0].AsDouble());
    }
    
    [FunctionExport("Number")]
    public static Variant deg_to_rad(Variant[] args)
    {
        return args[0].AsDouble() / 180.0 * Math.PI;
    }
    
    [FunctionExport("Number")]
    public static Variant rad_to_deg(Variant[] args)
    {
        return args[0].AsDouble() / Math.PI * 180.0;
    }

    [FunctionExport("Number", "Number", MinArgs = 1)]
    public static Variant log(Variant[] args) => args.Length switch
    {
        1 => Math.Log(args[0].AsDouble()),
        2 => Math.Log(args[0].AsDouble(), args[1].AsDouble()),
        _ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}.")
    };

    [FunctionExport("Number")]
    public static Variant log2(Variant[] args)
    {
        return Math.Log2(args[0].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant log10(Variant[] args)
    {
        return Math.Log10(args[0].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant sin(Variant[] args)
    {
        return Math.Sin(args[0].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant cos(Variant[] args)
    {
        return Math.Cos(args[0].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant tan(Variant[] args)
    {
        return Math.Tan(args[0].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant asin(Variant[] args)
    {
        return Math.Asin(args[0].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant acos(Variant[] args)
    {
        return Math.Acos(args[0].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant atan(Variant[] args)
    {
        return Math.Atan(args[0].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant sqrt(Variant[] args)
    {
        return Math.Sqrt(args[0].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant cbrt(Variant[] args)
    {
        return Math.Cbrt(args[0].AsDouble());
    }

    [FunctionExport("Number", "Number")]
    public static Variant pow(Variant[] args)
    {
        return Math.Pow(args[0].AsDouble(), args[1].AsDouble());
    }

    [FunctionExport("Number")]
    public static Variant exp(Variant[] args)
    {
        return Math.Exp(args[0].AsDouble());
    }

    [FunctionExport("Number", "Number")]
    public static Variant min(Variant[] args)
    {
        return  Math.Min(args[0].AsDouble(), args[1].AsDouble());
    }

    [FunctionExport("Number", "Number")]
    public static Variant max(Variant[] args)
    {
        return Math.Max(args[0].AsDouble(), args[1].AsDouble());
    }

    [FunctionExport("Number", "Number", "Number")]
    public static Variant clamp(Variant[] args)
    {
        return Math.Clamp(args[0].AsDouble(), args[1].AsDouble(), args[2].AsDouble());
    }
    
    [FunctionExport("Number")]
    public static Variant is_integer(Variant[] args)
    {
        return double.IsInteger(args[0].AsDouble());
    }
}