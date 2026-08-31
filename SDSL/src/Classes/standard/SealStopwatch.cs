using System.Diagnostics;
using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public class SealStopwatch : SealObject
{
    private readonly Stopwatch _sw;

    public SealStopwatch()
    {
        _sw = new Stopwatch();
    }
    
    public SealStopwatch(Stopwatch sw)
    {
        _sw = sw;
    }
    
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        GlobalConfig.GlobalNamespace,
        "Stopwatch",
        ValueType.Object,
        false
    );

    public override SealClass TypeClass => Class;
    
    [FunctionExport("new() -> Stopwatch")]
    public static SealValue New(SealValue[] _)
        => new SealStopwatch();
    
    [FunctionExport("start_new() -> Stopwatch")]
    public static SealValue StartNew(SealValue[] _)
        => new SealStopwatch(Stopwatch.StartNew());
    
    [FunctionExport("start()")]
    public static void Start(SealValue self, SealValue[] _)
        => self.AsSealObject<SealStopwatch>()._sw.Start();
    
    [FunctionExport("stop()")]
    public static void Stop(SealValue self, SealValue[] _)
        => self.AsSealObject<SealStopwatch>()._sw.Stop();
    
    [FunctionExport("restart()")]
    public static void Restart(SealValue self, SealValue[] _)
        => self.AsSealObject<SealStopwatch>()._sw.Restart();
    
    [FunctionExport("reset()")]
    public static void Reset(SealValue self, SealValue[] _)
        => self.AsSealObject<SealStopwatch>()._sw.Reset();
    
    [FunctionExport("elapsed() -> TimeSpan")]
    public static SealValue Elapsed(SealValue self, SealValue[] _)
        => self.AsSealObject<SealStopwatch>()._sw.Elapsed;
}