namespace SDSL;

public abstract class SealException : Exception
{
	protected SealException(string prefix, SourceLocation location, string message)
		: base($"{prefix} in {location}, {message}")
	{
	}
	
	protected SealException(string prefix, ISourceLocated source, string message)
		: base($"{prefix} in {source?.Location ?? SourceLocation.Invalid}, {message}")
	{
	}
}