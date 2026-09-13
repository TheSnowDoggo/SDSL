using System.Text;

namespace SDSL;

public abstract class Function : VariantObject
{
	public const int AnyArgs = int.MaxValue;
	
	public string Name { get; protected init; }
	public bool IsStatic { get; protected init; }
	public FunctionSignature Signature { get; protected init; }

	public string FullName => $"{Class.Name}.{Name}";

	public Variant MemberInvoke(Variant self, params Variant[] args)
	{
		if (IsStatic)
		{
			throw new InvalidOperationException("Cannot call static function in a static context.");
		}

		if (!self.IsAssignableTo(Class))
		{
			throw new ArgumentException($"Self parameter {self.ToSafeString()} is not assignable to class {Class}.");
		}
		
		ValidateArguments(args);
		
		return Invoke(self, args);
	}
	
	public Variant StaticInvoke(params Variant[] args)
	{
		if (!IsStatic)
		{
			throw new InvalidOperationException("Cannot call member function in a non-static context.");
		}

		ValidateArguments(args);
		
		return Invoke(Variant.Nil, args);
	}
	
	protected abstract Variant Invoke(Variant self, Variant[] args);

	public override string ToString()
	{
		var sb = new StringBuilder();

		if (IsStatic)
		{
			sb.Append("static ");
		}

		sb.Append("func ");
		
		sb.Append(Class.Name);
		sb.Append('.');
		sb.Append(Name);

		sb.Append('(');

		sb.AppendJoin<FunctionArgument>(", ", Signature.Arguments);

		sb.Append(") -> ");

		sb.Append(Signature.ReturnType == null ? "Any" : Signature.ReturnType.Name);

		return sb.ToString();
	}

	public override string ToSafeString()
	{
		return $"Function<{FullName}>";
	}

	private void ValidateArguments(Variant[] args)
	{
		if (args.Length < Signature.MinArgs)
		{
			throw new ArgumentException(
				$"Function {FullName} : Expected minimum of {Signature.MinArgs} arguments, got {args.Length}.");
		}

		if (args.Length > Signature.MaxArgs)
		{
			throw new ArgumentException(
				$"Function {FullName} : Expected maximum of {Signature.MaxArgs} arguments, got {args.Length}.");
		}

		int length = Math.Min(args.Length, Signature.Arguments.Length);

		for (int i = 0; i < length; i++)
		{
			FunctionArgument fArgument = Signature.Arguments[i];

			if (!args[i].IsAssignableTo(fArgument.VariantClass))
			{
				// Function Test.foo : Argument 0 [ x: Number ] expected value of type Number, got String.
				throw new ArgumentException(
					$"Function {FullName} Argument {i + 1} [ {fArgument} ] expected value of type {fArgument.VariantClass}, got {args[i].Class}.");
			}
		}
	}
}