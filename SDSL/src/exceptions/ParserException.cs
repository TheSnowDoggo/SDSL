namespace SDSL;

public class ParserException : SealException
{
	private const string Prefix = "Parser error";
	
	public ParserException(SourceLocation location, string message, Exception innerException = null)
		: base(Prefix, location, message, innerException)
	{
	}
    
	public ParserException(ISourceLocated source, string message, Exception innerException = null)
		: base(Prefix, source, message, innerException)
	{
	}
}