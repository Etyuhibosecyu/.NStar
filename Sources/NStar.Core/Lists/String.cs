using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NStar.Core;

[DebuggerDisplay("{ToString()}")]
[ComVisible(true)]
[Serializable]
public unsafe class String : List<char, String>, IComparable, IComparable<char[]>, IComparable<IEnumerable<char>>, IComparable<string>, IComparable<String>
{
	private protected static readonly CompareInfo CurrentCompareInfo = CompareInfo.GetCompareInfo(CultureInfo.CurrentCulture.LCID);
	private protected static readonly CompareInfo DefaultCompareInfo = CompareInfo.GetCompareInfo(CultureInfo.InvariantCulture.LCID);
	private const string CompareMessage = "Этот метод не работает в .NStar и всегда выбрасывает исключение."
			+ " Используйте strA.CompareTo(strB, ...).";
	private const string CompareRangeMessage = "Этот метод не работает в .NStar и всегда выбрасывает исключение. Используйте"
		+ " strA.GetRange(indexA, length).CompareTo(strB.GetRange(indexB, length), ...) - в нативных коллекциях, какой является"
		+ " NStar.Core.String, метод GetRange() использует арифметику указателей и работает очень быстро.";
	private const string CompareTrivialMessage = "Этот метод не работает в .NStar и всегда выбрасывает исключение."
		+ " Используйте strA.CompareTo(strB).";

	public String() : base() { }

	public String(IEnumerable<char> collection) : base(collection) { }

	public String(int capacity) : base(capacity) { }

	public String(int capacity, IEnumerable<char> collection) : base(capacity, collection) { }

	public String(int capacity, string s) : base(capacity, [.. s]) { }

	public String(int capacity, params char[] array) : base(capacity, array) { }

	public String(int capacity, ReadOnlySpan<char> span) : base(capacity, span) { }

	public String(string s) : base([.. s]) { }

	public String(params char[] array) : base(array) { }

	public String(ReadOnlySpan<char> span) : base(span) { }

	public String(char c, int length) : base(length)
	{
		for (var i = 0; i < length; i++)
			SetInternal(i, c);
		_size = length;
	}

	protected override Func<int, String> CapacityCreator { get; } = x => new(x);

	protected override Func<IEnumerable<char>, String> CollectionCreator { get; } = x => new(x);

	protected override Func<ReadOnlySpan<char>, String> SpanCreator { get; } = x => new(x);

	public virtual String AddRange(string s) => Insert(_size, s);

	[Obsolete(CompareTrivialMessage, true)]
	public static int Compare(String? strA, String? strB) =>
		throw new NotSupportedException(CompareTrivialMessage);

	[Obsolete(CompareMessage, true)]
	public static int Compare(String? strA, String? strB, bool ignoreCase) =>
		throw new NotSupportedException(CompareMessage);

	[Obsolete(CompareMessage, true)]
	public static int Compare(String? strA, String? strB, bool ignoreCase, CultureInfo culture) =>
		throw new NotSupportedException(CompareMessage);

	[Obsolete(CompareMessage, true)]
	public static int Compare(String? strA, String? strB, CultureInfo culture) =>
		throw new NotSupportedException(CompareMessage);

	[Obsolete(CompareMessage, true)]
	public static int Compare(String? strA, String? strB, CultureInfo culture, CompareOptions options) =>
		throw new NotSupportedException(CompareMessage);

	[Obsolete(CompareMessage, true)]
	public static int Compare(String? strA, String? strB, StringComparison comparisonType) =>
		throw new NotSupportedException(CompareMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length) =>
		throw new NotSupportedException(CompareRangeMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length, bool ignoreCase) =>
		throw new NotSupportedException(CompareRangeMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length, bool ignoreCase,
		CultureInfo culture) => throw new NotSupportedException(CompareRangeMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length, CultureInfo culture) =>
		throw new NotSupportedException(CompareRangeMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length, CultureInfo culture,
		CompareOptions options) => throw new NotSupportedException(CompareRangeMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length,
		StringComparison comparisonType) =>
		throw new NotSupportedException(CompareRangeMessage);

	public virtual int CompareTo(object? other) => other switch
	{
		null => 1,
		IEnumerable<char> enumerable => CompareTo(enumerable),
		IComparable<String> icns => -icns.CompareTo(this),
		IComparable<char[]> icarr => -icarr.CompareTo(ToArray()),
		IComparable<string> ics => -ics.CompareTo(ToString()),
		_ => throw new ArgumentException("Невозможно сравнить строку с этим объектом!", nameof(other)),
	};

	public virtual int CompareTo(char[]? other) => DefaultCompareInfo.Compare(AsSpan(), other ?? []);

	public virtual int CompareTo(IEnumerable<char>? other) => other switch
	{
		null => 1,
		char[] chars => CompareTo(chars.AsSpan()),
		List<char> list => CompareTo(list.AsSpan()),
		string s => CompareToNotNull(s),
		String ns => CompareTo(ns.AsSpan()),
		IComparable<String> icns => -icns.CompareTo(this),
		IComparable<char[]> icarr => -icarr.CompareTo(ToArray()),
		IComparable<string> ics => -ics.CompareTo(ToString()),
		_ => CompareToNotNull([.. other]),
	};

	public virtual int CompareTo(ReadOnlySpan<char> other) => DefaultCompareInfo.Compare(AsSpan(), other);

	public virtual int CompareTo(string? other) => DefaultCompareInfo.Compare(AsSpan(), other ?? "");

	public virtual int CompareTo(String? other) => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan());

	public virtual int CompareTo(String? other, bool ignoreCase) => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan(),
		ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);

	public virtual int CompareTo(String? other, bool ignoreCase, CultureInfo culture) =>
		CompareInfo.GetCompareInfo(culture.LCID).Compare(AsSpan(), (other ?? []).AsSpan(),
			ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);

	public virtual int CompareTo(String? other, CultureInfo culture) =>
		CompareInfo.GetCompareInfo(culture.LCID).Compare(AsSpan(), (other ?? []).AsSpan());

	[Experimental("CS9216")]
	public virtual int CompareTo(String? other, CultureInfo culture, CompareOptions options) =>
		CompareInfo.GetCompareInfo(culture.LCID).Compare(AsSpan(), (other ?? []).AsSpan(), options);

	/// <summary>
	/// WARNING!!! This methods works wrong with StringComparison.Ordinal!
	/// (But probably works right with StringComparison.OrdinalIgnoreCase.)
	/// </summary>
	/// <param name="comparisonType">NOT StringComparison.Ordinal!!!</param>
	public virtual int CompareTo(String? other, StringComparison comparisonType) => comparisonType switch
	{
		StringComparison.CurrentCulture => CurrentCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan()),
		StringComparison.CurrentCultureIgnoreCase => CurrentCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan(),
			CompareOptions.IgnoreCase),
		StringComparison.InvariantCulture => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan()),
		StringComparison.InvariantCultureIgnoreCase => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan(),
			CompareOptions.IgnoreCase),
		StringComparison.Ordinal => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan(), CompareOptions.Ordinal),
		StringComparison.OrdinalIgnoreCase => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan(),
			CompareOptions.OrdinalIgnoreCase),
		_ => throw new ArgumentException("Такой способ сравнения строк не существует!", nameof(comparisonType)),
	};

	protected virtual int CompareToNotNull([NotNull] char[] other) => DefaultCompareInfo.Compare(AsSpan(), other);

	protected virtual int CompareToNotNull([NotNull] string other) => DefaultCompareInfo.Compare(AsSpan(), other);

	public virtual bool Contains(char value, bool ignoreCase) => Contains(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool Contains(char value, StringComparison comparisonType) => AsSpan().Contains([value], comparisonType);

	public virtual bool Contains(ReadOnlySpan<char> value, bool ignoreCase) => Contains(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool Contains(ReadOnlySpan<char> value, StringComparison comparisonType) => AsSpan().Contains(value, comparisonType);

	public virtual bool Contains(String value, bool ignoreCase) => Contains(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool Contains(String value, StringComparison comparisonType) => AsSpan().Contains(value.AsSpan(), comparisonType);

	public virtual bool EndsWith(char value, bool ignoreCase) => EndsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool EndsWith(char value, StringComparison comparisonType) => AsSpan().EndsWith([value], comparisonType);

	public virtual bool EndsWith(ReadOnlySpan<char> value, bool ignoreCase) => EndsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool EndsWith(ReadOnlySpan<char> value, StringComparison comparisonType) => AsSpan().EndsWith(value, comparisonType);

	public virtual bool EndsWith(String value, bool ignoreCase) => EndsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool EndsWith(String value, StringComparison comparisonType) => AsSpan().EndsWith(value.AsSpan(), comparisonType);

	public override bool Equals(object? obj) => base.Equals(obj);

	public override int GetHashCode() => base.GetHashCode();

	public virtual int IndexOf(char value, bool ignoreCase) => IndexOf(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual int IndexOf(char value, StringComparison comparisonType) => AsSpan().IndexOf([value], comparisonType);

	public virtual int IndexOf(ReadOnlySpan<char> value, bool ignoreCase) => IndexOf(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual int IndexOf(ReadOnlySpan<char> value, StringComparison comparisonType) => AsSpan().IndexOf(value, comparisonType);

	public virtual int IndexOf(String value, bool ignoreCase) => IndexOf(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual int IndexOf(String value, StringComparison comparisonType) => AsSpan().IndexOf(value.AsSpan(), comparisonType);

	public virtual String Insert(int index, string s)
	{
		var length = s.Length;
		if (length == 0)
			return this;
		if (Capacity < _size + length)
		{
			var min = _size + length;
			var newCapacity = Max(DefaultCapacity, Capacity * 2);
			if ((uint)newCapacity > int.MaxValue)
				newCapacity = int.MaxValue;
			if (newCapacity < min)
				newCapacity = min;
			var newItems = new char[newCapacity];
			if (_items != null)
			{
				if (index > 0)
					CopyMemory(_items, 0, newItems, 0, index);
				if (index < _size)
					CopyMemory(_items, index, newItems, index + length, _size - index);
			}
			fixed (char* ptr = s)
				fixed (char* newItemsPtr = newItems)
					CopyMemory(ptr, 0, newItemsPtr, index, length);
			_items = newItems;
		}
		else
		{
			if (_items != null && index < _size)
				CopyMemory(_items, index, _items, index + length, _size - index);
			fixed (char* ptr = s)
				fixed (char* _itemsPtr = _items)
					CopyMemory(ptr, 0, _itemsPtr, index, length);
		}
		_size += length;
		Changed();
		return this;
	}

	public static String Join(char separator, IEnumerable<string> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		var en = collection.GetEnumerator();
		if (!en.MoveNext())
			return [];
		var result = en.Current.ToNString();
		while (en.MoveNext())
		{
			result.Add(separator);
			result.AddRange(en.Current);
		}
		return result;
	}

	public static String Join(String separator, IEnumerable<string> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		var en = collection.GetEnumerator();
		if (!en.MoveNext())
			return [];
		var result = en.Current.ToNString();
		while (en.MoveNext())
		{
			result.AddRange(separator);
			result.AddRange(en.Current);
		}
		return result;
	}

	public static String Join(char separator, IEnumerable<String> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		var en = collection.GetEnumerator();
		if (!en.MoveNext())
			return [];
		var result = en.Current.Copy();
		while (en.MoveNext())
		{
			result.Add(separator);
			result.AddRange(en.Current);
		}
		return result;
	}

	public static String Join(String separator, IEnumerable<String> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		var en = collection.GetEnumerator();
		if (!en.MoveNext())
			return [];
		var result = en.Current.Copy();
		while (en.MoveNext())
		{
			result.AddRange(separator);
			result.AddRange(en.Current);
		}
		return result;
	}

	public static String Join(char separator, params string[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Length == 0)
			return [];
		else if (array.Length == 1)
			return array[0];
		var result = array[0].ToNString();
		for (var i = 1; i < array.Length; i++)
		{
			result.Add(separator);
			result.AddRange(array[i]);
		}
		return result;
	}

	public static String Join(String separator, params string[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Length == 0)
			return [];
		else if (array.Length == 1)
			return array[0];
		var result = array[0].ToNString();
		for (var i = 1; i < array.Length; i++)
		{
			result.AddRange(separator);
			result.AddRange(array[i]);
		}
		return result;
	}

	public static String Join(char separator, params String[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Length == 0)
			return [];
		else if (array.Length == 1)
			return array[0];
		var result = array[0].Copy();
		for (var i = 1; i < array.Length; i++)
		{
			result.Add(separator);
			result.AddRange(array[i]);
		}
		return result;
	}

	public static String Join(String separator, params String[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Length == 0)
			return [];
		else if (array.Length == 1)
			return array[0];
		var result = array[0].Copy();
		for (var i = 1; i < array.Length; i++)
		{
			result.AddRange(separator);
			result.AddRange(array[i]);
		}
		return result;
	}

	public override String Pad(int length) => Pad(length, ' ');

	public override String PadInPlace(int length) => PadInPlace(length, ' ');

	public override String PadLeft(int length) => PadLeft(length, ' ');

	public override String PadLeftInPlace(int length) => PadLeftInPlace(length, ' ');

	public override String PadRight(int length) => PadRight(length, ' ');

	public override String PadRightInPlace(int length) => PadRightInPlace(length, ' ');

	public virtual String Replace(string s) => Replace(s.AsSpan());

	// TODO: этот метод разбиения игнорирует флаг TrimEntries в опциях. Правильное поведение этого флага в разработке.
	public virtual List<String> Split(char separator, StringSplitOptions options = StringSplitOptions.None)
	{
		if (_size == 0)
			return [];
		else if (_size == 1)
		{
			if (GetInternal(0) == separator)
				return options.HasFlag(StringSplitOptions.RemoveEmptyEntries) ? [] : [[], []];
			else
				return [GetInternal(0)];
		}
		var prevPos = 0;
		List<String> result = [];
		for (var i = 0; i < _size; i++)
			if (GetInternal(i) == separator)
			{
				if (!(prevPos == i && options.HasFlag(StringSplitOptions.RemoveEmptyEntries)))
					result.Add(GetRange(prevPos..i));
				prevPos = i + 1;
			}
		if (!(prevPos == _size && options.HasFlag(StringSplitOptions.RemoveEmptyEntries)))
			result.Add(GetRange(prevPos.._size));
		return result;
	}

	// TODO: этот метод разбиения игнорирует флаг TrimEntries в опциях. Правильное поведение этого флага в разработке.
	public virtual List<String> Split(String separator, StringSplitOptions options = StringSplitOptions.None)
	{
		if (_size == 0)
			return [];
		else if (_size < separator.Length)
			return [this];
		var prevPos = 0;
		LimitedQueue<char> queue = new(separator.Length);
		List<String> result = [];
		for (var i = 0; i < _size; i++)
		{
			queue.Enqueue(GetInternal(i));
			if (separator.Equals(queue))
			{
				if (!(prevPos >= i + 1 - queue.Length && options.HasFlag(StringSplitOptions.RemoveEmptyEntries)))
					result.Add(GetRange(prevPos..(i + 1 - queue.Length)));
				prevPos = i + 1;
				queue.Clear();
			}
		}
		if (!(prevPos == _size && options.HasFlag(StringSplitOptions.RemoveEmptyEntries)))
			result.Add(GetRange(prevPos.._size));
		return result;
	}

	public virtual bool StartsWith(char value, bool ignoreCase) => StartsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool StartsWith(char value, StringComparison comparisonType) => AsSpan().StartsWith([value], comparisonType);

	public virtual bool StartsWith(ReadOnlySpan<char> value, bool ignoreCase) => StartsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool StartsWith(ReadOnlySpan<char> value, StringComparison comparisonType) => AsSpan().StartsWith(value, comparisonType);

	public virtual bool StartsWith(String value, bool ignoreCase) => StartsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool StartsWith(String value, StringComparison comparisonType) => AsSpan().StartsWith(value.AsSpan(), comparisonType);

	public virtual String ToLower()
	{
		for (var i = 0; i < _size; i++)
			SetInternal(i, char.ToLower(GetInternal(i)));
		return this;
	}

	public override string ToString() => new(AsSpan());

	public virtual String ToUpper()
	{
		for (var i = 0; i < _size; i++)
			SetInternal(i, char.ToUpper(GetInternal(i)));
		return this;
	}

	public virtual String Trim() => TrimEnd().TrimStart();

	public virtual String Trim(char c) => TrimEnd(c).TrimStart(c);

	public virtual String Trim(IEnumerable<char> chars) => TrimEnd(chars).TrimStart(chars);

	public virtual String Trim(params char[] chars) => Trim((IEnumerable<char>)chars);

	public virtual String TrimEnd()
	{
		for (var i = _size - 1; i >= 0; i--)
			if (!char.IsWhiteSpace(GetInternal(i)))
			{
				RemoveEnd(i + 1);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimEnd(char c)
	{
		for (var i = _size - 1; i >= 0; i--)
			if (GetInternal(i) != c)
			{
				RemoveEnd(i + 1);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimEnd(IEnumerable<char> chars)
	{
		ArgumentNullException.ThrowIfNull(chars);
		if (chars is not ISet<char> set)
			set = chars.ToHashSet();
		for (var i = _size - 1; i >= 0; i--)
			if (!set.Contains(GetInternal(i)))
			{
				RemoveEnd(i + 1);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimEnd(params char[] chars) => TrimEnd((IEnumerable<char>)chars);

	public virtual String TrimStart()
	{
		for (var i = 0; i < _size; i++)
			if (!char.IsWhiteSpace(GetInternal(i)))
			{
				Remove(0, i);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimStart(char c)
	{
		for (var i = 0; i < _size; i++)
			if (GetInternal(i) != c)
			{
				Remove(0, i);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimStart(IEnumerable<char> chars)
	{
		ArgumentNullException.ThrowIfNull(chars);
		if (chars is not ISet<char> set)
			set = chars.ToHashSet();
		for (var i = 0; i < _size; i++)
			if (!set.Contains(GetInternal(i)))
			{
				Remove(0, i);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimStart(params char[] chars) => TrimStart((IEnumerable<char>)chars);

	public static bool operator ==(String? x, String? y) => x?.Equals(y) ?? y is null;

	public static bool operator ==(String? x, string? y) => x is null && y is null || x is not null && y is not null && x.Equals((String)y);

	public static bool operator ==(string? x, String? y) => x is null && y is null || x is not null && y is not null && ((String)x).Equals(y);

	public static bool operator !=(String? x, String? y) => !(x == y);

	public static bool operator !=(String? x, string? y) => !(x == y);

	public static bool operator !=(string? x, String? y) => !(x == y);

	public static implicit operator String(char x) => new(32, x);

	public static implicit operator String(char[]? x) => x == null ? [] : new(32, x);

	public static implicit operator String(string? x) => x == null ? [] : new(32, (ReadOnlySpan<char>)x);

	public static explicit operator String((char, char) x) => [x.Item1, x.Item2];

	public static explicit operator String((char, char, char) x) => [x.Item1, x.Item2, x.Item3];

	public static explicit operator String((char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4];

	public static explicit operator String((char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5];

	public static explicit operator String((char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6];

	public static explicit operator String((char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7];

	public static explicit operator String((char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8];

	public static explicit operator String((char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16];

	public static explicit operator (char, char)(String x) => x._size == 2 ? (x.GetInternal(0), x.GetInternal(1)) : throw new InvalidOperationException("Список должен иметь 2 элемента.");

	public static explicit operator (char, char, char)(String x) => x._size == 3 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2)) : throw new InvalidOperationException("Список должен иметь 3 элемента.");

	public static explicit operator (char, char, char, char)(String x) => x._size == 4 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3)) : throw new InvalidOperationException("Список должен иметь 4 элемента.");

	public static explicit operator (char, char, char, char, char)(String x) => x._size == 5 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4)) : throw new InvalidOperationException("Список должен иметь 5 элементов.");

	public static explicit operator (char, char, char, char, char, char)(String x) => x._size == 6 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5)) : throw new InvalidOperationException("Список должен иметь 6 элементов.");

	public static explicit operator (char, char, char, char, char, char, char)(String x) => x._size == 7 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6)) : throw new InvalidOperationException("Список должен иметь 7 элементов.");

	public static explicit operator (char, char, char, char, char, char, char, char)(String x) => x._size == 8 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7)) : throw new InvalidOperationException("Список должен иметь 8 элементов.");

	public static explicit operator (char, char, char, char, char, char, char, char, char)(String x) => x._size == 9 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8)) : throw new InvalidOperationException("Список должен иметь 9 элементов.");

	public static explicit operator (char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 10 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9)) : throw new InvalidOperationException("Список должен иметь 10 элементов.");

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 11 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10)) : throw new InvalidOperationException("Список должен иметь 11 элементов.");

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 12 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11)) : throw new InvalidOperationException("Список должен иметь 12 элементов.");

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 13 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12)) : throw new InvalidOperationException("Список должен иметь 13 элементов.");

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 14 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13)) : throw new InvalidOperationException("Список должен иметь 14 элементов.");

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 15 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14)) : throw new InvalidOperationException("Список должен иметь 15 элементов.");

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 16 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14), x.GetInternal(15)) : throw new InvalidOperationException("Список должен иметь 16 элементов.");
}
