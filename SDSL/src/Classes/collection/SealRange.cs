using System.Collections;
using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public class SealRange : SealObject, IEnumerable<SealValue>
{
    private readonly IEnumerable<SealValue> _values;

    public SealRange(IEnumerable<SealValue> values)
    {
        _values = values;
    }

    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        "global",
        "Range",
        ValueType.Object,
        false
    );
    
    public override SealClass TypeClass => Class;
    
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
    
    private static IEnumerable<SealValue> GetRange(double start, double end, double step)
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

    public IEnumerator<SealValue> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}