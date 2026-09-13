using System.Collections;

namespace SDSL.Classes;

[ClassExport]
public class SealRange : VariantObject, IEnumerable<Variant>
{
    private readonly IEnumerable<Variant> _values;

    public SealRange(IEnumerable<Variant> values)
    {
        _values = values;
    }

    public static NativeVariantClass Class { get; } = new NativeVariantClass("Range", VariantType.Object);
    
    public override VariantClass TypeClass => Class;
    
    public static void Generate(VariantAssembly assembly)
    {
        VariantClassFactory.GenerateClass<SealRange>(assembly, Class);
    }
    
    public static SealRange CreateRange(double start, double end, double step)
    {
        return new SealRange(GetRange(start, end, step));
    }
    
    public static SealRange CreateRange(double start, double end)
    {
        return new SealRange(GetRange(start, end, end >= start ? 1 : -1));
    }

    public static SealRange CreateRange(double end)
    {
        return new SealRange(GetRange(0, end, end >= 0 ? 1 : -1));
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