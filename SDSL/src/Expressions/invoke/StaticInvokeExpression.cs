namespace SDSL.Expressions;

public class StaticInvokeExpression : InvokeExpression
{
    public StaticInvokeExpression(
        SourceLocation location,
        Expression functionExpression,
        Expression[] argumentExpressions)
    {
        Location = location;
        FunctionExpression = functionExpression;
        ArgumentExpressions = argumentExpressions;
    }
    
    public Expression FunctionExpression { get; }
    
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        SealValue value = FunctionExpression.Evaluate(assembly, variables);

        if (value.Class != SealClass.Function)
        {
            throw new LangException(Location,
                $"Cannot invoke non-invokable class {value.Class}.");
        }
        
        Function function = value.AsFunction();
        
        SealValue[] args = EvaluateArgs(assembly, variables);

        return function.Invoke(args);
    }
    
    public override string ToString()
    {
        return $"{FunctionExpression}({string.Join<Expression>(", ", ArgumentExpressions)})";
    }
}