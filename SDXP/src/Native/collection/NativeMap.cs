using System.Collections;
using System.Text;

namespace SDSL.Native;

[ClassExport]
public class NativeMap : VariantObject, IReadOnlyCollection<Variant>
{
    private readonly Dictionary<Variant, Variant> _values;

    public NativeMap()
    {
        _values = [];
    }

    public NativeMap(Dictionary<Variant, Variant> values)
    {
        _values = values;
    }
    
    public static NativeClass Class { get; } = NativeClass.InheritObject("Map");

    public override VariantClass ParentClass => Class;

    public int Count => _values.Count;

    public static void Generate(VariantAssembly assembly)
    {
        NativeClassFactory.GenerateClass<NativeMap>(assembly, Class);
    }

    public void Add(Variant key, Variant value)
    {
        _values.Add(key, value);
    }

    [ConstructorExport]
    public static Variant _new()
    {
        return new NativeMap();
    }

    [PropertyExport]
    public Variant size => _values.Count;

    [FunctionInfo("key")]
    [FunctionExport("Any")]
    public Variant _get(Variant[] args)
    {
        return _values[args[0]];
    }

    [FunctionInfo("key", "value")]
    [FunctionExport("Any", "Any")]
    public void _set(Variant[] args)
    {
        _values[args[0]] = args[1];
    }

    [FunctionInfo("key", "value")]
    [FunctionExport("Any", "Any")]
    public Variant insert(Variant[] args)
    {
        return _values.TryAdd(args[0], args[1]);
    }

    [FunctionInfo("key", "default_value")]
    [FunctionExport("Any", "Any")]
    public Variant get(Variant[] args)
    {
        return _values.GetValueOrDefault(args[0], args[1]);
    }

    [FunctionInfo("key")]
    [FunctionExport("Any")]
    public Variant erase(Variant[] args)
    {
        return _values.Remove(args[0]);
    }

    [FunctionInfo("key")]
    [FunctionExport("Any")]
    public Variant has(Variant[] args)
    {
        return _values.ContainsKey(args[0]);
    }

    [FunctionExport]
    public void clear()
    {
        _values.Clear();
    }
    
    [FunctionExport]
    public Variant to_string()
    {
        return ToString();
    }
    
    public override string ToString()
    {
        if (_values.Count == 0)
        {
            return "{  }";
        }
        
        var sb = new StringBuilder();

        sb.Append("{ ");

        foreach (var kvp in _values)
        {
            sb.Append(kvp.Key.ToUnsafeString());
            sb.Append(": ");
            sb.Append(kvp.Value.ToUnsafeString());
            sb.Append(", ");
        }

        sb[^2] = ' ';
        sb[^1] = '}';
            
        return sb.ToString();
    }

    public IEnumerator<Variant> GetEnumerator()
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