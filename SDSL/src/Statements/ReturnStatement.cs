using SDSL.Expressions;

namespace SDSL.Statements;

public class ReturnStatement : Statement
{
    private readonly Expression _expression;
    
    public ReturnStatement(
        SourceLocation location,
        Expression expression)
    {
        Location = location;
        _expression = expression;
    }
    
    public override ReturnValue Invoke(Variable[] variables)
    {
        SealValue value = _expression.Evaluate(variables);
        
        return new ReturnValue(ReturnValueType.Return, value);
    }

    public override string ToString()
    {
        return $"return {_expression};";
    }
}