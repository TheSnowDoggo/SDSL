using System.Collections.Frozen;
using SDSL.Functions;

namespace SDSL;

public class SealClass
{
    public SealClass(
        string namespaceName,
        string name,
        ValueType valueType,
        bool generateConstructor)
    {
        Namespace = namespaceName;
        Name = name;
        ValueType = valueType;
        GenerateConstructor = generateConstructor;
    }
    
    public string Namespace { get; }
    public string Name { get; }
    public ValueType ValueType { get; }
    public bool GenerateConstructor { get; }
    
    // Maps function names to instance functions
    public FrozenDictionary<string, Function> FunctionTable { get; set; }
    
    // Maps fields names to instance field locations
    public FrozenDictionary<string, int> FieldTable { get; set; }

    // Contains instance field type and expression information
    public FieldDefinition[] InstanceFields { get; set; }
    
    // User or Native constructor
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