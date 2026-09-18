namespace SDSL.Native;

public static class NilClass
{
	public static NativeClass Class { get; } = NativeClass.CreatePrimitive("Nil", VariantType.Nil);
}