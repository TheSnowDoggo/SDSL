using System.Text;
using SDSL.Native;

namespace SDSL.Expressions;

public class MapExpression : Expression
{
    public MapExpression(Dictionary<Expression, Expression> itemExpressions)
    {
        ItemExpressions = itemExpressions;
    }
    
    public Dictionary<Expression, Expression> ItemExpressions { get; }
    
    public override Variant Evaluate(Variable[] variables)
    {
        var values = new Dictionary<Variant, Variant>();

        foreach (var kvp in ItemExpressions)
        {
            Variant key = kvp.Key.Evaluate(variables);
            Variant value = kvp.Value.Evaluate(variables);

            if (!values.TryAdd(key, value))
            {
                throw new RuntimeException($"Failed to initialize map: Got duplicate key {key}.");
            }
        }
        
        return new NativeMap(values);
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