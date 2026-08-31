using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class DefineStatement : Statement
{
    private readonly int _refLocation;
    private readonly SealClass _class;
    private readonly Expression _expression;

    public DefineStatement(
        SourceLocation location,
        int refLocation,
        SealClass pClass,
        Expression expression = null)
    {
        Location = location;
        _refLocation = refLocation;
        _class = pClass;
        _expression = expression;
    }
    
    public override ReturnValue Invoke(Variable[] variables)
    {
        SealValue defaultValue = _expression == null
            ? SealClass.GetDefaultValue(_class)
            : _expression.Evaluate(variables);
        
        variables[_refLocation] = new Variable(_class, defaultValue);
        
        return ReturnValue.None;
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        
        sb.Append("var ");

        sb.Append("Local_");
        sb.Append(_refLocation);

        if (_class != null)
        {
            sb.Append(": ");
            sb.Append(_class);
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