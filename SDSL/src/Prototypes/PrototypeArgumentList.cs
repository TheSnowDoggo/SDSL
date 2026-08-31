namespace SDSL.Prototypes;

public class PrototypeArgumentList
{
    public PrototypeArgumentList(
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

    public static readonly PrototypeArgumentList Empty = new PrototypeArgumentList([], 0, 0);
}