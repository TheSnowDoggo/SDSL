using System.Text;

namespace SDSL.Prototypes;

public class PrototypeField
{
    public PrototypeField(
        PrototypeClass @class,
        string name,
        PrototypeDataType dataType,
        ArraySegment<Token> expression,
        bool isConst,
        bool isStatic)
    {
        Class = @class;
        Name = name;
        DataType = dataType;
        Expression = expression;
        IsConst = isConst;
        IsStatic = isStatic;
    }
    
    public PrototypeClass Class { get; }
    public string Name { get; }
    public PrototypeDataType DataType { get; }
    public ArraySegment<Token> Expression { get; }
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

        if (Expression.Count != 0)
        {
            sb.Append(" = ");
            sb.Append($"Expression[{Expression.Count}]");
        }
        
        return sb.ToString();
    }
}