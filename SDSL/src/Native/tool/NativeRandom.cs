namespace SDSL.Native;

[ClassExport]
public class NativeRandom : VariantObject
{
    private readonly Random _random;

    public NativeRandom()
    {
        _random = new Random();
    }

    public NativeRandom(int seed)
    {
        _random = new Random(seed);
    }
    
    public NativeRandom(Random random)
    {
        _random = random;
    }

    public static NativeClass Class { get; } = NativeClass.InheritObject("Random");
    
    [PropertyExport]
    public static Variant Shared { get; } = new NativeRandom(Random.Shared);

    public override VariantClass ObjectClass => Class;

    public static void Generate(VariantAssembly assembly)
    {
        NativeClassFactory.GenerateClass<NativeRandom>(assembly, Class);
    }

    [ConstructorExport("Number", MinArgs = 0)]
    public static Variant _new(Variant[] args) => args.Length switch
    {
        0 => new NativeRandom(),
        1 => new NativeRandom((int)args[0].AsDouble()),
        _ => throw new ArgumentException($"Expected 0 or 1 args, got {args.Length}.")
    };

    [FunctionExport("Number", "Number", MinArgs = 0)]
    public Variant nexti(Variant[] args) => args.Length switch
    {
        0 => _random.Next(),
        1 => Nexti((int)args[0].AsDouble()),
        2 => Nexti((int)args[0].AsDouble(), (int)args[1].AsDouble()),
        _ => throw new ArgumentException($"Expected 0, 1 or 2 args, got {args.Length}.")
    };

    private Variant Nexti(int max)
    {
        return max >= 0 ? _random.Next(max) : Variant.Nil;
    }

    private Variant Nexti(int min, int max)
    {
        return min >= 0 ? _random.Next(min, max) : Variant.Nil;
    }
    
    [FunctionExport("Number", "Number", MinArgs = 0)]
    public Variant nextf(Variant[] args) => args.Length switch
    {
        0 => _random.NextDouble(),
        1 => Nextf(args[0].AsDouble()),
        2 => Nextf(args[0].AsDouble(), args[1].AsDouble()),
        _ => throw new ArgumentException($"Expected 0, 1 or 2 args, got {args.Length}.")
    };

    private double Nextf(double max)
    {
        return _random.NextDouble() * max;
    }

    private double Nextf(double min, double max)
    {
        return double.Lerp(min, max, _random.NextDouble());
    }
}