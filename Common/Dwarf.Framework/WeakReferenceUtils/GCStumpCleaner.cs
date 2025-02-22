namespace Dwarf.Framework.WeakReferenceUtils;

internal class GCStumpCleaner : IStumpCleaner
{
	Action? clean;

	public bool IsStarted { get; private set; }

	public void Start(Action clean)
	{
		ArgumentNullException.ThrowIfNull(clean);
		IsStarted = true;
		_ = new GCObserver(this);
	}

	public void Stop()
	{
		IsStarted = false;
		clean = null;
	}

	class GCObserver
	{
		readonly GCStumpCleaner parent;

		public GCObserver(GCStumpCleaner parent)
		{
			this.parent = parent;
		}

		~GCObserver()
		{
			if (parent.IsStarted && parent.clean != null)
			{
				parent.clean();
				_ = new GCObserver(parent);
			}
		}
	}
}
