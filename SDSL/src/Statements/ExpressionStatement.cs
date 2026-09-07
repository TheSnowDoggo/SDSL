using SDSL.Expressions;

namespace SDSL.Statements;

public class ExpressionStatement : Statement
{
    private readonly Expression _expression;
    
    public ExpressionStatement(
        SourceLocation location,
        Expression expression)
    {
        Location = location;
        _expression = expression;
    }
    
    public override ReturnValue Invoke(Variable[] variables)
    {
        _expression.Evaluate(variables);
        
        return ReturnValue.None;
    }
    
    public override string ToString()
    {
        return $"{_expression};";
    }
}