using System.Collections;

namespace Dwarf.Framework.WeakReferenceUtils;

public class WeakReferencePool<T> : IEnumerable<T> where T : class
{
	private readonly List<WeakReference<T>> refList = [];

	public List<WeakReference<T>> RefList => refList;

	public IEnumerator<T> GetEnumerator()
	{
		var ind = 0;
		while (ind < RefList.Count)
		{
			var _ref = RefList[ind];
			if (_ref.TryGetTarget(out var obj))
			{
				ind++;
				yield return obj;
			}
			else RefList.Remove(_ref);
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Push(T item) => RefList.Add(new WeakReference<T>(item));
}
