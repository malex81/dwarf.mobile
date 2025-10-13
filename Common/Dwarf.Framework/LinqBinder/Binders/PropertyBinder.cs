using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dwarf.Framework.LinqBinder.Binders;

class PropertyBinder : MemberBinder<PropertyInfo>
{
	public PropertyBinder(IBinder parent, PropertyInfo pi) : base(parent, pi) { }

	public override object? Value
	{
		get
		{
			var obj = parent.Value;
			return obj == null ? mInfo.PropertyType.GetDefault() : mInfo.GetValue(obj, null);
		}
		set
		{
			var obj = parent.Value;
			if (obj != null)
				using (MakeSilent())
					mInfo.SetValue(obj, value, null);
			CallChangeTrigger();
		}
	}
}
