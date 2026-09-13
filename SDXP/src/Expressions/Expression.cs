namespace SDSL.Expressions;

public abstract class Expression
{
    public abstract Variant Evaluate(Variable[] variables);

    public abstract bool IsConstantEval();
}