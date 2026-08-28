namespace SDSL.Expressions;

public class StaticInvokeExpression : InvokeExpression
{
    public StaticInvokeExpression(
        SourceLocation location,
        Expression[] argumentExpressions,
        Expression functionExpression)
    {
        Location = location;
        ArgumentExpressions = argumentExpressions;
        FunctionExpression = functionExpression;
    }
    
    public Expression FunctionExpression { get; }
    
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        SealValue value = FunctionExpression.Evaluate(assembly, variables);

        if (value.ValueType != SealValueType.Function)
            throw new LangException(Location,
                $"Cannot invoke non-invokable type {value.ValueType}.");
        
        Function function = value.AsFunction();
        
        SealValue[] args = EvaluateArgs(assembly, variables);

        return function.Invoke(args);
    }
    
    public override string ToString()
    {
        return $"{FunctionExpression}({string.Join<Expression>(", ", ArgumentExpressions)})";
    }
}