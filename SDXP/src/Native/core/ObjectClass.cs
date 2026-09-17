namespace SDSL.Native;

[ClassExport]
public static class ObjectClass
{
	public static NativeClass Class { get; } = new NativeClass("Object", VariantType.Nil, null);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		NativeClassFactory.GenerateClass(variantAssembly, typeof(ObjectClass), Class);
	}

	[FunctionExport]
	public static Variant to_string(Variant self)
	{
		return self.ToString();
	}
	
	[FunctionExport]
	public static Variant to_bool(Variant self)
	{
		return self.ToBool();
	}
	
	[FunctionInfo("other")]
	[FunctionExport("Any")]
	public static Variant equals(Variant self, Variant[] args)
	{
		return self.Equals(args[0]);
	}
	
	[FunctionExport]
	public static Variant get_hash_code(Variant self)
	{
		return self.GetHashCode();
	}
	
	[GetterFunctionExport]
	public static Variant type(Variant self)
	{
		return self.Class;
	}
}