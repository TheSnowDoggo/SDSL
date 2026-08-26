namespace SDSL.Expressions;

public class LiteralExpression : Expression
{
    private readonly SealValue _value;
    
    public LiteralExpression(SealValue value)
    {
        _value = value;
    }
    
    public override SealValue Evaluate(SourceLocation error, SealAssembly assembly, Variable[] variables)
    {
        return _value;
    }
}