using SDSL.Classes;
using SDSL.Functions;

namespace SDSL.Expressions;

public class StaticInvokeExpression : InvokeExpression
{
    public StaticInvokeExpression(
        SourceLocation location,
        Expression[] argumentExpressions,
        Expression functionExpression)
        : base(argumentExpressions)
    {
        Location = location;
        FunctionExpression = functionExpression;
    }
    
    public Expression FunctionExpression { get; }
    
    public override Variant Evaluate(Variable[] variables)
    {
        Variant value = FunctionExpression.Evaluate(variables);

        if (value.ValueType != SealValueType.Function)
        {
            throw new RuntimeException(Location,
                $"Cannot invoke non-invokable type {value.ValueType}.");
        }
        
        Function function = value.AsFunction();
        
        Variant[] args = EvaluateArgs(variables);

        try
        {
            return function.Invoke(args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(Location, 
                $"{StringClass.FormatStaticInvokeFail(function, args)}\n  --> {ex.Message}", ex);
        }
    }
    
    public override string ToString()
    {
        return $"{FunctionExpression}({string.Join<Expression>(", ", _argumentExpressions)})";
    }
}