using System.Text;

namespace SDSL.Prototypes;

public class PrototypeField
{
    public PrototypeField(
        PrototypeClass pClass,
        string name,
        PrototypeDataType dataType,
        ArraySegment<Token> tokens,
        bool isConst,
        bool isStatic)
    {
        Class = pClass;
        Name = name;
        DataType = dataType;
        Tokens = tokens;
        IsConst = isConst;
        IsStatic = isStatic;
    }
    
    public PrototypeClass Class { get; }
    public string Name { get; }
    public PrototypeDataType DataType { get; }
    public ArraySegment<Token> Tokens { get; }
    public bool IsConst { get; }
    public bool IsStatic { get; }
    
    public int AssemblyLocation { get; set; } = -1;

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (IsStatic)
        {
            sb.Append("static ");
        }

        sb.Append(IsConst ? "const" : "var");
        sb.Append(' ');
        
        sb.Append(Name);
        sb.Append(": ");
        sb.Append(DataType);

        if (Tokens.Count != 0)
        {
            sb.Append(" = ");
            sb.Append($"Expression[{Tokens.Count}]");
        }

        sb.Append(';');
        
        return sb.ToString();
    }
}