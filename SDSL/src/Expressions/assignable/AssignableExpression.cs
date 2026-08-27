namespace SDSL.Expressions;

public abstract class AssignableExpression : Expression
{
    public abstract void SetValue(SealAssembly assembly, Variable[] variables, SealValue value);
}