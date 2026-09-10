using System.Diagnostics;
using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
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
    
    public static readonly SealClass Class = SealClass.CreateGlobal("Stopwatch");

    public override SealClass TypeClass => Class;

    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate<SealStopwatch>(pAssembly, Class);
    }

    [SealConstructor]
    [FunctionExport]
    public static SealValue _new()
    {
        return new SealStopwatch();
    }

    [FunctionExport]
    public static SealValue start_new()
    {
        return new SealStopwatch(Stopwatch.StartNew());
    }

    [FunctionExport]
    public void start()
    {
        _sw.Start();
    }

    [FunctionExport]
    public void stop()
    {
        _sw.Stop();
    }

    [FunctionExport]
    public void restart()
    {
        _sw.Restart();
    }

    [FunctionExport]
    public void reset()
    {
        _sw.Reset();
    }

    [FunctionExport]
    public SealValue elapsed()
    {
        return _sw.Elapsed;
    }
}