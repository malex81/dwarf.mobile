using Dwarf.Toolkit.Base.SystemExtension;

namespace Dwarf.Framework.LinqBinder;

public class BindingSection
{
	readonly ContentData content;
	readonly Func<BindingSplice, bool> condition;

	internal BindingSection(ContentData content, Func<BindingSplice, bool> condition)
	{
		this.content = content;
		this.condition = condition;
	}

	IEnumerable<BindingSplice> Splicers
	{
		get { return content.Splicers.Where(condition); }
	}

	public void ReadSource()
	{
		foreach (var s in Splicers)
			s.ReadSource();
	}

	public void ReadTarget()
	{
		foreach (var s in Splicers)
			s.ReadTarget();
	}

	public IDisposable Suspend()
	{
		foreach (var s in Splicers)
			s.SuspendTarget();
		return DisposableHelper.FromAction(ReadSource);
	}

}
