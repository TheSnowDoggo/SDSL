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
        try
        {
            _expression.Evaluate(variables);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(Location, 
                $"Failed to evaluate expression.\n  --> {ex.Message}", ex);
        }
        
        return ReturnValue.None;
    }
    
    public override string ToString()
    {
        return $"{_expression};";
    }
}