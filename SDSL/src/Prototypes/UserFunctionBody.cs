namespace SDSL.Prototypes;

public class UserFunctionBody : FunctionBody
{
    public UserFunctionBody(ArraySegment<Token> tokens)
    {
        Tokens = tokens;
    }
    
    public ArraySegment<Token> Tokens { get; }

    public override string ToString()
    {
        return $"UserFunctionBody[{Tokens.Count}]";
    }
}