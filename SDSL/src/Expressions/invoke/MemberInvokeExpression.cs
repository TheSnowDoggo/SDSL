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
    
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        SealValue value = MemberExpression.GetValue(assembly, variables, out SealValue instance);

        if (value.ValueType != SealValueType.Function)
            throw new LangException(Location,
                $"Cannot invoke non-invokable type {value.ValueType}.");
        
        Function function = value.AsFunction();
        
        SealValue[] args = EvaluateArgs(assembly, variables);

        return function.Invoke(instance, args);
    }

    public override string ToString()
    {
        return $"{MemberExpression}({string.Join<Expression>(", ", ArgumentExpressions)})";
    }
}