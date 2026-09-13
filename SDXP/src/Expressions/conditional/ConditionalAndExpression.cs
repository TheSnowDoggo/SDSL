namespace SDSL.Expressions;

public class ConditionalAndExpression : BinaryExpression
{
    public ConditionalAndExpression(
        Expression left,
        Expression right)
    {
        Left = left;
        Right = right;
    }

    public override Variant Evaluate(Variable[] variables)
    {
        return Left.Evaluate(variables).ToBool() && Right.Evaluate(variables).ToBool();
    }

    public override string ToString()
    {
        return $"&&({Left}, {Right})";
    }
}