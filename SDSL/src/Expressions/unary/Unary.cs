namespace SDSL.Expressions;

public static class Unary
{
    public static SealValue Evaluate(
        TokenType operatorType,
        SourceLocation error,
        SealValue a)
    {
        return operatorType switch
        {
            TokenType.Minus  => EvaluteMinus(error, a),
            TokenType.Not    => !a.ToBool(),
            TokenType.Typeof => a.Class.ToString(),
            _ => throw new RuntimeException(error,
                $"Tried to evaluate invalid unary operator type: {operatorType}."),
        };
    }

    private static SealValue EvaluteMinus(SourceLocation error, SealValue a)
    {
        if (a.ValueType == ValueType.Number)
            return -a.AsNumber();
        
        if (a.ValueType == ValueType.TimeSpan)
            return -a.AsTimeSpan();

        throw new RuntimeException(error,
            $"No minus overload found for -{a.Class}.");
    }
}