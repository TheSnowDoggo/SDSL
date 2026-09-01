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
    
    // Maps function names to instance functions locations
    public FrozenDictionary<string, int> FunctionTable { get; set; }
    
    // Maps fields names to instance field locations
    public FrozenDictionary<string, int> FieldTable { get; set; }

    // Contains instance field type and expression information
    public FieldDefinition[] InstanceFields { get; set; }

    public FrozenSet<SealClass> BaseClasses { get; set; } = FrozenSet<SealClass>.Empty;
    
    // User or Native constructor
    public Function Constructor { get; set; }

    public bool TryGetFunction(string name, out Function function)
    {
        if (FunctionTable.TryGetValue(name, out int location))
        {
            function = SealAssembly.Current.StaticFunctions[location];
            return true;
        }

        function = null;
        return false;
    }
    
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

    public bool IsAssignableTo(SealClass classType)
    {
        if (classType == null)
        {
            return true;
        }

        if (this == classType)
        {
            return true;
        }

        if (ValueType == ValueType.Nil && classType.ValueType == ValueType.Object)
        {
            return true;
        }

        return BaseClasses.Contains(classType);
    }

    public override string ToString()
    {
        return $"{Namespace}::{Name}";
    }
}