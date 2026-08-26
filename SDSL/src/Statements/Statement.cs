namespace SDSL.Statements;

public abstract class Statement : ISourceLocated
{
    public SourceLocation Location { get; init; }
    
    public abstract ReturnValue Invoke(SealAssembly assembly, Variable[] variables);
}