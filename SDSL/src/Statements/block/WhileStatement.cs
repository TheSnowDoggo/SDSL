using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class WhileStatement : BlockStatement
{
    private readonly Expression _condition;
    
    public WhileStatement(
        SourceLocation location,
        Statement[] statements,
        Expression condition)
    : base(location, statements)
    {
        _condition = condition;
    }

    public override ReturnValue Invoke(Variable[] variables)
    {
        while (_condition.Evaluate(variables).ToBool())
        {
            for (int i = 0; i < _statements.Length; i++)
            {
                ReturnValue returnValue = _statements[i].Invoke(variables);

                switch (returnValue.ReturnValueType)
                {
                case ReturnValueType.Return:
                    return returnValue;
                case ReturnValueType.Break:
                    return ReturnValue.None;
                case ReturnValueType.Continue:
                    i = _statements.Length; // skip to end
                    break;
                }
            }
        }
        
        return ReturnValue.None;
    }
    
    public override void Append(StringBuilder sb, int level)
    {
        sb.Append("while ");
        sb.Append(_condition);
        sb.AppendLine(" {");

        AppendStatements(sb, level + 1);
        
        sb.Append(' ', level * LevelSize);
        sb.Append('}');
    }
}