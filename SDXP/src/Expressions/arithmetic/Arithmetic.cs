namespace SDSL.Expressions;

public static class Arithmetic
{
    public static Variant Evaluate(
        TokenType operatorType,
        Variant a,
        Variant b)
    {
        switch (operatorType)
        {
        case TokenType.Power:
        case TokenType.PowerAssign:
            return EvaluatePower(a, b);
        case TokenType.Multiply:
        case TokenType.MultiplyAssign:
            return EvaluateMultiply(a, b);
        case TokenType.Divide:
        case TokenType.DivideAssign:
            return EvaluateDivide(a, b);
        case TokenType.IDivide:
        case TokenType.IDivideAssign:
            return EvaluateIDivide(a, b);
        case TokenType.Modulo:
        case TokenType.ModuloAssign:
            return EvaluateModulo(a, b);
        case TokenType.Add:
        case TokenType.AddAssign:
            return EvaluateAdd(a, b);
        case TokenType.Subtract:
        case TokenType.SubtractAssign:
            return EvaluateSubtract(a, b);
        case TokenType.ShiftLeft:
        case TokenType.ShiftLeftAssign:
            return EvaluateShiftLeft(a, b);
        case TokenType.ShiftRight:
        case TokenType.ShiftRightAssign:
            return EvaluateShiftRight(a, b);
        case TokenType.ShiftRightU: 
        case TokenType.ShiftRightUAssign:
            return EvaluateShiftRightU(a, b);
        case TokenType.And:
        case TokenType.AndAssign:
            return EvaluateAnd(a, b);
        case TokenType.Xor:
        case TokenType.XorAssign:
            return EvaluateXor(a, b);
        case TokenType.Or:
        case TokenType.OrAssign:
            return EvaluateOr(a, b);
        default:
            throw new RuntimeException($"Tried to evaluate invalid arithmetic operator type: {operatorType}.");
        }
    }
    
    private static Variant EvaluatePower(Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return Math.Pow(a.AsDouble(), b.AsDouble());
        }

        return EvaluateOverload(a, b, "_power");
    }
    
    private static Variant EvaluateMultiply(Variant a, Variant b)
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

        return EvaluateOverload(a, b, "_multiply");
    }
    
    private static Variant EvaluateDivide(Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsDouble() / b.AsDouble();
        }

        if (a.VariantType == VariantType.TimeSpan && b.VariantType == VariantType.Number)
        {
            return a.AsTimeSpan() / b.AsDouble();
        }

        return EvaluateOverload(a, b, "_divide");
    }
    
    private static Variant EvaluateIDivide(Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return Math.Truncate(a.AsDouble() / b.AsDouble());
        }

        throw new RuntimeException($"No idivide overload found between {a.VariantType} // {b.VariantType}.");
    }
    
    private static Variant EvaluateModulo(Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsDouble() % b.AsDouble();
        }

        return EvaluateOverload(a, b, "_modulo");
    }
    
    private static Variant EvaluateAdd(Variant a, Variant b)
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

        return EvaluateOverload(a, b, "_add");
    }
    
    private static Variant EvaluateSubtract(Variant a, Variant b)
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

        return EvaluateOverload(a, b, "_subtract");
    }
    
    private static Variant EvaluateShiftLeft(Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() << b.AsInt32();
        }

        return EvaluateOverload(a, b, "_shift_left");
    }
    
    private static Variant EvaluateShiftRight(Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() >> b.AsInt32();
        }

        return EvaluateOverload(a, b, "_shift_right");
    }
    
    private static Variant EvaluateShiftRightU(Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() >>> b.AsInt32();
        }

        return EvaluateOverload(a, b, "_shift_right_u");
    }
    
    private static Variant EvaluateAnd(Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() & b.AsInt32();
        }

        if (a.VariantType == VariantType.Bool && b.VariantType == VariantType.Bool)
        {
            return a.AsBool() & b.AsBool();
        }

        return EvaluateOverload(a, b, "_and");
    }
    
    private static Variant EvaluateXor(Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() ^ b.AsInt32();
        }

        return EvaluateOverload(a, b, "_xor");
    }
    
    private static Variant EvaluateOr(Variant a, Variant b)
    {
        if (a.VariantType == VariantType.Number && b.VariantType == VariantType.Number)
        {
            return a.AsInt32() | b.AsInt32();
        }

        if (a.VariantType == VariantType.Bool && b.VariantType == VariantType.Bool)
        {
            return a.AsBool() | b.AsBool();
        }

        return EvaluateOverload(a, b, "_or");
    }

    private static Variant EvaluateOverload(Variant a, Variant b, string name)
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
                throw new RuntimeException(
                    $"[overload] {a.ToSafeString()}->{name}({b.ToSafeString()})\n  --> {ex.Message}");
            }
        }
        
        throw new RuntimeException($"No {name} overload found between {a.Class} and {b.Class}.");
    }
}