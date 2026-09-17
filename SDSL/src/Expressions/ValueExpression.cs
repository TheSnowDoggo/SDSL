namespace SDSL.Expressions;

public class ValueExpression : Expression
{
    private readonly Variant _value;
    
    public ValueExpression(
        Variant value)
    {
        _value = value;
    }

    public static readonly ValueExpression Nil = new ValueExpression(Variant.Nil);
    
    public override Variant Evaluate(Variable[] variables)
    {
        return _value;
    }

    public override bool IsConstantExpression()
    {
        // Prevents overloaded functions from being executed in a const context.
        return _value.VariantType != VariantType.Object;
    }

    public override string ToString()
    {
        return _value.ToString();
    }
}