using SDSL.Classes;
using SDSL.Functions;

namespace SDSL.Expressions;

public class StaticInvokeExpression : InvokeExpression
{
    public StaticInvokeExpression(
        SourceLocation location,
        Expression[] argumentExpressions,
        Expression functionExpression)
    {
        Location = location;
        ArgumentExpressions = argumentExpressions;
        FunctionExpression = functionExpression;
    }
    
    public Expression FunctionExpression { get; }
    
    public override SealValue Evaluate(Variable[] variables)
    {
        SealValue value = FunctionExpression.Evaluate(variables);

        if (value.ValueType != ValueType.Function)
        {
            throw new RuntimeException(Location,
                $"Cannot invoke non-invokable type {value.ValueType}.");
        }
        
        Function function = value.AsFunction();
        
        SealValue[] args = EvaluateArgs(variables);

        try
        {
            return function.Invoke(args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(Location, 
                $"{SealString.FormatStaticInvokeFail(function, args)}\n  --> {ex.Message}", ex);
        }
    }
    
    public override string ToString()
    {
        return $"{FunctionExpression}({string.Join<Expression>(", ", ArgumentExpressions)})";
    }
}