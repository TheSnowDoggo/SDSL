namespace SDSL;

public readonly struct SourceLocation
{
    private readonly int _line;
    private readonly int _column;
    
    public SourceLocation(int line, int column)
    {
        _line = line;
        _column = column;
    }

    public static readonly SourceLocation Empty = new SourceLocation(-1, -1);

    public int Line => _line;
    public int Column => _column;

    public override string ToString()
    {
        if (_line == -1 || _column == -1)
            return "[?]";
        return $"[{_line}:{_column}]";
    }
}