using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public class SealRandom : SealObject
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
    
    public static readonly SealClass Class = SealClass.CreateGlobal("Random");
    
    [ConstantExport]
    public static readonly SealRandom Shared = new SealRandom(Random.Shared);

    public override SealClass TypeClass => Class;

    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate<SealRandom>(pAssembly, Class);
    }

    [SealConstructor]
    [FunctionExport(SealNumber.Number, MinArgs = 0)]
    public static SealValue _new(SealValue[] args) => args.Length switch
    {
        0 => new SealRandom(),
        1 => new SealRandom((int)args[0].AsDouble()),
        _ => throw new ArgumentException($"Expected 0 or 1 args, got {args.Length}.")
    };

    [FunctionExport(SealNumber.Number, SealNumber.Number, MinArgs = 0)]
    public SealValue nexti(SealValue[] args)
    {
        return args.Length switch
        {
            0 => _random.Next(),
            1 => Nexti((int)args[0].AsDouble()),
            2 => Nexti((int)args[0].AsDouble(), (int)args[1].AsDouble()),
            _ => throw new ArgumentException($"Expected 0, 1 or 2 args, got {args.Length}.")
        };
    }

    private SealValue Nexti(int max)
    {
        return max >= 0 ? _random.Next(max) : SealValue.Nil;
    }

    private SealValue Nexti(int min, int max)
    {
        return min >= 0 ? _random.Next(min, max) : SealValue.Nil;
    }
    
    [FunctionExport(SealNumber.Number, SealNumber.Number, MinArgs = 0)]
    public  SealValue nextf(SealValue[] args)
    {
        return args.Length switch
        {
            0 => _random.NextDouble(),
            1 => Nextf(args[0].AsDouble()),
            2 => Nextf(args[0].AsDouble(), args[1].AsDouble()),
            _ => throw new ArgumentException($"Expected 0, 1 or 2 args, got {args.Length}.")
        };
    }

    private SealValue Nextf(double max)
    {
        return _random.NextDouble() * max;
    }

    private SealValue Nextf(double min, double max)
    {
        return double.Lerp(min, max, _random.NextDouble());
    }
}