namespace SDSL.Expressions;

public class AssignExpression : Expression
{
    public AssignExpression(
        SourceLocation location,
        AssignableExpression left,
        Expression right)
    {
        Location = location;
        Left = left;
        Right = right;
    }
    
    protected AssignableExpression Left { get; }
    protected Expression Right { get; }

    public override SealValue Evaluate(Variable[] variables)
    {
        SealValue value = Right.Evaluate(variables);

        Left.SetValue(variables, value);
        
        return value;
    }

    public override bool IsConstantEval()
    {
        return false;
    }

    public override string ToString()
    {
        return $"{Left} = {Right}";
    }
}