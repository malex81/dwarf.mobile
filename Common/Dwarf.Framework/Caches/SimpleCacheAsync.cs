namespace Dwarf.Framework.Caches;

public class SimpleCacheAsync<T> where T : class
{
	public class Config
	{
		public bool ThrowOnTimeout { get; set; } = true;
	}

	private readonly Func<Task<T>> create;
	private readonly TimeSpan lifetime;
	private readonly Config config;
	private T? data = null;
	private DateTime lastUpdate = DateTime.Now;

	public SimpleCacheAsync(Func<Task<T>> create, TimeSpan lifetime, Config? config = null)
	{
		this.create = create ?? throw new ArgumentNullException(nameof(create));
		this.lifetime = lifetime;
		this.config = config ?? new Config();
	}

	private readonly SemaphoreSlim semaphore = new(1);
	public async Task<T?> GetData()
	{
		var wRes = await semaphore.WaitAsync(lifetime);
		if (!wRes)
		{
			if (config.ThrowOnTimeout)
				throw new CacheException($"Obtaint data of type '{typeof(T)}' timeout. Waiting time {lifetime}");
			return data;
		}
		try
		{
			if (data == null || DateTime.Now - lastUpdate > lifetime)
			{
				data = await create();
				lastUpdate = DateTime.Now;
			}
		}
		finally
		{
			semaphore.Release();
		}
		return data;
	}
}
