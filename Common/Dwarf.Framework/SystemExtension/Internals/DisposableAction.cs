namespace Dwarf.Framework.SystemExtension.Internals;

internal partial class DisposableAction : IDisposable
{
	private readonly Action? originAction = null;
	private Action<IDisposable>? dispAction;
	private readonly bool obligatory;

	public DisposableAction(Action<IDisposable> onDispose, bool obligatory)
	{
		dispAction = onDispose;
		this.obligatory = obligatory;
	}
	public DisposableAction(Action onDispose, bool obligatory)
	{
		originAction = onDispose;
		dispAction = s => onDispose();
		this.obligatory = obligatory;
	}

	#region IDisposable Members

	public void Dispose()
	{
		if (dispAction != null)
		{
			dispAction(this);
			dispAction = null;
		}
		GC.SuppressFinalize(this);
	}

	~DisposableAction()
	{
		if (obligatory)
		{
			var ex = new InvalidOperationException("Dispose was not called in class DisposableAction");
			var action = originAction as Delegate ?? dispAction;
			ex.Data["TargetType"] = action?.Target?.GetType();
			ex.Data["Method"] = action?.Method;
			throw ex;
		}
	}

	#endregion
}
