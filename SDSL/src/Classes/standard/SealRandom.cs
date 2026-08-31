using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
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
    
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        GlobalConfig.GlobalNamespace,
        "Random",
        ValueType.Object,
        false
    );
    
    [ConstantExport]
    public static readonly SealRandom Shared = new SealRandom(Random.Shared);

    public override SealClass TypeClass => Class;

    [FunctionExport("new(seed: Number = ?) -> Random")]
    public static SealValue New(SealValue[] args) => args.Length switch
    {
        0 => new SealRandom(),
        1 => new SealRandom((int)args[0].AsNumber()),
        _ => throw new ArgumentException($"Expected 0 or 1 args, got {args.Length}.")
    };

    [FunctionExport("nexti(min: Number = ?, max: Number = ?) -> Number")]
    public static SealValue Nexti(SealValue self, SealValue[] args)
    {
        SealRandom r = self.AsSealObject<SealRandom>();

        return args.Length switch
        {
            0 => r._random.Next(),
            1 => r.Nexti((int)args[0].AsNumber()),
            2 => r.Nexti((int)args[0].AsNumber(), (int)args[1].AsNumber()),
            _ => throw new ArgumentException($"Expected 0, 1 or 2 args, got {args.Length}.")
        };
    }
    
    private SealValue Nexti(int max)
        => max >= 0 ? _random.Next(max) : SealValue.Nil;
    
    private SealValue Nexti(int min, int max)
        => min >= 0 ? _random.Next(min, max) : SealValue.Nil;
    
    [FunctionExport("nextf(min: Number = ?, max: Number = ?) -> Number")]
    public static SealValue Nextf(SealValue self, SealValue[] args)
    {
        SealRandom r = self.AsSealObject<SealRandom>();

        return args.Length switch
        {
            0 => r._random.NextDouble(),
            1 => r.Nextf(args[0].AsNumber()),
            2 => r.Nextf(args[0].AsNumber(), args[1].AsNumber()),
            _ => throw new ArgumentException($"Expected 0, 1 or 2 args, got {args.Length}.")
        };
    }

    private SealValue Nextf(double max)
        => _random.NextDouble() * max;
    
    private SealValue Nextf(double min, double max)
        => double.Lerp(min, max, _random.NextDouble());
}