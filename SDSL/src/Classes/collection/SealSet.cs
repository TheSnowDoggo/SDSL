using System.Text;
using System.Collections;
using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[CustomClassGenerator]
public class SealSet : SealObject, IReadOnlyCollection<SealValue>
{
    private readonly HashSet<SealValue> _values;

    public SealSet()
    {
        _values = [];
    }
    
    public SealSet(HashSet<SealValue> values)
    {
        _values = values;
    }
    
    public static readonly SealClass Class = SealClass.CreateGlobal("Set");

    public override SealClass TypeClass => Class;
    
    public int Count => _values.Count;

    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate<SealSet>(pAssembly, Class);
    }
    
    public void Add(SealValue value)
    {
        _values.Add(value);
    }

    [SealConstructor]
    [SealFunctionInfo("collection")]
    [SealFunctionExport("Any", MinArgs = 0)]
    public static SealValue _new(SealValue[] args)
    {
        if (args.Length == 0)
        {
            return new SealSet();
        }
        
        SealValue collection = args[0];

        switch (collection.ValueType)
        {
        case ValueType.String:
            return _new_from_string(collection.ToString());
        case ValueType.Object:
            if (collection.AsSealObject() is not IEnumerable<SealValue> enumerable)
            {
                throw new ArgumentException($"Expected object to be enumerable, got {collection.Class}.");
            }
            
            return new SealSet([..enumerable]);
        default:
            throw new ArgumentException($"Expected value of type String or Object, got {collection.ValueType}.");
        }

        static SealSet _new_from_string(string s)
        {
            var hashSet = new HashSet<SealValue>(s.Length);

            for (var i = 0; i < s.Length; i++)
            {
                hashSet.Add(s[i]);
            }

            return new SealSet(hashSet);
        }
    }
    
    [SealFunctionExport]
    public SealValue size()
    {
        return _values.Count;
    }
    
    [SealFunctionInfo("value")]
    [SealFunctionExport("Any")]
    public SealValue add(SealValue[] args)
    {
        return _values.Add(args[0]);
    }
    
    [SealFunctionInfo("value")]
    [SealFunctionExport("Any")]
    public SealValue remove(SealValue[] args)
    {
        return _values.Remove(args[0]);
    }

    [SealFunctionInfo("value")]
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
    public SealValue to_array()
    {
        var items = new List<SealValue>(_values.Count);

        foreach (SealValue item in _values)
        {
            items.Add(item);
        }
        
        return new SealArray(items);
    }

    public override string ToString()
    {
        if (_values.Count == 0)
        {
            return "{  }";
        }

        var sb = new StringBuilder();

        sb.Append("{ ");

        foreach (SealValue value in _values)
        {
            sb.Append(value.ToString());
            sb.Append(", ");
        }
        
        sb[^2] = ' ';
        sb[^1] = '}';
        
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