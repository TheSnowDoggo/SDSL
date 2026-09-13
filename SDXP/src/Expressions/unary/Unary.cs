namespace SDSL.Expressions;

public static class Unary
{
    public static Variant Evaluate(
        TokenType operatorType,
        SourceLocation error,
        Variant a)
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

    private static Variant EvaluateMinus(SourceLocation error, Variant a)
    {
        if (a.VariantType == VariantType.Number)
        {
            return -a.AsDouble();
        }

        if (a.VariantType == VariantType.TimeSpan)
        {
            return -a.AsTimeSpan();
        }

        return EvaluateOverload(error, a, "_minus");
    }
    
    private static Variant EvaluatePlus(SourceLocation error, Variant a)
    {
        if (a.VariantType == VariantType.Number)
        {
            return +a.AsDouble();
        }

        return EvaluateOverload(error, a, "_plus");
    }

    private static Variant EvaluateOverload(SourceLocation error, Variant a, string name)
    {
        if (a.Class.FunctionMap.TryGetValue(name, out Function function)
            && function.Signature.MinArgs == 0)
        {
            try
            {
                return function.MemberInvoke(a);
            }
            catch (Exception ex)
            {
                throw new RuntimeException(error,
                    $"[overload] {a.ToSafeString()}->()\n  --> {ex.Message}");
            }
        }

        throw new RuntimeException(error,
            $"No {name} overload found for {a.Class}.");
    }
}