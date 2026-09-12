namespace SDSL.Expressions;

public static class Comparison
{
    public static SealValue Evaluate(
        TokenType operatorType,
        SourceLocation error,
        SealValue a,
        SealValue b)
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

    private static bool CompareLessThan(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType != b.ValueType)
        {
            throw new RuntimeException(error,
                $"No comparison operator defined between compare({a.ValueType}, {b.ValueType}).");
        }

        return a.ValueType switch
        {
            SealValueType.Number   => a.AsDouble() < b.AsDouble(),
            SealValueType.String   => string.Compare(a.AsString(), b.AsString(), StringComparison.Ordinal) < 0,
            SealValueType.DateTime => a.AsDateTime() < b.AsDateTime(),
            SealValueType.TimeSpan => a.AsTimeSpan() < b.AsTimeSpan(),
            _ => throw new RuntimeException(error,
                $"No comparison operator defined between compare({a.ValueType}, {b.ValueType}).")
        };
    }
}