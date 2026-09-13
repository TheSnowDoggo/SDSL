namespace SDSL.Expressions;

public class ConstructorExpression : InvokeExpression
{
    private readonly VariantClass _variantClass;
    
    public ConstructorExpression(
        SourceLocation location,
        VariantClass variantClass,
        Expression[] argumentExpressions)
        : base(argumentExpressions)
    {
        Location = location;
        _variantClass = variantClass;
    }
    
    public override Variant Evaluate(Variable[] variables)
    {
        Function constructor = _variantClass.Constructor;
        
        if (constructor == null)
        {
            throw new RuntimeException(Location, $"Class {_variantClass} is not a constructable type.");
        }

        Variant[] args = EvaluateArgs(variables);
        
        try
        {
            return constructor.StaticInvoke(args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(Location,
                $"{StringClass.FormatStaticInvokeFail(constructor, args)}\n  --> {ex.Message}", ex);
        }
    }

    public override string ToString()
    {
        return $"new {_variantClass}({string.Join<Expression>(", ", _argumentExpressions)})";
    }
}