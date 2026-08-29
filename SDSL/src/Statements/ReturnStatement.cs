using SDSL.Expressions;

namespace SDSL.Statements;

public class ReturnStatement : Statement
{
    public ReturnStatement(
        SourceLocation location,
        Expression expression)
    {
        Location = location;
        Expression = expression;
    }
    
    public Expression Expression { get; }
    
    public override ReturnValue Invoke(Variable[] variables)
    {
        SealValue value = Expression.Evaluate(variables);
        
        return new ReturnValue(ReturnValueType.Return, value);
    }

    public override string ToString()
    {
        return $"return {Expression};";
    }
}