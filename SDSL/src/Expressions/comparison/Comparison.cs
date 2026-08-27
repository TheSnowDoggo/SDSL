namespace SDSL.Expressions;

public static class Comparison
{
    public static SealValue Evaluate(
        ComparisonOperatorType operatorType,
        SourceLocation error,
        SealValue a,
        SealValue b)
    {
        return operatorType switch
        {
            ComparisonOperatorType.LessThan           => CompareLessThan(error, a, b),  // a < b
            ComparisonOperatorType.GreaterThan        => CompareLessThan(error, b, a),  // b < a
            ComparisonOperatorType.LessThanOrEqual    => !CompareLessThan(error, b, a), // !(b < a)
            ComparisonOperatorType.GreaterThanOrEqual => !CompareLessThan(error, a, b), // !(a < b)
            ComparisonOperatorType.Equal    => a.Equals(b),
            ComparisonOperatorType.NotEqual => !a.Equals(b),
            _ => throw new LangException(error,
                $"Got invalid comparison operator type: {operatorType}.")
        };
    }

    private static bool CompareLessThan(SourceLocation error, SealValue a, SealValue b)
    {
        if (a.Class == SealClass.Number && b.Class == SealClass.Number)
            return a.AsNumber() < b.AsNumber();
        
        if (a.Class == SealClass.String && b.Class == SealClass.String)
            return string.Compare(a.AsString(), b.AsString(), StringComparison.Ordinal) < 0;

        throw new LangException(error,
            $"No comparison operator defined between compare({a.Class}, {b.Class}).");
    }
}