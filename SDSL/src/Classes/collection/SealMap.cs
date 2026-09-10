using SDSL.Prototypes;
using System.Collections;
using System.Text;
using SDSL.Factory;

namespace SDSL.Classes;

[ClassExport]
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
    
    public static readonly SealClass Class = SealClass.CreateGlobal("Map");

    public override SealClass TypeClass => Class;

    public int Count => _values.Count;

    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate<SealMap>(pAssembly, Class);
    }

    public void Add(SealValue key, SealValue value)
    {
        _values.Add(key, value);
    }

    [SealConstructor]
    [FunctionExport]
    public static SealValue _new()
    {
        return new SealMap();
    }

    [FunctionExport]
    public SealValue size()
    {
        return _values.Count;
    }

    [FunctionInfo("key")]
    [FunctionExport("Any")]
    public SealValue _get(SealValue[] args)
    {
        return _values[args[0]];
    }

    [FunctionInfo("key", "value")]
    [FunctionExport("Any", "Any")]
    public void _set(SealValue[] args)
    {
        _values[args[0]] = args[1];
    }

    [FunctionInfo("key", "value")]
    [FunctionExport("Any", "Any")]
    public SealValue insert(SealValue[] args)
    {
        return _values.TryAdd(args[0], args[1]);
    }

    [FunctionInfo("key", "default_value")]
    [FunctionExport("Any", "Any")]
    public SealValue get(SealValue[] args)
    {
        return _values.GetValueOrDefault(args[0], args[1]);
    }

    [FunctionInfo("key")]
    [FunctionExport("Any")]
    public SealValue erase(SealValue[] args)
    {
        return _values.Remove(args[0]);
    }

    [FunctionInfo("key")]
    [FunctionExport("Any")]
    public SealValue has(SealValue[] args)
    {
        return _values.ContainsKey(args[0]);
    }

    [FunctionExport]
    public void clear()
    {
        _values.Clear();
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