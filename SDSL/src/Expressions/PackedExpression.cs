namespace SDSL.Expressions;

public class PackedExpression
{
    public PackedExpression(
        Expression expression,
        SourceLocation location,
        SealAssembly assembly)
    {
        Expression = expression;
        Location = location;
        Assembly = assembly;
    }
    
    public Expression Expression { get; }
    public SourceLocation Location { get; }
    public SealAssembly Assembly { get; }

    public SealValue Evaluate(Variable[] variables)
    {
        return Expression.Evaluate(Location, Assembly, variables);
    }
}