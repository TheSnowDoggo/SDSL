namespace SDSL.Expressions;

public abstract class AssignableExpression : Expression
{
    public abstract void SetValue(Variable[] variables, SealValue value);
}