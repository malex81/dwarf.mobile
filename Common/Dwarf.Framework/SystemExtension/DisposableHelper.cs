namespace Dwarf.Framework.SystemExtension;

public static partial class DisposableHelper
{
	partial class EmptyDisposable : IDisposable { public void Dispose() { } }

	private static IDisposable? empty;
	public static IDisposable Empty
	{
		get
		{
			empty ??= new EmptyDisposable();
			return empty;
		}
	}

	public static T DisposeWith<T>(this T elem, ICollection<IDisposable> dispList) where T : IDisposable
	{
		dispList.Add(elem);
		return elem;
	}
	public static T DisposeWith<T>(this T elem, IList<IDisposable> dispList, bool first = false) where T : IDisposable
	{
		if (first) dispList.Insert(0, elem);
		else dispList.Add(elem);
		return elem;
	}

	public static IDisposable FromAction(Action action, bool obligatory = true) => new Internals.DisposableAction(action, obligatory);
	public static IDisposable FromAction(Action<IDisposable> action, bool obligatory = true) => new Internals.DisposableAction(action, obligatory);
	public static void DisposeAll<T>(this IEnumerable<T> list) => list?.OfType<IDisposable>().ForEach(d => d.Dispose());
	public static void AddAction(this ICollection<IDisposable> dispList, Action action, bool obligatory = true) => dispList.Add(FromAction(action, obligatory));
	public static IDisposable ToDisposable(this IEnumerable<IDisposable> collection) => new DisposableList(collection);
	public static T TryAdd<T>(this ICollection<IDisposable> dispList, T elem)
	{
		if (elem is IDisposable disp)
			dispList.Add(disp);
		return elem;
	}
}
