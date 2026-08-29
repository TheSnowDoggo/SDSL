namespace SDSL;

public class SealAssembly
{
    public SealAssembly(
        string name,
        Function[] functions,
        Variable[] fields)
    {
        Name = name;
        Functions = functions;
        Fields = fields;
    }
    
    public static SealAssembly Current { get; set; }
    
    public string Name { get; }
    public Function[] Functions { get; }
    public Variable[] Fields { get; }
    
    public UserFunction EntryPoint { get; set; }

    public SealValue Run(params List<SealValue> args)
    {
        if (EntryPoint.MinArgs == 0)
            return EntryPoint.Invoke();
        
        return EntryPoint.Invoke(new SealArray(args));
    }
    
    public SealValue Run(params ReadOnlySpan<string> strArgs)
    {
        var args = new List<SealValue>(strArgs.Length);
        
        for (int i = 0; i < strArgs.Length; i++)
            args.Add(strArgs[i]);
        
        return Run(args);
    }
}