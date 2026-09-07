using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class IfStatement : BlockStatement
{
    private readonly Expression _condition;
    private readonly BlockStatement _elseBlock;
    
    public IfStatement(
        SourceLocation location,
        Statement[] statements,
        Expression condition,
        BlockStatement elseBlock)
    : base(location, statements)
    {
        _condition = condition;
        _elseBlock = elseBlock;
    }
    
    public override ReturnValue Invoke(Variable[] variables)
    {
        if (_condition.Evaluate(variables).ToBool())
        {
            return base.Invoke(variables);
        }

        if (_elseBlock != null)
        {
            return _elseBlock.Invoke(variables);
        }
        
        return ReturnValue.None;
    }

    public override void Append(StringBuilder sb, int level)
    {
        sb.Append("if ");
        sb.Append(_condition);
        sb.AppendLine(" {");

        AppendStatements(sb, level + 1);
        
        sb.Append(' ', level * LevelSize);
        sb.Append('}');

        if (_elseBlock != null)
        {
            sb.Append(" else ");
            _elseBlock.Append(sb, level);
        }
    }
}