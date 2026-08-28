namespace SDSL.Expressions;

public class ConditionalAndExpression : Expression
{
    public ConditionalAndExpression(
        SourceLocation location,
        Expression left,
        Expression right)
    {
        Location = location;
        Left = left;
        Right = right;
    }
    
    public Expression Left { get; }
    public Expression Right { get; }

    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        return Left.Evaluate(assembly, variables).InterpretAsBool()
               && Right.Evaluate(assembly, variables).InterpretAsBool();
    }

    public override string ToString()
    {
        return $"&&({Left}, {Right})";
    }
}