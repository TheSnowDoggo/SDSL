namespace SDSL.Expressions;

public class CompoundArithmeticExpression : Expression
{
    public CompoundArithmeticExpression(
        SourceLocation location,
        ArithmeticOperatorType operatorType,
        AssignableExpression left,
        Expression right)
    {
        Location = location;
        OperatorType = operatorType;
        Left = left;
        Right = right;
    }
    
    public ArithmeticOperatorType OperatorType { get; }
    public AssignableExpression Left { get; }
    public Expression Right { get; }
    
    public static bool TryParseOperatorType(TokenType tokenType, out ArithmeticOperatorType operatorType)
    {
        const ArithmeticOperatorType Invalid = (ArithmeticOperatorType)(-1);
        
        operatorType = tokenType switch
        {
            TokenType.MultiplyAssign => ArithmeticOperatorType.Multiply,
            TokenType.DivideAssign   => ArithmeticOperatorType.Divide,
            TokenType.IDivideAssign  => ArithmeticOperatorType.IDivide,
            TokenType.ModuloAssign   => ArithmeticOperatorType.Modulo,
            TokenType.AddAssign      => ArithmeticOperatorType.Add,
            TokenType.SubtractAssign => ArithmeticOperatorType.Subtract,
            TokenType.AndAssign      => ArithmeticOperatorType.And,
            TokenType.XorAssign      => ArithmeticOperatorType.Xor,
            TokenType.OrAssign       => ArithmeticOperatorType.Or,
            _  => Invalid
        };
        
        return operatorType != Invalid;
    }

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