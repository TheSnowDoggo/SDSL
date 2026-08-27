namespace SDSL.Expressions;

public class UnaryExpression : Expression
{
    public UnaryExpression(
        SourceLocation location,
        UnaryOperatorType operatorType,
        Expression operand)
    {
        Location = location;
        OperatorType = operatorType;
        Operand = operand;
    }
    
    public UnaryOperatorType OperatorType { get; }
    public Expression Operand { get; }
    
    public static bool TryParseOperatorType(TokenType tokenType, out UnaryOperatorType operatorType)
    {
        const UnaryOperatorType Invalid = (UnaryOperatorType)(-1);

        operatorType = tokenType switch
        {
            TokenType.Minus => UnaryOperatorType.Minus,
            TokenType.Not   => UnaryOperatorType.Not,
            _ => Invalid
        };
        
        return operatorType != Invalid;
    }

    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        return Unary.Evaluate(
            OperatorType,
            Location,
            Operand.Evaluate(assembly, variables)
        );
    }
}