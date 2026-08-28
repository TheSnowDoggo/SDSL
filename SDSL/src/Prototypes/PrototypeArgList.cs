namespace SDSL.Prototypes;

public class PrototypeArgList
{
    public PrototypeArgList(
        PrototypeArgument[] args,
        int minArgs,
        int maxArgs)
    {
        Args = args;
        MinArgs = minArgs;
        MaxArgs = maxArgs;
    }
    
    public PrototypeArgument[] Args { get; }
    public int MinArgs { get; }
    public int MaxArgs { get; }

    public static readonly PrototypeArgList Empty = new PrototypeArgList([], 0, 0);
}