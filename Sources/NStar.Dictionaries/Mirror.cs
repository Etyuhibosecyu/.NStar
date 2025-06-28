using NStar.Core;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NStar.Dictionaries;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class Mirror<TKey, TValue> : IDictionary<TKey, TValue>, NStar.Core.IDictionary, IReadOnlyDictionary<TKey, TValue> where TKey : notnull where TValue : notnull
{
	protected struct Entry
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
		public TKey key;	 // Key of entry
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
	private protected G.IEqualityComparer<TKey> _comparer;
	private protected G.IEqualityComparer<TValue> _comparerM;
	private protected KeyCollection? _keys;
	private protected ValueCollection? _values;
	private protected const int StartOfFreeList = -3;

	public Mirror() : this(0, (G.IEqualityComparer<TKey>?)null, null) { }

	public Mirror(int capacity) : this(capacity, (G.IEqualityComparer<TKey>?)null, null) { }

	public Mirror(G.IEqualityComparer<TKey>? keyComparer) : this(0, keyComparer, null) { }

	public Mirror(Func<TKey, TKey, bool> equalFunction) : this(new EComparer<TKey>(equalFunction)) { }

	public Mirror(Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(G.IEqualityComparer<TKey>? keyComparer, G.IEqualityComparer<TValue>? valueComparer) : this(0, keyComparer, valueComparer) { }

	public Mirror(Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public Mirror(int capacity, G.IEqualityComparer<TKey>? keyComparer) : this(capacity, keyComparer, null) { }

	public Mirror(int capacity, Func<TKey, TKey, bool> equalFunction) : this(capacity, new EComparer<TKey>(equalFunction)) { }

	public Mirror(int capacity, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(capacity, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(int capacity, G.IEqualityComparer<TKey>? keyComparer, G.IEqualityComparer<TValue>? valueComparer)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		if (capacity > 0)
			Initialize(capacity);
		if (!typeof(TKey).IsValueType)
			_comparer = keyComparer ?? G.EqualityComparer<TKey>.Default;
		else if (keyComparer != null && keyComparer != G.EqualityComparer<TKey>.Default) // first check for null to avoid forcing default comparer instantiation unnecessarily
			_comparer = keyComparer;
		else
			_comparer = G.EqualityComparer<TKey>.Default;
		if (!typeof(TValue).IsValueType)
			_comparerM = valueComparer ?? G.EqualityComparer<TValue>.Default;
		else if (valueComparer != null && valueComparer != G.EqualityComparer<TValue>.Default) // first check for null to avoid forcing default comparer instantiation unnecessarily
			_comparerM = valueComparer;
		else
			_comparerM = G.EqualityComparer<TValue>.Default;
	}

	public Mirror(int capacity, Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(capacity, new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(int capacity, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(capacity, new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary) : this(dictionary, (G.IEqualityComparer<TKey>?)null, null) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary, G.IEqualityComparer<TKey>? keyComparer) : this(dictionary, keyComparer, null) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction) : this(dictionary, new EComparer<TKey>(equalFunction)) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(dictionary, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary, G.IEqualityComparer<TKey>? keyComparer, G.IEqualityComparer<TValue>? valueComparer) : this(dictionary?.Count ?? 0, keyComparer, valueComparer)
	{
		ArgumentNullException.ThrowIfNull(dictionary);
		AddRange(dictionary);
	}

	public Mirror(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(dictionary, new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(dictionary, new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public Mirror(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection) : this(collection, (G.IEqualityComparer<TKey>?)null, null) { }

	public Mirror(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, G.IEqualityComparer<TKey>? keyComparer) : this(collection, keyComparer, null) { }

	public Mirror(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction) : this(collection, new EComparer<TKey>(equalFunction)) { }

	public Mirror(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, G.IEqualityComparer<TKey>? keyComparer, G.IEqualityComparer<TValue>? valueComparer) : this(collection != null && collection.TryGetLengthEasily(out var length) ? length : 0, keyComparer, valueComparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		AddRange(collection);
	}

	public Mirror(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(collection, new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public Mirror(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection) : this(keyCollection, valueCollection, (G.IEqualityComparer<TKey>?)null) { }

	public Mirror(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, G.IEqualityComparer<TKey>? keyComparer) : this(keyCollection, valueCollection, keyComparer, null) { }

	public Mirror(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction)) { }

	public Mirror(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, G.IEqualityComparer<TKey>? keyComparer, G.IEqualityComparer<TValue>? valueComparer) : this(new UnsortedDictionary<TKey, TValue>(keyCollection, valueCollection), keyComparer, valueComparer) { }

	public Mirror(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public Mirror(G.IEnumerable<(TKey Key, TValue Value)> collection) : this(collection, (G.IEqualityComparer<TKey>?)null) { }

	public Mirror(G.IEnumerable<(TKey Key, TValue Value)> collection, G.IEqualityComparer<TKey>? keyComparer) : this(collection, keyComparer, null) { }

	public Mirror(G.IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction) : this(collection, new EComparer<TKey>(equalFunction)) { }

	public Mirror(G.IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Mirror(G.IEnumerable<(TKey Key, TValue Value)> collection, G.IEqualityComparer<TKey>? keyComparer, G.IEqualityComparer<TValue>? valueComparer) : this(new UnsortedDictionary<TKey, TValue>(collection), keyComparer, valueComparer) { }

	public Mirror(G.IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction, Func<TValue, TValue, bool> equalFunctionM) : this(collection, new EComparer<TKey>(equalFunction), new EComparer<TValue>(equalFunctionM)) { }

	public Mirror(G.IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, Func<TValue, TValue, bool> equalFunctionM, Func<TValue, int> hashCodeFunctionM) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction), new EComparer<TValue>(equalFunctionM, hashCodeFunctionM)) { }

	public virtual TKey this[TValue targetValue] { get => GetKey(targetValue); set => SetKey(targetValue, value); }

	public virtual TValue this[TKey key] { get => GetValue(key); set => SetValue(key, value); }

	bool G.ICollection<G.KeyValuePair<TKey, TValue>>.IsReadOnly => false;

	public virtual G.IEqualityComparer<TKey> KeyComparer => _comparer;

	public virtual KeyCollection Keys => _keys ??= new KeyCollection(this);

	G.ICollection<TKey> G.IDictionary<TKey, TValue>.Keys => Keys;

	G.IEnumerable<TKey> G.IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	public virtual int Length => _count - _freeCount;

	public virtual G.IEqualityComparer<TValue> ValueComparer => _comparerM;

	public virtual ValueCollection Values => _values ??= new ValueCollection(this);

	G.ICollection<TValue> G.IDictionary<TKey, TValue>.Values => Values;

	G.IEnumerable<TValue> G.IReadOnlyDictionary<TKey, TValue>.Values => Values;

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
			ArgumentNullException.ThrowIfNull(key);
			ArgumentNullException.ThrowIfNull(value);
			try
			{
				var tempKey = (TKey)key;
				try
				{
					this[tempKey] = (TValue)value;
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

	void G.ICollection<G.KeyValuePair<TKey, TValue>>.Add(G.KeyValuePair<TKey, TValue> keyValuePair) => Add(keyValuePair.Key, keyValuePair.Value);

	void System.Collections.IDictionary.Add(object key, object? value)
	{
		ArgumentNullException.ThrowIfNull(key);
		ArgumentNullException.ThrowIfNull(value);
		try
		{
			var tempKey = (TKey)key;
			try
			{
				Add(tempKey, (TValue)value);
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

	protected virtual void AddRange(G.IEnumerable<G.KeyValuePair<TKey, TValue>> enumerable)
	{
		// It is likely that the passed-in enumerable is Mirror<TKey,TValue>. When this is the case,
		// avoid the enumerator allocation and overhead by looping through the entries array directly.
		// We only do this when dictionary is Mirror<TKey,TValue> and not a subclass, to maintain
		// back-compat with subclasses that may have overridden the enumerator behavior.
		if (enumerable is Mirror<TKey, TValue> source)
		{
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
		ReadOnlySpan<G.KeyValuePair<TKey, TValue>> span;
		if (enumerable is G.KeyValuePair<TKey, TValue>[] array)
			span = array;
		else if (enumerable is List<G.KeyValuePair<TKey, TValue>> list)
			span = list.AsSpan();
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
			Debug.Assert(_bucketsM != null, "_bucketsM should be non-null");
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

	public virtual bool Contains(G.KeyValuePair<TKey, TValue> keyValuePair) => Contains(keyValuePair.Key, keyValuePair.Value);

	public virtual bool Contains(TKey key, TValue value)
	{
		ref var value2 = ref FindValue(key);
		if (!Unsafe.IsNullRef(ref value2) && G.EqualityComparer<TValue>.Default.Equals(value2, value))
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

	protected virtual void CopyEntries(Entry[] entries, int length)
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
				entry.nextM = bucketM - 1; // Value in _bucketsM is 1-based
				bucketM = newCount + 1;
				newCount++;
			}
		}
		_count = newCount;
		_freeCount = 0;
	}

	protected virtual void CopyTo(G.KeyValuePair<TKey, TValue>[] array, int index)
	{
		ArgumentNullException.ThrowIfNull(array);
		if ((uint)index > (uint)array.Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (array.Length - index < Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		Debug.Assert(_entries != null);
		var length = _count;
		Debug.Assert(_entries != null);
		var entries = _entries;
		for (var i = 0; i < length; i++)
			if (entries[i].next >= -1 && entries[i].nextM >= -1)
				array[index++] = new G.KeyValuePair<TKey, TValue>(entries[i].key, entries[i].value);
	}

	void G.ICollection<G.KeyValuePair<TKey, TValue>>.CopyTo(G.KeyValuePair<TKey, TValue>[] array, int index) => CopyTo(array, index);

	void System.Collections.ICollection.CopyTo(Array array, int index)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Rank != 1)
			throw new RankException("Массив должен иметь одно измерение.");
		if (array.GetLowerBound(0) != 0)
			throw new ArgumentException("Нижняя граница массива должна быть равной нулю.", nameof(array));
		if ((uint)index > (uint)array.Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (array.Length - index < Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		Debug.Assert(_entries != null);
		if (array is G.KeyValuePair<TKey, TValue>[] pairs)
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
					objects[index++] = new G.KeyValuePair<TKey, TValue>(entries[i].key, entries[i].value);
		}
	}

	/// <summary>
	/// Ensures that the dictionary can hold up to 'capacity' entries without any further expansion of its backing storage
	/// </summary>
	protected virtual int EnsureCapacity(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
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

	private protected static int Find(Entry[] entries, int otherCurrent, int bucket, bool mirrored = false)
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
				throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
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
			// KeyType: Devirtualize with G.EqualityComparer<TValue>.Default intrinsic
			currentM--; // Key in _bucketsM is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
			do
			{
				// Should be a while loop https://github.com/dotnet/runtime/issues/9422
				// Test in if to drop range check for following array access
				if ((uint)currentM >= (uint)entries.Length)
					goto ReturnNotFound;
				entry = ref entries[currentM];
				if (entry.hashCodeM == hashCodeM && G.EqualityComparer<TValue>.Default.Equals(entry.value, value))
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
			currentM--; // Key in _bucketsM is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
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
		throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
			+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
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
			// ValueType: Devirtualize with G.EqualityComparer<TKey>.Default intrinsic
			current--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
			do
			{
				// Should be a while loop https://github.com/dotnet/runtime/issues/9422
				// Test in if to drop range check for following array access
				if ((uint)current >= (uint)entries.Length)
					goto ReturnNotFound;
				entry = ref entries[current];
				if (entry.hashCode == hashCode && G.EqualityComparer<TKey>.Default.Equals(entry.key, key))
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
		throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
			+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	ReturnFound:
		ref var value = ref entry.value;
	Return:
		return ref value!;
	ReturnNotFound:
		value = ref Unsafe.NullRef<TValue>()!;
		goto Return;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual ref int GetBucket(uint hashCode)
	{
		Debug.Assert(_buckets != null, "_buckets should be non-null");
		var buckets = _buckets;
		return ref buckets[hashCode % buckets.Length];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual ref int GetBucketM(uint hashCode)
	{
		Debug.Assert(_bucketsM != null, "_bucketsM should be non-null");
		var buckets = _bucketsM;
		return ref buckets[hashCode % buckets.Length];
	}

	public virtual Enumerator GetEnumerator() => new(this, Enumerator.KeyValuePair);

	G.IEnumerator<G.KeyValuePair<TKey, TValue>> G.IEnumerable<G.KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => ((G.IEnumerable<G.KeyValuePair<TKey, TValue>>)this).GetEnumerator();

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
		throw new G.KeyNotFoundException();
	}

	protected virtual int Initialize(int capacity)
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
		ArgumentNullException.ThrowIfNull(key);
		return key is TKey;
	}

	private protected static bool IsCompatibleValue(object value)
	{
		ArgumentNullException.ThrowIfNull(value);
		return value is TValue;
	}

	private protected static void ProcessLast(Entry[] entries, Entry entry, ref int bucket, int last, bool mirrored = false)
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
		// The overload RemoveKey(TKey key, out TValue value) is a copy of this method with one
		// additional statement to copy the value for entry being removed into the output parameter.
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
			if (entry.hashCode == hashCode && (typeof(TKey).IsValueType && comparer == null ? G.EqualityComparer<TKey>.Default.Equals(entry.key, key) : comparer!.Equals(entry.key, key)))
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
				throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		}
		return false;
	}

	public virtual bool RemoveKey(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		// This overload is a copy of the overload RemoveKey(TKey key) with one additional
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
			if (entry.hashCode == hashCode && (typeof(TKey).IsValueType || comparer == null ? G.EqualityComparer<TKey>.Default.Equals(entry.key, key) : comparer.Equals(entry.key, key)))
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
				throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		}
		value = default;
		return false;
	}

	public virtual bool RemoveValue(TValue value)
	{
		// The overload RemoveValue(TValue value, out TKey key) is a copy of this method with one
		// additional statement to copy the key for entry being removed into the output parameter.
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
			if (entry.hashCodeM == hashCodeM && (typeof(TValue).IsValueType && comparerM == null ? G.EqualityComparer<TValue>.Default.Equals(entry.value, value) : comparerM!.Equals(entry.value, value)))
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
				throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		}
		return false;
	}

	public virtual bool RemoveValue(TValue value, [MaybeNullWhen(false)] out TKey key)
	{
		// This overload is a copy of the overload RemoveValue(TValue value) with one additional
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
			if (entry.hashCodeM == hashCodeM && (typeof(TValue).IsValueType && comparerM == null ? G.EqualityComparer<TValue>.Default.Equals(entry.value, value) : comparerM!.Equals(entry.value, value)))
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
				throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		}
		key = default;
		return false;
	}

	bool ICollection<G.KeyValuePair<TKey, TValue>>.RemoveValue(G.KeyValuePair<TKey, TValue> keyValuePair)
	{
		ref var value = ref FindValue(keyValuePair.Key);
		if (!Unsafe.IsNullRef(ref value) && G.EqualityComparer<TValue>.Default.Equals(value, keyValuePair.Value))
		{
			RemoveKey(keyValuePair.Key);
			return true;
		}
		return false;
	}

	protected virtual void Resize() => Resize(HashHelpers.ExpandPrime(_count), false);

	protected virtual void Resize(int newSize, bool forceNewHashCodes)
	{
		// Value types never rehash
		Debug.Assert(!forceNewHashCodes || !typeof(TKey).IsValueType || !typeof(TValue).IsValueType);
		Debug.Assert(_entries != null, "_entries should be non-null");
		Debug.Assert(newSize >= _entries.Length);
		var entries = new Entry[newSize];
		var length = _count;
		Array.Copy(_entries, entries, length);
		if ((!typeof(TKey).IsValueType || !typeof(TValue).IsValueType) && forceNewHashCodes)
		{
			var comparer = _comparer = G.EqualityComparer<TKey>.Default;
			var comparerM = _comparerM = G.EqualityComparer<TValue>.Default;
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
		_entries = entries;
		Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
		Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
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
		ArgumentOutOfRangeException.ThrowIfLessThan(capacity, Length);
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
		Debug.Assert(_buckets != null, "_buckets should be non-null");
		Debug.Assert(_bucketsM != null, "_bucketsM should be non-null");
		var entries = _entries;
		Debug.Assert(entries != null, "expected entries to be non-null");
		var comparer = _comparer;
		var comparerM = _comparerM;
		var hashCode = (uint)(typeof(TKey).IsValueType && comparer == null ? key.GetHashCode() : comparer.GetHashCode(key));
		uint collisionCount = 0;
		ref var bucket = ref GetBucket(hashCode);
		var current = bucket - 1; // Value in _buckets is 1-based
		if (typeof(TKey).IsValueType && comparer == null) // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
			while (true)
			{
				var a = TryInsertInternal(key, value, behavior, entries, G.EqualityComparer<TKey>.Default, comparerM, hashCode, ref collisionCount, ref current);
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
		var hashCodeM = (uint)(typeof(TValue).IsValueType && comparer == null ? value.GetHashCode() : comparerM.GetHashCode(value));
		uint collisionCountM = 0;
		ref var bucketM = ref GetBucketM(hashCodeM);
		var currentM = bucketM - 1; // Value in _bucketsM is 1-based
		if (typeof(TValue).IsValueType && comparerM == null) // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
			while (true)
			{
				var a = TryInsertInternal(key, value, behavior, entries, comparer ?? G.EqualityComparer<TKey>.Default, G.EqualityComparer<TValue>.Default, hashCodeM, ref collisionCountM, ref currentM, true);
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
		entry.nextM = bucketM - 1; // Value in _bucketsM is 1-based
		entry.key = key;
		entry.value = value;
		bucket = index + 1; // Value in _buckets is 1-based
		bucketM = indexM + 1; // Value in _bucketsM is 1-based
		_version++;
		Debug.Assert(entries.All(x => x.next >= -1 == x.nextM >= -1));
		Debug.Assert(this.All(x => _comparer.Equals(x.Key, GetKey(x.Value)) && _comparerM.Equals(GetValue(x.Key), x.Value)));
		// Value types never rehash
		if (!typeof(TKey).IsValueType && collisionCount > HashHelpers.HashCollisionThreshold || !typeof(TValue).IsValueType && collisionCountM > HashHelpers.HashCollisionThreshold)
			Resize(entries.Length, true);
		return true;
	}

	private protected int TryInsertInternal(TKey key, TValue value, InsertionBehavior behavior, Entry[] entries, G.IEqualityComparer<TKey> comparer, G.IEqualityComparer<TValue> comparerM, uint hashCode, ref uint collisionCount, ref int current, bool mirrored = false)
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
					InsertionBehavior.ThrowOnExisting => throw new ArgumentException("Невозможно вставить такой элемент.", mirrored ? nameof(value) : nameof(key)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка."),
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
					entry.hashCode = (uint)(typeof(TKey).IsValueType && comparer == null ? key.GetHashCode() : comparer.GetHashCode(key));
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
					entry.hashCodeM = (uint)(typeof(TValue).IsValueType && comparer == null ? value.GetHashCode() : comparerM.GetHashCode(value));
					bucketM = ref GetBucketM(entry.hashCodeM);
					entry.nextM = bucketM - 1; // Value in _bucketsM is 1-based
					entries[current].value = value;
					bucketM = current + 1; // Value in _bucketsM is 1-based
				}
				return 1;
			}
			return behavior == InsertionBehavior.ThrowOnExisting ? throw new ArgumentException("Невозможно вставить такой элемент.", mirrored ? nameof(value) : nameof(key)) : 2;
		}
		current = mirrored ? entries[current].nextM : entries[current].next;
		collisionCount++;
		if (collisionCount > (uint)entries.Length)
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		return 3;
	}

	public struct Enumerator : G.IEnumerator<G.KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
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

		public G.KeyValuePair<TKey, TValue> Current { get; private set; }

		readonly object? IEnumerator.Current
		{
			get
			{
				if (_index == 0 || _index == _dictionary._count + 1)
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				if (_getEnumeratorRetType == DictEntry)
					return new DictionaryEntry(Current.Key, Current.Value);
				return new G.KeyValuePair<TKey, TValue>(Current.Key, Current.Value);
			}
		}

		readonly DictionaryEntry IDictionaryEnumerator.Entry
		{
			get
			{
				if (_index == 0 || _index == _dictionary._count + 1)
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				return new DictionaryEntry(Current.Key, Current.Value);
			}
		}

		readonly object IDictionaryEnumerator.Key
		{
			get
			{
				if (_index == 0 || _index == _dictionary._count + 1)
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				return Current.Key;
			}
		}

		readonly object? IDictionaryEnumerator.Value
		{
			get
			{
				if (_index == 0 || _index == _dictionary._count + 1)
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				return Current.Value;
			}
		}

		public readonly void Dispose() { }

		public bool MoveNext()
		{
			if (_version != _dictionary._version)
				throw new InvalidOperationException("Коллекцию нельзя изменять во время перечисления по ней.");
			// Use unsigned comparison since we set index to dictionary.length+1 when the enumeration ends.
			// dictionary.length+1 could be negative if dictionary.length is int.MaxValue
			while ((uint)_index < (uint)_dictionary._count)
			{
				ref var entry = ref _dictionary._entries![_index++];
				if (entry.next >= -1 && entry.nextM >= -1)
				{
					Current = new G.KeyValuePair<TKey, TValue>(entry.key, entry.value);
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
				throw new InvalidOperationException("Коллекцию нельзя изменять во время перечисления по ней.");
			_index = 0;
			Current = default;
		}
	}

	[DebuggerDisplay("Length = {Length}"), Serializable]
	public sealed class KeyCollection(Mirror<TKey, TValue> dictionary) : ICollection<TKey>, NStar.Core.ICollection, IReadOnlyCollection<TKey>
	{
		private readonly Mirror<TKey, TValue> _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

		bool G.ICollection<TKey>.IsReadOnly => true;

		bool System.Collections.ICollection.IsSynchronized => false;

		public int Length => _dictionary.Length;

		object System.Collections.ICollection.SyncRoot => ((System.Collections.ICollection)_dictionary).SyncRoot;

		void G.ICollection<TKey>.Add(TKey item) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		void G.ICollection<TKey>.Clear() =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		public bool Contains(TKey item) => _dictionary.ContainsKey(item);

		public void CopyTo(TKey[] array, int index)
		{
			ArgumentNullException.ThrowIfNull(array);
			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (array.Length - index < _dictionary.Length)
				throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
			var length = _dictionary._count;
			var entries = _dictionary._entries;
			for (var i = 0; i < length; i++)
				if (entries![i].next >= -1 && entries![i].nextM >= -1)
					array[index++] = entries[i].key;
		}

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			ArgumentNullException.ThrowIfNull(array);
			if (array.Rank != 1)
				throw new RankException("Массив должен иметь одно измерение.");
			if (array.GetLowerBound(0) != 0)
				throw new ArgumentException("Нижняя граница массива должна быть равной нулю.", nameof(array));
			if ((uint)index > (uint)array.Length)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (array.Length - index < _dictionary.Length)
				throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
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

		public KeyEnumerator GetEnumerator() => new(_dictionary);

		G.IEnumerator<TKey> G.IEnumerable<TKey>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((G.IEnumerable<TKey>)this).GetEnumerator();

		bool ICollection<TKey>.RemoveValue(TKey item) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте метод Remove() оригинального словаря.");
	}

	public struct KeyEnumerator : G.IEnumerator<TKey>, IEnumerator
	{
		private readonly Mirror<TKey, TValue> _dictionary;
		private int _index;
		private readonly int _version;

		internal KeyEnumerator(Mirror<TKey, TValue> dictionary)
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
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				return Current;
			}
		}

		public readonly void Dispose() { }

		public bool MoveNext()
		{
			if (_version != _dictionary._version)
				throw new InvalidOperationException("Коллекцию нельзя изменять во время перечисления по ней.");
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
				throw new InvalidOperationException("Коллекцию нельзя изменять во время перечисления по ней.");
			_index = 0;
			Current = default!;
		}
	}

	[DebuggerDisplay("Length = {Length}"), Serializable]
	public sealed class ValueCollection(Mirror<TKey, TValue> dictionary) : ICollection<TValue>, NStar.Core.ICollection, IReadOnlyCollection<TValue>
	{
		private readonly Mirror<TKey, TValue> _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

		bool G.ICollection<TValue>.IsReadOnly => true;

		bool System.Collections.ICollection.IsSynchronized => false;

		public int Length => _dictionary.Length;

		object System.Collections.ICollection.SyncRoot => ((System.Collections.ICollection)_dictionary).SyncRoot;

		void G.ICollection<TValue>.Add(TValue item) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		void G.ICollection<TValue>.Clear() =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте одноименный метод оригинального словаря.");

		bool G.ICollection<TValue>.Contains(TValue item) => _dictionary.ContainsValue(item);

		public void CopyTo(TValue[] array, int index)
		{
			ArgumentNullException.ThrowIfNull(array);
			if ((uint)index > array.Length)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (array.Length - index < _dictionary.Length)
				throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
			var length = _dictionary._count;
			var entries = _dictionary._entries;
			for (var i = 0; i < length; i++)
				if (entries![i].next >= -1 && entries![i].nextM >= -1)
					array[index++] = entries[i].value;
		}

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			ArgumentNullException.ThrowIfNull(array);
			if (array.Rank != 1)
				throw new RankException("Массив должен иметь одно измерение.");
			if (array.GetLowerBound(0) != 0)
				throw new ArgumentException("Нижняя граница массива должна быть равной нулю.", nameof(array));
			if ((uint)index > (uint)array.Length)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (array.Length - index < _dictionary.Length)
				throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
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

		public ValueEnumerator GetEnumerator() => new(_dictionary);

		G.IEnumerator<TValue> G.IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((G.IEnumerable<TValue>)this).GetEnumerator();

		bool ICollection<TValue>.RemoveValue(TValue item) =>
			throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Вместо этого используйте метод Remove() оригинального словаря.");
	}

	public struct ValueEnumerator : G.IEnumerator<TValue>, IEnumerator
	{
		private readonly Mirror<TKey, TValue> _dictionary;
		private int _index;
		private readonly int _version;

		internal ValueEnumerator(Mirror<TKey, TValue> dictionary)
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
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				return Current;
			}
		}

		public readonly void Dispose() { }

		public bool MoveNext()
		{
			if (_version != _dictionary._version)
				throw new InvalidOperationException("Коллекцию нельзя изменять во время перечисления по ней.");
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
				throw new InvalidOperationException("Коллекцию нельзя изменять во время перечисления по ней.");
			_index = 0;
			Current = default!;
		}
	}
}

[Serializable]
public class ValueNotFoundException : SystemException
{
	public ValueNotFoundException() : this("The given value was not present in the dictionary.") { }

	public ValueNotFoundException(string? message) : base(message) { }

	public ValueNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}
