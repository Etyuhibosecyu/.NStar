using System.Globalization;
using System.Threading;

namespace NStar.Core.Tests;

[TestClass]
public class StringTests
{
	[TestMethod]
	public void ConstructionTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var array = new String[1000];
		for (var i = 0; i < array.Length; i++)
		{
			array[i] = new[]
			{
				() => new String(), () => new(500),
				() => new(RedStarLinq.FillArray(random.Next(500), _ => (char)random.Next(1, 1000))),
				() => new(RedStarLinq.FillArray(random.Next(500), _ => (char)random.Next(1, 1000)).AsSpan()),
				() => new((G.IEnumerable<char>)RedStarLinq.FillArray(random.Next(500), _ => (char)random.Next(1, 1000))),
				() => new(RedStarLinq.Fill(random.Next(500), _ => (char)random.Next(1, 1000))),
				() => new(RedStarLinq.NFill(random.Next(500), _ => (char)random.Next(1, 1000))),
				() => new(E.Select(RedStarLinq.Fill(random.Next(500), _ => (char)random.Next(1, 1000)), x => x)),
				() => new(E.SkipWhile(RedStarLinq.Fill(random.Next(500), _ => (char)random.Next(1, 1000)), _ => random.Next(10) == -1)),
				() => new('c', 500),
				() => new(500, RedStarLinq.FillArray(random.Next(500), _ => (char)random.Next(1, 1000))),
				() => new(500, RedStarLinq.FillArray(random.Next(500), _ => (char)random.Next(1, 1000)).AsSpan()),
				() => new(500, (G.IEnumerable<char>)RedStarLinq.FillArray(random.Next(500), _ => (char)random.Next(1, 1000))),
				() => new(500, RedStarLinq.Fill(random.Next(500), _ => (char)random.Next(1, 1000))),
				() => new(500, RedStarLinq.NFill(random.Next(500), _ => (char)random.Next(1, 1000))),
				() => new(500, E.Select(RedStarLinq.Fill(random.Next(500), _ => (char)random.Next(1, 1000)), x => x)),
				() => new(500, E.SkipWhile(RedStarLinq.Fill(random.Next(500), _ => (char)random.Next(1, 1000)), _ => random.Next(10) == -1))
			}.Random(random)();
			for (var j = 0; j < 1000; j++)
			{
				array[i].Add((char)random.Next(1, 1000));
				Assert.IsTrue(array[i].Capacity >= array[i].Length);
			}
		}
		Thread.Sleep(50);
		for (var i = 0; i < array.Length; i++)
			array[i].Add((char)random.Next(1, 1000));
	}

	[TestMethod]
	public void TestAdd()
	{
		var a = nString.ToNString().Add(defaultNChar);
		var b = new string(E.ToArray(nString)) + defaultNChar;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new String(10, nString).Add(defaultNChar);
		b = new string(E.ToArray(nString)) + defaultNChar;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestAddSeries()
	{
		var a = nString.ToNString();
		a.AddSeries('X', 0);
		G.List<char> b = new(nString);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries('X', 3);
		b.AddRange(['X', 'X', 'X']);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries('X', 101);
		b.AddRange(E.Repeat('X', 101));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.AddSeries('X', -1));
		a.Replace(nString);
		a.AddSeries(index => (char)(index ^ index >> 1), 0);
		b.Clear();
		b.AddRange(nString);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(index => (char)(index ^ index >> 1), 3);
		b.AddRange(['\0', '\x1', '\x3']);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(index => (char)(index ^ index >> 1), 101);
		b.AddRange(E.Select(E.Range(0, 101), index => (char)(index ^ index >> 1)));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.AddSeries(index => (char)(index ^ index >> 1), -1));
		a.Replace(nString);
		a.AddSeries(0, index => (char)(index ^ index >> 1));
		b.Clear();
		b.AddRange(nString);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(3, index => (char)(index ^ index >> 1));
		b.AddRange(['\0', '\x1', '\x3']);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(101, index => (char)(index ^ index >> 1));
		b.AddRange(E.Select(E.Range(0, 101), index => (char)(index ^ index >> 1)));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.AddSeries(-1, index => (char)(index ^ index >> 1)));
	}

	[TestMethod]
	public void TestAppend()
	{
		var a = nString.ToNString();
		var b = a.Append(defaultNChar);
		var c = E.Append(new string(E.ToArray(nString)), defaultNChar);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestBreakFilter()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.BreakFilter(x => x != '\0', out var c);
		var d = new string(E.ToArray(nString));
		d = d.Insert(3, "$#");
		var e = E.ToList(E.Where(d, x => x != '\0'));
		var f = E.ToList(E.Where(d, x => x == '\0'));
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, b));
		Assert.IsTrue(c.Equals(f));
		Assert.IsTrue(E.SequenceEqual(f, c));
		b = a.BreakFilter(x => x is >= 'A' and <= 'Z', out c);
		e = E.ToList(E.Where(d, x => x is >= 'A' and <= 'Z'));
		f = E.ToList(E.Where(d, x => x is not (>= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, b));
		Assert.IsTrue(c.Equals(f));
		Assert.IsTrue(E.SequenceEqual(f, c));
	}

	[TestMethod]
	public void TestBreakFilterInPlace()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.BreakFilterInPlace(x => x != '\0', out var c);
		var d = new string(E.ToArray(nString));
		d = d.Insert(3, "$#");
		var e = E.ToList(E.Where(d, x => x == '\0'));
		d = new(E.ToArray(E.Where(d, x => x != '\0')));
		BaseListTests<char, String>.BreakFilterInPlaceAsserts(a, b, c, new(d), e);
		a = nString.ToNString().Insert(3, new String('$', '#'));
		b = a.BreakFilterInPlace(x => x is >= 'A' and <= 'Z', out c);
		d = new string(E.ToArray(nString));
		d = d.Insert(3, "$#");
		e = E.ToList(E.Where(d, x => x is not (>= 'A' and <= 'Z')));
		d = new(E.ToArray(E.Where(d, x => x is >= 'A' and <= 'Z')));
		BaseListTests<char, String>.BreakFilterInPlaceAsserts(a, b, c, new(d), e);
	}

	[TestMethod]
	public void TestClear()
	{
		var a = nString.ToNString();
		a.Clear(2, 4);
		var b = new G.List<char>(nString);
		for (var i = 0; i < 4; i++)
			b[2 + i] = default!;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestCompare()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var a = new String(E.Select(E.Range(0, random.Next(3, 100)), _ => Next()));
			var b = new String(a);
			var n = random.Next(0, a.Length);
			do
				b[n] = Next();
			while (b[n] == a[n]);
			Assert.AreEqual(n, a.Compare(b));
			a = new(E.Select(E.Range(0, random.Next(5, 100)), _ => Next()));
			b = new(a);
			n = random.Next(2, a.Length);
			do
				b[n] = Next();
			while (b[n] == a[n]);
			Assert.AreEqual(n - 1, a.Compare(b, n - 1));
			a = new(E.Select(E.Range(0, random.Next(5, 100)), _ => Next()));
			b = new(a);
			var length = a.Length;
			n = random.Next(2, a.Length);
			do
				b[n] = Next();
			while (b[n] == a[n]);
			int index = random.Next(2, 50), otherIndex = random.Next(2, 50);
			a.Insert(0, E.Select(E.Range(0, index), _ => Next()));
			b = b.Insert(0, E.Select(E.Range(0, otherIndex), _ => Next()));
			Assert.AreEqual(n, a.Compare(index, b, otherIndex));
			Assert.AreEqual(n, a.Compare(index, b, otherIndex, length));
		}
		char Next() => (char)random.Next(1000);
	}

	[TestMethod]
	public void TestCompareTo()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var a = new String(E.Select(E.Range(0, random.Next(2, 100)), _ => Next()));
			ProcessA(a, a);
			ProcessA(a, E.Append(a, Next()));
			ProcessA(a, E.Skip(a, 1));
			ProcessA(a, E.Prepend(a, Next()));
			ProcessA(a, E.SkipLast(a, 1));
			ProcessA(a, E.Append(E.SkipLast(a, 1), Next()));
			ProcessA(a, E.Prepend(E.Skip(a, 1), Next()));
			ProcessA(a, []);
		}
		for (var i = 0; i < 1000; i++)
		{
			var a = new String(E.Select(E.Range(0, random.Next(2, 100)), _ => Next()));
			{
				var b = new String(a);
				FullCompare(a, b);
				b = new String(E.Append(a, Next()));
				FullCompare(a, b);
				b = new String(E.Skip(a, 1));
				FullCompare(a, b);
				b = new String(E.Prepend(a, Next()));
				FullCompare(a, b);
				b = new String(E.SkipLast(b, 1));
				FullCompare(a, b);
				b = new String(E.Append(E.SkipLast(b, 1), Next()));
				FullCompare(a, b);
				b = new String(E.Prepend(E.Skip(a, 1), Next()));
				FullCompare(a, b);
				b = CreateVar(new String(), out _)!;
				FullCompare(a, b);
			}
			{
				var b = new List<char>(a);
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new List<char>(E.Append(a, Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new List<char>(E.Skip(a, 1));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new List<char>(E.Prepend(a, Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new List<char>(E.SkipLast(b, 1));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new List<char>(E.Append(E.SkipLast(b, 1), Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new List<char>(E.Prepend(E.Skip(a, 1), Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = CreateVar(new List<char>(), out _)!;
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
			}
			{
				var b = E.ToArray(a);
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = E.ToArray(E.Append(a, Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = E.ToArray(E.Skip(a, 1));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = E.ToArray(E.Prepend(a, Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = E.ToArray(E.SkipLast(b, 1));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = E.ToArray(E.Append(E.SkipLast(b, 1), Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = E.ToArray(E.Prepend(E.Skip(a, 1), Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = CreateVar(Array.Empty<char>(), out _)!;
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
			}
			{
				var b = new string(E.ToArray(a));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new string(E.ToArray(E.Append(a, Next())));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new string(E.ToArray(E.Skip(a, 1)));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new string(E.ToArray(E.Prepend(a, Next())));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new string(E.ToArray(E.SkipLast(b, 1)));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new string(E.ToArray(E.Append(E.SkipLast(b, 1), Next())));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = new string(E.ToArray(E.Prepend(E.Skip(a, 1), Next())));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
				b = CreateVar("", out _)!;
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
			}
			{
				Span<char> b = E.ToArray(a);
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(b.ToArray()), new string(E.ToArray(a))));
				b = E.ToArray(E.Append(a, Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(b.ToArray()), new string(E.ToArray(a))));
				b = E.ToArray(E.Skip(a, 1));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(b.ToArray()), new string(E.ToArray(a))));
				b = E.ToArray(E.Prepend(a, Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(b.ToArray()), new string(E.ToArray(a))));
				b = E.ToArray(E.SkipLast(a, 1));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(b.ToArray()), new string(E.ToArray(a))));
				b = E.ToArray(E.Append(E.SkipLast(a, 1), Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(b.ToArray()), new string(E.ToArray(a))));
				b = E.ToArray(E.Prepend(E.Skip(a, 1), Next()));
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(b.ToArray()), new string(E.ToArray(a))));
				b = CreateVar(Array.Empty<char>(), out _)!;
				Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(b.ToArray()), new string(E.ToArray(a))));
			}
		}
		char Next() => (char)random.Next(1000);
		void ProcessA(String a, G.IEnumerable<char> enumerable)
		{
			G.IEnumerable<char> b = new String(enumerable);
			Assert.AreEqual(-string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))), a.CompareTo(b));
			b = new List<char>(enumerable);
			Assert.AreEqual(-string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))), a.CompareTo(b));
			b = E.ToArray(enumerable);
			Assert.AreEqual(-string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))), a.CompareTo(b));
			b = new G.List<char>(enumerable);
			Assert.AreEqual(-string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))), a.CompareTo(b));
			b = new string(E.ToArray(enumerable));
			Assert.AreEqual(-string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))), a.CompareTo(b));
			b = E.Select(enumerable, x => x);
			Assert.AreEqual(-string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))), a.CompareTo(b));
			b = E.SkipWhile(enumerable, _ => random.Next(10) != -1);
			Assert.AreEqual(-string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))), a.CompareTo(b));
		}
		static void FullCompare(String a, String b)
		{
			Assert.AreEqual(a.CompareTo(b), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a))));
			Assert.AreEqual(a.CompareTo(b, false), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), false));
			Assert.AreEqual(a.CompareTo(b, true), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), true));
			Assert.AreEqual(a.CompareTo(b, new CultureInfo("en-US")), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), false, new("en-US")));
			Assert.AreEqual(a.CompareTo(b, false, new("en-US")), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), false, new("en-US")));
			Assert.AreEqual(a.CompareTo(b, true, new("en-US")), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), true, new("en-US")));
			Assert.AreEqual(a.CompareTo(b, new CultureInfo("ru-RU")), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), false, new("ru-RU")));
			Assert.AreEqual(a.CompareTo(b, false, new("ru-RU")), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), false, new("ru-RU")));
			Assert.AreEqual(a.CompareTo(b, true, new("ru-RU")), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), true, new("ru-RU")));
			Assert.AreEqual(a.CompareTo(b, new CultureInfo("es-ES")), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), false, new("es-ES")));
			Assert.AreEqual(a.CompareTo(b, false, new("es-ES")), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), false, new("es-ES")));
			Assert.AreEqual(a.CompareTo(b, true, new("es-ES")), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), true, new("es-ES")));
			Assert.AreEqual(a.CompareTo(b, CultureInfo.InvariantCulture), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), false, CultureInfo.InvariantCulture));
			Assert.AreEqual(a.CompareTo(b, false, CultureInfo.InvariantCulture), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), false, CultureInfo.InvariantCulture));
			Assert.AreEqual(a.CompareTo(b, true, CultureInfo.InvariantCulture), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), true, CultureInfo.InvariantCulture));
			Assert.AreEqual(a.CompareTo(b, StringComparison.CurrentCulture), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), StringComparison.CurrentCulture));
			Assert.AreEqual(a.CompareTo(b, StringComparison.CurrentCultureIgnoreCase), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(a.CompareTo(b, StringComparison.InvariantCulture), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), StringComparison.InvariantCulture));
			Assert.AreEqual(a.CompareTo(b, StringComparison.InvariantCultureIgnoreCase), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), StringComparison.InvariantCultureIgnoreCase));
			//Assert.AreEqual(a.CompareTo(b, StringComparison.Ordinal), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), StringComparison.Ordinal));
			Assert.AreEqual(a.CompareTo(b, StringComparison.OrdinalIgnoreCase), -string.Compare(new string(E.ToArray(b)), new string(E.ToArray(a)), StringComparison.OrdinalIgnoreCase));
		}
	}

	[TestMethod]
	public void TestConcat()
	{
		var a = nString.ToNString();
		var b = a.Concat(defaultNSCollection);
		var c = E.Concat(new string(E.ToArray(nString)), defaultNSCollection);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestContains()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var a = new String(E.Select(E.Range(0, random.Next(2, 100)), _ => Next()));
			ProcessA(a, a);
			ProcessA(a, E.Append(a, Next()));
			ProcessA(a, E.Skip(a, 1));
			ProcessA(a, E.Prepend(a, Next()));
			ProcessA(a, E.SkipLast(a, 1));
			ProcessA(a, E.Append(E.SkipLast(a, 1), Next()));
			ProcessA(a, E.Prepend(E.Skip(a, 1), Next()));
			ProcessA(a, []);
		}
		char Next() => (char)random.Next(1000);
		void ProcessA(String a, G.IEnumerable<char> enumerable)
		{
			G.IEnumerable<char> b = new String(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).Contains(new string(E.ToArray(b)), StringComparison.Ordinal), a.Contains(b));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(b)), a.Contains(E.FirstOrDefault(b)));
			b = new List<char>(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).Contains(new string(E.ToArray(b)), StringComparison.Ordinal), a.Contains(b));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(b)), a.Contains(E.FirstOrDefault(b)));
			b = E.ToArray(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).Contains(new string(E.ToArray(b)), StringComparison.Ordinal), a.Contains(b));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(b)), a.Contains(E.FirstOrDefault(b)));
			b = new G.List<char>(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).Contains(new string(E.ToArray(b)), StringComparison.Ordinal), a.Contains(b));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(b)), a.Contains(E.FirstOrDefault(b)));
			b = new string(E.ToArray(enumerable));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(new string(E.ToArray(b)), StringComparison.Ordinal), a.Contains(b));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(b)), a.Contains(E.FirstOrDefault(b)));
			b = E.Select(enumerable, x => x);
			Assert.AreEqual(new string(E.ToArray(a)).Contains(new string(E.ToArray(b)), StringComparison.Ordinal), a.Contains(b));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(b)), a.Contains(E.FirstOrDefault(b)));
			b = E.SkipWhile(enumerable, _ => random.Next(10) != -1);
			Assert.AreEqual(new string(E.ToArray(a)).Contains(new string(E.ToArray(b)), StringComparison.Ordinal), a.Contains(b));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(b)), a.Contains(E.FirstOrDefault(b)));
			var c = new String(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(c), StringComparison.InvariantCulture), a.Contains(E.FirstOrDefault(c), false));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(c), StringComparison.InvariantCultureIgnoreCase), a.Contains(E.FirstOrDefault(c), true));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(c), StringComparison.CurrentCulture), a.Contains(E.FirstOrDefault(c), StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(c), StringComparison.CurrentCultureIgnoreCase), a.Contains(E.FirstOrDefault(c), StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(c), StringComparison.InvariantCulture), a.Contains(E.FirstOrDefault(c), StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(c), StringComparison.InvariantCultureIgnoreCase), a.Contains(E.FirstOrDefault(c), StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(c), StringComparison.Ordinal), a.Contains(E.FirstOrDefault(c), StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(E.FirstOrDefault(c), StringComparison.OrdinalIgnoreCase), a.Contains(E.FirstOrDefault(c), StringComparison.OrdinalIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.InvariantCulture), a.Contains(c.AsSpan(), false));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.Contains(c.AsSpan(), true));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.CurrentCulture), a.Contains(c.AsSpan(), StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.CurrentCultureIgnoreCase), a.Contains(c.AsSpan(), StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.InvariantCulture), a.Contains(c.AsSpan(), StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.Contains(c.AsSpan(), StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.Ordinal), a.Contains(c.AsSpan(), StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.OrdinalIgnoreCase), a.Contains(c.AsSpan(), StringComparison.OrdinalIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.InvariantCulture), a.Contains(c, false));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.Contains(c, true));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.CurrentCulture), a.Contains(c, StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.CurrentCultureIgnoreCase), a.Contains(c, StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.InvariantCulture), a.Contains(c, StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.Contains(c, StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.Ordinal), a.Contains(c, StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).Contains(c.ToString(), StringComparison.OrdinalIgnoreCase), a.Contains(c, StringComparison.OrdinalIgnoreCase));
		}
	}

	[TestMethod]
	public void TestContainsAny()
	{
		var a = nString.ToNString();
		var b = a.ContainsAny(new String('P', 'D', 'M'));
		Assert.IsTrue(b);
		b = a.ContainsAny(new String('L', 'M', 'N'));
		Assert.IsTrue(b);
		b = a.ContainsAny(new String('X', 'Y', 'Z'));
		Assert.IsFalse(b);
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		var a = nString.ToNString();
		var b = a.ContainsAnyExcluding(new String('P', 'D', 'M'));
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(new String('X', 'Y', 'Z'));
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(a);
		Assert.IsFalse(b);
	}

	[TestMethod]
	public void TestConvert()
	{
		var a = nString.ToNString();
		var b = a.Convert((x, index) => (x, index));
		var c = E.Select(new string(E.ToArray(nString)), (x, index) => (x, index));
		var d = a.Convert(x => x + "A");
		var e = E.Select(new string(E.ToArray(nString)), x => x + "A");
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(d.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestCopy()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		int length, capacity;
		String a;
		String b;
		int index;
		for (var i = 0; i < 1000; i++)
		{
			length = random.Next(1, 51);
			capacity = length + random.Next(9951);
			a = new(capacity);
			for (var j = 0; j < length; j++)
				a.Add((char)random.Next(33, 126));
			b = a.Copy();
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(a, b));
			index = random.Next(a.Length);
			a[index] = (char)(a[index] + 1);
			Assert.IsFalse(RedStarLinq.Equals(a, b));
			Assert.IsFalse(E.SequenceEqual(a, b));
		}
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = nString.ToNString();
		var b = RedStarLinq.FillArray(16, x => (char)random.Next(65536));
		var c = (char[])b.Clone();
		var d = (char[])b.Clone();
		var e = (char[])b.Clone();
		a.CopyTo(b);
		new string(E.ToArray(nString)).CopyTo(c);
		a.CopyTo(d, 3);
		new string(E.ToArray(nString)).CopyTo(e.AsSpan(3));
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestEndsWith()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var a = new String(E.Select(E.Range(0, random.Next(2, 100)), _ => Next()));
			ProcessA(a, a);
			ProcessA(a, E.Append(a, Next()));
			ProcessA(a, E.Skip(a, 1));
			ProcessA(a, E.Prepend(a, Next()));
			ProcessA(a, E.SkipLast(a, 1));
			ProcessA(a, E.Append(E.SkipLast(a, 1), Next()));
			ProcessA(a, E.Prepend(E.Skip(a, 1), Next()));
			ProcessA(a, []);
		}
		char Next() => (char)random.Next(1000);
		void ProcessA(String a, G.IEnumerable<char> enumerable)
		{
			G.IEnumerable<char> b = new String(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.EndsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(E.FirstOrDefault(b)), a.EndsWith(E.FirstOrDefault(b)));
			b = new List<char>(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.EndsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(E.FirstOrDefault(b)), a.EndsWith(E.FirstOrDefault(b)));
			b = E.ToArray(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.EndsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(E.FirstOrDefault(b)), a.EndsWith(E.FirstOrDefault(b)));
			b = new G.List<char>(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.EndsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(E.FirstOrDefault(b)), a.EndsWith(E.FirstOrDefault(b)));
			b = new string(E.ToArray(enumerable));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.EndsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(E.FirstOrDefault(b)), a.EndsWith(E.FirstOrDefault(b)));
			b = E.Select(enumerable, x => x);
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.EndsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(E.FirstOrDefault(b)), a.EndsWith(E.FirstOrDefault(b)));
			b = E.SkipWhile(enumerable, _ => random.Next(10) != -1);
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.EndsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(E.FirstOrDefault(b)), a.EndsWith(E.FirstOrDefault(b)));
			var c = new String(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string([E.FirstOrDefault(c)]), StringComparison.InvariantCulture), a.EndsWith(E.FirstOrDefault(c), false));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string([E.FirstOrDefault(c)]), StringComparison.InvariantCultureIgnoreCase), a.EndsWith(E.FirstOrDefault(c), true));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string([E.FirstOrDefault(c)]), StringComparison.CurrentCulture), a.EndsWith(E.FirstOrDefault(c), StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string([E.FirstOrDefault(c)]), StringComparison.CurrentCultureIgnoreCase), a.EndsWith(E.FirstOrDefault(c), StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string([E.FirstOrDefault(c)]), StringComparison.InvariantCulture), a.EndsWith(E.FirstOrDefault(c), StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string([E.FirstOrDefault(c)]), StringComparison.InvariantCultureIgnoreCase), a.EndsWith(E.FirstOrDefault(c), StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string([E.FirstOrDefault(c)]), StringComparison.Ordinal), a.EndsWith(E.FirstOrDefault(c), StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(new string([E.FirstOrDefault(c)]), StringComparison.OrdinalIgnoreCase), a.EndsWith(E.FirstOrDefault(c), StringComparison.OrdinalIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.InvariantCulture), a.EndsWith(c.AsSpan(), false));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.EndsWith(c.AsSpan(), true));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.CurrentCulture), a.EndsWith(c.AsSpan(), StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.CurrentCultureIgnoreCase), a.EndsWith(c.AsSpan(), StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.InvariantCulture), a.EndsWith(c.AsSpan(), StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.EndsWith(c.AsSpan(), StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.Ordinal), a.EndsWith(c.AsSpan(), StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.OrdinalIgnoreCase), a.EndsWith(c.AsSpan(), StringComparison.OrdinalIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.InvariantCulture), a.EndsWith(c, false));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.EndsWith(c, true));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.CurrentCulture), a.EndsWith(c, StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.CurrentCultureIgnoreCase), a.EndsWith(c, StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.InvariantCulture), a.EndsWith(c, StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.EndsWith(c, StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.Ordinal), a.EndsWith(c, StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).EndsWith(c.ToString(), StringComparison.OrdinalIgnoreCase), a.EndsWith(c, StringComparison.OrdinalIgnoreCase));
		}
	}

	[TestMethod]
	public void TestEquals()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var a = new String(E.Select(E.Range(0, random.Next(2, 100)), _ => Next()));
			G.IEnumerable<char> b = new String(a);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new String(E.Append(a, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new String(E.Skip(a, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new String(E.Prepend(a, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new String(E.SkipLast(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new String(E.Append(E.SkipLast(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new String(E.Prepend(E.Skip(a, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = CreateVar(new String(), out _)!;
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<char>(a);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<char>(E.Append(a, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<char>(E.Skip(a, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<char>(E.Prepend(a, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<char>(E.SkipLast(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<char>(E.Append(E.SkipLast(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<char>(E.Prepend(E.Skip(a, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = CreateVar(new List<char>(), out _)!;
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(a);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.Append(a, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.Skip(a, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.Prepend(a, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.SkipLast(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.Append(E.SkipLast(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.Prepend(E.Skip(a, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = CreateVar(Array.Empty<char>(), out _)!;
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<char>(a);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<char>(E.Append(a, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<char>(E.Skip(a, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<char>(E.Prepend(a, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<char>(E.SkipLast(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<char>(E.Append(E.SkipLast(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<char>(E.Prepend(E.Skip(a, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = CreateVar(new G.List<char>(), out _)!;
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new string(E.ToArray(a));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new string(E.ToArray(E.Append(a, Next())));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new string(E.ToArray(E.Skip(a, 1)));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new string(E.ToArray(E.Prepend(a, Next())));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new string(E.ToArray(E.SkipLast(b, 1)));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new string(E.ToArray(E.Append(E.SkipLast(b, 1), Next())));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new string(E.ToArray(E.Prepend(E.Skip(a, 1), Next())));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = CreateVar("", out _)!;
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(a, x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Append(a, Next()), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Skip(a, 1), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Prepend(a, Next()), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.SkipLast(b, 1), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Append(E.SkipLast(b, 1), Next()), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Prepend(E.Skip(a, 1), Next()), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Take(a, 0), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(a, _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.Append(a, Next()), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.Skip(a, 1), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.Prepend(a, Next()), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.SkipLast(a, 1), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.Append(E.SkipLast(a, 1), Next()), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.Prepend(E.Skip(a, 1), Next()), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.TakeWhile(a, _ => random.Next(10) == -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
		}
		char Next() => (char)random.Next(1000);
	}

	[TestMethod]
	public void TestFillInPlace()
	{
		var a = nString.ToNString();
		a.FillInPlace('X', 0);
		G.List<char> b = [];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace('X', 3);
		b = ['X', 'X', 'X'];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace('X', 101);
		b = [.. E.Repeat('X', 101)];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.FillInPlace('X', -1));
		a.FillInPlace(index => (char)(index ^ index >> 1), 0);
		b = [];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(index => (char)(index ^ index >> 1), 3);
		b = ['\0', '\x1', '\x3'];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(index => (char)(index ^ index >> 1), 101);
		b = [.. E.Select(E.Range(0, 101), index => (char)(index ^ index >> 1))];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.FillInPlace(index => (char)(index ^ index >> 1), -1));
		a.FillInPlace(0, index => (char)(index ^ index >> 1));
		b = [];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(3, index => (char)(index ^ index >> 1));
		b = ['\0', '\x1', '\x3'];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(101, index => (char)(index ^ index >> 1));
		b = [.. E.Select(E.Range(0, 101), index => (char)(index ^ index >> 1))];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.FillInPlace(-1, index => (char)(index ^ index >> 1)));
	}

	[TestMethod]
	public void TestFilter()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.Filter(x => x != '\0');
		var c = new string(E.ToArray(nString));
		c = c.Insert(3, "$#");
		var d = E.Where(c, x => x != '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		b = a.Filter((x, index) => x is >= 'A' and <= 'Z' && index >= 1);
		d = E.Where(c, (x, index) => x is >= 'A' and <= 'Z' && index >= 1);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestFilterInPlace()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.FilterInPlace(x => x != '\0');
		var c = new string(E.ToArray(nString));
		c = c.Insert(3, "$#");
		c = new(E.ToArray(E.Where(c, x => x != '\0')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nString.ToNString().Insert(3, new String('$', '#'));
		b = a.FilterInPlace((x, index) => x is >= 'A' and <= 'Z' && index >= 1);
		c = new string(E.ToArray(nString));
		c = c.Insert(3, "$#");
		c = new(E.ToArray(E.Where(c, (x, index) => x is >= 'A' and <= 'Z' && index >= 1)));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestFind()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.Find(x => x == '\0');
		var c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		var d = c.Find(x => x == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = nString.ToNString().Insert(3, new String('$', '#'));
		b = a.Find(x => x is not (>= 'A' and <= 'Z'));
		c = new(nString);
		c.InsertRange(3, "$#");
		d = c.Find(x => x is not (>= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestFindAll()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.FindAll(x => x == '\0');
		var c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		var d = c.FindAll(x => x == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = nString.ToNString().Insert(3, new String('$', '#'));
		b = a.FindAll(x => x is not (>= 'A' and <= 'Z'));
		c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		d = c.FindAll(x => x is not (>= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestFindIndex()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.FindIndex(x => x == '\0');
		var c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		var d = c.FindIndex(x => x == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = nString.ToNString().Insert(3, new String('$', '#'));
		b = a.FindIndex(x => x is not (>= 'A' and <= 'Z'));
		c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		d = c.FindIndex(x => x is not (>= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestFindLast()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.FindLast(x => x == '\0');
		var c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		var d = c.FindLast(x => x == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = nString.ToNString().Insert(3, new String('$', '#'));
		b = a.FindLast(x => x is not (>= 'A' and <= 'Z'));
		c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		d = c.FindLast(x => x is not (>= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestFindLastIndex()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.FindLastIndex(x => x == '\0');
		var c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		var d = c.FindLastIndex(x => x == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = nString.ToNString().Insert(3, new String('$', '#'));
		b = a.FindLastIndex(x => x is not (>= 'A' and <= 'Z'));
		c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		d = c.FindLastIndex(x => x is not (>= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestGetAfter()
	{
		var a = nString.ToNString();
		var b = a.GetAfter('D');
		var c = "MED";
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter([]);
		c = new(E.ToArray(nString));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter(new('D', 'M'));
		c = "ED";
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetAfterLast()
	{
		var a = nString.ToNString();
		var b = a.GetAfterLast('M');
		var c = "ED";
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfterLast([]);
		c = new(E.ToArray(nString));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfterLast(new('D', 'M'));
		c = "ED";
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBefore()
	{
		var a = nString.ToNString();
		var b = a.GetBefore('D');
		var c = "MBP";
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore([]);
		c = "";
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore(new('D', 'M'));
		c = "MBP";
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeLast()
	{
		var a = nString.ToNString();
		var b = a.GetBeforeLast('M');
		var c = "MBPD";
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBeforeLast([]);
		c = "";
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBeforeLast(new('D', 'M'));
		c = "MBP";
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeSetAfter()
	{
		var a = nString.ToNString();
		var b = a.GetBeforeSetAfter('D');
		var c = "MBP";
		var d = "MED";
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeSetAfterLast()
	{
		var a = nString.ToNString();
		var b = a.GetBeforeSetAfterLast('M');
		var c = "MBPD";
		var d = "ED";
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetRange()
	{
		var a = nString.ToNString();
		var b = a.GetRange(.., true);
		b.Add(defaultNChar);
		var c = new string(E.ToArray(nString))[..nString.Length];
		c += defaultNChar;
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		new BaseIndexableTests<char, String>(a, nString, defaultNChar, defaultNSCollection).TestGetRange();
	}

	[TestMethod]
	public void TestGetSlice()
	{
		var a = nString.ToNString();
		new BaseIndexableTests<char, String>(a, nString, defaultNChar, defaultNSCollection).TestGetSlice();
	}

	[TestMethod]
	public void TestIndexOf()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var a = new String(E.Select(E.Range(0, random.Next(2, 100)), _ => Next()));
			ProcessA(a, a);
			ProcessA(a, E.Append(a, Next()));
			ProcessA(a, E.Skip(a, 1));
			ProcessA(a, E.Prepend(a, Next()));
			ProcessA(a, E.SkipLast(a, 1));
			ProcessA(a, E.Append(E.SkipLast(a, 1), Next()));
			ProcessA(a, E.Prepend(E.Skip(a, 1), Next()));
			ProcessA(a, []);
		}
		char Next() => (char)random.Next(1000);
		void ProcessA(String a, G.IEnumerable<char> enumerable)
		{
			G.IEnumerable<char> b = new String(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(new string(E.ToArray(b)), StringComparison.Ordinal), a.IndexOf(b));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(b)), a.IndexOf(E.FirstOrDefault(b)));
			b = new List<char>(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(new string(E.ToArray(b)), StringComparison.Ordinal), a.IndexOf(b));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(b)), a.IndexOf(E.FirstOrDefault(b)));
			b = E.ToArray(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(new string(E.ToArray(b)), StringComparison.Ordinal), a.IndexOf(b));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(b)), a.IndexOf(E.FirstOrDefault(b)));
			b = new G.List<char>(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(new string(E.ToArray(b)), StringComparison.Ordinal), a.IndexOf(b));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(b)), a.IndexOf(E.FirstOrDefault(b)));
			b = new string(E.ToArray(enumerable));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(new string(E.ToArray(b)), StringComparison.Ordinal), a.IndexOf(b));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(b)), a.IndexOf(E.FirstOrDefault(b)));
			b = E.Select(enumerable, x => x);
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(new string(E.ToArray(b)), StringComparison.Ordinal), a.IndexOf(b));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(b)), a.IndexOf(E.FirstOrDefault(b)));
			b = E.SkipWhile(enumerable, _ => random.Next(10) != -1);
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(new string(E.ToArray(b)), StringComparison.Ordinal), a.IndexOf(b));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(b)), a.IndexOf(E.FirstOrDefault(b)));
			var c = new String(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(c), StringComparison.InvariantCulture), a.IndexOf(E.FirstOrDefault(c), false));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(c), StringComparison.InvariantCultureIgnoreCase), a.IndexOf(E.FirstOrDefault(c), true));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(c), StringComparison.CurrentCulture), a.IndexOf(E.FirstOrDefault(c), StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(c), StringComparison.CurrentCultureIgnoreCase), a.IndexOf(E.FirstOrDefault(c), StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(c), StringComparison.InvariantCulture), a.IndexOf(E.FirstOrDefault(c), StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(c), StringComparison.InvariantCultureIgnoreCase), a.IndexOf(E.FirstOrDefault(c), StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(c), StringComparison.Ordinal), a.IndexOf(E.FirstOrDefault(c), StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(E.FirstOrDefault(c), StringComparison.OrdinalIgnoreCase), a.IndexOf(E.FirstOrDefault(c), StringComparison.OrdinalIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.InvariantCulture), a.IndexOf(c.AsSpan(), false));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.IndexOf(c.AsSpan(), true));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.CurrentCulture), a.IndexOf(c.AsSpan(), StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.CurrentCultureIgnoreCase), a.IndexOf(c.AsSpan(), StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.InvariantCulture), a.IndexOf(c.AsSpan(), StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.IndexOf(c.AsSpan(), StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.Ordinal), a.IndexOf(c.AsSpan(), StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.OrdinalIgnoreCase), a.IndexOf(c.AsSpan(), StringComparison.OrdinalIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.InvariantCulture), a.IndexOf(c, false));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.IndexOf(c, true));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.CurrentCulture), a.IndexOf(c, StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.CurrentCultureIgnoreCase), a.IndexOf(c, StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.InvariantCulture), a.IndexOf(c, StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.IndexOf(c, StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.Ordinal), a.IndexOf(c, StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).IndexOf(c.ToString(), StringComparison.OrdinalIgnoreCase), a.IndexOf(c, StringComparison.OrdinalIgnoreCase));
		}
	}

	[TestMethod]
	public void TestIndexOfAny()
	{
		var a = nString.ToNString();
		var b = a.IndexOfAny(new String('P', 'D', 'M'));
		Assert.AreEqual(0, b);
		b = a.IndexOfAny(new String('L', 'N', 'P'));
		Assert.AreEqual(2, b);
		b = a.IndexOfAny("LNP", 4);
		Assert.AreEqual(-1, b);
		b = a.IndexOfAny(new String('X', 'Y', 'Z'));
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.IndexOfAny((G.IEnumerable<char>)null!));
	}

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		var a = nString.ToNString();
		var b = a.IndexOfAnyExcluding(new String('P', 'D', 'M'));
		Assert.AreEqual(1, b);
		b = a.IndexOfAnyExcluding(new String('X', 'Y', 'Z'));
		Assert.AreEqual(0, b);
		b = a.IndexOfAnyExcluding(a);
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<char>)null!));
	}

	[TestMethod]
	public void TestJoin()
	{
		var array = new[] { "AAA", "BBB", "CCC" };
		var a = String.Join(' ', array.ToArray(x => (String)x));
		var b = string.Join(' ', array);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = String.Join(' ', array);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = String.Join(' ', (String)"AAA");
		b = string.Join(' ', "AAA");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = String.Join(' ', "AAA");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = String.Join(' ', Array.Empty<String>());
		b = string.Join(' ', Array.Empty<string>());
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = String.Join(' ', Array.Empty<string>());
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = String.Join(", ", array.ToArray(x => (String)x));
		b = string.Join(", ", array);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = String.Join(", ", array);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = String.Join(", ", (String)"AAA");
		b = string.Join(", ", "AAA");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = String.Join(' ', "AAA");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = String.Join(", ", Array.Empty<String>());
		b = string.Join(", ", Array.Empty<string>());
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = String.Join(", ", Array.Empty<string>());
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestLastIndexOf()
	{
		var a = nString.ToNString();
		var b = a.LastIndexOf('M');
		Assert.AreEqual(4, b);
		b = a.LastIndexOf('B', 2);
		Assert.AreEqual(1, b);
		b = a.LastIndexOf('B', 3, 2);
		Assert.AreEqual(-1, b);
		b = a.LastIndexOf(new String('P', 'D', 'M'));
		Assert.AreEqual(2, b);
		b = a.LastIndexOf(new String('P', 'D', 'N'));
		Assert.AreEqual(-1, b);
		b = a.LastIndexOf("ME", 3);
		Assert.AreEqual(-1, b);
		b = a.LastIndexOf("ME", 5, 4);
		Assert.AreEqual(4, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.LastIndexOf((G.IEnumerable<char>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAny()
	{
		var a = nString.ToNString();
		var b = a.LastIndexOfAny(new String('P', 'D', 'M'));
		Assert.AreEqual(6, b);
		b = a.LastIndexOfAny(new String('L', 'N', 'P'));
		Assert.AreEqual(2, b);
		b = a.LastIndexOfAny("LNE", 4);
		Assert.AreEqual(-1, b);
		b = a.LastIndexOfAny(new String('X', 'Y', 'Z'));
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.LastIndexOfAny((G.IEnumerable<char>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		var a = nString.ToNString();
		var b = a.LastIndexOfAnyExcluding(new String('P', 'D', 'M'));
		Assert.AreEqual(5, b);
		b = a.LastIndexOfAnyExcluding(new String('X', 'Y', 'Z'));
		Assert.AreEqual(6, b);
		b = a.LastIndexOfAnyExcluding(a);
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<char>)null!));
	}

	[TestMethod]
	public void TestPad()
	{
		var a = nString.ToNString();
		var b = a.Pad(10);
		var c = new string(E.ToArray(nString));
		c = c.PadLeft(8).PadRight(10);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(10, 'X');
		c = new string(E.ToArray(nString));
		c = c.PadLeft(8, 'X').PadRight(10, 'X');
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(5);
		c = new string(E.ToArray(nString));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(5, 'X');
		c = new string(E.ToArray(nString));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.Pad(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.Pad(-1, 'X'));
	}

	[TestMethod]
	public void TestPadInPlace()
	{
		var a = nString.ToNString();
		var b = a.PadInPlace(10);
		var c = new string(E.ToArray(nString));
		c = c.PadLeft(8).PadRight(10);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nString);
		b = a.PadInPlace(10, 'X');
		c = new string(E.ToArray(nString));
		c = c.PadLeft(8, 'X').PadRight(10, 'X');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nString);
		b = a.PadInPlace(5);
		c = new string(E.ToArray(nString));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nString);
		b = a.PadInPlace(5, 'X');
		c = new string(E.ToArray(nString));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadInPlace(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadInPlace(-1, 'X'));
	}

	[TestMethod]
	public void TestPadLeft()
	{
		var a = nString.ToNString();
		var b = a.PadLeft(10);
		var c = new string(E.ToArray(nString));
		c = c.PadLeft(10);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(10, 'X');
		c = new string(E.ToArray(nString));
		c = c.PadLeft(10, 'X');
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(5);
		c = new string(E.ToArray(nString));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(5, 'X');
		c = new string(E.ToArray(nString));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeft(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeft(-1, 'X'));
	}

	[TestMethod]
	public void TestPadLeftInPlace()
	{
		var a = nString.ToNString();
		var b = a.PadLeftInPlace(10);
		var c = new string(E.ToArray(nString));
		c = c.PadLeft(10);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nString);
		b = a.PadLeftInPlace(10, 'X');
		c = new string(E.ToArray(nString));
		c = c.PadLeft(10, 'X');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nString);
		b = a.PadLeftInPlace(5);
		c = new string(E.ToArray(nString));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nString);
		b = a.PadLeftInPlace(5, 'X');
		c = new string(E.ToArray(nString));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1, 'X'));
	}

	[TestMethod]
	public void TestPadRight()
	{
		var a = nString.ToNString();
		var b = a.PadRight(10);
		var c = new string(E.ToArray(nString)).PadRight(10);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(10, 'X');
		c = new string(E.ToArray(nString)).PadRight(10, 'X');
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(5);
		c = new string(E.ToArray(nString));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(5, 'X');
		c = new string(E.ToArray(nString));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRight(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRight(-1, 'X'));
	}

	[TestMethod]
	public void TestPadRightInPlace()
	{
		var a = nString.ToNString();
		var b = a.PadRightInPlace(10);
		var c = new string(E.ToArray(nString)).PadRight(10);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nString);
		b = a.PadRightInPlace(10, 'X');
		c = new string(E.ToArray(nString)).PadRight(10, 'X');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nString);
		b = a.PadRightInPlace(5);
		c = new string(E.ToArray(nString));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nString);
		b = a.PadRightInPlace(5, 'X');
		c = new string(E.ToArray(nString));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1, 'X'));
	}

	[TestMethod]
	public void TestRemove()
	{
		var a = nString.ToNString();
		var b = new String(a).RemoveEnd(6);
		var c = new string(E.ToArray(nString));
		c = c.Remove(6, 1);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new String(a).Remove(0, 1);
		c = new string(E.ToArray(nString));
		c = c.Remove(0, 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new String(a).Remove(1, nString.Length - 2);
		c = new string(E.ToArray(nString));
		c = c.Remove(1, nString.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new String(a).Remove(1, 4);
		c = new string(E.ToArray(nString));
		c = c.Remove(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new String(a).Remove(nString.Length - 5, 4);
		c = new string(E.ToArray(nString));
		c = c.Remove(nString.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new String(a).Remove(nString.Length - 5, 10 - nString.Length);
		c = new string(E.ToArray(nString));
		c = c.Remove(nString.Length - 5, 10 - nString.Length);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new String(a).Remove(-1, 6));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new String(a).Remove(nString.Length - 1, 2 - nString.Length));
		Assert.ThrowsExactly<ArgumentException>(() => b = new String(a).Remove(1, 1000));
		b = new String(a).Remove(..);
		c = "";
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new String(a).Remove(..^1);
		c = new string(E.ToArray(nString));
		c = c.Remove(0, nString.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new String(a).Remove(1..);
		c = new string(E.ToArray(nString));
		c = c.Remove(1, nString.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new String(a).Remove(1..^1);
		c = new string(E.ToArray(nString));
		c = c.Remove(1, nString.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new String(a).Remove(1..5);
		c = new string(E.ToArray(nString));
		c = c.Remove(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new String(a).Remove(^5..^1);
		c = new string(E.ToArray(nString));
		c = c.Remove(nString.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new String(a).Remove(^5..5);
		c = new string(E.ToArray(nString));
		c = c.Remove(nString.Length - 5, 10 - nString.Length);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new String(a).Remove(-1..5));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new String(a).Remove(^1..1));
		Assert.ThrowsExactly<ArgumentException>(() => b = new String(a).Remove(1..1000));
	}

	[TestMethod]
	public void TestRemoveAll()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.RemoveAll(x => x == '\0');
		var c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		var d = c.RemoveAll(x => x == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = nString.ToNString().Insert(3, new String('$', '#'));
		b = a.RemoveAll(x => x is not (>= 'A' and <= 'Z'));
		c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		d = c.RemoveAll(x => x is not (>= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestRemoveAt()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = nString.ToNString();
		for (var i = 0; i < 1000; i++)
		{
			var index = random.Next(a.Length);
			var b = new String(a).RemoveAt(index);
			var c = new string(E.ToArray(a));
			c = c.Remove(index, 1);
			Assert.IsTrue(a[..index].Equals(b[..index]));
			Assert.IsTrue(E.SequenceEqual(b[..index], a[..index]));
			Assert.IsTrue(a[(index + 1)..].Equals(b[index..]));
			Assert.IsTrue(E.SequenceEqual(b[index..], a[(index + 1)..]));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestRepeat()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var arr = RedStarLinq.FillArray(random.Next(1, 1001), _ => (char)random.Next(65536));
			var a = arr.ToNString();
			var b = E.ToList(arr);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			var n = random.Next(11);
			var c = a.Repeat(n);
			var d = new G.List<char>();
			for (var j = 0; j < n; j++)
				d.AddRange(b);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
		}
	}

	[TestMethod]
	public void TestReplace()
	{
		var a = nString.ToNString().Replace(defaultNSCollection);
		var b = defaultNSCollection;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nString.ToNString().AddRange(defaultNSCollection.AsSpan(2, 3));
		b = new string(E.ToArray(nString));
		b += RedStarLinq.ToString(E.Take(E.Skip(defaultNSCollection, 2), 3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestReplaceRange()
	{
		var a = nString.ToNString().ReplaceRange(2, 3, defaultNSCollection);
		var b = new string(E.ToArray(nString));
		b = b.Remove(2, 3);
		b = b.Insert(2, RedStarLinq.ToString(defaultNSCollection));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nString.ToNString().Insert(2, defaultNSCollection.AsSpan(2, 3));
		b = new string(E.ToArray(nString));
		b = b.Insert(2, RedStarLinq.ToString(E.Take(E.Skip(defaultNSCollection, 2), 3)));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentException>(() => a = nString.ToNString().ReplaceRange(1, 1000, defaultNChar));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => nString.ToNString().ReplaceRange(-1, 3, defaultNSCollection));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => nString.ToNString().ReplaceRange(4, -2, defaultNSCollection));
		Assert.ThrowsExactly<ArgumentNullException>(() => nString.ToNString().ReplaceRange(4, 1, null!));
	}

	[TestMethod]
	public void TestReverse()
	{
		var a = nString.ToNString().Reverse();
		var b = new G.List<char>(nString);
		b.Reverse();
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nString.ToNString().AddRange(defaultNSCollection.AsSpan(2, 3)).Reverse();
		b = new(nString);
		b.AddRange(E.Take(E.Skip(defaultNSCollection, 2), 3));
		b.Reverse();
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nString.ToNString().AddRange(defaultNSCollection.AsSpan(2, 3)).Reverse(2, 4);
		b = new(nString);
		b.AddRange(E.Take(E.Skip(defaultNSCollection, 2), 3));
		b.Reverse(2, 4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSetAll()
	{
		var a = nString.ToNString().SetAll(defaultNChar);
		var b = new G.List<char>(nString);
		for (var i = 0; i < b.Count; i++)
			b[i] = defaultNChar;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nString.ToNString().SetAll(defaultNChar, 3);
		b = new(nString);
		for (var i = 3; i < b.Count; i++)
			b[i] = defaultNChar;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nString.ToNString().SetAll(defaultNChar, 2, 4);
		b = new(nString);
		for (var i = 2; i < 6; i++)
			b[i] = defaultNChar;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nString.ToNString().SetAll(defaultNChar, ^5);
		b = new(nString);
		for (var i = b.Count - 5; i < b.Count; i++)
			b[i] = defaultNChar;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nString.ToNString().SetAll(defaultNChar, ^6..4);
		b = new(nString);
		for (var i = b.Count - 6; i < 4; i++)
			b[i] = defaultNChar;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSetOrAdd()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = nString.ToNString();
		var b = new G.List<char>(nString);
		for (var i = 0; i < 1000; i++)
		{
			var index = (int)Floor(Cbrt(random.NextDouble()) * (a.Length + 1));
			var n = (char)random.Next(1000);
			a.SetOrAdd(index, n);
			if (index < b.Count)
				b[index] = n;
			else
				b.Add(n);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
	}

	[TestMethod]
	public void TestSetRange()
	{
		var hs = defaultNSCollection.ToHashSet();
		var a = nString.ToNString().SetRange(2, hs);
		var b = new G.List<char>(nString);
		for (var i = 0; i < hs.Length; i++)
			b[i + 2] = hs[i];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentException>(() => a = nString.ToNString().SetRange(5, hs));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => nString.ToNString().SetRange(-1, hs));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => nString.ToNString().SetRange(1000, hs));
		Assert.ThrowsExactly<ArgumentNullException>(() => nString.ToNString().SetRange(4, null!));
	}

	[TestMethod]
	public void TestSkip()
	{
		var a = nString.ToNString();
		var b = a.Skip(2);
		var c = E.Skip(new string(E.ToArray(nString)), 2);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.Skip(0);
		c = E.Skip(new string(E.ToArray(nString)), 0);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.Skip(1000);
		c = E.Skip(new string(E.ToArray(nString)), 1000);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.Skip(-4);
		c = E.Skip(new string(E.ToArray(nString)), -4);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipLast()
	{
		var a = nString.ToNString();
		var b = a.SkipLast(2);
		var c = E.SkipLast(new string(E.ToArray(nString)), 2);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.SkipLast(0);
		c = E.SkipLast(new string(E.ToArray(nString)), 0);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.SkipLast(1000);
		c = E.SkipLast(new string(E.ToArray(nString)), 1000);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.SkipLast(-4);
		c = E.SkipLast(new string(E.ToArray(nString)), -4);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipWhile()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.SkipWhile(x => x != '\0');
		var c = new string(E.ToArray(nString));
		c = c.Insert(3, "$#");
		var d = new string(E.ToArray(E.SkipWhile(c, x => x != '\0')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = nString.ToNString().Insert(3, new String('$', '#'));
		b = a.SkipWhile((x, index) => x is >= 'A' and <= 'Z' || index < 1);
		c = new string(E.ToArray(nString));
		c = c.Insert(3, "$#");
		d = new(E.ToArray(E.SkipWhile(E.Skip(c, 1), x => x is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestSort()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var c = new string(new byte[256].ToArray(x => (char)random.Next(65536)));
		var a = new String(c);
		var b = new String(a).Sort(x => x);
		c = new(E.ToArray(E.OrderBy(c, x => x)));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSplit()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var chars = new byte[1000].ToArray(x => (char)random.Next(9, 39));
			var separator = (char)random.Next(9, 39);
			using var @string = new String(chars);
			var a = @string.Split(separator).ToList(x => x.ToString());
			var b = new string(chars).Split(separator);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = @string.Split(separator, StringSplitOptions.None).ToList(x => x.ToString());
			b = new string(chars).Split(separator, StringSplitOptions.None);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = @string.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList(x => x.ToString());
			b = new string(chars).Split(separator, StringSplitOptions.RemoveEmptyEntries);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		for (var i = 0; i < 10000; i++)
		{
			var chars = new byte[10000].ToArray(x => (char)random.Next(9, 39));
			var separator = new string(E.ToArray(E.Select(new byte[random.Next(2, 5)], _ => (char)random.Next(9, 39))));
			using var @string = new String(chars);
			var strings = @string.Split(separator);
			var a = strings.ToList(x => x.ToString());
			var b = new string(chars).Split(separator);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = @string.Split(separator, StringSplitOptions.None).ToList(x => x.ToString());
			b = new string(chars).Split(separator, StringSplitOptions.None);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = @string.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList(x => x.ToString());
			b = new string(chars).Split(separator, StringSplitOptions.RemoveEmptyEntries);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
	}

	[TestMethod]
	public void TestStartsWith()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var a = new String(E.Select(E.Range(0, random.Next(2, 100)), _ => Next()));
			ProcessA(a, a);
			ProcessA(a, E.Append(a, Next()));
			ProcessA(a, E.Skip(a, 1));
			ProcessA(a, E.Prepend(a, Next()));
			ProcessA(a, E.SkipLast(a, 1));
			ProcessA(a, E.Append(E.SkipLast(a, 1), Next()));
			ProcessA(a, E.Prepend(E.Skip(a, 1), Next()));
			ProcessA(a, []);
		}
		char Next() => (char)random.Next(1000);
		void ProcessA(String a, G.IEnumerable<char> enumerable)
		{
			G.IEnumerable<char> b = new String(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.StartsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(E.FirstOrDefault(b)), a.StartsWith(E.FirstOrDefault(b)));
			b = new List<char>(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.StartsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(E.FirstOrDefault(b)), a.StartsWith(E.FirstOrDefault(b)));
			b = E.ToArray(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.StartsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(E.FirstOrDefault(b)), a.StartsWith(E.FirstOrDefault(b)));
			b = new G.List<char>(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.StartsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(E.FirstOrDefault(b)), a.StartsWith(E.FirstOrDefault(b)));
			b = new string(E.ToArray(enumerable));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.StartsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(E.FirstOrDefault(b)), a.StartsWith(E.FirstOrDefault(b)));
			b = E.Select(enumerable, x => x);
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.StartsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(E.FirstOrDefault(b)), a.StartsWith(E.FirstOrDefault(b)));
			b = E.SkipWhile(enumerable, _ => random.Next(10) != -1);
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string(E.ToArray(b)), StringComparison.Ordinal), a.StartsWith(b));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(E.FirstOrDefault(b)), a.StartsWith(E.FirstOrDefault(b)));
			var c = new String(enumerable);
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string([E.FirstOrDefault(c)]), StringComparison.InvariantCulture), a.StartsWith(E.FirstOrDefault(c), false));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string([E.FirstOrDefault(c)]), StringComparison.InvariantCultureIgnoreCase), a.StartsWith(E.FirstOrDefault(c), true));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string([E.FirstOrDefault(c)]), StringComparison.CurrentCulture), a.StartsWith(E.FirstOrDefault(c), StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string([E.FirstOrDefault(c)]), StringComparison.CurrentCultureIgnoreCase), a.StartsWith(E.FirstOrDefault(c), StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string([E.FirstOrDefault(c)]), StringComparison.InvariantCulture), a.StartsWith(E.FirstOrDefault(c), StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string([E.FirstOrDefault(c)]), StringComparison.InvariantCultureIgnoreCase), a.StartsWith(E.FirstOrDefault(c), StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string([E.FirstOrDefault(c)]), StringComparison.Ordinal), a.StartsWith(E.FirstOrDefault(c), StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(new string([E.FirstOrDefault(c)]), StringComparison.OrdinalIgnoreCase), a.StartsWith(E.FirstOrDefault(c), StringComparison.OrdinalIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.InvariantCulture), a.StartsWith(c.AsSpan(), false));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.StartsWith(c.AsSpan(), true));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.CurrentCulture), a.StartsWith(c.AsSpan(), StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.CurrentCultureIgnoreCase), a.StartsWith(c.AsSpan(), StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.InvariantCulture), a.StartsWith(c.AsSpan(), StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.StartsWith(c.AsSpan(), StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.Ordinal), a.StartsWith(c.AsSpan(), StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.OrdinalIgnoreCase), a.StartsWith(c.AsSpan(), StringComparison.OrdinalIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.InvariantCulture), a.StartsWith(c, false));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.StartsWith(c, true));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.CurrentCulture), a.StartsWith(c, StringComparison.CurrentCulture));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.CurrentCultureIgnoreCase), a.StartsWith(c, StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.InvariantCulture), a.StartsWith(c, StringComparison.InvariantCulture));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.InvariantCultureIgnoreCase), a.StartsWith(c, StringComparison.InvariantCultureIgnoreCase));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.Ordinal), a.StartsWith(c, StringComparison.Ordinal));
			Assert.AreEqual(new string(E.ToArray(a)).StartsWith(c.ToString(), StringComparison.OrdinalIgnoreCase), a.StartsWith(c, StringComparison.OrdinalIgnoreCase));
		}
	}

	[TestMethod]
	public void TestTake()
	{
		var a = nString.ToNString();
		var b = a.Take(2);
		var c = E.Take(new string(E.ToArray(nString)), 2);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.Take(0);
		c = E.Take(new string(E.ToArray(nString)), 0);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.Take(1000);
		c = E.Take(new string(E.ToArray(nString)), 1000);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.Take(-4);
		c = E.Take(new string(E.ToArray(nString)), -4);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeLast()
	{
		var a = nString.ToNString();
		var b = a.TakeLast(2);
		var c = E.TakeLast(new string(E.ToArray(nString)), 2);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.TakeLast(0);
		c = E.TakeLast(new string(E.ToArray(nString)), 0);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.TakeLast(1000);
		c = E.TakeLast(new string(E.ToArray(nString)), 1000);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nString.ToNString();
		b = a.TakeLast(-4);
		c = E.TakeLast(new string(E.ToArray(nString)), -4);
		Assert.IsTrue(a.Equals(nString));
		Assert.IsTrue(E.SequenceEqual(nString, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeWhile()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.TakeWhile(x => x != '\0');
		var c = new string(E.ToArray(nString));
		c = c.Insert(3, "$#");
		var d = new string(E.ToArray(E.TakeWhile(c, x => x != '\0')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = nString.ToNString().Insert(3, new String('$', '#'));
		b = a.TakeWhile((x, index) => x is >= 'A' and <= 'Z' && index < 10);
		c = new string(E.ToArray(nString));
		c = c.Insert(3, "$#");
		d = new(E.ToArray(E.TakeWhile(E.Take(c, 10), x => x is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestToArray()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		BaseListTests<char, String>.TestToArray(() => (char)random.Next(33, 127));
	}

	[TestMethod]
	public void TestToLower()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var chars = new byte[1000].ToArray(x => (char)random.Next(32, 127));
			using var @string = new String(chars);
			var a = @string.ToLower();
			var b = new string(chars).ToLower();
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
	}

	[TestMethod]
	public void TestToUpper()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var chars = new byte[1000].ToArray(x => (char)random.Next(32, 127));
			using var @string = new String(chars);
			var a = @string.ToUpper();
			var b = new string(chars).ToUpper();
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
	}

	[TestMethod]
	public void TestTrim()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var chars = new byte[1000].ToArray(x => (char)random.Next(9, 39));
			using var @string = new String(chars);
			var a = @string.Trim();
			var b = new string(chars).Trim();
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			var @char = chars.Random(random);
			@string.Replace(chars);
			a = @string.Trim(@char);
			b = new string(chars).Trim(@char);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			chars = new byte[1000].ToArray(x => (char)random.Next('A', 'Z' + 1));
			@string.Replace(chars);
			a = @string.Trim(nString);
			b = new string(chars).Trim(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.Trim(nString.ToHashSet());
			b = new string(chars).Trim(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.Trim(E.ToArray(nString));
			b = new string(chars).Trim(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.Trim(E.ToList(nString));
			b = new string(chars).Trim(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.Trim(nSEnumerable);
			b = new string(chars).Trim(E.ToArray(nSEnumerable));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.Trim(nSEnumerable2);
			b = new string(chars).Trim(E.ToArray(nSEnumerable2));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.Trim([]);
			b = new string(chars).Trim([]);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.Trim('M', 'P', 'X');
			b = new string(chars).Trim('M', 'P', 'X');
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
	}

	[TestMethod]
	public void TestTrimEnd()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var chars = new byte[1000].ToArray(x => (char)random.Next(9, 39));
			using var @string = new String(chars);
			var a = @string.TrimEnd();
			var b = new string(chars).TrimEnd();
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			var @char = chars.Random(random);
			@string.Replace(chars);
			a = @string.TrimEnd(@char);
			b = new string(chars).TrimEnd(@char);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			chars = new byte[1000].ToArray(x => (char)random.Next('A', 'Z' + 1));
			@string.Replace(chars);
			a = @string.TrimEnd(nString);
			b = new string(chars).TrimEnd(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimEnd(nString.ToHashSet());
			b = new string(chars).TrimEnd(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimEnd(E.ToArray(nString));
			b = new string(chars).TrimEnd(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimEnd(E.ToList(nString));
			b = new string(chars).TrimEnd(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimEnd(nSEnumerable);
			b = new string(chars).TrimEnd(E.ToArray(nSEnumerable));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimEnd(nSEnumerable2);
			b = new string(chars).TrimEnd(E.ToArray(nSEnumerable2));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimEnd([]);
			b = new string(chars).TrimEnd([]);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimEnd('M', 'P', 'X');
			b = new string(chars).TrimEnd('M', 'P', 'X');
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
	}

	[TestMethod]
	public void TestTrimStart()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var chars = new byte[1000].ToArray(x => (char)random.Next(9, 39));
			using var @string = new String(chars);
			var a = @string.TrimStart();
			var b = new string(chars).TrimStart();
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			var @char = chars.Random(random);
			@string.Replace(chars);
			a = @string.TrimStart(@char);
			b = new string(chars).TrimStart(@char);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			chars = new byte[1000].ToArray(x => (char)random.Next('A', 'Z' + 1));
			@string.Replace(chars);
			a = @string.TrimStart(nString);
			b = new string(chars).TrimStart(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimStart(nString.ToHashSet());
			b = new string(chars).TrimStart(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimStart(E.ToArray(nString));
			b = new string(chars).TrimStart(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimStart(E.ToList(nString));
			b = new string(chars).TrimStart(E.ToArray(nString));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimStart(nSEnumerable);
			b = new string(chars).TrimStart(E.ToArray(nSEnumerable));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimStart(nSEnumerable2);
			b = new string(chars).TrimStart(E.ToArray(nSEnumerable2));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimStart([]);
			b = new string(chars).TrimStart([]);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			@string.Replace(chars);
			a = @string.TrimStart('M', 'P', 'X');
			b = new string(chars).TrimStart('M', 'P', 'X');
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
	}

	[TestMethod]
	public void TestTrimExcess()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		BaseListTests<char, String>.TestTrimExcess(() => (char)random.Next(33, 127));
	}

	[TestMethod]
	public void TestTrueForAll()
	{
		var a = nString.ToNString().Insert(3, new String('$', '#'));
		var b = a.TrueForAll(x => x != '\0');
		var c = new G.List<char>(nString);
		c.InsertRange(3, "$#");
		var d = c.TrueForAll(x => x != '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		b = a.TrueForAll(x => x != '\0');
		d = c.TrueForAll(x => x != '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		b = a.TrueForAll(x => new[] { x }.Length > 3);
		d = c.TrueForAll(x => new[] { x }.Length > 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestTuples()
	{
		var a = (String)('A', 'B');
		var b = new String('A', 'B');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C');
		b = new String('A', 'B', 'C');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D');
		b = new String('A', 'B', 'C', 'D');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E');
		b = new String('A', 'B', 'C', 'D', 'E');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E', 'F');
		b = new String('A', 'B', 'C', 'D', 'E', 'F');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E', 'F', 'G');
		b = new String('A', 'B', 'C', 'D', 'E', 'F', 'G');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H');
		b = new String('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I');
		b = new String('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J');
		b = new String('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K');
		b = new String('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L');
		b = new String('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M');
		b = new String('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N');
		b = new String('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O');
		b = new String('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (String)('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P');
		b = new String('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P');
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		var fullNList = b.Copy();
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char))fullNList[..1]);
		Assert.AreEqual(((char, char))fullNList[..2], ('A', 'B'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char))fullNList[..3]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char))fullNList[..2]);
		Assert.AreEqual(((char, char, char))fullNList[..3], ('A', 'B', 'C'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char))fullNList[..4]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char))fullNList[..3]);
		Assert.AreEqual(((char, char, char, char))fullNList[..4], ('A', 'B', 'C', 'D'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char))fullNList[..5]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char))fullNList[..4]);
		Assert.AreEqual(((char, char, char, char, char))fullNList[..5], ('A', 'B', 'C', 'D', 'E'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char))fullNList[..6]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char))fullNList[..5]);
		Assert.AreEqual(((char, char, char, char, char, char))fullNList[..6], ('A', 'B', 'C', 'D', 'E', 'F'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char))fullNList[..7]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char))fullNList[..6]);
		Assert.AreEqual(((char, char, char, char, char, char, char))fullNList[..7], ('A', 'B', 'C', 'D', 'E', 'F', 'G'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char))fullNList[..8]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char))fullNList[..7]);
		Assert.AreEqual(((char, char, char, char, char, char, char, char))fullNList[..8], ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char))fullNList[..9]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char))fullNList[..8]);
		Assert.AreEqual(((char, char, char, char, char, char, char, char, char))fullNList[..9], ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char))fullNList[..10]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char))fullNList[..9]);
		Assert.AreEqual(((char, char, char, char, char, char, char, char, char, char))fullNList[..10], ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char))fullNList[..11]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char, char))fullNList[..10]);
		Assert.AreEqual(((char, char, char, char, char, char, char, char, char, char, char))fullNList[..11], ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char, char))fullNList[..12]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..11]);
		Assert.AreEqual(((char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..12], ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..13]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..12]);
		Assert.AreEqual(((char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..13], ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..14]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..13]);
		Assert.AreEqual(((char, char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..14], ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..15]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..14]);
		Assert.AreEqual(((char, char, char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..15], ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O'));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..16]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((char, char, char, char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..15]);
		Assert.AreEqual(((char, char, char, char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..16], ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P'));
		Assert.ThrowsExactly<ArgumentException>(() => ((char, char, char, char, char, char, char, char, char, char, char, char, char, char, char, char))fullNList[..17]);
	}
}
