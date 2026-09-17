using System.Collections;
using System.Text;

namespace SDSL.Native;

[ClassExport]
public class PackedStringArray : VariantObject, IEnumerable<Variant>
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

	public override VariantClass ParentClass => Class;

	public static void Generate(VariantAssembly assembly)
	{
		NativeClassFactory.GenerateClass<PackedStringArray>(assembly, Class);
	}

	[FunctionInfo("size")]
	[ConstructorExport("Number", ReturnType = ClassName)]
	public static Variant _new(Variant[] args)
	{
		return new PackedStringArray(args[0].AsInt32());
	}

	[PropertyExport("Number")]
	public Variant size => _array.Length;
	
	[FunctionExport("Number", ReturnType = "String")]
	public Variant _get(Variant[] args)
	{
		return _array[args[0].AsInt32()];
	}
	
	[FunctionExport("Number", "String")]
	public Variant _set(Variant[] args)
	{
		return _array[args[0].AsInt32()] = args[1].AsString();
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