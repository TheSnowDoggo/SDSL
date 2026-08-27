namespace SDSL.Expressions;

public abstract class Expression
{
    protected SourceLocation Location { get; init; }
    
    public abstract SealValue Evaluate(SealAssembly assembly, Variable[] variables);
}