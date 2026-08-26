namespace SDSL.Expressions;

public class ReferenceExpression : Expression
{
    private readonly ReferenceType _referenceType;
    private readonly int _index;

    public ReferenceExpression(ReferenceType referenceType, int index)
    {
        _referenceType = referenceType;
        _index = index;
    }
    
    public override SealValue Evaluate(SourceLocation error, SealAssembly assembly, Variable[] variables)
    {
        return _referenceType switch
        {
            ReferenceType.LocalVariable
                => variables[_index].Value,
            ReferenceType.StaticFunction
                => assembly.Functions[_index],
            ReferenceType.StaticField
                => assembly.Fields[_index].Value,
            _ => throw new LangException(error, $"Cannot get reference type {_referenceType}.")
        };
    }

    public void SetValue(SourceLocation error, SealAssembly assembly, Variable[] variables, SealValue value)
    {
        switch (_referenceType)
        {
        case ReferenceType.LocalVariable:
            TryAssign(error, ref variables[_index], value);
            break;
        case ReferenceType.StaticFunction:
            throw new LangException(error,
                "Cannot re-assign static function.");
        case ReferenceType.StaticField:
            TryAssign(error, ref assembly.Fields[_index], value);
            break;
        default:
            throw new LangException(error,
                $"Cannot get reference type {_referenceType}.");
        }
    }

    private static void TryAssign(SourceLocation error, ref Variable variable, SealValue value)
    {
        if (variable.IsConst)
        {
            throw new LangException(error,
                "Variable/Field cannot be assigned to as it is const.");
        }

        if (variable.Class != null && variable.Class != value.Class)
        {
            throw new LangException(error,
                $"Variable/Field expected value of type {variable.Class}, got {variable.Class}.");
        }
            
        variable.Value = value;
    }
}