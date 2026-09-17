using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class DefineStatement : Statement
{
    private readonly int _index;
    private readonly string _identifer;
    private readonly VariantClass _variableClass;
    private readonly Expression _expression;

    public DefineStatement(
        SourceLocation location,
        int index,
        string identifier,
        VariantClass variableClass,
        Expression expression)
    {
        Location = location;
        _index = index;
        _identifer = identifier;
        _variableClass = variableClass;
        _expression = expression;
    }
    
    public override ReturnValue Invoke(Variable[] variables)
    {
        Variant defaultValue;

        try
        {
            defaultValue = _expression.Evaluate(variables);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(Location, 
                $"Failed to initialize local variable '{_identifer}'.\n  --> {ex.Message}", ex);
        }

        VariantClass sClass = _variableClass;

        if (_variableClass == IncompleteClass.Implicit)
        {
            sClass = defaultValue.Class;
        }
        else if (!defaultValue.IsAssignableTo(_variableClass))
        {
            throw new RuntimeException(Location,
                $"Value of type {defaultValue.Class} is not assignable to variable '{_identifer}' of type {_variableClass}.");
        }
        
        variables[_index] = new Variable(sClass, defaultValue);
        
        return ReturnValue.None;
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        
        sb.Append("var ");

        sb.Append(_identifer);

        if (_variableClass != null)
        {
            sb.Append(": ");
            sb.Append(_variableClass);
        }

        if (_expression != null)
        {
            sb.Append(" = ");
            sb.Append(_expression);
        }

        sb.Append(';');
        
        return sb.ToString();
    }
}