using System.Collections.Frozen;
using SDSL.Expressions;

namespace SDSL;

public class SealClass
{
    public SealClass(
        string namespaceName,
        string name)
    {
        Namespace = namespaceName;
        Name = name;
    }
    
    public string Namespace { get; }
    public string Name { get; }
    
    // Maps member functions to assembly locations
    public FrozenDictionary<string, int> FunctionTable { get; set; }
    
    // Maps member fields to instance field locations
    public FrozenDictionary<string, int> FieldTable { get; set; }

    public InstanceField[] InstanceFields { get; set; }
    
    public Function Constructor { get; set; }
    
    public static SealValue GetDefaultValue(SealClass sClass)
    {
        if (sClass == null)
            return SealValue.Nil;
        
        return sClass.GetTypeCatagory() switch
        {
            TypeCatagory.Bool   => false,
            TypeCatagory.Number => 0,
            TypeCatagory.String => string.Empty,
            _ => SealValue.Nil
        };
    }

    public TypeCatagory GetTypeCatagory()
    {
        return LangConfig.TypeCatagoryMap.GetValueOrDefault(this, TypeCatagory.Object);
    }

    public override string ToString()
    {
        return $"{Namespace}::{Name}";
    }
}