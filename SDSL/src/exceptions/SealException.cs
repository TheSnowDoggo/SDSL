namespace SDSL;

public abstract class SealException : Exception
{
	protected SealException(
		SourceLocation location,
		string message,
		Exception innerException
	) : base($"in {location}, {message}", innerException)
	{
	}
	
	protected SealException(
		ISourceLocated source,
		string message,
		Exception innerException
	) : base($"in {source?.Location ?? SourceLocation.Invalid}, {message}", innerException)
	{
	}
}