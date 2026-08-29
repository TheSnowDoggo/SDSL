using System.Text;

namespace SDSL;

public abstract class Function : ISourceLocated
{
    public const string SelfName = "self";

    public const int AnyArgs = -1;
    
    public SourceLocation Location { get; init; }
    public SealClass Class { get; init; }
    public string Name { get; init; }
    public FunctionArgument[] Args { get; init; }
    public int MinArgs { get; init; }
    public int MaxArgs { get; init; }
    public SealClass ReturnType { get; init; }
    public bool IsStatic { get; init; }
    
    public string FullName => $"{Class}.{Name}";

    public SealValue MemberInvoke(SealValue self, params ReadOnlySpan<SealValue> args)
    {
        if (!IsStatic && self.Class != Class && Class != SealGlobal.Class)
            throw new LangException(Location,
                $"{ToString()} expected self parameter to be of type {Class}.");
        
        ValidateArgs(args);
        
        return _Invoke(self, args);
    }

    public SealValue Invoke(params ReadOnlySpan<SealValue> args)
    {
        if (!IsStatic)
            throw new LangException(Location,
                $"{ToString()} expected self parameter as it's not static.");
        
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

        sb.Append(Class);
        sb.Append('.');
        sb.Append(Name);
        
        sb.Append('(');
        sb.AppendJoin<FunctionArgument>(", ", Args);
        sb.Append(')');

        sb.Append(" -> ");

        if (ReturnType == null)
        {
            sb.Append("Any");
        }
        else
        {
            sb.Append(ReturnType);
        }
        
        return sb.ToString();
    }

    private void ValidateArgs(ReadOnlySpan<SealValue> args)
    {
        if (args.Length < MinArgs)
            throw new LangException(Location,
                $"{FullName} expected minimum of {MinArgs} arguments, got {args.Length}.");
        
        if (MaxArgs >= 0 && args.Length > MaxArgs)
            throw new LangException(Location,
                $"{FullName} expected maximum of {MaxArgs} arguments, got {args.Length}.");

        int length = Math.Min(args.Length, Args.Length);
        
        for (int i = 0; i < length; i++)
        {
            SealClass expectedClass = Args[i].Class;

            if (expectedClass != null
                && args[i].Class != expectedClass)
            {
                throw new LangException(Location,
                    $"{FullName} expected argument {i} [{Args[i]}] to be of type {expectedClass}, got {args[i].Class}.");
            }
        }
    }
}