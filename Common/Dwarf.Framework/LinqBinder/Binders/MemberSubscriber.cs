using System.ComponentModel;
using System.Reflection;

namespace Dwarf.Framework.LinqBinder.Binders;

class MemberSubscriber
{
	public static MemberSubscriber Create(MemberInfo member, IBinder binder)
	{
		return new MemberSubscriber(member, binder);
	}

	readonly MemberInfo member;
	object? source;
	readonly IBinder binder;
	Action? unsubscribe;

	MemberSubscriber(MemberInfo member, IBinder binder)
	{
		this.member = member;
		this.binder = binder;
	}

	public void SubscribeChangeEvent(object? source)
	{
		if (this.source == source) return;
		UnsubscribeChangeEvent();
		this.source = source;
		if (source != null)
			InnerSubscribe();
	}

	public void UnsubscribeChangeEvent()
	{
		unsubscribe?.Invoke();
		unsubscribe = null;
		source = null;
	}

	void InnerSubscribe()
	{
		if (source is INotifyPropertyChanged npc)
		{
			npc.PropertyChanged += Npc_PropertyChanged;
			unsubscribe = () => npc.PropertyChanged -= Npc_PropertyChanged;
			return;
		}
		var pce = source?.GetType().GetEvent(member.Name + "Changed");
		if (pce != null)
		{
			pce.AddEventHandler(source, (EventHandler)Pce_PropertyChanged);
			unsubscribe = () => pce.RemoveEventHandler(source, new EventHandler(Pce_PropertyChanged));
		}
	}

	void Npc_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == member.Name)
			binder.CallChangeTrigger();
	}

	void Pce_PropertyChanged(object? sender, EventArgs e)
	{
		binder.CallChangeTrigger();
	}
}
