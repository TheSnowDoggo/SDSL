namespace SDSL.Expressions;

public class ConstructorExpression : InvokeExpression
{
    private readonly VariantClass _variantClass;
    
    public ConstructorExpression(
        VariantClass variantClass,
        Expression[] argumentExpressions)
        : base(argumentExpressions)
    {
        _variantClass = variantClass;
    }
    
    public override Variant Evaluate(Variable[] variables)
    {
        Function constructor = _variantClass.Constructor;
        
        if (constructor == null)
        {
            throw new RuntimeException($"Class {_variantClass} is not a constructable type.");
        }

        Variant[] args = EvaluateArgs(variables);
        
        try
        {
            return constructor.StaticInvoke(args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(
                $"{StringClass.FormatStaticInvokeFail(constructor, args)}\n  --> {ex.Message}", ex);
        }
    }

    public override string ToString()
    {
        return $"new {_variantClass}({string.Join<Expression>(", ", _argumentExpressions)})";
    }
}