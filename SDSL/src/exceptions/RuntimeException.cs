namespace SDSL;

public class RuntimeException : SealException
{
	private const string Prefix = "Runtime error";
	
	public RuntimeException(SourceLocation location, string message)
		: base(Prefix, location, message)
	{
	}
    
	public RuntimeException(ISourceLocated source, string message)
		: base(Prefix, source, message)
	{
	}
}