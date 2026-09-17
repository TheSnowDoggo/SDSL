namespace SDSL.Expressions;

public class AssignExpression : Expression
{
    protected readonly AssignableExpression _left;
    protected readonly Expression _right;
    
    public AssignExpression(
        AssignableExpression left,
        Expression right)
    {
        _left = left;
        _right = right;
    }

    public override Variant Evaluate(Variable[] variables)
    {
        Variant value = _right.Evaluate(variables);

        _left.SetValue(variables, value);
        
        return value;
    }

    public override string ToString()
    {
        return $"{_left} = {_right}";
    }
}