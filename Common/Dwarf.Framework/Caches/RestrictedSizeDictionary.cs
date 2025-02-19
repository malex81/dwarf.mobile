using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Dwarf.Framework.Caches;

public class RestrictedSizeDictionary<TKey, TValue>(int maxSize) : IDictionary<TKey, TValue> where TKey : notnull
{
	private Dictionary<TKey, TValue> old = [];
	private Dictionary<TKey, TValue> current = [];

	void CheckOversize()
	{
		if (current.Count >= MaxSize)
		{
			old = current;
			current = [];
		}
	}

	public TValue this[TKey key]
	{
		get => current.TryGetValue(key, out var res) ? res : old[key];
		set
		{
			current[key] = value;
			CheckOversize();
		}
	}
	public ICollection<TKey> Keys => current.Keys.Union(old.Keys).ToList();
	public ICollection<TValue> Values => throw new NotImplementedException();
	public int Count => throw new NotImplementedException();
	public bool IsReadOnly => false;
	public int MaxSize { get; } = maxSize;

	public void Add(TKey key, TValue value)
	{
		current.Add(key, value);
		CheckOversize();
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		((ICollection<KeyValuePair<TKey, TValue>>)current).Add(item);
		CheckOversize();
	}

	public void Clear()
	{
		current.Clear();
		old.Clear();
	}

	public bool Contains(KeyValuePair<TKey, TValue> item) => current.Contains(item) || old.Contains(item);
	public bool ContainsKey(TKey key) => current.ContainsKey(key) || old.ContainsKey(key);

	public bool Remove(TKey key)
	{
		var res = current.Remove(key);
		return old.Remove(key) || res;
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		var res = ((ICollection<KeyValuePair<TKey, TValue>>)current).Remove(item);
		return ((ICollection<KeyValuePair<TKey, TValue>>)old).Remove(item) || res;
	}

	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => current.TryGetValue(key, out value) || old.TryGetValue(key, out value);

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => throw new NotImplementedException();
	IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}
