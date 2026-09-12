using SDSL.Functions;

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
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return Math.Pow(a.AsDouble(), b.AsDouble());
        }

        return EvaluateOverload(error, a, b, "_power");
    }
    
    private static SealValue EvaluateMultiply(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return a.AsDouble() * b.AsDouble();
        }

        if (a.ValueType == SealValueType.TimeSpan && b.ValueType == SealValueType.Number)
        {
            return a.AsTimeSpan() * b.AsDouble();
        }

        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.TimeSpan)
        {
            return a.AsDouble() * b.AsTimeSpan();
        }

        return EvaluateOverload(error, a, b, "_multiply");
    }
    
    private static SealValue EvaluateDivide(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return a.AsDouble() / b.AsDouble();
        }

        if (a.ValueType == SealValueType.TimeSpan && b.ValueType == SealValueType.Number)
        {
            return a.AsTimeSpan() / b.AsDouble();
        }

        return EvaluateOverload(error, a, b, "_divide");
    }
    
    private static SealValue EvaluateIDivide(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return Math.Truncate(a.AsDouble() / b.AsDouble());
        }

        throw new RuntimeException(error,
            $"No idivide overload found between {a.ValueType} // {b.ValueType}.");
    }
    
    private static SealValue EvaluateModulo(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return a.AsDouble() % b.AsDouble();
        }

        return EvaluateOverload(error, a, b, "_modulo");
    }
    
    private static SealValue EvaluateAdd(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return a.AsDouble() + b.AsDouble();
        }

        if (a.ValueType == SealValueType.String || b.ValueType == SealValueType.String)
        {
            return a.ToString() + b.ToString();
        }

        if (a.ValueType == SealValueType.DateTime && b.ValueType == SealValueType.TimeSpan)
        {
            return a.AsDateTime() + b.AsTimeSpan();
        }

        return EvaluateOverload(error, a, b, "_add");
    }
    
    private static SealValue EvaluateSubtract(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return a.AsDouble() - b.AsDouble();
        }

        if (a.ValueType == SealValueType.DateTime && b.ValueType == SealValueType.DateTime)
        {
            return a.AsDateTime() - b.AsDateTime();
        }

        if (a.ValueType == SealValueType.TimeSpan && b.ValueType == SealValueType.TimeSpan)
        {
            return a.AsTimeSpan() - b.AsTimeSpan();
        }

        return EvaluateOverload(error, a, b, "_subtract");
    }
    
    private static SealValue EvaluateShiftLeft(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return a.AsInt32() << b.AsInt32();
        }

        return EvaluateOverload(error, a, b, "_shift_left");
    }
    
    private static SealValue EvaluateShiftRight(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return a.AsInt32() >> b.AsInt32();
        }

        return EvaluateOverload(error, a, b, "_shift_right");
    }
    
    private static SealValue EvaluateShiftRightU(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return a.AsInt32() >>> b.AsInt32();
        }

        return EvaluateOverload(error, a, b, "_shift_right_u");
    }
    
    private static SealValue EvaluateAnd(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return a.AsInt32() & b.AsInt32();
        }

        if (a.ValueType == SealValueType.Bool && b.ValueType == SealValueType.Bool)
        {
            return a.AsBool() & b.AsBool();
        }

        return EvaluateOverload(error, a, b, "_and");
    }
    
    private static SealValue EvaluateXor(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return a.AsInt32() ^ b.AsInt32();
        }

        return EvaluateOverload(error, a, b, "_xor");
    }
    
    private static SealValue EvaluateOr(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return a.AsInt32() | b.AsInt32();
        }

        if (a.ValueType == SealValueType.Bool && b.ValueType == SealValueType.Bool)
        {
            return a.AsBool() | b.AsBool();
        }

        return EvaluateOverload(error, a, b, "_or");
    }

    private static SealValue EvaluateOverload(SourceLocation error, SealValue a, SealValue b, string name)
    {
        if (a.Class.TryGetFunction(name, out Function function)
            && function.MinArgs == 1
            && b.Class.IsAssignableTo(function.Args[0].Class))
        {
            try
            {
                return function.MemberInvoke(a, b);
            }
            catch (Exception ex)
            {
                throw new RuntimeException(error,
                    $"{a.ToString(true)}->{name}({b.ToString(true)})\n  --> {ex.Message}");
            }
        }
        
        throw new RuntimeException(error,
            $"No {name} overload found between {a.ValueType} and {b.ValueType}.");
    }
}