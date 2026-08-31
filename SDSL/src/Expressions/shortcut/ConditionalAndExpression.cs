namespace SDSL.Expressions;

public class ConditionalAndExpression : BinaryExpression
{
    public ConditionalAndExpression(
        SourceLocation location,
        Expression left,
        Expression right)
    {
        Location = location;
        Left = left;
        Right = right;
    }

    public override SealValue Evaluate(Variable[] variables)
    {
        return Left.Evaluate(variables).ToBool() && Right.Evaluate(variables).ToBool();
    }

    public override string ToString()
    {
        return $"&&({Left}, {Right})";
    }
}