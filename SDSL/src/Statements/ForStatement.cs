using System.Text;
using SDSL.Expressions;

namespace SDSL.Statements;

public class ForStatement : BlockStatement
{
    public ForStatement(
        SourceLocation location,
        Statement[] statements,
        int variableLocation,
        SealClass variableClass,
        Expression expression)
    : base(location, statements)
    {
        VariableLocation = variableLocation;
        VariableClass = variableClass;
        Expression = expression;
    }
    
    public int VariableLocation { get; }
    public SealClass VariableClass { get; }
    public Expression Expression { get; }
    
    public override ReturnValue Invoke(Variable[] variables)
    {
        SealValue enumerableValue = Expression.Evaluate(variables);

        variables[VariableLocation] = new Variable(VariableClass, true);

        foreach (SealValue value in GetEnumerable(enumerableValue))
        {
            ref Variable variable = ref variables[VariableLocation];
            
            if (variable.Class != null && variable.Class != value.Class)
                throw new LangException(Location,
                    $"Loop variable expected value of type {variable.Class}, got {value.Class}.");
            
            variable.Value = value;
            
            for (int i = 0; i < Statements.Length; i++)
            {
                ReturnValue returnValue = Statements[i].Invoke(variables);

                switch (returnValue.ReturnValueType)
                {
                case ReturnValueType.Return:
                    return returnValue;
                case ReturnValueType.Break:
                    return ReturnValue.None;
                case ReturnValueType.Continue:
                    i = Statements.Length; // skip to end
                    break;
                }
            }
        }
        
        return ReturnValue.None;
    }

    public override void Append(StringBuilder sb, int level)
    {
        sb.Append("for Local_");
        sb.Append(VariableLocation);

        if (VariableClass != null)
        {
            sb.Append(": ");
            sb.Append(VariableClass);
        }

        sb.Append(" in ");
        sb.Append(Expression);
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
        
        throw new LangException(Location,
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