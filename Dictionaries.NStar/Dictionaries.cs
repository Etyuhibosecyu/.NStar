global using Corlib.NStar;
global using Mpir.NET;
global using System;
global using System.Collections;
global using System.Diagnostics;
global using System.Runtime.InteropServices;
global using G = System.Collections.Generic;
global using static Corlib.NStar.Extents;
global using static System.Math;
global using E = System.Linq.Enumerable;
global using String = Corlib.NStar.String;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace Dictionaries.NStar;

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
public abstract class BaseDictionary<TKey, TValue, TCertain> : IDictionary<TKey, TValue>, Corlib.NStar.IDictionary, IReadOnlyDictionary<TKey, TValue> where TKey : notnull where TCertain : BaseDictionary<TKey, TValue, TCertain>, new()
{
	[NonSerialized]
	private protected object? _syncRoot;
	public abstract TValue this[TKey key] { get; set; }

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
					throw new ArgumentException("Ошибка, такое значение не подходит для этой коллекции.", nameof(value));
				}
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException("Ошибка, такой ключ не подходит для этой коллекции.", nameof(key));
			}
		}
	}

	public abstract G.ICollection<TKey> Keys { get; }

	G.ICollection<TKey> G.IDictionary<TKey, TValue>.Keys => Keys;

	System.Collections.ICollection System.Collections.IDictionary.Keys => GetKeyListHelper();

	G.IEnumerable<TKey> G.IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	public abstract G.ICollection<TValue> Values { get; }

	G.ICollection<TValue> G.IDictionary<TKey, TValue>.Values => Values;

	System.Collections.ICollection System.Collections.IDictionary.Values => GetValueListHelper();

	G.IEnumerable<TValue> G.IReadOnlyDictionary<TKey, TValue>.Values => Values;

	bool G.ICollection<G.KeyValuePair<TKey, TValue>>.IsReadOnly => false;

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

	public abstract int Length { get; }

	public abstract void Add(TKey key, TValue value);

	public virtual void Add((TKey Key, TValue Value) item) => Add(item.Key, item.Value);

	public virtual void Add(G.KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

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

	void G.ICollection<G.KeyValuePair<TKey, TValue>>.Add(G.KeyValuePair<TKey, TValue> keyValuePair) => Add(keyValuePair.Key, keyValuePair.Value);

	public abstract void Clear();

	public virtual bool Contains((TKey Key, TValue Value) item)
	{
		if (TryGetValue(item.Key, out var value) && G.EqualityComparer<TValue>.Default.Equals(value, item.Value))
			return true;
		return false;
	}

	public virtual bool Contains(G.KeyValuePair<TKey, TValue> keyValuePair) => Contains((keyValuePair.Key, keyValuePair.Value));

	bool System.Collections.IDictionary.Contains(object key)
	{
		if (IsCompatibleKey(key))
			return ContainsKey((TKey)key);
		return false;
	}

	public abstract bool ContainsKey(TKey key);

	void G.ICollection<G.KeyValuePair<TKey, TValue>>.CopyTo(G.KeyValuePair<TKey, TValue>[] array, int arrayIndex) => CopyToHelper(array, arrayIndex);

	void System.Collections.ICollection.CopyTo(Array array, int arrayIndex) => CopyToHelper(array, arrayIndex);

	protected abstract void CopyToHelper(Array array, int arrayIndex);

	protected abstract void CopyToHelper(G.KeyValuePair<TKey, TValue>[] array, int arrayIndex);

	public abstract void ExceptWith(G.IEnumerable<(TKey Key, TValue Value)> other);

	public abstract void ExceptWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other);

	public abstract void ExceptWith(G.IEnumerable<TKey> other);

	public abstract G.IEnumerator<G.KeyValuePair<TKey, TValue>> GetEnumerator();

	G.IEnumerator<G.KeyValuePair<TKey, TValue>> G.IEnumerable<G.KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() => GetEnumeratorHelper();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	protected abstract IDictionaryEnumerator GetEnumeratorHelper();

	internal abstract System.Collections.ICollection GetKeyListHelper();

	internal abstract System.Collections.ICollection GetValueListHelper();

	public abstract void IntersectWith(G.IEnumerable<(TKey Key, TValue Value)> other);

	public abstract void IntersectWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other);

	public abstract void IntersectWith(G.IEnumerable<TKey> other);

	private protected static bool IsCompatibleKey(object key)
	{
		ArgumentNullException.ThrowIfNull(key);
		return key is TKey;
	}

	public abstract bool Remove(TKey key);

	public abstract bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value);

	void System.Collections.IDictionary.Remove(object key)
	{
		if (IsCompatibleKey(key))
			Remove((TKey)key);
	}

	public abstract bool RemoveValue(G.KeyValuePair<TKey, TValue> keyValuePair);

	public virtual bool RemoveValue(TKey key, TValue value) => RemoveValue((key, value));

	public virtual bool RemoveValue((TKey Key, TValue Value) item) => RemoveValue(new G.KeyValuePair<TKey, TValue>(item.Key, item.Value));

	public virtual TCertain SymmetricExceptWith(G.IEnumerable<(TKey Key, TValue Value)> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		var this2 = (TCertain)this;
		if (Length == 0)
		{
			UnionWith(other);
			return this2;
		}
		if (other == this)
		{
			Clear();
			return this2;
		}
		return SymmetricExceptInternal(other);
	}

	public virtual TCertain SymmetricExceptWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		var this2 = (TCertain)this;
		if (Length == 0)
		{
			UnionWith(other);
			return this2;
		}
		if (other == this)
		{
			Clear();
			return this2;
		}
		return SymmetricExceptInternal(other);
	}

	protected virtual TCertain SymmetricExceptInternal(G.IEnumerable<(TKey Key, TValue Value)> other)
	{
		foreach (var item in other is G.IDictionary<TKey, TValue> dic ? dic : other.ToDictionary())
		{
			var result = ContainsKey(item.Key) ? Remove(item.Key) : TryAdd(item);
			Debug.Assert(result);
		}
		return (TCertain)this;
	}

	protected virtual TCertain SymmetricExceptInternal(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other)
	{
		foreach (var item in other is G.IDictionary<TKey, TValue> dic ? dic : other.ToDictionary())
		{
			var result = ContainsKey(item.Key) ? Remove(item.Key) : TryAdd(item);
			Debug.Assert(result);
		}
		return (TCertain)this;
	}

	public abstract void TrimExcess();

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

	public virtual bool TryAdd((TKey Key, TValue Value) item) => TryAdd(item.Key, item.Value);

	public virtual bool TryAdd(G.KeyValuePair<TKey, TValue> item) => TryAdd(item.Key, item.Value);

	public abstract bool TryGetValue(TKey key, out TValue value);

	public virtual void UnionWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other)
	{
		foreach (var x in other)
			this[x.Key] = x.Value;
	}

	public virtual void UnionWith(G.IEnumerable<(TKey Key, TValue Value)> other)
	{
		foreach (var x in other)
			this[x.Key] = x.Value;
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
// Оптимизированный словарь, который для маленького числа элементов использует поэлементное сравнение
// и добавление в конец (импортируя этот функционал из SortedDictionary, чтобы не создавать
// лишнее дублирование), а при увеличении числа элементов действует как классический словарь от Microsoft.
public class Dictionary<TKey, TValue> : BaseDictionary<TKey, TValue, Dictionary<TKey, TValue>> where TKey : notnull
{
	private protected SortedDictionary<TKey, TValue>? low;
	private protected G.Dictionary<TKey, TValue>? high;
	private protected bool isHigh;
	private protected readonly G.IEqualityComparer<TKey> comparer;
	private protected const int _hashThreshold = 64;

	public Dictionary() : this(G.EqualityComparer<TKey>.Default) { }

	public Dictionary(int capacity) : this(capacity, (G.IEqualityComparer<TKey>?)null) { }

	public Dictionary(G.IEqualityComparer<TKey>? comparer) : this(0, comparer) { }

	public Dictionary(Func<TKey, TKey, bool> equalFunction) : this(new EComparer<TKey>(equalFunction)) { }

	public Dictionary(Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(int capacity, G.IEqualityComparer<TKey>? comparer)
	{
		comparer ??= G.EqualityComparer<TKey>.Default;
		this.comparer = comparer;
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
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

	public Dictionary(G.IDictionary<TKey, TValue> dictionary) : this(dictionary, (G.IEqualityComparer<TKey>?)null) { }

	public Dictionary(G.IDictionary<TKey, TValue> dictionary, G.IEqualityComparer<TKey>? comparer)
	{
		comparer ??= G.EqualityComparer<TKey>.Default;
		this.comparer = comparer;
		ArgumentNullException.ThrowIfNull(dictionary);
		if (dictionary.Count < _hashThreshold)
			low = new(dictionary, new Comparer<TKey>((x, y) => comparer.Equals(x, y) ? 0 : -1));
		else
		{
			high = new(dictionary, comparer);
			isHigh = true;
		}
	}

	public Dictionary(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction) : this(dictionary, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) : this(dictionary, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, bool unordered = false) : this(keyCollection, valueCollection, (G.IEqualityComparer<TKey>?)null, unordered) { }

	public Dictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, G.IEqualityComparer<TKey>? comparer, bool unordered = false) : this(new UnsortedDictionary<TKey, TValue>(keyCollection, valueCollection, unordered), comparer) { }

	public Dictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction, bool unordered = false) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction), unordered) { }

	public Dictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, bool unordered = false) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction, hashCodeFunction), unordered) { }

	public Dictionary(G.IEnumerable<(TKey Key, TValue Value)> collection, bool unordered = false) : this(collection, (G.IEqualityComparer<TKey>?)null, unordered) { }

	public Dictionary(G.IEnumerable<(TKey Key, TValue Value)> collection, G.IEqualityComparer<TKey>? comparer, bool unordered = false) : this(new UnsortedDictionary<TKey, TValue>(collection, unordered), comparer) { }

	public Dictionary(G.IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction, bool unordered = false) : this(collection, new EComparer<TKey>(equalFunction), unordered) { }

	public Dictionary(G.IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, bool unordered = false) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction), unordered) { }

	public Dictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, bool unordered = false) : this(collection, (G.IEqualityComparer<TKey>?)null, unordered) { }

	public Dictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, G.IEqualityComparer<TKey>? comparer, bool unordered = false) : this(new UnsortedDictionary<TKey, TValue>(collection, unordered), comparer) { }

	public Dictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction, bool unordered = false) : this(collection, new EComparer<TKey>(equalFunction), unordered) { }

	public Dictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction, bool unordered = false) : this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction), unordered) { }

	public override TValue this[TKey key]
	{
		get
		{
			if (!isHigh && low != null)
				return low[key];
			else if (high != null)
				return high[key];
			else
				throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		}
		set
		{
			if (!isHigh && low != null)
				low[key] = value;
			else if (high != null)
				high[key] = value;
			else
				throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
			if (!isHigh && low != null && Length >= _hashThreshold)
			{
				high = new(low, comparer);
				low = null;
				isHigh = true;
			}
		}
	}

	public virtual G.IEqualityComparer<TKey> Comparer => comparer;

	public override int Length
	{
		get
		{
			if (!isHigh && low != null)
				return low.Length;
			else if (high != null)
				return high.Count;
			else
				throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		}
	}

	public override G.ICollection<TKey> Keys
	{
		get
		{
			if (!isHigh && low != null)
				return low.Keys;
			else if (high != null)
				return high.Keys;
			else
				throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		}
	}

	public override G.ICollection<TValue> Values
	{
		get
		{
			if (!isHigh && low != null)
				return low.Values;
			else if (high != null)
				return high.Values;
			else
				throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		}
	}

	public override void Add(TKey key, TValue value)
	{
		if (!isHigh && low != null)
			low.Add(key, value);
		else if (high != null)
			high.Add(key, value);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		if (!isHigh && low != null && Length >= _hashThreshold)
		{
			high = new(low, comparer);
			low = null;
			isHigh = true;
		}
	}

	public override void Clear()
	{
		if (!isHigh && low != null)
			low.Clear();
		else if (high != null)
			high.Clear();
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override bool ContainsKey(TKey key)
	{
		if (!isHigh && low != null)
			return low.ContainsKey(key);
		else if (high != null)
			return high.ContainsKey(key);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	protected override void CopyToHelper(Array array, int arrayIndex)
	{
		if (!isHigh && low != null)
			((Corlib.NStar.ICollection)low).CopyTo(array, arrayIndex);
		else if (high != null)
			((Corlib.NStar.ICollection)high).CopyTo(array, arrayIndex);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	protected override void CopyToHelper(G.KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		if (!isHigh && low != null)
			((G.ICollection<G.KeyValuePair<TKey, TValue>>)low).CopyTo(array, arrayIndex);
		else if (high != null)
			((G.ICollection<G.KeyValuePair<TKey, TValue>>)high).CopyTo(array, arrayIndex);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override void ExceptWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other)
	{
		if (!isHigh && low != null)
			low.ExceptWith(other);
		else if (high != null)
			foreach (var x in other)
				RemoveValue(x);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override void ExceptWith(G.IEnumerable<TKey> other)
	{
		if (!isHigh && low != null)
			low.ExceptWith(other);
		else if (high != null)
			foreach (var x in other)
				Remove(x);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override void ExceptWith(G.IEnumerable<(TKey Key, TValue Value)> other)
	{
		if (!isHigh && low != null)
			low.ExceptWith(other);
		else if (high != null)
			foreach (var x in other)
				RemoveValue(x);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override G.IEnumerator<G.KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		if (!isHigh && low != null)
			return low.GetEnumerator();
		else if (high != null)
			return high.GetEnumerator();
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	protected override IDictionaryEnumerator GetEnumeratorHelper()
	{
		if (!isHigh && low != null)
			return ((System.Collections.IDictionary)low).GetEnumerator();
		else if (high != null)
			return high.GetEnumerator();
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	internal override System.Collections.ICollection GetKeyListHelper()
	{
		if (!isHigh && low != null)
			return low.GetKeyListHelper();
		else if (high != null)
			return high.Keys;
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	internal override System.Collections.ICollection GetValueListHelper()
	{
		if (!isHigh && low != null)
			return low.GetValueListHelper();
		else if (high != null)
			return high.Values;
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override void IntersectWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other)
	{
		if (!isHigh && low != null)
			low.IntersectWith(other);
		else if (high != null)
		{
			var hs = other.ToHashSet();
			foreach (var x in high)
				if (!hs.Contains(x))
					high.Remove(x.Key);
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override void IntersectWith(G.IEnumerable<TKey> other)
	{
		if (!isHigh && low != null)
			low.IntersectWith(other);
		else if (high != null)
		{
			var hs = other.ToHashSet();
			foreach (var x in high)
				if (!hs.Contains(x.Key))
					high.Remove(x.Key);
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override void IntersectWith(G.IEnumerable<(TKey Key, TValue Value)> other)
	{
		if (!isHigh && low != null)
			low.IntersectWith(other);
		else if (high != null)
		{
			var hs = other.ToHashSet();
			foreach (var x in high)
				if (!hs.Contains((x.Key, x.Value)))
					high.Remove(x.Key);
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override bool Remove(TKey key)
	{
		if (!isHigh && low != null)
			return low.Remove(key);
		else if (high != null)
			return high.Remove(key);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		if (!isHigh && low != null)
			return low.Remove(key, out value);
		else if (high != null)
			return high.Remove(key, out value);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override bool RemoveValue(G.KeyValuePair<TKey, TValue> keyValuePair)
	{
		if (!isHigh && low != null)
			return low.RemoveValue(keyValuePair);
		else if (high != null)
			return ((G.ICollection<G.KeyValuePair<TKey, TValue>>)high).Remove(keyValuePair);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override void TrimExcess()
	{
		if (!isHigh && low != null)
			low.TrimExcess();
		else if (high != null)
			high.TrimExcess();
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public override bool TryGetValue(TKey key, out TValue value)
	{
		if (!isHigh && low != null)
			return low.TryGetValue(key, out value);
		else if (high != null)
			return high.TryGetValue(key, out value!);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public static implicit operator G.Dictionary<TKey, TValue>?(Dictionary<TKey, TValue>? x)
	{
		if (x == null)
			return null;
		else if (!x.isHigh && x.low != null)
			return new(x.low);
		else if (x.high != null)
			return x.high;
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один словарь"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}
}

internal class UnsortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
	private protected readonly List<TKey> keys;
	private protected readonly List<TValue> values;
	[NonSerialized]
	private protected object _syncRoot = new();

	public UnsortedDictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection, bool unordered = false) => (keys, values) = E.DistinctBy(E.Zip(keyCollection, valueCollection), x => x.First).Break();

	public UnsortedDictionary(G.IEnumerable<(TKey Key, TValue Value)> collection, bool unordered = false) => (keys, values) = E.DistinctBy(collection, x => x.Key).Break();

	public UnsortedDictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, bool unordered = false) => (keys, values) = E.DistinctBy(collection, x => x.Key).Break(x => x.Key, x => x.Value);

	public virtual TValue this[TKey key] => throw new NotSupportedException();

	TValue G.IDictionary<TKey, TValue>.this[TKey key] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

	public virtual G.IEnumerable<TKey> Keys => throw new NotSupportedException();

	public virtual int Length => keys.Length;

	public virtual G.IEnumerable<TValue> Values => throw new NotSupportedException();

	G.ICollection<TKey> G.IDictionary<TKey, TValue>.Keys => throw new NotSupportedException();

	G.ICollection<TValue> G.IDictionary<TKey, TValue>.Values => throw new NotSupportedException();

	public virtual bool IsReadOnly => false;

	public bool IsSynchronized => false;

	public object SyncRoot => _syncRoot;

	public virtual void Add(TKey key, TValue value)
	{
		if (!ContainsKey(key))
		{
			keys.Add(key);
			values.Add(value);
		}
	}

	public virtual void Add(G.KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

	public virtual void Clear()
	{
		keys.Clear();
		values.Clear();
	}

	public virtual bool Contains(G.KeyValuePair<TKey, TValue> item)
	{
		var index = IndexOfKey(item.Key);
		return index >= 0 && G.EqualityComparer<TValue>.Default.Equals(values[index], item.Value);
	}

	public virtual bool ContainsKey(TKey key) => keys.Contains(key);

	public void CopyTo(Array array, int index) => throw new NotSupportedException();

	public virtual void CopyTo(G.KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotSupportedException();

	public virtual Enumerator GetEnumerator() => new(this);

	G.IEnumerator<G.KeyValuePair<TKey, TValue>> G.IEnumerable<G.KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private protected int IndexOfKey(TKey key) => keys.IndexOf(key);

	public virtual bool Remove(TKey key) => throw new NotSupportedException();

	public virtual bool RemoveValue(G.KeyValuePair<TKey, TValue> item) => throw new NotSupportedException();

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

	public struct Enumerator(UnsortedDictionary<TKey, TValue> dictionary) : G.IEnumerator<G.KeyValuePair<TKey, TValue>>
	{
		private readonly UnsortedDictionary<TKey, TValue> _dict = dictionary;
		private int index = 0;

		public G.KeyValuePair<TKey, TValue> Current { get; private set; } = default!;

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
