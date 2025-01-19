﻿
namespace Corlib.NStar;

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
	private protected readonly List<TKey> keys;
	private protected readonly List<TValue> values;
	private protected readonly IComparer<TKey> comparer;
	private protected KeyList? keyList;
	private protected ValueList? valueList;
	[NonSerialized]
	private protected object? _syncRoot;

	private protected const int _defaultCapacity = 32;
	private protected const int _sortingThreshold = 65;

	public SortedDictionary()
	{
		keys = [];
		values = [];
		comparer = G.Comparer<TKey>.Default;
	}

	public SortedDictionary(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
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
		if (keys.Length > _sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, int> compareFunction) : this(dictionary, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection) : this(keyCollection, valueCollection, G.Comparer<TKey>.Default) { }

	public SortedDictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, IComparer<TKey>? comparer) : this(keyCollection != null && valueCollection != null ? List<TKey>.TryGetLengthEasilyEnumerable(keyCollection, out var length) && List<TValue>.TryGetLengthEasilyEnumerable(valueCollection, out var count2) ? Min(length, count2) : _defaultCapacity : throw new ArgumentNullException(null), comparer)
	{
		(keys, values) = (keyCollection, valueCollection).RemoveDoubles();
		if (keys.Length > _sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(IEnumerable<TKey> keyCollection, IEnumerable<TValue> valueCollection, Func<TKey, TKey, int> compareFunction) : this(keyCollection, valueCollection, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(IEnumerable<(TKey Key, TValue Value)> collection) : this(collection, G.Comparer<TKey>.Default) { }

	public SortedDictionary(IEnumerable<(TKey Key, TValue Value)> collection, IComparer<TKey>? comparer) : this(collection != null ? List<TKey>.TryGetLengthEasilyEnumerable(collection, out var length) ? length : _defaultCapacity : throw new ArgumentNullException(nameof(collection)), comparer)
	{
		(keys, values) = collection.RemoveDoubles(x => x.Key).Break();
		if (keys.Length > _sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, int> compareFunction) : this(collection, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, G.Comparer<TKey>.Default) { }

	public SortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IComparer<TKey>? comparer) : this(collection != null ? List<TKey>.TryGetLengthEasilyEnumerable(collection, out var length) ? length : _defaultCapacity : throw new ArgumentNullException(nameof(collection)), comparer)
	{
		(keys, values) = collection.RemoveDoubles(x => x.Key).Break(x => x.Key, x => x.Value);
		if (keys.Length > _sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, int> compareFunction) : this(collection, new Comparer<TKey>(compareFunction)) { }

	public virtual TValue this[TKey key]
	{
		get
		{
			var i = IndexOfKey(key);
			if (i >= 0)
				return values.GetInternal(i);
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
					return values.GetInternal(i);
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
					throw new ArgumentException("Ошибка, такое значение не подходит для этой коллекции.", nameof(value));
				}
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException("Ошибка, такой ключ не подходит для этой коллекции.", nameof(key));
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
			throw new ArgumentException("Ошибка, элемент с таким ключом уже был добавлен.", nameof(key));
		Insert(~i, key, value);
	}

	public virtual void Add((TKey Key, TValue Value) item) => Add(item.Key, item.Value);

	void System.Collections.IDictionary.Add(object key, object? value)
	{
		ArgumentNullException.ThrowIfNull(key);
		try
		{
			var tempKey = (TKey)key;
			try
			{
				Add(tempKey, (TValue?)value ?? throw new ArgumentNullException(nameof(value)));
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException("Ошибка, такое значение не подходит для этой коллекции.", nameof(value));
			}
		}
		catch (InvalidCastException)
		{
			throw new ArgumentException("Ошибка, такой ключ не подходит для этой коллекции.", nameof(key));
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
		if (index >= 0 && EqualityComparer<TValue>.Default.Equals(values.GetInternal(index), keyValuePair.Value))
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
		ArgumentNullException.ThrowIfNull(array);
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		for (var i = 0; i < Length; i++)
		{
			KeyValuePair<TKey, TValue> entry = new(keys.GetInternal(i), values.GetInternal(i));
			array[arrayIndex + i] = entry;
		}
	}

	void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Rank != 1)
			throw new RankException();
		if (array.GetLowerBound(0) != 0)
			throw new ArgumentException("Нижняя граница массива должна быть равной нулю.", nameof(array));
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		if (array is KeyValuePair<TKey, TValue>[] keyValuePairArray)
			for (var i = 0; i < Length; i++)
				keyValuePairArray[i + arrayIndex] = new(keys.GetInternal(i), values.GetInternal(i));
		else
		{
			if (array is not object[] objects)
				throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
			try
			{
				for (var i = 0; i < Length; i++)
					objects[i + arrayIndex] = new KeyValuePair<TKey, TValue>(keys.GetInternal(i), values.GetInternal(i));
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
			}
		}
	}

	public virtual void ExceptWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
	{
		var indexes = other.Convert(IndexOf).ToHashSet();
		keys.FilterInPlace((x, index) => !indexes.Contains(index));
		values.FilterInPlace((x, index) => !indexes.Contains(index));
	}

	public virtual void ExceptWith(IEnumerable<TKey> other)
	{
		var indexes = other.Convert(IndexOfKey).ToHashSet();
		keys.FilterInPlace((x, index) => !indexes.Contains(index));
		values.FilterInPlace((x, index) => !indexes.Contains(index));
	}

	public virtual void ExceptWith(IEnumerable<(TKey Key, TValue Value)> other)
	{
		var indexes = other.Convert(IndexOf).ToHashSet();
		keys.FilterInPlace((x, index) => !indexes.Contains(index));
		values.FilterInPlace((x, index) => !indexes.Contains(index));
	}

	public virtual TValue GetByIndex(int index)
	{
		if (index < 0 || index >= keys.Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		return values.GetInternal(index);
	}

	public virtual Enumerator GetEnumerator() => new(this, Enumerator.KeyValuePair);

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual TKey GetKey(int index)
	{
		if (index < 0 || index >= keys.Length) throw new ArgumentOutOfRangeException(nameof(index));
		return keys.GetInternal(index);
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

	public virtual int IndexOf(KeyValuePair<TKey, TValue> item) => IndexOf((item.Key, item.Value));

	public virtual int IndexOf((TKey Key, TValue Value) item)
	{
		var index = IndexOfKey(item.Key);
		if (index == -1 || EqualityComparer<TValue>.Default.Equals(values.GetInternal(index), item.Value))
			return index;
		return -1;
	}

	public virtual int IndexOfKey(TKey key)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (keys.Length <= _sortingThreshold)
			return keys.FindIndex(x => comparer.Compare(x, key) == 0);
		var ret = Search(key);
		return ret >= 0 ? ret : -1;
	}

	public virtual int IndexOfValue(TValue value) => values.IndexOf(value, 0, keys.Length);

	private protected void Insert(int index, TKey key, TValue value)
	{
		keys.Insert(index, key);
		values.Insert(index, value);
		if (keys.Length >= _sortingThreshold)
			keys.Sort(values, comparer);
	}

	public virtual void IntersectWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
	{
		var indexes = other.Convert(IndexOf).ToHashSet();
		keys.FilterInPlace((x, index) => indexes.Contains(index));
		values.FilterInPlace((x, index) => indexes.Contains(index));
	}

	public virtual void IntersectWith(IEnumerable<TKey> other)
	{
		var indexes = other.Convert(IndexOfKey).ToHashSet();
		keys.FilterInPlace((x, index) => indexes.Contains(index));
		values.FilterInPlace((x, index) => indexes.Contains(index));
	}

	public virtual void IntersectWith(IEnumerable<(TKey Key, TValue Value)> other)
	{
		var indexes = other.Convert(IndexOf).ToHashSet();
		keys.FilterInPlace((x, index) => indexes.Contains(index));
		values.FilterInPlace((x, index) => indexes.Contains(index));
	}

	private protected static bool IsCompatibleKey(object key)
	{
		ArgumentNullException.ThrowIfNull(key);
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

	public virtual bool RemoveValue(KeyValuePair<TKey, TValue> keyValuePair)
	{
		var index = IndexOfKey(keyValuePair.Key);
		if (index >= 0 && EqualityComparer<TValue>.Default.Equals(values.GetInternal(index), keyValuePair.Value))
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}

	public virtual bool RemoveValue(TKey key, TValue value) => RemoveValue((key, value));

	public virtual bool RemoveValue((TKey Key, TValue Value) item) => RemoveValue(new KeyValuePair<TKey, TValue>(item.Key, item.Value));

	public virtual int Search(TKey key)
	{
		if (keys.Length <= _sortingThreshold)
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

	public virtual bool TryAdd(TKey key, TValue value)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		var i = Search(key);
		if (i >= 0)
			return false;
		Insert(~i, key, value);
		return true;
	}

	public virtual bool TryAdd((TKey Key, TValue Value) item) => TryAdd(item.Key, item.Value);

	public virtual bool TryGetValue(TKey key, out TValue value)
	{
		var i = IndexOfKey(key);
		if (i >= 0)
		{
			value = values.GetInternal(i);
			return true;
		}
		value = default!;
		return false;
	}

	public virtual void UnionWith(IEnumerable<KeyValuePair<TKey, TValue>> other) => other.ForEach(x => this[x.Key] = x.Value);

	public virtual void UnionWith(IEnumerable<(TKey Key, TValue Value)> other) => other.ForEach(x => this[x.Key] = x.Value);

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
				key = _sortedDictionary.keys.GetInternal(index);
				value = _sortedDictionary.values.GetInternal(index);
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
				currentKey = _sortedDictionary.keys.GetInternal(index);
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
				currentValue = _sortedDictionary.values.GetInternal(index);
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

		public TKey this[int index] {
			get => _dict.GetKey(index);
			set => throw new NotSupportedException("Это действие не поддерживается в этой коллекции."
			+ " Если оно нужно вам, используйте индексатор или методы Add(), TryAdd() оригинального словаря.");
		}

		public int Length => _dict.Length;

		public bool IsReadOnly => true;

		bool System.Collections.ICollection.IsSynchronized => false;

		object System.Collections.ICollection.SyncRoot => ((ICollection)_dict).SyncRoot;

		public void Add(TKey key) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		public void Clear() =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		public bool Contains(TKey key) => _dict.ContainsKey(key);

		public void CopyTo(TKey[] array, int arrayIndex) => _dict.keys.CopyTo(0, array, arrayIndex, _dict.Length);

		void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
		{
			ArgumentNullException.ThrowIfNull(array);
			if (array.Rank != 1)
				throw new RankException();
			try
			{
				((ICollection)_dict.keys).CopyTo(array, arrayIndex);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
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

		public void Insert(int index, TKey value) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		public void RemoveAt(int index) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		public bool RemoveValue(TKey key) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");
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

		public void Add(TValue key) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		public void Clear() =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		public bool Contains(TValue value) => _dict.ContainsValue(value);

		public void CopyTo(TValue[] array, int arrayIndex) => _dict.values.CopyTo(0, array, arrayIndex, _dict.Length);

		void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
		{
			ArgumentNullException.ThrowIfNull(array);
			if (array.Rank != 1)
				throw new RankException();
			try
			{
				((ICollection)_dict.values).CopyTo(array, arrayIndex);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
			}
		}

		internal ValueEnumerator GetEnumerator() => new(_dict);

		IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int IndexOf(TValue value) => _dict.values.IndexOf(value, 0, _dict.Length);

		public void Insert(int index, TValue value) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		public void RemoveAt(int index) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		public bool RemoveValue(TValue value) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");
	}
}
