namespace SDSL;

public abstract class SealException : Exception
{
	protected SealException(string prefix,
		SourceLocation location,
		string message,
		Exception innerException
	) : base($"{prefix} in {location}, {message}", innerException)
	{
	}
	
	protected SealException(
		string prefix,
		ISourceLocated source,
		string message,
		Exception innerException
	) : base($"{prefix} in {source?.Location ?? SourceLocation.Invalid}, {message}", innerException)
	{
	}
}