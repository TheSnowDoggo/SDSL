namespace SDSL.Expressions;

public class ComparisonExpression : BinaryExpression
{
    public ComparisonExpression(
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
    
    public override Variant Evaluate(Variable[] variables)
    {
        return Comparison.Evaluate(
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