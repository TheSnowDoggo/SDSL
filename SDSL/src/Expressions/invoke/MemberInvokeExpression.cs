namespace SDSL.Expressions;

public class MemberInvokeExpression : InvokeExpression
{
    public MemberInvokeExpression(
        SourceLocation location,
        MemberExpression memberExpression,
        Expression[] argumentExpressions)
    {
        Location = location;
        MemberExpression = memberExpression;
        ArgumentExpressions = argumentExpressions;
    }
    
    public MemberExpression MemberExpression { get; }
    
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        SealValue value = MemberExpression.GetValue(assembly, variables, out SealValue instance);

        if (value.Class != SealFunction.Class)
        {
            throw new LangException(Location,
                $"Cannot invoke non-invokable class {value.Class}.");
        }
        
        Function function = value.AsFunction();
        
        SealValue[] args = EvaluateArgs(assembly, variables);

        return function.Invoke(instance, args);
    }

    public override string ToString()
    {
        return $"{MemberExpression}({string.Join<Expression>(", ", ArgumentExpressions)})";
    }
}