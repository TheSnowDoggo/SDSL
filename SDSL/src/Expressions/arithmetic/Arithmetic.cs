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
            TokenType.Multiply or TokenType.MultiplyAssign => EvaluateMultiply(error, a, b),
            TokenType.Divide   or TokenType.DivideAssign   => EvaluateDivide(error, a, b),
            TokenType.IDivide  or TokenType.IDivideAssign  => EvaluateIDivide(error, a, b),
            TokenType.Modulo   or TokenType.ModuloAssign   => EvaluateModulo(error, a, b),
            TokenType.Add      or TokenType.AddAssign      => EvaluateAdd(error, a, b),
            TokenType.Subtract or TokenType.SubtractAssign => EvaluateSubtract(error, a, b),
            TokenType.And      or TokenType.AndAssign      => EvaluateAnd(error, a, b),
            TokenType.Xor      or TokenType.XorAssign      => EvaluateXor(error, a, b),
            TokenType.Or       or TokenType.OrAssign       => EvaluateOr(error, a, b),
            _ => throw new LangException(error,
                $"Got invalid arithmetic operator type: {operatorType}.")
        };
    }
    
    private static SealValue EvaluateMultiply(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.Class == SealClass.Number && b.Class == SealClass.Number)
            return a.AsNumber() * b.AsNumber();

        throw new LangException(error,
            $"No multiply overload found between {a.Class} * {b.Class}.");
    }
    
    private static SealValue EvaluateDivide(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.Class == SealClass.Number && b.Class == SealClass.Number)
            return a.AsNumber() / b.AsNumber();

        throw new LangException(error,
            $"No divide overload found between {a.Class} / {b.Class}.");
    }
    
    private static SealValue EvaluateIDivide(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.Class == SealClass.Number && b.Class == SealClass.Number)
            return Math.Truncate(a.AsNumber() / b.AsNumber());

        throw new LangException(error,
            $"No idivide overload found between {a.Class} // {b.Class}.");
    }
    
    private static SealValue EvaluateModulo(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.Class == SealClass.Number && b.Class == SealClass.Number)
            return a.AsNumber() % b.AsNumber();

        throw new LangException(error,
            $"No modulo overload found between {a.Class} % {b.Class}.");
    }
    
    private static SealValue EvaluateAdd(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.Class == SealClass.Number && b.Class == SealClass.Number)
            return a.AsNumber() + b.AsNumber();

        if (a.Class == SealClass.String || b.Class == SealClass.String)
            return a.ToString() + b.ToString();

        throw new LangException(error,
            $"No add overload found between {a.Class} + {b.Class}.");
    }
    
    private static SealValue EvaluateSubtract(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.Class == SealClass.Number && b.Class == SealClass.Number)
            return a.AsNumber() - b.AsNumber();

        throw new LangException(error,
            $"No subtract overload found between {a.Class} - {b.Class}.");
    }
    
    private static SealValue EvaluateAnd(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.Class == SealClass.Number && b.Class == SealClass.Number)
            return (int)a.AsNumber() & (int)b.AsNumber();

        if (a.Class == SealClass.Bool && b.Class == SealClass.Bool)
            return a.AsBool() & b.AsBool();

        throw new LangException(error,
            $"No and overload found between {a.Class} & {b.Class}.");
    }
    
    private static SealValue EvaluateXor(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.Class == SealClass.Number && b.Class == SealClass.Number)
            return (int)a.AsNumber() ^ (int)b.AsNumber();

        throw new LangException(error,
            $"No xor overload found between {a.Class} ^ {b.Class}.");
    }
    
    private static SealValue EvaluateOr(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.Class == SealClass.Number && b.Class == SealClass.Number)
            return (int)a.AsNumber() | (int)b.AsNumber();

        if (a.Class == SealClass.Bool && b.Class == SealClass.Bool)
            return a.AsBool() | b.AsBool();

        throw new LangException(error,
            $"No or overload found between {a.Class} | {b.Class}.");
    }
}