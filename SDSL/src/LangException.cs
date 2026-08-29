namespace SDSL;

public class LangException : Exception
{
    public LangException(SourceLocation location, string message)
        : base($"Error in {location}, {message}")
    {
    }
    
    public LangException(ISourceLocated located, string message)
        : base($"Error in {located?.Location ?? SourceLocation.Invalid}, {message}")
    {
    }
}