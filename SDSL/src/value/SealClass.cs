using System.Collections.Frozen;
using SDSL.Expressions;
using SDSL.Functions;

namespace SDSL;

public class SealClass
{
    public SealClass(
        string namespaceName,
        string name,
        SealValueType valueType,
        bool generateConstructor = false)
    {
        Namespace = namespaceName;
        Name = name;
        ValueType = valueType;
        GenerateConstructor = generateConstructor;
    }

    public static readonly SealClass Implicit = new SealClass("_", "Implicit", SealValueType.Nil);
    
    public string Namespace { get; }
    
    public string Name { get; }
    
    public SealValueType ValueType { get; }
    
    public bool GenerateConstructor { get; }

    public string FullName => $"{Name}::{Namespace}";
    
    public SealAssembly CurrentAssembly { get; set; }
    
    // Maps function names to instance functions locations
    public FrozenDictionary<string, int> FunctionTable { get; set; }
    
    // Maps fields names to instance field locations
    public FrozenDictionary<string, MemberProperty> FieldTable { get; set; }

    // Contains instance field type and expression information
    public FieldDefinition[] InstanceFields { get; set; }

    public FrozenSet<SealClass> BaseClasses { get; set; } = FrozenSet<SealClass>.Empty;
    
    // User or Native constructor
    public Function Constructor { get; set; }
    
    public Func<double, object> StructObjectConverter { get; init; }
    public Func<double, string> StructStringConverter { get; init; }

    public static SealClass CreateGlobal(string name, SealValueType valueType = SealValueType.Object)
    {
        return new SealClass(GlobalConfig.Global, name, valueType);
    }
    
    public static SealClass CreateStruct(
        string namespaceName,
        string name,
        Func<double, object> structObjectConverter,
        Func<double, string> structStringConverter)
    {
        return new SealClass(namespaceName, name, SealValueType.Struct)
        {
            StructObjectConverter = structObjectConverter,
            StructStringConverter = structStringConverter,
        };
    }
    
    public static SealValue GetDefaultValue(SealClass sClass)
    {
        if (sClass == null)
        {
            return SealValue.Nil;
        }
        
        return sClass.ValueType switch
        {
            SealValueType.Bool   => false,
            SealValueType.Number => 0,
            SealValueType.String => string.Empty,
            _ => SealValue.Nil,
        };
    }

    public bool TryGetFunction(string name, out Function function)
    {
        if (CurrentAssembly != null 
            && FunctionTable.TryGetValue(name, out int location))
        {
            function = CurrentAssembly.StaticFunctions[location];
            return true;
        }

        function = null;
        return false;
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

        if (ValueType == SealValueType.Nil && classType.ValueType == SealValueType.Object)
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