using Dwarf.Framework.SystemExtension;
using Dwarf.Toolkit.Base.SystemExtension;
using System.Linq.Expressions;
using System.Reflection;

namespace Dwarf.Framework.LinqBinder.Binders;

abstract class Binder : IBinder
{
	public static IBinder Create(Expression expr)
	{
		if (expr is MemberExpression mExp && mExp.Expression != null)
		{
			if (mExp.Member is PropertyInfo pInfo)
				return new PropertyBinder(Create(mExp.Expression), pInfo);
			if (mExp.Member is FieldInfo mInfo)
				return new FieldBinder(Create(mExp.Expression), mInfo);
		}
		else if (expr is ParameterExpression)
			return new ParameterBinder();

		throw new InvalidOperationException(string.Format("Неизвестный тип выражения '{0}'", expr.ToString()));
	}

	Action? onValueChanged;
	bool silent;

	public void CallChangeTrigger()
	{
		if (!silent && onValueChanged != null)
			onValueChanged();
	}

	#region IBinder Members

	public abstract void AttachSource(object source);
	public abstract object? Value { get; set; }

	public virtual void SetValueChangeTrigger(Action? onValueChanged)
	{
		this.onValueChanged = onValueChanged;
	}

	public virtual IDisposable MakeSilent()
	{
		silent = true;
		return DisposableHelper.FromAction(() => silent = false);
	}

	#endregion

}
