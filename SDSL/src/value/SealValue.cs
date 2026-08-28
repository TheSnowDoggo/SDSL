using System.Globalization;

namespace SDSL;

public readonly struct SealValue : IEquatable<SealValue>
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

    public SealValue(SealObject value)
    {
        _class = value.Class;
        _obj = value;
    }
    
    public static readonly SealValue Nil = new SealValue();
    
    public SealClass Class => _class ?? SealClass.Nil;

    public static implicit operator SealValue(bool value)
        => new SealValue(value);
    public static implicit operator SealValue(double value)
        => new SealValue(value);
    public static implicit operator SealValue(string value)
        => new SealValue(value);
    public static implicit operator SealValue(Function value)
        => new SealValue(value);
    public static implicit operator SealValue(SealObject value)
        => new SealValue(value);

    public static explicit operator bool(SealValue value)
        => value._value != 0;
    public static explicit operator double(SealValue value)
        => value._value;
    public static explicit operator string(SealValue value)
        => (string)value._obj;
    public static explicit operator Function(SealValue value)
        => (Function)value._obj;
    public static explicit operator SealObject(SealValue value)
        => (SealObject)value._obj;

    public static SealValue FromObject(object obj) => obj switch
    {
        bool boolValue         => boolValue,
        double doubleValue     => doubleValue,
        string stringValue     => stringValue,
        Function functionValue => functionValue,
        SealObject sealValue   => sealValue,
        _ => Nil,
    };

    public bool AsBool()
        => _value != 0;

    public double AsNumber()
        => _value;

    public string AsString()
        => (string)_obj;
    
    public Function AsFunction()
        => (Function)_obj;

    public SealObject AsSealObject()
        => (SealObject)_obj;
    
    public bool InterpretAsBool() => Class.GetTypeCatagory() switch
    {
        TypeCatagory.Nil
            => false,
        TypeCatagory.Bool or TypeCatagory.Number
            => _value != 0,
        TypeCatagory.String
            => AsString().Length != 0,
        _ => true
    };

    public bool Equals(SealValue other)
    {
        if (Class != other.Class)
            return false;

        return Class.GetTypeCatagory() switch
        {
            TypeCatagory.Nil
                => true,
            TypeCatagory.Bool or TypeCatagory.Number
                => _value == other._value,
            _ => Equals(_obj, other._obj),
        };
    }

    public override bool Equals(object obj)
    {
        return obj is SealValue value && Equals(value);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Class, _value, _obj);
    }

    public override string ToString()
    {
        return ToString(true);
    }

    public string ToString(bool useRawString) => Class.GetTypeCatagory() switch
    {
        TypeCatagory.Nil
            => "nil",
        TypeCatagory.Bool
            => _value != 0 ? "true" : "false",
        TypeCatagory.Number
            => _value.ToString(CultureInfo.InvariantCulture),
        TypeCatagory.String
            => useRawString ? AsString() : AsString().ToEscapePreview(),
        _ => _obj.ToString()
    };
}