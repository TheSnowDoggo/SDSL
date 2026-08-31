namespace SDSL.Expressions;

public abstract class InvokeExpression : Expression
{
    public Expression[] ArgumentExpressions { get; init; }

    public override bool IsConstantEval()
    {
        return false;
    }

    protected SealValue[] EvaluateArgs(Variable[] variables)
    {
        int length = ArgumentExpressions.Length;

        if (length == 0)
        {
            return [];
        }
        
        var args = new SealValue[length];

        for (int i = 0; i < length; i++)
        {
            args[i] = ArgumentExpressions[i].Evaluate(variables);
        }

        return args;
    }
}