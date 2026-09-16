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
	
	public UserInstanceProperty[] InstanceFields { get; set; }
}