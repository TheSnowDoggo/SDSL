using System.Text;
using SDSL.Native;

namespace SDSL.Expressions;

public class ArrayExpression : Expression
{
    public ArrayExpression(Expression[] itemExpressions)
    {
        ItemExpressions = itemExpressions;
    }
    
    public Expression[] ItemExpressions { get; }
    
    public override Variant Evaluate(Variable[] variables)
    {
        var items = new List<Variant>(ItemExpressions.Length);

        for (int i = 0; i < ItemExpressions.Length; i++)
        {
            items.Add(ItemExpressions[i].Evaluate(variables));
        }
        
        return new NativeArray(items);
    }
    
    public override string ToString()
    {
        switch (ItemExpressions.Length)
        {
        case 0:
            return "new Array[  ]";
        default:
            var sb = new StringBuilder();

            sb.Append("new Array[ ");

            for (int i = 0; i < ItemExpressions.Length; i++)
            {
                sb.Append(ItemExpressions[i]);
                sb.Append(", ");
            }

            sb[^2] = ' ';
            sb[^1] = ']';
            
            return sb.ToString();
        }
    }
}