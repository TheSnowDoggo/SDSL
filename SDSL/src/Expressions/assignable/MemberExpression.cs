namespace SDSL.Expressions;

public class MemberExpression : AssignableExpression
{
    public MemberExpression(
        SourceLocation location,
        Expression instanceExpression,
        string identifier)
    {
        Location = location;
        InstanceExpression = instanceExpression;
        Identifier = identifier;
    }
    
    public Expression InstanceExpression { get; }
    public string Identifier { get; }
    
    public override SealValue Evaluate(SealAssembly assembly, Variable[] variables)
    {
        return GetValue(assembly, variables, out _);
    }

    public SealValue GetValue(SealAssembly assembly, Variable[] variables, out SealValue instance)
    {
        instance = InstanceExpression.Evaluate(assembly, variables);

        int location;

        if (instance.Class.FunctionTable.TryGetValue(Identifier, out location))
        {
            return assembly.Functions[location];
        }

        SealObject obj = GetInstanceObject(instance);

        if (obj is SealUserObject userObject
            && obj.TypeClass.FieldTable.TryGetValue(Identifier, out location))
        {
            return userObject.Fields[location].Value;
        }
        
        throw new LangException(Location,
            $"Class {obj.TypeClass} does not contain member function/field '{Identifier}'.");
    }

    public override void SetValue(SealAssembly assembly, Variable[] variables, SealValue value)
    {
        SealObject obj = GetInstanceObject(InstanceExpression.Evaluate(assembly, variables));

        if (obj is not SealUserObject userObj)
            throw new LangException(Location,
                $"Cannot set field from non-user defined class {obj.TypeClass}.");
        
        if (!userObj.TypeClass.FieldTable.TryGetValue(Identifier, out int location))
            throw new LangException(Location,
                $"Class {obj.TypeClass} does not contain member field '{Identifier}'.");
        
        ref Variable field = ref userObj.Fields[location];

        if (field.IsConst)
            throw new LangException(Location,
                $"Cannot set readonly instance field '{Identifier}' in class {obj.TypeClass}.");

        if (field.Class != null && field.Class != value.Class)
            throw new LangException(Location,
                $"Cannot set field of class {field.Class} to value of class {value.Class}.");
        
        field.Value = value;
    }

    public override string ToString()
    {
        return $"{InstanceExpression}.{Identifier}";
    }

    private SealObject GetInstanceObject(SealValue instance)
    {
        if (instance.ValueType != SealValueType.Object)
        {
            throw new LangException(Location,
                $"Expected an Object class, got {instance.ValueType}.");
        }

        return instance.AsSealObject();
    }
}