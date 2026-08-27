namespace SDSL.Expressions;

public class ArithmeticExpression : Expression
{
    public ArithmeticExpression(
        SourceLocation location,
        ArithmeticOperatorType operatorType,
        Expression left,
        Expression right)
    {
        Location = location;
        OperatorType = operatorType;
        Left = left;
        Right = right;
    }
    
    public ArithmeticOperatorType OperatorType { get; }
    public Expression Left { get; }
    public Expression Right { get; }

    public static bool TryParseOperatorType(TokenType tokenType, out ArithmeticOperatorType operatorType)
    {
        const ArithmeticOperatorType Invalid = (ArithmeticOperatorType)(-1);
        
        operatorType = tokenType switch
        {
            TokenType.Multiply => ArithmeticOperatorType.Multiply,
            TokenType.Divide   => ArithmeticOperatorType.Divide,
            TokenType.IDivide  => ArithmeticOperatorType.IDivide,
            TokenType.Modulo   => ArithmeticOperatorType.Modulo,
            TokenType.Add      => ArithmeticOperatorType.Add,
            TokenType.Subtract => ArithmeticOperatorType.Subtract,
            TokenType.And      => ArithmeticOperatorType.And,
            TokenType.Xor      => ArithmeticOperatorType.Xor,
            TokenType.Or       => ArithmeticOperatorType.Or,
            _  => Invalid
        };
        
        return operatorType != Invalid;
    }

    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        return Arithmetic.Evaluate(
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