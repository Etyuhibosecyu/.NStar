global using NStar.Core;
global using System;
global using System.Collections;
global using System.Diagnostics;
global using System.Runtime.InteropServices;
global using static NStar.Core.Extents;
global using static System.Math;
global using E = System.Linq.Enumerable;
global using G = System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NStar.Dictionaries;

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
public abstract class BaseDictionary<TKey, TValue, TCertain> : IDictionary<TKey, TValue>, Core.IDictionary,
	IReadOnlyDictionary<TKey, TValue> where TKey : notnull where TCertain : BaseDictionary<TKey, TValue, TCertain>, new()
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

	System.Collections.ICollection System.Collections.IDictionary.Keys => GetKeyListHelper();

	G.IEnumerable<TKey> G.IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	public abstract G.ICollection<TValue> Values { get; }

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
			if (_syncRoot is null)
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

	void G.ICollection<G.KeyValuePair<TKey, TValue>>.Add(G.KeyValuePair<TKey, TValue> keyValuePair) =>
		Add(keyValuePair.Key, keyValuePair.Value);

	public abstract void Clear();

	public virtual bool Contains((TKey Key, TValue Value) item)
	{
		if (TryGetValue(item.Key, out var value) && G.EqualityComparer<TValue>.Default.Equals(value, item.Value))
			return true;
		return false;
	}

	public virtual bool Contains(G.KeyValuePair<TKey, TValue> keyValuePair) =>
		Contains((keyValuePair.Key, keyValuePair.Value));

	bool System.Collections.IDictionary.Contains(object key)
	{
		if (IsCompatibleKey(key))
			return ContainsKey((TKey)key);
		return false;
	}

	public abstract bool ContainsKey(TKey key);

	void G.ICollection<G.KeyValuePair<TKey, TValue>>.CopyTo(G.KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
		CopyToHelper(array, arrayIndex);

	void System.Collections.ICollection.CopyTo(Array array, int index) => CopyToHelper(array, index);

	protected abstract void CopyToHelper(Array array, int arrayIndex);

	protected abstract void CopyToHelper(G.KeyValuePair<TKey, TValue>[] array, int arrayIndex);

	/// <summary>
	/// Удаляет из данного словаря все пары ключ-значение, <b>при</b>сутствующие в указанной последовательности.
	/// </summary>
	/// <param name="other">Последовательность для удаления элементов.</param>
	/// <remarks>
	/// Если в данном словаре и в указанной последовательности окажутся разные значения по одинаковому ключу,
	/// данный метод этот ключ не удаляет - используйте <see cref="ExceptWith(G.IEnumerable{TKey})"/>.
	/// </remarks>
	public abstract void ExceptWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other);

	/// <summary>
	/// Удаляет из данного словаря все ключи, <b>при</b>сутствующие в указанной последовательности.
	/// </summary>
	/// <param name="other">Последовательность для удаления элементов.</param>
	public abstract void ExceptWith(G.IEnumerable<TKey> other);

	/// <summary>
	/// Удаляет из данного словаря все кортежи из ключа и значения, <b>при</b>сутствующие в указанной последовательности.
	/// </summary>
	/// <param name="other">Последовательность для удаления элементов.</param>
	/// <remarks>
	/// Если в данном словаре и в указанной последовательности окажутся разные значения по одинаковому ключу,
	/// данный метод этот ключ не удаляет - используйте <see cref="ExceptWith(G.IEnumerable{TKey})"/>.
	/// </remarks>
	public abstract void ExceptWith(G.IEnumerable<(TKey Key, TValue Value)> other);

	public abstract G.IEnumerator<G.KeyValuePair<TKey, TValue>> GetEnumerator();

	G.IEnumerator<G.KeyValuePair<TKey, TValue>> G.IEnumerable<G.KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() => GetEnumeratorHelper();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	protected abstract IDictionaryEnumerator GetEnumeratorHelper();

	internal virtual System.Collections.ICollection GetKeyListHelper() => (System.Collections.ICollection)Keys;

	internal virtual System.Collections.ICollection GetValueListHelper() => (System.Collections.ICollection)Values;

	/// <summary>
	/// Удаляет из данного словаря все пары ключ-значение, <b>от</b>сутствующие в указанной последовательности.
	/// </summary>
	/// <param name="other">Последовательность для удаления элементов.</param>
	public abstract void IntersectWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other);

	/// <summary>
	/// Удаляет из данного словаря все ключи, <b>от</b>сутствующие в указанной последовательности.
	/// </summary>
	/// <param name="other">Последовательность для удаления элементов.</param>
	public abstract void IntersectWith(G.IEnumerable<TKey> other);

	/// <summary>
	/// Удаляет из данного словаря все кортежи из ключа и значения, <b>от</b>сутствующие в указанной последовательности.
	/// </summary>
	/// <param name="other">Последовательность для удаления элементов.</param>
	public abstract void IntersectWith(G.IEnumerable<(TKey Key, TValue Value)> other);

	private protected static bool IsCompatibleKey(object key)
	{
		ArgumentNullException.ThrowIfNull(key);
		return key is TKey;
	}

	public abstract bool Remove(TKey key);

	/// <inheritdoc cref="G.Dictionary{TKey, TValue}.Remove(TKey, out TValue)"/>
	public abstract bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value);

	void System.Collections.IDictionary.Remove(object key)
	{
		if (IsCompatibleKey(key))
			Remove((TKey)key);
	}

	public abstract bool RemoveValue(G.KeyValuePair<TKey, TValue> keyValuePair);

	public virtual bool RemoveValue(TKey key, TValue value) => RemoveValue((key, value));

	public virtual bool RemoveValue((TKey Key, TValue Value) item) =>
		RemoveValue(new G.KeyValuePair<TKey, TValue>(item.Key, item.Value));

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

	/// <inheritdoc cref="G.Dictionary{TKey, TValue}.TrimExcess()"/>
	public abstract void TrimExcess();

	/// <inheritdoc cref="G.Dictionary{TKey, TValue}.TryAdd(TKey, TValue)"/>
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

	public abstract bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);

	public virtual void UnionWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other)
	{
		foreach (var x in other)
			this[x.Key] = x.Value;
	}

	public virtual void UnionWith(G.IEnumerable<(TKey Key, TValue Value)> other)
	{
		foreach (var (Key, Value) in other)
			this[Key] = Value;
	}
}

/// <summary>
/// Представляет коллекцию пар ключ-значение с доступом по ключу за Õ(1) ("O(1) в большинстве случаев").
/// Добавление и удаление ключа также занимают Õ(1) времени, а вот индексация отсутствует.
/// В отличие от словаря от Microsoft, этот также предоставляет дополнительные методы, такие как
/// <see cref="ExceptWith(G.IEnumerable{TKey})"/>, <see cref="IntersectWith(G.IEnumerable{TKey})"/> и некоторые другие.
/// </summary>
/// <typeparam name="TKey">Тип всех ключей в словаре.</typeparam>
/// <typeparam name="TValue">Тип всех значений в словаре.</typeparam>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class Dictionary<TKey, TValue> : BaseDictionary<TKey, TValue, Dictionary<TKey, TValue>> where TKey : notnull
{
	private protected G.Dictionary<TKey, TValue> _underlying;
	private protected readonly G.IEqualityComparer<TKey> comparer;
	private protected const int _hashThreshold = 64;

	public Dictionary() : this(G.EqualityComparer<TKey>.Default) { }

	public Dictionary(int capacity) : this(capacity, (G.IEqualityComparer<TKey>?)null) { }

	public Dictionary(G.IEqualityComparer<TKey>? comparer) : this(0, comparer) { }

	public Dictionary(Func<TKey, TKey, bool> equalFunction) : this(new EComparer<TKey>(equalFunction)) { }

	public Dictionary(Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	: this(new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(int capacity, G.IEqualityComparer<TKey>? comparer)
	{
		comparer ??= G.EqualityComparer<TKey>.Default;
		this.comparer = comparer;
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		_underlying = new(capacity);
	}

	public Dictionary(int capacity, Func<TKey, TKey, bool> equalFunction)
	: this(capacity, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(int capacity, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	: this(capacity, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(G.IDictionary<TKey, TValue> dictionary) : this(dictionary, (G.IEqualityComparer<TKey>?)null) { }

	public Dictionary(G.IDictionary<TKey, TValue> dictionary, G.IEqualityComparer<TKey>? comparer)
	{
		comparer ??= G.EqualityComparer<TKey>.Default;
		this.comparer = comparer;
		ArgumentNullException.ThrowIfNull(dictionary);
		_underlying = new(dictionary, comparer);
	}

	public Dictionary(G.IDictionary<TKey, TValue> dictionary, Func<TKey, TKey, bool> equalFunction)
		: this(dictionary, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(G.IDictionary<TKey, TValue> dictionary,
		Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	: this(dictionary, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection)
		: this(keyCollection, valueCollection, (G.IEqualityComparer<TKey>?)null) { }

	public Dictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection,
		G.IEqualityComparer<TKey>? comparer)
		: this(new UnsortedDictionary<TKey, TValue>(keyCollection, valueCollection), comparer) { }

	public Dictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection,
		Func<TKey, TKey, bool> equalFunction) : this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection,
		Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
		: this(keyCollection, valueCollection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(G.IEnumerable<(TKey Key, TValue Value)> collection)
	: this(collection, (G.IEqualityComparer<TKey>?)null) { }

	public Dictionary(G.IEnumerable<(TKey Key, TValue Value)> collection, G.IEqualityComparer<TKey>? comparer)
	: this(new UnsortedDictionary<TKey, TValue>(collection), comparer) { }

	public Dictionary(G.IEnumerable<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction)
	: this(collection, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(G.IEnumerable<(TKey Key, TValue Value)> collection,
		Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	: this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection)
	: this(collection, (G.IEqualityComparer<TKey>?)null) { }

	public Dictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, G.IEqualityComparer<TKey>? comparer)
	: this(new UnsortedDictionary<TKey, TValue>(collection), comparer) { }

	public Dictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction)
	: this(collection, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection,
		Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	: this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(List<(TKey Key, TValue Value)> collection)
	: this(collection, (G.IEqualityComparer<TKey>?)null) { }

	public Dictionary(List<(TKey Key, TValue Value)> collection, G.IEqualityComparer<TKey>? comparer)
	: this(new UnsortedDictionary<TKey, TValue>(collection), comparer) { }

	public Dictionary(List<(TKey Key, TValue Value)> collection, Func<TKey, TKey, bool> equalFunction)
	: this(collection, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(List<(TKey Key, TValue Value)> collection,
		Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	: this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public Dictionary(List<G.KeyValuePair<TKey, TValue>> collection)
	: this(collection, (G.IEqualityComparer<TKey>?)null) { }

	public Dictionary(List<G.KeyValuePair<TKey, TValue>> collection, G.IEqualityComparer<TKey>? comparer)
	: this(new UnsortedDictionary<TKey, TValue>(collection), comparer) { }

	public Dictionary(List<G.KeyValuePair<TKey, TValue>> collection, Func<TKey, TKey, bool> equalFunction)
	: this(collection, new EComparer<TKey>(equalFunction)) { }

	public Dictionary(List<G.KeyValuePair<TKey, TValue>> collection,
		Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	: this(collection, new EComparer<TKey>(equalFunction, hashCodeFunction)) { }

	public override TValue this[TKey key] { get => _underlying[key]; set => _underlying[key] = value; }

	public virtual G.IEqualityComparer<TKey> Comparer => comparer;

	public override int Length => _underlying.Count;

	public override G.ICollection<TKey> Keys => _underlying.Keys;

	public override G.ICollection<TValue> Values => _underlying.Values;

	public override void Add(TKey key, TValue value) => _underlying.Add(key, value);

	public override void Clear() => _underlying.Clear();

	public override bool ContainsKey(TKey key) => _underlying.ContainsKey(key);

	protected override void CopyToHelper(Array array, int arrayIndex) =>
		((System.Collections.ICollection)_underlying).CopyTo(array, arrayIndex);

	protected override void CopyToHelper(G.KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
		((G.ICollection<G.KeyValuePair<TKey, TValue>>)_underlying).CopyTo(array, arrayIndex);

	/// <inheritdoc/>
	public override void ExceptWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other)
	{
		foreach (var x in other)
			RemoveValue(x);
	}

	/// <inheritdoc/>
	public override void ExceptWith(G.IEnumerable<TKey> other)
	{
		foreach (var x in other)
			Remove(x);
	}

	/// <inheritdoc/>
	public override void ExceptWith(G.IEnumerable<(TKey Key, TValue Value)> other)
	{
		foreach (var x in other)
			RemoveValue(x);
	}

	public override G.IEnumerator<G.KeyValuePair<TKey, TValue>> GetEnumerator() => _underlying.GetEnumerator();

	protected override IDictionaryEnumerator GetEnumeratorHelper() => _underlying.GetEnumerator();

	internal override System.Collections.ICollection GetKeyListHelper() => _underlying.Keys;

	internal override System.Collections.ICollection GetValueListHelper() => _underlying.Values;

	/// <inheritdoc/>
	public override void IntersectWith(G.IEnumerable<G.KeyValuePair<TKey, TValue>> other) =>
		ExceptWith(this.ToHashSet().ExceptWith(other));

	/// <inheritdoc/>
	public override void IntersectWith(G.IEnumerable<TKey> other) => ExceptWith(Keys.ToHashSet().ExceptWith(other));

	/// <inheritdoc/>
	public override void IntersectWith(G.IEnumerable<(TKey Key, TValue Value)> other) =>
		ExceptWith(this.Convert(x => (x.Key, x.Value)).ToHashSet().ExceptWith(other));

	public override bool Remove(TKey key) => _underlying.Remove(key);

	/// <inheritdoc/>
	public override bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value) => _underlying.Remove(key, out value);

	public override bool RemoveValue(G.KeyValuePair<TKey, TValue> keyValuePair) =>
		((G.ICollection<G.KeyValuePair<TKey, TValue>>)_underlying).Remove(keyValuePair);

	/// <inheritdoc/>
	public override void TrimExcess() => _underlying.TrimExcess();

	public override bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) =>
		_underlying.TryGetValue(key, out value);

	public static implicit operator Dictionary<TKey, TValue>((TKey, TValue) x) => new([x]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue)) x) => new([x.Item1, x.Item2]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue)) x) =>
		new([x.Item1, x.Item2, x.Item3]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue)) x) => new([x.Item1, x.Item2, x.Item3, x.Item4]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue)) x) => new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue)) x) => new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue)) x) =>
		new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) =>
		new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) =>
		new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) =>
		new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue)) x) =>
		new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue)) x) =>
		new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) =>
		new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10,
			x.Item11, x.Item12, x.Item13]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) =>
		new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10,
			x.Item11, x.Item12, x.Item13, x.Item14]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue)) x) =>
		new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10,
			x.Item11, x.Item12, x.Item13, x.Item14, x.Item15]);

	public static implicit operator Dictionary<TKey, TValue>(((TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue),
		(TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue), (TKey, TValue)) x) =>
		new([x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10,
			x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16]);

	public static implicit operator G.Dictionary<TKey, TValue>?(Dictionary<TKey, TValue>? x)
	{
		if (x is null)
			return null;
		return x._underlying
			?? throw new InvalidOperationException("Невозможно выполнить преобразование. Возможные причины:\r\n"
			+ InternalError
			+ $"Текущее состояние: длина - {x.Length},"
			+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
	}
}

internal class UnsortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
	private const string NotSupportedMethod = "Этот метод не поддерживается в этой коллекции."
		+ " Если он нужен вам, используйте Dictionary<TKey, TValue> или SortedDictionary<TKey, TValue>.";
	private protected readonly List<TKey> keys;
	private protected readonly List<TValue> values;
	[NonSerialized]
	private protected object _syncRoot = new();

	public UnsortedDictionary(G.IEnumerable<TKey> keyCollection, G.IEnumerable<TValue> valueCollection) =>
		(keys, values) = E.DistinctBy(E.Zip(keyCollection, valueCollection), x => x.First).Break();

	public UnsortedDictionary(G.IEnumerable<(TKey Key, TValue Value)> collection) =>
		(keys, values) = E.DistinctBy(collection, x => x.Key).Break();

	public UnsortedDictionary(G.IEnumerable<G.KeyValuePair<TKey, TValue>> collection) =>
		(keys, values) = E.DistinctBy(collection, x => x.Key).Break(x => x.Key, x => x.Value);

	public virtual TValue this[TKey key] => throw new NotSupportedException(NotSupportedMethod);

	TValue G.IDictionary<TKey, TValue>.this[TKey key]
	{
		get => throw new NotSupportedException(NotSupportedMethod);
		set => throw new NotSupportedException(NotSupportedMethod);
	}

	public virtual G.IEnumerable<TKey> Keys => throw new NotSupportedException(NotSupportedMethod);

	public virtual int Length => keys.Length;

	public virtual G.IEnumerable<TValue> Values =>
		throw new NotSupportedException(NotSupportedMethod);

	G.ICollection<TKey> G.IDictionary<TKey, TValue>.Keys =>
		throw new NotSupportedException(NotSupportedMethod);

	G.ICollection<TValue> G.IDictionary<TKey, TValue>.Values =>
		throw new NotSupportedException(NotSupportedMethod);

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

	public virtual bool ContainsKey(TKey key) => keys.Contains(item: key);

	public void CopyTo(Array array, int index) =>
		throw new NotSupportedException(NotSupportedMethod);

	public virtual void CopyTo(G.KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
		throw new NotSupportedException(NotSupportedMethod);

	public virtual Enumerator GetEnumerator() => new(this);

	G.IEnumerator<G.KeyValuePair<TKey, TValue>> G.IEnumerable<G.KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private protected int IndexOfKey(TKey key) => keys.IndexOf(key);

	public virtual bool Remove(TKey key) => throw new NotSupportedException(NotSupportedMethod);

	public virtual bool RemoveValue(G.KeyValuePair<TKey, TValue> item) =>
		throw new NotSupportedException(NotSupportedMethod);

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

		public G.KeyValuePair<TKey, TValue> Current { get; private set; } = default;

		readonly object IEnumerator.Current => Current;

		public void Dispose() => Reset();

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
				Current = default;
				return false;
			}
		}

		public void Reset()
		{
			index = 0;
			Current = default;
		}
	}
}
