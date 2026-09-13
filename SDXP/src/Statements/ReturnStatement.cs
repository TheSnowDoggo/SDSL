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
        Variant value;

        try
        {
            value = _expression.Evaluate(variables);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(Location, 
                $"Failed to evaluate return expression.\n  --> {ex.Message}", ex);
        }
        
        return new ReturnValue(ReturnValueType.Return, value);
    }

    public override string ToString()
    {
        return $"return {_expression};";
    }
}