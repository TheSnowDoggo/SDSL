using System.Text;
using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public class SealStringBuilder : SealObject
{
	private readonly StringBuilder _sb;

	public SealStringBuilder()
	{
		_sb = new StringBuilder();
	}
	
	[ClassExport]
	public static readonly SealClass Class = new SealClass(
		GlobalConfig.GlobalNamespace,
		"StringBuilder",
		ValueType.Object,
		false
	);

	public override SealClass TypeClass => Class;

	[FunctionExport("size() -> Number")]
	public static SealValue GetSize(SealValue self, SealValue[] args)
	{
		return GetStringBuilder(self).Length;
	}
	
	[FunctionExport("append(value: Any) -> StringBuilder")]
	public static SealValue Append(SealValue self, SealValue[] args)
	{
		GetStringBuilder(self).Append(args[0].ToString());
		return self;
	}
	
	[FunctionExport("append_line(value: Any) -> StringBuilder")]
	public static SealValue AppendLine(SealValue self, SealValue[] args)
	{
		GetStringBuilder(self).AppendLine(args[0].ToString());
		return self;
	}

	[FunctionExport("append_join(seperator: String, args..) -> StringBuilder")]
	public static SealValue AppendJoin(SealValue self, SealValue[] args)
	{
		GetStringBuilder(self).Append(SealString.Join(args));
		return self;
	}
	
	[FunctionExport("append_format(format: String, args..) -> StringBuilder")]
	public static SealValue AppendFormat(SealValue self, SealValue[] args)
	{
		GetStringBuilder(self).Append(SealString.Format(args));
		return self;
	}

	[FunctionExport("insert(index: Number, value: Any) -> Bool")]
	public static SealValue Insert(SealValue self, SealValue[] args)
	{
		var sb = GetStringBuilder(self);

		int index = args[0].AsInt32();

		if (index < 0 || index >= sb.Length)
		{
			return false;
		}

		sb.Insert(index, args[1].ToString());
		
		return true;
	}
	
	[FunctionExport("remove(start_index: Number, count: Number) -> StringBuilder")]
	public static SealValue Remove(SealValue self, SealValue[] args)
	{
		var sb = GetStringBuilder(self);

		return args.Length switch
		{
			1 => Remove(sb, args[0].AsInt32()),
			2 => Remove(sb, args[0].AsInt32(), args[1].AsInt32()),
			_ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}.")
		};
	}
	
	private static bool Remove(StringBuilder sb, int startIndex)
	{
		if (startIndex < 0 || startIndex >= sb.Length)
		{
			return false;
		}

		sb.Remove(startIndex, sb.Length - startIndex);

		return true;
	}
	
	private static bool Remove(StringBuilder sb, int startIndex, int count)
	{
		if (startIndex < 0 || count < 0
		    || startIndex + count > sb.Length)
		{
			return false;
		}
		
		sb.Remove(startIndex, count);

		return true;
	}

	[FunctionExport("replace(old_string: String, new_string: String) -> StringBuilder")]
	public static SealValue Replace(SealValue self, SealValue[] args)
	{
		GetStringBuilder(self).Replace(args[0].AsString(), args[1].AsString());
		return self;
	}
	
	[FunctionExport("clear()")]
	public static void Clear(SealValue self, SealValue[] args)
	{
		GetStringBuilder(self).Clear();
	}
	
	[FunctionExport("to_string(start_index: Number = ?, count: Number = ?) -> String")]
	public static SealValue ToString(SealValue self, SealValue[] args)
	{
		var sb = GetStringBuilder(self);

		return args.Length switch
		{
			0 => sb.ToString(),
			1 => ToString(sb, args[0].AsInt32()),
			2 => ToString(sb, args[0].AsInt32(), args[1].AsInt32()),
			_ => throw new ArgumentException($"Expected 0, 1 or 2 arguments, got {args.Length}."),
		};
	}

	private static string ToString(StringBuilder sb, int startIndex)
	{
		if (startIndex < 0 || startIndex >= sb.Length)
		{
			return string.Empty;
		}
		
		return sb.ToString(startIndex, sb.Length - startIndex);
	}
	
	private static string ToString(StringBuilder sb, int startIndex, int count)
	{
		if (startIndex < 0 || count < 0
		    || startIndex + count > sb.Length)
		{
			return string.Empty;
		}
		
		return sb.ToString(startIndex, sb.Length - startIndex);
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