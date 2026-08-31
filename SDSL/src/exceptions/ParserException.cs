namespace SDSL;

public class ParserException : SealException
{
	private const string Prefix = "Parser error";
	
	public ParserException(SourceLocation location, string message)
		: base(Prefix, location, message)
	{
	}
    
	public ParserException(ISourceLocated source, string message)
		: base(Prefix, source, message)
	{
	}
}