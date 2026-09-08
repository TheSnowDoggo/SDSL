using SDSL.Prototypes;
using System.Collections;

namespace SDSL.Classes;

[CustomClassGenerator]
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
		SealClassFactory<PackedStringArray>.Generate(pAssembly, Class);
	}

	[SealConstructor]
	[SealFunctionExport("Number")]
	public static SealValue _new(SealValue[] args)
	{
		return new PackedStringArray(args[0].AsInt32());
	}

	[SealFunctionExport]
	public SealValue size()
	{
		return _array.Length;
	}
	
	[SealFunctionExport("Number")]
	public SealValue _get(SealValue[] args)
	{
		return _array[args[0].AsInt32()];
	}
	
	[SealFunctionExport("Number", "String")]
	public SealValue _set(SealValue[] args)
	{
		return _array[args[0].AsInt32()] = args[1].AsString();
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