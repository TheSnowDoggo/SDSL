using SDSL.Classes;

namespace SDSL.Expressions;

public class ConstructorExpression : InvokeExpression
{
    public ConstructorExpression(
        SourceLocation location,
        SealClass sClass,
        Expression[] argumentExpressions)
        : base(argumentExpressions)
    {
        Location = location;
        Class = sClass;
    }
    
    public SealClass Class { get; }

    public override SealValue Evaluate(Variable[] variables)
    {
        if (Class.Constructor == null)
        {
            throw new RuntimeException(Location, $"Class {Class} is not a constructable type.");
        }

        SealValue[] args = EvaluateArgs(variables);
        
        try
        {
            return Class.Constructor.Invoke(args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(Location,
                $"{SealString.FormatStaticInvokeFail(Class.Constructor, args)}\n  --> {ex.Message}", ex);
        }
    }

    public override string ToString()
    {
        return $"new {Class}({string.Join<Expression>(", ", _argumentExpressions)})";
    }
}