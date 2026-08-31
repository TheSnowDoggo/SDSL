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
    
    public AssignableExpression Left { get; }
    public Expression Right { get; }

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