namespace SDSL.Expressions;

public class UnaryExpression : Expression
{
    public UnaryExpression(
        TokenType operatorType,
        Expression operand)
    {
        OperatorType = operatorType;
        Operand = operand;
    }
    
    public TokenType OperatorType { get; }
    public Expression Operand { get; }

    public override Variant Evaluate(Variable[] variables)
    {
        return Unary.Evaluate(OperatorType, Operand.Evaluate(variables));
    }

    public override bool IsConstantExpression()
    {
        return Operand.IsConstantExpression();
    }

    public override string ToString()
    {
        return $"{OperatorType}({Operand})";
    }
}