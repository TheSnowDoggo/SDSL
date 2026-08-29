using System.Collections;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public class SealRange : SealObject, IEnumerable<SealValue>
{
    private readonly IEnumerable<SealValue> _values;

    public SealRange(IEnumerable<SealValue> values)
    {
        _values = values;
    }

    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        "global",
        "Range",
        ValueType.Object,
        true
    );
    
    public static IEnumerable<SealValue> CreateRange(double start, double end, double step)
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

    public static IEnumerable<SealValue> CreateRange(double start, double end)
        => CreateRange(start, end, end >= start ? 1 : -1);
    
    public static IEnumerable<SealValue> CreateRange(double end)
        => CreateRange(0, end, end >= 0 ? 1 : -1);

    public override SealClass TypeClass => Class;

    public IEnumerator<SealValue> GetEnumerator()
        => _values.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator()
        => _values.GetEnumerator();
}