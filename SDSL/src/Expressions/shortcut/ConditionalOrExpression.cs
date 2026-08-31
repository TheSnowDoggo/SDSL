namespace SDSL.Expressions;

public class ConditionalOrExpression : Expression
{
    public ConditionalOrExpression(
        SourceLocation location,
        Expression left,
        Expression right)
    {
        Location = location;
        Left = left;
        Right = right;
    }
    
    public Expression Left { get; }
    public Expression Right { get; }

    public override SealValue Evaluate(Variable[] variables)
    {
        return Left.Evaluate(variables).ToBool() || Right.Evaluate(variables).ToBool();
    }
    
    public override string ToString()
    {
        return $"||({Left}, {Right})";
    }
}