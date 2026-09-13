using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class DefineStatement : Statement
{
    private readonly int _refLocation;
    private readonly VariantClass _variantClass;
    private readonly Expression _expression;

    public DefineStatement(
        SourceLocation location,
        int refLocation,
        VariantClass variantClass,
        Expression expression)
    {
        Location = location;
        _refLocation = refLocation;
        _variantClass = variantClass;
        _expression = expression;
    }
    
    public override ReturnValue Invoke(Variable[] variables)
    {
        Variant defaultValue = _expression.Evaluate(variables);

        VariantClass sClass = _variantClass;

        if (_variantClass == ImplicitVariantClass.Instance)
        {
            sClass = defaultValue.Class;
        }
        else if (!defaultValue.IsAssignableTo(_variantClass))
        {
            throw new RuntimeException(Location,
                $"Value of type {defaultValue.Class} is not assignable to type {_variantClass}.");
        }
        
        variables[_refLocation] = new Variable(sClass, defaultValue);
        
        return ReturnValue.None;
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        
        sb.Append("var ");

        sb.Append("Local_");
        sb.Append(_refLocation);

        if (_variantClass != null)
        {
            sb.Append(": ");
            sb.Append(_variantClass);
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