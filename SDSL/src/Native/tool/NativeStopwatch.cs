using System.Diagnostics;

namespace SDSL.Native;

[ClassExport]
public class NativeStopwatch : VariantObject
{
    private readonly Stopwatch _sw;

    public NativeStopwatch()
    {
        _sw = new Stopwatch();
    }
    
    public NativeStopwatch(Stopwatch sw)
    {
        _sw = sw;
    }

    public static NativeClass Class { get; } = NativeClass.InheritObject("Stopwatch");

    public override VariantClass ObjectClass => Class;

    public static void Generate(VariantAssembly assembly)
    {
        NativeClassFactory.GenerateClass<NativeStopwatch>(assembly, Class);
    }

    [ConstructorExport]
    public static Variant _new()
    {
        return new NativeStopwatch();
    }
    
    [PropertyExport]
    public Variant elapsed => _sw.Elapsed;

    [FunctionExport]
    public static Variant start_new()
    {
        return new NativeStopwatch(Stopwatch.StartNew());
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