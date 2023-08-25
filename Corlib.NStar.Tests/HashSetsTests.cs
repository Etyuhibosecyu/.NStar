﻿
namespace Corlib.NStar.Tests;

[TestClass]
public class FastDelHashSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var arr = RedStarLinq.FillArray(16, _ => random.Next(16));
		FastDelHashSet<int> fhs = new(arr);
		G.HashSet<int> gs = new(arr);
		var collectionActions = new[] { (int[] arr) =>
		{
			fhs.ExceptWith(arr);
			gs.ExceptWith(arr);
			Assert.IsTrue(fhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(fhs));
		}, arr =>
		{
			fhs.IntersectWith(arr);
			gs.IntersectWith(arr);
			Assert.IsTrue(fhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(fhs));
		}, arr =>
		{
			fhs.SymmetricExceptWith(arr);
			gs.SymmetricExceptWith(arr);
			Assert.IsTrue(fhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(fhs));
		}, arr =>
		{
			fhs.UnionWith(arr);
			gs.UnionWith(arr);
			Assert.IsTrue(fhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(fhs));
		} };
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			fhs.Add(n);
			gs.Add(n);
			Assert.IsTrue(fhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(fhs));
		}, () =>
		{
			if (fhs.Length == 0) return;
			if (random.Next(2) == 0)
			{
				int n;
				do
					n = random.Next(fhs.Size);
				while (!fhs.IsValidIndex(n));
				gs.Remove(fhs[n, suppressException : true]);
				fhs.RemoveAt(n);
			}
			else
			{
				var n = random.Next(16);
				fhs.RemoveValue(n);
				gs.Remove(n);
			}
			Assert.IsTrue(fhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(fhs));
		}, () =>
		{
			var arr = RedStarLinq.FillArray(5, _ => random.Next(16));
			collectionActions.Random(random)(arr);
			Assert.IsTrue(fhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(fhs));
		}, () =>
		{
			if (fhs.Length == 0) return;
			int n;
			do
				n = random.Next(fhs.Size);
			while (!fhs.IsValidIndex(n));
			var n2 = random.Next(16);
			gs.Remove(fhs[n, suppressException: true]);
			fhs.RemoveValue(n2);
			fhs[n, suppressException: true] = n2;
			gs.Add(n2);
			Assert.IsTrue(fhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(fhs));
		}, () =>
		{
			if (fhs.Length == 0) return;
			int n;
			do
				n = random.Next(fhs.Size);
			while (!fhs.IsValidIndex(n));
			Assert.AreEqual(fhs.IndexOf(fhs[n, suppressException : true]), n);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
	}
}

[TestClass]
public class ListHashSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var arr = RedStarLinq.FillArray(16, _ => random.Next(16));
		ListHashSet<int> lhs = new(arr);
		G.HashSet<int> gs = new(arr);
		var collectionActions = new[] { (int[] arr) =>
		{
			lhs.ExceptWith(arr);
			gs.ExceptWith(arr);
			Assert.IsTrue(lhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(lhs));
		}, arr =>
		{
			lhs.IntersectWith(arr);
			gs.IntersectWith(arr);
			Assert.IsTrue(lhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(lhs));
		}, arr =>
		{
			lhs.SymmetricExceptWith(arr);
			gs.SymmetricExceptWith(arr);
			Assert.IsTrue(lhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(lhs));
		}, arr =>
		{
			lhs.UnionWith(arr);
			gs.UnionWith(arr);
			Assert.IsTrue(lhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(lhs));
		} };
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			lhs.Add(n);
			gs.Add(n);
			Assert.IsTrue(lhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(lhs));
		}, () =>
		{
			if (lhs.Length == 0) return;
			if (random.Next(2) == 0)
			{
				var n = random.Next(lhs.Length);
				gs.Remove(lhs[n]);
				lhs.RemoveAt(n);
			}
			else
			{
				var n = random.Next(16);
				lhs.RemoveValue(n);
				gs.Remove(n);
			}
			Assert.IsTrue(lhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(lhs));
		}, () =>
		{
			var arr = RedStarLinq.FillArray(5, _ => random.Next(16));
			collectionActions.Random(random)(arr);
			Assert.IsTrue(lhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(lhs));
		}, () =>
		{
			if (lhs.Length == 0)
				return;
			var n = random.Next(lhs.Length);
			var n2 = random.Next(16);
			if (lhs[n] == n2)
				return;
			gs.Remove(lhs[n]);
			if (lhs.TryGetIndexOf(n2, out var index) && index < n)
				n--;
			lhs.RemoveValue(n2);
			lhs[n] = n2;
			gs.Add(n2);
			Assert.IsTrue(lhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(lhs));
		}, () =>
		{
			if (lhs.Length == 0) return;
			var n = random.Next(lhs.Length);
			Assert.AreEqual(lhs.IndexOf(lhs[n]), n);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
	}

	[TestMethod]
	public void TestAdd()
	{
		var a = new ListHashSet<string>(list).Add(defaultString);
		var b = E.ToHashSet(new G.List<string>(list) { defaultString });
		var bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
	}

	[TestMethod]
	public void TestAddRange()
	{
		var a = new ListHashSet<string>(list).AddRange(defaultCollection);
		var b = new G.List<string>(list);
		b.AddRange(defaultCollection);
		var bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
	}

	[TestMethod]
	public void TestAppend()
	{
		var a = new ListHashSet<string>(list);
		var b = a.Append(defaultString);
		var c = E.Append(new G.List<string>(E.Distinct(list)), defaultString);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestBreakFilter()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.BreakFilter(x => x.Length == 3, out var c);
		var d = new G.List<string>(E.Distinct(list));
		d.InsertRange(3, new G.List<string>() { "$", "###" });
		var e = E.Where(d, x => x.Length == 3);
		var f = E.Where(d, x => x.Length != 3);
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, b));
		Assert.IsTrue(c.Equals(f));
		Assert.IsTrue(E.SequenceEqual(f, c));
		b = a.BreakFilter(x => x.All(y => y is >= 'A' and <= 'Z'), out c);
		e = E.Where(d, x => E.All(x, y => y is >= 'A' and <= 'Z'));
		f = E.Where(d, x => !E.All(x, y => y is >= 'A' and <= 'Z'));
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
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.BreakFilterInPlace(x => x.Length == 3, out var c);
		var d = new G.List<string>(E.Distinct(list));
		d.InsertRange(3, new G.List<string>() { "$", "###" });
		var e = E.ToList(E.Where(d, x => x.Length != 3));
		d = E.ToList(E.Where(d, x => x.Length == 3));
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		var bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
		Assert.IsTrue(c.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, c));
		a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		b = a.BreakFilterInPlace(x => x.All(y => y is >= 'A' and <= 'Z'), out c);
		d = new G.List<string>(E.Distinct(list));
		d.InsertRange(3, new G.List<string>() { "$", "###" });
		e = E.ToList(E.Where(d, x => !E.All(x, y => y is >= 'A' and <= 'Z')));
		d = E.ToList(E.Where(d, x => E.All(x, y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
		Assert.IsTrue(c.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, c));
	}

	[TestMethod]
	public void TestClear()
	{
		var a = new ListHashSet<string>(list);
		a.Clear(2, 3);
		var b = new G.List<string>(E.Distinct(list));
		for (var i = 0; i < 3; i++)
			b[2 + i] = default!;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestCompare()
	{
		for (var i = 0; i < 1000; i++)
		{
			var a = new List<string>(E.Select(E.Range(0, random.Next(3, 100)), _ => random.Next(1000).ToString("D3")));
			var b = new List<string>(a);
			var n = random.Next(0, a.Length);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n]);
			Assert.IsTrue(a.Compare(b) == n);
			a = new(E.Select(E.Range(0, random.Next(5, 100)), _ => random.Next(1000).ToString("D3")));
			b = new(a);
			n = random.Next(2, a.Length);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n]);
			Assert.IsTrue(a.Compare(b, n - 1) == n - 1);
			a = new(E.Select(E.Range(0, random.Next(5, 100)), _ => random.Next(1000).ToString("D3")));
			b = new(a);
			var length = a.Length;
			n = random.Next(2, a.Length);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n]);
			int index = random.Next(2, 50), otherIndex = random.Next(2, 50);
			a.Insert(0, E.Select(E.Range(0, index), _ => random.Next(1000).ToString("D3")));
			b.Insert(0, E.Select(E.Range(0, otherIndex), _ => random.Next(1000).ToString("D3")));
			Assert.IsTrue(a.Compare(index, b, otherIndex) == n);
			Assert.IsTrue(a.Compare(index, b, otherIndex, length) == n);
		}
	}

	[TestMethod]
	public void TestConcat()
	{
		var a = new ListHashSet<string>(list);
		var b = a.Concat(new(defaultCollection));
		var c = E.ToHashSet(E.Concat(new G.List<string>(list), defaultCollection));
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestContains()
	{
		var a = new ListHashSet<string>(list);
		var b = a.Contains("MMM");
		Assert.IsTrue(b);
		b = a.Contains("BBB", 2);
		Assert.IsTrue(!b);
		b = a.Contains(new List<string>("PPP", "DDD", "EEE"));
		Assert.IsTrue(b);
		b = a.Contains(new List<string>("PPP", "DDD", "NNN"));
		Assert.IsTrue(!b);
		Assert.ThrowsException<ArgumentNullException>(() => a.Contains((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestContainsAny()
	{
		var a = new ListHashSet<string>(list);
		var b = a.ContainsAny(new List<string>("PPP", "DDD", "MMM"));
		Assert.IsTrue(b);
		b = a.ContainsAny(new List<string>("LLL", "MMM", "NNN"));
		Assert.IsTrue(b);
		b = a.ContainsAny(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		var a = new ListHashSet<string>(list);
		var b = a.ContainsAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(a);
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestConvert()
	{
		var a = new ListHashSet<string>(list);
		var b = a.Convert((x, index) => (x, index));
		var c = E.Select(E.ToHashSet(new G.List<string>(list)), (x, index) => (x, index));
		var d = a.Convert(x => x + "A");
		var e = E.Select(E.ToHashSet(new G.List<string>(list)), x => x + "A");
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(d.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var a = new ListHashSet<string>(list);
		var b = RedStarLinq.FillArray(16, x => new string(RedStarLinq.FillArray(3, x => (char)random.Next(65536))));
		var c = (string[])b.Clone();
		var d = (string[])b.Clone();
		var e = (string[])b.Clone();
		a.CopyTo(b);
		new G.List<string>(E.Distinct(list)).CopyTo(c);
		a.CopyTo(d, 3);
		new G.List<string>(E.Distinct(list)).CopyTo(e, 3);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestEndsWith()
	{
		var a = new ListHashSet<string>(list);
		var b = a.EndsWith("EEE");
		Assert.IsTrue(b);
		b = a.EndsWith(new List<string>("PPP", "DDD", "EEE"));
		Assert.IsTrue(b);
		b = a.EndsWith(new List<string>("MMM", "DDD", "EEE"));
		Assert.IsTrue(!b);
		b = a.EndsWith(new List<string>("MMM", "EEE", "NNN"));
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestEquals()
	{
		var a = new ListHashSet<string>(list);
		var b = a.Contains("MMM");
		Assert.IsTrue(b);
		b = a.Equals(new List<string>("BBB", "PPP", "DDD"), 1);
		Assert.IsTrue(b);
		b = a.Equals(new List<string>("PPP", "DDD", "NNN"), 1);
		Assert.IsTrue(!b);
		b = a.Equals(new List<string>("PPP", "DDD", "MMM"), 2);
		Assert.IsTrue(!b);
		b = a.Equals(new List<string>("BBB", "PPP", "DDD"), 1, true);
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestFilter()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.Filter(x => x.Length == 3);
		var c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		var d = E.Where(c, x => x.Length == 3);
		var chs = E.ToHashSet(c);
		Assert.IsTrue(a.Equals(chs));
		Assert.IsTrue(E.SequenceEqual(chs, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		b = a.Filter((x, index) => x.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		d = E.Where(c, (x, index) => x.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		chs = E.ToHashSet(c);
		Assert.IsTrue(a.Equals(chs));
		Assert.IsTrue(E.SequenceEqual(chs, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestFilterInPlace()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.FilterInPlace(x => x.Length == 3);
		var c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		c = E.ToList(E.Where(c, x => x.Length == 3));
		var chs = E.ToHashSet(c);
		Assert.IsTrue(a.Equals(chs));
		Assert.IsTrue(E.SequenceEqual(chs, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		var bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
		a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		b = a.FilterInPlace((x, index) => x.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		c = E.ToList(E.Where(c, (x, index) => E.All(x, y => y is >= 'A' and <= 'Z') && index >= 1));
		chs = E.ToHashSet(c);
		Assert.IsTrue(a.Equals(chs));
		Assert.IsTrue(E.SequenceEqual(chs, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
	}

	[TestMethod]
	public void TestFind()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.Find(x => x.Length != 3);
		var c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		var d = c.Find(x => x.Length != 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		b = a.Find(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		d = c.Find(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestFindAll()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.FindAll(x => x.Length != 3);
		var c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		var d = c.FindAll(x => x.Length != 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		b = a.FindAll(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		d = c.FindAll(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestFindIndex()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.FindIndex(x => x.Length != 3);
		var c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		var d = c.FindIndex(x => x.Length != 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		b = a.FindIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		d = c.FindIndex(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestFindLast()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.FindLast(x => x.Length != 3);
		var c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		var d = c.FindLast(x => x.Length != 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		b = a.FindLast(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		d = c.FindLast(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestFindLastIndex()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.FindLastIndex(x => x.Length != 3);
		var c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		var d = c.FindLastIndex(x => x.Length != 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		b = a.FindLastIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		d = c.FindLastIndex(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestGetAfter()
	{
		var a = new ListHashSet<string>(list);
		var b = a.GetAfter(new("PPP"));
		var c = new G.List<string>() { "DDD", "EEE" };
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter(new());
		c = new();
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter(new("DDD", "EEE"));
		c = new G.List<string>();
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBefore()
	{
		var a = new ListHashSet<string>(list);
		var b = a.GetBefore(new("DDD"));
		var c = new G.List<string>() { "MMM", "BBB", "PPP" };
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore(new());
		c = new(E.Distinct(list));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore(new("DDD", "EEE"));
		c = new G.List<string>() { "MMM", "BBB", "PPP" };
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeSetAfter()
	{
		var a = new ListHashSet<string>(list);
		var b = a.GetBeforeSetAfter(new("DDD"));
		var c = new G.List<string>() { "MMM", "BBB", "PPP" };
		var d = new G.List<string>() { "EEE" };
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetRange()
	{
		var length = E.ToHashSet(list).Count;
		var a = new ListHashSet<string>(list);
		var b = a.GetRange(..);
		var c = new G.List<string>(E.Distinct(list));
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(.., true);
		b.Add(defaultString);
		c = new G.List<string>(E.Distinct(list)).GetRange(0, length);
		c.Add(defaultString);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(..^1);
		c = new G.List<string>(E.Distinct(list)).GetRange(0, length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(1..);
		c = new G.List<string>(E.Distinct(list)).GetRange(1, length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(1..^1);
		c = new G.List<string>(E.Distinct(list)).GetRange(1, length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(1..4);
		c = new G.List<string>(E.Distinct(list)).GetRange(1, 3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(^4..);
		c = new G.List<string>(E.Distinct(list)).GetRange(length - 4, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(^4..^1);
		c = new G.List<string>(E.Distinct(list)).GetRange(length - 4, 3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(^4..4);
		c = new G.List<string>(E.Distinct(list)).GetRange(length - 4, 8 - length);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetRange(-1..4));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetRange(^1..1));
		Assert.ThrowsException<ArgumentException>(() => b = a.GetRange(1..1000));
	}

	[TestMethod]
	public void TestGetSlice()
	{
		var length = E.ToHashSet(list).Count;
		var a = new ListHashSet<string>(list);
		var b = a.GetSlice(..);
		var c = new G.List<string>(E.Distinct(list));
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(1);
		c = new G.List<string>(E.Distinct(list)).GetRange(1, length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(1, 3);
		c = new G.List<string>(E.Distinct(list)).GetRange(1, 3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(^4);
		c = new G.List<string>(E.Distinct(list)).GetRange(length - 4, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(..^1);
		c = new G.List<string>(E.Distinct(list)).GetRange(0, length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(1..);
		c = new G.List<string>(E.Distinct(list)).GetRange(1, length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(1..^1);
		c = new G.List<string>(E.Distinct(list)).GetRange(1, length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(1..4);
		c = new G.List<string>(E.Distinct(list)).GetRange(1, 3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(^4..);
		c = new G.List<string>(E.Distinct(list)).GetRange(length - 4, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(^4..^1);
		c = new G.List<string>(E.Distinct(list)).GetRange(length - 4, 3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(^4..4);
		c = new G.List<string>(E.Distinct(list)).GetRange(length - 4, 8 - length);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetSlice(-1..4));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetSlice(^1..1));
		Assert.ThrowsException<ArgumentException>(() => b = a.GetSlice(1..1000));
	}

	[TestMethod]
	public void TestIndexOf()
	{
		var a = new ListHashSet<string>(list);
		var b = a.IndexOf("MMM");
		Assert.AreEqual(b, 0);
		b = a.IndexOf("BBB", 2);
		Assert.AreEqual(b, -1);
		b = a.IndexOf("BBB", 1, 2);
		Assert.AreEqual(b, 1);
		b = a.IndexOf(new List<string>("PPP", "DDD", "EEE"));
		Assert.AreEqual(b, 2);
		b = a.IndexOf(new List<string>("PPP", "DDD", "NNN"));
		Assert.AreEqual(b, -1);
		b = a.IndexOf(new[] { "DDD", "EEE" }, 3);
		Assert.AreEqual(b, 3);
		b = a.IndexOf(new[] { "DDD", "EEE" }, 0, 3);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.IndexOf((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestIndexOfAny()
	{
		var a = new ListHashSet<string>(list);
		var b = a.IndexOfAny(new List<string>("PPP", "DDD", "MMM"));
		Assert.AreEqual(b, 0);
		b = a.IndexOfAny(new List<string>("LLL", "NNN", "PPP"));
		Assert.AreEqual(b, 2);
		b = a.IndexOfAny(new[] { "LLL", "NNN", "PPP" }, 3);
		Assert.AreEqual(b, -1);
		b = a.IndexOfAny(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAny((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		var a = new ListHashSet<string>(list);
		var b = a.IndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
		Assert.AreEqual(b, 1);
		b = a.IndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.AreEqual(b, 0);
		b = a.IndexOfAnyExcluding(a);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestInsert()
	{
		var a = new ListHashSet<string>(list).Insert(3, defaultString);
		var b = new G.List<string>(E.Distinct(list));
		b.Insert(3, defaultString);
		var bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
		a = new ListHashSet<string>(list).Insert(3, defaultCollection);
		b = new G.List<string>(E.Distinct(list));
		b.InsertRange(3, defaultCollection);
		bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a = new ListHashSet<string>(list).Insert(1000, defaultString));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ListHashSet<string>(list).Insert(-1, defaultCollection));
		Assert.ThrowsException<ArgumentNullException>(() => new ListHashSet<string>(list).Insert(4, (G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAny()
	{
		var a = new ListHashSet<string>(list);
		var b = a.LastIndexOfAny(new List<string>("PPP", "DDD", "MMM"));
		Assert.AreEqual(b, 3);
		b = a.LastIndexOfAny(new List<string>("LLL", "NNN", "PPP"));
		Assert.AreEqual(b, 2);
		b = a.LastIndexOfAny(new[] { "LLL", "NNN", "EEE" }, 3);
		Assert.AreEqual(b, -1);
		b = a.LastIndexOfAny(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAny((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		var a = new ListHashSet<string>(list);
		var b = a.LastIndexOfAnyExcluding(new List<string>("BBB", "EEE", "DDD"));
		Assert.AreEqual(b, 2);
		b = a.LastIndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.AreEqual(b, 4);
		b = a.LastIndexOfAnyExcluding(a);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestNSort()
	{
		var c = new G.List<string>(new string[256].ToArray(x => new byte[random.Next(1, 17)].ToString(y => (char)random.Next(65536))));
		var a = new List<string>(c);
		var b = new List<string>(a).NSort(x => x[^1]);
		c = E.ToList(E.OrderBy(c, x => x[^1]));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestRemove()
	{
		var length = E.ToHashSet(list).Count;
		var a = new ListHashSet<string>(list);
		var b = new List<string>(a).Remove(4);
		var c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(4, 1);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(0, 1);
		c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(0, 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1, length - 2);
		c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(1, length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1, 3);
		c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(1, 3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(length - 4, 3);
		c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(length - 4, 3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(length - 4, 8 - length);
		c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(length - 4, 8 - length);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(-1, 6));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(length - 1, 2 - length));
		Assert.ThrowsException<ArgumentException>(() => b = new List<string>(a).Remove(1, 1000));
		b = new List<string>(a).Remove(..);
		c = new G.List<string>();
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(..^1);
		c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(0, length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1..);
		c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(1, length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1..^1);
		c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(1, length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1..4);
		c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(1, 3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(^4..^1);
		c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(length - 4, 3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(^4..4);
		c = new G.List<string>(E.Distinct(list));
		c.RemoveRange(length - 4, 8 - length);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(-1..4));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(^1..1));
		Assert.ThrowsException<ArgumentException>(() => b = new List<string>(a).Remove(1..1000));
	}

	[TestMethod]
	public void TestRemoveAll()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.RemoveAll(x => x.Length != 3);
		var c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		var d = c.RemoveAll(x => x.Length != 3);
		var chs = E.ToHashSet(c);
		Assert.IsTrue(a.Equals(chs));
		Assert.IsTrue(E.SequenceEqual(chs, a));
		Assert.AreEqual(b, d);
		a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		b = a.RemoveAll(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		d = c.RemoveAll(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
		chs = E.ToHashSet(c);
		Assert.IsTrue(a.Equals(chs));
		Assert.IsTrue(E.SequenceEqual(chs, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestRemoveAt()
	{
		var a = new ListHashSet<string>(list);
		for (var i = 0; i < 1000; i++)
		{
			var index = random.Next(a.Length);
			var b = new List<string>(a).RemoveAt(index);
			var c = new G.List<string>(a);
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
	public void TestRemoveValue()
	{
		var a = new Chain(15, 10).ToList();
		for (var i = 0; i < 1000; i++)
		{
			var value = a[random.Next(a.Length)];
			var b = new List<int>(a);
			b.RemoveValue(value);
			var c = new G.List<int>(a);
			c.Remove(value);
			foreach (var x in a)
				Assert.AreEqual(b.Contains(x), x != value);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestReplace()
	{
		var a = new ListHashSet<string>(list).Replace(defaultCollection);
		var b = new G.List<string>(E.Distinct(list));
		b.Clear();
		b.AddRange(defaultCollection);
		var bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
	}

	[TestMethod]
	public void TestReplace2()
	{
		for (var i = 0; i < 1000; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < 1000; j++)
				arr[j] = (char)random.Next(33, 127);
			string s = new(arr);
			String a = s;
			var oldItem = (char)random.Next(33, 127);
			var newItem = (char)random.Next(33, 127);
			var b = a.Replace(oldItem, newItem);
			var c = s.Replace(oldItem, newItem);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < 1000; j++)
				arr[j] = (char)random.Next(33, 127);
			string s = new(arr);
			String a = s;
			var oldCollection = a.GetRange(random.Next(991), random.Next(1, 10)).ToString();
			var newArray = new char[random.Next(10)];
			for (var j = 0; j < newArray.Length; j++)
				newArray[j] = (char)random.Next(33, 127);
			string newCollection = new(newArray);
			var b = a.Replace(oldCollection, newArray);
			var c = s.Replace(oldCollection, newCollection);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestReplaceRange()
	{
		var a = new ListHashSet<string>(list).ReplaceRange(2, 1, defaultCollection);
		var b = new G.List<string>(E.Distinct(list));
		b.RemoveRange(2, 1);
		b.InsertRange(2, defaultCollection);
		var bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
		a = new ListHashSet<string>(list).ReplaceRange(1, 3, defaultCollection);
		b = new G.List<string>(E.Distinct(list));
		b.RemoveRange(1, 3);
		b.InsertRange(1, defaultCollection);
		bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
		Assert.ThrowsException<ArgumentException>(() => a = new ListHashSet<string>(list).ReplaceRange(1, 1000, new(defaultString)));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ListHashSet<string>(list).ReplaceRange(-1, 3, defaultCollection));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ListHashSet<string>(list).ReplaceRange(3, -2, defaultCollection));
		Assert.ThrowsException<ArgumentNullException>(() => new ListHashSet<string>(list).ReplaceRange(3, 1, null!));
	}

	[TestMethod]
	public void TestSetAll()
	{
		var a = new ListHashSet<string>(list).SetAll(defaultString);
		var b = new G.List<string>(E.Distinct(list));
		for (var i = 0; i < b.Count; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new ListHashSet<string>(list).SetAll(defaultString, 3);
		b = new G.List<string>(E.Distinct(list));
		for (var i = 3; i < b.Count; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new ListHashSet<string>(list).SetAll(defaultString, 2, 2);
		b = new G.List<string>(E.Distinct(list));
		for (var i = 2; i < 4; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new ListHashSet<string>(list).SetAll(defaultString, ^4);
		b = new G.List<string>(E.Distinct(list));
		for (var i = b.Count - 4; i < b.Count; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new ListHashSet<string>(list).SetAll(defaultString, ^4..3);
		b = new G.List<string>(E.Distinct(list));
		for (var i = b.Count - 4; i < 3; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSetOrAdd()
	{
		var a = new ListHashSet<string>(list);
		var b = new G.List<string>(E.Distinct(list));
		for (var i = 0; i < 1000; i++)
		{
			var n = (int)Floor(Cbrt(random.NextDouble()) * (a.Length + 1));
			var n2 = random.Next(1000).ToString("D3");
			a.SetOrAdd(n, n2);
			if (n < b.Count)
				b[n] = n2;
			else
				b.Add(n2);
		}
	}

	[TestMethod]
	public void TestSetRange()
	{
		var hs = defaultCollection.ToHashSet().GetSlice(..3);
		var hs2 = E.Except(hs, list).ToHashSet().GetSlice();
		var a = new ListHashSet<string>(list).SetRange(2, hs);
		var b = new G.List<string>(E.Distinct(list));
		for (var i = 0; i < hs2.Length; i++)
			b[i + 2] = hs2[i];
		var bhs = E.ToHashSet(b);
		Assert.IsTrue(a.Equals(bhs));
		Assert.IsTrue(E.SequenceEqual(bhs, a));
		Assert.ThrowsException<ArgumentException>(() => a = new ListHashSet<string>(list).SetRange(4, hs));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ListHashSet<string>(list).SetRange(-1, hs));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ListHashSet<string>(list).SetRange(1000, hs));
		Assert.ThrowsException<ArgumentNullException>(() => new ListHashSet<string>(list).SetRange(3, null!));
	}

	[TestMethod]
	public void TestSkip()
	{
		var a = new ListHashSet<string>(list);
		var b = a.Skip(2);
		var c = E.Skip(new G.List<string>(E.Distinct(list)), 2);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.Skip(0);
		c = E.Skip(new G.List<string>(E.Distinct(list)), 0);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.Skip(1000);
		c = E.Skip(new G.List<string>(E.Distinct(list)), 1000);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.Skip(-3);
		c = E.Skip(new G.List<string>(E.Distinct(list)), -3);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipLast()
	{
		var a = new ListHashSet<string>(list);
		var b = a.SkipLast(2);
		var c = E.SkipLast(new G.List<string>(E.Distinct(list)), 2);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.SkipLast(0);
		c = E.SkipLast(new G.List<string>(E.Distinct(list)), 0);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.SkipLast(1000);
		c = E.SkipLast(new G.List<string>(E.Distinct(list)), 1000);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.SkipLast(-3);
		c = E.SkipLast(new G.List<string>(E.Distinct(list)), -3);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipWhile()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.SkipWhile(x => x.Length == 3);
		var c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		var d = E.ToList(E.SkipWhile(c, x => x.Length == 3));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		b = a.SkipWhile((x, index) => x.All(y => y is >= 'A' and <= 'Z') || index < 1);
		c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		d = E.ToList(E.SkipWhile(E.Skip(c, 1), x => E.All(x, y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestSort()
	{
		var toSort = new G.List<string>(new string[256].ToArray(x => new byte[random.Next(1, 17)].ToString(y => (char)random.Next(65536))));
		var a = new List<string>(toSort);
		var b = new List<string>(a).Sort();
		var c = new G.List<string>(a);
		c.Sort();
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Sort(new Comparer<string>((x, y) => y.CompareTo(x)));
		c = new G.List<string>(a);
		c.Sort(new Comparer<string>((x, y) => y.CompareTo(x)));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Sort(2, 3, new Comparer<string>((x, y) => y.CompareTo(x)));
		c = new G.List<string>(a);
		c.Sort(2, 3, new Comparer<string>((x, y) => y.CompareTo(x)));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestStartsWith()
	{
		var a = new ListHashSet<string>(list);
		var b = a.StartsWith("MMM");
		Assert.IsTrue(b);
		b = a.StartsWith(new List<string>("MMM", "BBB", "PPP"));
		Assert.IsTrue(b);
		b = a.StartsWith(new List<string>("MMM", "BBB", "XXX"));
		Assert.IsTrue(!b);
		Assert.ThrowsException<ArgumentNullException>(() => a.StartsWith((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestTake()
	{
		var a = new ListHashSet<string>(list);
		var b = a.Take(2);
		var c = E.Take(new G.List<string>(E.Distinct(list)), 2);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.Take(0);
		c = E.Take(new G.List<string>(E.Distinct(list)), 0);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.Take(1000);
		c = E.Take(new G.List<string>(E.Distinct(list)), 1000);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.Take(-3);
		c = E.Take(new G.List<string>(E.Distinct(list)), -3);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeLast()
	{
		var a = new ListHashSet<string>(list);
		var b = a.TakeLast(2);
		var c = E.TakeLast(new G.List<string>(E.Distinct(list)), 2);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.TakeLast(0);
		c = E.TakeLast(new G.List<string>(E.Distinct(list)), 0);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.TakeLast(1000);
		c = E.TakeLast(new G.List<string>(E.Distinct(list)), 1000);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new ListHashSet<string>(list);
		b = a.TakeLast(-3);
		c = E.TakeLast(new G.List<string>(E.Distinct(list)), -3);
		Assert.IsTrue(a.Equals(E.Distinct(list)));
		Assert.IsTrue(E.SequenceEqual(E.Distinct(list), a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeWhile()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.TakeWhile(x => x.Length == 3);
		var c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		var d = E.ToList(E.TakeWhile(c, x => x.Length == 3));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		b = a.TakeWhile((x, index) => x.All(y => y is >= 'A' and <= 'Z') && index < 10);
		c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		d = E.ToList(E.TakeWhile(E.Take(c, 10), x => E.All(x, y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestToArray()
	{
		int length, capacity;
		List<string> a;
		G.List<string> b;
		string[] array;
		string[] array2;
		string elem;
		for (var i = 0; i < 1000; i++)
		{
			length = random.Next(51);
			capacity = length + random.Next(151);
			a = new(capacity);
			b = new(capacity);
			for (var j = 0; j < length; j++)
			{
				a.Add(elem = new((char)random.Next(33, 127), random.Next(10)));
				b.Add(elem);
			}
			array = a.ToArray();
			array2 = b.ToArray();
			Assert.IsTrue(RedStarLinq.Equals(array, array2));
			Assert.IsTrue(E.SequenceEqual(array, array2));
		}
	}

	[TestMethod]
	public void TestTrimExcess()
	{
		int length, capacity;
		List<string> a;
		G.List<string> b;
		string elem;
		for (var i = 0; i < 1000; i++)
		{
			length = random.Next(51);
			capacity = length + random.Next(9951);
			a = new(capacity);
			b = new(capacity);
			for (var j = 0; j < length; j++)
			{
				a.Add(elem = new((char)random.Next(33, 127), random.Next(10)));
				b.Add(elem);
			}
			a.TrimExcess();
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(a, b));
		}
	}

	[TestMethod]
	public void TestTrueForAll()
	{
		var a = new ListHashSet<string>(list).Insert(3, new List<string>("$", "###"));
		var b = a.TrueForAll(x => x.Length == 3);
		var c = new G.List<string>(E.Distinct(list));
		c.InsertRange(3, new G.List<string>() { "$", "###" });
		var d = c.TrueForAll(x => x.Length == 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		b = a.TrueForAll(x => x.Length <= 3);
		d = c.TrueForAll(x => x.Length <= 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		b = a.TrueForAll(x => x.Length > 3);
		d = c.TrueForAll(x => x.Length > 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}
}

[TestClass]
public class ParallelHashSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		ParallelHashSet<int> phs = new();
		G.HashSet<int> gs = new();
		for (var i = 0; i < 100; i++)
		{
			var n = random.Next(16);
			phs.Add(n);
			gs.Add(n);
		}
		Assert.IsTrue(phs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(phs));
		for (var i = 0; i < 10; i++)
		{
			var n = random.Next(16);
			phs.RemoveValue(n);
			gs.Remove(n);
		}
		Assert.IsTrue(phs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(phs));
		for (var i = 0; i < 10; i++)
		{
			var n = random.Next(16);
			phs.Add(n);
			gs.Add(n);
		}
		Assert.IsTrue(phs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(phs));
		for (var i = 0; i < 100; i++)
		{
			int n;
			do
			{
				n = random.Next(phs.Length);
			} while (phs[n, suppressException: true] == 0);
			Assert.AreEqual(phs.IndexOf(phs[n, suppressException: true]), n);
		}
	}
}

[TestClass]
public class TreeHashSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var arr = RedStarLinq.FillArray(16, _ => random.Next(16));
		TreeHashSet<int> ths = new(arr);
		G.HashSet<int> gs = new(arr);
		var collectionActions = new[] { (int[] arr) =>
		{
			ths.ExceptWith(arr);
			gs.ExceptWith(arr);
			Assert.IsTrue(ths.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(ths));
		}, arr =>
		{
			ths.IntersectWith(arr);
			gs.IntersectWith(arr);
			Assert.IsTrue(ths.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(ths));
		}, arr =>
		{
			ths.SymmetricExceptWith(arr);
			gs.SymmetricExceptWith(arr);
			Assert.IsTrue(ths.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(ths));
		}, arr =>
		{
			ths.UnionWith(arr);
			gs.UnionWith(arr);
			Assert.IsTrue(ths.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(ths));
		} };
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			ths.Add(n);
			gs.Add(n);
			Assert.IsTrue(ths.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(ths));
		}, () =>
		{
			if (ths.Length == 0) return;
			if (random.Next(2) == 0)
			{
				var n = random.Next(ths.Length);
				gs.Remove(ths[n]);
				ths.RemoveAt(n);
			}
			else
			{
				var n = random.Next(16);
				ths.RemoveValue(n);
				gs.Remove(n);
			}
			Assert.IsTrue(ths.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(ths));
		}, () =>
		{
			var arr = RedStarLinq.FillArray(5, _ => random.Next(16));
			collectionActions.Random(random)(arr);
			Assert.IsTrue(ths.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(ths));
		}, () =>
		{
			if (ths.Length == 0)
				return;
			var n = random.Next(ths.Length);
			var n2 = random.Next(16);
			if (ths[n] == n2)
				return;
			gs.Remove(ths[n]);
			if (ths.TryGetIndexOf(n2, out var index) && index < n)
				n--;
			ths.RemoveValue(n2);
			ths[n] = n2;
			gs.Add(n2);
			Assert.IsTrue(ths.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(ths));
		}, () =>
		{
			if (ths.Length == 0) return;
			var n = random.Next(ths.Length);
			Assert.AreEqual(ths.IndexOf(ths[n]), n);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
	}
}
