namespace SDSL.Native;

[ClassExport]
public class SealRandom : VariantObject
{
    private readonly Random _random;

    public SealRandom()
    {
        _random = new Random();
    }

    public SealRandom(int seed)
    {
        _random = new Random(seed);
    }
    
    public SealRandom(Random random)
    {
        _random = random;
    }

    public static NativeClass Class { get; } = new NativeClass("Random");
    
    [PropertyExport]
    public static Variant Shared { get; } = new SealRandom(Random.Shared);

    public override VariantClass ParentClass => Class;

    public static void Generate(VariantAssembly assembly)
    {
        NativeClassFactory.GenerateClass<SealRandom>(assembly, Class);
    }

    [ConstructorExport]
    [FunctionExport("Number", MinArgs = 0)]
    public static Variant _new(Variant[] args) => args.Length switch
    {
        0 => new SealRandom(),
        1 => new SealRandom((int)args[0].AsDouble()),
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