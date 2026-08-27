namespace SDSL.Statements;

public class BlockStatement : Statement
{
    public BlockStatement(
        SourceLocation location,
        Statement[] statements)
    {
        Location = location;
        Statements = statements;
    }
    
    public Statement[] Statements { get; }
    
    public override ReturnValue Invoke(SealAssembly assembly, Variable[] variables)
    {
        for (int i = 0; i < Statements.Length; i++)
        {
            ReturnValue returnValue = Statements[i].Invoke(assembly, variables);

            if (returnValue.ReturnValueType != ReturnValueType.None)
                return returnValue;
        }
        
        return ReturnValue.None;
    }
}