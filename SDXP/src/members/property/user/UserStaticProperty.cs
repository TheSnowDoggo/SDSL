namespace SDSL;

public class UserStaticProperty : UserProperty,
	ISourceLocated
{
	public UserStaticProperty(
		string name,
		VariantClass declaredClass,
		string prototypeValueClass,
		SourceLocation location,
		ArraySegment<Token> tokens)
	{
		Name = name;
		DeclaredClass = declaredClass;
		PrototypeValueClass = prototypeValueClass;
		Location = location;
		Tokens = tokens;
	}

	public override bool IsStatic => true;
	
	public Variant Value { get; set; }
	
	protected override Variant Get(Variant self)
	{
		return Value;
	}

	protected override void Set(Variant self, Variant value)
	{
		Value = value;
	}
}