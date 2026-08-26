using System.Text;
using SDSL.Statements;

namespace SDSL;

public class Function
{
    public SealClass Class { get; init; }
    public string Name { get; init; }
    public int Locations { get; init; }
    public FunctionArg[] Args { get; init; }
    public int MinArgs { get; init; }
    public SealClass ReturnType { get; init; }
    public bool IsStatic { get; init; }
    public Statement[] Statements { get; init; }

    public SealValue Invoke(SealValue self, params ReadOnlySpan<SealValue> args)
    {
        var variables = new Variable[Locations];
        
        DeclareArguments(self, args, variables);
        
        for (int i = 0; i < Statements.Length; i++)
        {
            Statement statement = Statements[i];

            ReturnValue returnValue = statement.Invoke(null, variables);

            switch (returnValue.ReturnValueType)
            {
            case ReturnValueType.None:
                break;
            case ReturnValueType.Return:
                return returnValue.Value;
            default:
                throw new LangException(statement,
                    $"Got invalid return value type: {returnValue.ReturnValueType}.");
            }
        }

        return SealValue.Nil;
    }

    private void DeclareArguments(SealValue self, ReadOnlySpan<SealValue> args, Variable[] variables)
    {
        if (!IsStatic && self.Class != Class)
        {
            throw new ArgumentException($"{ToString()} expected self parameter to be of type {Class}.");
        }
        
        if (args.Length > Args.Length)
        {
            throw new ArgumentException($"{ToString()} expected maximum of {Args.Length} arguments, got {args.Length}.");
        }
        
        if (args.Length < MinArgs)
        {
            throw new ArgumentException($"{ToString()} expected minimum of {MinArgs} arguments, got {args.Length}.");
        }

        int i = 0;

        if (!IsStatic)
        {
            variables[i++] = new Variable(Class, true, self);
        }
        
        for (; i < args.Length; i++)
        {
            FunctionArg arg = Args[i];
            variables[i] = new Variable(arg.Class, arg.IsConst, args[i]);
        }

        for (; i < Args.Length; i++)
        {
            FunctionArg arg = Args[i];
            SealValue value = arg.Expression.Evaluate(null);
            
            variables[i] = new Variable(arg.Class, arg.IsConst, value);
        }
    }
    
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
}