namespace Dwarf.Framework.SystemExtension;

public static class ArrayExtensions
{
	public static bool IsNullOrEmpty(this Array array) => (array == null || array.Length == 0);

	public static T[] Concat<T>(this T[] source, params T[] items)
	{
		if (items.Length == 0) return source;
		var res = new T[source.Length + items.Length];
		source.CopyTo(res, 0);
		items.CopyTo(res, source.Length);
		return res;
	}

	public static T[] Concat<T>(this T[] source, params T[][] items)
	{
		if (items.Length == 0) return source;
		var itemsCount = items.Sum(b => b.Length);
		var res = new T[source.Length + itemsCount];
		source.CopyTo(res, 0);
		var ind = source.Length;
		foreach (var item in items)
		{
			item.CopyTo(res, ind);
			ind += item.Length;
		}
		return res;
	}
}
