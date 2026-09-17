namespace SDSL.Expressions;

public abstract class Expression
{
    public abstract Variant Evaluate(Variable[] variables);

    public virtual bool IsConstantExpression()
    {
        return false;
    }
}