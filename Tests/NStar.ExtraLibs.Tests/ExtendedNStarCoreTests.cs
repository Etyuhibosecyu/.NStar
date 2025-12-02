namespace NStar.ExtraLibs.Tests;

[TestClass]
public class BitListTests
{
	[TestMethod]
	public void ComplexTest2()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var toInsert = Array.Empty<bool>();
		var counter = 0;
	l1:
		var arr = RedStarLinq.FillArray(129, _ => random.Next(2) == 1);
		BitList bl = new(arr);
		G.List<bool> gl = new(arr);
		var secondaryActions = new[] { () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(41), _ => random.Next(2) == 1);
			bl.AddRange(toInsert);
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(41), _ => random.Next(2) == 1);
			bl.AddRange(toInsert.AsSpan());
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(41), _ => random.Next(2) == 1);
			bl.AddRange(toInsert.ToList());
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(41), _ => random.Next(2) == 1);
			bl.AddRange(E.Select(toInsert, x => x));
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(41), _ => random.Next(2) == 1);
			bl.AddRange(E.SkipWhile(toInsert, _ => random.Next(10) == -1));
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			var n = random.Next(bl.Length);
			toInsert = RedStarLinq.FillArray(random.Next(41), _ => random.Next(2) == 1);
			bl.Insert(n, toInsert);
			gl.InsertRange(n, toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			var n = random.Next(bl.Length);
			toInsert = RedStarLinq.FillArray(random.Next(41), _ => random.Next(2) == 1);
			bl.Insert(n, toInsert.AsSpan());
			gl.InsertRange(n, toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			var n = random.Next(bl.Length);
			toInsert = RedStarLinq.FillArray(random.Next(41), _ => random.Next(2) == 1);
			bl.Insert(n, toInsert.ToList());
			gl.InsertRange(n, toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			var n = random.Next(bl.Length);
			toInsert = RedStarLinq.FillArray(random.Next(41), _ => random.Next(2) == 1);
			bl.Insert(n, E.Select(toInsert, x => x));
			gl.InsertRange(n, toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			var n = random.Next(bl.Length);
			toInsert = RedStarLinq.FillArray(random.Next(41), _ => random.Next(2) == 1);
			bl.Insert(n, E.SkipWhile(toInsert, _ => random.Next(10) == -1));
			gl.InsertRange(n, toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			var length = Min(random.Next(65), bl.Length);
			if (bl.Length < length)
				return;
			var start = random.Next(bl.Length - length + 1);
			bl.Clear(start, length);
			gl.SetAll(default!, start, length);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			var n = random.Next(2) == 1;
			Assert.AreEqual(gl.Contains(n), bl.Contains(n));
			Assert.AreEqual(gl.IndexOf(n), bl.IndexOf(n));
			Assert.AreEqual(gl.LastIndexOf(n), bl.LastIndexOf(n));
		}, () =>
		{
			if (bl.Length == 0)
				return;
			var index = random.Next(bl.Length);
			var n = random.Next(2) == 1;
			Assert.AreEqual(gl.IndexOf(n, index) >= 0, bl.Contains(n, index));
			Assert.AreEqual(gl.IndexOf(n, index), bl.IndexOf(n, index));
			Assert.AreEqual(gl.LastIndexOf(n, index), bl.LastIndexOf(n, index));
		}, () =>
		{
			var n = random.Next(2) == 1;
			var length = Min(random.Next(65), bl.Length);
			if (length == 0)
				return;
			var start = random.Next(bl.Length - length + 1);
			Assert.AreEqual(gl.IndexOf(n, start, length) >= 0, bl.Contains(n, start, length));
			Assert.AreEqual(gl.IndexOf(n, start, length), bl.IndexOf(n, start, length));
			Assert.AreEqual(gl.LastIndexOf(n, gl.Count - 1 - start, length), bl.LastIndexOf(n, bl.Length - 1 - start, length));
		} };
		var actions = new[] { () =>
		{
			var n = random.Next(2) == 1;
			bl.Add(n);
			gl.Add(n);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			var index = random.Next(bl.Length);
			var n = random.Next(2) == 1;
			bl.Insert(index, n);
			gl.Insert(index, n);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			if (bl.Length == 0) return;
			var n = random.Next(bl.Length);
			bl.RemoveAt(n);
			gl.RemoveAt(n);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			if (bl.Length == 0) return;
			var n = random.Next(2) == 1;
			bl.RemoveValue(n);
			gl.Remove(n);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			secondaryActions.Random(random)();
			Assert.IsTrue(RedStarLinq.Equals(bl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			var length = Min(random.Next(65), bl.Length);
			if (bl.Length < length)
				return;
			var start = random.Next(bl.Length - length + 1);
			bl.Remove(start, length);
			gl.RemoveRange(start, length);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			if (bl.Length == 0)
				return;
			var index = random.Next(bl.Length);
			var n = random.Next(2) == 1;
			if (bl[index] == n)
				return;
			bl[index] = n;
			gl[index] = n;
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			if (bl.Length == 0) return;
			var n = random.Next(bl.Length);
			Assert.AreEqual(bl[bl.IndexOf(bl[n])], bl[n]);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 10000)
			goto l1;
	}

	[TestMethod]
	public void TestAddRange()
	{
		var a = new BitList(bitList).AddRange(defaultBitCollection);
		var b = new G.List<bool>(bitList);
		b.AddRange(defaultBitCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).AddRange(defaultBitCollection.Take(2..5));
		b = new G.List<bool>(bitList);
		b.AddRange(defaultBitCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).AddRange(defaultByteCollection);
		b = new G.List<bool>(bitList);
		b.AddRange(E.ToArray(E.SelectMany(defaultByteCollection, x => E.Select(E.Range(0, BitsPerByte), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).AddRange(defaultByteCollection.Take(2..4));
		b = new G.List<bool>(bitList);
		b.AddRange(E.ToArray(E.SelectMany(defaultByteCollection.Skip(2).Take(2), x => E.Select(E.Range(0, BitsPerByte), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).AddRange(defaultIntCollection);
		b = new G.List<bool>(bitList);
		b.AddRange(E.ToArray(E.SelectMany(defaultIntCollection, x => E.Select(E.Range(0, BitsPerInt), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).AddRange(defaultIntCollection.Take(1..));
		b = new G.List<bool>(bitList);
		b.AddRange(E.ToArray(E.SelectMany(defaultIntCollection.Skip(1), x => E.Select(E.Range(0, BitsPerInt), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).AddRange(defaultUIntCollection);
		b = new G.List<bool>(bitList);
		b.AddRange(E.ToArray(E.SelectMany(defaultUIntCollection, x => E.Select(E.Range(0, BitsPerInt), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).AddRange(defaultUIntCollection.Take(1..));
		b = new G.List<bool>(bitList);
		b.AddRange(E.ToArray(E.SelectMany(defaultUIntCollection.Skip(1), x => E.Select(E.Range(0, BitsPerInt), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestInsert()
	{
		var a = new BitList(bitList).Insert(3, defaultBit);
		var b = new G.List<bool>(bitList);
		b.Insert(3, defaultBit);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(4, defaultBitCollection);
		b = new G.List<bool>(bitList);
		b.InsertRange(4, defaultBitCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(2, defaultBitCollection.Take(2..5));
		b = new G.List<bool>(bitList);
		b.InsertRange(2, defaultBitCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(4, defaultByteCollection);
		b = new G.List<bool>(bitList);
		b.InsertRange(4, E.ToArray(E.SelectMany(defaultByteCollection, x => E.Select(E.Range(0, BitsPerByte), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(2, defaultByteCollection.Take(2..4));
		b = new G.List<bool>(bitList);
		b.InsertRange(2, E.ToArray(E.SelectMany(defaultByteCollection.Skip(2).Take(2), x => E.Select(E.Range(0, BitsPerByte), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(4, defaultIntCollection);
		b = new G.List<bool>(bitList);
		b.InsertRange(4, E.ToArray(E.SelectMany(defaultIntCollection, x => E.Select(E.Range(0, BitsPerInt), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(2, defaultIntCollection.Take(1..));
		b = new G.List<bool>(bitList);
		b.InsertRange(2, E.ToArray(E.SelectMany(defaultIntCollection.Skip(1), x => E.Select(E.Range(0, BitsPerInt), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(4, defaultUIntCollection);
		b = new G.List<bool>(bitList);
		b.InsertRange(4, E.ToArray(E.SelectMany(defaultUIntCollection, x => E.Select(E.Range(0, BitsPerInt), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(2, defaultUIntCollection.Take(1..));
		b = new G.List<bool>(bitList);
		b.InsertRange(2, E.ToArray(E.SelectMany(defaultUIntCollection.Skip(1), x => E.Select(E.Range(0, BitsPerInt), y => (x & 1 << y) != 0))));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a = new BitList(bitList).Insert(1000, defaultBit));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BitList(bitList).Insert(-1, defaultBitCollection));
		Assert.ThrowsExactly<ArgumentNullException>(() => new BitList(bitList).Insert(1, (BitArray)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => new BitList(bitList).Insert(1, (G.IEnumerable<byte>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => new BitList(bitList).Insert(1, (G.IEnumerable<bool>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => new BitList(bitList).Insert(1, (G.IEnumerable<int>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => new BitList(bitList).Insert(1, (G.IEnumerable<uint>)null!));
	}
}

[TestClass]
public class StringTests
{
	[TestMethod]
	public void TestAddRange()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		String a;
		for (var i = 0; i < 10000; i++)
		{
			var array = RedStarLinq.FillArray(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1));
			a = array.ToNString();
			var b = E.ToList(array);
			var @case = random.Next(50);
			switch (@case)
			{
				case 0:
				a.AddRange(a);
				b.AddRange(array);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 1:
				int length = random.Next(a.Length + 1), index2 = random.Next(a.Length - length + 1);
				a.AddRange(a.GetRange(index2, length));
				b.AddRange(array.Skip(index2).Take(length));
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 2:
				var array2 = RedStarLinq.Fill(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1));
				a.AddRange(array2);
				b.AddRange(array2);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 3:
				var array3 = RedStarLinq.FillArray(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1));
				a.AddRange(array3);
				b.AddRange(array3);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 4:
				var array4 = new Chain(random.Next(1001)).ToNString(_ => (char)random.Next('A', 'Z' + 1));
				a.AddRange(array4);
				b.AddRange(array4);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 5:
				var array5 = RedStarLinq.Fill(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1)).ToList().Insert(0, 'X').GetSlice(1);
				a.AddRange(array5);
				b.AddRange(array5);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 6:
				var array6 = RedStarLinq.Fill(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1)).Prepend('X');
				a.AddRange(array6);
				b.AddRange(array6);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 7:
				var seed = random.Next();
				Random random2 = new(seed), random3 = new(seed);
				var array7 = E.Select(E.Range(0, random2.Next(1001)), _ => (char)random2.Next('A', 'Z' + 1)).Prepend('X');
				a.AddRange(array7);
				b.AddRange(E.Select(E.Range(0, random3.Next(1001)), _ => (char)random3.Next('A', 'Z' + 1)).Prepend('X'));
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 8:
				seed = random.Next();
				random2 = new(seed);
				random3 = new(seed);
				var array8 = E.SkipWhile(E.Select(E.Range(0, random2.Next(1001)), _ => (char)random2.Next('A', 'Z' + 1)).Prepend('X'), x => x == '0');
				a.AddRange(array8);
				b.AddRange(E.SkipWhile(E.Select(E.Range(0, random3.Next(1001)), _ => (char)random3.Next('A', 'Z' + 1)).Prepend('X'), x => x == '0'));
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
			}
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => nString.ToNString().AddRange((G.IEnumerable<char>)null!));
	}

	[TestMethod]
	public void TestInsert()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		String a;
		for (var i = 0; i < 10000; i++)
		{
			var array = RedStarLinq.FillArray(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1));
			a = array.ToNString();
			var b = E.ToList(array);
			var @case = random.Next(50);
			var index = random.Next(a.Length);
			switch (@case)
			{
				case 0:
				a.Insert(index, a);
				b.InsertRange(index, array);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 1:
				int length = random.Next(a.Length + 1), index2 = random.Next(a.Length - length + 1);
				a.Insert(index, a.GetRange(index2, length));
				b.InsertRange(index, array.Skip(index2).Take(length));
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 2:
				var array2 = RedStarLinq.Fill(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1));
				a.Insert(index, array2);
				b.InsertRange(index, array2);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 3:
				var array3 = RedStarLinq.FillArray(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1));
				a.Insert(index, array3);
				b.InsertRange(index, array3);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 4:
				var array4 = new Chain(random.Next(1001)).ToNString(_ => (char)random.Next('A', 'Z' + 1));
				a.Insert(index, array4);
				b.InsertRange(index, array4);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 5:
				var array5 = RedStarLinq.Fill(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1)).ToList().Insert(0, 'X').GetSlice(1);
				a.Insert(index, array5);
				b.InsertRange(index, array5);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 6:
				var array6 = RedStarLinq.Fill(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1)).Prepend('X');
				a.Insert(index, array6);
				b.InsertRange(index, array6);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 7:
				var seed = random.Next();
				Random random2 = new(seed), random3 = new(seed);
				var array7 = E.Select(E.Range(0, random2.Next(1001)), _ => (char)random2.Next('A', 'Z' + 1)).Prepend('X');
				a.Insert(index, array7);
				b.InsertRange(index, E.Select(E.Range(0, random3.Next(1001)), _ => (char)random3.Next('A', 'Z' + 1)).Prepend('X'));
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
				case 8:
				seed = random.Next();
				random2 = new(seed);
				random3 = new(seed);
				var array8 = E.SkipWhile(E.Select(E.Range(0, random2.Next(1001)), _ => (char)random2.Next('A', 'Z' + 1)).Prepend('X'), x => x == '0');
				a.Insert(index, array8);
				b.InsertRange(index, E.SkipWhile(E.Select(E.Range(0, random3.Next(1001)), _ => (char)random3.Next('A', 'Z' + 1)).Prepend('X'), x => x == '0'));
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				break;
			}
		}
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a = nString.ToNString().Insert(1000, 'X'));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => nString.ToNString().Insert(-1, defaultNString));
		Assert.ThrowsExactly<ArgumentNullException>(() => nString.ToNString().Insert(5, (G.IEnumerable<char>)null!));
	}

	[TestMethod]
	public void TestRemoveValue()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = new Chain(15, 10).ToNString(x => (char)x);
		for (var i = 0; i < 1000; i++)
		{
			var value = a.Random(random);
			var b = new String(a);
			b.RemoveValue(value);
			var c = new string(E.ToArray(a));
			c = c.Replace("" + value, "");
			foreach (var x in a)
				Assert.AreEqual(b.Contains(x), x != value);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestShuffle()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var toShuffle = new string(new byte[256].ToArray(x => (char)random.Next(65536)));
		var a = new String(toShuffle);
		var b = new String(a).Shuffle();
		b.Replace(b.ToList().Sort());
		var c = new G.List<char>(a);
		c.Shuffle();
		c.Sort();
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}
}
