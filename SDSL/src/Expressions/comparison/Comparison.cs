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
            _ => throw new LangException(error,
                $"Got invalid comparison operator type: {operatorType}.")
        };
    }

    private static bool CompareLessThan(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.ValueType == ValueType.Number && b.ValueType == ValueType.Number)
            return a.AsNumber() < b.AsNumber();
        
        if (a.ValueType == ValueType.String && b.ValueType == ValueType.String)
            return string.Compare(a.AsString(), b.AsString(), StringComparison.Ordinal) < 0;

        throw new LangException(error,
            $"No comparison operator defined between compare({a.ValueType}, {b.ValueType}).");
    }
}