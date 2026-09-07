using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class ForStatement : BlockStatement
{
    private readonly int _variableLocation;
    private readonly SealClass _variableClass;
    private readonly Expression _expression;
    
    public ForStatement(
        SourceLocation location,
        Statement[] statements,
        int variableLocation,
        SealClass variableClass,
        Expression expression)
    : base(location, statements)
    {
        _variableLocation = variableLocation;
        _variableClass = variableClass;
        _expression = expression;
    }
    
    public override ReturnValue Invoke(Variable[] variables)
    {
        SealValue enumerableValue = _expression.Evaluate(variables);

        variables[_variableLocation] = new Variable(_variableClass, default);
        
        ref Variable field = ref variables[_variableLocation];

        foreach (SealValue value in GetEnumerable(enumerableValue))
        {
            if (field.Class != null && field.Class != value.Class)
            {
                throw new RuntimeException(Location,
                    $"Loop variable expected value of type {field.Class}, got {value.Class}.");
            }
            
            field.Value = value;
            
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
        sb.Append(_variableLocation);

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

    private IEnumerable<SealValue> GetEnumerable(SealValue value)
    {
        switch (value.ValueType)
        {
        case ValueType.String:
            return GetStringEnumerable(value.AsString());
        case ValueType.Object:
            if (value.AsSealObject() is IEnumerable<SealValue> enumerable)
                return enumerable;
            break;
        }
        
        throw new RuntimeException(Location,
            $"Class {value.Class} is not enumerable.");
    }

    private static IEnumerable<SealValue> GetStringEnumerable(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            yield return s[i].ToString();
        }
    }
}