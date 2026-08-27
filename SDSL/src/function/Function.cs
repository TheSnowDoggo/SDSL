using System.Text;

namespace SDSL;

public abstract class Function
{
    public const string SelfName = "self";
    
    public SealAssembly Assembly { get; init; }
    public SealClass Class { get; init; }
    public string Name { get; init; }
    public FunctionArg[] Args { get; init; }
    public int MinArgs { get; init; }
    public SealClass ReturnType { get; init; }
    public bool IsStatic { get; init; }

    public SealValue Invoke(SealValue self, params ReadOnlySpan<SealValue> args)
    {
        if (!IsStatic && self.Class != Class)
            throw new ArgumentException($"{ToString()} expected self parameter to be of type {Class}.");
        
        ValidateArgs(args);
        
        return _Invoke(self, args);
    }

    public SealValue Invoke(params SealValue[] args)
    {
        if (!IsStatic)
            throw new ArgumentException($"{ToString()} expected self parameter as it's not static.");
        
        ValidateArgs(args);
        
        return _Invoke(SealValue.Nil, args);
    }
    
    protected abstract SealValue _Invoke(SealValue self, params ReadOnlySpan<SealValue> args);

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (IsStatic)
        {
            sb.Append("static ");
        }

        sb.Append("func ");
        sb.Append(Name);
        
        sb.Append('(');
        sb.AppendJoin<FunctionArg>(", ", Args);
        sb.Append(')');

        sb.Append(" -> ");
        sb.Append(ReturnType);
        
        return sb.ToString();
    }

    private void ValidateArgs(ReadOnlySpan<SealValue> args)
    {
        if (args.Length > Args.Length)
            throw new ArgumentException($"{ToString()} expected maximum of {Args.Length} arguments, got {args.Length}.");
        
        if (args.Length < MinArgs)
            throw new ArgumentException($"{ToString()} expected minimum of {MinArgs} arguments, got {args.Length}.");
    }
}