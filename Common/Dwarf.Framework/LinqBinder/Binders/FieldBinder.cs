using System.Reflection;

namespace Dwarf.Framework.LinqBinder.Binders;

class FieldBinder : MemberBinder<FieldInfo>
{
	public FieldBinder(IBinder parent, FieldInfo fi) : base(parent, fi) { }

	public override object? Value
	{
		get
		{
			var obj = parent.Value;
			return obj == null ? mInfo.FieldType.GetDefault() : mInfo.GetValue(obj);
		}
		set
		{
			var obj = parent.Value;
			if (obj != null)
				using (MakeSilent())
					mInfo.SetValue(obj, value);
			CallChangeTrigger();
		}
	}
}
