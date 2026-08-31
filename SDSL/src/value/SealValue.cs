using System.Globalization;
using System.Runtime.CompilerServices;
using SDSL.Classes;
using SDSL.Functions;

namespace SDSL;

public readonly struct SealValue : IEquatable<SealValue>
{
    private readonly ValueType _valueType;
    private readonly object _obj;
    private readonly double _value;

    public SealValue(bool value)
    {
        _valueType = ValueType.Bool;
        _value = value ? 1 : 0;
    }

    public SealValue(double value)
    {
        _valueType = ValueType.Number;
        _value = value;
    }

    public SealValue(DateTime value)
    {
        _valueType = ValueType.DateTime;
        _value = Unsafe.BitCast<DateTime, double>(value);
    }
    
    public SealValue(TimeSpan value)
    {
        _valueType = ValueType.TimeSpan;
        _value = Unsafe.BitCast<TimeSpan, double>(value);
    }
    
    public SealValue(string value)
    {
        _valueType = ValueType.String;
        _obj = value;
    }
    
    public SealValue(Function value)
    {
        _valueType = ValueType.Function;
        _obj = value;
    }

    public SealValue(SealObject value)
    {
        _valueType = ValueType.Object;
        _obj = value;
    }
    
    public static readonly SealValue Nil = new SealValue();
    
    public ValueType ValueType => _valueType;

    public SealClass Class => _valueType switch
    {
        ValueType.Nil      => SealNil.Class,
        ValueType.Bool     => SealBool.Class,
        ValueType.Number   => SealNumber.Class,
        ValueType.DateTime => SealDateTime.Class,
        ValueType.TimeSpan => SealTimeSpan.Class,
        ValueType.String   => SealString.Class,
        ValueType.Function => SealFunction.Class,
        ValueType.Object   => AsSealObject().TypeClass,
        _ => throw new InvalidOperationException($"Value type {_valueType} is invalid.")
    };
    
    public static bool operator ==(SealValue left, SealValue right) => left.Equals(right);
    public static bool operator !=(SealValue left, SealValue right) => !left.Equals(right);

    public static implicit operator SealValue(bool value)
        => new SealValue(value);
    public static implicit operator SealValue(double value)
        => new SealValue(value);
    public static implicit operator SealValue(DateTime value)
        => new SealValue(value);
    public static implicit operator SealValue(TimeSpan value)
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
    public static explicit operator DateTime(SealValue value)
        => Unsafe.BitCast<double, DateTime>(value._value);
    public static explicit operator TimeSpan(SealValue value)
        => Unsafe.BitCast<double, TimeSpan>(value._value);
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
        DateTime dateTimeValue => dateTimeValue,
        TimeSpan timeSpanValue => timeSpanValue,
        Function functionValue => functionValue,
        SealObject sealValue   => sealValue,
        _ => Nil,
    };
    
    public bool AsBool()
        => _value != 0;

    public double AsNumber()
        => _value;
    
    public int AsInt32()
        => (int)_value;
    
    public DateTime AsDateTime()
        => Unsafe.BitCast<double, DateTime>(_value);
    
    public TimeSpan AsTimeSpan()
        => Unsafe.BitCast<double, TimeSpan>(_value);

    public string AsString()
        => (string)_obj;
    
    public Function AsFunction()
        => (Function)_obj;

    public SealObject AsSealObject()
        => (SealObject)_obj;

    public TObject AsSealObject<TObject>()
        where TObject : SealObject
    {
        return (TObject)_obj;
    }

    public object ToObject() => _valueType switch
    {
        ValueType.Nil      => null,
        ValueType.Bool     => AsBool(),
        ValueType.Number   => _value,
        ValueType.DateTime => AsDateTime(),
        ValueType.TimeSpan => AsTimeSpan(),
        _ => _obj,
    };
    
    public bool ToBool() => _valueType switch
    {
        ValueType.Nil
            => false,
        ValueType.Bool or ValueType.Number
            => _value != 0,
        ValueType.String
            => AsString().Length != 0,
        ValueType.Object
            => AsSealObject().ToBool(),
        _ => true,
    };

    public bool Equals(SealValue other)
    {
        if (_valueType != other._valueType)
            return false;
        
        return _valueType switch
        {
            ValueType.Nil
                => true,
            ValueType.Bool 
                or ValueType.Number
                or ValueType.DateTime
                or ValueType.TimeSpan
                => _value == other._value,
            ValueType.Object
                => AsSealObject().Equals(other.AsSealObject()),
            _ => Equals(_obj, other._obj),
        };
    }

    public bool RefEquals(SealValue other)
    {
        if (_valueType != other._valueType)
            return false;
        
        return _valueType switch
        {
            ValueType.Nil
                => true,
            ValueType.Bool
                or ValueType.Number
                or ValueType.DateTime
                or ValueType.TimeSpan
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
    
    public string ToString(bool useRawString) => _valueType switch
    {
        ValueType.Nil
            => "nil",
        ValueType.Bool
            => _value != 0 ? "true" : "false",
        ValueType.DateTime
            => AsDateTime().ToString(CultureInfo.InvariantCulture),
        ValueType.TimeSpan
            => AsTimeSpan().ToString(),
        ValueType.Number
            => _value.ToString(CultureInfo.InvariantCulture),
        ValueType.String
            => useRawString ? AsString() : AsString().ToEscapePreview(),
        _ => _obj.ToString()
    };
}