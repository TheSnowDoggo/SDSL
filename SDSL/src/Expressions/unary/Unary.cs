namespace SDSL.Expressions;

public static class Unary
{
    public static SealValue Evaluate(
        UnaryOperatorType operatorType,
        SourceLocation error,
        SealValue a)
    {
        return operatorType switch
        {
            UnaryOperatorType.Minus => EvaluteMinus(error, a),
            UnaryOperatorType.Not   => !a.InterpretAsBool(),
            _ => throw new LangException(error,
                $"Got invalid unary operator type: {operatorType}.")
        };
    }

    private static SealValue EvaluteMinus(SourceLocation error, SealValue a)
    {
        if (a.Class == SealClass.Number)
            return -a.AsNumber();

        throw new LangException(error,
            $"No minus overload found for -{a.Class}.");
    }
}