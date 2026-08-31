namespace SDSL.Expressions;

public class LiteralExpression : Expression
{
    private readonly SealValue _value;
    
    public LiteralExpression(
        SourceLocation location,
        SealValue value)
    {
        Location = location;
        _value = value;
    }

    public static readonly LiteralExpression Nil = new LiteralExpression(
        SourceLocation.Invalid, 
        SealValue.Nil
    );
    
    public override SealValue Evaluate(Variable[] variables)
    {
        return _value;
    }

    public override bool IsConstantEval()
    {
        return true;
    }

    public override string ToString()
    {
        return _value.ToString(false);
    }
}