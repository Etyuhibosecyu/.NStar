﻿using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using G = System.Collections.Generic;
using static BigCollections.Extents;
using static System.Math;

namespace BigCollections;

[DebuggerDisplay("Count = {Count}")]
[Serializable()]
[ComVisible(false)]
// Внимание! Несмотря на название "SortedDictionary", этот словарь не всегда является отсортированным!
// При небольшом количестве элементов проверка каждого на равенство ненамного медленнее
// бинарного поиска, который проверяет не только равенство, но и то, какой элемент "больше",
// а вставка в конец несравнимо быстрее, чем в середину. Разумеется, в большинстве случаев
// при использовании словаря пользователю нужен словарь, а не сортировка в чистом виде, поэтому
// он и получил словарь, а для сортировки в чистом виде есть метод List<T>.Sort(). Название "SortedDictionary",
// возможно, неудачное, но я не смог придумать лучшего, так как просто "Dictionary" используется
// другой коллекцией, которая применяется существенно чаще, чем данная, поэтому приоритет в очереди
// за более простым названием отдается ей. Если кто знает лучшее название для этой коллекции - пишите.
public class SortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
{
	private readonly List<TKey> keys;
	private readonly List<TValue> values;
	private readonly IComparer<TKey> comparer;
	private KeyList? keyList;
	private ValueList? valueList;
	[NonSerialized]
	private object? _syncRoot;

	private const int _defaultCapacity = 4;
	private const int _sortingThreshold = 64;

	public SortedDictionary()
	{
		keys = new();
		values = new();
		comparer = G.Comparer<TKey>.Default;
	}

	public SortedDictionary(int capacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		keys = new(capacity);
		values = new(capacity);
		comparer = G.Comparer<TKey>.Default;
	}

	public SortedDictionary(IComparer<TKey>? comparer) : this()
	{
		if (comparer != null)
			this.comparer = comparer;
	}

	public SortedDictionary(Func<TKey, TKey, int> compareFunction) : this(new Comparer<TKey>(compareFunction))
	{
	}

	public SortedDictionary(int capacity, IComparer<TKey>? comparer) : this(comparer) => Capacity = capacity;

	public SortedDictionary(int capacity, Func<TKey, TKey, int> compareFunction) : this(capacity, new Comparer<TKey>(compareFunction))
	{
	}

	public SortedDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, G.Comparer<TKey>.Default)
	{
	}

	public SortedDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey>? comparer) : this(dictionary != null ? dictionary.Count : throw new ArgumentNullException(nameof(dictionary)), comparer)
	{
		(keys, values) = dictionary.RemoveDoubles(x => x.Key).Break(x => x.Key, x => x.Value);
		if (keys.Count > SortedDictionary<TKey, TValue>._sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, int> compareFunction) : this(dictionary, new Comparer<TKey>(compareFunction))
	{
	}

	public SortedDictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection) : this(keyCollection, valueCollection, G.Comparer<TKey>.Default)
	{
	}

	public SortedDictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, IComparer<TKey>? comparer) : this(keyCollection != null && valueCollection != null ? List<TKey>.TryGetCountEasilyEnumerable(keyCollection, out int count) && List<TValue>.TryGetCountEasilyEnumerable(valueCollection, out int count2) ? Min(count, count2) : _defaultCapacity : throw new ArgumentNullException(null), comparer)
	{
		(keys, values) = (keyCollection, valueCollection).RemoveDoubles();
		if (keys.Count > SortedDictionary<TKey, TValue>._sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, Func<TKey, TKey, int> compareFunction) : this(keyCollection, valueCollection, new Comparer<TKey>(compareFunction))
	{
	}

	public SortedDictionary(IEnumerable<(TKey Key, TValue Value)> collection) : this(collection, G.Comparer<TKey>.Default)
	{
	}

	public SortedDictionary(IEnumerable<(TKey Key, TValue Value)> collection, IComparer<TKey>? comparer) : this(collection != null ? List<TKey>.TryGetCountEasilyEnumerable(collection, out int count) ? count : _defaultCapacity : throw new ArgumentNullException(nameof(collection)), comparer)
	{
		(keys, values) = collection.RemoveDoubles(x => x.Key).Break();
		if (keys.Count > SortedDictionary<TKey, TValue>._sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, int> compareFunction) : this(collection, new Comparer<TKey>(compareFunction))
	{
	}

	public SortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, G.Comparer<TKey>.Default)
	{
	}

	public SortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IComparer<TKey>? comparer) : this(collection != null ? List<TKey>.TryGetCountEasilyEnumerable(collection, out int count) ? count : _defaultCapacity : throw new ArgumentNullException(nameof(collection)), comparer)
	{
		(keys, values) = collection.RemoveDoubles(x => x.Key).Break(x => x.Key, x => x.Value);
		if (keys.Count > SortedDictionary<TKey, TValue>._sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, int> compareFunction) : this(collection, new Comparer<TKey>(compareFunction))
	{
	}

	public virtual TValue this[TKey key]
	{
		get
		{
			int i = IndexOfKey(key);
			if (i >= 0)
				return values[i];
			throw new KeyNotFoundException();
		}
		set
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			int i = Search(key);
			if (i >= 0)
			{
				values[i] = value;
				return;
			}
			Insert(~i, key, value);
		}
	}

	object? IDictionary.this[object key]
	{
		get
		{
			if (IsCompatibleKey(key))
			{
				int i = IndexOfKey((TKey)key);
				if (i >= 0)
					return values[i];
			}
			return null;
		}
		set
		{
			if (!IsCompatibleKey(key))
				throw new ArgumentNullException(nameof(key));
			try
			{
				TKey tempKey = (TKey)key;
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

	public virtual int Capacity { get => keys.Capacity; set => values.Capacity = keys.Capacity = value; }

	public IComparer<TKey> Comparer => comparer;

	public int Count => keys.Count;

	public IList<TKey> Keys => GetKeyListHelper();

	ICollection<TKey> IDictionary<TKey, TValue>.Keys => GetKeyListHelper();

	ICollection IDictionary.Keys => GetKeyListHelper();

	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => GetKeyListHelper();

	public IList<TValue> Values => GetValueListHelper();

	ICollection<TValue> IDictionary<TKey, TValue>.Values => GetValueListHelper();

	ICollection IDictionary.Values => GetValueListHelper();

	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => GetValueListHelper();

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

	bool IDictionary.IsReadOnly => false;

	bool IDictionary.IsFixedSize => false;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot
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
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		int i = Search(key);
		if (i >= 0)
			throw new ArgumentException(null);
		Insert(~i, key, value);
	}

	public virtual void Add((TKey Key, TValue Value) item) => Add(item.Key, item.Value);

	void IDictionary.Add(object key, object? value)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		try
		{
			TKey tempKey = (TKey)key;
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

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair) => Add(keyValuePair.Key, keyValuePair.Value);

	public virtual void Clear()
	{
		keys.Clear();
		values.Clear();
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
	{
		int index = IndexOfKey(keyValuePair.Key);
		if (index >= 0 && EqualityComparer<TValue>.Default.Equals(values[index], keyValuePair.Value))
			return true;
		return false;
	}

	bool IDictionary.Contains(object key)
	{
		if (IsCompatibleKey(key))
			return ContainsKey((TKey)key);
		return false;
	}

	public bool ContainsKey(TKey key) => IndexOfKey(key) >= 0;

	public bool ContainsValue(TValue value) => IndexOfValue(value) >= 0;

	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < Count)
			throw new ArgumentException(null);
		for (int i = 0; i < Count; i++)
		{
			KeyValuePair<TKey, TValue> entry = new(keys[i], values[i]);
			array[arrayIndex + i] = entry;
		}
	}

	void ICollection.CopyTo(Array array, int arrayIndex)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		if (array.Rank != 1)
			throw new ArgumentException(null);
		if (array.GetLowerBound(0) != 0)
			throw new ArgumentException(null);
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < Count)
			throw new ArgumentException(null);
		if (array is KeyValuePair<TKey, TValue>[] keyValuePairArray)
		{
			for (int i = 0; i < Count; i++)
				keyValuePairArray[i + arrayIndex] = new(keys[i], values[i]);
		}
		else
		{
			if (array is not object[] objects)
				throw new ArgumentException(null);
			try
			{
				for (int i = 0; i < Count; i++)
					objects[i + arrayIndex] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException(null);
			}
		}
	}

	public virtual TValue GetByIndex(int index)
	{
		if (index < 0 || index >= keys.Count)
			throw new ArgumentOutOfRangeException(nameof(index));
		return values[index];
	}

	public Enumerator GetEnumerator() => new(this, Enumerator.KeyValuePair);

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IDictionaryEnumerator IDictionary.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual TKey GetKey(int index)
	{
		if (index < 0 || index >= keys.Count) throw new ArgumentOutOfRangeException(nameof(index));
		return keys[index];
	}

	internal KeyList GetKeyListHelper()
	{
		if (keyList == null)
			keyList = new(this);
		return keyList;
	}

	internal ValueList GetValueListHelper()
	{
		if (valueList == null)
			valueList = new(this);
		return valueList;
	}

	public virtual int IndexOfKey(TKey key)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (keys.Count <= SortedDictionary<TKey, TValue>._sortingThreshold)
			return keys.FindIndex(x => comparer.Compare(x, key) == 0);
		int ret = Search(key);
		return ret >= 0 ? ret : -1;
	}

	public virtual int IndexOfValue(TValue value) => values.IndexOf(value, 0, keys.Count);

	private void Insert(int index, TKey key, TValue value)
	{
		keys.Insert(index, key);
		values.Insert(index, value);
		if (keys.Count == 65)
			keys.Sort(values, comparer);
	}

	private static bool IsCompatibleKey(object key)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		return key is TKey;
	}

	public virtual bool Remove(TKey key)
	{
		int i = IndexOfKey(key);
		if (i >= 0)
			RemoveAt(i);
		return i >= 0;
	}

	void IDictionary.Remove(object key)
	{
		if (IsCompatibleKey(key))
			Remove((TKey)key);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
	{
		int index = IndexOfKey(keyValuePair.Key);
		if (index >= 0 && EqualityComparer<TValue>.Default.Equals(values[index], keyValuePair.Value))
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}

	public virtual void RemoveAt(int index)
	{
		keys.RemoveAt(index);
		values.RemoveAt(index);
	}

	public virtual int Search(TKey key)
	{
		if (keys.Count <= SortedDictionary<TKey, TValue>._sortingThreshold)
		{
			int index = keys.IndexOf(key);
			if (index >= 0)
				return index;
			else
				return ~keys.Count;
		}
		else
			return keys.BinarySearch(key, comparer);
	}

	public virtual void SetByIndex(int index, TValue value)
	{
		if (index < 0 || index >= keys.Count)
			throw new ArgumentOutOfRangeException(nameof(index));
		values[index] = value;
	}

	public virtual void TrimExcess()
	{
		keys.TrimExcess();
		values.TrimExcess();
	}

	public virtual bool TryGetValue(TKey key, out TValue value)
	{
		int i = IndexOfKey(key);
		if (i >= 0)
		{
			value = values[i];
			return true;
		}
		value = default!;
		return false;
	}

	[Serializable()]
	public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
	{
		private readonly SortedDictionary<TKey, TValue> _sortedDictionary;
		private TKey key;
		private TValue value;
		private int index;
		private readonly int getEnumeratorRetType;

		internal const int KeyValuePair = 1;
		internal const int DictEntry = 2;

		internal Enumerator(SortedDictionary<TKey, TValue> sortedDictionary, int getEnumeratorRetType)
		{
			_sortedDictionary = sortedDictionary;
			index = 0;
			this.getEnumeratorRetType = getEnumeratorRetType;
			key = default!;
			value = default!;
		}

		object IDictionaryEnumerator.Key
		{
			get
			{
				if (index == 0 || (index == _sortedDictionary.Count + 1))
					throw new InvalidOperationException();
				return key!;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				if (index == 0 || (index == _sortedDictionary.Count + 1))
					throw new InvalidOperationException();
				if (getEnumeratorRetType == DictEntry)
					return new DictionaryEntry(key!, value);
				else
					return new KeyValuePair<TKey, TValue>(key, value);
			}
		}

		object IDictionaryEnumerator.Value
		{
			get
			{
				if (index == 0 || (index == _sortedDictionary.Count + 1))
					throw new InvalidOperationException();
				return value!;
			}
		}

		public void Dispose()
		{
			index = 0;
			key = default!;
			value = default!;
		}

		public bool MoveNext()
		{
			if ((uint)index < (uint)_sortedDictionary.Count)
			{
				key = _sortedDictionary.keys[index];
				value = _sortedDictionary.values[index];
				index++;
				return true;
			}
			index = _sortedDictionary.Count + 1;
			key = default!;
			value = default!;
			return false;
		}

		DictionaryEntry IDictionaryEnumerator.Entry
		{
			get
			{
				if (index == 0 || index == _sortedDictionary.Count + 1)
					throw new InvalidOperationException();
				return new(key!, value);
			}
		}

		public KeyValuePair<TKey, TValue> Current => new(key, value);

		void IEnumerator.Reset()
		{
			index = 0;
			key = default!;
			value = default!;
		}
	}

	[Serializable()]
	internal sealed class KeyEnumerator : IEnumerator<TKey>, IEnumerator
	{
		private readonly SortedDictionary<TKey, TValue> _sortedDictionary;
		private int index;
		private TKey currentKey;

		internal KeyEnumerator(SortedDictionary<TKey, TValue> sortedDictionary)
		{
			_sortedDictionary = sortedDictionary;
			currentKey = default!;
		}

		public void Dispose()
		{
			index = 0;
			currentKey = default!;
		}

		public bool MoveNext()
		{
			if ((uint)index < (uint)_sortedDictionary.Count)
			{
				currentKey = _sortedDictionary.keys[index];
				index++;
				return true;
			}
			index = _sortedDictionary.Count + 1;
			currentKey = default!;
			return false;
		}

		public TKey Current => currentKey;

		object IEnumerator.Current
		{
			get
			{
				if (index == 0 || (index == _sortedDictionary.Count + 1))
					throw new InvalidOperationException();
				return currentKey!;
			}
		}

		void IEnumerator.Reset()
		{
			index = 0;
			currentKey = default!;
		}
	}

	[Serializable()]
	internal sealed class ValueEnumerator : IEnumerator<TValue>, IEnumerator
	{
		private readonly SortedDictionary<TKey, TValue> _sortedDictionary;
		private int index;
		private TValue currentValue;

		internal ValueEnumerator(SortedDictionary<TKey, TValue> sortedDictionary)
		{
			_sortedDictionary = sortedDictionary;
			currentValue = default!;
		}

		public TValue Current => currentValue;

		object? IEnumerator.Current
		{
			get
			{
				if (index == 0 || (index == _sortedDictionary.Count + 1))
					throw new InvalidOperationException();
				return currentValue;
			}
		}

		public void Dispose()
		{
			index = 0;
			currentValue = default!;
		}

		public bool MoveNext()
		{
			if ((uint)index < (uint)_sortedDictionary.Count)
			{
				currentValue = _sortedDictionary.values[index];
				index++;
				return true;
			}
			index = _sortedDictionary.Count + 1;
			currentValue = default!;
			return false;
		}

		void IEnumerator.Reset()
		{
			index = 0;
			currentValue = default!;
		}
	}

	[DebuggerDisplay("Count = {Count}")]
	[Serializable()]
	internal sealed class KeyList : IList<TKey>, ICollection
	{
		private readonly SortedDictionary<TKey, TValue> _dict;

		internal KeyList(SortedDictionary<TKey, TValue> dictionary)
		{
			_dict = dictionary;
		}

		public TKey this[int index] { get => _dict.GetKey(index); set => throw new NotSupportedException(); }

		public int Count => _dict.Count;

		public bool IsReadOnly => true;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => ((ICollection)_dict).SyncRoot;

		public void Add(TKey key) => throw new NotSupportedException();

		public void Clear() => throw new NotSupportedException();

		public bool Contains(TKey key) => _dict.ContainsKey(key);

		public void CopyTo(TKey[] array, int arrayIndex) => _dict.keys.CopyTo(0, array, arrayIndex, _dict.Count);

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			if (array.Rank != 1)
				throw new ArgumentException(null);
			try
			{
				((ICollection)_dict.keys).CopyTo(array, arrayIndex);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException(null);
			}
		}

		internal KeyEnumerator GetEnumerator() => new(_dict);

		IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int IndexOf(TKey key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			int i = _dict.Search(key);
			if (i >= 0) return i;
			return -1;
		}

		public void Insert(int index, TKey value) => throw new NotSupportedException();

		public bool Remove(TKey key) => throw new NotSupportedException();

		public void RemoveAt(int index) => throw new NotSupportedException();
	}

	[DebuggerDisplay("Count = {Count}")]
	[Serializable()]
	internal sealed class ValueList : IList<TValue>, ICollection
	{
		private readonly SortedDictionary<TKey, TValue> _dict;

		internal ValueList(SortedDictionary<TKey, TValue> dictionary) => _dict = dictionary;

		public TValue this[int index] { get => _dict.GetByIndex(index); set => _dict.SetByIndex(index, value); }

		public int Count => _dict.Count;

		public bool IsReadOnly => true;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => ((ICollection)_dict).SyncRoot;

		public void Add(TValue key) => throw new NotSupportedException();

		public void Clear() => throw new NotSupportedException();

		public bool Contains(TValue value) => _dict.ContainsValue(value);

		public void CopyTo(TValue[] array, int arrayIndex) => _dict.values.CopyTo(0, array, arrayIndex, _dict.Count);

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			if (array.Rank != 1)
				throw new ArgumentException(null);
			try
			{
				((ICollection)_dict.values).CopyTo(array, arrayIndex);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException(null);
			}
		}

		internal ValueEnumerator GetEnumerator() => new(_dict);

		IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int IndexOf(TValue value) => _dict.values.IndexOf(value, 0, _dict.Count);

		public void Insert(int index, TValue value) => throw new NotSupportedException();

		public bool Remove(TValue value) => throw new NotSupportedException();

		public void RemoveAt(int index) => throw new NotSupportedException();
	}
}

[DebuggerDisplay("Count = {Count}")]
[Serializable()]
[ComVisible(false)]
// Оптимизированный словарь, который для маленького числа элементов использует поэлементное сравнение
// и добавление в конец (импортируя этот функционал из SortedDictionary, чтобы не создавать
// лишнее дублирование), а при увеличении числа элементов действует как классический словарь от Microsoft.
public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
	private SortedDictionary<TKey, TValue>? low = null;
	private G.Dictionary<TKey, TValue>? high = null;
	private bool isHigh = false;
	private readonly IEqualityComparer<TKey> comparer;
	[NonSerialized]
	private object? _syncRoot;

	private const int _hashThreshold = 64;

	public Dictionary() : this(EqualityComparer<TKey>.Default)
	{
	}

	public Dictionary(int capacity) : this(capacity, EqualityComparer<TKey>.Default)
	{
	}

	public Dictionary(IEqualityComparer<TKey>? comparer) : this(0, comparer)
	{
	}

	public Dictionary(Func<TKey, TKey, bool> equalFunction) : this(new EComparer<TKey>(equalFunction))
	{
	}

	public Dictionary(Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(new EComparer<TKey>(equalFunction, hashCodeFunction))
	{
	}

	public Dictionary(int capacity, IEqualityComparer<TKey>? comparer)
	{
		if (comparer == null)
			comparer = EqualityComparer<TKey>.Default;
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

	public Dictionary(int capacity, Func<TKey, TKey, bool> equalFunction) : this(capacity, new EComparer<TKey>(equalFunction))
	{
	}

	public Dictionary(int capacity, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(capacity, new EComparer<TKey>(equalFunction, hashCodeFunction))
	{
	}

	public Dictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, EqualityComparer<TKey>.Default)
	{
	}

	public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer) : this(dictionary != null ? dictionary.Count : throw new ArgumentNullException(nameof(dictionary)), comparer)
	{
		if (dictionary.Count <= _hashThreshold)
			low = new(dictionary);
		else
		{
			high = new(dictionary);
			isHigh = true;
		}
	}

	public Dictionary(IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction) : this(dictionary, new EComparer<TKey>(equalFunction))
	{
	}

	public Dictionary(IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(dictionary, new EComparer<TKey>(equalFunction, hashCodeFunction))
	{
	}

	public Dictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection) : this(keyCollection, valueCollection, EqualityComparer<TKey>.Default)
	{
	}

	public Dictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, IEqualityComparer<TKey>? comparer) : this(new UnsortedDictionary(keyCollection, valueCollection), comparer)
	{
	}

	public Dictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction))
	{
	}

	public Dictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction, hashCodeFunction))
	{
	}

	public Dictionary(IEnumerable<(TKey Key, TValue Value)> collection) : this(collection, EqualityComparer<TKey>.Default)
	{
	}

	public Dictionary(IEnumerable<(TKey Key, TValue Value)> collection, IEqualityComparer<TKey>? comparer) : this(new UnsortedDictionary(collection), comparer)
	{
	}

	public Dictionary(IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction) : this(collection, new EComparer<TKey>(equalFunction))
	{
	}

	public Dictionary(IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction))
	{
	}

	public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, EqualityComparer<TKey>.Default)
	{
	}

	public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer) : this(new UnsortedDictionary(collection), comparer)
	{
	}

	public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction) : this(collection, new EComparer<TKey>(equalFunction))
	{
	}

	public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction))
	{
	}

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
			if (!isHigh && low != null && Count >= _hashThreshold)
			{
				high = new(low);
				low = null;
				isHigh = true;
			}
		}
	}

	object? IDictionary.this[object key]
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
				TKey tempKey = (TKey)key;
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

	public IEqualityComparer<TKey> Comparer => comparer;

	public int Count
	{
		get
		{
			if (!isHigh && low != null)
				return low.Count;
			else if (high != null)
				return high.Count;
			else
				throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		}
	}

	public ICollection<TKey> Keys
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

	ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

	ICollection IDictionary.Keys
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

	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	public ICollection<TValue> Values
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

	ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

	ICollection IDictionary.Values
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

	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

	bool IDictionary.IsReadOnly => false;

	bool IDictionary.IsFixedSize => false;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot
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
		if (!isHigh && low != null && Count >= _hashThreshold)
		{
			high = new(low);
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

	void IDictionary.Add(object key, object? value)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		try
		{
			TKey tempKey = (TKey)key;
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

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair) => Add(keyValuePair.Key, keyValuePair.Value);

	public virtual void Clear()
	{
		if (!isHigh && low != null)
			low.Clear();
		else if (high != null)
			high.Clear();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
	{
		if (ContainsKey(keyValuePair.Key) && EqualityComparer<TValue>.Default.Equals(this[keyValuePair.Key], keyValuePair.Value))
			return true;
		return false;
	}

	bool IDictionary.Contains(object key)
	{
		if (IsCompatibleKey(key))
			return ContainsKey((TKey)key);
		return false;
	}

	public bool ContainsKey(TKey key)
	{
		if (!isHigh && low != null)
			return low.ContainsKey(key);
		else if (high != null)
			return high.ContainsKey(key);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		if (!isHigh && low != null)
			((ICollection<KeyValuePair<TKey, TValue>>)low).CopyTo(array, arrayIndex);
		else if (high != null)
			((ICollection<KeyValuePair<TKey, TValue>>)high).CopyTo(array, arrayIndex);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	void ICollection.CopyTo(Array array, int arrayIndex)
	{
		if (!isHigh && low != null)
			((ICollection)low).CopyTo(array, arrayIndex);
		else if (high != null)
			((ICollection)high).CopyTo(array, arrayIndex);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		if (!isHigh && low != null)
			return low.GetEnumerator();
		else if (high != null)
			return high.GetEnumerator();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IDictionaryEnumerator IDictionary.GetEnumerator()
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

	void IDictionary.Remove(object key)
	{
		if (IsCompatibleKey(key))
			Remove((TKey)key);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
	{
		if (!isHigh && low != null)
			return ((ICollection<KeyValuePair<TKey, TValue>>)low).Remove(keyValuePair);
		else if (high != null)
			return ((ICollection<KeyValuePair<TKey, TValue>>)high).Remove(keyValuePair);
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

	public virtual bool TryGetValue(TKey key, out TValue value)
	{
		if (!isHigh && low != null)
			return low.TryGetValue(key, out value);
		else if (high != null)
			return high.TryGetValue(key, out value!);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private class UnsortedDictionary : IDictionary<TKey, TValue>
	{
		private readonly List<TKey> keys;
		private readonly List<TValue> values;

		public UnsortedDictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection)
		{
			(keys, values) = (keyCollection, valueCollection).RemoveDoubles();
		}

		public UnsortedDictionary(IEnumerable<(TKey Key, TValue Value)> collection)
		{
			(keys, values) = collection.RemoveDoubles(x => x.Key).Break();
		}

		public UnsortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
		{
			(keys, values) = collection.RemoveDoubles(x => x.Key).Break(x => x.Key, x => x.Value);
		}

		public TValue this[TKey key] => throw new NotSupportedException();

		TValue IDictionary<TKey, TValue>.this[TKey key] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public int Count => keys.Count;

		public IEnumerable<TKey> Keys => throw new NotSupportedException();

		public IEnumerable<TValue> Values => throw new NotSupportedException();

		ICollection<TKey> IDictionary<TKey, TValue>.Keys => throw new NotSupportedException();

		ICollection<TValue> IDictionary<TKey, TValue>.Values => throw new NotSupportedException();

		public bool IsReadOnly => false;

		public void Add(TKey key, TValue value)
		{
			if (!ContainsKey(key))
			{
				keys.Add(key);
				values.Add(value);
			}
		}

		public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

		public void Clear()
		{
			keys.Clear();
			values.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			int index = IndexOfKey(item.Key);
			return index >= 0 && EqualityComparer<TValue>.Default.Equals(values[index], item.Value);
		}

		public bool ContainsKey(TKey key) => keys.Contains(key);

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public Enumerator GetEnumerator() => new(this);

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private int IndexOfKey(TKey key) => keys.IndexOf(key);

		public bool Remove(TKey key)
		{
			throw new NotSupportedException();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			int index = IndexOfKey(key);
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
			private readonly UnsortedDictionary _dict;
			private int index = 0;

			public Enumerator(UnsortedDictionary dictionary)
			{
				_dict = dictionary;
				Current = default!;
			}

			public KeyValuePair<TKey, TValue> Current { get; private set; }

			object IEnumerator.Current => Current;

			public void Dispose()
			{
				index = 0;
				Current = default!;
			}

			public bool MoveNext()
			{
				if (index < _dict.Count)
				{
					Current = new(_dict.keys[index], _dict.values[index++]);
					return true;
				}
				else
				{
					index = _dict.Count + 1;
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
}
