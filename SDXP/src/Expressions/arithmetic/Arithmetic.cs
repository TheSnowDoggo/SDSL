namespace SDSL.Expressions;

public static class Arithmetic
{
    public static Variant Evaluate(
        TokenType operatorType,
        SourceLocation error,
        Variant a,
        Variant b)
    {
        switch (operatorType)
        {
        case TokenType.Power:
        case TokenType.PowerAssign:
            return EvaluatePower(error, a, b);
        case TokenType.Multiply:
        case TokenType.MultiplyAssign:
            return EvaluateMultiply(error, a, b);
        case TokenType.Divide:
        case TokenType.DivideAssign:
            return EvaluateDivide(error, a, b);
        case TokenType.IDivide:
        case TokenType.IDivideAssign:
            return EvaluateIDivide(error, a, b);
        case TokenType.Modulo:
        case TokenType.ModuloAssign:
            return EvaluateModulo(error, a, b);
        case TokenType.Add:
        case TokenType.AddAssign:
            return EvaluateAdd(error, a, b);
        case TokenType.Subtract:
        case TokenType.SubtractAssign:
            return EvaluateSubtract(error, a, b);
        case TokenType.ShiftLeft:
        case TokenType.ShiftLeftAssign:
            return EvaluateShiftLeft(error, a, b);
        case TokenType.ShiftRight:
        case TokenType.ShiftRightAssign:
            return EvaluateShiftRight(error, a, b);
        case TokenType.ShiftRightU: 
        case TokenType.ShiftRightUAssign:
            return EvaluateShiftRightU(error, a, b);
        case TokenType.And:
        case TokenType.AndAssign:
            return EvaluateAnd(error, a, b);
        case TokenType.Xor:
        case TokenType.XorAssign:
            return EvaluateXor(error, a, b);
        case TokenType.Or:
        case TokenType.OrAssign:
            return EvaluateOr(error, a, b);
        default:
            throw new RuntimeException(error,
                $"Tried to evaluate invalid arithmetic operator type: {operatorType}.");
        }
    }
    
    private static Variant EvaluatePower(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return Math.Pow(a.AsDouble(), b.AsDouble());
        }

        return EvaluateOverload(error, a, b, "_power");
    }
    
    private static Variant EvaluateMultiply(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsDouble() * b.AsDouble();
        }

        if (a.VariantType == VariantType.TimeSpan && b.VariantType == VariantType.Number)
        {
            return a.AsTimeSpan() * b.AsDouble();
        }

        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.TimeSpan)
        {
            return a.AsDouble() * b.AsTimeSpan();
        }

        return EvaluateOverload(error, a, b, "_multiply");
    }
    
    private static Variant EvaluateDivide(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsDouble() / b.AsDouble();
        }

        if (a.VariantType == VariantType.TimeSpan && b.VariantType == VariantType.Number)
        {
            return a.AsTimeSpan() / b.AsDouble();
        }

        return EvaluateOverload(error, a, b, "_divide");
    }
    
    private static Variant EvaluateIDivide(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return Math.Truncate(a.AsDouble() / b.AsDouble());
        }

        throw new RuntimeException(error,
            $"No idivide overload found between {a.VariantType} // {b.VariantType}.");
    }
    
    private static Variant EvaluateModulo(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsDouble() % b.AsDouble();
        }

        return EvaluateOverload(error, a, b, "_modulo");
    }
    
    private static Variant EvaluateAdd(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsDouble() + b.AsDouble();
        }

        if (a.VariantType == VariantType.String || b.VariantType == VariantType.String)
        {
            return a.ToString() + b.ToString();
        }

        if (a.VariantType == VariantType.DateTime && b.VariantType == VariantType.TimeSpan)
        {
            return a.AsDateTime() + b.AsTimeSpan();
        }

        return EvaluateOverload(error, a, b, "_add");
    }
    
    private static Variant EvaluateSubtract(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsDouble() - b.AsDouble();
        }

        if (a.VariantType == VariantType.DateTime && b.VariantType == VariantType.DateTime)
        {
            return a.AsDateTime() - b.AsDateTime();
        }

        if (a.VariantType == VariantType.TimeSpan && b.VariantType == VariantType.TimeSpan)
        {
            return a.AsTimeSpan() - b.AsTimeSpan();
        }

        return EvaluateOverload(error, a, b, "_subtract");
    }
    
    private static Variant EvaluateShiftLeft(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() << b.AsInt32();
        }

        return EvaluateOverload(error, a, b, "_shift_left");
    }
    
    private static Variant EvaluateShiftRight(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() >> b.AsInt32();
        }

        return EvaluateOverload(error, a, b, "_shift_right");
    }
    
    private static Variant EvaluateShiftRightU(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() >>> b.AsInt32();
        }

        return EvaluateOverload(error, a, b, "_shift_right_u");
    }
    
    private static Variant EvaluateAnd(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() & b.AsInt32();
        }

        if (a.VariantType == VariantType.Bool && b.VariantType == VariantType.Bool)
        {
            return a.AsBool() & b.AsBool();
        }

        return EvaluateOverload(error, a, b, "_and");
    }
    
    private static Variant EvaluateXor(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() ^ b.AsInt32();
        }

        return EvaluateOverload(error, a, b, "_xor");
    }
    
    private static Variant EvaluateOr(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() | b.AsInt32();
        }

        if (a.VariantType == VariantType.Bool && b.VariantType == VariantType.Bool)
        {
            return a.AsBool() | b.AsBool();
        }

        return EvaluateOverload(error, a, b, "_or");
    }

    private static Variant EvaluateOverload(SourceLocation error, Variant a, Variant b, string name)
    {
        if (a.Class.FunctionMap.TryGetValue(name, out Function function)
            && function.Signature.MinArgs == 1
            && b.IsAssignableTo(function.Signature.Arguments[0].VariantClass))
        {
            try
            {
                return function.MemberInvoke(a, b);
            }
            catch (Exception ex)
            {
                throw new RuntimeException(error,
                    $"[overload] {a.ToSafeString()}->{name}({b.ToSafeString()})\n  --> {ex.Message}");
            }
        }
        
        throw new RuntimeException(error,
            $"No {name} overload found between {a.Class} and {b.Class}.");
    }
}