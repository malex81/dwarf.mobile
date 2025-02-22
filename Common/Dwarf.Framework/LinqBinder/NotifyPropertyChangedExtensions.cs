using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace Dwarf.Framework.LinqBinder
{
	public static class NotifyPropertyChangedExtensions
	{
		public static BindingContent<TSrc> CreateBindingContent<TSrc>(this TSrc source) where TSrc : INotifyPropertyChanged
			=> new BindingContent<TSrc>(source);

		public static BindingNode<TSrc, TVal> BindProperty<TSrc, TVal>(this TSrc source, Expression<Func<TSrc, TVal>> path) where TSrc : INotifyPropertyChanged
			=> new BindingNode<TSrc, TVal>(source, path);
	}
}
