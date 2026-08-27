namespace SDSL.Expressions;

public class StaticInvokeExpression : Expression
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
    public Expression[] ArgumentExpressions { get; }
    
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        SealValue value = FunctionExpression.Evaluate(assembly, variables);

        if (value.Class != SealClass.Function)
        {
            throw new LangException(Location,
                $"Cannot invoke non-invokable class {value.Class}.");
        }
        
        Function function = value.AsFunction();
        
        int length = ArgumentExpressions.Length;

        var args = new SealValue[length];
        for (int i = 0; i < length; i++)
            args[i] = ArgumentExpressions[i].Evaluate(assembly, variables);

        return function.Invoke(args);
    }
    
    public override string ToString()
    {
        return $"{FunctionExpression}({string.Join<Expression>(", ", ArgumentExpressions)})";
    }
}