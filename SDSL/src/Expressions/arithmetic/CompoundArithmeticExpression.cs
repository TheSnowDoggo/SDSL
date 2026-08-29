namespace SDSL.Expressions;

public class CompoundArithmeticExpression : Expression
{
    public CompoundArithmeticExpression(
        SourceLocation location,
        TokenType operatorType,
        AssignableExpression left,
        Expression right)
    {
        Location = location;
        OperatorType = operatorType;
        Left = left;
        Right = right;
    }
    
    public TokenType OperatorType { get; }
    public AssignableExpression Left { get; }
    public Expression Right { get; }
   
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