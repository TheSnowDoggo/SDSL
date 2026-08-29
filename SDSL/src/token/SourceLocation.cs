namespace SDSL;

public class SourceLocation
{
    public SourceLocation(
        int line,
        int column,
        string file)
    {
        Line = line;
        Column = column;
        File = file;
    }

    public static readonly SourceLocation Invalid = new SourceLocation(-1, -1, null);

    public int Line { get; }
    public int Column { get; }
    public string File { get; }

    public override string ToString()
    {
        if (Line == -1 || Column == -1)
            return "[?]";
        return $"{File} at {Line}:{Line}";
    }
}