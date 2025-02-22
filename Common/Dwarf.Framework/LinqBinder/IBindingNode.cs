using System.Linq.Expressions;

namespace Dwarf.Framework.LinqBinder;

public interface IBindingNode
{
	object Source { get; set; }
	object? Value { get; set; }
	Type ValueType { get; }
	void AddValueChangeTrigger(Action onValueChanged);
	void RemoveValueChangeTrigger(Action onValueChanged);
}

public interface IBindingNode<TSrc, TVal> : IBindingNode where TSrc : notnull
{
	new TSrc Source { get; set; }
	new TVal? Value { get; set; }

	BindingSplice<TSrc, TVal, TTarg, TTargV> To<TTarg, TTargV>(TTarg target, Expression<Func<TTarg, TTargV>> path) where TTarg : notnull;
}
