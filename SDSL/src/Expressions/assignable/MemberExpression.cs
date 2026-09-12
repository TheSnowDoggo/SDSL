using SDSL.Classes;
using SDSL.Functions;

namespace SDSL.Expressions;

public class MemberExpression : AssignableExpression
{
    private readonly Expression _instanceExpression;
    private readonly string _identifier;
    
    public MemberExpression(
        SourceLocation location,
        Expression instanceExpression,
        string identifier)
    {
        Location = location;
        _instanceExpression = instanceExpression;
        _identifier = identifier;
    }
    
    public override SealValue Evaluate(Variable[] variables)
    {
        return GetValue(variables, out _);
    }

    public SealValue GetValue(Variable[] variables, out SealValue instance)
    {
        instance = _instanceExpression.Evaluate(variables);

        if (instance.Class.TryGetFunction(_identifier, out Function function))
        {
            return function;
        }

        if (instance.ValueType == SealValueType.Object
            && instance.AsSealObject() is SealUserObject obj
            && obj.TypeClass.FieldTable.TryGetValue(_identifier, out int location))
        {
            return obj.Fields[location].Value;
        }

        if (SealGlobal.Class.TryGetFunction(_identifier, out function))
        {
            return function;
        }
        
        throw new RuntimeException(Location,
            $"Class {instance.Class} does not contain member function/field '{_identifier}'.");
    }

    public override void SetValue(Variable[] variables, SealValue value)
    {
        SealValue instance = _instanceExpression.Evaluate(variables);

        if (instance.ValueType != SealValueType.Object
            || instance.AsSealObject() is not SealUserObject obj)
        {
            throw new RuntimeException(Location,
                $"Cannot set field from non-user defined class {instance.Class}.");
        }

        if (!obj.TypeClass.FieldTable.TryGetValue(_identifier, out int location))
        {
            throw new RuntimeException(Location,
                $"Class {obj.TypeClass} does not contain member field '{_identifier}'.");
        }
        
        ref Field field = ref obj.Fields[location];

        if (field.IsConst)
        {
            throw new RuntimeException(Location,
                $"Cannot set readonly instance field '{_identifier}' in class {obj.TypeClass}.");
        }

        if (!value.Class.IsAssignableTo(field.Class))
        {
            throw new RuntimeException(Location,
                $"Value {value.Class} is not assignable to field {ToString()} of class {field.Class}.");
        }
        
        field.Value = value;
    }
    
    public override bool IsConstantEval()
    {
        return false;
    }

    public override string ToString()
    {
        return $"{_instanceExpression}.{_identifier}";
    }
}