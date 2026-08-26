using System.Text;

namespace SDSL.Prototypes;

public class PrototypeFunction
{
    public PrototypeFunction(
        PrototypeClass @class,
        string name,
        PrototypeArgList argList,
        PrototypeDataType returnType,
        ArraySegment<Token> tokens,
        bool isStatic)
    {
        Class = @class;
        Name = name;
        ArgList = argList;
        ReturnType = returnType;
        Tokens = tokens;
        IsStatic = isStatic;
    }   
    
    public PrototypeClass Class { get; }
    public string Name { get; }
    public PrototypeArgList ArgList { get; }
    public PrototypeDataType ReturnType { get; }
    public ArraySegment<Token> Tokens { get; }
    public bool IsStatic { get; }

    public int AssemblyLocation { get; set; } = -1;

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (IsStatic)
        {
            sb.Append("static ");
        }

        sb.Append("func ");
        sb.Append(Name);
        
        sb.Append('(');
        sb.AppendJoin<PrototypeArg>(", ", ArgList.Args);
        sb.Append(')');

        sb.Append(" -> ");
        sb.Append(ReturnType?.ToString() ?? "?");
        
        return sb.ToString();
    }
}