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
    
    public string Name { get; }
    public Function[] Functions { get; }
    public Variable[] Fields { get; }
    
    public UserFunction EntryPoint { get; set; }
}