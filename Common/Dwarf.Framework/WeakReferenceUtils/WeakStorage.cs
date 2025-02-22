namespace Dwarf.Framework.WeakReferenceUtils;

public class WeakStorage<TKey, TVal>
	where TKey : notnull
	where TVal : class
{
	private readonly Dictionary<TKey, WeakReference<TVal>> data = [];

	public TVal? this[TKey key]
	{
		get
		{
			if (!data.TryGetValue(key, out var wRef)) return null;
			if (wRef.TryGetTarget(out var res)) return res;
			data.Remove(key);
			return null;
		}
		set
		{
			if (value != null)
				data[key] = new WeakReference<TVal>(value);
			else data.Remove(key);
		}
	}

}
