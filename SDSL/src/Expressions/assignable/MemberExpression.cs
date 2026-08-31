using SDSL.Classes;
using SDSL.Functions;

namespace SDSL.Expressions;

public class MemberExpression : AssignableExpression
{
    public MemberExpression(
        SourceLocation location,
        Expression instanceExpression,
        string identifier)
    {
        Location = location;
        InstanceExpression = instanceExpression;
        Identifier = identifier;
    }
    
    public Expression InstanceExpression { get; }
    public string Identifier { get; }
    
    public override SealValue Evaluate(Variable[] variables)
    {
        return GetValue(variables, out _);
    }

    public SealValue GetValue(Variable[] variables, out SealValue instance)
    {
        instance = InstanceExpression.Evaluate(variables);

        if (instance.Class.FunctionTable.TryGetValue(Identifier, out Function function))
        {
            return function;
        }

        if (instance.ValueType == ValueType.Object
            && instance.AsSealObject() is SealUserObject obj
            && obj.TypeClass.FieldTable.TryGetValue(Identifier, out int location))
        {
            return obj.Fields[location].Value;
        }

        if (SealGlobal.Class.FunctionTable.TryGetValue(Identifier, out function))
        {
            return function;
        }
        
        throw new RuntimeException(Location,
            $"Class {instance.Class} does not contain member function/field '{Identifier}'.");
    }

    public override void SetValue(Variable[] variables, SealValue value)
    {
        SealValue instance = InstanceExpression.Evaluate(variables);

        if (instance.ValueType != ValueType.Object
            || instance.AsSealObject() is not SealUserObject obj)
        {
            throw new RuntimeException(Location,
                $"Cannot set field from non-user defined class {instance.Class}.");
        }

        if (!obj.TypeClass.FieldTable.TryGetValue(Identifier, out int location))
        {
            throw new RuntimeException(Location,
                $"Class {obj.TypeClass} does not contain member field '{Identifier}'.");
        }
        
        ref Field field = ref obj.Fields[location];

        if (field.IsConst)
        {
            throw new RuntimeException(Location,
                $"Cannot set readonly instance field '{Identifier}' in class {obj.TypeClass}.");
        }

        if (field.Class != null && field.Class != value.Class)
        {
            throw new RuntimeException(Location,
                $"Cannot set field of class {field.Class} to value of class {value.Class}.");
        }
        
        field.Value = value;
    }

    public override string ToString()
    {
        return $"{InstanceExpression}.{Identifier}";
    }
}