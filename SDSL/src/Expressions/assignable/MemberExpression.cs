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

        if (instance.Class.FieldTable.TryGetValue(Identifier, out location))
        {
            return obj.Fields[location].Value;
        }
        
        throw new LangException(Location,
            $"Class {instance.Class} does not contain member function/field '{Identifier}'.");
    }

    public override void SetValue(SealAssembly assembly, Variable[] variables, SealValue value)
    {
        SealObject obj = GetInstanceObject(InstanceExpression.Evaluate(assembly, variables));
        
        if (!obj.Class.FieldTable.TryGetValue(Identifier, out int location))
        {
            throw new LangException(Location,
                $"Class {obj.Class} does not contain member field '{Identifier}'.");
        }
        
        ref Variable field = ref obj.Fields[location];

        if (field.IsConst)
        {
            throw new LangException(Location,
                $"Cannot set readonly instance field '{Identifier}' in class {obj.Class}.");
        }

        if (field.Class != null
            && field.Class != obj.Class)
        {
            throw new LangException(Location,
                $"Cannot set field of class {field.Class} to value of class {obj.Class}.");
        }
        
        field.Value = value;
    }

    public override string ToString()
    {
        return $"{InstanceExpression}.{Identifier}";
    }

    private SealObject GetInstanceObject(SealValue instance)
    {
        TypeCatagory catagory = instance.Class.GetTypeCatagory();

        if (catagory != TypeCatagory.Object)
        {
            throw new LangException(Location,
                $"Expected an Object class, got {catagory}.");
        }

        return instance.AsSealObject();
    }
}