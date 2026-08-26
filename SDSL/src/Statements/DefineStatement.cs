using SDSL.Expressions;

namespace SDSL.Statements;

public class DefineStatement : Statement
{
    private readonly int _location;
    private readonly SealClass _class;
    private readonly bool _isConst;
    private readonly PackedExpression _expression;

    public DefineStatement(
        int location,
        SealClass @class,
        bool isConst,
        PackedExpression expression = null)
    {
        _location = location;
        _class = @class;
        _isConst = isConst;
        _expression = expression;
    }
    
    public override ReturnValue Invoke(SealAssembly assembly, Variable[] variables)
    {
        SealValue defaultValue = SealValue.Nil;

        if (_expression != null)
        {
            defaultValue = _expression.Evaluate(variables);
        }
        
        variables[_location] = new Variable(_class, _isConst, defaultValue);
        
        return ReturnValue.None;
    }
}