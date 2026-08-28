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
            TokenType.Not    => !a.InterpretAsBool(),
            TokenType.Typeof => a.Class.ToString(),
            _ => throw new LangException(error,
                $"Got invalid unary operator type: {operatorType}.")
        };
    }

    private static SealValue EvaluteMinus(SourceLocation error, SealValue a)
    {
        if (a.ValueType == SealValueType.Number)
            return -a.AsNumber();

        throw new LangException(error,
            $"No minus overload found for -{a.Class}.");
    }
}