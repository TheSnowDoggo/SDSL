using System.Collections;
using System.Text;

namespace SDSL.Native;

public class PackedStringArray : VariantObject, IReadOnlyCollection<Variant>
{
	public const string ClassName = "PackedStringArray";
	
	private readonly string[] _array;

	public PackedStringArray(string[] array)
	{
		_array = array;
	}
	
	public PackedStringArray(int length)
	{
		_array = new string[length];
	}

	public static NativeClass Class { get; } = NativeClass.InheritObject(ClassName);

	public static PackedStringArray Empty { get; } = new PackedStringArray([]);

	public override VariantClass ObjectClass => Class;
	
	public int Count => _array.Length;

	[FunctionInfo("size")]
	[ConstructorExport("Number", ReturnType = ClassName)]
	public static Variant _new(Variant[] args)
	{
		return new PackedStringArray(args[0].AsInt32());
	}

	[PropertyExport("Number")]
	public Variant size => _array.Length;
	
	[FunctionExport("Number", ReturnType = "String", Name = "get[]")]
	public Variant _get(Variant[] args)
	{
		return _array[args[0].AsInt32()];
	}
	
	[FunctionExport("Number", "String", Name = "set[]")]
	public Variant _set(Variant[] args)
	{
		return _array[args[0].AsInt32()] = args[1].AsString();
	}

	[FunctionExport(ReturnType = "Array")]
	public Variant to_array()
	{
		return NativeArray.FromList(_array, static v => v);
	}

	public override string ToStringVolatile()
	{
		return $"[ {string.Join(", ", _array)} ]";
	}

	public IEnumerator<Variant> GetEnumerator()
	{
		for (int i = 0; i < _array.Length; i++)
		{
			yield return _array[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}