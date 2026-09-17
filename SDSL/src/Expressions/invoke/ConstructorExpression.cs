using SDSL.Native;

namespace SDSL.Expressions;

public class ConstructorExpression : Expression
{
    private readonly VariantClass _variantClass;
    private readonly Expression[] _argumentList;
    
    public ConstructorExpression(
        VariantClass variantClass,
        Expression[] argumentList)
    {
        _variantClass = variantClass;
        _argumentList = argumentList;
    }
    
    public override Variant Evaluate(Variable[] variables)
    {
        Function constructor = _variantClass.Constructor;
        
        if (constructor == null)
        {
            throw new RuntimeException($"Class {_variantClass} is not a constructable type.");
        }

        Variant[] args = InvokeHelpers.EvaluateArgs(variables, _argumentList);
        
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
        return $"new {_variantClass}({string.Join<Expression>(", ", _argumentList)})";
    }
}