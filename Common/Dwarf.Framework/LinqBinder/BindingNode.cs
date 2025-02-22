using Dwarf.Framework.LinqBinder.Binders;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Dwarf.Framework.LinqBinder;

public class BindingNode : IBindingNode, IContentProvider
{
	readonly IBinder binder;
	object source;
	readonly List<Action> valueChangedTriggers = [];

	public BindingNode(object source, Expression expr, Type valType)
	{
		binder = Binder.Create(expr);
		Source = source;
		ValueType = valType;
	}

	internal ContentData? content;
	ContentData? IContentProvider.Content => content;

	#region IBindingNode Members

	public object Source
	{
		get { return source; }
		[MemberNotNull(nameof(source))]
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			source = value;
			binder.AttachSource(source);
		}
	}

	public object? Value
	{
		get { return binder.Value; }
		set { binder.Value = value; }
	}

	public Type ValueType { get; private set; }

	public void AddValueChangeTrigger(Action onValueChanged)
	{
		ArgumentNullException.ThrowIfNull(onValueChanged);
		if (valueChangedTriggers.Count == 0)
			binder.SetValueChangeTrigger(OnValueChanged);
		valueChangedTriggers.Add(onValueChanged);
	}

	public void RemoveValueChangeTrigger(Action onValueChanged)
	{
		ArgumentNullException.ThrowIfNull(onValueChanged);
		valueChangedTriggers.Remove(onValueChanged);
		if (valueChangedTriggers.Count == 0)
			binder.SetValueChangeTrigger(null);
	}

	protected void OnValueChanged()
	{
		valueChangedTriggers.ForEach(t => t());
	}

	#endregion

}

public class BindingNode<TSrc, TVal> : BindingNode, IBindingNode<TSrc, TVal> where TSrc : notnull
{
	public BindingNode(TSrc source, Expression<Func<TSrc, TVal>> path) : base(source, path.Body, typeof(TVal)) { }

	#region IBindingNode<TObj,TVal> Members

	public new TSrc Source
	{
		get { return (TSrc)base.Source; }
		set { base.Source = value; }
	}

	public new TVal? Value
	{
		get { return (TVal?)base.Value; }
		set { base.Value = value; }
	}

	#endregion

	public BindingSplice<TSrc, TVal, TTarg, TTargV> To<TTarg, TTargV>(TTarg target, Expression<Func<TTarg, TTargV>> path) where TTarg : notnull
	{
		return new BindingSplice<TSrc, TVal, TTarg, TTargV>(this, new BindingNode<TTarg, TTargV>(target, path));
	}
}
