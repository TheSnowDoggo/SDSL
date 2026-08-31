using SDSL.Functions;
using SDSL.Classes;

namespace SDSL;

public class SealAssembly
{
    public SealAssembly(
        string name,
        Function[] staticFunctions,
        Field[] staticFields)
    {
        Name = name;
        StaticFunctions = staticFunctions;
        StaticFields = staticFields;
    }
    
    public static SealAssembly Current { get; set; }
    
    public string Name { get; }
    public Function[] StaticFunctions { get; }
    public Field[] StaticFields { get; }
    
    public UserFunction EntryPoint { get; set; }

    public SealValue RunMain(params List<SealValue> args)
    {
        if (EntryPoint == null)
            throw new InvalidOperationException("No entry point was defined.");
        
        if (EntryPoint.MinArgs == 0)
            return EntryPoint.Invoke();
        
        return EntryPoint.Invoke(new SealArray(args));
    }
    
    public SealValue RunMain(params ReadOnlySpan<string> strArgs)
    {
        var args = new List<SealValue>(strArgs.Length);
        
        for (int i = 0; i < strArgs.Length; i++)
            args.Add(strArgs[i]);
        
        return RunMain(args);
    }
}