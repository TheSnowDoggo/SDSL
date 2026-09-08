using SDSL.Prototypes;
using System.Collections;
using System.Text;

namespace SDSL.Classes;

[CustomClassGenerator]
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
    
    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory<SealArray>.Generate(pAssembly, Class);
    }

    [SealConstructor]
    [SealFunctionExport("Number", MinArgs = 0)]
    public static SealValue _new(SealValue[] args) => args.Length switch
    {
        0 => new SealArray(),
        1 => Create((int)args[0].AsNumber()),
        _ => throw new ArgumentException($"Expected 0 or 1 arguments, got {args.Length}."),
    };

    [SealFunctionExport]
    public SealValue size()
    {
        return _values.Count;
    }

    [SealFunctionExport("Number")]
    public SealValue _get(SealValue[] args)
    {
        return _values[args[0].AsInt32()];
    }
    
    [SealFunctionExport("Number", "Any")]
    public SealValue _set(SealValue[] args)
    {
        return _values[args[0].AsInt32()] = args[1];
    }
    
    [SealFunctionExport("Any")]
    public void push_back(SealValue[] args)
    {
        _values.Add(args[0]);
    }

    [SealFunctionExport("Any")]
    public void push_front(SealValue[] args)
    {
        _values.Insert(0, args[0]);
    }

    [SealFunctionExport]
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
    
    [SealFunctionExport]
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

    [SealFunctionExport("Any")]
    public SealValue erase(SealValue[] args)
    {
        return _values.Remove(args[0]);
    }

    [SealFunctionExport("Number")]
    public SealValue erase_at(SealValue[] args)
    {
        int index = (int)args[0].AsNumber();

        if (index < 0 || index >= _values.Count)
        {
            return false;
        }
        
        _values.RemoveAt(index);
        
        return true;
    }

    [SealFunctionExport("Any")]
    public SealValue index_of(SealValue[] args)
    {
        return _values.IndexOf(args[0]);
    }

    [SealFunctionExport("Any")]
    public SealValue has(SealValue[] args)
    {
        return _values.Contains(args[0]);
    }

    [SealFunctionExport]
    public void clear()
    {
        _values.Clear();
    }

    [SealFunctionExport]
    public void sort()
    {
        _values.Sort();
    }
    
    [SealFunctionExport("Any")]
    public void fill(SealValue[] args)
    {
        SealValue value = args[0];
        
        for (int i = 0; i < _values.Count; i++)
        {
            _values[i] = value;
        }
    }
    
    public override string ToString()
    {
        switch (_values.Count)
        {
        case 0:
            return "[  ]";
        default:
            var sb = new StringBuilder();

            sb.Append("[ ");

            for (int i = 0; i < _values.Count; i++)
            {
                sb.Append(_values[i].ToString(false));
                sb.Append(", ");
            }

            sb[^2] = ' ';
            sb[^1] = ']';

            return sb.ToString();
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