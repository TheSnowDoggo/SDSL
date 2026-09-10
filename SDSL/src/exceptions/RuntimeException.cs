using System.Text;

namespace SDSL;

public class RuntimeException : SealException
{
	private const string Prefix = "Runtime error";
	
	public RuntimeException(SourceLocation location, string message, Exception innerException = null)
		: base(Prefix, location, message, innerException)
	{
	}
    
	public RuntimeException(ISourceLocated source, string message, Exception innerException = null)
		: base(Prefix, source, message, innerException)
	{
	}
}