using System.Diagnostics;

namespace SDSL.Classes;

[ClassExport]
public class SealStopwatch : VariantObject
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

    public static NativeVariantClass Class { get; } = new NativeVariantClass("Stopwatch");

    public override VariantClass ParentClass => Class;

    public static void Generate(VariantAssembly assembly)
    {
        VariantClassFactory.GenerateClass<SealStopwatch>(assembly, Class);
    }

    [ConstructorExport]
    [FunctionExport]
    public static Variant _new()
    {
        return new SealStopwatch();
    }
    
    [PropertyExport]
    public Variant elapsed => _sw.Elapsed;

    [FunctionExport]
    public static Variant start_new()
    {
        return new SealStopwatch(Stopwatch.StartNew());
    }

    [FunctionExport]
    public void start() => _sw.Start();

    [FunctionExport]
    public void stop() =>  _sw.Stop();

    [FunctionExport]
    public void restart() => _sw.Restart();

    [FunctionExport]
    public void reset() => _sw.Reset();
}