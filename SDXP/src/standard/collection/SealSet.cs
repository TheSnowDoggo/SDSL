using System.Text;
using System.Collections;

namespace SDSL.Classes;

[ClassExport]
public class SealSet : VariantObject, IReadOnlyCollection<Variant>
{
    private readonly HashSet<Variant> _values;

    public SealSet()
    {
        _values = [];
    }
    
    public SealSet(HashSet<Variant> values)
    {
        _values = values;
    }
    
    public static NativeVariantClass Class { get; } = new NativeVariantClass("Set", VariantType.Object);

    public override VariantClass TypeClass => Class;
    
    public int Count => _values.Count;

    public static void Generate(VariantAssembly assembly)
    {
        VariantClassFactory.GenerateClass<SealSet>(assembly, Class);
    }
    
    public void Add(Variant value)
    {
        _values.Add(value);
    }

    [ConstructorExport]
    [FunctionInfo("collection")]
    [FunctionExport("Any", MinArgs = 0)]
    public static Variant _new(Variant[] args)
    {
        if (args.Length == 0)
        {
            return new SealSet();
        }
        
        Variant collection = args[0];

        switch (collection.VariantType)
        {
        case VariantType.String:
            return _new_from_string(collection.ToString());
        case VariantType.Object:
            if (collection.AsVariantObject() is not IEnumerable<Variant> enumerable)
            {
                throw new ArgumentException($"Expected object to be enumerable, got {collection.Class}.");
            }
            
            return new SealSet([..enumerable]);
        default:
            throw new ArgumentException($"Expected value of type String or Object, got {collection.VariantType}.");
        }

        static SealSet _new_from_string(string s)
        {
            var hashSet = new HashSet<Variant>(s.Length);

            for (var i = 0; i < s.Length; i++)
            {
                hashSet.Add(s[i]);
            }

            return new SealSet(hashSet);
        }
    }
    
    [PropertyExport]
    public Variant size => _values.Count;
    
    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public Variant add(Variant[] args)
    {
        return _values.Add(args[0]);
    }
    
    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public Variant remove(Variant[] args)
    {
        return _values.Remove(args[0]);
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
    public Variant to_array()
    {
        var items = new List<Variant>(_values.Count);

        foreach (Variant item in _values)
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

        foreach (Variant value in _values)
        {
            sb.Append(value.ToString());
            sb.Append(", ");
        }
        
        sb[^2] = ' ';
        sb[^1] = '}';
        
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