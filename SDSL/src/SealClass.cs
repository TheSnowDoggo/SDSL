using System.Collections.Frozen;

namespace SDSL;

public class SealClass
{
    private readonly Func<SealValue> _defaultValueFunc;
    
    public SealClass(
        string @namespace,
        string name,
        Func<SealValue> defaultValueFunc = null)
    {
        Namespace = @namespace;
        Name = name;
        _defaultValueFunc = defaultValueFunc ?? (static () => SealValue.Nil);
    }
    
    public string Namespace { get; }
    public string Name { get; }
    
    // Maps member functions to assembly locations
    public FrozenDictionary<string, int> FunctionTable { get; set; }
    
    // Maps member fields to instance field locations
    public FrozenDictionary<string, int> FieldTable { get; set; }

    public static readonly SealClass Nil = new SealClass(
        LangConfig.GlobalNamespace,
        "Nil"
    );

    public static readonly SealClass Bool = new SealClass(
        LangConfig.GlobalNamespace,
        "Bool",
        static () => false
    );

    public static readonly SealClass Number = new SealClass(
        LangConfig.GlobalNamespace,
        "Number",
        static () => 0
    );

    public static readonly SealClass String = new SealClass(
        LangConfig.GlobalNamespace,
        "String",
        static () => string.Empty
    );
    
    public static readonly SealClass Function = new SealClass(
        LangConfig.GlobalNamespace,
        "Function"
    );

    public SealValue GetDefaultValue()
        => _defaultValueFunc();
    
    public TypeCatagory GetTypeCatagory()
    {
        return LangConfig.TypeCatagoryMap.GetValueOrDefault(this, TypeCatagory.Object);
    }

    public override string ToString()
    {
        return $"{Namespace}::{Name}";
    }
}