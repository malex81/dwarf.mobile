using System.Reflection;

namespace Dwarf.Framework.LinqBinder.Binders;

abstract class MemberBinder<TMemb> : Binder where TMemb : MemberInfo
{
	protected readonly IBinder parent;
	protected readonly TMemb mInfo;

	public MemberBinder(IBinder parent, TMemb member)
	{
		this.parent = parent;
		mInfo = member;
	}

	public override void AttachSource(object source)
	{
		parent.AttachSource(source);
	}

	MemberSubscriber? subscriber;

	protected void SubscribeChangeEvent()
	{
		subscriber ??= MemberSubscriber.Create(mInfo, this);
		subscriber.SubscribeChangeEvent(parent.Value);
	}

	protected void UnsubscribeChangeEvent()
	{
		subscriber?.UnsubscribeChangeEvent();
	}

	protected void OnSourceChanged()
	{
		SubscribeChangeEvent();
		CallChangeTrigger();
	}

	public override void SetValueChangeTrigger(Action? onValueChanged)
	{
		base.SetValueChangeTrigger(onValueChanged);
		if (onValueChanged == null) UnsubscribeChangeEvent();
		else SubscribeChangeEvent();
		parent.SetValueChangeTrigger(onValueChanged == null ? null : OnSourceChanged);
	}
}
