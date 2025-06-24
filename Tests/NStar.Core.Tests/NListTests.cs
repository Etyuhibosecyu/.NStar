using System.Threading;

namespace NStar.Core.Tests;

[TestClass]
public class NListTests
{
	[TestMethod]
	public void ConstructionTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var array = new NList<int>[1000];
		for (var i = 0; i < array.Length; i++)
		{
			array[i] = new[] { () => new NList<int>(), () => new(500), () => new(RedStarLinq.FillArray(random.Next(500), _ => random.Next())), () => new(RedStarLinq.FillArray(random.Next(500), _ => random.Next()).AsSpan()), () => new((G.IEnumerable<int>)RedStarLinq.FillArray(random.Next(500), _ => random.Next())), () => new(RedStarLinq.Fill(random.Next(500), _ => random.Next())), () => new(RedStarLinq.NFill(random.Next(500), _ => random.Next())), () => new(E.Select(RedStarLinq.Fill(random.Next(500), _ => random.Next()), x => x)), () => new(E.SkipWhile(RedStarLinq.Fill(random.Next(500), _ => random.Next()), _ => random.Next(10) == -1)), () => new(500, RedStarLinq.FillArray(random.Next(500), _ => random.Next())), () => new(500, RedStarLinq.FillArray(random.Next(500), _ => random.Next()).AsSpan()), () => new(500, (G.IEnumerable<int>)RedStarLinq.FillArray(random.Next(500), _ => random.Next())), () => new(500, RedStarLinq.Fill(random.Next(500), _ => random.Next())), () => new(500, RedStarLinq.NFill(random.Next(500), _ => random.Next())), () => new(500, E.Select(RedStarLinq.Fill(random.Next(500), _ => random.Next()), x => x)), () => new(500, E.SkipWhile(RedStarLinq.Fill(random.Next(500), _ => random.Next()), _ => random.Next(10) == -1)) }.Random(random)();
			for (var j = 0; j < 1000; j++)
			{
				array[i].Add(random.Next());
				Assert.IsTrue(array[i].Capacity >= array[i].Length);
			}
		}
		Thread.Sleep(50);
		for (var i = 0; i < array.Length; i++)
			array[i].Add(random.Next());
	}

	[TestMethod]
	public void TestAdd()
	{
		var a = nList.ToNList().Add(defaultNString);
		var b = new G.List<(char, char, char)>(nList) { defaultNString };
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new NList<(char, char, char)>(10, nList).Add(defaultNString);
		b = [.. nList, defaultNString];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestAddSeries()
	{
		var a = nList.ToNList();
		a.AddSeries(('X', 'X', 'X'), 0);
		G.List<(char, char, char)> b = [.. nList];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(('X', 'X', 'X'), 3);
		b.AddRange([('X', 'X', 'X'), ('X', 'X', 'X'), ('X', 'X', 'X')]);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(('X', 'X', 'X'), 101);
		b.AddRange(E.Repeat(('X', 'X', 'X'), 101));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.AddSeries(('X', 'X', 'X'), -1));
		a.Replace(nList);
		a.AddSeries(index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList(), 0);
		b.Clear();
		b.AddRange(nList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList(), 3);
		b.AddRange([('0', '0', '0'), ('0', '0', '1'), ('0', '0', '3')]);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList(), 101);
		b.AddRange(E.Select(E.Range(0, 101), index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList()));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.AddSeries(index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList(), -1));
		a.Replace(nList);
		a.AddSeries(0, index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList());
		b.Clear();
		b.AddRange(nList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(3, index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList());
		b.AddRange([('0', '0', '0'), ('0', '0', '1'), ('0', '0', '3')]);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(101, index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList());
		b.AddRange(E.Select(E.Range(0, 101), index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList()));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.AddSeries(-1, index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList()));
	}

	[TestMethod]
	public void TestAppend()
	{
		var a = nList.ToNList();
		var b = a.Append(defaultNString);
		var c = E.Append([.. nList], defaultNString);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestBreakFilter()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.BreakFilter(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0', out var c);
		var d = new G.List<(char, char, char)>(nList);
		d.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var e = E.ToList(E.Where(d, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
		var f = E.ToList(E.Where(d, x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0'));
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, b));
		Assert.IsTrue(c.Equals(f));
		Assert.IsTrue(E.SequenceEqual(f, c));
		b = a.BreakFilter(x => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'), out c);
		e = E.ToList(E.Where(d, x => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z')));
		f = E.ToList(E.Where(d, x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z')));
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
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.BreakFilterInPlace(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0', out var c);
		var d = new G.List<(char, char, char)>(nList);
		d.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var e = E.ToList(E.Where(d, x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0'));
		d = E.ToList(E.Where(d, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
		BaseListTests<(char, char, char), NList<(char, char, char)>>.BreakFilterInPlaceAsserts(a, b, c, d, e);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.BreakFilterInPlace(x => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'), out c);
		d = [.. nList];
		d.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		e = E.ToList(E.Where(d, x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z')));
		d = E.ToList(E.Where(d, x => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z')));
		BaseListTests<(char, char, char), NList<(char, char, char)>>.BreakFilterInPlaceAsserts(a, b, c, d, e);
	}

	[TestMethod]
	public void TestClear()
	{
		var a = nList.ToNList();
		a.Clear(2, 4);
		var b = new G.List<(char, char, char)>(nList);
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
			var a = new NList<(char, char, char)>(E.Select(E.Range(0, random.Next(3, 100)), _ => Next()));
			var b = new NList<(char, char, char)>(a);
			var n = random.Next(0, a.Length);
			do
				b[n] = Next();
			while (b[n] == a[n]);
			Assert.AreEqual(n, a.Compare(b));
			a = new(E.Select(E.Range(0, random.Next(5, 100)), _ => Next()));
			b = [.. a];
			n = random.Next(2, a.Length);
			do
				b[n] = Next();
			while (b[n] == a[n]);
			Assert.AreEqual(n - 1, a.Compare(b, n - 1));
			a = new(E.Select(E.Range(0, random.Next(5, 100)), _ => Next()));
			b = [.. a];
			var length = a.Length;
			n = random.Next(2, a.Length);
			do
				b[n] = Next();
			while (b[n] == a[n]);
			int index = random.Next(2, 50), otherIndex = random.Next(2, 50);
			a.Insert(0, E.Select(E.Range(0, index), _ => Next()));
			b.Insert(0, E.Select(E.Range(0, otherIndex), _ => Next()));
			Assert.AreEqual(n, a.Compare(index, b, otherIndex));
			Assert.AreEqual(n, a.Compare(index, b, otherIndex, length));
		}
		(char, char, char) Next() => ((char, char, char))RedStarLinq.ToList(random.Next(1000).ToString("D3"));
	}

	[TestMethod]
	public void TestConcat()
	{
		var a = nList.ToNList();
		var b = a.Concat(defaultNCollection);
		var c = E.Concat(nList, defaultNCollection);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestContains()
	{
		var a = nList.ToNList();
		var b = a.Contains(('M', 'M', 'M'));
		Assert.IsTrue(b);
		b = a.Contains(('B', 'B', 'B'), 2);
		Assert.IsFalse(b);
		b = a.Contains(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.IsTrue(b);
		b = a.Contains(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('N', 'N', 'N')));
		Assert.IsFalse(b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.Contains((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestContainsAny()
	{
		var a = nList.ToNList();
		var b = a.ContainsAny(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.IsTrue(b);
		b = a.ContainsAny(new NList<(char, char, char)>(('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N')));
		Assert.IsTrue(b);
		b = a.ContainsAny(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.IsFalse(b);
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		var a = nList.ToNList();
		var b = a.ContainsAnyExcluding(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(a);
		Assert.IsFalse(b);
	}

	[TestMethod]
	public void TestConvert()
	{
		var a = nList.ToNList();
		var b = a.Convert((x, index) => (x, index));
		var c = E.Select(nList, (x, index) => (x, index));
		var d = a.Convert(x => x + "A");
		var e = E.Select(nList, x => x + "A");
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
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
		NList<(char, char, char)> a;
		NList<(char, char, char)> b;
		int index;
		for (var i = 0; i < 1000; i++)
		{
			length = random.Next(1, 51);
			capacity = length + random.Next(9951);
			a = new(capacity);
			for (var j = 0; j < length; j++)
				a.Add(((char)random.Next(33, 126), (char)random.Next(33, 126), (char)random.Next(33, 126)));
			b = a.Copy();
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(a, b));
			index = random.Next(a.Length);
			a[index] = ((char)(a[index].Item1 + 1), (char)(a[index].Item2 + 1), (char)(a[index].Item3 + 1));
			Assert.IsFalse(RedStarLinq.Equals(a, b));
			Assert.IsFalse(E.SequenceEqual(a, b));
		}
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = nList.ToNList();
		var b = RedStarLinq.FillArray(16, x => ((char)random.Next(65536), (char)random.Next(65536), (char)random.Next(65536)));
		var c = ((char, char, char)[])b.Clone();
		var d = ((char, char, char)[])b.Clone();
		var e = ((char, char, char)[])b.Clone();
		a.CopyTo(b);
		new G.List<(char, char, char)>(nList).CopyTo(c);
		a.CopyTo(d, 3);
		new G.List<(char, char, char)>(nList).CopyTo(e, 3);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestEndsWith()
	{
		var a = nList.ToNList();
		var b = a.EndsWith(('D', 'D', 'D'));
		Assert.IsTrue(b);
		b = a.EndsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('E', 'E', 'E'), ('D', 'D', 'D')));
		Assert.IsTrue(b);
		b = a.EndsWith(new NList<(char, char, char)>(('P', 'P', 'P'), ('E', 'E', 'E'), ('D', 'D', 'D')));
		Assert.IsFalse(b);
		b = a.EndsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('E', 'E', 'E'), ('N', 'N', 'N')));
		Assert.IsFalse(b);
	}

	[TestMethod]
	public void TestEquals()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var a = new NList<(char, char, char)>(E.Select(E.Range(0, random.Next(2, 100)), _ => Next()));
			G.IEnumerable<(char, char, char)> b = [.. a];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Append(a, Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Skip(a, 1)];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Prepend(a, Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.SkipLast(b, 1)];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Append(E.SkipLast(b, 1), Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Prepend(E.Skip(a, 1), Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = CreateVar(new NList<(char, char, char)>(), out _)!;
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. a];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Append(a, Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Skip(a, 1)];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Prepend(a, Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.SkipLast(b, 1)];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Append(E.SkipLast(b, 1), Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Prepend(E.Skip(a, 1), Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = CreateVar(new List<(char, char, char)>(), out _)!;
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
			b = CreateVar(Array.Empty<(char, char, char)>(), out _)!;
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. a];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Append(a, Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Skip(a, 1)];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Prepend(a, Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.SkipLast(b, 1)];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Append(E.SkipLast(b, 1), Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [.. E.Prepend(E.Skip(a, 1), Next())];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = CreateVar(new G.List<(char, char, char)>(), out _)!;
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
		(char, char, char) Next() => ((char, char, char))RedStarLinq.ToList(random.Next(1000).ToString("D3"));
	}

	[TestMethod]
	public void TestFillInPlace()
	{
		var a = nList.ToNList();
		a.FillInPlace(('X', 'X', 'X'), 0);
		G.List<(char, char, char)> b = [];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(('X', 'X', 'X'), 3);
		b = [('X', 'X', 'X'), ('X', 'X', 'X'), ('X', 'X', 'X')];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(('X', 'X', 'X'), 101);
		b = [.. E.Repeat(('X', 'X', 'X'), 101)];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.FillInPlace(('X', 'X', 'X'), -1));
		a.FillInPlace(index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList(), 0);
		b = [];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList(), 3);
		b = [('0', '0', '0'), ('0', '0', '1'), ('0', '0', '3')];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList(), 101);
		b = [.. E.Select(E.Range(0, 101), index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList())];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.FillInPlace(index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList(), -1));
		a.FillInPlace(0, index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList());
		b = [];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(3, index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList());
		b = [('0', '0', '0'), ('0', '0', '1'), ('0', '0', '3')];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(101, index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList());
		b = [.. E.Select(E.Range(0, 101), index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList())];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.FillInPlace(-1, index => ((char, char, char))(index ^ index >> 1).ToString("D3").ToNList()));
	}

	[TestMethod]
	public void TestFilter()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.Filter(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = E.Where(c, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		b = a.Filter((x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		d = E.Where(c, (x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestFilterInPlace()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.FilterInPlace(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		c = E.ToList(E.Where(c, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.FilterInPlace((x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		c = [.. nList];
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		c = E.ToList(E.Where(c, (x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') && index >= 1));
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
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.Find(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.Find(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.Find(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = [.. nList];
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.Find(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestFindAll()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.FindAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.FindAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.FindAll(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = [.. nList];
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.FindAll(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestFindIndex()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.FindIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.FindIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.FindIndex(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = [.. nList];
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.FindIndex(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestFindLast()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.FindLast(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.FindLast(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.FindLast(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = [.. nList];
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.FindLast(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestFindLastIndex()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.FindLastIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.FindLastIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.FindLastIndex(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = [.. nList];
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.FindLastIndex(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestGetAfter()
	{
		var a = nList.ToNList();
		var b = a.GetAfter(('D', 'D', 'D'));
		var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('E', 'E', 'E'), ('D', 'D', 'D') };
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter([]);
		c = [.. nList];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter(new(('D', 'D', 'D'), ('M', 'M', 'M')));
		c = [('E', 'E', 'E'), ('D', 'D', 'D')];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetAfterLast()
	{
		var a = nList.ToNList();
		var b = a.GetAfterLast(('M', 'M', 'M'));
		var c = new G.List<(char, char, char)>() { ('E', 'E', 'E'), ('D', 'D', 'D') };
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfterLast([]);
		c = [];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfterLast(new(('D', 'D', 'D'), ('M', 'M', 'M')));
		c = [('E', 'E', 'E'), ('D', 'D', 'D')];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBefore()
	{
		var a = nList.ToNList();
		var b = a.GetBefore(('D', 'D', 'D'));
		var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P') };
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore([]);
		c = [];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore(new(('D', 'D', 'D'), ('M', 'M', 'M')));
		c = [('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P')];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeLast()
	{
		var a = nList.ToNList();
		var b = a.GetBeforeLast(('M', 'M', 'M'));
		var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P'), ('D', 'D', 'D') };
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBeforeLast([]);
		c = [.. nList];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBeforeLast(new(('D', 'D', 'D'), ('M', 'M', 'M')));
		c = [('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P')];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeSetAfter()
	{
		var a = nList.ToNList();
		var b = a.GetBeforeSetAfter(('D', 'D', 'D'));
		var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P') };
		var d = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('E', 'E', 'E'), ('D', 'D', 'D') };
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeSetAfterLast()
	{
		var a = nList.ToNList();
		var b = a.GetBeforeSetAfterLast(('M', 'M', 'M'));
		var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P'), ('D', 'D', 'D') };
		var d = new G.List<(char, char, char)>() { ('E', 'E', 'E'), ('D', 'D', 'D') };
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetRange()
	{
		var a = nList.ToNList();
		var b = a.GetRange(.., true);
		b.Add(defaultNString);
		var c = new G.List<(char, char, char)>(nList).GetRange(0, nList.Length);
		c.Add(defaultNString);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		new BaseIndexableTests<(char, char, char), NList<(char, char, char)>>(a, nList, defaultNString, defaultNCollection).TestGetRange();
	}

	[TestMethod]
	public void TestGetSlice()
	{
		var a = nList.ToNList();
		new BaseIndexableTests<(char, char, char), NList<(char, char, char)>>(a, nList, defaultNString, defaultNCollection).TestGetSlice();
	}

	[TestMethod]
	public void TestIndexOf()
	{
		var a = nList.ToNList();
		var b = a.IndexOf(('M', 'M', 'M'));
		Assert.AreEqual(0, b);
		b = a.IndexOf(('B', 'B', 'B'), 2);
		Assert.AreEqual(-1, b);
		b = a.IndexOf(('B', 'B', 'B'), 1, 2);
		Assert.AreEqual(1, b);
		b = a.IndexOf(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(2, b);
		b = a.IndexOf(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('N', 'N', 'N')));
		Assert.AreEqual(-1, b);
		b = a.IndexOf(new[] { ('M', 'M', 'M'), ('E', 'E', 'E') }, 4);
		Assert.AreEqual(4, b);
		b = a.IndexOf(new[] { ('M', 'M', 'M'), ('E', 'E', 'E') }, 0, 4);
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.IndexOf((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestIndexOfAny()
	{
		var a = nList.ToNList();
		var b = a.IndexOfAny(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(0, b);
		b = a.IndexOfAny(new NList<(char, char, char)>(('L', 'L', 'L'), ('N', 'N', 'N'), ('P', 'P', 'P')));
		Assert.AreEqual(2, b);
		b = a.IndexOfAny(new[] { ('L', 'L', 'L'), ('N', 'N', 'N'), ('P', 'P', 'P') }, 4);
		Assert.AreEqual(-1, b);
		b = a.IndexOfAny(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.IndexOfAny((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		var a = nList.ToNList();
		var b = a.IndexOfAnyExcluding(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(1, b);
		b = a.IndexOfAnyExcluding(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.AreEqual(0, b);
		b = a.IndexOfAnyExcluding(a);
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestLastIndexOf()
	{
		var a = nList.ToNList();
		var b = a.LastIndexOf(('M', 'M', 'M'));
		Assert.AreEqual(4, b);
		b = a.LastIndexOf(('B', 'B', 'B'), 2);
		Assert.AreEqual(1, b);
		b = a.LastIndexOf(('B', 'B', 'B'), 3, 2);
		Assert.AreEqual(-1, b);
		b = a.LastIndexOf(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(2, b);
		b = a.LastIndexOf(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('N', 'N', 'N')));
		Assert.AreEqual(-1, b);
		b = a.LastIndexOf(new[] { ('M', 'M', 'M'), ('E', 'E', 'E') }, 3);
		Assert.AreEqual(-1, b);
		b = a.LastIndexOf(new[] { ('M', 'M', 'M'), ('E', 'E', 'E') }, 5, 4);
		Assert.AreEqual(4, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.LastIndexOf((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAny()
	{
		var a = nList.ToNList();
		var b = a.LastIndexOfAny(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(6, b);
		b = a.LastIndexOfAny(new NList<(char, char, char)>(('L', 'L', 'L'), ('N', 'N', 'N'), ('P', 'P', 'P')));
		Assert.AreEqual(2, b);
		b = a.LastIndexOfAny(new[] { ('L', 'L', 'L'), ('N', 'N', 'N'), ('E', 'E', 'E') }, 4);
		Assert.AreEqual(-1, b);
		b = a.LastIndexOfAny(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.LastIndexOfAny((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		var a = nList.ToNList();
		var b = a.LastIndexOfAnyExcluding(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(5, b);
		b = a.LastIndexOfAnyExcluding(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.AreEqual(6, b);
		b = a.LastIndexOfAnyExcluding(a);
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestPad()
	{
		var a = nList.ToNList();
		var b = a.Pad(10);
		var c = new G.List<(char, char, char)>(nList);
		c.Insert(0, default!);
		c.Add(default!);
		c.Add(default!);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(10, ('X', 'X', 'X'));
		c = [.. nList];
		c.Insert(0, ('X', 'X', 'X'));
		c.Add(('X', 'X', 'X'));
		c.Add(('X', 'X', 'X'));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(5);
		c = [.. nList];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(5, ('X', 'X', 'X'));
		c = [.. nList];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.Pad(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.Pad(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestPadInPlace()
	{
		var a = nList.ToNList();
		var b = a.PadInPlace(10);
		var c = new G.List<(char, char, char)>(nList);
		c.Insert(0, default!);
		c.Add(default!);
		c.Add(default!);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.PadInPlace(10, ('X', 'X', 'X'));
		c = [.. nList];
		c.Insert(0, ('X', 'X', 'X'));
		c.Add(('X', 'X', 'X'));
		c.Add(('X', 'X', 'X'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.PadInPlace(5);
		c = [.. nList];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.PadInPlace(5, ('X', 'X', 'X'));
		c = [.. nList];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadInPlace(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadInPlace(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestPadLeft()
	{
		var a = nList.ToNList();
		var b = a.PadLeft(10);
		var c = new G.List<(char, char, char)>(nList);
		c.Insert(0, default!);
		c.Insert(0, default!);
		c.Insert(0, default!);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(10, ('X', 'X', 'X'));
		c = [.. nList];
		c.Insert(0, ('X', 'X', 'X'));
		c.Insert(0, ('X', 'X', 'X'));
		c.Insert(0, ('X', 'X', 'X'));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(5);
		c = [.. nList];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(5, ('X', 'X', 'X'));
		c = [.. nList];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeft(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeft(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestPadLeftInPlace()
	{
		var a = nList.ToNList();
		var b = a.PadLeftInPlace(10);
		var c = new G.List<(char, char, char)>(nList);
		c.Insert(0, default!);
		c.Insert(0, default!);
		c.Insert(0, default!);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.PadLeftInPlace(10, ('X', 'X', 'X'));
		c = [.. nList];
		c.Insert(0, ('X', 'X', 'X'));
		c.Insert(0, ('X', 'X', 'X'));
		c.Insert(0, ('X', 'X', 'X'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.PadLeftInPlace(5);
		c = [.. nList];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.PadLeftInPlace(5, ('X', 'X', 'X'));
		c = [.. nList];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestPadRight()
	{
		var a = nList.ToNList();
		var b = a.PadRight(10);
		var c = new G.List<(char, char, char)>(nList) { default!, default!, default! };
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(10, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList) { ('X', 'X', 'X'), ('X', 'X', 'X'), ('X', 'X', 'X') };
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(5);
		c = [.. nList];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(5, ('X', 'X', 'X'));
		c = [.. nList];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRight(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRight(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestPadRightInPlace()
	{
		var a = nList.ToNList();
		var b = a.PadRightInPlace(10);
		var c = new G.List<(char, char, char)>(nList) { default!, default!, default! };
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.PadRightInPlace(10, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList) { ('X', 'X', 'X'), ('X', 'X', 'X'), ('X', 'X', 'X') };
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.PadRightInPlace(5);
		c = [.. nList];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.PadRightInPlace(5, ('X', 'X', 'X'));
		c = [.. nList];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestRemove()
	{
		var a = nList.ToNList();
		var b = new NList<(char, char, char)>(a).RemoveEnd(6);
		var c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(6, 1);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(0, 1);
		c = [.. nList];
		c.RemoveRange(0, 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(1, nList.Length - 2);
		c = [.. nList];
		c.RemoveRange(1, nList.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(1, 4);
		c = [.. nList];
		c.RemoveRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(nList.Length - 5, 4);
		c = [.. nList];
		c.RemoveRange(nList.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(nList.Length - 5, 10 - nList.Length);
		c = [.. nList];
		c.RemoveRange(nList.Length - 5, 10 - nList.Length);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new NList<(char, char, char)>(a).Remove(-1, 6));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new NList<(char, char, char)>(a).Remove(nList.Length - 1, 2 - nList.Length));
		Assert.ThrowsExactly<ArgumentException>(() => b = new NList<(char, char, char)>(a).Remove(1, 1000));
		b = new NList<(char, char, char)>(a).Remove(..);
		c = [];
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(..^1);
		c = [.. nList];
		c.RemoveRange(0, nList.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(1..);
		c = [.. nList];
		c.RemoveRange(1, nList.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(1..^1);
		c = [.. nList];
		c.RemoveRange(1, nList.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(1..5);
		c = [.. nList];
		c.RemoveRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(^5..^1);
		c = [.. nList];
		c.RemoveRange(nList.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(^5..5);
		c = [.. nList];
		c.RemoveRange(nList.Length - 5, 10 - nList.Length);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new NList<(char, char, char)>(a).Remove(-1..5));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new NList<(char, char, char)>(a).Remove(^1..1));
		Assert.ThrowsExactly<ArgumentException>(() => b = new NList<(char, char, char)>(a).Remove(1..1000));
	}

	[TestMethod]
	public void TestRemoveAll()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.RemoveAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.RemoveAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.RemoveAll(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = [.. nList];
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.RemoveAll(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestRemoveAt()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = nList.ToNList();
		for (var i = 0; i < 1000; i++)
		{
			var index = random.Next(a.Length);
			var b = new NList<(char, char, char)>(a).RemoveAt(index);
			var c = new G.List<(char, char, char)>(a);
			c.RemoveAt(index);
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
			var arr = RedStarLinq.FillArray(random.Next(1, 1001), _ => random.Next());
			var a = arr.ToNList();
			var b = E.ToList(arr);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			var n = random.Next(11);
			var c = a.Repeat(n);
			var d = new G.List<int>();
			for (var j = 0; j < n; j++)
				d.AddRange(b);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
		}
	}

	[TestMethod]
	public void TestReplace()
	{
		var a = nList.ToNList().Replace(defaultNCollection);
		var b = new G.List<(char, char, char)>(nList);
		b.Clear();
		b.AddRange(defaultNCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().AddRange(defaultNCollection.AsSpan(2, 3));
		b = [.. nList];
		b.AddRange(E.Take(E.Skip(defaultNCollection, 2), 3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestReplaceRange()
	{
		var a = nList.ToNList().ReplaceRange(2, 3, defaultNCollection);
		var b = new G.List<(char, char, char)>(nList);
		b.RemoveRange(2, 3);
		b.InsertRange(2, defaultNCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().Insert(2, defaultNCollection.AsSpan(2, 3));
		b = [.. nList];
		b.InsertRange(2, E.Take(E.Skip(defaultNCollection, 2), 3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentException>(() => a = nList.ToNList().ReplaceRange(1, 1000, defaultNString));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => nList.ToNList().ReplaceRange(-1, 3, defaultNCollection));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => nList.ToNList().ReplaceRange(4, -2, defaultNCollection));
		Assert.ThrowsExactly<ArgumentNullException>(() => nList.ToNList().ReplaceRange(4, 1, null!));
	}

	[TestMethod]
	public void TestReverse()
	{
		var a = nList.ToNList().Reverse();
		var b = new G.List<(char, char, char)>(nList);
		b.Reverse();
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().AddRange(defaultNCollection.AsSpan(2, 3)).Reverse();
		b = [.. nList];
		b.AddRange(E.Take(E.Skip(defaultNCollection, 2), 3));
		b.Reverse();
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().AddRange(defaultNCollection.AsSpan(2, 3)).Reverse(2, 4);
		b = [.. nList];
		b.AddRange(E.Take(E.Skip(defaultNCollection, 2), 3));
		b.Reverse(2, 4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSetAll()
	{
		var a = nList.ToNList().SetAll(defaultNString);
		var b = new G.List<(char, char, char)>(nList);
		for (var i = 0; i < b.Count; i++)
			b[i] = defaultNString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().SetAll(defaultNString, 3);
		b = [.. nList];
		for (var i = 3; i < b.Count; i++)
			b[i] = defaultNString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().SetAll(defaultNString, 2, 4);
		b = [.. nList];
		for (var i = 2; i < 6; i++)
			b[i] = defaultNString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().SetAll(defaultNString, ^5);
		b = [.. nList];
		for (var i = b.Count - 5; i < b.Count; i++)
			b[i] = defaultNString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().SetAll(defaultNString, ^6..4);
		b = [.. nList];
		for (var i = b.Count - 6; i < 4; i++)
			b[i] = defaultNString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSetOrAdd()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = nList.ToNList();
		var b = new G.List<(char, char, char)>(nList);
		for (var i = 0; i < 1000; i++)
		{
			var index = (int)Floor(Cbrt(random.NextDouble()) * (a.Length + 1));
			var nString = random.Next(1000).ToString("D3");
			var n = (nString[0], nString[1], nString[2]);
			a.SetOrAdd(index, n);
			if (index < b.Count)
				b[index] = n;
			else
				b.Add(n);
		}
	}

	[TestMethod]
	public void TestSetRange()
	{
		var hs = defaultNCollection.ToHashSet();
		var a = nList.ToNList().SetRange(2, hs);
		var b = new G.List<(char, char, char)>(nList);
		for (var i = 0; i < hs.Length; i++)
			b[i + 2] = hs[i];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentException>(() => a = nList.ToNList().SetRange(5, hs));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => nList.ToNList().SetRange(-1, hs));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => nList.ToNList().SetRange(1000, hs));
		Assert.ThrowsExactly<ArgumentNullException>(() => nList.ToNList().SetRange(4, null!));
	}

	[TestMethod]
	public void TestSkip()
	{
		var a = nList.ToNList();
		var b = a.Skip(2);
		var c = E.Skip(nList, 2);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Skip(0);
		c = E.Skip(nList, 0);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Skip(1000);
		c = E.Skip(nList, 1000);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Skip(-4);
		c = E.Skip(nList, -4);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipLast()
	{
		var a = nList.ToNList();
		var b = a.SkipLast(2);
		var c = E.SkipLast(nList, 2);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.SkipLast(0);
		c = E.SkipLast(nList, 0);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.SkipLast(1000);
		c = E.SkipLast(nList, 1000);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.SkipLast(-4);
		c = E.SkipLast(nList, -4);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipWhile()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.SkipWhile(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = E.ToList(E.SkipWhile(c, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.SkipWhile((x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') || index < 1);
		c = [.. nList];
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = E.ToList(E.SkipWhile(E.Skip(c, 1), x => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestSort()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var c = new G.List<(char, char, char)>(new byte[256].ToArray(x => ((char)random.Next(65536), (char)random.Next(65536), (char)random.Next(65536))));
		var a = new NList<(char, char, char)>(c);
		var b = new NList<(char, char, char)>(a).Sort(x => x.Item3);
		c = E.ToList(E.OrderBy(c, x => x.Item3));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestStartsWith()
	{
		var a = nList.ToNList();
		var b = a.StartsWith(('M', 'M', 'M'));
		Assert.IsTrue(b);
		b = a.StartsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P')));
		Assert.IsTrue(b);
		b = a.StartsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('B', 'B', 'B'), ('X', 'X', 'X')));
		Assert.IsFalse(b);
		b = a.StartsWith((G.IEnumerable<(char, char, char)>)null!);
		Assert.IsFalse(b);
	}

	[TestMethod]
	public void TestTake()
	{
		var a = nList.ToNList();
		var b = a.Take(2);
		var c = E.Take(nList, 2);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Take(0);
		c = E.Take(nList, 0);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Take(1000);
		c = E.Take(nList, 1000);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Take(-4);
		c = E.Take(nList, -4);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeLast()
	{
		var a = nList.ToNList();
		var b = a.TakeLast(2);
		var c = E.TakeLast(nList, 2);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.TakeLast(0);
		c = E.TakeLast(nList, 0);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.TakeLast(1000);
		c = E.TakeLast(nList, 1000);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.TakeLast(-4);
		c = E.TakeLast(nList, -4);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeWhile()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.TakeWhile(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = E.ToList(E.TakeWhile(c, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.TakeWhile((x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') && index < 10);
		c = [.. nList];
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = E.ToList(E.TakeWhile(E.Take(c, 10), x => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestToArray()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		BaseListTests<(char, char, char), NList<(char, char, char)>>.TestToArray(() => ((char)random.Next(33, 127), (char)random.Next(33, 127), (char)random.Next(33, 127)));
	}

	[TestMethod]
	public void TestTrimExcess()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		BaseListTests<(char, char, char), NList<(char, char, char)>>.TestTrimExcess(() => ((char)random.Next(33, 127), (char)random.Next(33, 127), (char)random.Next(33, 127)));
	}

	[TestMethod]
	public void TestTrueForAll()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.TrueForAll(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.TrueForAll(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		b = a.TrueForAll(x => x.Item1 != '\0');
		d = c.TrueForAll(x => x.Item1 != '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		b = a.TrueForAll(x => new[] { x.Item1, x.Item2, x.Item3 }.Length > 3);
		d = c.TrueForAll(x => new[] { x.Item1, x.Item2, x.Item3 }.Length > 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestTuples()
	{
		var a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'));
		var b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O'), ('P', 'P', 'P'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O'), ('P', 'P', 'P'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		var fullNList = b.Copy();
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char)))fullNList[..1]);
		Assert.AreEqual((((char, char, char), (char, char, char)))fullNList[..2], (('A', 'A', 'A'), ('B', 'B', 'B')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char)))fullNList[..3]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char)))fullNList[..2]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char)))fullNList[..3], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char)))fullNList[..4]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..3]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..4], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..5]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..4]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..5], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..6]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..5]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..6], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..7]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..6]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..7], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..8]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..7]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..8], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..9]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..8]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..9], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..10]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..9]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..10], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..11]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..10]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..11], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..12]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..11]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..12], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..13]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..12]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..13], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..14]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..13]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..14], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..15]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..14]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..15], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O')));
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..16]);
		Assert.ThrowsExactly<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..15]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..16], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O'), ('P', 'P', 'P')));
		Assert.ThrowsExactly<ArgumentException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..17]);
	}
}
