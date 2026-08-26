using SDSL.Expressions;

namespace SDSL.Statements;

public class DefineStatement : Statement
{
    private readonly int _location;
    private readonly SealClass _class;
    private readonly bool _isConst;

    public DefineStatement(
        int location,
        SealClass @class,
        bool isConst)
    {
        _location = location;
        _class = @class;
        _isConst = isConst;
    }
    
    public override ReturnValue Invoke(SealAssembly assembly, Variable[] variables)
    {
        variables[_location] = new Variable(_class, _isConst);
        return ReturnValue.None;
    }
}