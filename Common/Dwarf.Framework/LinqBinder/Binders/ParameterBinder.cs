namespace Dwarf.Framework.LinqBinder.Binders;

class ParameterBinder : Binder
{
	object? source;

	public ParameterBinder()
	{
	}

	public override void AttachSource(object? source)
	{
		this.source = source;
		CallChangeTrigger();
	}

	public override object? Value
	{
		get { return source; }
		set { AttachSource(value); }
	}

}
