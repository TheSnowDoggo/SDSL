using SDSL.Expressions;

namespace SDSL.Statements;

public class ExpressionStatement : Statement
{
    public ExpressionStatement(
        SourceLocation location,
        Expression expression)
    {
        Location = location;
        Expression = expression;
    }
    
    public Expression Expression { get; }
    
    public override ReturnValue Invoke(SealAssembly assembly, Variable[] variables)
    {
        Expression.Evaluate(assembly, variables);
        
        return ReturnValue.None;
    }
    
    public override string ToString()
    {
        return $"{Expression};";
    }
}