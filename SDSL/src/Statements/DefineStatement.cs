using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class DefineStatement : Statement
{
    private readonly int _refLocation;
    private readonly SealClass _class;
    private readonly bool _isConst;
    private readonly Expression _expression;

    public DefineStatement(
        SourceLocation location,
        int refLocation,
        SealClass @class,
        bool isConst,
        Expression expression = null)
    {
        Location = location;
        _refLocation = refLocation;
        _class = @class;
        _isConst = isConst;
        _expression = expression;
    }
    
    public override ReturnValue Invoke(SealAssembly assembly, Variable[] variables)
    {
        SealValue defaultValue = SealValue.Nil;

        if (_expression != null)
        {
            defaultValue = _expression.Evaluate(assembly, variables);
        }
        
        variables[_refLocation] = new Variable(_class, _isConst, defaultValue);
        
        return ReturnValue.None;
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        
        sb.Append(_isConst ? "const " : "var ");

        sb.Append("Local[");
        sb.Append(_refLocation);
        sb.Append(']');

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