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
        if (!value.Class.IsAssignableTo(variable.Class))
        {
            throw new RuntimeException(Location,
                $"Value {value.Class} is not assignable to variable {ToString()} of class {variable.Class}.");
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

        if (!value.Class.IsAssignableTo(field.Class))
        {
            throw new RuntimeException(Location,
                $"Value {value.Class} is not assignable to field {ToString()} of class {field.Class}.");
        }
            
        field.Value = value;
    }
}