namespace SDSL.Expressions;

public class UnaryExpression : Expression
{
    public UnaryExpression(
        SourceLocation location,
        TokenType operatorType,
        Expression operand)
    {
        Location = location;
        OperatorType = operatorType;
        Operand = operand;
    }
    
    public TokenType OperatorType { get; }
    public Expression Operand { get; }

    public override Variant Evaluate(Variable[] variables)
    {
        return Unary.Evaluate(OperatorType, Location, Operand.Evaluate(variables));
    }

    public override bool IsConstantEval()
    {
        return Operand.IsConstantEval();
    }

    public override string ToString()
    {
        return $"{OperatorType}({Operand})";
    }
}