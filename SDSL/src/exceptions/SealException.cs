namespace SDSL;

public abstract class SealException : Exception
{
	protected SealException(
		SourceLocation location,
		string message,
		Exception innerException
	) : base($"{location} {message}", innerException)
	{
	}
	
	protected SealException(
		ISourceLocated source,
		string message,
		Exception innerException
	) : base($"{source?.Location ?? SourceLocation.Invalid} {message}", innerException)
	{
	}
}