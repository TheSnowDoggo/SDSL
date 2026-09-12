using System.Text;

namespace SDSL.Prototypes;

public class PrototypeField : ISourceLocated
{
    public PrototypeField(
        SourceLocation location,
        PrototypeClass nativeClass,
        string name,
        PrototypeDataType dataType,
        bool isConst,
        bool isStatic,
        ArraySegment<Token> tokens)
    {
        Location = location;
        NativeClass = nativeClass;
        Name = name;
        DataType = dataType;
        IsConst = isConst;
        IsStatic = isStatic;
        Tokens = tokens;
    }
    
    public SourceLocation Location { get; }
    
    public PrototypeClass NativeClass { get; }
    
    public string Name { get; }
    
    public PrototypeDataType DataType { get; }
    
    public bool IsConst { get; }
    
    public bool IsStatic { get; }
    
    public ArraySegment<Token> Tokens { get; }
    
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

        sb.Append(';');
        
        return sb.ToString();
    }
}