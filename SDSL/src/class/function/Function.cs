using System.Text;

namespace SDSL;

public abstract class Function : VariantObject
{
	public const int AnyArgs = -1;
	
	public static NativeClass Class { get; } = NativeClass.InheritObject("Function");

	public override VariantClass ObjectClass => Class;
	
	public abstract VariantClass LocalClass { get; }

	public string Name { get; protected init; }
	
	public bool IsStatic { get; protected init; }
	
	public FunctionSignature Signature { get; protected init; }

	public string FullName => $"{LocalClass.Name}.{Name}";
	
	[PropertyExport("String")]
	public Variant name => Name;
	
	[PropertyExport("Bool")]
	public Variant is_static => IsStatic;
	
	public Variant MemberInvoke(Variant self, params Variant[] args)
	{
		if (IsStatic)
		{
			throw new RuntimeException("Cannot call static function as a member function.");
		}

		if (!self.IsAssignableTo(LocalClass))
		{
			throw new RuntimeException($"Self parameter {self} is not assignable to class {LocalClass}.");
		}
		
		ValidateArguments(args);
		
		return Invoke(self, args);
	}
	
	public Variant StaticInvoke(params Variant[] args)
	{
		if (!IsStatic)
		{
			throw new RuntimeException("Cannot call member function in a non-static context.");
		}

		ValidateArguments(args);
		
		return Invoke(Variant.Nil, args);
	}
	
	protected abstract Variant Invoke(Variant self, Variant[] args);

	public override string ToString()
	{
		return $"Function<{FullName}>";
	}

	public override string ToStringVolatile()
	{
		var sb = new StringBuilder();

		if (IsStatic)
		{
			sb.Append("static ");
		}

		sb.Append("func ");
		
		sb.Append(LocalClass.Name);
		sb.Append('.');
		sb.Append(Name);

		sb.Append('(');

		sb.AppendJoin<FunctionArgument>(", ", Signature.Arguments);

		sb.Append(") -> ");

		sb.Append(Signature.ReturnType == null ? "Any" : Signature.ReturnType.Name);

		return sb.ToString();
	}

	public void ValidateArguments(Variant[] args)
	{
		if (args.Length < Signature.MinArgs)
		{
			throw new RuntimeException(
				$"Function {FullName} : Expected minimum of {Signature.MinArgs} arguments, got {args.Length}.");
		}

		if (Signature.MaxArgs >= 0 && args.Length > Signature.MaxArgs)
		{
			throw new RuntimeException(
				$"Function {FullName} : Expected maximum of {Signature.MaxArgs} arguments, got {args.Length}.");
		}

		int length = Math.Min(args.Length, Signature.Arguments.Length);

		for (int i = 0; i < length; i++)
		{
			FunctionArgument fArgument = Signature.Arguments[i];

			if (!args[i].IsAssignableTo(fArgument.VariantClass))
			{
				// Function Test.foo : Argument 0 [ x: Number ] expected value of type Number, got String.
				throw new RuntimeException(
					$"Function {FullName} Argument {i + 1} [ {fArgument} ] expected value of type {fArgument.VariantClass}, got {args[i].Class}.");
			}
		}
	}
}