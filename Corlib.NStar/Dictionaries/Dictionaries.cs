using System.Diagnostics.CodeAnalysis;

namespace Corlib.NStar;

/// <summary>
/// Used internally to control behavior of insertion into a <see cref="Dictionary{TKey, TValue}"/> or <see cref="HashSet{T}"/>.
/// </summary>
internal enum InsertionBehavior : byte
{
	/// <summary>
	/// The default insertion behavior.
	/// </summary>
	None = 0,
	/// <summary>
	/// Specifies that an existing entry with the same key should be overwritten if encountered.
	/// </summary>
	OverwriteExisting = 1,
	/// <summary>
	/// Specifies that if an existing entry with the same key is encountered, an exception should be thrown.
	/// </summary>
	ThrowOnExisting = 2
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
// Оптимизированный словарь, который для маленького числа элементов использует поэлементное сравнение
// и добавление в конец (импортируя этот функционал из SortedDictionary, чтобы не создавать
// лишнее дублирование), а при увеличении числа элементов действует как классический словарь от Microsoft.
public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
	private SortedDictionary<TKey, TValue>? low;
	private G.Dictionary<TKey, TValue>? high;
	private bool isHigh;
	private readonly IEqualityComparer<TKey> comparer;
	[NonSerialized]
	private object? _syncRoot;

	private const int _hashThreshold = 64;

	public Dictionary() : this(EqualityComparer<TKey>.Default) { }

	public Dictionary(int capacity) : this(capacity, (IEqualityComparer<TKey>?)null) { }

	public Dictionary(IEqualityComparer<TKey>? comparer) : this(0, comparer) { }

	public Dictionary(Func<TKey, TKey, bool> equalFunction) : this(new EComparer<TKey>(equalFunction)) { }

	public Dictionary(Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(int capacity, IEqualityComparer<TKey>? comparer)
	{
		comparer ??= EqualityComparer<TKey>.Default;
		this.comparer = comparer;
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity <= _hashThreshold)
			low = new(capacity, new Comparer<TKey>((x, y) => comparer.Equals(x, y) ? 0 : -1));
		else
		{
			high = new(capacity);
			isHigh = true;
		}
	}

	public Dictionary(int capacity, Func<TKey, TKey, bool> equalFunction) : this(capacity, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(int capacity, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(capacity, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(G.IDictionary<TKey, TValue> dictionary) : this(dictionary, (IEqualityComparer<TKey>?)null) { }

	public Dictionary(G.IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer) : this(dictionary != null ? dictionary.Count : throw new ArgumentNullException(nameof(dictionary)), comparer)
	{
		if (dictionary.Count <= _hashThreshold)
			low = new(dictionary);
		else
		{
			high = new(dictionary, comparer);
			isHigh = true;
		}
	}

	public Dictionary(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction) : this(dictionary, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(dictionary, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection) : this(keyCollection, valueCollection, (IEqualityComparer<TKey>?)null) { }

	public Dictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, IEqualityComparer<TKey>? comparer) : this(new UnsortedDictionary<TKey, TValue>(keyCollection, valueCollection), comparer) { }

	public Dictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(IEnumerable<(TKey Key, TValue Value)> collection) : this(collection, (IEqualityComparer<TKey>?)null) { }

	public Dictionary(IEnumerable<(TKey Key, TValue Value)> collection, IEqualityComparer<TKey>? comparer) : this(new UnsortedDictionary<TKey, TValue>(collection), comparer) { }

	public Dictionary(IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction) : this(collection, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, (IEqualityComparer<TKey>?)null) { }

	public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer) : this(new UnsortedDictionary<TKey, TValue>(collection), comparer) { }

	public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction) : this(collection, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public virtual TValue this[TKey key]
	{
		get
		{
			if (!isHigh && low != null)
				return low[key];
			else if (high != null)
				return high[key];
			else
				throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		}
		set
		{
			if (!isHigh && low != null)
				low[key] = value;
			else if (high != null)
				high[key] = value;
			else
				throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
			if (!isHigh && low != null && Length >= _hashThreshold)
			{
				high = new(low, comparer);
				low = null;
				isHigh = true;
			}
		}
	}

	object? System.Collections.IDictionary.this[object key]
	{
		get
		{
			if (IsCompatibleKey(key))
				return this[(TKey)key];
			return null;
		}
		set
		{
			if (!IsCompatibleKey(key))
				throw new ArgumentNullException(nameof(key));
			try
			{
				var tempKey = (TKey)key;
				try
				{
					this[tempKey] = (TValue?)value ?? throw new ArgumentNullException(nameof(value));
				}
				catch (InvalidCastException)
				{
					throw new ArgumentException(null, nameof(value));
				}
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException(null, nameof(key));
			}
		}
	}

	public virtual IEqualityComparer<TKey> Comparer => comparer;

	public virtual int Length
	{
		get
		{
			if (!isHigh && low != null)
				return low.Length;
			else if (high != null)
				return high.Count;
			else
				throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		}
	}

	public virtual G.ICollection<TKey> Keys
	{
		get
		{
			if (!isHigh && low != null)
				return low.Keys;
			else if (high != null)
				return high.Keys;
			else
				throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		}
	}

	G.ICollection<TKey> G.IDictionary<TKey, TValue>.Keys => Keys;

	System.Collections.ICollection System.Collections.IDictionary.Keys
	{
		get
		{
			if (!isHigh && low != null)
				return low.GetKeyListHelper();
			else if (high != null)
				return high.Keys;
			else
				throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		}
	}

	IEnumerable<TKey> G.IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	public virtual G.ICollection<TValue> Values
	{
		get
		{
			if (!isHigh && low != null)
				return low.Values;
			else if (high != null)
				return high.Values;
			else
				throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		}
	}

	G.ICollection<TValue> G.IDictionary<TKey, TValue>.Values => Values;

	System.Collections.ICollection System.Collections.IDictionary.Values
	{
		get
		{
			if (!isHigh && low != null)
				return low.GetValueListHelper();
			else if (high != null)
				return high.Values;
			else
				throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		}
	}

	IEnumerable<TValue> G.IReadOnlyDictionary<TKey, TValue>.Values => Values;

	bool G.ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

	bool System.Collections.IDictionary.IsReadOnly => false;

	bool System.Collections.IDictionary.IsFixedSize => false;

	bool System.Collections.ICollection.IsSynchronized => false;

	object System.Collections.ICollection.SyncRoot
	{
		get
		{
			if (_syncRoot == null)
				Interlocked.CompareExchange(ref _syncRoot, new(), null);
			return _syncRoot;
		}
	}

	public virtual void Add(TKey key, TValue value)
	{
		if (!isHigh && low != null && Length >= _hashThreshold)
		{
			high = new(low, comparer);
			low = null;
			isHigh = true;
		}
		if (!isHigh && low != null)
			low.Add(key, value);
		else if (high != null)
			high.Add(key, value);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual void Add((TKey Key, TValue Value) item) => Add(item.Key, item.Value);

	void System.Collections.IDictionary.Add(object key, object? value)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		try
		{
			var tempKey = (TKey)key;
			try
			{
				Add(tempKey, (TValue?)value ?? throw new ArgumentNullException(nameof(value)));
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException(null, nameof(value));
			}
		}
		catch (InvalidCastException)
		{
			throw new ArgumentException(null, nameof(key));
		}
	}

	void G.ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair) => Add(keyValuePair.Key, keyValuePair.Value);

	public virtual void Clear()
	{
		if (!isHigh && low != null)
			low.Clear();
		else if (high != null)
			high.Clear();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	bool G.ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
	{
		if (TryGetValue(keyValuePair.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, keyValuePair.Value))
			return true;
		return false;
	}

	bool System.Collections.IDictionary.Contains(object key)
	{
		if (IsCompatibleKey(key))
			return ContainsKey((TKey)key);
		return false;
	}

	public virtual bool ContainsKey(TKey key)
	{
		if (!isHigh && low != null)
			return low.ContainsKey(key);
		else if (high != null)
			return high.ContainsKey(key);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	void G.ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		if (!isHigh && low != null)
			((G.ICollection<KeyValuePair<TKey, TValue>>)low).CopyTo(array, arrayIndex);
		else if (high != null)
			((G.ICollection<KeyValuePair<TKey, TValue>>)high).CopyTo(array, arrayIndex);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
	{
		if (!isHigh && low != null)
			((ICollection)low).CopyTo(array, arrayIndex);
		else if (high != null)
			((ICollection)high).CopyTo(array, arrayIndex);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		if (!isHigh && low != null)
			return low.GetEnumerator();
		else if (high != null)
			return high.GetEnumerator();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator()
	{
		if (!isHigh && low != null)
			return low.GetEnumerator();
		else if (high != null)
			return high.GetEnumerator();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private static bool IsCompatibleKey(object key)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		return key is TKey;
	}

	public virtual bool Remove(TKey key)
	{
		if (!isHigh && low != null)
			return low.Remove(key);
		else if (high != null)
			return high.Remove(key);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		if (!isHigh && low != null)
			return low.Remove(key, out value);
		else if (high != null)
			return high.Remove(key, out value);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	void System.Collections.IDictionary.Remove(object key)
	{
		if (IsCompatibleKey(key))
			Remove((TKey)key);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.RemoveValue(KeyValuePair<TKey, TValue> keyValuePair)
	{
		if (!isHigh && low != null)
			return ((G.ICollection<KeyValuePair<TKey, TValue>>)low).Remove(keyValuePair);
		else if (high != null)
			return ((G.ICollection<KeyValuePair<TKey, TValue>>)high).Remove(keyValuePair);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual void TrimExcess()
	{
		if (!isHigh && low != null)
			low.TrimExcess();
		else if (high != null)
			high.TrimExcess();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual bool TryAdd(TKey key, TValue value)
	{
		if (!ContainsKey(key))
		{
			Add(key, value);
			return true;
		}
		else
			return false;
	}

	public virtual bool TryGetValue(TKey key, out TValue value)
	{
		if (!isHigh && low != null)
			return low.TryGetValue(key, out value);
		else if (high != null)
			return high.TryGetValue(key, out value!);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}
}

internal class UnsortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
	private readonly List<TKey> keys;
	private readonly List<TValue> values;

	public UnsortedDictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection) => (keys, values) = (keyCollection, valueCollection).RemoveDoubles();

	public UnsortedDictionary(IEnumerable<(TKey Key, TValue Value)> collection) => (keys, values) = collection.RemoveDoubles(x => x.Key).Break();

	public UnsortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) => (keys, values) = collection.RemoveDoubles(x => x.Key).Break(x => x.Key, x => x.Value);

	public virtual TValue this[TKey key] => throw new NotSupportedException();

	TValue G.IDictionary<TKey, TValue>.this[TKey key] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

	public virtual IEnumerable<TKey> Keys => throw new NotSupportedException();

	public virtual int Length => keys.Length;

	public virtual IEnumerable<TValue> Values => throw new NotSupportedException();

	G.ICollection<TKey> G.IDictionary<TKey, TValue>.Keys => throw new NotSupportedException();

	G.ICollection<TValue> G.IDictionary<TKey, TValue>.Values => throw new NotSupportedException();

	public virtual bool IsReadOnly => false;

	public virtual void Add(TKey key, TValue value)
	{
		if (!ContainsKey(key))
		{
			keys.Add(key);
			values.Add(value);
		}
	}

	public virtual void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

	public virtual void Clear()
	{
		keys.Clear();
		values.Clear();
	}

	public virtual bool Contains(KeyValuePair<TKey, TValue> item)
	{
		var index = IndexOfKey(item.Key);
		return index >= 0 && EqualityComparer<TValue>.Default.Equals(values[index], item.Value);
	}

	public virtual bool ContainsKey(TKey key) => keys.Contains(key);

	public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotSupportedException();

	public virtual Enumerator GetEnumerator() => new(this);

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private int IndexOfKey(TKey key) => keys.IndexOf(key);

	public virtual bool Remove(TKey key) => throw new NotSupportedException();

	public virtual bool RemoveValue(KeyValuePair<TKey, TValue> item) => throw new NotSupportedException();

	public virtual bool TryGetValue(TKey key, out TValue value)
	{
		var index = IndexOfKey(key);
		if (index >= 0)
		{
			value = values[index];
			return true;
		}
		else
		{
			value = default!;
			return false;
		}
	}

	public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
	{
		private readonly UnsortedDictionary<TKey, TValue> _dict;
		private int index;

		public Enumerator(UnsortedDictionary<TKey, TValue> dictionary)
		{
			_dict = dictionary;
			index = 0;
			Current = default!;
		}

		public KeyValuePair<TKey, TValue> Current { get; private set; }

		readonly object IEnumerator.Current => Current;

		public void Dispose()
		{
			index = 0;
			Current = default!;
		}

		public bool MoveNext()
		{
			if (index < _dict.Length)
			{
				Current = new(_dict.keys[index], _dict.values[index++]);
				return true;
			}
			else
			{
				index = _dict.Length + 1;
				Current = default!;
				return false;
			}
		}

		public void Reset()
		{
			index = 0;
			Current = default!;
		}
	}
}
