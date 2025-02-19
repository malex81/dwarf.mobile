namespace Dwarf.Framework.Caches;

public class SimpleCache<T>(Func<T> create, TimeSpan lifetime) where T : class
{
	private readonly Func<T> create = create ?? throw new ArgumentNullException(nameof(create));
	private readonly TimeSpan lifetime = lifetime;
	private T? data = null;
	private DateTime lastUpdate = DateTime.Now;

	public T GetData()
	{
		lock (this)
		{
			if (data == null || DateTime.Now - lastUpdate > lifetime)
			{
				data = create();
				lastUpdate = DateTime.Now;
			}
		}
		return data;
	}
}
