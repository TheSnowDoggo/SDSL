namespace SDSL.Expressions;

public class ArithmeticExpression : Expression
{
    public ArithmeticExpression(
        SourceLocation location,
        TokenType operatorType,
        Expression left,
        Expression right)
    {
        Location = location;
        OperatorType = operatorType;
        Left = left;
        Right = right;
    }
    
    public TokenType OperatorType { get; }
    public Expression Left { get; }
    public Expression Right { get; }

    public override SealValue Evaluate(Variable[] variables)
    {
        return Arithmetic.Evaluate(
            OperatorType,
            Location,
            Left.Evaluate(variables),
            Right.Evaluate(variables)
        );
    }

    public override string ToString()
    {
        return $"{OperatorType}({Left}, {Right})";
    }
}