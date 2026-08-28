namespace SDSL.Expressions;

public abstract class InvokeExpression : Expression
{
    public Expression[] ArgumentExpressions { get; init; }

    protected SealValue[] EvaluateArgs(SealAssembly assembly, Variable[] variables)
    {
        int length = ArgumentExpressions.Length;
        
        var args = new SealValue[length];

        for (int i = 0; i < length; i++)
        {
            args[i] = ArgumentExpressions[i].Evaluate(assembly, variables);
        }

        return args;
    }
}