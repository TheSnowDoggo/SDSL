namespace SDSL.Expressions;

public class AssignExpression : Expression
{
    public AssignExpression(
        SourceLocation location,
        AssignableExpression assignable,
        Expression expression)
    {
        Location = location;
        Assignable = assignable;
        Expression = expression;
    }
    
    public AssignableExpression Assignable { get; }
    public Expression Expression { get; }

    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        SealValue value = Expression.Evaluate(assembly, variables);

        Assignable.SetValue(assembly, variables, value);
        
        return value;
    }
}