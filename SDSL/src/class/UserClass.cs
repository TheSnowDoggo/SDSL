namespace SDSL;

public class UserClass : VariantClass
{
	public UserClass(string name)
	{
		Name = name;
		VariantType = VariantType.Object;
	}
	
	public string PrototypeBaseClass { get; set; }
	
	public override Function Constructor => UserConstructor;
	public UserConstructor UserConstructor { get; set; }
	
	public override IReadOnlyList<Function> LocalFunctions => LocalUserFunctions;
	public List<UserFunction> LocalUserFunctions { get; set; } = [];
	
	public override IReadOnlyList<Property> LocalProperties => LocalUserProperties;
	public List<UserProperty> LocalUserProperties { get; set; } = [];

	public override IReadOnlyList<Constant> LocalConstants => LocalUserConstants;
	public List<Constant> LocalUserConstants { get; set; } = [];

	public UserInstanceProperty[] InstanceFields { get; set; }
	
	public void CreateConstant(string name, Variant value)
	{
		LocalUserConstants.Add(new Constant(name, this, value));
	}
}