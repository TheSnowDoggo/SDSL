namespace SDSL.Statements;

public class ControlStatement : Statement
{
    private readonly ReturnValue _returnValue;
    
    public ControlStatement(
        SourceLocation location,
        ReturnValue returnValue)
    {
        Location = location;
        _returnValue = returnValue;
    }

    public override ReturnValue Invoke(Variable[] variables)
    {
        return _returnValue;
    }
}