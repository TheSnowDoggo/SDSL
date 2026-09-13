namespace SDSL.Expressions;

public class ArithmeticExpression : BinaryExpression
{
    public ArithmeticExpression(
        TokenType operatorType,
        Expression left,
        Expression right)
    {
        OperatorType = operatorType;
        Left = left;
        Right = right;
    }
    
    public TokenType OperatorType { get; }

    public override Variant Evaluate(Variable[] variables)
    {
        return Arithmetic.Evaluate(
            OperatorType,
            Left.Evaluate(variables),
            Right.Evaluate(variables)
        );
    }

    public override string ToString()
    {
        return $"{OperatorType}({Left}, {Right})";
    }
}