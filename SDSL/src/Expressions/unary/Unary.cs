using SDSL.Functions;

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
            _ => throw new RuntimeException(error,
                $"Tried to evaluate invalid unary operator type: {operatorType}."),
        };
    }

    private static SealValue EvaluteMinus(SourceLocation error, SealValue a)
    {
        if (a.ValueType == SealValueType.Number)
        {
            return -a.AsDouble();
        }

        if (a.ValueType == SealValueType.TimeSpan)
        {
            return -a.AsTimeSpan();
        }

        if (a.Class.TryGetFunction("_minus", out Function function)
            && function.MinArgs == 0)
        {
            try
            {
                return function.MemberInvoke(a);
            }
            catch (Exception ex)
            {
                throw new RuntimeException(error,
                    $"{a.ToString(true)}->()\n  --> {ex.Message}");
            }
        }

        throw new RuntimeException(error,
            $"No minus overload found for -{a.Class}.");
    }
}