namespace SDSL;

public class ParserException : Exception
{
	public ParserException(SourceLocation location, string message, Exception innerException = null)
		: base($"{location} {message}", innerException)
	{
	}
    
	public ParserException(ISourceLocated source, string message, Exception innerException = null)
		: base($"{source?.Location ?? SourceLocation.Invalid} {message}", innerException)
	{
	}
}