using SDSL.Prototypes;

namespace SDSL;

public class NativeFunction : Function
{
    public NativeFunction(Func<SealValue, ReadOnlySpan<SealValue>, SealValue> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        Func = func;
    }
    
    public Func<SealValue, ReadOnlySpan<SealValue>, SealValue> Func { get; }
    
    public static NativeFunction Create(
        PrototypeFunction pFunction,
        Func<SealValue, ReadOnlySpan<SealValue>, SealValue> func)
    {
        PrototypeArgument[] pArgs = pFunction.ArgList.Args;
        int length = pArgs.Length;
        
        var args = new FunctionArgument[length];

        for (int i = 0; i < length; i++)
        {
            PrototypeArgument pArgument = pArgs[i];

            SealClass pClass = pFunction.Class.ResolveDataTypeClass(pArgument.DataType);
            
            args[i] = new FunctionArgument(
                pArgument.Name,
                pClass,
                null,
                false
            );
        }
        
        SealClass returnType = pFunction.Class.ResolveDataTypeClass(pFunction.ReturnType);
        
        return new NativeFunction(func)
        {
            Class = pFunction.Class.Class,
            Name = pFunction.Name,
            Args = args,
            MinArgs = pFunction.ArgList.MinArgs,
            MaxArgs = pFunction.ArgList.MaxArgs,
            ReturnType = returnType,
            IsStatic = pFunction.IsStatic
        };
    }
    
    protected override SealValue _Invoke(SealValue self, params ReadOnlySpan<SealValue> args)
    {
        return Func.Invoke(self, args);
    }
}