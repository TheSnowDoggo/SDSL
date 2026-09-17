using System.Collections;

namespace SDSL.Native;

[ClassExport]
public class NativeRange : VariantObject, IEnumerable<Variant>
{
    private readonly IEnumerable<Variant> _values;

    public NativeRange(IEnumerable<Variant> values)
    {
        _values = values;
    }

    public static NativeClass Class { get; } = NativeClass.InheritObject("Range");
    
    public override VariantClass ParentClass => Class;
    
    public static void Generate(VariantAssembly assembly)
    {
        NativeClassFactory.GenerateClass<NativeRange>(assembly, Class);
    }
    
    public static NativeRange CreateRange(double start, double end, double step)
    {
        return new NativeRange(GetRange(start, end, step));
    }
    
    public static NativeRange CreateRange(double start, double end)
    {
        return new NativeRange(GetRange(start, end, end >= start ? 1 : -1));
    }

    public static NativeRange CreateRange(double end)
    {
        return new NativeRange(GetRange(0, end, end >= 0 ? 1 : -1));
    }
    
    private static IEnumerable<Variant> GetRange(double start, double end, double step)
    {
        switch (step)
        {
            case 0:
                yield break;
            case > 0:
            {
                for (double i = start; i < end; i += step)
                    yield return i;
                break;
            }
            default:
            {
                for (double i = start; i > end; i += step)
                    yield return i;
                break;
            }
        }
    }

    public IEnumerator<Variant> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}