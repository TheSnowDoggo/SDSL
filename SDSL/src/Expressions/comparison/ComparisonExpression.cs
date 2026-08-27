namespace SDSL.Expressions;

public class ComparisonExpression : Expression
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
    public Expression Left { get; }
    public Expression Right { get; }

    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        return Comparison.Evaluate(
            OperatorType,
            Location,
            Left.Evaluate(assembly, variables),
            Right.Evaluate(assembly, variables)
        );
    }

    public override string ToString()
    {
        return $"{OperatorType}({Left}, {Right})";
    }
}