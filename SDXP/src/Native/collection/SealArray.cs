using System.Collections;
using System.Text;

namespace SDSL.Native;

[ClassExport]
public class SealArray : VariantObject, IReadOnlyCollection<Variant>
{
    private readonly List<Variant> _values = [];

    public SealArray()
    {
    }
    
    public SealArray(List<Variant> values)
    {
        _values = values;
    }

    public static NativeClass Class { get; } = new NativeClass("Array");

    public override VariantClass ParentClass => Class;

    public int Count => _values.Count;
    
    public static void Generate(VariantAssembly assembly)
    {
        NativeClassFactory.GenerateClass<SealArray>(assembly, Class);
    }
    
    public static SealArray Create(int size)
    {
        var values = new List<Variant>(size);

        for (int i = 0; i < size; i++)
        {
            values.Add(default);
        }
        
        return new SealArray(values);
    }

    public static SealArray FromArray<T>(T[] arr, Func<T, Variant> convert)
    {
        var items = new List<Variant>(arr.Length);

        for (int i = 0; i < arr.Length; i++)
        {
            items.Add(convert(arr[i]));
        }

        return new SealArray(items);
    }
    
    public void Add(Variant value)
    {
        _values.Add(value);
    }

    [ConstructorExport]
    [FunctionInfo("size")]
    [FunctionExport("Number", MinArgs = 0)]
    public static Variant _new(Variant[] args) => args.Length switch
    {
        0 => new SealArray(),
        1 => Create((int)args[0].AsDouble()),
        _ => throw new ArgumentException($"Expected 0 or 1 arguments, got {args.Length}."),
    };

    [PropertyExport]
    public Variant size => _values.Count;

    [FunctionInfo("index")]
    [FunctionExport("Number")]
    public Variant _get(Variant[] args)
    {
        return _values[args[0].AsInt32()];
    }
    
    [FunctionInfo("index", "value")]
    [FunctionExport("Number", "Any")]
    public Variant _set(Variant[] args)
    {
        return _values[args[0].AsInt32()] = args[1];
    }
    
    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public void push_back(Variant[] args)
    {
        _values.Add(args[0]);
    }

    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public void push_front(Variant[] args)
    {
        _values.Insert(0, args[0]);
    }

    [FunctionExport]
    public Variant pop_back()
    {
        if (_values.Count == 0)
        {
            throw new InvalidOperationException("Cannot pop, Array is empty.");
        }

        int lastIndex = _values.Count - 1;
        
        Variant item = _values[lastIndex];
        _values.RemoveAt(lastIndex);
        
        return item;
    }
    
    [FunctionExport]
    public Variant pop_front()
    {
        if (_values.Count == 0)
        {
            throw new InvalidOperationException("Cannot pop, Array is empty.");
        }

        Variant item = _values[0];
        _values.RemoveAt(0);
        
        return item;
    }

    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public Variant erase(Variant[] args)
    {
        return _values.Remove(args[0]);
    }

    [FunctionInfo("index")]
    [FunctionExport("Number")]
    public Variant erase_at(Variant[] args)
    {
        int index = (int)args[0].AsDouble();

        if (index < 0 || index >= _values.Count)
        {
            return false;
        }
        
        _values.RemoveAt(index);
        
        return true;
    }

    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public Variant index_of(Variant[] args)
    {
        return _values.IndexOf(args[0]);
    }

    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public Variant has(Variant[] args)
    {
        return _values.Contains(args[0]);
    }

    [FunctionExport]
    public void clear()
    {
        _values.Clear();
    }

    [FunctionExport]
    public void sort()
    {
        _values.Sort();
    }
    
    [FunctionInfo("fill_value")]
    [FunctionExport("Any")]
    public void fill(Variant[] args)
    {
        Variant value = args[0];
        
        for (int i = 0; i < _values.Count; i++)
        {
            _values[i] = value;
        }
    }

    [FunctionExport]
    public Variant to_array()
    {
        return new SealArray([.._values]);
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
            return "[  ]";
        }
        
        var sb = new StringBuilder();

        sb.Append("[ ");

        sb.Append(_values[0].ToUnsafeString());

        for (int i = 1; i < _values.Count; i++)
        {
            sb.Append(", ");
            sb.Append(_values[i].ToUnsafeString());
        }

        sb.Append(" ]");

        return sb.ToString();
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