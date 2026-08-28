using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class WhileStatement : BlockStatement
{
    public WhileStatement(
        SourceLocation location,
        Statement[] statements,
        Expression condition)
    : base(location, statements)
    {
        Condition = condition;
    }
    
    public Expression Condition { get; }

    public override ReturnValue Invoke(SealAssembly assembly, Variable[] variables)
    {
        while (Condition.Evaluate(assembly, variables).InterpretAsBool())
        {
            for (int i = 0; i < Statements.Length; i++)
            {
                ReturnValue returnValue = Statements[i].Invoke(assembly, variables);

                switch (returnValue.ReturnValueType)
                {
                case ReturnValueType.Return:
                    return returnValue;
                case ReturnValueType.Break:
                    return ReturnValue.None;
                case ReturnValueType.Continue:
                    i = Statements.Length; // skip to end
                    break;
                }
            }
        }
        
        return ReturnValue.None;
    }
    
    public override void Append(StringBuilder sb, int level)
    {
        sb.Append("while ");
        sb.Append(Condition);
        sb.AppendLine(" {");

        AppendStatements(sb, level + 1);
        
        sb.Append(' ', level * LevelSize);
        sb.Append('}');
    }
}