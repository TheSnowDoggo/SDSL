using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class IfStatement : BlockStatement
{
    public IfStatement(
        SourceLocation location,
        Statement[] statements,
        Expression condition,
        BlockStatement elseBlock)
    : base(location, statements)
    {
        Condition = condition;
        ElseBlock = elseBlock;
    }
    
    public Expression Condition { get; }
    public BlockStatement ElseBlock { get; }
    
    public override ReturnValue Invoke(SealAssembly assembly, Variable[] variables)
    {
        if (Condition.Evaluate(assembly, variables).InterpretAsBool())
        {
            return base.Invoke(assembly, variables);
        }

        if (ElseBlock != null)
        {
            return ElseBlock.Invoke(assembly, variables);
        }
        
        return ReturnValue.None;
    }

    public override void Append(StringBuilder sb, int level)
    {
        sb.Append("if ");
        sb.Append(Condition);
        sb.AppendLine(" {");

        AppendStatements(sb, level + 1);
        
        sb.Append(' ', level * LevelSize);
        sb.Append('}');

        if (ElseBlock != null)
        {
            sb.Append(" else ");
            ElseBlock.Append(sb, level);
        }
    }
}