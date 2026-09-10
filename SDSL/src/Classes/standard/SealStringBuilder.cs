using System.Text;
using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
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
		SealClassFactory.Generate<SealStringBuilder>(pAssembly, Class);
	}

	[SealConstructor]
	[FunctionExport]
	public static SealValue _new()
	{
		return new SealStringBuilder();
	}

	[FunctionExport]
	public SealValue size()
	{
		return _sb.Length;
	}
	
	[FunctionInfo("index")]
	[FunctionExport("Number")]
	public SealValue _get(SealValue[] args)
	{
		return _sb[args[0].AsInt32()].ToString();
	}
	
	[FunctionInfo("value")]
	[FunctionExport("Any")]
	public SealValue append(SealValue[] args)
	{
		_sb.Append(args[0]);
		return this;
	}
	
	[FunctionInfo("value")]
	[FunctionExport("Any")]
	public SealValue append_line(SealValue[] args)
	{
		_sb.AppendLine(args[0].ToString());
		return this;
	}

	[FunctionInfo("seperator", "args..")]
	[FunctionExport("String", MaxArgs = -1)]
	public SealValue append_join(SealValue[] args)
	{
		_sb.Append(SealString.join(args));
		return this;
	}
	
	[FunctionInfo("format", "args..")]
	[FunctionExport("String", MaxArgs = -1)]
	public SealValue append_format(SealValue[] args)
	{
		_sb.Append(SealString.format(args));
		return this;
	}

	[FunctionInfo("index", "s")]
	[FunctionExport("Number", "String")]
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
	
	[FunctionInfo("start_index", "count")]
	[FunctionExport("Number", "Number", MinArgs = 1)]
	public SealValue remove(SealValue[] args)
	{
		return args.Length switch
		{
			1 => _remove1(args[0].AsInt32()),
			2 => _remove2(args[0].AsInt32(), args[1].AsInt32()),
			_ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}."),
		};
		
		bool _remove1(int startIndex)
		{
			if (startIndex < 0 || startIndex >= _sb.Length)
			{
				return false;
			}

			_sb.Remove(startIndex, _sb.Length - startIndex);

			return true;
		}
	
		bool _remove2(int startIndex, int count)
		{
			if (startIndex < 0 || count < 0
			                   || startIndex + count > _sb.Length)
			{
				return false;
			}
		
			_sb.Remove(startIndex, count);

			return true;
		}
	}

	[FunctionInfo("old_str", "new_str")]
	[FunctionExport("String", "String")]
	public SealValue replace(SealValue[] args)
	{
		_sb.Replace(args[0].AsString(), args[1].AsString());
		return this;
	}
	
	[FunctionExport]
	public void clear()
	{
		_sb.Clear();
	}
	
	[FunctionInfo("start_index", "count")]
	[FunctionExport("Number", "Number", MinArgs = 0)]
	public SealValue to_string(SealValue[] args)
	{
		return args.Length switch
		{
			0 => _sb.ToString(),
			1 => _to_string1(args[0].AsInt32()),
			2 => _to_string2(args[0].AsInt32(), args[1].AsInt32()),
			_ => throw new ArgumentException($"Expected 0, 1 or 2 arguments, got {args.Length}."),
		};
		
		string _to_string1(int startIndex)
		{
			if (startIndex < 0 || startIndex >= _sb.Length)
			{
				return string.Empty;
			}
		
			return _sb.ToString(startIndex, _sb.Length - startIndex);
		}
		
		string _to_string2(int startIndex, int count)
		{
			if (startIndex < 0 || count < 0
			                   || startIndex + count > _sb.Length)
			{
				return string.Empty;
			}
		
			return _sb.ToString(startIndex, _sb.Length - startIndex);
		}
	}
	
	public override string ToString()
	{
		return _sb.ToString();
	}
}