using SDSL.Prototypes;
using System.Collections;
using System.Text;
using SDSL.Factory;

namespace SDSL.Classes;

[ClassExport]
public class SealArray : SealObject, IReadOnlyCollection<SealValue>
{
    private readonly List<SealValue> _values = [];

    public SealArray()
    {
    }
    
    public SealArray(List<SealValue> values)
    {
        _values = values;
    }
    
    public static readonly SealClass Class = SealClass.CreateGlobal("Array");

    public override SealClass TypeClass => Class;

    public int Count => _values.Count;
    
    public static SealArray Create(int size)
    {
        var values = new List<SealValue>(size);

        for (int i = 0; i < size; i++)
        {
            values.Add(default);
        }
        
        return new SealArray(values);
    }

    public static SealArray FromArray<T>(T[] arr, Func<T, SealValue> convert)
    {
        var items = new List<SealValue>(arr.Length);

        for (int i = 0; i < arr.Length; i++)
        {
            items.Add(convert(arr[i]));
        }

        return new SealArray(items);
    }
    
    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate<SealArray>(pAssembly, Class);
    }
    
    public void Add(SealValue value)
    {
        _values.Add(value);
    }

    [SealConstructor]
    [FunctionInfo("size")]
    [FunctionExport("Number", MinArgs = 0)]
    public static SealValue _new(SealValue[] args) => args.Length switch
    {
        0 => new SealArray(),
        1 => Create((int)args[0].AsDouble()),
        _ => throw new ArgumentException($"Expected 0 or 1 arguments, got {args.Length}."),
    };

    [FunctionExport]
    public SealValue size()
    {
        return _values.Count;
    }

    [FunctionInfo("index")]
    [FunctionExport("Number")]
    public SealValue _get(SealValue[] args)
    {
        return _values[args[0].AsInt32()];
    }
    
    [FunctionInfo("index", "value")]
    [FunctionExport("Number", "Any")]
    public SealValue _set(SealValue[] args)
    {
        return _values[args[0].AsInt32()] = args[1];
    }
    
    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public void push_back(SealValue[] args)
    {
        _values.Add(args[0]);
    }

    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public void push_front(SealValue[] args)
    {
        _values.Insert(0, args[0]);
    }

    [FunctionExport]
    public SealValue pop_back()
    {
        if (_values.Count == 0)
        {
            throw new InvalidOperationException("Cannot pop, Array is empty.");
        }

        int lastIndex = _values.Count - 1;
        
        SealValue item = _values[lastIndex];
        _values.RemoveAt(lastIndex);
        
        return item;
    }
    
    [FunctionExport]
    public SealValue pop_front()
    {
        if (_values.Count == 0)
        {
            throw new InvalidOperationException("Cannot pop, Array is empty.");
        }

        SealValue item = _values[0];
        _values.RemoveAt(0);
        
        return item;
    }

    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public SealValue erase(SealValue[] args)
    {
        return _values.Remove(args[0]);
    }

    [FunctionInfo("index")]
    [FunctionExport("Number")]
    public SealValue erase_at(SealValue[] args)
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
    public SealValue index_of(SealValue[] args)
    {
        return _values.IndexOf(args[0]);
    }

    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public SealValue has(SealValue[] args)
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
    public void fill(SealValue[] args)
    {
        SealValue value = args[0];
        
        for (int i = 0; i < _values.Count; i++)
        {
            _values[i] = value;
        }
    }

    [FunctionExport]
    public SealValue to_array()
    {
        return new SealArray([.._values]);
    }

    public override string ToString()
    {
        if (_values.Count == 0)
        {
            return "[  ]";
        }
        
        var sb = new StringBuilder();

        sb.Append("[ ");

        sb.Append(_values[0].ToString());

        for (int i = 1; i < _values.Count; i++)
        {
            sb.Append(", ");
            sb.Append(_values[i].ToString());
        }

        sb.Append(" ]");

        return sb.ToString();
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