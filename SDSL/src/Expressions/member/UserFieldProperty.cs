namespace SDSL.Expressions;

public class UserFieldProperty : MemberProperty
{
	private readonly int _location;

	public UserFieldProperty(int location)
	{
		_location = location;
	}

	public override SealValue Get(SealObject self)
	{
		var userObject = (SealUserObject)self;
		
		return userObject.Fields[_location].Value;
	}

	public override void Set(SealObject self, SealValue value)
	{
		var userObject = (SealUserObject)self;
		
		ref Field field = ref userObject.Fields[_location];

		if (field.IsConst)
		{
			throw new InvalidOperationException("Field is const.");
		}
		
		if (!value.Class.IsAssignableTo(field.Class))
		{
			throw new InvalidOperationException($"Value {value.Class} is not assignable of class {field.Class}.");
		}
        
		field.Value = value;
	}
}