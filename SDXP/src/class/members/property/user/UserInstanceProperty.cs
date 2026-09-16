using SDSL.Expressions;

namespace SDSL;

public class UserInstanceProperty : UserProperty,
	ISourceLocated
{
	public UserInstanceProperty(
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

	public override bool IsStatic => false;
	
	public int FieldLocation { get; set; }
	
	public Expression Expression { get; set; }
	
	protected override Variant Get(Variant self)
	{
		return self.AsVariantObject<UserObject>().Fields[FieldLocation];
	}

	protected override void Set(Variant self, Variant value)
	{
		self.AsVariantObject<UserObject>().Fields[FieldLocation] = value;
	}
}