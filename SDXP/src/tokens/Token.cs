using System.Text;

namespace SDSL;

public class Token : ISourceLocated
{
    public Token(
        SourceLocation location,
        TokenType tokenType,
        Variant value)
    {
        Location = location;
        TokenType = tokenType;
        Value = value;
    }
    
    public SourceLocation Location { get; }
    public TokenType TokenType { get; set; }
    public Variant Value { get; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(TokenType);
        
        if (Value.VariantType != VariantType.Nil)
        {
            sb.Append('(');
            sb.Append(Value.ToUnsafeString());
            sb.Append(')');
        }
        
        return sb.ToString();
    }
}