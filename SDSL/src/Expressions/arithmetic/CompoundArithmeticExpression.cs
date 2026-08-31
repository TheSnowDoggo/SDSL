namespace SDSL.Expressions;

public class CompoundArithmeticExpression : AssignExpression
{
    public CompoundArithmeticExpression(
        SourceLocation location,
        TokenType operatorType,
        AssignableExpression left,
        Expression right)
    : base(location, left, right)
    {
        OperatorType = operatorType;
    }
    
    public TokenType OperatorType { get; }
   
    public override SealValue Evaluate(Variable[] variables)
    {
        SealValue value = Arithmetic.Evaluate(
            OperatorType,
            Location,
            Left.Evaluate(variables),
            Right.Evaluate(variables)
        );
        
        Left.SetValue(variables, value);
        
        return value;
    }

    public override string ToString()
    {
        return $"{OperatorType}({Left}, {Right})";
    }
}