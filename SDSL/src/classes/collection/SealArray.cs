using SDSL.Prototypes;
using System.Collections;
using System.Text;

namespace SDSL;

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
    
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        "global",
        "Array",
        SealValueType.Object
    );

    public override SealClass TypeClass => Class;

    public int Count => _values.Count;
    public int Capacity => _values.Capacity;
    
    public static SealArray Create(int size)
    {
        var values = new List<SealValue>(size);
        for (int i = 0; i < values.Count; i++)
            values.Add(default);
        return new SealArray(values);
    }

    [FunctionExport("new(size: Number = ?) -> Array")]
    public static SealValue New(ReadOnlySpan<SealValue> args) => args.Length switch
    {
        0 => new SealArray(),
        1 => Create((int)args[0].AsNumber()),
        _ => throw new ArgumentException(
            $"Expected 0 or 1 arguments, got {args.Length}.")
    };
    
    [FunctionExport("size() -> Number")]
    public static SealValue GetSize(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsSealObject<SealArray>()._values.Count;
    
    [FunctionExport("_get(index: Number) -> Number")]
    public static SealValue _Get(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsSealObject<SealArray>()._values[(int)args[0].AsNumber()];

    [FunctionExport("_set(index: Number, value: Any)")]
    public static void _Set(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsSealObject<SealArray>()._values[(int)args[0].AsNumber()] = args[1];
    
    [FunctionExport("push_back(item: Any)")]
    public static void PushBack(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsSealObject<SealArray>()._values.Add(args[0]);
    
    [FunctionExport("push_front(item: Any)")]
    public static void PushFront(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsSealObject<SealArray>()._values.Insert(0, args[0]);
    
    [FunctionExport("erase(item: Any) -> Bool")]
    public static SealValue Erase(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsSealObject<SealArray>()._values.Remove(args[0]);

    [FunctionExport("erase_at(index: Number) -> Bool")]
    public static SealValue EraseAt(SealValue self, ReadOnlySpan<SealValue> args)
    {
        var arr = self.AsSealObject<SealArray>();
        
        int index = (int)args[0].AsNumber();
        if (index < 0 || index >= arr.Count)
            return false;
        
        arr._values.RemoveAt(index);
        
        return true;
    }
    
    [FunctionExport("index_of(item: Any) -> Number")]
    public static SealValue IndexOf(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsSealObject<SealArray>()._values.IndexOf(args[0]);

    [FunctionExport("has(item: Any) -> Bool")]
    public static SealValue Contains(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsSealObject<SealArray>()._values.Contains(args[0]);
    
    [FunctionExport("clear()")]
    public static void Clear(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsSealObject<SealArray>()._values.Clear();

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
        => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _values.GetEnumerator();
}