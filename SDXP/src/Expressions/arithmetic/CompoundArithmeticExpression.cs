namespace SDSL.Expressions;

public class CompoundArithmeticExpression : AssignExpression
{
    public CompoundArithmeticExpression(
        TokenType operatorType,
        AssignableExpression left,
        Expression right)
    : base(left, right)
    {
        OperatorType = operatorType;
    }
    
    public TokenType OperatorType { get; }
   
    public override Variant Evaluate(Variable[] variables)
    {
        Variant value = Arithmetic.Evaluate(
            OperatorType,
            _left.Evaluate(variables),
            _right.Evaluate(variables)
        );
        
        _left.SetValue(variables, value);
        
        return value;
    }

    public override string ToString()
    {
        return $"{OperatorType}({_left}, {_right})";
    }
}