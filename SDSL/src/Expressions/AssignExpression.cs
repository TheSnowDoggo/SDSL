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

    public override SealValue Evaluate(Variable[] variables)
    {
        SealValue value = Expression.Evaluate(variables);

        Assignable.SetValue(variables, value);
        
        return value;
    }

    public override string ToString()
    {
        return $"{Assignable} = {Expression}";
    }
}