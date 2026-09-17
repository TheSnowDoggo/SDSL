namespace SDSL.Expressions;

public class ConditionalOrExpression : BinaryExpression
{
    public ConditionalOrExpression(
        Expression left,
        Expression right)
    {
        Left = left;
        Right = right;
    }

    public override Variant Evaluate(Variable[] variables)
    {
        return Left.Evaluate(variables).ToBoolVolatile() || Right.Evaluate(variables).ToBoolVolatile();
    }
    
    public override string ToString()
    {
        return $"||({Left}, {Right})";
    }
}