using System.Linq.Expressions;

namespace Dwarf.Framework.LinqBinder;

#region Internal part

internal class ContentData
{
	readonly List<BindingSplice> splicers;

	public ContentData()
	{
		splicers = [];
		DefaultDirection = BindingWay.TwoWay;
	}

	public void AddSplicer(BindingSplice splicer)
	{
		splicers.Add(splicer);
	}

	public void Clear()
	{
		splicers.ForEach(s => s.Dispose());
		splicers.Clear();
	}

	public IEnumerable<BindingSplice> Splicers => splicers;
	public BindingWay DefaultDirection { get; set; }
}

internal interface IContentProvider
{
	ContentData? Content { get; }
}

#endregion

public class BindingContent<T> where T : notnull
{
	readonly T source;
	readonly ContentData content;

	public BindingContent(T source)
	{
		this.source = source;
		content = new ContentData();
	}

	public BindingWay DefaultDirection
	{
		get => content.DefaultDirection;
		set => content.DefaultDirection = value;
	}

	public IBindingNode<T, TVal> Bind<TVal>(Expression<Func<T, TVal>> path) => new BindingNode<T, TVal>(source, path) { content = content };

	public void ReadSource()
	{
		foreach (var s in content.Splicers)
			s.ReadSource();
	}

	public void ReadTarget()
	{
		foreach (var s in content.Splicers)
			s.ReadTarget();
	}

	public void Clear() => content.Clear();
	public BindingSection this[object target] => new BindingSection(content, s => s.target.Source == target);
}
