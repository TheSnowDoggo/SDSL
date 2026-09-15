using SDSL.Statements;

namespace SDSL;

public class UserFunction : Function, ISourceLocated
{
	public UserFunction(
		string name,
		VariantClass variantClass,
		bool isStatic,
		FunctionSignature signature,
		SourceLocation location,
		ArraySegment<Token> tokens)
	{
		Name = name;
		DeclaredClass = variantClass;
		IsStatic = isStatic;
		Signature = signature;
		Location = location;
		Tokens = tokens;
	}
	
	public override VariantClass DeclaredClass { get; }
	
	public SourceLocation Location { get; }
	
	public ArraySegment<Token> Tokens { get; set; }
	
	public Statement[] Statements { get; set; }
	public int VariableCount { get; set; }
	
	protected override Variant Invoke(Variant self, Variant[] args)
	{
		Variable[] variables = InitializeVariables(self, args);

		for (int i = 0; i < Statements.Length; i++)
		{
			Statement statement = Statements[i];

			ReturnValue returnValue = statement.Invoke(variables);

			switch (returnValue.ReturnValueType)
			{
			case ReturnValueType.None:
				break;
			case ReturnValueType.Return:
				if (!returnValue.Value.IsAssignableTo(Signature.ReturnType))
				{
					throw new RuntimeException(statement,
						$"Function {FullName} expected return type {Signature.ReturnType}, but tried to return {returnValue.Value.Class}.");
				}
                
				return returnValue.Value;
			default:
				throw new RuntimeException(statement,
					$"Function {FullName} got invalid return value type: {returnValue.ReturnValueType}.");
			}
		}
		
		if (Signature.ReturnType == null || Signature.ReturnType == NilClass.Class)
		{
			return Variant.Nil;
		}
        
		throw new RuntimeException(Location,
			$"Function {FullName} expected return type {Signature.ReturnType}, but function ended before returning.");
	}

	private Variable[] InitializeVariables(Variant self, Variant[] args)
	{
		var variables = new Variable[VariableCount];
		
		int variableIndex = 0;
		
		if (!IsStatic)
		{
			variables[variableIndex++] = new Variable(DeclaredClass, self);
		}

		FunctionArgument[] arguments = Signature.Arguments;

		for (int i = 0; i < args.Length; i++)
		{
			variables[variableIndex++] = new Variable(arguments[i].VariantClass, args[i]);
		}

		return variables;
	}
}