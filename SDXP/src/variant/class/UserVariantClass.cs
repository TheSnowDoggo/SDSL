namespace SDSL;

public class UserVariantClass : VariantClass
{
	public UserVariantClass(string name)
	{
		Name = name;
		VariantType = VariantType.Object;
	}
	
	public string PrototypeBaseClass { get; set; }

	public UserFieldInfo[] InstanceFields { get; set; } = [];
}