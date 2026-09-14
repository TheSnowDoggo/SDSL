using SDSL.Expressions;

namespace SDSL;

public class UserInstanceProperty : UserProperty,
	ISourceLocated
{
	public UserInstanceProperty(
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

	public override bool IsStatic => false;
	
	public int FieldLocation { get; set; }
	
	public Expression Expression { get; set; }
	
	public override Variant Get(Variant self)
	{
		return self.AsVariantObject<UserVariantObject>().Fields[FieldLocation];
	}

	public override void Set(Variant self, Variant value)
	{
		if (!value.IsAssignableTo(ValueClass))
		{
			throw new RuntimeException(Location,
				$"Value of type {value} is not assignable to field {FullName} of type {ValueClass}");
		}
		
		self.AsVariantObject<UserVariantObject>().Fields[FieldLocation] = value;
	}
}