using SDSL.Prototypes;
using System.Collections;
using System.Text;

namespace SDSL.Classes;

[SealClass]
public class SealMap : SealObject, IReadOnlyCollection<SealValue>
{
    private readonly Dictionary<SealValue, SealValue> _values;

    public SealMap()
    {
        _values = [];
    }

    public SealMap(Dictionary<SealValue, SealValue> values)
    {
        _values = values;
    }
    
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        "global",
        "Map",
        ValueType.Object,
        false
    );

    public override SealClass TypeClass => Class;

    public int Count => _values.Count;
    public int Capacity => _values.Capacity;

    [FunctionExport("new() -> Map")]
    public static SealValue New(SealValue[] args)
    {
        return new SealMap();
    }

    [FunctionExport("size() -> Number")]
    public static SealValue GetSize(SealValue self, SealValue[] args)
    {
        return self.AsSealObject<SealMap>()._values.Count;
    }

    [FunctionExport("_get(key: Any) -> Any")]
    public static SealValue _Get(SealValue self, SealValue[] args)
    {
        return self.AsSealObject<SealMap>()._values[args[0]];
    }

    [FunctionExport("_set(key: Any, value: Any)")]
    public static void _Set(SealValue self, SealValue[] args)
    {
        self.AsSealObject<SealMap>()._values[args[0]] = args[1];
    }

    [FunctionExport("insert(key: Any, value: Any) -> Bool")]
    public static SealValue Insert(SealValue self, SealValue[] args)
    {
        return self.AsSealObject<SealMap>()._values.TryAdd(args[0], args[1]);
    }

    [FunctionExport("get(key: Any, defaultValue: Any) -> Any")]
    public static SealValue Get(SealValue self, SealValue[] args)
    {
        return self.AsSealObject<SealMap>()._values.GetValueOrDefault(args[0], args[1]);
    }

    [FunctionExport("erase(key: Any) -> Bool")]
    public static SealValue Erase(SealValue self, SealValue[] args)
    {
        return self.AsSealObject<SealMap>()._values.Remove(args[0]);
    }

    [FunctionExport("has(key: Any) -> Bool")]
    public static SealValue Has(SealValue self, SealValue[] args)
    {
        return self.AsSealObject<SealMap>()._values.ContainsKey(args[0]);
    }

    [FunctionExport("clear()")]
    public static void Clear(SealValue self, SealValue[] args)
    {
        self.AsSealObject<SealMap>()._values.Clear();
    }
    
    public override string ToString()
    {
        switch (_values.Count)
        {
        case 0:
            return "{  }";
        default:
            var sb = new StringBuilder();

            sb.Append("{ ");

            foreach (var kvp in _values)
            {
                sb.Append(kvp.Key.ToString(false));
                sb.Append(": ");
                sb.Append(kvp.Value.ToString(false));
                sb.Append(", ");
            }

            sb[^2] = ' ';
            sb[^1] = '}';
            
            return sb.ToString();
        }
    }

    public IEnumerator<SealValue> GetEnumerator()
    {
        foreach (var kvp in _values)
        {
            yield return kvp.Key;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}