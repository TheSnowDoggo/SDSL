namespace SDSL;

public readonly struct SealValue
{
    private readonly SealClass _class;
    private readonly object _obj;
    private readonly double _value;

    public SealValue(bool value)
    {
        _class = SealClass.Bool;
        _value = value ? 1 : 0;
    }

    public SealValue(double value)
    {
        _class = SealClass.Number;
        _value = value;
    }
    
    public SealValue(string value)
    {
        _class = SealClass.String;
        _obj = value;
    }
    
    public SealValue(Function value)
    {
        _class = SealClass.Function;
        _obj = value;
    }
    
    public static readonly SealValue Nil = new SealValue();
    
    public SealClass Class => _class ?? SealClass.Nil;

    public static implicit operator SealValue(bool value) => new SealValue(value);
    public static implicit operator SealValue(double value) => new SealValue(value);
    public static implicit operator SealValue(string value) => new SealValue(value);
    public static implicit operator SealValue(Function value) => new SealValue(value);

    public static explicit operator bool(SealValue value) => value._value != 0;
    public static explicit operator double(SealValue value) => value._value;
    public static explicit operator string(SealValue value) => (string)value._obj;
    public static explicit operator Function(SealValue value) => (Function)value._obj;

    public bool AsBool() => _value != 0;

    public double AsNumber() => _value;

    public string AsString() => (string)_obj;
    
    public Function AsFunction() => (Function)_obj;
}