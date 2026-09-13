namespace SDSL.Expressions;

public static class Comparison
{
    public static Variant Evaluate(
        TokenType operatorType,
        SourceLocation error,
        Variant a,
        Variant b)
    {
        return operatorType switch
        {
            TokenType.LessThan           => CompareLessThan(error, a, b),  // a < b
            TokenType.GreaterThan        => CompareLessThan(error, b, a),  // b < a
            TokenType.LessThanOrEqual    => !CompareLessThan(error, b, a), // !(b < a)
            TokenType.GreaterThanOrEqual => !CompareLessThan(error, a, b), // !(a < b)
            TokenType.Equal    => a.Equals(b),
            TokenType.NotEqual => !a.Equals(b),
            _ => throw new RuntimeException(error,
                $"Tried to evaluate invalid comparison operator type: {operatorType}."),
        };
    }

    private static bool CompareLessThan(SourceLocation error, Variant a, Variant b)
    {
        if (a.VariantType != b.VariantType)
        {
            throw new RuntimeException(error,
                $"No comparison operator defined between compare({a.VariantType}, {b.VariantType}).");
        }

        return a.VariantType switch
        {
            VariantType.Number   => a.AsDouble() < b.AsDouble(),
            VariantType.String   => string.Compare(a.AsString(), b.AsString(), StringComparison.Ordinal) < 0,
            VariantType.DateTime => a.AsDateTime() < b.AsDateTime(),
            VariantType.TimeSpan => a.AsTimeSpan() < b.AsTimeSpan(),
            _ => throw new RuntimeException(error,
                $"No comparison operator defined between compare({a.VariantType}, {b.VariantType}).")
        };
    }
}