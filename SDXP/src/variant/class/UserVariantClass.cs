namespace SDSL;

public class UserVariantClass : VariantClass
{
	public UserVariantClass(string name)
	{
		Name = name;
		VariantType = VariantType.Object;
	}
	
	public string PrototypeBaseClass { get; set; }
	
	public override Function Constructor => UserConstructor;
	
	public UserConstructor UserConstructor { get; set; }
	
	public UserInstanceProperty[] InstanceFields { get; set; }
}