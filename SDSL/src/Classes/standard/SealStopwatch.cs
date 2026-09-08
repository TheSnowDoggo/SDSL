using System.Diagnostics;
using SDSL.Prototypes;

namespace SDSL.Classes;

[CustomClassGenerator]
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
        SealClassFactory<SealStopwatch>.Generate(pAssembly, Class);
    }

    [SealConstructor]
    [SealFunctionExport]
    public static SealValue _new()
    {
        return new SealStopwatch();
    }

    [SealFunctionExport]
    public static SealValue start_new()
    {
        return new SealStopwatch(Stopwatch.StartNew());
    }

    [SealFunctionExport]
    public void start()
    {
        _sw.Start();
    }

    [SealFunctionExport]
    public void stop()
    {
        _sw.Stop();
    }

    [SealFunctionExport]
    public void restart()
    {
        _sw.Restart();
    }

    [SealFunctionExport]
    public void reset()
    {
        _sw.Reset();
    }

    [SealFunctionExport]
    public SealValue elapsed()
    {
        return _sw.Elapsed;
    }
}