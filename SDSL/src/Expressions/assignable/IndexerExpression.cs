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

    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        SealValue instance = InstanceExpression.Evaluate(assembly, variables);

        if (!instance.Class.FunctionTable.TryGetValue(GetterName, out int location))
            throw new LangException(Location,
                $"Class {instance.Class} has no get indexer function.");
        
        Function function = assembly.Functions[location];
        
        int length = ArgumentExpressions.Length;

        var args = new SealValue[length];
        
        for (int i = 0; i < length; i++)
            args[i] = ArgumentExpressions[i].Evaluate(assembly, variables);

        return function.Invoke(instance, args);
    }

    public override void SetValue(SealAssembly assembly, Variable[] variables, SealValue value)
    {
        SealValue instance = InstanceExpression.Evaluate(assembly, variables);

        if (!instance.Class.FunctionTable.TryGetValue(SetterName, out int location))
            throw new LangException(Location,
                $"Class {instance.Class} has no set indexer function.");
        
        Function function = assembly.Functions[location];

        int length = ArgumentExpressions.Length;
        
        var args = new SealValue[length + 1];
        
        for (int i = 0; i < length; i++)
            args[i] = ArgumentExpressions[i].Evaluate(assembly, variables);
        args[^1] = value;
        
        function.Invoke(instance, args);
    }

    public override string ToString()
    {
        return $"{InstanceExpression}[{string.Join<Expression>(", ", ArgumentExpressions)}]";
    }
}