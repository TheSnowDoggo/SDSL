using SDSL.Statements;

namespace SDSL;

public class UserFunction : Function
{
    public UserFunction(Statement[] statements, int locations)
    {
        Statements = statements;
        Locations = locations;
    }
    
    public Statement[] Statements { get; }
    public int Locations { get; }
    
    protected override SealValue _Invoke(SealValue self, params ReadOnlySpan<SealValue> args)
    {
        var variables = new Variable[Locations];
        
        DeclareArguments(self, args, variables);
        
        for (int i = 0; i < Statements.Length; i++)
        {
            Statement statement = Statements[i];

            ReturnValue returnValue = statement.Invoke(Assembly, variables);

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
            SealValue value = arg.Expression.Evaluate(Assembly, null);
            
            variables[i] = new Variable(arg.Class, arg.IsConst, value);
        }
    }
}