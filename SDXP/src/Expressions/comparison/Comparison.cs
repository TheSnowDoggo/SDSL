namespace SDSL.Expressions;

public static class Comparison
{
    public static Variant Evaluate(
        TokenType operatorType,
        Variant a,
        Variant b)
    {
        return operatorType switch
        {
            TokenType.LessThan           =>  CompareLessThan(a, b),  // a < b
            TokenType.GreaterThan        =>  CompareLessThan(b, a),  // b < a
            TokenType.LessThanOrEqual    => !CompareLessThan(b, a), // !(b < a)
            TokenType.GreaterThanOrEqual => !CompareLessThan(a, b), // !(a < b)
            TokenType.Equal    =>  a.Equals(b, true),
            TokenType.NotEqual => !a.Equals(b, true),
            _ => throw new RuntimeException($"Tried to evaluate invalid comparison operator type: {operatorType}."),
        };
    }

    private static bool CompareLessThan(Variant a, Variant b)
    {
        if (a.VariantType != b.VariantType)
        {
            throw new RuntimeException(
                $"No comparison operator defined between compare({a.VariantType}, {b.VariantType}).");
        }

        return a.VariantType switch
        {
            VariantType.Number   => a.AsDouble() < b.AsDouble(),
            VariantType.String   => string.Compare(a.AsString(), b.AsString(), StringComparison.Ordinal) < 0,
            VariantType.DateTime => a.AsDateTime() < b.AsDateTime(),
            VariantType.TimeSpan => a.AsTimeSpan() < b.AsTimeSpan(),
            _ => throw new RuntimeException(
                $"No comparison operator defined between compare({a.VariantType}, {b.VariantType}).")
        };
    }
}