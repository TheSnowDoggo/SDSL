using System.Text;

namespace SDSL;

public class Token : ISourceLocated
{
    public Token(
        SourceLocation location,
        TokenType tokenType,
        SealValue value)
    {
        Location = location;
        TokenType = tokenType;
        Value = value;
    }
    
    public SourceLocation Location { get; }
    public TokenType TokenType { get; set; }
    public SealValue Value { get; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append("Token");
        sb.Append(Location);
        
        sb.Append('(');
        sb.Append(TokenType);

        if (Value.Class != SealNil.Class)
        {
            sb.Append(", ");
            sb.Append(Value.ToString());
        }

        sb.Append(')');
        
        return sb.ToString();
    }
}