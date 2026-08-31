namespace SDSL.Expressions;

public abstract class Expression : ISourceLocated
{
    public SourceLocation Location { get; protected init; }
    
    public abstract SealValue Evaluate(Variable[] variables);

    public abstract bool IsConstantEval();
}