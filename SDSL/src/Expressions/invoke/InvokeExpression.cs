namespace SDSL.Expressions;

public abstract class InvokeExpression : Expression
{
    protected readonly Expression[] _argumentExpressions;

    protected InvokeExpression(Expression[] argumentExpressions)
    {
        _argumentExpressions = argumentExpressions;
    }

    public override bool IsConstantEval()
    {
        return false;
    }

    protected SealValue[] EvaluateArgs(Variable[] variables)
    {
        int length = _argumentExpressions.Length;

        if (length == 0)
        {
            return [];
        }
        
        var args = new SealValue[length];

        for (int i = 0; i < length; i++)
        {
            args[i] = _argumentExpressions[i].Evaluate(variables);
        }

        return args;
    }
}