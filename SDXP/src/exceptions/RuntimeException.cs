namespace SDSL;

public class RuntimeException : Exception
{
	public RuntimeException(SourceLocation location, string message, Exception innerException = null)
		: base($"{location} {message}", innerException)
	{
	}
    
	public RuntimeException(ISourceLocated source, string message, Exception innerException = null)
		: base($"{source?.Location ?? SourceLocation.Invalid} {message}", innerException)
	{
	}
}