using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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
/// <summary>
/// Внимание! Несмотря на название "SortedDictionary", этот словарь не всегда является отсортированным!
/// При небольшом количестве элементов проверка каждого на равенство ненамного медленнее
/// бинарного поиска, который проверяет не только равенство, но и то, какой элемент "больше",
/// а вставка в конец несравнимо быстрее, чем в середину. Разумеется, в большинстве случаев
/// при использовании словаря пользователю нужен словарь, а не сортировка в чистом виде, поэтому он и
/// получил словарь, а для сортировки в чистом виде есть метод List<T>.Sort(). Название "SortedDictionary",
/// возможно, неудачное, но я не смог придумать лучшего, так как просто "Dictionary" используется
/// другой коллекцией, которая применяется существенно чаще, чем данная, поэтому приоритет в очереди
/// за более простым названием отдается ей. Если кто знает лучшее название для этой коллекции - пишите.
/// </summary>
public class SortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
{
	private readonly List<TKey> keys;
	private readonly List<TValue> values;
	private readonly IComparer<TKey> comparer;
	private KeyList? keyList;
	private ValueList? valueList;
	[NonSerialized]
	private object? _syncRoot;

	private const int _defaultCapacity = 32;
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

	public SortedDictionary(Func<TKey, TKey, int> compareFunction) : this(new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(int capacity, IComparer<TKey>? comparer) : this(comparer) => Capacity = capacity;

	public SortedDictionary(int capacity, Func<TKey, TKey, int> compareFunction) : this(capacity, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(G.IDictionary<TKey, TValue> dictionary) : this(dictionary, G.Comparer<TKey>.Default) { }

	public SortedDictionary(G.IDictionary<TKey, TValue> dictionary, IComparer<TKey>? comparer) : this(dictionary != null ? dictionary.Count : throw new ArgumentNullException(nameof(dictionary)), comparer)
	{
		(keys, values) = dictionary.RemoveDoubles(x => x.Key).Break(x => x.Key, x => x.Value);
		if (keys.Length > SortedDictionary<TKey, TValue>._sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, int> compareFunction) : this(dictionary, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection) : this(keyCollection, valueCollection, G.Comparer<TKey>.Default) { }

	public SortedDictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, IComparer<TKey>? comparer) : this(keyCollection != null && valueCollection != null ? List<TKey>.TryGetLengthEasilyEnumerable(keyCollection, out var length) && List<TValue>.TryGetLengthEasilyEnumerable(valueCollection, out var count2) ? Min(length, count2) : _defaultCapacity : throw new ArgumentNullException(null), comparer)
	{
		(keys, values) = (keyCollection, valueCollection).RemoveDoubles();
		if (keys.Length > SortedDictionary<TKey, TValue>._sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, Func<TKey, TKey, int> compareFunction) : this(keyCollection, valueCollection, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(IEnumerable<(TKey Key, TValue Value)> collection) : this(collection, G.Comparer<TKey>.Default) { }

	public SortedDictionary(IEnumerable<(TKey Key, TValue Value)> collection, IComparer<TKey>? comparer) : this(collection != null ? List<TKey>.TryGetLengthEasilyEnumerable(collection, out var length) ? length : _defaultCapacity : throw new ArgumentNullException(nameof(collection)), comparer)
	{
		(keys, values) = collection.RemoveDoubles(x => x.Key).Break();
		if (keys.Length > SortedDictionary<TKey, TValue>._sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, int> compareFunction) : this(collection, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, G.Comparer<TKey>.Default) { }

	public SortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IComparer<TKey>? comparer) : this(collection != null ? List<TKey>.TryGetLengthEasilyEnumerable(collection, out var length) ? length : _defaultCapacity : throw new ArgumentNullException(nameof(collection)), comparer)
	{
		(keys, values) = collection.RemoveDoubles(x => x.Key).Break(x => x.Key, x => x.Value);
		if (keys.Length > SortedDictionary<TKey, TValue>._sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, int> compareFunction) : this(collection, new Comparer<TKey>(compareFunction)) { }

	public virtual TValue this[TKey key]
	{
		get
		{
			var i = IndexOfKey(key);
			if (i >= 0)
				return values[i];
			throw new KeyNotFoundException();
		}
		set
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			var i = Search(key);
			if (i >= 0)
			{
				values[i] = value;
				return;
			}
			Insert(~i, key, value);
		}
	}

	object? System.Collections.IDictionary.this[object key]
	{
		get
		{
			if (IsCompatibleKey(key))
			{
				var i = IndexOfKey((TKey)key);
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

	public virtual int Capacity { get => keys.Capacity; set => values.Capacity = keys.Capacity = value; }

	public virtual IComparer<TKey> Comparer => comparer;

	public virtual int Length => keys.Length;

	public virtual IList<TKey> Keys => GetKeyListHelper();

	G.ICollection<TKey> G.IDictionary<TKey, TValue>.Keys => GetKeyListHelper();

	System.Collections.ICollection System.Collections.IDictionary.Keys => GetKeyListHelper();

	IEnumerable<TKey> G.IReadOnlyDictionary<TKey, TValue>.Keys => GetKeyListHelper();

	public virtual IList<TValue> Values => GetValueListHelper();

	G.ICollection<TValue> G.IDictionary<TKey, TValue>.Values => GetValueListHelper();

	System.Collections.ICollection System.Collections.IDictionary.Values => GetValueListHelper();

	IEnumerable<TValue> G.IReadOnlyDictionary<TKey, TValue>.Values => GetValueListHelper();

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
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		var i = Search(key);
		if (i >= 0)
			throw new ArgumentException(null);
		Insert(~i, key, value);
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
		keys.Clear();
		values.Clear();
	}

	bool G.ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
	{
		var index = IndexOfKey(keyValuePair.Key);
		if (index >= 0 && EqualityComparer<TValue>.Default.Equals(values[index], keyValuePair.Value))
			return true;
		return false;
	}

	bool System.Collections.IDictionary.Contains(object key)
	{
		if (IsCompatibleKey(key))
			return ContainsKey((TKey)key);
		return false;
	}

	public virtual bool ContainsKey(TKey key) => IndexOfKey(key) >= 0;

	public virtual bool ContainsValue(TValue value) => IndexOfValue(value) >= 0;

	void G.ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < Length)
			throw new ArgumentException(null);
		for (var i = 0; i < Length; i++)
		{
			KeyValuePair<TKey, TValue> entry = new(keys[i], values[i]);
			array[arrayIndex + i] = entry;
		}
	}

	void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		if (array.Rank != 1)
			throw new ArgumentException(null);
		if (array.GetLowerBound(0) != 0)
			throw new ArgumentException(null);
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < Length)
			throw new ArgumentException(null);
		if (array is KeyValuePair<TKey, TValue>[] keyValuePairArray)
			for (var i = 0; i < Length; i++)
				keyValuePairArray[i + arrayIndex] = new(keys[i], values[i]);
		else
		{
			if (array is not object[] objects)
				throw new ArgumentException(null);
			try
			{
				for (var i = 0; i < Length; i++)
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
		if (index < 0 || index >= keys.Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		return values[index];
	}

	public virtual Enumerator GetEnumerator() => new(this, Enumerator.KeyValuePair);

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual TKey GetKey(int index)
	{
		if (index < 0 || index >= keys.Length) throw new ArgumentOutOfRangeException(nameof(index));
		return keys[index];
	}

	internal KeyList GetKeyListHelper()
	{
		keyList ??= new(this);
		return keyList;
	}

	internal ValueList GetValueListHelper()
	{
		valueList ??= new(this);
		return valueList;
	}

	public virtual int IndexOfKey(TKey key)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (keys.Length <= SortedDictionary<TKey, TValue>._sortingThreshold)
			return keys.FindIndex(x => comparer.Compare(x, key) == 0);
		var ret = Search(key);
		return ret >= 0 ? ret : -1;
	}

	public virtual int IndexOfValue(TValue value) => values.IndexOf(value, 0, keys.Length);

	private void Insert(int index, TKey key, TValue value)
	{
		keys.Insert(index, key);
		values.Insert(index, value);
		if (keys.Length == 65)
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
		var i = IndexOfKey(key);
		if (i >= 0)
			RemoveAt(i);
		return i >= 0;
	}

	void System.Collections.IDictionary.Remove(object key)
	{
		if (IsCompatibleKey(key))
			Remove((TKey)key);
	}

	public virtual void RemoveAt(int index)
	{
		keys.RemoveAt(index);
		values.RemoveAt(index);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.RemoveValue(KeyValuePair<TKey, TValue> keyValuePair)
	{
		var index = IndexOfKey(keyValuePair.Key);
		if (index >= 0 && EqualityComparer<TValue>.Default.Equals(values[index], keyValuePair.Value))
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}

	public virtual int Search(TKey key)
	{
		if (keys.Length <= SortedDictionary<TKey, TValue>._sortingThreshold)
		{
			var index = keys.IndexOf(key);
			return index >= 0 ? index : ~keys.Length;
		}
		else
			return keys.BinarySearch(key, comparer);
	}

	public virtual void SetByIndex(int index, TValue value)
	{
		if (index < 0 || index >= keys.Length)
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
		var i = IndexOfKey(key);
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

		readonly object IDictionaryEnumerator.Key
		{
			get
			{
				if (index == 0 || index == _sortedDictionary.Length + 1)
					throw new InvalidOperationException();
				return key!;
			}
		}

		readonly object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == _sortedDictionary.Length + 1)
					throw new InvalidOperationException();
				if (getEnumeratorRetType == DictEntry)
					return new DictionaryEntry(key!, value);
				else
					return new KeyValuePair<TKey, TValue>(key, value);
			}
		}

		readonly object IDictionaryEnumerator.Value
		{
			get
			{
				if (index == 0 || index == _sortedDictionary.Length + 1)
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
			if ((uint)index < (uint)_sortedDictionary.Length)
			{
				key = _sortedDictionary.keys[index];
				value = _sortedDictionary.values[index];
				index++;
				return true;
			}
			index = _sortedDictionary.Length + 1;
			key = default!;
			value = default!;
			return false;
		}

		readonly DictionaryEntry IDictionaryEnumerator.Entry
		{
			get
			{
				if (index == 0 || index == _sortedDictionary.Length + 1)
					throw new InvalidOperationException();
				return new(key!, value);
			}
		}

		public readonly KeyValuePair<TKey, TValue> Current => new(key, value);

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
			if ((uint)index < (uint)_sortedDictionary.Length)
			{
				currentKey = _sortedDictionary.keys[index];
				index++;
				return true;
			}
			index = _sortedDictionary.Length + 1;
			currentKey = default!;
			return false;
		}

		public TKey Current => currentKey;

		object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == _sortedDictionary.Length + 1)
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
				if (index == 0 || index == _sortedDictionary.Length + 1)
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
			if ((uint)index < (uint)_sortedDictionary.Length)
			{
				currentValue = _sortedDictionary.values[index];
				index++;
				return true;
			}
			index = _sortedDictionary.Length + 1;
			currentValue = default!;
			return false;
		}

		void IEnumerator.Reset()
		{
			index = 0;
			currentValue = default!;
		}
	}

	[DebuggerDisplay("Length = {Length}"), Serializable]
	internal sealed class KeyList : IList<TKey>, ICollection
	{
		private readonly SortedDictionary<TKey, TValue> _dict;

		internal KeyList(SortedDictionary<TKey, TValue> dictionary) => _dict = dictionary;

		public TKey this[int index] { get => _dict.GetKey(index); set => throw new NotSupportedException(); }

		public int Length => _dict.Length;

		public bool IsReadOnly => true;

		bool System.Collections.ICollection.IsSynchronized => false;

		object System.Collections.ICollection.SyncRoot => ((ICollection)_dict).SyncRoot;

		public void Add(TKey key) => throw new NotSupportedException();

		public void Clear() => throw new NotSupportedException();

		public bool Contains(TKey key) => _dict.ContainsKey(key);

		public void CopyTo(TKey[] array, int arrayIndex) => _dict.keys.CopyTo(0, array, arrayIndex, _dict.Length);

		void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
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
			var i = _dict.Search(key);
			return i >= 0 ? i : -1;
		}

		public void Insert(int index, TKey value) => throw new NotSupportedException();

		public void RemoveAt(int index) => throw new NotSupportedException();

		public bool RemoveValue(TKey key) => throw new NotSupportedException();
	}

	[DebuggerDisplay("Length = {Length}"), Serializable]
	internal sealed class ValueList : IList<TValue>, ICollection
	{
		private readonly SortedDictionary<TKey, TValue> _dict;

		internal ValueList(SortedDictionary<TKey, TValue> dictionary) => _dict = dictionary;

		public TValue this[int index] { get => _dict.GetByIndex(index); set => _dict.SetByIndex(index, value); }

		public int Length => _dict.Length;

		public bool IsReadOnly => true;

		bool System.Collections.ICollection.IsSynchronized => false;

		object System.Collections.ICollection.SyncRoot => ((ICollection)_dict).SyncRoot;

		public void Add(TValue key) => throw new NotSupportedException();

		public void Clear() => throw new NotSupportedException();

		public bool Contains(TValue value) => _dict.ContainsValue(value);

		public void CopyTo(TValue[] array, int arrayIndex) => _dict.values.CopyTo(0, array, arrayIndex, _dict.Length);

		void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
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

		public int IndexOf(TValue value) => _dict.values.IndexOf(value, 0, _dict.Length);

		public void Insert(int index, TValue value) => throw new NotSupportedException();

		public void RemoveAt(int index) => throw new NotSupportedException();

		public bool RemoveValue(TValue value) => throw new NotSupportedException();
	}
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

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class Mirror<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue> where TKey : notnull where TValue : notnull
{
	private protected struct Entry
	{
		public uint hashCode;
		public uint hashCodeM;
		/// <summary>
		/// 0-based index of next entry in chain: -1 means end of chain
		/// also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
		/// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
		/// </summary>
		public int next;
		public int nextM;
		public TKey key;     // Key of entry
		public TValue value; // Value of entry
	}

	private protected int[]? _buckets;
	private protected int[]? _bucketsM;
	private protected Entry[]? _entries;
	private protected int _count;
	private protected int _freeList;
	private protected int _freeListM;
	private protected int _freeCount;
	private protected int _version;
	private protected IEqualityComparer<TKey> _comparer;
	private protected IEqualityComparer<TValue> _comparerM;
	private protected KeyCollection? _keys;
	private protected ValueCollection? _values;
	private protected const int StartOfFreeList = -3;

	public Mirror() : this(0, (IEqualityComparer<TKey>?)null, null) { }

	public Mirror(int capacity) : this(capacity, (IEqualityComparer<TKey>?)null, null) { }

	public Mirror(IEqualityComparer<TKey>? keyComparer) : this(0, keyComparer, null) { }

	public Mirror(Func<TKey, TKey, bool> equalFunction) : this(new EComparer<TKey>(equalFunction)) { }

	public Mirror(Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer) : this(0, keyComparer, valueComparer) { }

	public Mirror(Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public Mirror(int capacity, IEqualityComparer<TKey>? keyComparer) : this(capacity, keyComparer, null) { }

	public Mirror(int capacity, Func<TKey, TKey, bool> equalFunction) : this(capacity, new EComparer<TKey>(equalFunction)) { }

	public Mirror(int capacity, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(capacity, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(int capacity, IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity > 0)
			Initialize(capacity);
		if (!typeof(TKey).IsValueType)
			_comparer = keyComparer ?? EqualityComparer<TKey>.Default;
		else if (keyComparer != null && keyComparer != EqualityComparer<TKey>.Default) // first check for null to avoid forcing default comparer instantiation unnecessarily
			_comparer = keyComparer;
		else
			_comparer = EqualityComparer<TKey>.Default;
		if (!typeof(TValue).IsValueType)
			_comparerM = valueComparer ?? EqualityComparer<TValue>.Default;
		else if (valueComparer != null && valueComparer != EqualityComparer<TValue>.Default) // first check for null to avoid forcing default comparer instantiation unnecessarily
			_comparerM = valueComparer;
		else
			_comparerM = EqualityComparer<TValue>.Default;
	}

	public Mirror(int capacity, Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(capacity, new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(int capacity, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(capacity, new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary) : this(dictionary, (IEqualityComparer<TKey>?)null, null) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? keyComparer) : this(dictionary, keyComparer, null) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction) : this(dictionary, new EComparer<TKey>(equalFunction)) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(dictionary, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer) : this(dictionary?.Count ?? 0, keyComparer, valueComparer)
	{
		if (dictionary == null)
			throw new ArgumentNullException(nameof(dictionary));
		AddRange(dictionary);
	}

	public Mirror(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(dictionary, new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(dictionary, new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public Mirror(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, (IEqualityComparer<TKey>?)null, null) { }

	public Mirror(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? keyComparer) : this(collection, keyComparer, null) { }

	public Mirror(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction) : this(collection, new EComparer<TKey>(equalFunction)) { }

	public Mirror(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer) : this(collection != null && collection.TryGetLengthEasily(out var length) ? length : 0, keyComparer, valueComparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		AddRange(collection);
	}

	public Mirror(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(collection, new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public Mirror(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection) : this(keyCollection, valueCollection, (IEqualityComparer<TKey>?)null) { }

	public Mirror(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, IEqualityComparer<TKey>? keyComparer) : this(keyCollection, valueCollection, keyComparer, null) { }

	public Mirror(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction)) { }

	public Mirror(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer) : this(new UnsortedDictionary<TKey, TValue>(keyCollection, valueCollection), keyComparer, valueComparer) { }

	public Mirror(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public Mirror(IEnumerable<(TKey Key, TValue Value)> collection) : this(collection, (IEqualityComparer<TKey>?)null) { }

	public Mirror(IEnumerable<(TKey Key, TValue Value)> collection, IEqualityComparer<TKey>? keyComparer) : this(collection, keyComparer, null) { }

	public Mirror(IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction) : this(collection, new EComparer<TKey>(equalFunction)) { }

	public Mirror(IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(IEnumerable<(TKey Key, TValue Value)> collection, IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer) : this(new UnsortedDictionary<TKey, TValue>(collection), keyComparer, valueComparer) { }

	public Mirror(IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(collection, new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public virtual TKey this[TValue targetValue] { get => GetKey(targetValue); set => SetKey(targetValue, value); }

	public virtual TValue this[TKey key] { get => GetValue(key); set => SetValue(key, value); }

	bool G.ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

	public virtual IEqualityComparer<TKey> KeyComparer => _comparer;

	public virtual KeyCollection Keys => _keys ??= new KeyCollection(this);

	G.ICollection<TKey> G.IDictionary<TKey, TValue>.Keys => Keys;

	IEnumerable<TKey> G.IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	public virtual int Length => _count - _freeCount;

	public virtual IEqualityComparer<TValue> ValueComparer => _comparerM;

	public virtual ValueCollection Values => _values ??= new ValueCollection(this);

	G.ICollection<TValue> G.IDictionary<TKey, TValue>.Values => Values;

	IEnumerable<TValue> G.IReadOnlyDictionary<TKey, TValue>.Values => Values;

	object? System.Collections.IDictionary.this[object key]
	{
		get
		{
			if (IsCompatibleKey(key))
			{
				ref var value = ref FindValue((TKey)key);
				if (!Unsafe.IsNullRef(ref value))
					return value;
			}
			else if (IsCompatibleValue(key))
			{
				ref var value = ref FindKey((TValue)key);
				if (!Unsafe.IsNullRef(ref value))
					return value;
			}
			return null;
		}
		set
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			try
			{
				var tempKey = (TKey)key;
				try
				{
					this[tempKey] = (TValue)value;
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

	bool System.Collections.IDictionary.IsFixedSize => false;

	bool System.Collections.IDictionary.IsReadOnly => false;

	bool System.Collections.ICollection.IsSynchronized => false;

	System.Collections.ICollection System.Collections.IDictionary.Keys => Keys;

	object System.Collections.ICollection.SyncRoot => this;

	System.Collections.ICollection System.Collections.IDictionary.Values => Values;

	public virtual void Add(TKey key, TValue value)
	{
		var modified = TryInsert(key, value, InsertionBehavior.ThrowOnExisting);
		Debug.Assert(modified); // If there was an existing key and the Add failed, an exception will already have been thrown.
	}

	void G.ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair) => Add(keyValuePair.Key, keyValuePair.Value);

	void System.Collections.IDictionary.Add(object key, object? value)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		try
		{
			var tempKey = (TKey)key;
			try
			{
				Add(tempKey, (TValue)value);
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

	private protected virtual void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
	{
		// It is likely that the passed-in enumerable is Mirror<TKey,TValue>. When this is the case,
		// avoid the enumerator allocation and overhead by looping through the entries array directly.
		// We only do this when dictionary is Mirror<TKey,TValue> and not a subclass, to maintain
		// back-compat with subclasses that may have overridden the enumerator behavior.
		if (enumerable.GetType() == typeof(Mirror<TKey, TValue>))
		{
			var source = (Mirror<TKey, TValue>)enumerable;
			if (source.Length == 0)
				return;
			// This is not currently a true .AddRange as it needs to be an initialized dictionary
			// of the correct size, and also an empty dictionary with no current entities (and no argument checks).
			Debug.Assert(source._entries != null);
			Debug.Assert(_entries != null);
			Debug.Assert(_entries.Length >= source.Length);
			Debug.Assert(_count == 0);
			var oldEntries = source._entries;
			if (source._comparer == _comparer && source._comparerM == _comparerM)
			{
				// If comparers are the same, we can copy _entries without rehashing.
				CopyEntries(oldEntries, source._count);
				return;
			}
			// Comparers differ need to rehash all the entries via Add
			var length = source._count;
			for (var i = 0; i < length; i++)
				if (oldEntries[i].next >= -1 && oldEntries[i].nextM >= -1)
					TryAdd(oldEntries[i].key, oldEntries[i].value);
			return;
		}
		// We similarly special-case KVP<>[] and List<KVP<>>, as they're commonly used to seed dictionaries, and
		// we want to avoid the enumerator costs (e.g. allocation) for them as well. Extract a span if possible.
		ReadOnlySpan<KeyValuePair<TKey, TValue>> span;
		if (enumerable is KeyValuePair<TKey, TValue>[] array)
			span = array;
		else if (enumerable.GetType() == typeof(List<KeyValuePair<TKey, TValue>>))
			span = ((List<KeyValuePair<TKey, TValue>>)enumerable).AsSpan();

		else
		{
			// Fallback path for all other enumerables
			foreach (var pair in enumerable)
				TryAdd(pair.Key, pair.Value);

			return;
		}
		// We got a span. Add the elements to the dictionary.
		foreach (var pair in span)
			TryAdd(pair.Key, pair.Value);
	}

	public virtual void Clear()
	{
		var length = _count;
		if (length > 0)
		{
			Debug.Assert(_buckets != null, "_buckets should be non-null");
			Debug.Assert(_bucketsM != null, "_buckets should be non-null");
			Debug.Assert(_entries != null, "_entries should be non-null");
			Array.Clear(_buckets);
			Array.Clear(_bucketsM);
			_count = 0;
			_freeList = -1;
			_freeListM = -1;
			_freeCount = 0;
			Array.Clear(_entries, 0, length);
		}
	}

	public virtual bool Contains(KeyValuePair<TKey, TValue> keyValuePair) => Contains(keyValuePair.Key, keyValuePair.Value);

	public virtual bool Contains(TKey key, TValue value)
	{
		ref var value2 = ref FindValue(key);
		if (!Unsafe.IsNullRef(ref value2) && EqualityComparer<TValue>.Default.Equals(value2, value))
			return true;
		return false;
	}

	bool System.Collections.IDictionary.Contains(object key)
	{
		if (IsCompatibleKey(key))
			return ContainsKey((TKey)key);
		else if (IsCompatibleValue(key))
			return ContainsValue((TValue)key);
		return false;
	}

	public virtual bool ContainsKey(TKey key) => !Unsafe.IsNullRef(ref FindValue(key));

	public virtual bool ContainsValue(TValue value) => !Unsafe.IsNullRef(ref FindKey(value));

	private protected virtual void CopyEntries(Entry[] entries, int length)
	{
		Debug.Assert(_entries != null);
		var newEntries = _entries;
		var newCount = 0;
		for (var i = 0; i < length; i++)
		{
			uint hashCode = entries[i].hashCode, hashCodeM = entries[i].hashCodeM;
			if (entries[i].next >= -1 && entries[i].nextM >= -1)
			{
				ref var entry = ref newEntries[newCount];
				entry = entries[i];
				ref var bucket = ref GetBucket(hashCode);
				entry.next = bucket - 1; // Value in _buckets is 1-based
				bucket = newCount + 1;
				ref var bucketM = ref GetBucketM(hashCodeM);
				entry.nextM = bucketM - 1; // Value in _buckets is 1-based
				bucketM = newCount + 1;
				newCount++;
			}
		}
		_count = newCount;
		_freeCount = 0;
	}

	private protected virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		if ((uint)index > (uint)array.Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (array.Length - index < Length)
			throw new ArgumentException(null, nameof(index));
		Debug.Assert(_entries != null);
		var length = _count;
		Debug.Assert(_entries != null);
		var entries = _entries;
		for (var i = 0; i < length; i++)
			if (entries[i].next >= -1 && entries[i].nextM >= -1)
				array[index++] = new KeyValuePair<TKey, TValue>(entries[i].key, entries[i].value);
	}

	void G.ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index) => CopyTo(array, index);

	void System.Collections.ICollection.CopyTo(Array array, int index)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		if (array.Rank != 1)
			throw new RankException();
		if (array.GetLowerBound(0) != 0)
			throw new ArgumentException(null, nameof(array));
		if ((uint)index > (uint)array.Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (array.Length - index < Length)
			throw new ArgumentException(null);
		Debug.Assert(_entries != null);
		if (array is KeyValuePair<TKey, TValue>[] pairs)
			CopyTo(pairs, index);
		else if (array is DictionaryEntry[] dictEntryArray)
		{
			var entries = _entries;
			for (var i = 0; i < _count; i++)
				if (entries[i].next >= -1 && entries[i].nextM >= -1)
					dictEntryArray[index++] = new DictionaryEntry(entries[i].key, entries[i].value);
		}
		else
		{
			if (array is not object[] objects)
				throw new ArrayTypeMismatchException();
			var length = _count;
			var entries = _entries;
			for (var i = 0; i < length; i++)
				if (entries[i].next >= -1 && entries[i].nextM >= -1)
					objects[index++] = new KeyValuePair<TKey, TValue>(entries[i].key, entries[i].value);
		}
	}

	/// <summary>
	/// Ensures that the dictionary can hold up to 'capacity' entries without any further expansion of its backing storage
	/// </summary>
	private protected virtual int EnsureCapacity(int capacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		var currentCapacity = _entries == null ? 0 : _entries.Length;
		if (currentCapacity >= capacity)
			return currentCapacity;
		_version++;
		if (_buckets == null || _bucketsM == null)
			return Initialize(capacity);
		var newSize = HashHelpers.GetPrime(capacity);
		Resize(newSize, forceNewHashCodes: false);
		return newSize;
	}

	private static int Find(Entry[] entries, int otherCurrent, int bucket, bool mirrored = false)
	{
		uint collisionCount = 0;
		var last = -1;
		var current = bucket - 1; // Value in buckets is 1-based
		while (current >= 0)
		{
			if (current == otherCurrent)
				break;
			last = current;
			current = mirrored ? entries[current].next : entries[current].nextM;
			collisionCount++;
			if (collisionCount > (uint)entries.Length)
				throw new InvalidOperationException();
		}
		return last;
	}

	internal ref TKey FindKey(TValue value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		ref var entry = ref Unsafe.NullRef<Entry>();
		if (_buckets == null || _bucketsM == null)
			goto ReturnNotFound;
		Debug.Assert(_entries != null, "expected entries to be != null");
		var comparerM = _comparerM;
		if (typeof(TValue).IsValueType && comparerM == null) // comparer can only be null for key types; enable JIT to eliminate entire if block for ref types
		{
			var hashCodeM = (uint)value.GetHashCode();
			var currentM = GetBucketM(hashCodeM);
			var entries = _entries;
			uint collisionCountM = 0;
			// KeyType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
			currentM--; // Key in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
			do
			{
				// Should be a while loop https://github.com/dotnet/runtime/issues/9422
				// Test in if to drop range check for following array access
				if ((uint)currentM >= (uint)entries.Length)
					goto ReturnNotFound;
				entry = ref entries[currentM];
				if (entry.hashCodeM == hashCodeM && EqualityComparer<TValue>.Default.Equals(entry.value, value))
					goto ReturnFound;
				currentM = entry.nextM;
				collisionCountM++;
			} while (collisionCountM <= (uint)entries.Length);
			// The chain of entries forms a loop; which means a concurrent update has happened.
			// Break out of the loop and throw, rather than looping forever.
			goto ConcurrentOperation;
		}
		else
		{
			Debug.Assert(comparerM != null);
			var hashCodeM = (uint)comparerM.GetHashCode(value);
			var currentM = GetBucketM(hashCodeM);
			var entries = _entries;
			uint collisionCountM = 0;
			currentM--; // Key in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
			do
			{
				// Should be a while loop https://github.com/dotnet/runtime/issues/9422
				// Test in if to drop range check for following array access
				if ((uint)currentM >= (uint)entries.Length)
					goto ReturnNotFound;
				entry = ref entries[currentM];
				if (entry.hashCodeM == hashCodeM && comparerM.Equals(entry.value, value))
					goto ReturnFound;
				currentM = entry.nextM;
				collisionCountM++;
			} while (collisionCountM <= (uint)entries.Length);
			// The chain of entries forms a loop; which means a concurrent update has happened.
			// Break out of the loop and throw, rather than looping forever.
			goto ConcurrentOperation;
		}
	ConcurrentOperation:
		throw new InvalidOperationException();
	ReturnFound:
		ref var key = ref entry.key;
	Return:
		return ref key!;
	ReturnNotFound:
		key = ref Unsafe.NullRef<TKey>()!;
		goto Return;
	}

	internal ref TValue FindValue(TKey key)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		ref var entry = ref Unsafe.NullRef<Entry>();
		if (_buckets == null || _bucketsM == null)
			goto ReturnNotFound;
		Debug.Assert(_entries != null, "expected entries to be != null");
		var comparer = _comparer;
		if (typeof(TKey).IsValueType && comparer == null) // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
		{
			var hashCode = (uint)key.GetHashCode();
			var current = GetBucket(hashCode);
			var entries = _entries;
			uint collisionCount = 0;
			// ValueType: Devirtualize with EqualityComparer<TKey>.Default intrinsic
			current--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
			do
			{
				// Should be a while loop https://github.com/dotnet/runtime/issues/9422
				// Test in if to drop range check for following array access
				if ((uint)current >= (uint)entries.Length)
					goto ReturnNotFound;
				entry = ref entries[current];
				if (entry.hashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entry.key, key))
					goto ReturnFound;
				current = entry.next;
				collisionCount++;
			} while (collisionCount <= (uint)entries.Length);
			// The chain of entries forms a loop; which means a concurrent update has happened.
			// Break out of the loop and throw, rather than looping forever.
			goto ConcurrentOperation;
		}
		else
		{
			Debug.Assert(comparer != null);
			var hashCode = (uint)comparer.GetHashCode(key);
			var current = GetBucket(hashCode);
			var entries = _entries;
			uint collisionCount = 0;
			current--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
			do
			{
				// Should be a while loop https://github.com/dotnet/runtime/issues/9422
				// Test in if to drop range check for following array access
				if ((uint)current >= (uint)entries.Length)
					goto ReturnNotFound;
				entry = ref entries[current];
				if (entry.hashCode == hashCode && comparer.Equals(entry.key, key))
					goto ReturnFound;
				current = entry.next;
				collisionCount++;
			} while (collisionCount <= (uint)entries.Length);
			// The chain of entries forms a loop; which means a concurrent update has happened.
			// Break out of the loop and throw, rather than looping forever.
			goto ConcurrentOperation;
		}
	ConcurrentOperation:
		throw new InvalidOperationException();
	ReturnFound:
		ref var value = ref entry.value;
	Return:
		return ref value!;
	ReturnNotFound:
		value = ref Unsafe.NullRef<TValue>()!;
		goto Return;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private protected virtual ref int GetBucket(uint hashCode)
	{
		Debug.Assert(_buckets != null);
		var buckets = _buckets;
		return ref buckets[hashCode % buckets.Length];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private protected virtual ref int GetBucketM(uint hashCode)
	{
		Debug.Assert(_bucketsM != null);
		var buckets = _bucketsM;
		return ref buckets[hashCode % buckets.Length];
	}

	public virtual Enumerator GetEnumerator() => new(this, Enumerator.KeyValuePair);

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();

	IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() => new Enumerator(this, Enumerator.DictEntry);

	public virtual TKey GetKey(TValue value)
	{
		ref var key = ref FindKey(value);
		if (!Unsafe.IsNullRef(ref key))
			return key;
		throw new ValueNotFoundException();
	}

	public virtual TValue GetValue(TKey key)
	{
		ref var value = ref FindValue(key);
		if (!Unsafe.IsNullRef(ref value))
			return value;
		throw new KeyNotFoundException();
	}

	private protected virtual int Initialize(int capacity)
	{
		var size = HashHelpers.GetPrime(capacity);
		var buckets = new int[size];
		var bucketsM = new int[size];
		var entries = new Entry[size];
		// Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
		_freeList = -1;
		_freeListM = -1;
		_buckets = buckets;
		_bucketsM = bucketsM;
		_entries = entries;
		return size;
	}

	private protected static bool IsCompatibleKey(object key)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		return key is TKey;
	}

	private protected static bool IsCompatibleValue(object value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		return value is TValue;
	}

	private static void ProcessLast(Entry[] entries, Entry entry, ref int bucket, int last, bool mirrored = false)
	{
		if (last < 0)
			bucket = (mirrored ? entry.nextM : entry.next) + 1; // Value in buckets is 1-based
		else if (mirrored)
			entries[last].nextM = entry.nextM;
		else
			entries[last].next = entry.next;
	}

	bool G.IDictionary<TKey, TValue>.Remove(TKey key) => RemoveKey(key);

	void System.Collections.IDictionary.Remove(object key)
	{
		if (IsCompatibleKey(key))
			RemoveKey((TKey)key);
		else if (IsCompatibleValue(key))
			RemoveValue((TValue)key);
	}

	public virtual bool RemoveKey(TKey key)
	{
		// The overload Remove(TKey key, out TValue value) is a copy of this method with one additional
		// statement to copy the value for entry being removed into the output parameter.
		// Code has been intentionally duplicated for performance reasons.
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (_buckets == null || _bucketsM == null)
			return false;
		Debug.Assert(_entries != null, "entries should be non-null");
		uint collisionCount = 0;
		var comparer = _comparer;
		Debug.Assert(typeof(TKey).IsValueType || comparer != null);
		var hashCode = (uint)(typeof(TKey).IsValueType && comparer == null ? key.GetHashCode() : comparer.GetHashCode(key));
		ref var bucket = ref GetBucket(hashCode);
		var entries = _entries;
		var last = -1;
		var current = bucket - 1; // Value in buckets is 1-based
		while (current >= 0)
		{
			ref var entry = ref entries[current];
			if (entry.hashCode == hashCode && (typeof(TKey).IsValueType && comparer == null ? EqualityComparer<TKey>.Default.Equals(entry.key, key) : comparer!.Equals(entry.key, key)))
			{
				ref var bucketM = ref GetBucketM(entry.hashCodeM);
				var lastM = Find(entries, current, bucketM);
				ProcessLast(entries, entry, ref bucket, last);
				ProcessLast(entries, entry, ref bucketM, lastM, true);
				Debug.Assert(StartOfFreeList - _freeList < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
				entry.next = StartOfFreeList - _freeList;
				Debug.Assert(StartOfFreeList - _freeListM < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
				entry.nextM = StartOfFreeList - _freeListM;
				if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
					entry.key = default!;
				if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
					entry.value = default!;
				_freeListM = _freeList = current;
				_freeCount++;
				Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
				Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
				return true;
			}
			last = current;
			current = entry.next;
			collisionCount++;
			if (collisionCount > (uint)entries.Length)
				throw new InvalidOperationException();
		}
		return false;
	}

	public virtual bool RemoveKey(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		// This overload is a copy of the overload Remove(TKey key) with one additional
		// statement to copy the value for entry being removed into the output parameter.
		// Code has been intentionally duplicated for performance reasons.
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (_buckets == null || _bucketsM == null)
		{
			value = default;
			return false;
		}
		Debug.Assert(_entries != null, "entries should be non-null");
		uint collisionCount = 0;
		var comparer = _comparer;
		Debug.Assert(typeof(TKey).IsValueType || comparer != null);
		var hashCode = (uint)(typeof(TKey).IsValueType && comparer == null ? key.GetHashCode() : comparer.GetHashCode(key));
		ref var bucket = ref GetBucket(hashCode);
		var entries = _entries;
		var last = -1;
		var current = bucket - 1; // Value in buckets is 1-based
		while (current >= 0)
		{
			ref var entry = ref entries[current];
			if (entry.hashCode == hashCode && (typeof(TKey).IsValueType || comparer == null ? EqualityComparer<TKey>.Default.Equals(entry.key, key) : comparer.Equals(entry.key, key)))
			{
				ref var bucketM = ref GetBucketM(entry.hashCodeM);
				var lastM = Find(entries, current, bucketM);
				ProcessLast(entries, entry, ref bucket, last);
				ProcessLast(entries, entry, ref bucketM, lastM, true);
				value = entry.value;
				Debug.Assert(StartOfFreeList - _freeList < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
				entry.next = StartOfFreeList - _freeList;
				Debug.Assert(StartOfFreeList - _freeListM < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
				entry.nextM = StartOfFreeList - _freeListM;
				if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
					entry.key = default!;
				if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
					entry.value = default!;
				_freeListM = _freeList = current;
				_freeCount++;
				Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
				Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
				return true;
			}
			last = current;
			current = entry.next;
			collisionCount++;
			if (collisionCount > (uint)entries.Length)
				throw new InvalidOperationException();
		}
		value = default;
		return false;
	}

	public virtual bool RemoveValue(TValue value)
	{
		// The overload Remove(TValue value, out TKey key) is a copy of this method with one additional
		// statement to copy the key for entry being removed into the output parameter.
		// Code has been intentionally duplicated for performance reasons.
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_buckets == null || _bucketsM == null)
			return false;
		Debug.Assert(_entries != null, "entries should be non-null");
		uint collisionCountM = 0;
		var comparerM = _comparerM;
		Debug.Assert(typeof(TValue).IsValueType || comparerM != null);
		var hashCodeM = (uint)(typeof(TValue).IsValueType && comparerM == null ? value.GetHashCode() : comparerM.GetHashCode(value));
		ref var bucketM = ref GetBucketM(hashCodeM);
		var entries = _entries;
		var lastM = -1;
		var currentM = bucketM - 1; // Key in buckets is 1-based
		while (currentM >= 0)
		{
			ref var entry = ref entries[currentM];
			if (entry.hashCodeM == hashCodeM && (typeof(TValue).IsValueType && comparerM == null ? EqualityComparer<TValue>.Default.Equals(entry.value, value) : comparerM!.Equals(entry.value, value)))
			{
				ref var bucket = ref GetBucket(entry.hashCode);
				var last = Find(entries, currentM, bucket, true);
				ProcessLast(entries, entry, ref bucket, last);
				ProcessLast(entries, entry, ref bucketM, lastM, true);
				Debug.Assert(StartOfFreeList - _freeList < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
				entry.next = StartOfFreeList - _freeList;
				Debug.Assert(StartOfFreeList - _freeListM < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
				entry.nextM = StartOfFreeList - _freeListM;
				if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
					entry.value = default!;
				if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
					entry.key = default!;
				_freeListM = _freeList = currentM;
				_freeCount++;
				Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
				Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
				return true;
			}
			lastM = currentM;
			currentM = entry.nextM;
			collisionCountM++;
			if (collisionCountM > (uint)entries.Length)
				throw new InvalidOperationException();
		}
		return false;
	}

	public virtual bool RemoveValue(TValue value, [MaybeNullWhen(false)] out TKey key)
	{
		// This overload is a copy of the overload Remove(TValue value) with one additional
		// statement to copy the key for entry being removed into the output parameter.
		// Code has been intentionally duplicated for performance reasons.
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_buckets == null || _bucketsM == null)
		{
			key = default;
			return false;
		}
		Debug.Assert(_entries != null, "entries should be non-null");
		uint collisionCountM = 0;
		var comparerM = _comparerM;
		Debug.Assert(typeof(TValue).IsValueType || comparerM != null);
		var hashCodeM = (uint)(typeof(TValue).IsValueType && comparerM == null ? value.GetHashCode() : comparerM.GetHashCode(value));
		ref var bucketM = ref GetBucketM(hashCodeM);
		var entries = _entries;
		var lastM = -1;
		var currentM = bucketM - 1; // Key in buckets is 1-based
		while (currentM >= 0)
		{
			ref var entry = ref entries[currentM];
			if (entry.hashCodeM == hashCodeM && (typeof(TValue).IsValueType && comparerM == null ? EqualityComparer<TValue>.Default.Equals(entry.value, value) : comparerM!.Equals(entry.value, value)))
			{
				ref var bucket = ref GetBucket(entry.hashCode);
				var last = Find(entries, currentM, bucket, true);
				ProcessLast(entries, entry, ref bucket, last);
				ProcessLast(entries, entry, ref bucketM, lastM, true);
				key = entry.key;
				Debug.Assert(StartOfFreeList - _freeList < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
				entry.next = StartOfFreeList - _freeList;
				Debug.Assert(StartOfFreeList - _freeListM < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
				entry.nextM = StartOfFreeList - _freeListM;
				if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
					entry.value = default!;
				if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
					entry.key = default!;
				_freeListM = _freeList = currentM;
				_freeCount++;
				Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
				Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
				return true;
			}
			lastM = currentM;
			currentM = entry.nextM;
			collisionCountM++;
			if (collisionCountM > (uint)entries.Length)
				throw new InvalidOperationException();
		}
		key = default;
		return false;
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.RemoveValue(KeyValuePair<TKey, TValue> keyValuePair)
	{
		ref var value = ref FindValue(keyValuePair.Key);
		if (!Unsafe.IsNullRef(ref value) && EqualityComparer<TValue>.Default.Equals(value, keyValuePair.Value))
		{
			RemoveKey(keyValuePair.Key);
			return true;
		}
		return false;
	}

	private protected virtual void Resize() => Resize(HashHelpers.ExpandPrime(_count), false);

	private protected virtual void Resize(int newSize, bool forceNewHashCodes)
	{
		// Value types never rehash
		Debug.Assert(!forceNewHashCodes || !typeof(TKey).IsValueType);
		Debug.Assert(_entries != null, "_entries should be non-null");
		Debug.Assert(newSize >= _entries.Length);
		var entries = new Entry[newSize];
		var length = _count;
		Array.Copy(_entries, entries, length);
		if (!typeof(TKey).IsValueType && forceNewHashCodes)
		{
			var comparer = _comparer = EqualityComparer<TKey>.Default;
			var comparerM = _comparerM = EqualityComparer<TValue>.Default;
			for (var i = 0; i < length; i++)
				if (entries[i].next >= -1 && entries[i].nextM >= -1)
				{
					entries[i].hashCode = (uint)comparer.GetHashCode(entries[i].key);
					entries[i].hashCodeM = (uint)comparerM.GetHashCode(entries[i].value);
				}
		}
		// Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
		_buckets = new int[newSize];
		_bucketsM = new int[newSize];
		for (var i = 0; i < length; i++)
			if (entries[i].next >= -1 && entries[i].nextM >= -1)
			{
				ref var bucket = ref GetBucket(entries[i].hashCode);
				entries[i].next = bucket - 1;
				bucket = i + 1;
				ref var bucketM = ref GetBucketM(entries[i].hashCodeM);
				entries[i].nextM = bucketM - 1;
				bucketM = i + 1;
			}
		Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
		Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
		_entries = entries;
	}

	public virtual void SetKey(TValue value, TKey key)
	{
		var modified = TryInsert(key, value, InsertionBehavior.OverwriteExisting);
		Debug.Assert(modified);
	}

	public virtual void SetValue(TKey key, TValue value)
	{
		var modified = TryInsert(key, value, InsertionBehavior.OverwriteExisting);
		Debug.Assert(modified);
	}

	/// <summary>
	/// Sets the capacity of this dictionary to what it would be if it had been originally initialized with all its entries
	/// </summary>
	/// <remarks>
	/// This method can be used to minimize the memory overhead
	/// once it is known that no new elements will be added.
	///
	/// To allocate minimum size storage array, execute the following statements:
	///
	/// dictionary.Clear();
	/// dictionary.TrimExcess();
	/// </remarks>
	public virtual void TrimExcess() => TrimExcess(Length);

	/// <summary>
	/// Sets the capacity of this dictionary to hold up 'capacity' entries without any further expansion of its backing storage
	/// </summary>
	/// <remarks>
	/// This method can be used to minimize the memory overhead
	/// once it is known that no new elements will be added.
	/// </remarks>
	public virtual void TrimExcess(int capacity)
	{
		if (capacity < Length)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		var newSize = HashHelpers.GetPrime(capacity);
		var oldEntries = _entries;
		var currentCapacity = oldEntries == null ? 0 : oldEntries.Length;
		if (newSize >= currentCapacity)
			return;
		var oldCount = _count;
		_version++;
		Initialize(newSize);
		Debug.Assert(oldEntries != null);
		CopyEntries(oldEntries, oldCount);
	}

	public virtual bool TryAdd(TKey key, TValue value) => TryInsert(key, value, InsertionBehavior.None);

	public virtual bool TryGetKey(TValue value, [MaybeNullWhen(false)] out TKey key)
	{
		ref var keyRef = ref FindKey(value);
		if (!Unsafe.IsNullRef(ref keyRef))
		{
			key = keyRef;
			return true;
		}
		key = default;
		return false;
	}

	public virtual bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		ref var valRef = ref FindValue(key);
		if (!Unsafe.IsNullRef(ref valRef))
		{
			value = valRef;
			return true;
		}
		value = default;
		return false;
	}

	private protected virtual bool TryInsert(TKey key, TValue value, InsertionBehavior behavior)
	{
		// NOTE: this method is mirrored in CollectionsMarshal.GetValueRefOrAddDefault below.
		// If you make any changes here, make sure to keep that version in sync as well.
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_buckets == null || _bucketsM == null)
			Initialize(0);
		Debug.Assert(_buckets != null);
		Debug.Assert(_bucketsM != null);
		var entries = _entries;
		Debug.Assert(entries != null, "expected entries to be non-null");
		var comparer = _comparer;
		var comparerM = _comparerM;
		var hashCode = (uint)((typeof(TKey).IsValueType && comparer == null) ? key.GetHashCode() : comparer.GetHashCode(key));
		uint collisionCount = 0;
		ref var bucket = ref GetBucket(hashCode);
		var current = bucket - 1; // Value in _buckets is 1-based
		if (typeof(TKey).IsValueType && comparer == null) // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
			while (true)
			{
				var a = TryInsertInternal(key, value, behavior, entries, EqualityComparer<TKey>.Default, comparerM, hashCode, ref collisionCount, ref current);
				Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
				Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
				if (a == 0)
					break;
				else if (a == 1)
					return true;
				else if (a == 2)
					return false;
			}
		else
		{
			Debug.Assert(comparer != null);
			while (true)
			{
				// Should be a while loop https://github.com/dotnet/runtime/issues/9422
				// Test uint in if rather than loop condition to drop range check for following array access
				var a = TryInsertInternal(key, value, behavior, entries, comparer, comparerM, hashCode, ref collisionCount, ref current);
				Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
				Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
				if (a == 0)
					break;
				else if (a == 1)
					return true;
				else if (a == 2)
					return false;
			}
		}
		var hashCodeM = (uint)((typeof(TValue).IsValueType && comparer == null) ? value.GetHashCode() : comparerM.GetHashCode(value));
		uint collisionCountM = 0;
		ref var bucketM = ref GetBucketM(hashCodeM);
		var currentM = bucketM - 1; // Value in _buckets is 1-based
		if (typeof(TValue).IsValueType && comparerM == null) // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
			while (true)
			{
				var a = TryInsertInternal(key, value, behavior, entries, comparer ?? EqualityComparer<TKey>.Default, EqualityComparer<TValue>.Default, hashCodeM, ref collisionCountM, ref currentM, true);
				Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
				Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
				if (a == 0)
					break;
				else if (a == 1)
					return true;
				else if (a == 2)
					return false;
			}
		else
		{
			Debug.Assert(comparer != null);
			while (true)
			{
				// Should be a while loop https://github.com/dotnet/runtime/issues/9422
				// Test uint in if rather than loop condition to drop range check for following array access
				var a = TryInsertInternal(key, value, behavior, entries, comparer, comparerM, hashCodeM, ref collisionCountM, ref currentM, true);
				Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
				Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
				if (a == 0)
					break;
				else if (a == 1)
					return true;
				else if (a == 2)
					return false;
			}
		}
		int index, indexM;
		if (_freeCount > 0)
		{
			index = _freeList;
			indexM = _freeListM;
			Debug.Assert(StartOfFreeList - entries[_freeList].next >= -1, "shouldn't overflow because `next` cannot underflow");
			_freeList = StartOfFreeList - entries[_freeList].next;
			Debug.Assert(StartOfFreeList - entries[_freeListM].nextM >= -1, "shouldn't overflow because `next` cannot underflow");
			_freeListM = StartOfFreeList - entries[_freeListM].nextM;
			_freeCount--;
		}
		else
		{
			var length = _count;
			if (length == entries.Length)
			{
				Resize();
				bucket = ref GetBucket(hashCode);
				bucketM = ref GetBucketM(hashCodeM);
			}
			indexM = index = length;
			_count = length + 1;
			entries = _entries;
		}
		ref var entry = ref entries![index];
		entry.hashCode = hashCode;
		entry.hashCodeM = hashCodeM;
		entry.next = bucket - 1; // Value in _buckets is 1-based
		entry.nextM = bucketM - 1; // Value in _buckets is 1-based
		entry.key = key;
		entry.value = value;
		bucket = index + 1; // Value in _buckets is 1-based
		bucketM = indexM + 1; // Value in _buckets is 1-based
		_version++;
		Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
		Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
		// Value types never rehash
		if (!typeof(TKey).IsValueType && collisionCount > HashHelpers.HashCollisionThreshold || !typeof(TValue).IsValueType && collisionCountM > HashHelpers.HashCollisionThreshold)
			Resize(entries.Length, true);
		return true;
	}

	private int TryInsertInternal(TKey key, TValue value, InsertionBehavior behavior, Entry[] entries, IEqualityComparer<TKey> comparer, IEqualityComparer<TValue> comparerM, uint hashCode, ref uint collisionCount, ref int current, bool mirrored = false)
	{
		if ((uint)current >= (uint)entries.Length)
			return 0;
		if ((mirrored ? entries[current].hashCodeM : entries[current].hashCode) == hashCode && (mirrored ? comparerM.Equals(entries[current].value, value) : comparer.Equals(entries[current].key, key)))
		{
			if (mirrored ? comparer.Equals(entries[current].key, key) : comparerM.Equals(entries[current].value, value))
				return behavior switch
				{
					InsertionBehavior.None => 2,
					InsertionBehavior.OverwriteExisting => 1,
					InsertionBehavior.ThrowOnExisting => throw new ArgumentException(null, mirrored ? nameof(value) : nameof(key)),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна."),
				};
			if (behavior == InsertionBehavior.OverwriteExisting)
			{
				if (mirrored)
				{
					RemoveKey(key);
					ref var entry = ref entries[current];
					ref var bucket = ref GetBucket(entry.hashCode);
					var last = Find(entries, current, bucket, true);
					ProcessLast(entries, entry, ref bucket, last);
					entry.hashCode = (uint)((typeof(TKey).IsValueType && comparer == null) ? key.GetHashCode() : comparer.GetHashCode(key));
					bucket = ref GetBucket(entry.hashCode);
					entry.next = bucket - 1; // Value in _buckets is 1-based
					entries[current].key = key;
					bucket = current + 1; // Value in _buckets is 1-based
				}
				else
				{
					RemoveValue(value);
					ref var entry = ref entries[current];
					ref var bucketM = ref GetBucketM(entry.hashCodeM);
					var lastM = Find(entries, current, bucketM);
					ProcessLast(entries, entry, ref bucketM, lastM, true);
					entry.hashCodeM = (uint)((typeof(TValue).IsValueType && comparer == null) ? value.GetHashCode() : comparerM.GetHashCode(value));
					bucketM = ref GetBucketM(entry.hashCodeM);
					entry.nextM = bucketM - 1; // Value in _buckets is 1-based
					entries[current].value = value;
					bucketM = current + 1; // Value in _buckets is 1-based
				}
				return 1;
			}
			return behavior == InsertionBehavior.ThrowOnExisting ? throw new ArgumentException(null, mirrored ? nameof(value) : nameof(key)) : 2;
		}
		current = mirrored ? entries[current].nextM : entries[current].next;
		collisionCount++;
		if (collisionCount > (uint)entries.Length)
			throw new InvalidOperationException();
		return 3;
	}

	public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
	{
		private readonly Mirror<TKey, TValue> _dictionary;
		private readonly int _version;
		private int _index;
		private readonly int _getEnumeratorRetType;  // What should Enumerator.Current return?

		internal const int DictEntry = 1;
		internal const int KeyValuePair = 2;

		internal Enumerator(Mirror<TKey, TValue> dictionary, int getEnumeratorRetType)
		{
			_dictionary = dictionary;
			_version = dictionary._version;
			_index = 0;
			_getEnumeratorRetType = getEnumeratorRetType;
			Current = default;
		}

		public KeyValuePair<TKey, TValue> Current { get; private set; }

		readonly object? IEnumerator.Current
		{
			get
			{
				if (_index == 0 || _index == _dictionary._count + 1)
					throw new InvalidOperationException();
				if (_getEnumeratorRetType == DictEntry)
					return new DictionaryEntry(Current.Key, Current.Value);
				return new KeyValuePair<TKey, TValue>(Current.Key, Current.Value);
			}
		}

		readonly DictionaryEntry IDictionaryEnumerator.Entry
		{
			get
			{
				if (_index == 0 || _index == _dictionary._count + 1)
					throw new InvalidOperationException();
				return new DictionaryEntry(Current.Key, Current.Value);
			}
		}

		readonly object IDictionaryEnumerator.Key
		{
			get
			{
				if (_index == 0 || _index == _dictionary._count + 1)
					throw new InvalidOperationException();
				return Current.Key;
			}
		}

		readonly object? IDictionaryEnumerator.Value
		{
			get
			{
				if (_index == 0 || _index == _dictionary._count + 1)
					throw new InvalidOperationException();
				return Current.Value;
			}
		}

		public readonly void Dispose() { }

		public bool MoveNext()
		{
			if (_version != _dictionary._version)
				throw new InvalidOperationException();
			// Use unsigned comparison since we set index to dictionary.length+1 when the enumeration ends.
			// dictionary.length+1 could be negative if dictionary.length is int.MaxValue
			while ((uint)_index < (uint)_dictionary._count)
			{
				ref var entry = ref _dictionary._entries![_index++];
				if (entry.next >= -1 && entry.nextM >= -1)
				{
					Current = new KeyValuePair<TKey, TValue>(entry.key, entry.value);
					return true;
				}
			}
			_index = _dictionary._count + 1;
			Current = default;
			return false;
		}

		void IEnumerator.Reset()
		{
			if (_version != _dictionary._version)
				throw new InvalidOperationException();
			_index = 0;
			Current = default;
		}
	}

	[DebuggerDisplay("Length = {Length}"), Serializable]
	public sealed class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyCollection<TKey>
	{
		private readonly Mirror<TKey, TValue> _dictionary;

		public KeyCollection(Mirror<TKey, TValue> dictionary) => _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

		bool G.ICollection<TKey>.IsReadOnly => true;

		bool System.Collections.ICollection.IsSynchronized => false;

		public int Length => _dictionary.Length;

		object System.Collections.ICollection.SyncRoot => ((System.Collections.ICollection)_dictionary).SyncRoot;

		void G.ICollection<TKey>.Add(TKey item) => throw new NotSupportedException();

		void G.ICollection<TKey>.Clear() => throw new NotSupportedException();

		public bool Contains(TKey item) => _dictionary.ContainsKey(item);

		public void CopyTo(TKey[] array, int index)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (array.Length - index < _dictionary.Length)
				throw new ArgumentException(null);
			var length = _dictionary._count;
			var entries = _dictionary._entries;
			for (var i = 0; i < length; i++)
				if (entries![i].next >= -1 && entries![i].nextM >= -1)
					array[index++] = entries[i].key;
		}

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			if (array.Rank != 1)
				throw new RankException();
			if (array.GetLowerBound(0) != 0)
				throw new ArgumentException(null, nameof(array));
			if ((uint)index > (uint)array.Length)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (array.Length - index < _dictionary.Length)
				throw new ArgumentException(null);
			if (array is TKey[] keys)
				CopyTo(keys, index);
			else
			{
				if (array is not object[] objects)
					throw new ArrayTypeMismatchException();
				var length = _dictionary._count;
				var entries = _dictionary._entries;
				for (var i = 0; i < length; i++)
					if (entries![i].next >= -1 && entries![i].nextM >= -1) objects[index++] = entries[i].key;
			}
		}

		public Enumerator GetEnumerator() => new(_dictionary);

		IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TKey>)this).GetEnumerator();

		bool ICollection<TKey>.RemoveValue(TKey item) => throw new NotSupportedException();

		public struct Enumerator : IEnumerator<TKey>, IEnumerator
		{
			private readonly Mirror<TKey, TValue> _dictionary;
			private int _index;
			private readonly int _version;

			internal Enumerator(Mirror<TKey, TValue> dictionary)
			{
				_dictionary = dictionary;
				_version = dictionary._version;
				_index = 0;
				Current = default!;
			}

			public TKey Current { get; private set; }

			readonly object? IEnumerator.Current
			{
				get
				{
					if (_index == 0 || _index == _dictionary._count + 1)
						throw new InvalidOperationException();
					return Current;
				}
			}

			public readonly void Dispose() { }

			public bool MoveNext()
			{
				if (_version != _dictionary._version)
					throw new InvalidOperationException();
				while ((uint)_index < (uint)_dictionary._count)
				{
					ref var entry = ref _dictionary._entries![_index++];
					if (entry.next >= -1 && entry.nextM >= -1)
					{
						Current = entry.key;
						return true;
					}
				}
				_index = _dictionary._count + 1;
				Current = default!;
				return false;
			}
			void IEnumerator.Reset()
			{
				if (_version != _dictionary._version)
					throw new InvalidOperationException();
				_index = 0;
				Current = default!;
			}
		}
	}

	[DebuggerDisplay("Length = {Length}"), Serializable]
	public sealed class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue>
	{
		private readonly Mirror<TKey, TValue> _dictionary;

		public ValueCollection(Mirror<TKey, TValue> dictionary) => _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

		bool G.ICollection<TValue>.IsReadOnly => true;

		bool System.Collections.ICollection.IsSynchronized => false;

		public int Length => _dictionary.Length;

		object System.Collections.ICollection.SyncRoot => ((System.Collections.ICollection)_dictionary).SyncRoot;

		void G.ICollection<TValue>.Add(TValue item) => throw new NotSupportedException();

		void G.ICollection<TValue>.Clear() => throw new NotSupportedException();

		bool G.ICollection<TValue>.Contains(TValue item) => _dictionary.ContainsValue(item);

		public void CopyTo(TValue[] array, int index)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			if ((uint)index > array.Length)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (array.Length - index < _dictionary.Length)
				throw new ArgumentException(null);
			var length = _dictionary._count;
			var entries = _dictionary._entries;
			for (var i = 0; i < length; i++)
				if (entries![i].next >= -1 && entries![i].nextM >= -1)
					array[index++] = entries[i].value;
		}

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			if (array.Rank != 1)
				throw new RankException();
			if (array.GetLowerBound(0) != 0)
				throw new ArgumentException(null, nameof(array));
			if ((uint)index > (uint)array.Length)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (array.Length - index < _dictionary.Length)
				throw new ArgumentException(null);
			if (array is TValue[] values)
				CopyTo(values, index);
			else
			{
				if (array is not object[] objects)
					throw new ArrayTypeMismatchException();
				var length = _dictionary._count;
				var entries = _dictionary._entries;
				for (var i = 0; i < length; i++)
					if (entries![i].next >= -1 && entries![i].nextM >= -1) objects[index++] = entries[i].value;
			}
		}

		public Enumerator GetEnumerator() => new(_dictionary);

		IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TValue>)this).GetEnumerator();

		bool ICollection<TValue>.RemoveValue(TValue item) => throw new NotSupportedException();

		public struct Enumerator : IEnumerator<TValue>, IEnumerator
		{
			private readonly Mirror<TKey, TValue> _dictionary;
			private int _index;
			private readonly int _version;

			internal Enumerator(Mirror<TKey, TValue> dictionary)
			{
				_dictionary = dictionary;
				_version = dictionary._version;
				_index = 0;
				Current = default!;
			}

			public TValue Current { get; private set; }

			readonly object? IEnumerator.Current
			{
				get
				{
					if (_index == 0 || _index == _dictionary._count + 1)
						throw new InvalidOperationException();
					return Current;
				}
			}

			public readonly void Dispose() { }

			public bool MoveNext()
			{
				if (_version != _dictionary._version)
					throw new InvalidOperationException();
				while ((uint)_index < (uint)_dictionary._count)
				{
					ref var entry = ref _dictionary._entries![_index++];
					if (entry.next >= -1 && entry.nextM >= -1)
					{
						Current = entry.value;
						return true;
					}
				}
				_index = _dictionary._count + 1;
				Current = default!;
				return false;
			}
			void IEnumerator.Reset()
			{
				if (_version != _dictionary._version)
					throw new InvalidOperationException();
				_index = 0;
				Current = default!;
			}
		}
	}
}
