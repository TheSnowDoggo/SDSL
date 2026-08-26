using System.Text;

namespace SDSL.Prototypes;

public class PrototypeConstructor
{
    public PrototypeConstructor(
        PrototypeArgList argList,
        ArraySegment<Token> tokens)
    {
        ArgList = argList;
        Tokens = tokens;
    }
    
    public PrototypeArgList ArgList { get; }
    public ArraySegment<Token> Tokens { get; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append("new(");
        sb.AppendJoin<PrototypeArg>(", ", ArgList.Args);
        sb.Append(')');
        
        return sb.ToString();
    }
}