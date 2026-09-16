using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class ForStatement : BlockStatement
{
    private readonly int _refLocation;
    private readonly VariantClass _variableClass;
    private readonly Expression _expression;
    
    public ForStatement(
        SourceLocation location,
        Statement[] statements,
        int refLocation,
        VariantClass variableClass,
        Expression expression)
    : base(location, statements)
    {
        _refLocation = refLocation;
        _variableClass = variableClass;
        _expression = expression;
    }
    
    public override ReturnValue Invoke(Variable[] variables)
    {
        Variant collection;

        try
        {
            collection = _expression.Evaluate(variables);
        }
        catch (Exception ex)
        {
            throw new RuntimeException(Location, 
                $"Failed to evaluate collection.\n  --> {ex.Message}", ex);
        }

        variables[_refLocation] = new Variable(_variableClass, default);
        
        ref Variable variable = ref variables[_refLocation];

        foreach (Variant value in GetEnumerable(collection))
        {
            if (!value.IsAssignableTo(_variableClass))
            {
                throw new RuntimeException(Location,
                    $"Loop value of type {value.Class} is not assignable to type {_variableClass}.");
            }
            
            variable.Value = value;
            
            for (int i = 0; i < _statements.Length; i++)
            {
                ReturnValue returnValue = _statements[i].Invoke(variables);

                switch (returnValue.ReturnValueType)
                {
                case ReturnValueType.Return:
                    return returnValue;
                case ReturnValueType.Break:
                    return ReturnValue.None;
                case ReturnValueType.Continue:
                    i = _statements.Length; // skip to end
                    break;
                }
            }
        }
        
        return ReturnValue.None;
    }

    public override void Append(StringBuilder sb, int level)
    {
        sb.Append("for Local_");
        sb.Append(_refLocation);

        if (_variableClass != null)
        {
            sb.Append(": ");
            sb.Append(_variableClass);
        }

        sb.Append(" in ");
        sb.Append(_expression);
        sb.AppendLine(" {");

        AppendStatements(sb, level + 1);
        
        sb.Append(' ', level * LevelSize);
        sb.Append('}');
    }

    private IEnumerable<Variant> GetEnumerable(Variant value)
    {
        switch (value.VariantType)
        {
        case VariantType.String:
            return GetStringEnumerable(value.AsString());
        case VariantType.Object:
            if (value.AsVariantObject() is IEnumerable<Variant> enumerable)
            {
                return enumerable;
            }
            break;
        }
        
        throw new RuntimeException(Location,
            $"Class {value.Class} is not enumerable.");
    }

    private static IEnumerable<Variant> GetStringEnumerable(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            yield return s[i].ToString();
        }
    }
}