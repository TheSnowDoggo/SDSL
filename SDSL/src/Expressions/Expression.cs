namespace SDSL.Expressions;

public abstract class Expression
{
    public abstract SealValue Evaluate(SourceLocation error, SealAssembly assembly, Variable[] variables);
}