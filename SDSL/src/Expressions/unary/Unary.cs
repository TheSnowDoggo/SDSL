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
            TokenType.Minus  => EvaluateMinus(error, a),
            TokenType.Plus   => EvaluatePlus(error, a),
            TokenType.Not    => !a.ToBool(),
            _ => throw new RuntimeException(error,
                $"Tried to evaluate invalid unary operator type: {operatorType}."),
        };
    }

    private static SealValue EvaluateMinus(SourceLocation error, SealValue a)
    {
        if (a.ValueType == SealValueType.Number)
        {
            return -a.AsDouble();
        }

        if (a.ValueType == SealValueType.TimeSpan)
        {
            return -a.AsTimeSpan();
        }

        return EvaluateOverload(error, a, "_minus");
    }
    
    private static SealValue EvaluatePlus(SourceLocation error, SealValue a)
    {
        if (a.ValueType == SealValueType.Number)
        {
            return +a.AsDouble();
        }

        return EvaluateOverload(error, a, "_plus");
    }

    private static SealValue EvaluateOverload(SourceLocation error, SealValue a, string name)
    {
        if (a.Class.TryGetFunction(name, out Function function)
            && function.MinArgs == 0)
        {
            try
            {
                return function.MemberInvoke(a);
            }
            catch (Exception ex)
            {
                throw new RuntimeException(error,
                    $"[overload] {a.ToString(true)}->()\n  --> {ex.Message}");
            }
        }

        throw new RuntimeException(error,
            $"No {name} overload found for {a.Class}.");
    }
}