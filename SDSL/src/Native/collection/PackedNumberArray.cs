using System.Collections;
using System.Text;

namespace SDSL.Native;

public class PackedNumberArray : VariantObject, IReadOnlyCollection<Variant>
{
	public const string ClassName = "PackedNumberArray";
	
	private readonly double[] _array;

	public PackedNumberArray(double[] array)
	{
		_array = array;
	}
	
	public PackedNumberArray(int length)
	{
		_array = new double[length];
	}

	public static NativeClass Class { get; } = NativeClass.InheritObject(ClassName);

	public override VariantClass ObjectClass => Class;

	public int Count => _array.Length;

	[FunctionInfo("size")]
	[ConstructorExport("Number", ReturnType = ClassName)]
	public static Variant _new(Variant[] args)
	{
		return new PackedNumberArray(args[0].AsInt32());
	}

	[PropertyExport("Number")]
	public Variant size => _array.Length;
	
	[FunctionExport("Number", ReturnType = "Number", Name = "get[]")]
	public Variant _get(Variant[] args)
	{
		return _array[args[0].AsInt32()];
	}
	
	[FunctionExport("Number", "Number", Name = "set[]")]
	public Variant _set(Variant[] args)
	{
		return _array[args[0].AsInt32()] = args[1].AsDouble();
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