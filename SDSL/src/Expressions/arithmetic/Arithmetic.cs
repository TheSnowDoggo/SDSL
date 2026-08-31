namespace SDSL.Expressions;

public static class Arithmetic
{
    public static SealValue Evaluate(
        TokenType operatorType,
        SourceLocation error,
        SealValue a,
        SealValue b)
    {
        return operatorType switch
        {
            TokenType.Power    or TokenType.PowerAssign    => EvaluatePower(error, a, b),
            TokenType.Multiply or TokenType.MultiplyAssign => EvaluateMultiply(error, a, b),
            TokenType.Divide   or TokenType.DivideAssign   => EvaluateDivide(error, a, b),
            TokenType.IDivide  or TokenType.IDivideAssign  => EvaluateIDivide(error, a, b),
            TokenType.Modulo   or TokenType.ModuloAssign   => EvaluateModulo(error, a, b),
            TokenType.Add      or TokenType.AddAssign      => EvaluateAdd(error, a, b),
            TokenType.Subtract or TokenType.SubtractAssign => EvaluateSubtract(error, a, b),
            TokenType.And      or TokenType.AndAssign      => EvaluateAnd(error, a, b),
            TokenType.Xor      or TokenType.XorAssign      => EvaluateXor(error, a, b),
            TokenType.Or       or TokenType.OrAssign       => EvaluateOr(error, a, b),
            _ => throw new RuntimeException(error,
                $"Tried to evaluate invalid arithmetic operator type: {operatorType}."),
        };
    }
    
    private static SealValue EvaluatePower(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return Math.Pow(a.AsNumber(), b.AsNumber());

        throw new RuntimeException(error,
            $"No power overload found between {a.ValueType} ** {b.ValueType}.");
    }
    
    private static SealValue EvaluateMultiply(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsNumber() * b.AsNumber();

        if (a.ValueType == ValueType.TimeSpan && b.ValueType == ValueType.Number)
            return a.AsTimeSpan() * b.AsNumber();
        
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.TimeSpan)
            return a.AsNumber() * b.AsTimeSpan();

        throw new RuntimeException(error,
            $"No multiply overload found between {a.ValueType} * {b.ValueType}.");
    }
    
    private static SealValue EvaluateDivide(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsNumber() / b.AsNumber();
        
        if (a.ValueType == ValueType.TimeSpan && b.ValueType == ValueType.Number)
            return a.AsTimeSpan() / b.AsNumber();
        
        throw new RuntimeException(error,
            $"No divide overload found between {a.ValueType} / {b.ValueType}.");
    }
    
    private static SealValue EvaluateIDivide(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return Math.Truncate(a.AsNumber() / b.AsNumber());

        throw new RuntimeException(error,
            $"No idivide overload found between {a.ValueType} // {b.ValueType}.");
    }
    
    private static SealValue EvaluateModulo(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsNumber() % b.AsNumber();

        throw new RuntimeException(error,
            $"No modulo overload found between {a.ValueType} % {b.ValueType}.");
    }
    
    private static SealValue EvaluateAdd(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsNumber() + b.AsNumber();

        if (a.ValueType == ValueType.String || b.ValueType == ValueType.String)
            return a.ToString() + b.ToString();

        if (a.ValueType == ValueType.DateTime && b.ValueType == ValueType.TimeSpan)
            return a.AsDateTime() + b.AsTimeSpan();

        throw new RuntimeException(error,
            $"No add overload found between {a.ValueType} + {b.ValueType}.");
    }
    
    private static SealValue EvaluateSubtract(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsNumber() - b.AsNumber();
        
        if (a.ValueType == ValueType.DateTime && b.ValueType == ValueType.DateTime)
            return a.AsDateTime() - b.AsDateTime();
        
        if (a.ValueType == ValueType.TimeSpan && b.ValueType == ValueType.TimeSpan)
            return a.AsTimeSpan() - b.AsTimeSpan();

        throw new RuntimeException(error,
            $"No subtract overload found between {a.ValueType} - {b.ValueType}.");
    }
    
    private static SealValue EvaluateAnd(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return (int)a.AsNumber() & (int)b.AsNumber();

        if (a.ValueType == ValueType.Bool && b.ValueType == ValueType.Bool)
            return a.AsBool() & b.AsBool();

        throw new RuntimeException(error,
            $"No and overload found between {a.ValueType} & {b.ValueType}.");
    }
    
    private static SealValue EvaluateXor(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return (int)a.AsNumber() ^ (int)b.AsNumber();

        throw new RuntimeException(error,
            $"No xor overload found between {a.ValueType} ^ {b.ValueType}.");
    }
    
    private static SealValue EvaluateOr(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return (int)a.AsNumber() | (int)b.AsNumber();

        if (a.ValueType == ValueType.Bool && b.ValueType == ValueType.Bool)
            return a.AsBool() | b.AsBool();

        throw new RuntimeException(error,
            $"No or overload found between {a.ValueType} | {b.ValueType}.");
    }
}