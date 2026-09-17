using System.Collections;
using System.Text;

namespace SDSL.Native;

[ClassExport]
public class PackedNumberArray : VariantObject, IEnumerable<Variant>
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

	public static void Generate(VariantAssembly assembly)
	{
		NativeClassFactory.GenerateClass<PackedNumberArray>(assembly, Class);
	}

	[FunctionInfo("size")]
	[ConstructorExport("Number", ReturnType = ClassName)]
	public static Variant _new(Variant[] args)
	{
		return new PackedNumberArray(args[0].AsInt32());
	}

	[PropertyExport("Number")]
	public Variant size => _array.Length;
	
	[FunctionExport("Number", ReturnType = "Number")]
	public Variant _get(Variant[] args)
	{
		return _array[args[0].AsInt32()];
	}
	
	[FunctionExport("Number", "Number")]
	public Variant _set(Variant[] args)
	{
		return _array[args[0].AsInt32()] = args[1].AsDouble();
	}

	[FunctionExport(ReturnType = "Array")]
	public Variant to_array()
	{
		return NativeArray.FromArray(_array, static v => v);
	}

	[FunctionExport(ReturnType = "String")]
	public Variant to_string()
	{
		if (_array.Length == 0)
		{
			return "[  ]";
		}

		var sb = new StringBuilder();

		sb.Append("[ ");

		sb.Append(_array[0]);
		
		for (int i = 1; i < _array.Length; i++)
		{
			sb.Append(", ");
			sb.Append(_array[i]);
		}

		sb.Append(" ]");

		return sb.ToString();
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