namespace SDSL.Functions;

public class UserConstructor : Function
{
    public UserConstructor(
        SourceLocation location,
        SealClass sClass,
        FunctionArgument[] args,
        int minArgs,
        int maxArgs,
        UserFunction function)
    {
        Location = location;
        Class = sClass;
        Name = "new";
        Args = args;
        MinArgs = minArgs;
        MaxArgs = maxArgs;
        ReturnType = sClass;
        IsStatic = true;
        Function = function;
    }
    
    public UserFunction Function { get; }
    
    protected override SealValue _Invoke(SealValue self, params SealValue[] args)
    {
        int length = Class.InstanceFields.Length;
        
        var fields = new Field[length];

        for (int i = 0; i < length; i++)
        {
            FieldDefinition fd = Class.InstanceFields[i];

            SealValue defaultValue = fd.Expression == null
                ? SealClass.GetDefaultValue(fd.Class)
                : fd.Expression.Evaluate(null);

            bool isConst = Function == null && fd.IsConst;
            
            fields[i] = new Field(fd.Class, isConst, defaultValue);
        }
        
        var instance = new SealUserObject(Class, fields);
        
        var value = new SealValue(instance);

        if (Function == null)
        {
            return value;
        }
        
        Function.MemberInvoke(value, args);

        // All fields are initialized to not-const so they can be set in the constructor
        // After the user constructor is ran, we can set them to what they should be
        for (int i = 0; i < length; i++)
        {
            fields[i].IsConst = Class.InstanceFields[i].IsConst;
        }
        
        return value;
    }
}