using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Dwarf.Framework.WeakReferenceUtils;

#region WeakDictionaryKey

class WeakDictionaryKey : WeakReference
{
	readonly int hashCode;

	public WeakDictionaryKey(object target)
		: base(target)
	{
		hashCode = target.GetHashCode();
	}

	public override bool Equals(object? obj)
	{
		if (obj == null || obj.GetHashCode() != hashCode)
			return false;

		if (obj is WeakDictionaryKey wdk)
		{
			if (IsAlive && wdk.IsAlive)
				return Target!.Equals(wdk.Target);
			if (!IsAlive && !wdk.IsAlive)
				return ReferenceEquals(this, obj);
			return false;
		}

		return IsAlive && Target!.Equals(obj);
	}

	public override int GetHashCode()
	{
		return hashCode;
	}
}

#endregion

public class WeakDictionary<TKey, TVal> : IDictionary<TKey, TVal> where TKey : notnull
{
	#region Базовая часть

	readonly Dictionary<object, TVal> innerDict;
	readonly IStumpCleaner cleaner;
	Action<TVal>? onRemove;

	public WeakDictionary() : this(new GCStumpCleaner()) { }

	public WeakDictionary(int capacity)
	{
		innerDict = new Dictionary<object, TVal>(capacity);
		cleaner = new GCStumpCleaner();
	}

	public WeakDictionary(IStumpCleaner cleaner)
	{
		innerDict = [];
		this.cleaner = cleaner;
	}

	public WeakDictionary<TKey, TVal> WithRemoveCallback(Action<TVal> onRemove)
	{
		this.onRemove = onRemove;
		return this;
	}

	#endregion Базовая часть

	#region IDictionary<TKey,TVal> Members

	public void Add(TKey key, TVal value)
	{
		lock (innerDict)
			innerDict.Add(new WeakDictionaryKey(key), value);
		StartCleaner();
	}

	public bool ContainsKey(TKey key)
	{
		return innerDict.ContainsKey(key);
	}

	ICollection<TKey> IDictionary<TKey, TVal>.Keys
	{
		get { throw new NotImplementedException(); }
	}

	ICollection<TVal> IDictionary<TKey, TVal>.Values
	{
		get { throw new NotImplementedException(); }
	}

	bool InternalRemove(object key)
	{
		if (onRemove != null && innerDict.TryGetValue(key, out TVal? value))
			onRemove(value);
		return innerDict.Remove(key);
	}

	public bool Remove(TKey key)
	{
		lock (innerDict)
			return InternalRemove(key);
	}

	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TVal value) => innerDict.TryGetValue(key, out value);

	public TVal this[TKey key]
	{
		get => innerDict[key];
		set
		{
			lock (innerDict)
			{
				if (!innerDict.ContainsKey(key))
					Add(key, value);
				else
				{
					var oldVal = innerDict[key];
					if (oldVal != null && !oldVal.Equals(value))
					{
						onRemove?.Invoke(oldVal);
						innerDict[key] = value;
					}
				}
			}
		}
	}

	#endregion

	#region ICollection<KeyValuePair<TKey,TVal>> Members

	void ICollection<KeyValuePair<TKey, TVal>>.Add(KeyValuePair<TKey, TVal> item) => throw new NotImplementedException();

	public void Clear()
	{
		lock (innerDict)
		{
			if (onRemove != null)
				foreach (var v in innerDict.Values)
					onRemove(v);
			innerDict.Clear();
		}
	}

	bool ICollection<KeyValuePair<TKey, TVal>>.Contains(KeyValuePair<TKey, TVal> item) => throw new NotImplementedException();
	void ICollection<KeyValuePair<TKey, TVal>>.CopyTo(KeyValuePair<TKey, TVal>[] array, int arrayIndex) => throw new NotImplementedException();
	int ICollection<KeyValuePair<TKey, TVal>>.Count => throw new NotImplementedException();

	public bool IsReadOnly => false;

	bool ICollection<KeyValuePair<TKey, TVal>>.Remove(KeyValuePair<TKey, TVal> item) => throw new NotImplementedException();

	#endregion

	#region IEnumerable<KeyValuePair<TKey,TVal>> Members

	public IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator() => throw new NotImplementedException();

	#endregion

	#region IEnumerable Members

	IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

	#endregion

	#region Cleaner

	void StartCleaner()
	{
		if (cleaner != null && !cleaner.IsStarted)
			cleaner.Start(CleanStump);
	}

	void CleanStump()
	{
		lock (innerDict)
		{
			var stumpKeys = innerDict.Keys.Where(k => !((WeakDictionaryKey)k).IsAlive).ToArray();
			foreach (var k in stumpKeys)
				InternalRemove(k);
			if (innerDict.Count == 0)
				cleaner.Stop();
		}
	}

	#endregion

}
