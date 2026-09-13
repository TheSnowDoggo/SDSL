namespace SDSL.Expressions;

public class ValueExpression : Expression
{
    private readonly Variant _value;
    
    public ValueExpression(
        SourceLocation location,
        Variant value)
    {
        Location = location;
        _value = value;
    }

    public static readonly ValueExpression Nil = new ValueExpression(
        SourceLocation.Invalid, 
        Variant.Nil
    );
    
    public override Variant Evaluate(Variable[] variables)
    {
        return _value;
    }

    public override bool IsConstantEval()
    {
        return true;
    }

    public override string ToString()
    {
        return _value.ToSafeString();
    }
}