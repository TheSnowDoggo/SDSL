namespace SDSL;

public class ParserException : SealException
{
	public ParserException(SourceLocation location, string message, Exception innerException = null)
		: base(location, message, innerException)
	{
	}
    
	public ParserException(ISourceLocated source, string message, Exception innerException = null)
		: base(source, message, innerException)
	{
	}
}