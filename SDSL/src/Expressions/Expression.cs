namespace SDSL.Expressions;

public abstract class Expression
{
    public SourceLocation Location { get; init; }
    
    public abstract SealValue Evaluate(SealAssembly assembly, Variable[] variables);
}