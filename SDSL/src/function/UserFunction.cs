using SDSL.Statements;

namespace SDSL;

public class UserFunction : Function
{
    public UserFunction(
        SourceLocation location,
        Statement[] statements,
        int variables)
    {
        Location = location;
        Statements = statements;
        Variables = variables;
    }
    
    public SourceLocation Location { get; }
    public Statement[] Statements { get; }
    public int Variables { get; }
    
    protected override SealValue _Invoke(SealValue self, params ReadOnlySpan<SealValue> args)
    {
        var variables = new Variable[Variables];
        
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
                if (ReturnType != null
                    && returnValue.Value.Class != ReturnType)
                {
                    throw new LangException(statement,
                        $"{FullName} expected return type {ReturnType}, but tried to return {returnValue.Value.Class}.");
                }
                
                return returnValue.Value;
            default:
                throw new LangException(statement,
                    $"{FullName} got invalid return value type: {returnValue.ReturnValueType}.");
            }
        }
        
        if (ReturnType == null || ReturnType == SealClass.Nil)
        {
            return SealValue.Nil;
        }
        
        throw new LangException(Location,
            $"{FullName} expected return type {ReturnType}, but function ended before returning.");
    }

    private void DeclareArguments(SealValue self, ReadOnlySpan<SealValue> args, Variable[] variables)
    {
        int variable = 0;

        if (!IsStatic)
        {
            variables[variable++] = new Variable(Class, true, self);
        }

        int i = 0;
        
        for (; i < args.Length; i++)
        {
            FunctionArgument argument = Args[i];
            variables[variable++] = new Variable(argument.Class, argument.IsConst, args[i]);
        }

        for (; i < Args.Length; i++)
        {
            FunctionArgument argument = Args[i];
            SealValue value = argument.Expression.Evaluate(Assembly, null);
            
            variables[variable++] = new Variable(argument.Class, argument.IsConst, value);
        }
    }
}