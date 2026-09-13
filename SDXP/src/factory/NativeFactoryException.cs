namespace SDSL;

public class NativeFactoryException : Exception
{
	public NativeFactoryException(string message, Exception innerException = null)
		: base(message, innerException)
	{
	}
}