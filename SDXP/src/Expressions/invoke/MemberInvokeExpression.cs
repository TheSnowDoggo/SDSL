using SDSL.Classes;
using SDSL.Functions;

namespace SDSL.Expressions;

public class MemberInvokeExpression : InvokeExpression
{
    private readonly InstanceFunctionExpression _functionExpression;
    
    public MemberInvokeExpression(
        SourceLocation location,
        Expression[] argumentExpressions,
        InstanceFunctionExpression functionExpression)
        : base(argumentExpressions)
    {
        Location = location;
        _functionExpression = functionExpression;
    }
    
    public override Variant Evaluate(Variable[] variables)
    {
        Function function = _functionExpression.Function;

        Variant self = _functionExpression.GetInstance(variables);
        
        Variant[] args = EvaluateArgs(variables);

        try
        {
            return function.MemberInvoke(self, args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(Location, 
                $"{StringClass.FormatMemberInvokeFail(function, self, args)}\n  --> {ex.Message}", ex);
        }
    }

    public override string ToString()
    {
        return $"{_functionExpression}({string.Join<Expression>(", ", _argumentExpressions)})";
    }
}