namespace SDSL.Expressions;

public class ArrayExpression : Expression
{
    public ArrayExpression(
        SourceLocation location,
        Expression[] itemExpressions)
    {
        Location = location;
        ItemExpressions = itemExpressions;
    }
    
    public Expression[] ItemExpressions { get; }
    
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        var items = new List<SealValue>(ItemExpressions.Length);
        for (int i = 0; i < ItemExpressions.Length; i++)
            items.Add(ItemExpressions[i].Evaluate(assembly, variables));
        return new SealArray(items);
    }
}