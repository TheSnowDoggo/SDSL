using System.Text;

namespace SDSL.Prototypes;

public class PrototypeFunction
{
    public PrototypeFunction(
        SourceLocation location,
        PrototypeClass nativeClass,
        string name,
        PrototypeArgumentList argList,
        PrototypeDataType returnType,
        bool isStatic,
        PrototypeType prototypeType,
        object data)
    {
        Location = location;
        NativeClass = nativeClass;
        Name = name;
        ArgList = argList;
        ReturnType = returnType;
        IsStatic = isStatic;
        PrototypeType = prototypeType;
        Data = data;
    }
    
    public SourceLocation Location { get; }
    
    public PrototypeClass NativeClass { get; }
    
    public string Name { get; }
    
    public PrototypeArgumentList ArgList { get; }
    
    public PrototypeDataType ReturnType { get; }
    
    public bool IsStatic { get; }
    
    public PrototypeType PrototypeType { get; }
    public object Data { get; }

    public string FullName => $"{NativeClass.FullName}.{Name}";

    public int AssemblyLocation { get; set; } = -1;
    
    public override string ToString()
    {
        var sb = new StringBuilder();

        if (IsStatic)
        {
            sb.Append("static ");
        }

        sb.Append("func ");

        sb.Append(NativeClass);
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