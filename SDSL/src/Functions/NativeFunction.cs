using SDSL.Prototypes;

namespace SDSL.Functions;

public class NativeFunction : Function
{
    public NativeFunction(
        SealClass sClass,
        string name,
        FunctionArgument[] args,
        int minArgs,
        int maxArgs,
        SealClass returrnType,
        bool isStatic,
        Func<SealValue, SealValue[], SealValue> func)
    {
        Class = sClass;
        Name = name;
        Args = args;
        MinArgs = minArgs;
        MaxArgs = maxArgs;
        ReturnType = returrnType;
        IsStatic = isStatic;
        Func = func;
    }
    
    public Func<SealValue, SealValue[], SealValue> Func { get; }
    
    public static NativeFunction Create(
        PrototypeFunction pFunction,
        Func<SealValue, SealValue[], SealValue> func)
    {
        FunctionArgument[] arguments = CreateArguments(pFunction);
        
        SealClass returnType = pFunction.NativeClass.ResolveDataTypeSealClass(pFunction.ReturnType);

        return new NativeFunction(
            pFunction.NativeClass.Class,
            pFunction.Name,
            arguments,
            pFunction.ArgList.MinArgs,
            pFunction.ArgList.MaxArgs,
            returnType,
            pFunction.IsStatic,
            func
        );
    }
    
    protected override SealValue _Invoke(SealValue self, params SealValue[] args)
    {
        return Func.Invoke(self, args);
    }

    private static FunctionArgument[] CreateArguments(PrototypeFunction pFunction)
    {
        PrototypeArgument[] pArgs = pFunction.ArgList.Args;
        
        int length = pArgs.Length;
        
        var args = new FunctionArgument[length];

        for (int i = 0; i < length; i++)
        {
            PrototypeArgument pArgument = pArgs[i];

            SealClass pClass = pFunction.NativeClass.ResolveDataTypeSealClass(pArgument.DataType);
            
            args[i] = new FunctionArgument(
                pArgument.Name,
                pClass,
                null
            );
        }
        
        return args;
    }
}