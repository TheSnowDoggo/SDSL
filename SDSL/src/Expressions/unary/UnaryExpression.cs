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
   
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        return Unary.Evaluate(
            OperatorType,
            Location,
            Operand.Evaluate(assembly, variables)
        );
    }
}