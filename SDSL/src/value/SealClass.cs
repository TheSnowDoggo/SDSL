using System.Collections.Frozen;
using SDSL.Expressions;

namespace SDSL;

public class SealClass
{
    public SealClass(
        string namespaceName,
        string name,
        ValueType valueType)
    {
        Namespace = namespaceName;
        Name = name;
        ValueType = valueType;
    }
    
    public string Namespace { get; }
    public string Name { get; }
    public ValueType ValueType { get; }
    
    // Maps member functions to assembly locations
    public FrozenDictionary<string, Function> FunctionTable { get; set; }
    
    // Maps member fields to instance field locations
    public FrozenDictionary<string, int> FieldTable { get; set; }

    public InstanceField[] InstanceFields { get; set; }
    
    public Function Constructor { get; set; }
    
    public static SealValue GetDefaultValue(SealClass sClass)
    {
        if (sClass == null)
            return SealValue.Nil;
        
        return sClass.ValueType switch
        {
            ValueType.Bool   => false,
            ValueType.Number => 0,
            ValueType.String => string.Empty,
            _ => SealValue.Nil
        };
    }

    public override string ToString()
    {
        return $"{Namespace}::{Name}";
    }
}