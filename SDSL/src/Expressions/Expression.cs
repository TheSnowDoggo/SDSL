namespace SDSL.Expressions;

public abstract class Expression
{
    public SourceLocation Location { get; protected init; }
    
    public abstract SealValue Evaluate(Variable[] variables);
}