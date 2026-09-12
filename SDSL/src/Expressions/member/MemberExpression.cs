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

        if (instance.ValueType == SealValueType.Object)
        {
            SealObject self = instance.AsSealObject();

            if (self.TypeClass.FieldTable.TryGetValue(_identifier, out MemberProperty property))
            {
                try
                {
                    return property.Get(self);
                }
                catch (Exception ex)
                {
                    throw new RuntimeException(Location,
                        $"{self.TypeClass.FullName}.{_identifier}<get>()\n  --> {ex.Message}", ex);
                }
            }
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

        if (instance.ValueType != SealValueType.Object)
        {
            throw new RuntimeException(Location,
                $"Cannot set field in non-object type {instance.Class}.");
        }

        SealObject self = instance.AsSealObject();

        if (!self.TypeClass.FieldTable.TryGetValue(_identifier, out MemberProperty property))
        {
            throw new RuntimeException(Location,
                $"Class {self.TypeClass} does not contain member field '{_identifier}'.");
        }
        
        try
        {
            property.Set(self, value);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(Location,
                $"{self.TypeClass.FullName}.{_identifier}<set>({value.ToString(true)})\n  --> {ex.Message}", ex);
        }
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