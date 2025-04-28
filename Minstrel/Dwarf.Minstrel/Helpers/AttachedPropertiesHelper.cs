namespace Dwarf.Minstrel.Helpers;

public static class AttachedPropertiesHelper
{
	public static T WithAttached<T>(this T obj, BindableProperty property, object val) where T : BindableObject
	{
		obj.SetValue(property, val);
		return obj;
	}
}