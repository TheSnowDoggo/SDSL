namespace SDSL.Expressions;

public static class Arithmetic
{
    public static SealValue Evaluate(
        TokenType operatorType,
        SourceLocation error,
        SealValue a,
        SealValue b)
    {
        switch (operatorType)
        {
        case TokenType.Power or TokenType.PowerAssign:
            return EvaluatePower(error, a, b);
        
        case TokenType.Multiply or TokenType.MultiplyAssign:
            return EvaluateMultiply(error, a, b);
        case TokenType.Divide or TokenType.DivideAssign:
            return EvaluateDivide(error, a, b);
        case TokenType.IDivide or TokenType.IDivideAssign:
            return EvaluateIDivide(error, a, b);
        case TokenType.Modulo or TokenType.ModuloAssign:
            return EvaluateModulo(error, a, b);
        
        case TokenType.Add or TokenType.AddAssign:
            return EvaluateAdd(error, a, b);
        case TokenType.Subtract or TokenType.SubtractAssign:
            return EvaluateSubtract(error, a, b);
        
        case TokenType.ShiftLeft or TokenType.ShiftLeftAssign:
            return EvaluateShiftLeft(error, a, b);
        case TokenType.ShiftRight or TokenType.ShiftRightAssign:
            return EvaluateShiftRight(error, a, b);
        case TokenType.ShiftRightU or TokenType.ShiftRightUAssign:
            return EvaluateShiftRightU(error, a, b);
        
        case TokenType.And or TokenType.AndAssign:
            return EvaluateAnd(error, a, b);
        case TokenType.Xor or TokenType.XorAssign:
            return EvaluateXor(error, a, b);
        case TokenType.Or or TokenType.OrAssign:
            return EvaluateOr(error, a, b);
        default:
            throw new RuntimeException(error,
                $"Tried to evaluate invalid arithmetic operator type: {operatorType}.");
        }
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
    
    private static SealValue EvaluateShiftLeft(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsInt32() << b.AsInt32();

        throw new RuntimeException(error,
            $"No shift left overload found between {a.ValueType} << {b.ValueType}.");
    }
    
    private static SealValue EvaluateShiftRight(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsInt32() >> b.AsInt32();

        throw new RuntimeException(error,
            $"No shift right overload found between {a.ValueType} >> {b.ValueType}.");
    }
    
    private static SealValue EvaluateShiftRightU(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsInt32() >>> b.AsInt32();

        throw new RuntimeException(error,
            $"No shift right unsigned overload found between {a.ValueType} >>> {b.ValueType}.");
    }
    
    private static SealValue EvaluateAnd(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsInt32() & b.AsInt32();

        if (a.ValueType == ValueType.Bool && b.ValueType == ValueType.Bool)
            return a.AsBool() & b.AsBool();

        throw new RuntimeException(error,
            $"No and overload found between {a.ValueType} & {b.ValueType}.");
    }
    
    private static SealValue EvaluateXor(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsInt32() ^ b.AsInt32();

        throw new RuntimeException(error,
            $"No xor overload found between {a.ValueType} ^ {b.ValueType}.");
    }
    
    private static SealValue EvaluateOr(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsInt32() | b.AsInt32();

        if (a.ValueType == ValueType.Bool && b.ValueType == ValueType.Bool)
            return a.AsBool() | b.AsBool();

        throw new RuntimeException(error,
            $"No or overload found between {a.ValueType} | {b.ValueType}.");
    }
}