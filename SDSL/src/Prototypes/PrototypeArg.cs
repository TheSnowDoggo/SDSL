using System.Text;

namespace SDSL.Prototypes;

public class PrototypeArg
{
    public PrototypeArg(
        string name,
        PrototypeDataType dataType,
        ArraySegment<Token> tokens,
        bool isConst)
    {
        Name = name;
        DataType = dataType;
        Tokens = tokens;
        IsConst = isConst;
    }
    
    public string Name { get; }
    public PrototypeDataType DataType { get; }
    public ArraySegment<Token> Tokens { get; }
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
        sb.Append(DataType);

        if (Tokens.Count != 0)
        {
            sb.Append(" = ");
            sb.Append($"Expression[{Tokens.Count}]");
        }
        
        return sb.ToString();
    }
}