namespace SDSL.Expressions;

public class ComparisonExpression : Expression
{
    public ComparisonExpression(
        SourceLocation location,
        ComparisonOperatorType operatorType,
        Expression left,
        Expression right)
    {
        Location = location;
        OperatorType = operatorType;
        Left = left;
        Right = right;
    }
    
    public ComparisonOperatorType OperatorType { get; }
    public Expression Left { get; }
    public Expression Right { get; }

    public static bool TryParseOperatorType(TokenType tokenType, out ComparisonOperatorType operatorType)
    {
        const ComparisonOperatorType Invalid = (ComparisonOperatorType)(-1);

        operatorType = tokenType switch
        {
            TokenType.LessThan           => ComparisonOperatorType.LessThan,
            TokenType.GreaterThan        => ComparisonOperatorType.GreaterThan,
            TokenType.LessThanOrEqual    => ComparisonOperatorType.LessThanOrEqual,
            TokenType.GreaterThanOrEqual => ComparisonOperatorType.GreaterThanOrEqual,
            TokenType.Equal    => ComparisonOperatorType.Equal,
            TokenType.NotEqual => ComparisonOperatorType.NotEqual,
            _ => Invalid
        };
        
        return operatorType != Invalid;
    }
    
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