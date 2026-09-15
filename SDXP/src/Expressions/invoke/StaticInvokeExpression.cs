namespace SDSL.Expressions;

public class StaticInvokeExpression : Expression
{
    private readonly Expression _functionExpression;
    private readonly Expression[] _argumentList;
    
    public StaticInvokeExpression(
        Expression functionExpression,
        Expression[] argumentList)
    {
        _functionExpression = functionExpression;
        _argumentList = argumentList;
    }
    
    public override Variant Evaluate(Variable[] variables)
    {
        Variant value = _functionExpression.Evaluate(variables);

        if (!value.TryAsVariantObject(out Function function))
        {
            throw new RuntimeException($"Cannot invoke non-invokable type {value.Class}.");
        }
        
        Variant[] args = InvokeHelpers.EvaluateArgs(variables, _argumentList);

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

    public override bool IsConstantEval()
    {
        return false;
    }

    public override string ToString()
    {
        return $"{_functionExpression}({string.Join<Expression>(", ", _argumentList)})";
    }
}