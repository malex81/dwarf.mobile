using System.ComponentModel;

namespace Dwarf.Framework.LinqBinder;

[Flags]
public enum BindingWay { Manual = 0, SourceToTarget = 1, TargetToSource = 2, TwoWay = 3 }

public partial class BindingSplice : IDisposable
{
	internal readonly IBindingNode source, target;
	BindingWay direction;
	bool initialized;

	bool silentSource, silentTarget;

	public BindingSplice(IBindingNode source, IBindingNode target)
	{
		this.source = source;
		this.target = target;
		direction = BindingWay.TwoWay;

		if (source is IContentProvider contentProvider && contentProvider.Content != null)
		{
			direction = contentProvider.Content.DefaultDirection;
			contentProvider.Content.AddSplicer(this);
		}
	}

	public bool IsDisposed { get; private set; }

	public BindingSplice ReadSource()
	{
		silentTarget = true;
		try { CopyValue(source, target, true); } finally { silentTarget = false; }
		if (!initialized) InitTriggers();
		return this;
	}

	public BindingSplice ReadTarget()
	{
		silentSource = true;
		try { CopyValue(target, source, false); } finally { silentSource = false; }
		if (!initialized) InitTriggers();
		return this;
	}

	public void SuspendTarget() { silentTarget = true; }

	public BindingSplice Direction(BindingWay direction)
	{
		this.direction = direction;
		if (initialized) InitTriggers();
		return this;
	}

	void RemoveTriggers()
	{
		source.RemoveValueChangeTrigger(OnSourceChanged);
		target.RemoveValueChangeTrigger(OnTargetChanged);
	}

	void InitTriggers()
	{
		if (IsDisposed)
			throw new InvalidOperationException("Класс уже уничтожен");
		if (initialized)
			RemoveTriggers();
		if ((direction & BindingWay.SourceToTarget) == BindingWay.SourceToTarget)
			source.AddValueChangeTrigger(OnSourceChanged);
		if ((direction & BindingWay.TargetToSource) == BindingWay.TargetToSource)
			target.AddValueChangeTrigger(OnTargetChanged);
		initialized = true;
	}

	void OnSourceChanged()
	{
		if ((direction & BindingWay.SourceToTarget) != BindingWay.SourceToTarget)
			throw new InvalidOperationException("Internal error at source trigger handler");
		if (!silentSource)
			ReadSource();
	}

	void OnTargetChanged()
	{
		if ((direction & BindingWay.TargetToSource) != BindingWay.TargetToSource)
			throw new InvalidOperationException("Internal error at target trigger handler");
		if (!silentTarget)
			ReadTarget();
	}

	protected virtual void CopyValue(IBindingNode from, IBindingNode to, bool sourceToTarget)
	{
		var fromValue = from.Value;
		if (fromValue == null ? to.ValueType.IsClass : to.ValueType.IsAssignableFrom(fromValue.GetType()))
		{
			to.Value = fromValue;
			return;
		}
		if (fromValue != null)
		{
			var convFrom = TypeDescriptor.GetConverter(fromValue.GetType());
			if (convFrom.CanConvertTo(to.ValueType))
			{
				to.Value = convFrom.ConvertTo(fromValue, to.ValueType);
				return;
			}
			var convTo = TypeDescriptor.GetConverter(to.ValueType);
			if (convTo.CanConvertFrom(fromValue.GetType()))
			{
				to.Value = convTo.ConvertFrom(fromValue);
				return;
			}
		}
		throw new InvalidCastException(string.Format("Incompatible data types {0} and {1}", from.ValueType, to.ValueType));
	}

	#region IDisposable Members

	public void Dispose()
	{
		RemoveTriggers();
		initialized = false;
		IsDisposed = true;
		GC.SuppressFinalize(this);
	}

	#endregion
}

public partial class BindingSplice<TSrc, TSrcV, TTarg, TTargV> : BindingSplice
{
	Func<TSrcV, TTargV> sourceToTargetConverter;
	Func<TTargV, TSrcV> targetToSourceConverter;

	public BindingSplice(IBindingNode<TSrc, TSrcV> source, IBindingNode<TTarg, TTargV> target) : base(source, target) { }

	public new BindingSplice<TSrc, TSrcV, TTarg, TTargV> ReadSource()
	{
		return (BindingSplice<TSrc, TSrcV, TTarg, TTargV>)base.ReadSource();
	}
	public new BindingSplice<TSrc, TSrcV, TTarg, TTargV> ReadTarget()
	{
		return (BindingSplice<TSrc, TSrcV, TTarg, TTargV>)base.ReadTarget();
	}
	public new BindingSplice<TSrc, TSrcV, TTarg, TTargV> Direction(BindingWay direction)
	{
		return (BindingSplice<TSrc, TSrcV, TTarg, TTargV>)base.Direction(direction);
	}

	public BindingSplice<TSrc, TSrcV, TTarg, TTargV> WithConverting(Func<TSrcV, TTargV> sourceToTargetConverter, Func<TTargV, TSrcV> targetToSourceConverter)
	{
		this.sourceToTargetConverter = sourceToTargetConverter;
		this.targetToSourceConverter = targetToSourceConverter;
		return this;
	}

	protected override void CopyValue(IBindingNode from, IBindingNode to, bool sourceToTarget)
	{
		var converter = sourceToTarget ? (Delegate)sourceToTargetConverter : (Delegate)targetToSourceConverter;
		if (converter == null)
			base.CopyValue(from, to, sourceToTarget);
		else
			to.Value = converter.DynamicInvoke(from.Value);
	}
}
