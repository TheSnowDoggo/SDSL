namespace SDSL.Prototypes;

public class UserPrototypeFunction : PrototypeFunction
{
    public UserPrototypeFunction(
        PrototypeClass @class,
        string name,
        PrototypeArgList argList,
        PrototypeDataType returnType,
        bool isStatic,
        ArraySegment<Token> tokens)
    : base(@class, name, argList, returnType, isStatic)
    {
        Tokens = tokens;
    }
    
    public ArraySegment<Token> Tokens { get; }
}