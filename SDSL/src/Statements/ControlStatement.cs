namespace SDSL.Statements;

public class ControlStatement : Statement
{
    public ControlStatement(
        SourceLocation location,
        ReturnValue returnValue)
    {
        Location = location;
        ReturnValue = returnValue;
    }
    
    public ReturnValue ReturnValue { get; }

    public override ReturnValue Invoke(Variable[] variables)
    {
        return ReturnValue;
    }
}