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
    
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        return _value;
    }

    public override string ToString()
    {
        return _value.ToString();
    }
}