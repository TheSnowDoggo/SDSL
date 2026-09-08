using System.Text;
using SDSL.Prototypes;

namespace SDSL.Classes;

[CustomClassGenerator]
public class SealStringBuilder : SealObject
{
	private readonly StringBuilder _sb;

	public SealStringBuilder()
	{
		_sb = new StringBuilder();
	}
	
	public static readonly SealClass Class = SealClass.CreateGlobal("StringBuilder");

	public override SealClass TypeClass => Class;

	public static void Generate(PrototypeAssembly pAssembly)
	{
		SealClassFactory<SealStringBuilder>.Generate(pAssembly, Class);
	}

	[SealConstructor]
	[SealFunctionExport]
	public static SealValue _new()
	{
		return new SealStringBuilder();
	}

	[SealFunctionExport]
	public SealValue size()
	{
		return _sb.Length;
	}
	
	[SealFunctionExport("Any")]
	public SealValue append(SealValue[] args)
	{
		_sb.Append(args[0]);
		return this;
	}
	
	[SealFunctionExport("Any")]
	public SealValue append_line(SealValue[] args)
	{
		_sb.AppendLine(args[0].ToString());
		return this;
	}

	[SealFunctionExport("String", MaxArgs = -1)]
	public SealValue append_join(SealValue[] args)
	{
		_sb.Append(SealString.Join(args));
		return this;
	}
	
	[SealFunctionExport("String", MaxArgs = -1)]
	public SealValue append_format(SealValue[] args)
	{
		_sb.Append(SealString.Format(args));
		return this;
	}

	[SealFunctionExport("Number", "String")]
	public SealValue insert(SealValue[] args)
	{
		int index = args[0].AsInt32();

		if (index < 0 || index >= _sb.Length)
		{
			return false;
		}

		_sb.Insert(index, args[1].ToString());
		
		return true;
	}
	
	[SealFunctionExport("Number", "Number", MinArgs = 1)]
	public SealValue remove(SealValue[] args)
	{
		return args.Length switch
		{
			1 => Remove(args[0].AsInt32()),
			2 => Remove(args[0].AsInt32(), args[1].AsInt32()),
			_ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}."),
		};
	}
	
	private bool Remove(int startIndex)
	{
		if (startIndex < 0 || startIndex >= _sb.Length)
		{
			return false;
		}

		_sb.Remove(startIndex, _sb.Length - startIndex);

		return true;
	}
	
	private bool Remove(int startIndex, int count)
	{
		if (startIndex < 0 || count < 0
		    || startIndex + count > _sb.Length)
		{
			return false;
		}
		
		_sb.Remove(startIndex, count);

		return true;
	}

	[SealFunctionExport("String", "String")]
	public SealValue replace(SealValue[] args)
	{
		_sb.Replace(args[0].AsString(), args[1].AsString());
		return this;
	}
	
	[SealFunctionExport]
	public void clear()
	{
		_sb.Clear();
	}
	
	[SealFunctionExport("Number", "Number", MinArgs = 0)]
	public SealValue to_string(SealValue[] args)
	{
		return args.Length switch
		{
			0 => _sb.ToString(),
			1 => ToString(args[0].AsInt32()),
			2 => ToString(args[0].AsInt32(), args[1].AsInt32()),
			_ => throw new ArgumentException($"Expected 0, 1 or 2 arguments, got {args.Length}."),
		};
	}

	private string ToString(int startIndex)
	{
		if (startIndex < 0 || startIndex >= _sb.Length)
		{
			return string.Empty;
		}
		
		return _sb.ToString(startIndex, _sb.Length - startIndex);
	}
	
	private string ToString(int startIndex, int count)
	{
		if (startIndex < 0 || count < 0
		    || startIndex + count > _sb.Length)
		{
			return string.Empty;
		}
		
		return _sb.ToString(startIndex, _sb.Length - startIndex);
	}

	public override string ToString()
	{
		return _sb.ToString();
	}

	private static StringBuilder GetStringBuilder(SealValue self)
	{
		return self.AsSealObject<SealStringBuilder>()._sb;
	}
}