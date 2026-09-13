using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace SDSL;

public readonly struct Variant : IEquatable<Variant>, IFormattable
{
	private static readonly FrozenDictionary<Type, VariantType> TypeMap = new Dictionary<Type, VariantType>()
	{
		{ typeof(Variant), VariantType.Nil    },
		{ typeof(bool)   , VariantType.Bool   },
		{ typeof(double) , VariantType.Number },
		{ typeof(string) , VariantType.String },
	}.ToFrozenDictionary();
	
	private readonly double _double;
	private readonly object _object;
	private readonly VariantType _variantType;

	public Variant(bool value)
	{
		_double = value ? 1 : 0;
		_variantType = VariantType.Bool;
	}
	
	public Variant(double value)
	{
		_double = value;
		_variantType = VariantType.Number;
	}
	
	public Variant(string value)
	{
		_object = value;
		_variantType = VariantType.String;
	}
	
	public Variant(VariantObject value)
	{
		_object = value;
		_variantType = VariantType.Object;
	}

	public static Variant Nil { get; } = new Variant();

	public VariantType VariantType => _variantType;

	public VariantClass Class => _variantType switch
	{
		VariantType.Nil    => NilClass.Class,
		VariantType.Bool   => BoolClass.Class,
		VariantType.Number => NumberClass.Class,
		VariantType.String => StringClass.Class,
		VariantType.Object => AsVariantObject().Class,
		_ => throw new InvalidOperationException($"Had invalid Variant type {_variantType}."),
	};

	public static implicit operator Variant(bool value)
		=> new Variant(value);
	
	public static implicit operator Variant(double value)
		=> new Variant(value);
	
	public static implicit operator Variant(string value)
		=> new Variant(value);
	
	public static implicit operator Variant(VariantObject value)
		=> new Variant(value);

	public static bool operator ==(Variant left, Variant right) => left.Equals(right);
	public static bool operator !=(Variant left, Variant right) => !left.Equals(right);

	public static Variant FromObject(object obj)
	{
		Type type = obj.GetType();
		
		if (TypeMap.TryGetValue(type, out VariantType variantType))
		{
			return variantType switch
			{
				VariantType.Nil    => (Variant)obj,
				VariantType.Bool   => (bool)obj,
				VariantType.Number => (double)obj,
				VariantType.String => (string)obj,
				_ => throw new InvalidOperationException($"Got invalid Variant type {variantType}."),
			};
		}

		if (type.IsAssignableTo(typeof(VariantObject)))
		{
			return (VariantObject)obj;
		}

		throw new ArgumentException($"Cannot create Variant from object of type {type}.");
	}
	
	public bool AsBool()
	{
		return _double != 0;
	}

	public double AsDouble()
	{
		return _double;
	}
	
	public float AsSingle()
	{
		return (float)_double;
	}
	
	public int AsInt32()
	{
		return (int)_double;
	}

	public string AsString()
	{
		return (string)_object;
	}
	
	public VariantObject AsVariantObject()
	{
		return (VariantObject)_object;
	}
	
	public TObject AsVariantObject<TObject>()
		where TObject : VariantObject
	{
		return (TObject)_object;
	}

	public override string ToString()
	{
		return ToString(null, null);
	}

	public string ToString(string format, IFormatProvider formatProvider)
	{
		return _variantType switch
		{
			VariantType.Nil    => "nil",
			VariantType.Bool   => _double != 0 ? "true" : "false",
			VariantType.Number => _double.ToString(format, formatProvider),
			VariantType.String => (string)_object,
			VariantType.Object => AsVariantObject().ToString(),
			_ => throw new InvalidOperationException($"Had invalid Variant type {_variantType}."),
		};
	}

	public string ToString(string format)
	{
		return ToString(format, null);
	}
	
	public string ToString(IFormatProvider formatProvider)
	{
		return ToString(null, formatProvider);
	}
	
	// Will avoid using user implemented string conversions
	public string ToSafeString()
	{
		return _variantType switch
		{
			VariantType.Nil    => "nil",
			VariantType.Bool   => _double != 0 ? "true" : "false",
			VariantType.Number => _double.ToString(CultureInfo.InvariantCulture),
			VariantType.String => AsString().ToEscapePreview(),
			VariantType.Object => AsVariantObject().ToSafeString(),
			_ => throw new InvalidOperationException($"Had invalid Variant type {_variantType}."),
		};
	}

	public bool Equals(Variant other)
	{
		if (_variantType != other._variantType)
		{
			return false;
		}

		return _variantType switch
		{
			VariantType.Nil => true,
			VariantType.Bool or VariantType.Number => _double == other._double,
			VariantType.String => AsString().Equals(other.AsString(), StringComparison.Ordinal),
			VariantType.Object => AsVariantObject().Equals(other.AsVariantObject()),
			_ => throw new InvalidOperationException($"Had invalid Variant type {_variantType}."),
		};
	}

	public override bool Equals(object obj)
	{
		return obj is Variant other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_double, _object, _variantType);
	}

	public bool IsAssignableTo([AllowNull] VariantClass variantClass)
	{
		// Null represents untyped/Any
		if (variantClass == null)
		{
			return true;
		}

		if (_variantType != variantClass.VariantType)
		{
			return false;
		}

		// The base class set contains the class itself
		return Class.BaseClassSet.Contains(variantClass);
	}
}