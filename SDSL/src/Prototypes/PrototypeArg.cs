using System.Text;

namespace SDSL.Prototypes;

public class PrototypeArg
{
    public PrototypeArg(
        string name,
        PrototypeDataType dataType,
        bool isConst,
        ArraySegment<Token> tokens = default)
    {
        Name = name;
        DataType = dataType;
        IsConst = isConst;
        Tokens = tokens;
    }
    
    public string Name { get; }
    public PrototypeDataType DataType { get; }
    public bool IsConst { get; }
    public ArraySegment<Token> Tokens { get; }

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