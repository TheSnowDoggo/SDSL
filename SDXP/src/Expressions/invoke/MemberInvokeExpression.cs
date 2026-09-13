namespace SDSL.Expressions;

public class MemberInvokeExpression : InvokeExpression
{
    private readonly IMemberFunctionExpression _functionExpression;
    
    public MemberInvokeExpression(
        Expression[] argumentExpressions,
        IMemberFunctionExpression functionExpression)
        : base(argumentExpressions)
    {
        _functionExpression = functionExpression;
    }
    
    public override Variant Evaluate(Variable[] variables)
    {
        (Variant self, Function function) = _functionExpression.GetFunctionInfo(variables);

        Variant[] args = EvaluateArgs(variables);

        try
        {
            return function.MemberInvoke(self, args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(
                $"{StringClass.FormatMemberInvokeFail(function, self, args)}\n  --> {ex.Message}", ex);
        }
    }

    public override string ToString()
    {
        return $"{_functionExpression}({string.Join<Expression>(", ", _argumentExpressions)})";
    }
}