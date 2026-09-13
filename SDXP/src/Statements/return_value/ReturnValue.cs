namespace SDSL.Statements;

public readonly struct ReturnValue
{
    private readonly ReturnValueType _returnValueType;
    private readonly Variant _value;
    
    public ReturnValue(ReturnValueType returnValueType, Variant value = default)
    {
        _returnValueType = returnValueType;
        _value = value;
    }
    
    public static ReturnValue None { get; } = new ReturnValue(ReturnValueType.None);
    public static ReturnValue Break { get; } = new ReturnValue(ReturnValueType.Break);
    public static ReturnValue Continue { get; } = new ReturnValue(ReturnValueType.Continue);
    
    public ReturnValueType ReturnValueType => _returnValueType;
    
    public Variant Value => _value;
    
    public override string ToString()
    {
        return $"ReturnValue<{_returnValueType}>({_value})";
    }
}