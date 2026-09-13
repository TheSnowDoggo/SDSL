namespace SDSL;

public abstract class UserProperty : Property
{
	public SourceLocation Location { get; protected init; }
	public ArraySegment<Token> Tokens { get; protected init; }
}