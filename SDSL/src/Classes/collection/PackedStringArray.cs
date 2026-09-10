using System.Collections;
using SDSL.Prototypes;
using SDSL.Factory;

namespace SDSL.Classes;

[ClassExport]
public class PackedStringArray : SealObject, IEnumerable<SealValue>
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

	public static readonly SealClass Class = SealClass.CreateGlobal("PackedStringArray");

	public override SealClass TypeClass => Class;

	public static void Generate(PrototypeAssembly pAssembly)
	{
		SealClassFactory.Generate<PackedStringArray>(pAssembly, Class);
	}

	[SealConstructor]
	[FunctionExport("Number")]
	public static SealValue _new(SealValue[] args)
	{
		return new PackedStringArray(args[0].AsInt32());
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
	
	[FunctionExport("Number", "String")]
	public SealValue _set(SealValue[] args)
	{
		return _array[args[0].AsInt32()] = args[1].AsString();
	}

	[FunctionExport]
	public SealValue to_array()
	{
		var items = new List<SealValue>(_array.Length);

		for (int i = 0; i < _array.Length; i++)
		{
			items.Add(_array[i]);
		}

		return new SealArray(items);
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