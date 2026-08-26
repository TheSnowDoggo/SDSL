namespace SDSL.Prototypes;

public class PrototypeArgList
{
    private readonly PrototypeArg[] _args;
    private readonly int _minArgs;
    
    public PrototypeArgList(PrototypeArg[] args, int minArgs)
    {
        _args = args;
        _minArgs = minArgs;
    }

    public PrototypeArg[] Args => _args;
    public int MinArgs => _minArgs;
    
    public static readonly PrototypeArgList Empty = new PrototypeArgList([], 0);
}