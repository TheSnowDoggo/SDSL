namespace SDSL.Expressions;

public class ConstructorExpression : InvokeExpression
{
    public ConstructorExpression(
        SourceLocation location,
        SealClass sClass,
        Expression[] argumentExpressions)
    {
        Location = location;
        Class = sClass;
        ArgumentExpressions = argumentExpressions;
    }
    
    public SealClass Class { get; }

    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        if (Class.Constructor == null)
            throw new LangException(Location,
                $"Class {Class} is not a constructable type.");
        
        return Class.Constructor.Invoke(EvaluateArgs(assembly, variables));
    }

    public override string ToString()
    {
        return $"new {Class}({string.Join<Expression>(", ", ArgumentExpressions)})";
    }
}