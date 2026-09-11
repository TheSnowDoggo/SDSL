using System.Collections;
using System.Text;
using SDSL.Prototypes;
using SDSL.Factory;

namespace SDSL.Classes;

[ClassExport]
public class PackedNumberArray : SealObject, IEnumerable<SealValue>
{
	private readonly double[] _array;

	public PackedNumberArray(double[] array)
	{
		_array = array;
	}
	
	public PackedNumberArray(int length)
	{
		_array = new double[length];
	}

	public static readonly SealClass Class = SealClass.CreateGlobal("PackedNumberArray");

	public override SealClass TypeClass => Class;

	public static void Generate(PrototypeAssembly pAssembly)
	{
		SealClassFactory.Generate<PackedNumberArray>(pAssembly, Class);
	}

	[SealConstructor]
	[FunctionExport("Number")]
	public static SealValue _new(SealValue[] args)
	{
		return new PackedNumberArray(args[0].AsInt32());
	}

	[FunctionExport]
	public SealValue size()
	{
		return _array.Length;
	}
	
	[FunctionExport("Number")]
	public SealValue _get(SealValue[] args)
	{
		return _array[args[0].AsInt32()];
	}
	
	[FunctionExport("Number", "Number")]
	public SealValue _set(SealValue[] args)
	{
		return _array[args[0].AsInt32()] = args[1].AsNumber();
	}

	[FunctionExport]
	public SealValue to_array()
	{
		return SealArray.FromArray(_array, static v => v);
	}

	[FunctionExport]
	public SealValue to_string()
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
	
	public IEnumerator<SealValue> GetEnumerator()
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