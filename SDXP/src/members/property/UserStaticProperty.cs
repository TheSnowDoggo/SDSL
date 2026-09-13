namespace SDSL;

public class UserStaticProperty : Property,
	ISourceLocated
{
	public UserStaticProperty(
		string name,
		VariantClass variantClass,
		string prototypeValueClass,
		SourceLocation location,
		ArraySegment<Token> tokens)
	{
		Name = name;
		VariantClass = variantClass;
		PrototypeValueClass = prototypeValueClass;
		Location = location;
		Tokens = tokens;
	}

	public override bool IsStatic => true;
	
	public SourceLocation Location { get; }
	public ArraySegment<Token> Tokens { get; }
	
	public Variant Value { get; set; }
	
	public override Variant Get(Variant self)
	{
		return Value;
	}

	public override void Set(Variant self, Variant value)
	{
		if (!value.IsAssignableTo(ValueClass))
		{
			throw new RuntimeException(Location,
				$"Value of type {value} is not assignable to static field {FullName} of type {ValueClass}");
		}

		Value = value;
	}
}