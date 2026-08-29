using System.Text;

namespace SDSL.Expressions;

public class MapExpression : Expression
{
    public MapExpression(
        SourceLocation location,
        Dictionary<Expression, Expression> itemExpressions)
    {
        Location = location;
        ItemExpressions = itemExpressions;
    }
    
    public Dictionary<Expression, Expression> ItemExpressions { get; }
    
    public override SealValue Evaluate(Variable[] variables)
    {
        var values = new Dictionary<SealValue, SealValue>();

        foreach (var kvp in ItemExpressions)
        {
            SealValue key = kvp.Key.Evaluate(variables);
            SealValue value = kvp.Value.Evaluate(variables);

            if (!values.TryAdd(key, value))
                throw new LangException(kvp.Key.Location,
                    $"Failed to initialize map: Got duplicate key {key}.");
        }
        
        return new SealMap(values);
    }

    public override string ToString()
    {
        switch (ItemExpressions.Count)
        {
        case 0:
            return "new Map{  }";
        default:
            var sb = new StringBuilder();
        
            sb.Append("new Map{ ");

            foreach (var kvp in ItemExpressions)
            {
                sb.Append(kvp.Key);
                sb.Append(": ");
                sb.Append(kvp.Value);
                sb.Append(", ");
            }

            sb[^2] = ' ';
            sb[^1] = '}';
            
            return sb.ToString();
        }
    }
}