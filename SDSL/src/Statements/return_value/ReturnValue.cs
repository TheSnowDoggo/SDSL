namespace SDSL.Statements;

public readonly struct ReturnValue
{
    private readonly ReturnValueType _returnValueType;
    private readonly SealValue _value;
    
    public ReturnValue(ReturnValueType returnValueType, SealValue value = default)
    {
        _returnValueType = returnValueType;
        _value = value;
    }
    
    public static readonly ReturnValue None = new ReturnValue(ReturnValueType.None);
    
    public ReturnValueType ReturnValueType => _returnValueType;
    public SealValue Value => _value;
    
    public override string ToString()
    {
        return $"ReturnValue<{_returnValueType}>({_value})";
    }
}