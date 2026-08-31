using System.Text;
using SDSL.Classes;

namespace SDSL.Functions;

public abstract class Function : ISourceLocated
{
    public const string SelfName = "self";

    public const int AnyArgs = -1;
    
    public SourceLocation Location { get; protected init; }
    public SealClass Class { get; protected init; }
    public string Name { get; protected init; }
    public FunctionArgument[] Args { get; protected init; }
    public int MinArgs { get; protected init; }
    public int MaxArgs { get; protected init; }
    public SealClass ReturnType { get; protected init; }
    public bool IsStatic { get; protected init; }
    
    public string FullName => $"{Class}.{Name}";

    public SealValue MemberInvoke(SealValue self, params SealValue[] args)
    {
        if (!IsStatic && self.Class != Class && Class != SealGlobal.Class)
        {
            throw new RuntimeException(Location,
                $"Member function {FullName} expected self parameter to be of type {Class}, got {self.Class}.");
        }
        
        ValidateArgs(args);
        
        return _Invoke(self, args);
    }

    public SealValue Invoke(params SealValue[] args)
    {
        if (!IsStatic)
        {
            throw new RuntimeException(Location,
                $"Member function {FullName} expected a self parameter.");
        }
        
        ValidateArgs(args);
        
        return _Invoke(SealValue.Nil, args);
    }
    
    protected abstract SealValue _Invoke(SealValue self, params SealValue[] args);

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

    private void ValidateArgs(SealValue[] args)
    {
        if (args.Length < MinArgs)
        {
            throw new RuntimeException(Location,
                $"Function {FullName} expected minimum of {MinArgs} arguments, got {args.Length}.");
        }

        if (MaxArgs >= 0 && args.Length > MaxArgs)
        {
            throw new RuntimeException(Location,
                $"Function {FullName} expected maximum of {MaxArgs} arguments, got {args.Length}.");
        }

        int length = Math.Min(args.Length, Args.Length);
        
        for (int i = 0; i < length; i++)
        {
            SealClass expectedClass = Args[i].Class;

            if (expectedClass != null && args[i].Class != expectedClass)
            {
                throw new RuntimeException(Location,
                    $"{FullName} expected argument {i} [{Args[i]}] to be of type {expectedClass}, got {args[i].Class}.");
            }
        }
    }
}