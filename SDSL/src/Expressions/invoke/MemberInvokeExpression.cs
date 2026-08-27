namespace SDSL.Expressions;

public class MemberInvokeExpression : Expression
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
    public Expression[] ArgumentExpressions { get; }
    
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        SealValue value = MemberExpression.GetValue(assembly, variables, out SealValue instance);

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

        return function.Invoke(instance, args);
    }

    public override string ToString()
    {
        return $"{MemberExpression}({string.Join<Expression>(", ", ArgumentExpressions)})";
    }
}