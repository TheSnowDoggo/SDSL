using System.Reflection.Metadata;
using System.Text;
using SDSL.Expressions;

namespace SDSL;

public class FunctionArg
{
    public FunctionArg(
        string name,
        SealClass @class,
        Expression expression,
        bool isConst)
    {
        Name = name;
        Class = @class;
        Expression = expression;
        IsConst = isConst;
    }
    
    public string Name { get; }
    public SealClass Class { get; }
    public Expression Expression { get; }
    public bool IsConst { get; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (IsConst)
        {
            sb.Append("const ");
        }

        sb.Append(Name);
        sb.Append(": ");

        if (Class != null)
        {
            sb.Append(Class);
        }
        else
        {
            sb.Append("Any");
        }

        if (Expression != null)
        {
            sb.Append(" = ");
            sb.Append(Expression);
        }
        
        return sb.ToString();
    }
}