namespace SDSL.Expressions;

public class CompoundArithmeticExpression : Expression
{
    public CompoundArithmeticExpression(
        SourceLocation location,
        TokenType operatorType,
        AssignableExpression left,
        Expression right)
    {
        Location = location;
        OperatorType = operatorType;
        Left = left;
        Right = right;
    }
    
    public TokenType OperatorType { get; }
    public AssignableExpression Left { get; }
    public Expression Right { get; }
   
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        SealValue value = Arithmetic.Evaluate(
            OperatorType,
            Location,
            Left.Evaluate(assembly, variables),
            Right.Evaluate(assembly, variables)
        );
        
        Left.SetValue(assembly, variables, value);
        
        return value;
    }

    public override string ToString()
    {
        return $"{Left} = {OperatorType}({Left}, {Right})";
    }
}