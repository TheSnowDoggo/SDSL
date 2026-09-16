using SDSL.Native;

namespace SDSL.Expressions;

public class IndexerExpression : AssignableExpression
{
    private const string GetterName = "_get";
    private const string SetterName = "_set";
    
    private readonly Expression[] _argumentExpressions;
    private readonly Expression _selfExpression;
    
    public IndexerExpression(
        Expression[] argumentExpressions,
        Expression selfExpression)
    {
        _argumentExpressions = argumentExpressions;
        _selfExpression = selfExpression;
    }

    public override Variant Evaluate(Variable[] variables)
    {
        Variant self = _selfExpression.Evaluate(variables);

        if (!self.Class.FunctionMap.TryGetValue(GetterName, out Function function))
        {
            throw new RuntimeException($"Class {self.Class} has no get indexer function.");
        }

        Variant[] args = EvaluateGetArgs(variables);

        try
        {
            return function.MemberInvoke(self, args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException($"{StringClass.FormatMemberInvokeFail(function, self, args)}\n  --> {ex.Message}", ex);
        }
    }
    
    public override void SetValue(Variable[] variables, Variant value)
    {
        Variant self = _selfExpression.Evaluate(variables);

        if (!self.Class.FunctionMap.TryGetValue(SetterName, out Function function))
        {
            throw new RuntimeException($"Class {self.Class} has no set indexer function.");
        }
        
        int length = _argumentExpressions.Length;
        
        var args = new Variant[length + 1];

        for (int i = 0; i < length; i++)
        {
            args[i] = _argumentExpressions[i].Evaluate(variables);
        }
        
        args[^1] = value;
        
        try
        {
            function.MemberInvoke(self, args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException($"{StringClass.FormatMemberInvokeFail(function, self, args)}\n  --> {ex.Message}", ex);
        }
    }

    public override bool IsConstantEval()
    {
        return false;
    }
    
    public override string ToString()
    {
        return $"{_selfExpression}[{string.Join<Expression>(", ", _argumentExpressions)}]";
    }

    private Variant[] EvaluateGetArgs(Variable[] variables)
    {
        int length = _argumentExpressions.Length;

        if (length == 0)
        {
            return [];
        }

        var args = new Variant[length];

        for (int i = 0; i < length; i++)
        {
            args[i] = _argumentExpressions[i].Evaluate(variables);
        }

        return args;
    }
}