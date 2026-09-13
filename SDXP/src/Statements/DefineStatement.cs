using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class DefineStatement : Statement
{
    private readonly int _index;
    private readonly VariantClass _variableClass;
    private readonly Expression _expression;

    public DefineStatement(
        SourceLocation location,
        int index,
        VariantClass variableClass,
        Expression expression)
    {
        Location = location;
        _index = index;
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
                $"Failed to initialize Local_{_index}.\n  --> {ex.Message}", ex);
        }

        VariantClass sClass = _variableClass;

        if (_variableClass == ImplicitVariantClass.Instance)
        {
            sClass = defaultValue.Class;
        }
        else if (!defaultValue.IsAssignableTo(_variableClass))
        {
            throw new RuntimeException(Location,
                $"Value of type {defaultValue.Class} is not assignable to type {_variableClass}.");
        }
        
        variables[_index] = new Variable(sClass, defaultValue);
        
        return ReturnValue.None;
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        
        sb.Append("var ");

        sb.Append("Local_");
        sb.Append(_index);

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