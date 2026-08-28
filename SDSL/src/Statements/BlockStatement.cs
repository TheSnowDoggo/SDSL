using System.Text;

namespace SDSL.Statements;

public class BlockStatement : Statement
{
    public const int LevelSize = 4;
    
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

    public override string ToString()
    {
        var sb = new StringBuilder();
        Append(sb, 0, true);
        return sb.ToString();
    }

    public virtual void Append(StringBuilder sb, int level, bool isStandalone)
    {
        if (isStandalone)
            sb.Append(' ', level * LevelSize);
        
        sb.AppendLine("{");
        
        AppendStatements(sb, level + 1);
        
        sb.Append(' ', level * LevelSize);
        sb.Append('}');
    }

    protected void AppendStatements(StringBuilder sb, int level)
    {
        for (int i = 0; i < Statements.Length; i++)
        {
            Statement statement = Statements[i];
            
            if (statement is BlockStatement blockStatement)
            {
                blockStatement.Append(sb, level, true);
            }
            else
            {
                sb.Append(' ', level * LevelSize);
                sb.Append(statement);
            }
            
            sb.AppendLine();
        }
    }
}