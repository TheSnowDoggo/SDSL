using System.Text;

namespace SDSL;

[ClassExport]
public static class StringClass
{
	public static NativeVariantClass Class { get; } = new NativeVariantClass("String", VariantType.String);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass(variantAssembly, typeof(StringClass), Class);
	}

	[FunctionExport]
	public static Variant to_string(Variant self)
	{
		return self;
	}
	
	public static string FormatStaticInvokeFail(Function function, Variant[] args)
	{
		var sb = new StringBuilder();

		sb.Append(function.FullName);

		sb.Append('(');
        
		for (int i = 0; i < args.Length; i++)
		{
			if (i != 0)
			{
				sb.Append(", ");
			}

			sb.Append(args[i].ToSafeString());
		}

		sb.Append(')');

		return sb.ToString();
	}
    
	public static string FormatMemberInvokeFail(Function function, Variant self, Variant[] args)
	{
		var sb = new StringBuilder();

		sb.Append(self.ToSafeString());

		sb.Append("->");

		sb.Append(function.FullName);

		sb.Append('(');
        
		for (int i = 0; i < args.Length; i++)
		{
			if (i != 0)
			{
				sb.Append(", ");
			}

			sb.Append(args[i].ToSafeString());
		}

		sb.Append(')');

		return sb.ToString();
	}
}