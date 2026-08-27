using System.Collections.Frozen;

namespace SDSL;

public class SealClass
{
    public SealClass(
        string @namespace,
        string name)
    {
        Namespace = @namespace;
        Name = name;
    }
    
    public string Namespace { get; }
    public string Name { get; }
    
    // Maps member functions to assembly locations
    public FrozenDictionary<string, int> FunctionTable { get; init; }
    
    // Maps member fields to instance field locations
    public FrozenDictionary<string, int> FieldTable { get; init; }

    public static readonly SealClass Nil = new SealClass(
        LangConfig.GlobalNamespace,
        "Nil"
    );

    public static readonly SealClass Bool = new SealClass(
        LangConfig.GlobalNamespace,
        "Bool"
    );
    
    public static readonly SealClass Number = new SealClass(
        LangConfig.GlobalNamespace,
        "Number"
    );
    
    public static readonly SealClass String = new SealClass(
        LangConfig.GlobalNamespace,
        "String"
    );
    
    public static readonly SealClass Function = new SealClass(
        LangConfig.GlobalNamespace,
        "Function"
    );

    public TypeCatagory GetTypeCatagory()
    {
        return LangConfig.TypeCatagoryMap.GetValueOrDefault(this, TypeCatagory.Object);
    }

    public override string ToString()
    {
        return $"{Namespace}::{Name}";
    }
}