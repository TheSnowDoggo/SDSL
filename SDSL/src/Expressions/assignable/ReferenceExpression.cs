using SDSL.Functions;

namespace SDSL.Expressions;

public class ReferenceExpression : AssignableExpression
{
    public ReferenceExpression(
        SourceLocation location,
        ReferenceType referenceType,
        int index)
    {
        Location = location;
        ReferenceType = referenceType;
        Index = index;
    }
    
    public ReferenceType ReferenceType { get; }
    public int Index { get; }
    
    public override SealValue Evaluate(Variable[] variables) => ReferenceType switch
    {
        ReferenceType.Local
            => variables[Index].Value,
        ReferenceType.StaticFunction
            => SealAssembly.Current.StaticFunctions[Index],
        ReferenceType.StaticField
            => SealAssembly.Current.StaticFields[Index].Value,
        _ => throw new RuntimeException(Location,
            $"Cannot get reference type {ReferenceType}."),
    };

    public override void SetValue(Variable[] variables, SealValue value)
    {
        switch (ReferenceType)
        {
        case ReferenceType.Local:
            TryAssignVariable(ref variables[Index], value);
            break;
        case ReferenceType.StaticField:
            TryAssignStaticField(ref SealAssembly.Current.StaticFields[Index], value);
            break;
        default:
            throw new RuntimeException(Location,
                $"Cannot assiign to a {ReferenceType}.");
        }
    }
    
    public override bool IsConstantEval()
    {
        return false;
    }
    
    public override string ToString()
    {
        return $"{ReferenceType}_{Index}";
    }

    private void TryAssignVariable(ref Variable variable, SealValue value)
    {
        if (variable.Class != null && variable.Class != value.Class)
        {
            throw new RuntimeException(Location,
                $"Variable {ToString()} expected value of type {variable.Class}, got {value.Class}.");
        }

        variable.Value = value;
    }

    private void TryAssignStaticField(ref Field field, SealValue value)
    {
        if (field.IsConst)
        {
            throw new RuntimeException(Location,
                $"Field {ToString()} cannot be assigned to as it is const.");
        }
        
        if (field.Class != null && field.Class != value.Class)
        {
            throw new RuntimeException(Location,
                $"Field {ToString()} expected value of type {field.Class}, got {value.Class}.");
        }
            
        field.Value = value;
    }
}