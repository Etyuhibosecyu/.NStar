using System.Diagnostics.CodeAnalysis;

namespace NStar.Dictionaries;

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
public class SortedDictionary<TKey, TValue> : BaseDictionary<TKey, TValue, SortedDictionary<TKey, TValue>> where TKey : notnull
{
	private protected readonly List<TKey> keys;
	private protected readonly List<TValue> values;
	private protected readonly G.IComparer<TKey> comparer;
	private protected KeyList? keyList;
	private protected ValueList? valueList;

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

	public SortedDictionary(G.IComparer<TKey>? comparer) : this()
	{
		if (comparer is not null)
			this.comparer = comparer;
	}

	public SortedDictionary(Func<TKey, TKey, int> compareFunction) : this(new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(int capacity, G.IComparer<TKey>? comparer) : this(comparer) => Capacity = capacity;

	public SortedDictionary(int capacity, Func<TKey, TKey, int> compareFunction) : this(capacity, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(G.IDictionary<TKey, TValue> dictionary) : this(dictionary, G.Comparer<TKey>.Default) { }

	public SortedDictionary(G.IDictionary<TKey, TValue> dictionary, G.IComparer<TKey>? comparer) : this(dictionary is not null ? dictionary.Count : throw new ArgumentNullException(nameof(dictionary)), comparer)
	{
		(keys, values) = E.DistinctBy(dictionary, x => x.Key).Break(x => x.Key, x => x.Value);
		if (keys.Length > _sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, int> compareFunction) : this(dictionary, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection) : this(keyCollection, valueCollection, G.Comparer<TKey>.Default) { }

	public SortedDictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, G.IComparer<TKey>? comparer) : this(keyCollection is not null && valueCollection is not null ? keyCollection.TryGetLengthEasily(out var length) && valueCollection.TryGetLengthEasily(out var count2) ? Min(length, count2) : _defaultCapacity : throw new ArgumentNullException(null), comparer)
	{
		(keys, values) = E.DistinctBy(E.Zip(keyCollection, valueCollection), x => x.First).Break();
		if (keys.Length > _sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, Func<TKey, TKey, int> compareFunction) : this(keyCollection, valueCollection, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(G.IEnumerable<(TKey Key, TValue Value)> collection) : this(collection, G.Comparer<TKey>.Default) { }

	public SortedDictionary(G.IEnumerable<(TKey Key, TValue Value)> collection, G.IComparer<TKey>? comparer) : this(collection is not null ? collection.TryGetLengthEasily(out var length) ? length : _defaultCapacity : throw new ArgumentNullException(nameof(collection)), comparer)
	{
		(keys, values) = E.DistinctBy(collection, x => x.Key).Break();
		if (keys.Length > _sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(G.IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, int> compareFunction) : this(collection, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection) : this(collection, G.Comparer<TKey>.Default) { }

	public SortedDictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, G.IComparer<TKey>? comparer) : this(collection is not null ? collection.TryGetLengthEasily(out var length) ? length : _defaultCapacity : throw new ArgumentNullException(nameof(collection)), comparer)
	{
		(keys, values) = E.DistinctBy(collection, x => x.Key).Break(x => x.Key, x => x.Value);
		if (keys.Length > _sortingThreshold)
			keys.Sort(values, comparer);
	}

	public SortedDictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, int> compareFunction) : this(collection, new Comparer<TKey>(compareFunction)) { }

	public SortedDictionary(List<(TKey Key, TValue Value)> collection) : this(collection, G.Comparer<TKey>.Default) { }

	public SortedDictionary(List<(TKey Key, TValue Value)> collection, G.IComparer<TKey>? comparer) : this((G.IEnumerable<(TKey Key, TValue Value)>)collection, comparer) { }

	public SortedDictionary(List<G.KeyValuePair<TKey, TValue>> collection) : this(collection, G.Comparer<TKey>.Default) { }

	public SortedDictionary(List<G.KeyValuePair<TKey, TValue>> collection, G.IComparer<TKey>? comparer) : this((G.IEnumerable<G.KeyValuePair<TKey, TValue>>)collection, comparer) { }

	public override TValue this[TKey key]
	{
		get
		{
			var i = IndexOfKey(key);
			if (i >= 0)
				return values[i];
			throw new G.KeyNotFoundException();
		}
		set
		{
			if (key is null)
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

	public virtual int Capacity { get => keys.Capacity; set => values.Capacity = keys.Capacity = value; }

	public virtual G.IComparer<TKey> Comparer => comparer;

	public override int Length => keys.Length;

	public override IList<TKey> Keys => (IList<TKey>)GetKeyListHelper();

	public override IList<TValue> Values => (IList<TValue>)GetValueListHelper();

	public override void Add(TKey key, TValue value)
	{
		if (key is null)
			throw new ArgumentNullException(nameof(key));
		var i = Search(key);
		if (i >= 0)
			throw new ArgumentException("Ошибка, элемент с таким ключом уже был добавлен.", nameof(key));
		Insert(~i, key, value);
	}

	public override void Clear()
	{
		keys.Clear();
		values.Clear();
	}

	public override bool ContainsKey(TKey key) => IndexOfKey(key) >= 0;

	public virtual bool ContainsValue(TValue value) => IndexOfValue(value) >= 0;

	protected override void CopyToHelper(Array array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Rank != 1)
			throw new RankException("Массив должен иметь одно измерение.");
		if (array.GetLowerBound(0) != 0)
			throw new ArgumentException("Нижняя граница массива должна быть равной нулю.", nameof(array));
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		if (array is G.KeyValuePair<TKey, TValue>[] keyValuePairArray)
			for (var i = 0; i < Length; i++)
				keyValuePairArray[i + arrayIndex] = new(keys[i], values[i]);
		else
		{
			if (array is not object[] objects)
				throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
			try
			{
				for (var i = 0; i < Length; i++)
					objects[i + arrayIndex] = new G.KeyValuePair<TKey, TValue>(keys[i], values[i]);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
			}
		}
	}

	protected override void CopyToHelper(G.KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		for (var i = 0; i < Length; i++)
		{
			G.KeyValuePair<TKey, TValue> entry = new(keys[i], values[i]);
			array[arrayIndex + i] = entry;
		}
	}

	public override void ExceptWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other)
	{
		var indexes = other.Convert(IndexOf).ToHashSet();
		keys.FilterInPlace((x, index) => !indexes.Contains(index));
		values.FilterInPlace((x, index) => !indexes.Contains(index));
	}

	public override void ExceptWith(G.IEnumerable<TKey> other)
	{
		var indexes = other.Convert(IndexOfKey).ToHashSet();
		keys.FilterInPlace((x, index) => !indexes.Contains(index));
		values.FilterInPlace((x, index) => !indexes.Contains(index));
	}

	public override void ExceptWith(G.IEnumerable<(TKey Key, TValue Value)> other)
	{
		var indexes = other.Convert(IndexOf).ToHashSet();
		keys.FilterInPlace((x, index) => !indexes.Contains(index));
		values.FilterInPlace((x, index) => !indexes.Contains(index));
	}

	public virtual TValue GetByIndex(int index)
	{
		if (index < 0 || index >= keys.Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		return values[index];
	}

	public override G.IEnumerator<G.KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this, Enumerator.KeyValuePair);

	protected override IDictionaryEnumerator GetEnumeratorHelper() => new Enumerator(this, Enumerator.KeyValuePair);

	public virtual TKey GetKey(int index)
	{
		if (index < 0 || index >= keys.Length) throw new ArgumentOutOfRangeException(nameof(index));
		return keys[index];
	}

	internal override System.Collections.ICollection GetKeyListHelper()
	{
		keyList ??= new(this);
		return keyList;
	}

	internal override System.Collections.ICollection GetValueListHelper()
	{
		valueList ??= new(this);
		return valueList;
	}

	public virtual int IndexOf(G.KeyValuePair<TKey, TValue> item) => IndexOf((item.Key, item.Value));

	public virtual int IndexOf((TKey Key, TValue Value) item)
	{
		var index = IndexOfKey(item.Key);
		if (index == -1 || G.EqualityComparer<TValue>.Default.Equals(values[index], item.Value))
			return index;
		return -1;
	}

	public virtual int IndexOfKey(TKey key)
	{
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

	public override void IntersectWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other)
	{
		var indexes = other.Convert(IndexOf).ToHashSet();
		keys.FilterInPlace((x, index) => indexes.Contains(index));
		values.FilterInPlace((x, index) => indexes.Contains(index));
	}

	public override void IntersectWith(G.IEnumerable<TKey> other)
	{
		var indexes = other.Convert(IndexOfKey).ToHashSet();
		keys.FilterInPlace((x, index) => indexes.Contains(index));
		values.FilterInPlace((x, index) => indexes.Contains(index));
	}

	public override void IntersectWith(G.IEnumerable<(TKey Key, TValue Value)> other)
	{
		var indexes = other.Convert(IndexOf).ToHashSet();
		keys.FilterInPlace((x, index) => indexes.Contains(index));
		values.FilterInPlace((x, index) => indexes.Contains(index));
	}

	public override bool Remove(TKey key)
	{
		var i = IndexOfKey(key);
		if (i >= 0)
			RemoveAt(i);
		return i >= 0;
	}

	public override bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		var index = IndexOfKey(key);
		value = values[index];
		if (index >= 0)
			RemoveAt(index);
		return index >= 0;
	}

	public virtual void RemoveAt(int index)
	{
		keys.RemoveAt(index);
		values.RemoveAt(index);
	}

	public override bool RemoveValue(G.KeyValuePair<TKey, TValue> keyValuePair)
	{
		var index = IndexOfKey(keyValuePair.Key);
		if (index >= 0 && G.EqualityComparer<TValue>.Default.Equals(values[index], keyValuePair.Value))
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}

	public virtual int Search(TKey key)
	{
		if (keys.Length <= _sortingThreshold)
		{
			var index = keys.FindIndex(x => comparer.Compare(x, key) == 0);
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

	public override void TrimExcess()
	{
		keys.TrimExcess();
		values.TrimExcess();
	}

	public override bool TryAdd(TKey key, TValue value)
	{
		if (key is null)
			throw new ArgumentNullException(nameof(key));
		var i = Search(key);
		if (i >= 0)
			return false;
		Insert(~i, key, value);
		return true;
	}

	public override bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
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

	public static implicit operator SortedDictionary<TKey, TValue>((TKey, TValue) x) => new() { x };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15 };

	public static implicit operator SortedDictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) => new() { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16 };

	[Serializable()]
	public struct Enumerator : G.IEnumerator<G.KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
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
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				return key!;
			}
		}

		readonly object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == _sortedDictionary.Length + 1)
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				if (getEnumeratorRetType == DictEntry)
					return new DictionaryEntry(key!, value);
				else
					return new G.KeyValuePair<TKey, TValue>(key, value);
			}
		}

		readonly object IDictionaryEnumerator.Value
		{
			get
			{
				if (index == 0 || index == _sortedDictionary.Length + 1)
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
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
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				return new(key!, value);
			}
		}

		public readonly G.KeyValuePair<TKey, TValue> Current => new(key, value);

		void IEnumerator.Reset()
		{
			index = 0;
			key = default!;
			value = default!;
		}
	}

	[Serializable()]
	internal sealed class KeyEnumerator : G.IEnumerator<TKey>, IEnumerator
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
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
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
	internal sealed class ValueEnumerator : G.IEnumerator<TValue>, IEnumerator
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
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
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
	internal sealed class KeyList : IList<TKey>, System.Collections.ICollection
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

		object System.Collections.ICollection.SyncRoot => ((System.Collections.ICollection)_dict).SyncRoot;

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
				throw new RankException("Массив должен иметь одно измерение.");
			try
			{
				((System.Collections.ICollection)_dict.keys).CopyTo(array, arrayIndex);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
			}
		}

		internal KeyEnumerator GetEnumerator() => new(_dict);

		G.IEnumerator<TKey> G.IEnumerable<TKey>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int IndexOf(TKey key)
		{
			if (key is null)
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
	internal sealed class ValueList : IList<TValue>, System.Collections.ICollection
	{
		private readonly SortedDictionary<TKey, TValue> _dict;

		internal ValueList(SortedDictionary<TKey, TValue> dictionary) => _dict = dictionary;

		public TValue this[int index] { get => _dict.GetByIndex(index); set => _dict.SetByIndex(index, value); }

		public int Length => _dict.Length;

		public bool IsReadOnly => true;

		bool System.Collections.ICollection.IsSynchronized => false;

		object System.Collections.ICollection.SyncRoot => ((System.Collections.ICollection)_dict).SyncRoot;

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
				throw new RankException("Массив должен иметь одно измерение.");
			try
			{
				((System.Collections.ICollection)_dict.values).CopyTo(array, arrayIndex);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
			}
		}

		internal ValueEnumerator GetEnumerator() => new(_dict);

		G.IEnumerator<TValue> G.IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

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
