using System.Globalization;
using SDSL.Classes;
using SDSL.Functions;

namespace SDSL;

public readonly struct SealValue : IEquatable<SealValue>,
    IComparable<SealValue>
{
    private readonly SealValueType _valueType;
    private readonly double _value;
    private readonly object _obj;

    public SealValue(bool value)
    {
        _valueType = SealValueType.Bool;
        _value = value ? 1 : 0;
    }

    public SealValue(double value)
    {
        _valueType = SealValueType.Number;
        _value = value;
    }

    public SealValue(DateTime value)
    {
        _valueType = SealValueType.DateTime;
        _value = PackDateTime(value);
    }
    
    public SealValue(TimeSpan value)
    {
        _valueType = SealValueType.TimeSpan;
        _value = PackTimeSpan(value);
    }
    
    public SealValue(string value)
    {
        _valueType = SealValueType.String;
        _obj = value ?? string.Empty;
    }
    
    public SealValue(Function value)
    {
        _valueType = SealValueType.Function;
        _obj = value;
    }

    public SealValue(SealObject value)
    {
        _valueType = SealValueType.Object;
        _obj = value;
    }
    
    // Struct allows for custom value types (<= 8 bytes ofc)
    private SealValue(SealClass sClass, double value)
    {
        _valueType = SealValueType.Struct;
        _obj = sClass;
        _value = value;
    }
    
    public static readonly SealValue Nil = new SealValue();
    
    public SealValueType ValueType
    {
        get
        {
            return _valueType;
        }
    }

    public SealClass Class => _valueType switch
    {
        SealValueType.Nil      => SealNil.Class,
        SealValueType.Bool     => SealBool.Class,
        SealValueType.Number   => SealNumber.Class,
        SealValueType.DateTime => SealDateTime.Class,
        SealValueType.TimeSpan => SealTimeSpan.Class,
        SealValueType.String   => SealString.Class,
        SealValueType.Function => SealFunction.Class,
        SealValueType.Object   => AsSealObject().TypeClass,
        SealValueType.Struct   => (SealClass)_obj,
        _ => throw new InvalidOperationException($"Value type {_valueType} is invalid."),
    };

    public static bool operator ==(SealValue left, SealValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SealValue left, SealValue right)
    {
        return !left.Equals(right);
    }

    public static implicit operator SealValue(bool value)
    {
        return new SealValue(value);
    }

    public static implicit operator SealValue(double value)
    {
        return new SealValue(value);
    }

    public static implicit operator SealValue(DateTime value)
    {
        return new SealValue(value);
    }

    public static implicit operator SealValue(TimeSpan value)
    {
        return new SealValue(value);
    }

    public static implicit operator SealValue(string value)
    {
        return new SealValue(value);
    }

    public static implicit operator SealValue(Function value)
    {
        return new SealValue(value);
    }

    public static implicit operator SealValue(SealObject value)
    {
        return new SealValue(value);
    }

    public static SealValue FromObject(object obj)
    {
        if (GlobalConfig.TypeMaps.TryGetValue(obj.GetType(), out SealValueType valueType))
        {
            return valueType switch
            {
                SealValueType.Bool => (bool)obj,
                SealValueType.Number => (double)obj,
                SealValueType.DateTime => (DateTime)obj,
                SealValueType.TimeSpan => (TimeSpan)obj,
                SealValueType.String => (string)obj,
                SealValueType.Function => (Function)obj,
                _ => throw new InvalidOperationException(
                    $"Got unexepected value type {valueType} from type {obj.GetType()}."),
            };
        }

        if (obj is SealValue value)
        {
            return value;
        }

        if (obj is SealObject sealObject)
        {
            return sealObject;
        }

        throw new InvalidOperationException($"Cannot create SealValue from type {obj.GetType()}.");
    }

    public static SealValue CreateStruct(SealClass sClass, double value)
    {
        return new SealValue(sClass, value);
    }
    
    public bool AsBool()
    {
        return _value != 0;
    }

    public double AsDouble()
    {
        return _value;
    }
    
    public float AsSingle()
    {
        return (float)_value;
    }

    public int AsInt32()
    {
        return (int)_value;
    }

    public DateTime AsDateTime()
    {
        return UnpackDateTime(_value);
    }

    public TimeSpan AsTimeSpan()
    {
        return UnpackTimeSpan(_value);
    }

    public string AsString()
    {
        return (string)_obj;
    }

    public Function AsFunction()
    {
        return (Function)_obj;
    }

    public SealObject AsSealObject()
    {
        return (SealObject)_obj;
    }

    public TObject AsSealObject<TObject>()
        where TObject : SealObject
    {
        return (TObject)_obj;
    }

    public SealClass AsStructClass()
    {
        return (SealClass)_obj;
    }
    
    public object ToObject()
    {
        return _valueType switch
        {
            SealValueType.Nil      => null,
            SealValueType.Bool     => AsBool(),
            SealValueType.Number   => _value,
            SealValueType.DateTime => AsDateTime(),
            SealValueType.TimeSpan => AsTimeSpan(),
            SealValueType.Struct   => AsStructClass().StructObjectConverter?.Invoke(_value),
            _ => _obj,
        };
    }

    public bool ToBool()
    {
        return _valueType switch
        {
            SealValueType.Nil
                => false,
            SealValueType.Bool
                => _value != 0,
            SealValueType.Object
                => AsSealObject().ToBool(),
            _ => true,
        };
    }

    public bool Equals(SealValue other)
    {
        if (_valueType != other._valueType)
        {
            return false;
        }
        
        return _valueType switch
        {
            SealValueType.Nil
                => true,
            SealValueType.Bool 
                or SealValueType.Number
                or SealValueType.DateTime
                or SealValueType.TimeSpan
                or SealValueType.Struct
                => _value == other._value,
            SealValueType.Object
                => AsSealObject().Equals(other.AsSealObject()),
            _ => Equals(_obj, other._obj),
        };
    }

    public bool RefEquals(SealValue other)
    {
        if (_valueType != other._valueType)
        {
            return false;
        }
        
        return _valueType switch
        {
            SealValueType.Nil
                => true,
            SealValueType.Bool
                or SealValueType.Number
                or SealValueType.DateTime
                or SealValueType.TimeSpan
                or SealValueType.Struct
                => _value == other._value,
            _ => Equals(_obj, other._obj),
        };
    }
    
    public int CompareTo(SealValue other)
    {
        if (_valueType != other._valueType)
        {
            return 0;
        }

        return _valueType switch
        {
            SealValueType.Number   => _value.CompareTo(other._value),
            SealValueType.DateTime => AsDateTime().CompareTo(other.AsDateTime()),
            SealValueType.TimeSpan => AsTimeSpan().CompareTo(other.AsTimeSpan()),
            SealValueType.String   => string.Compare(AsString(), other.AsString(), StringComparison.Ordinal),
            _ => 0,
        };
    }

    public override bool Equals(object obj)
    {
        return obj is SealValue value && Equals(value);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_valueType, _value, _obj);
    }

    public override string ToString()
    {
        return ToString(false);
    }
    
    public string ToString(bool useSafeValue)
    {
        return _valueType switch
        {
            SealValueType.Nil
                => "nil",
            SealValueType.Bool
                => _value != 0 ? "true" : "false",
            SealValueType.DateTime
                => AsDateTime().ToString(CultureInfo.InvariantCulture),
            SealValueType.TimeSpan
                => AsTimeSpan().ToString(),
            SealValueType.Number
                => _value.ToString(CultureInfo.InvariantCulture),
            SealValueType.String
                => useSafeValue ? AsString().ToEscapePreview() : AsString(),
            SealValueType.Struct
                => StructToString(useSafeValue),
            _ => AsSealObject().ToString(useSafeValue),
        };
    }
    
    private string StructToString(bool useSafeValue)
    {
        SealClass sClass = AsStructClass();

        if (useSafeValue || sClass.StructStringConverter == null)
        {
            return $"struct<{sClass.FullName}>";
        }

        return sClass.StructStringConverter(_value);
    }

    private static unsafe double PackDateTime(DateTime value)
    {
        return *(double*)&value;
    }
    
    private static unsafe DateTime UnpackDateTime(double value)
    {
        return *(DateTime*)&value;
    }
    
    private static unsafe double PackTimeSpan(TimeSpan value)
    {
        return *(double*)&value;
    }
    
    private static unsafe TimeSpan UnpackTimeSpan(double value)
    {
        return *(TimeSpan*)&value;
    }
}