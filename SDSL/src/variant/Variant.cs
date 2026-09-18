using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SDSL.Native;

namespace SDSL;

/// <summary>
/// Represents a script value and its associated datatype.
/// </summary>
public readonly struct Variant : IEquatable<Variant>
{
	private readonly double _double;
	private readonly object _object;
	private readonly VariantType _variantType;

	/// <summary>
	/// Intializes a new instance of the <see cref="Variant"/> struct as a <see cref="VariantType.Bool"/>.
	/// </summary>
	/// <param name="value">The boolean value to store.</param>
	public Variant(bool value)
	{
		_double = value ? 1 : 0;
		_variantType = VariantType.Bool;
	}
	
	/// <summary>
	/// Intializes a new instance of the <see cref="Variant"/> struct as a <see cref="VariantType.Number"/>.
	/// </summary>
	/// <param name="value">The <see cref="double"/> value to store.</param>
	public Variant(double value)
	{
		_double = value;
		_variantType = VariantType.Number;
	}
	
	/// <summary>
	/// Intializes a new instance of the <see cref="Variant"/> struct as a <see cref="VariantType.DateTime"/>.
	/// </summary>
	/// <param name="value">The <see cref="DateTime"/> value to store.</param>
	public Variant(DateTime value)
	{
		_double = PackDateTime(value);
		_variantType = VariantType.DateTime;
	}
	
	/// <summary>
	/// Intializes a new instance of the <see cref="Variant"/> struct as a <see cref="VariantType.TimeSpan"/>.
	/// </summary>
	/// <param name="value">The <see cref="TimeSpan"/> value to store.</param>
	public Variant(TimeSpan value)
	{
		_double = PackTimeSpan(value);
		_variantType = VariantType.TimeSpan;
	}
	
	/// <summary>
	/// Intializes a new instance of the <see cref="Variant"/> struct as a <see cref="VariantType.String"/>.
	/// </summary>
	/// <param name="value">The <see cref="string"/> value to store.</param>
	public Variant(string value)
	{
		_object = value;
		_variantType = VariantType.String;
	}
	
	/// <summary>
	/// Intializes a new instance of the <see cref="Variant"/> struct as a <see cref="VariantType.Object"/>.
	/// </summary>
	/// <param name="value">The <see cref="string"/> value to store.</param>
	public Variant(VariantObject value)
	{
		_object = value;
		_variantType = VariantType.Object;
	}

	/// <summary>
	/// Gets the <see cref="VariantType.Nil"/> value.
	/// </summary>
	/// <remarks>
	/// The value is equivalent to the <see langword="default"/> value.
	/// </remarks>
	public static Variant Nil { get; } = new Variant();

	/// <summary>
	/// Gets the <see cref="VariantType"/> of the value.
	/// </summary>
	/// <remarks>
	/// For extended type information, get <see cref="Class"/>.
	/// </remarks>
	public VariantType VariantType => _variantType;

	/// <summary>
	/// Gets the associated <see cref="VariantClass"/> of the value.
	/// </summary>
	public VariantClass Class => _variantType switch
	{
		VariantType.Nil      => NilClass.Class,
		VariantType.Bool     => BoolClass.Class,
		VariantType.Number   => NumberClass.Class,
		VariantType.DateTime => DateTimeClass.Class,
		VariantType.TimeSpan => TimeSpanClass.Class,
		VariantType.String   => StringClass.Class,
		VariantType.Object   => AsVariantObject().ObjectClass,
		_ => throw new InvalidOperationException($"Had invalid Variant type {_variantType}."),
	};

	public static implicit operator Variant(bool value)
		=> new Variant(value);
	
	public static implicit operator Variant(double value)
		=> new Variant(value);
	
	public static implicit operator Variant(DateTime value)
		=> new Variant(value);
	
	public static implicit operator Variant(TimeSpan value)
		=> new Variant(value);
	
	public static implicit operator Variant(string value)
		=> new Variant(value);
	
	public static implicit operator Variant(VariantObject value)
		=> new Variant(value);

	public static bool operator ==(Variant left, Variant right) => left.Equals(right);
	public static bool operator !=(Variant left, Variant right) => !left.Equals(right);

	/// <summary>
	/// Creates a new <see cref="Variant"/> from the given <see cref="object"/> if it represents a valid type.
	/// </summary>
	/// <param name="obj">The object to create from.</param>
	/// <returns>A new <see cref="Variant"/> containing the associated <paramref name="obj"/> value.</returns>
	/// <exception cref="ArgumentException">Thrown if the given value does not represent a valid type.</exception>
	public static Variant FromObject(object obj)
	{
		Type type = obj.GetType();
		
		if (GlobalMaps.VariantTypeMap.TryGetValue(type, out VariantType variantType))
		{
			return variantType switch
			{
				VariantType.Nil      => (Variant)obj,
				VariantType.Bool     => (bool)obj,
				VariantType.Number   => (double)obj,
				VariantType.DateTime => (DateTime)obj,
				VariantType.TimeSpan => (TimeSpan)obj,
				VariantType.String   => (string)obj,
				_ => throw new InvalidOperationException($"Got invalid Variant type {variantType}."),
			};
		}

		if (type.IsAssignableTo(typeof(VariantObject)))
		{
			return (VariantObject)obj;
		}

		throw new ArgumentException($"Cannot create Variant from object of type {type}.");
	}
	
	/// <summary>
	/// Interprets the variant as a <see cref="VariantType.Bool"/>.
	/// </summary>
	/// <remarks>
	/// No type checking is performed, so only call if you know the type is <see cref="VariantType.Bool"/>.
	/// </remarks>
	/// <returns>The associated <see cref="bool"/> value.</returns>
	public bool AsBool()
	{
		return _double != 0;
	}

	/// <summary>
	/// Interprets the variant as a <see cref="VariantType.Number"/>.
	/// </summary>
	/// <remarks>
	/// No type checking is performed.
	/// </remarks>
	/// <returns>The associated double-precision floating-point value.</returns>
	public double AsDouble()
	{
		return _double;
	}
	
	/// <summary>
	/// Interprets the variant as a <see cref="VariantType.Number"/>, cast to a single-precision floating-point value.
	/// </summary>
	/// <remarks>
	/// No type checking is performed.
	/// </remarks>
	/// <returns>The associated single-precision floating-point value.</returns>
	public float AsSingle()
	{
		return (float)_double;
	}
	
	/// <summary>
	/// Interprets the variant as a <see cref="VariantType.Number"/>, cast to an <see cref="int"/>.
	/// </summary>
	/// <remarks>
	/// No type checking is performed.
	/// </remarks>
	/// <returns>The associated <see cref="int"/> value.</returns>
	public int AsInt32()
	{
		return (int)_double;
	}
	
	/// <summary>
	/// Interprets the variant as a <see cref="VariantType.DateTime"/>.
	/// </summary>
	/// <remarks>
	/// No type checking is performed.
	/// </remarks>
	/// <returns>The associated <see cref="DateTime"/> value.</returns>
	public DateTime AsDateTime()
	{
		return UnpackDateTime(_double);
	}
	
	/// <summary>
	/// Interprets the variant as a <see cref="VariantType.TimeSpan"/>.
	/// </summary>
	/// <remarks>
	/// No type checking is performed.
	/// </remarks>
	/// <returns>The associated <see cref="TimeSpan"/> value.</returns>
	public TimeSpan AsTimeSpan()
	{
		return UnpackTimeSpan(_double);
	}

	/// <summary>
	/// Interprets the variant as a <see cref="VariantType.String"/>.
	/// </summary>
	/// <remarks>
	/// No type checking is performed.
	/// </remarks>
	/// <returns>The associated <see cref="String"/> value.</returns>
	public string AsString()
	{
		return (string)_object;
	}
	
	/// <summary>
	/// Interprets the variant as a <see cref="VariantType.Object"/>.
	/// </summary>
	/// <remarks>
	/// No type checking is performed.
	/// </remarks>
	/// <returns>The associated <see cref="VariantObject"/> value.</returns>
	public VariantObject AsVariantObject()
	{
		return (VariantObject)_object;
	}
	
	/// <inheritdoc cref="AsVariantObject()"/>
	public TObject AsVariantObject<TObject>()
		where TObject : VariantObject
	{
		return (TObject)_object;
	}
	
	/// <summary>
	/// Tries to interpret the variant as a <see cref="VariantType.Object"/>.
	/// </summary>
	/// <param name="variantObject">The resulting <typeparamref name="TObject"/> or null if the variant is invalid.</param>
	/// <typeparam name="TObject">The <see cref="VariantObject"/> to cast to.</typeparam>
	/// <returns><see langword="true"/> if the variant is successfully interpreted; otherwise, <see langword="false"/>.</returns>
	public bool TryAsVariantObject<TObject>([NotNullWhen(true)] out TObject variantObject)
		where TObject : VariantObject
	{
		if (_variantType != VariantType.Object
		    || _object is not TObject obj)
		{
			variantObject = null;
			return false;
		}

		variantObject = obj;
		return true;
	}

	/// <summary>
	/// Returns a debug-friendly string representation of the current variant.
	/// </summary>
	/// <remarks>
	/// This method avoids user-defined string conversions for safe error messages.
	/// </remarks>
	/// <returns>A debug-friendly string that represents the current variant.</returns>
	public override string ToString() => _variantType switch
	{
		VariantType.Nil      => "nil",
		VariantType.Bool     => _double != 0 ? "true" : "false",
		VariantType.Number   => _double.ToString(CultureInfo.InvariantCulture),
		VariantType.DateTime => AsDateTime().ToString(CultureInfo.InvariantCulture),
		VariantType.TimeSpan => AsTimeSpan().ToString(null, CultureInfo.InvariantCulture),
		VariantType.String   => AsString().ToEscapePreview(),
		VariantType.Object   => AsVariantObject().ToString(),
		_ => throw new InvalidOperationException($"Had invalid Variant type {_variantType}."),
	};

	/// <summary>
	/// Returns a native or user defined string representation of the current variant.
	/// </summary>
	/// <param name="format">The format associated with the current <see cref="VariantType"/>.</param>
	/// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
	/// <returns>A native or user defined string that represents the current variant.</returns>
	public string ToStringVolatile(string format, IFormatProvider formatProvider) => _variantType switch
	{
		VariantType.Nil      => "nil",
		VariantType.Bool     => _double != 0 ? "true" : "false",
		VariantType.Number   => _double.ToString(format, formatProvider),
		VariantType.DateTime => AsDateTime().ToString(format, formatProvider),
		VariantType.TimeSpan => AsTimeSpan().ToString(format, formatProvider),
		VariantType.String   => (string)_object,
		VariantType.Object   => AsVariantObject().ToStringVolatile(),
		_ => throw new InvalidOperationException($"Had invalid Variant type {_variantType}."),
	};

	/// <summary>
	/// Returns a native or user defined string representation of the current variant.
	/// </summary>
	/// <returns>A native or user defined string that represents the current variant.</returns>
	public string ToStringVolatile()
	{
		return ToStringVolatile(null, null);
	}

	/// <summary>
	/// Converts the variant to a boolean value.
	/// </summary>
	/// <param name="isVolatile">Represents whether user-defined boolean conversions should be used.</param>
	/// <returns>The converted boolean value.</returns>
	public bool ToBool(bool isVolatile) => _variantType switch
	{
		VariantType.Nil    => false,
		VariantType.Bool   => AsBool(),
		VariantType.Object => isVolatile && AsVariantObject().ToBoolVolatile(),
		_ => true,
	};
	
	/// <inheritdoc/>
	public bool Equals(Variant other)
	{
		return Equals(other, false);
	}
	
	/// <summary>
	/// Indicates whether the current variant is equal to another variant.
	/// </summary>
	/// <param name="other">The other variant to compare to.</param>
	/// <param name="isVolatile">Represents whether user-defined equality should be used.</param>
	/// <returns><see langword="true"/> if the current variant is equal to the other variant; otherwise, <see langword="false"/>.</returns>
	public bool Equals(Variant other, bool isVolatile)
	{
		if (_variantType != other._variantType)
		{
			return false;
		}
		
		return _variantType switch
		{
			VariantType.Nil => true,
			VariantType.Bool or VariantType.Number => _double == other._double,
			VariantType.DateTime => AsDateTime().Equals(other.AsDateTime()),
			VariantType.TimeSpan => AsTimeSpan().Equals(other.AsTimeSpan()),
			VariantType.String   => AsString().Equals(other.AsString(), StringComparison.Ordinal),
			VariantType.Object   => isVolatile ? AsVariantObject().EqualsVolatile(other.AsVariantObject()) : _object == other._object,
			_ => throw new InvalidOperationException($"Had invalid Variant type {_variantType}."),
		};
	}

	/// <inheritdoc/>
	public override bool Equals(object obj)
	{
		return obj is Variant other && Equals(other);
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return HashCode.Combine(_double, _object, _variantType);
	}
	
	/// <summary>
	/// Indicates whether the variant is assignable to a variable/property of the given <see cref="VariantClass"/>.
	/// </summary>
	/// <param name="variantClass">The variant class to check against.</param>
	/// <returns><see langword="true"/> if the variant is assignable; otherwise, <see langword="false"/>.</returns>
	public bool IsAssignableTo([AllowNull] VariantClass variantClass)
	{
		return Class.IsAssignableTo(variantClass);
	}
	
	internal TObject NativeCast<TObject>()
		where TObject : VariantObject
	{
		if (_object is TObject variantObject)
		{
			return variantObject;
		}

		return (TObject)((UserObject)_object).CompositeBase;
	}
	
	private static unsafe double PackDateTime(DateTime value)
	{
		return *(double*)&value;
	}
	
	private static unsafe double PackTimeSpan(TimeSpan value)
	{
		return *(double*)&value;
	}
	
	private static unsafe DateTime UnpackDateTime(double value)
	{
		return *(DateTime*)&value;
	}
	
	private static unsafe TimeSpan UnpackTimeSpan(double value)
	{
		return *(TimeSpan*)&value;
	}
}