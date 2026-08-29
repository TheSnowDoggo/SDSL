namespace SDSL.Expressions;

public class IndexerExpression : AssignableExpression
{
    private const string GetterName = "_get";
    private const string SetterName = "_set";
    
    public IndexerExpression(
        SourceLocation location,
        Expression[] argumentExpressions,
        Expression instanceExpression)
    {
        Location = location;
        ArgumentExpressions = argumentExpressions;
        InstanceExpression = instanceExpression;
    }
    
    public Expression[] ArgumentExpressions { get; }
    public Expression InstanceExpression { get; }

    public override SealValue Evaluate(Variable[] variables)
    {
        SealValue instance = InstanceExpression.Evaluate(variables);

        if (!instance.Class.FunctionTable.TryGetValue(GetterName, out Function function))
            throw new LangException(Location,
                $"Class {instance.Class} has no get indexer function.");

        SealValue[] args = EvaluateGetArgs(variables);

        return function.MemberInvoke(instance, args);
    }

    public override void SetValue(Variable[] variables, SealValue value)
    {
        SealValue instance = InstanceExpression.Evaluate(variables);

        if (!instance.Class.FunctionTable.TryGetValue(SetterName, out Function function))
            throw new LangException(Location,
                $"Class {instance.Class} has no set indexer function.");
        
        int length = ArgumentExpressions.Length;
        
        var args = new SealValue[length + 1];
        
        for (int i = 0; i < length; i++)
            args[i] = ArgumentExpressions[i].Evaluate(variables);
        args[^1] = value;
        
        function.MemberInvoke(instance, args);
    }

    public override string ToString()
    {
        return $"{InstanceExpression}[{string.Join<Expression>(", ", ArgumentExpressions)}]";
    }

    private SealValue[] EvaluateGetArgs(Variable[] variables)
    {
        int length = ArgumentExpressions.Length;

        if (length == 0)
            return [];

        var args = new SealValue[length];
        
        for (int i = 0; i < length; i++)
            args[i] = ArgumentExpressions[i].Evaluate(variables);

        return args;
    }
}