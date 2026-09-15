using System.Collections;
using System.Text;

namespace SDSL;

[ClassExport]
public class PackedStringArray : VariantObject, IEnumerable<Variant>
{
	private readonly string[] _array;

	public PackedStringArray(string[] array)
	{
		_array = array;
	}
	
	public PackedStringArray(int length)
	{
		_array = new string[length];
	}

	public static NativeVariantClass Class { get; } = new NativeVariantClass("PackedStringArray");

	public override VariantClass ParentClass => Class;

	public static void Generate(VariantAssembly assembly)
	{
		VariantClassFactory.GenerateClass<PackedStringArray>(assembly, Class);
	}

	[ConstructorExport]
	[FunctionExport("Number")]
	public static Variant _new(Variant[] args)
	{
		return new PackedStringArray(args[0].AsInt32());
	}

	[PropertyExport]
	public Variant size => _array.Length;
	
	[FunctionExport("Number")]
	public Variant _get(Variant[] args)
	{
		return _array[args[0].AsInt32()];
	}
	
	[FunctionExport("Number", "String")]
	public Variant _set(Variant[] args)
	{
		return _array[args[0].AsInt32()] = args[1].AsString();
	}

	[FunctionExport]
	public Variant to_array()
	{
		return SealArray.FromArray(_array, static v => v);
	}

	[FunctionExport]
	public Variant to_string()
	{
		if (_array.Length == 0)
		{
			return "[  ]";
		}

		var sb = new StringBuilder();

		sb.Append("[ \"");

		sb.Append(_array[0]);
		
		for (int i = 1; i < _array.Length; i++)
		{
			sb.Append("\", \"");
			sb.Append(_array[i]);
		}

		sb.Append("\" ]");

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