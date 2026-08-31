using SDSL.Statements;
using SDSL.Classes;

namespace SDSL.Functions;

public class UserFunction : Function
{
    public UserFunction(
        SourceLocation location,
        SealClass sClass,
        string name,
        FunctionArgument[] args,
        int minArgs,
        int maxArgs,
        SealClass returnType,
        bool isStatic,
        Statement[] statements,
        int variables)
    {
        Class = sClass;
        Name = name;
        Args = args;
        MinArgs = minArgs;
        MaxArgs = maxArgs;
        ReturnType = returnType;
        IsStatic = isStatic;
        Location = location;
        Statements = statements;
        Variables = variables;
    }
    
    public Statement[] Statements { get; }
    public int Variables { get; }
    
    protected override SealValue _Invoke(SealValue self, params SealValue[] args)
    {
        var variables = new Variable[Variables];
        
        DeclareArguments(self, args, variables);
        
        for (int i = 0; i < Statements.Length; i++)
        {
            Statement statement = Statements[i];

            ReturnValue returnValue = statement.Invoke(variables);

            switch (returnValue.ReturnValueType)
            {
            case ReturnValueType.None:
                break;
            case ReturnValueType.Return:
                if (ReturnType != null
                    && returnValue.Value.Class != ReturnType)
                {
                    throw new RuntimeException(statement,
                        $"{FullName} expected return type {ReturnType}, but tried to return {returnValue.Value.Class}.");
                }
                
                return returnValue.Value;
            default:
                throw new RuntimeException(statement,
                    $"{FullName} got invalid return value type: {returnValue.ReturnValueType}.");
            }
        }
        
        if (ReturnType == null || ReturnType == SealNil.Class)
        {
            return SealValue.Nil;
        }
        
        throw new RuntimeException(Location,
            $"{FullName} expected return type {ReturnType}, but function ended before returning.");
    }

    private void DeclareArguments(SealValue self, SealValue[] args, Variable[] variables)
    {
        int variable = 0;

        if (!IsStatic)
        {
            variables[variable++] = new Variable(Class, self);
        }

        int i = 0;
        
        for (; i < args.Length; i++)
        {
            FunctionArgument fArg = Args[i];
            
            variables[variable++] = new Variable(fArg.Class, args[i]);
        }

        for (; i < Args.Length; i++)
        {
            FunctionArgument fArg = Args[i];
            SealValue defaultValue = fArg.Expression.Evaluate(null);
            
            variables[variable++] = new Variable(fArg.Class, defaultValue);
        }
    }
}