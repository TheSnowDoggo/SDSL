namespace SDSL.Expressions;

public class ReferenceExpression : AssignableExpression
{
    private readonly ReferenceType _referenceType;
    private readonly int _index;

    public ReferenceExpression(
        SourceLocation location,
        ReferenceType referenceType,
        int index)
    {
        Location = location;
        _referenceType = referenceType;
        _index = index;
    }
    
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        return _referenceType switch
        {
            ReferenceType.Local
                => variables[_index].Value,
            ReferenceType.StaticFunction
                => assembly.Functions[_index],
            ReferenceType.StaticField
                => assembly.Fields[_index].Value,
            _ => throw new LangException(Location,
                $"Cannot get reference type {_referenceType}.")
        };
    }

    public override void SetValue(SealAssembly assembly, Variable[] variables, SealValue value)
    {
        switch (_referenceType)
        {
        case ReferenceType.Local:
            TryAssign(ref variables[_index], value);
            break;
        case ReferenceType.StaticFunction:
            throw new LangException(Location,
                "Cannot assign to a static function.");
        case ReferenceType.StaticField:
            TryAssign(ref assembly.Fields[_index], value);
            break;
        default:
            throw new LangException(Location,
                $"Cannot set reference type {_referenceType}.");
        }
    }
    
    public override string ToString()
    {
        return $"{_referenceType}[{_index}]";
    }

    private void TryAssign(ref Variable variable, SealValue value)
    {
        if (variable.IsConst)
        {
            throw new LangException(Location,
                $"{_referenceType}[{_index}] cannot be assigned to as it is const.");
        }

        if (variable.Class != null && variable.Class != value.Class)
        {
            throw new LangException(Location,
                $"{_referenceType}[{_index}] expected value of type {variable.Class}, got {variable.Class}.");
        }
            
        variable.Value = value;
    }
}