namespace Dwarf.Framework.SystemExtension;

public static class EnumerableExtensions
{
	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
	{
		foreach (var item in source)
			if (item != null)
				yield return item;
	}

	public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
	{
		foreach (T item in enumeration)
			action(item);
	}
}