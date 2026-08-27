namespace SDSL.Prototypes;

public class NativePrototypeFunction : PrototypeFunction
{
    public NativePrototypeFunction(
        PrototypeClass @class,
        string name,
        PrototypeArgList argList,
        PrototypeDataType returnType,
        bool isStatic,
        Func<SealValue, ReadOnlySpan<SealValue>, SealValue> func)
        : base(@class, name, argList, returnType, isStatic)
    {
        Func = func;
    }
    
    public Func<SealValue, ReadOnlySpan<SealValue>, SealValue> Func { get; }

    public NativeFunction GenerateFunction(SealAssembly assembly)
    {
        PrototypeArg[] prototypeArgs = ArgList.Args;
        int length = prototypeArgs.Length;
        
        var args = new FunctionArg[length];

        for (int i = 0; i < length; i++)
        {
            PrototypeArg prototypeArg = prototypeArgs[i];

            SealClass @class = Class.ResolveDataTypeClass(prototypeArg.DataType);
            
            args[i] = new FunctionArg(
                prototypeArg.Name,
                @class,
                null,
                false
            );
        }
        
        SealClass returnType = Class.ResolveDataTypeClass(ReturnType);
        
        return new NativeFunction(Func)
        {
            Assembly = assembly,
            Class = Class.Class,
            Name = Name,
            Args = args,
            MinArgs = ArgList.MinArgs,
            ReturnType = returnType,
            IsStatic = IsStatic,
        };
    }
}