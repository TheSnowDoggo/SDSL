using SDSL.Native;

namespace SDSL.Expressions;

public class MemberInvokeExpression : Expression
{
    private readonly IMemberFunctionExpression _functionExpression;
    private readonly Expression[] _argumentList;
    
    public MemberInvokeExpression(
        IMemberFunctionExpression functionExpression,
        Expression[] argumentList)
    {
        _functionExpression = functionExpression;
        _argumentList = argumentList;
    }
    
    public override Variant Evaluate(Variable[] variables)
    {
        (Variant self, Function function) = _functionExpression.GetFunctionInfo(variables);

        Variant[] args = InvokeHelpers.EvaluateArgs(variables, _argumentList);

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
        return $"{_functionExpression}({string.Join<Expression>(", ", _argumentList)})";
    }
}