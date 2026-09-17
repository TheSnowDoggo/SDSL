namespace SDSL.Expressions;

public static class Unary
{
    public static Variant Evaluate(
        TokenType operatorType,
        Variant a)
    {
        return operatorType switch
        {
            TokenType.Minus  => EvaluateMinus(a),
            TokenType.Plus   => EvaluatePlus(a),
            TokenType.Not    => !a.ToBoolVolatile(),
            _ => throw new RuntimeException($"Tried to evaluate invalid unary operator type: {operatorType}."),
        };
    }

    private static Variant EvaluateMinus(Variant a)
    {
        if (a.VariantType == VariantType.Number)
        {
            return -a.AsDouble();
        }

        if (a.VariantType == VariantType.TimeSpan)
        {
            return -a.AsTimeSpan();
        }

        return EvaluateOverload(a, "_minus");
    }
    
    private static Variant EvaluatePlus(Variant a)
    {
        if (a.VariantType == VariantType.Number)
        {
            return +a.AsDouble();
        }

        return EvaluateOverload(a, "_plus");
    }

    private static Variant EvaluateOverload(Variant a, string name)
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
                throw new RuntimeException($"[overload] {a}->()\n  --> {ex.Message}");
            }
        }

        throw new RuntimeException($"No {name} overload found for {a.Class}.");
    }
}