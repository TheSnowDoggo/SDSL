using System.Text;

namespace SDSL.Prototypes;

public class PrototypeFunction
{
    public PrototypeFunction(
        SourceLocation location,
        PrototypeClass pClass,
        string name,
        PrototypeArgList argList,
        PrototypeDataType returnType,
        bool isStatic,
        FunctionBody body)
    {
        Location = location;
        Class = pClass;
        Name = name;
        ArgList = argList;
        ReturnType = returnType;
        IsStatic = isStatic;
        Body = body;
    }
    
    public SourceLocation Location { get; }
    public PrototypeClass Class { get; }
    public string Name { get; }
    public PrototypeArgList ArgList { get; }
    public PrototypeDataType ReturnType { get; }
    public bool IsStatic { get; }
    public FunctionBody Body { get; }

    public int AssemblyLocation { get; set; } = -1;

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (IsStatic)
        {
            sb.Append("static ");
        }

        sb.Append("func ");

        sb.Append(Class);
        sb.Append('.');
        sb.Append(Name);
        
        sb.Append('(');
        sb.AppendJoin<PrototypeArgument>(", ", ArgList.Args);
        sb.Append(')');

        sb.Append(" -> ");

        if (ReturnType == null)
        {
            sb.Append("Any");
        }
        else
        {
            sb.Append(ReturnType);
        }
        
        return sb.ToString();
    }
}