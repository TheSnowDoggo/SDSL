namespace SDSL.Expressions;

public class MemberInvokeExpression : InvokeExpression
{
    public MemberInvokeExpression(
        SourceLocation location,
        Expression[] argumentExpressions,
        MemberExpression memberExpression)
    {
        Location = location;
        ArgumentExpressions = argumentExpressions;
        MemberExpression = memberExpression;
    }
    
    public MemberExpression MemberExpression { get; }
    
    public override SealValue Evaluate(Variable[] variables)
    {
        SealValue value = MemberExpression.GetValue(variables, out SealValue instance);

        if (value.ValueType != ValueType.Function)
            throw new LangException(Location,
                $"Cannot invoke non-invokable type {value.ValueType}.");
        
        Function function = value.AsFunction();
        
        SealValue[] args = EvaluateArgs(variables);

        return function.MemberInvoke(instance, args.AsSpan());
    }

    public override string ToString()
    {
        return $"{MemberExpression}({string.Join<Expression>(", ", ArgumentExpressions)})";
    }
}