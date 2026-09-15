namespace SDSL;

public class UserVariantClass : VariantClass
{
	public UserVariantClass(string name)
	{
		Name = name;
		VariantType = VariantType.Object;
	}
	
	public string PrototypeBaseClass { get; set; }
	
	public NativeVariantClass CompositeClass { get; set; }
	
	public UserInstanceProperty[] InstanceFields { get; set; }
}