namespace SDSL.Expressions;

public class StaticInvokeExpression : InvokeExpression
{
    public StaticInvokeExpression(
        Expression[] argumentExpressions,
        Expression functionExpression)
        : base(argumentExpressions)
    {
        FunctionExpression = functionExpression;
    }
    
    public Expression FunctionExpression { get; }
    
    public override Variant Evaluate(Variable[] variables)
    {
        Variant value = FunctionExpression.Evaluate(variables);

        if (value.TryAsVariantObject(out Function function))
        {
            throw new RuntimeException($"Cannot invoke non-invokable type {value.Class}.");
        }
        
        Variant[] args = EvaluateArgs(variables);

        try
        {
            return function.StaticInvoke(args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(
                $"{StringClass.FormatStaticInvokeFail(function, args)}\n  --> {ex.Message}", ex);
        }
    }
    
    public override string ToString()
    {
        return $"{FunctionExpression}({string.Join<Expression>(", ", _argumentExpressions)})";
    }
}