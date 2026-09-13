using System.Text;

namespace SDSL.Statements;

public class BlockStatement : Statement
{
    protected const int LevelSize = 4;
    
    protected readonly Statement[] _statements;
    
    public BlockStatement(
        SourceLocation location,
        Statement[] statements)
    {
        Location = location;
        _statements = statements;
    }
    
    public override ReturnValue Invoke(Variable[] variables)
    {
        for (int i = 0; i < _statements.Length; i++)
        {
            ReturnValue returnValue = _statements[i].Invoke(variables);

            if (returnValue.ReturnValueType != ReturnValueType.None)
                return returnValue;
        }
        
        return ReturnValue.None;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        
        Append(sb, 0);
        
        return sb.ToString();
    }

    public virtual void Append(StringBuilder sb, int level)
    {
        sb.AppendLine("{");
        
        AppendStatements(sb, level + 1);
        
        sb.Append(' ', level * LevelSize);
        sb.Append('}');
    }

    protected void AppendStatements(StringBuilder sb, int level)
    {
        for (int i = 0; i < _statements.Length; i++)
        {
            Statement statement = _statements[i];
            
            sb.Append(' ', level * LevelSize);
            
            if (statement is BlockStatement blockStatement)
            {
                blockStatement.Append(sb, level);
            }
            else
            {
                sb.Append(statement);
            }
            
            sb.AppendLine();
        }
    }
}