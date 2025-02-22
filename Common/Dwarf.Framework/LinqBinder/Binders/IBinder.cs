namespace Dwarf.Framework.LinqBinder.Binders;

interface IBinder
{
	void AttachSource(object source);
	object? Value { get; set; }
	void SetValueChangeTrigger(Action? onValueChanged);
	void CallChangeTrigger();
}
