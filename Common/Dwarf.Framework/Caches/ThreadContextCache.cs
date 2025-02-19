namespace Dwarf.Framework.Caches;

public class ThreadContextCache<T> where T : class
{
	public class GarbageCondition
	{
		public int PreferLiveCount { get; set; } = 0;
		public TimeSpan MinLifetime { get; set; } = TimeSpan.Zero;
		public Func<T, bool>? AllowRecreate { get; set; } = null;
		public bool KeepDeadThreads { get; set; } = false;
	}

	class DataHolder
	{
		public T Data { get; }
		public DateTime LastTouch { get; private set; }
		public DataHolder(T data)
		{
			Data = data;
			Touch();
		}
		public void Touch() => LastTouch = DateTime.Now;
	}

	private readonly Func<T> create;
	private readonly GarbageCondition garbageCondition;
	private readonly Dictionary<Thread, DataHolder> dataDict = [];
	DateTime lastCleaning = DateTime.Now;

	public ThreadContextCache(Func<T> create, GarbageCondition? garbageCondition = null)
	{
		this.create = create ?? throw new ArgumentNullException(nameof(create));
		this.garbageCondition = garbageCondition ?? new GarbageCondition();
	}

	void ClearIrrelevantData()
	{
		var gc = garbageCondition;
		if (dataDict.Count <= gc.PreferLiveCount) return;
		var now = DateTime.Now;
		if (now - lastCleaning < gc.MinLifetime) return;
		var keyList = from p in dataDict
					  let v = p.Value
					  where gc.AllowRecreate != null && (now - v.LastTouch > gc.MinLifetime) && gc.AllowRecreate(v.Data)
							|| !gc.KeepDeadThreads && !p.Key.IsAlive
					  select p.Key;
		foreach (var key in keyList.ToArray())
		{
			if (dataDict.Remove(key, out var h))
				(h.Data as IDisposable)?.Dispose();
		}
		lastCleaning = now;
	}

	public T Current
	{
		get
		{
			var thread = Thread.CurrentThread;
			lock (dataDict)
			{
				if (dataDict.TryGetValue(thread, out var holder))
				{
					holder.Touch();
					return holder.Data;
				}
				ClearIrrelevantData();
				var res = create();
				dataDict[thread] = new DataHolder(res);
				return res;
			}

		}
	}
}
