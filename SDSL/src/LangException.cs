namespace SDSL;

public class LangException : Exception
{
    public LangException(SourceLocation location, string message)
        : base($"Error at {location}, {message}")
    {
    }
    
    public LangException(ISourceLocated located, string message)
        : base($"Error at {located?.Location ?? SourceLocation.Empty}, {message}")
    {
    }
}