namespace SDSL;

public class SourceLocation
{
    public SourceLocation(
        string file,
        int line,
        int column)
    {
        File = file;
        Line = line;
        Column = column;
    }
    
    public SourceLocation(string file)
    {
        File = file;
        Line = -1;
        Column = -1;
    }

    public static SourceLocation Invalid { get; } = new SourceLocation(null);
    public static SourceLocation Native  { get; } = new SourceLocation("Native");

    public string File { get; }
    public int Line { get; }
    public int Column { get; }

    public override string ToString()
    {
        if (this == Native)
        {
            return "<Native>";
        }
        
        return $"<{File} at {Line}:{Line}>";
    }
}