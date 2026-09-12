using SDSL.Classes;
using SDSL.Functions;

namespace SDSL.Expressions;

public class MemberInvokeExpression : InvokeExpression
{
    public MemberInvokeExpression(
        SourceLocation location,
        Expression[] argumentExpressions,
        MemberExpression memberExpression)
        : base(argumentExpressions)
    {
        Location = location;
        MemberExpression = memberExpression;
    }
    
    public MemberExpression MemberExpression { get; }
    
    public override SealValue Evaluate(Variable[] variables)
    {
        SealValue value = MemberExpression.GetValue(variables, out SealValue instance);

        if (value.ValueType != SealValueType.Function)
        {
            throw new RuntimeException(Location,
                $"Cannot invoke non-invokable type {value.ValueType}.");
        }
        
        Function function = value.AsFunction();
        
        SealValue[] args = EvaluateArgs(variables);

        try
        {
            return function.MemberInvoke(instance, args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(Location, 
                $"{SealString.FormatMemberInvokeFail(function, instance, args)}\n  --> {ex.Message}", ex);
        }
    }

    public override string ToString()
    {
        return $"{MemberExpression}({string.Join<Expression>(", ", _argumentExpressions)})";
    }
}