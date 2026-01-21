using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NStar.ExtraLibs.Tests;

[TestClass]
public class FastDelHashSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var arr = RedStarLinq.FillArray(16, _ => random.Next(16));
		var toInsert = Array.Empty<int>();
		FastDelHashSet<int> fhs = new(arr);
		G.HashSet<int> gs = [.. arr];
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
		var collectionActions2 = new[] { () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(6), _ => random.Next(16));
			fhs.AddRange(toInsert);
			gs.UnionWith(toInsert);
			Assert.IsTrue(fhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(fhs));
		}, () =>
		{
			var n = random.Next(fhs.Length);
			toInsert = RedStarLinq.FillArray(random.Next(6), _ => random.Next(16));
			var fhs2 = fhs.Copy();
			fhs.Insert(n, toInsert);
			gs.UnionWith(toInsert);
			Assert.IsTrue(fhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(fhs));
		}, () =>
		{
			var length = Min(random.Next(9), fhs.Length);
			if (fhs.Length < length)
				return;
			var start = random.Next(fhs.Length - length + 1);
			foreach (var item in fhs.GetSlice(start, length))
				gs.Remove(item);
			fhs.Remove(start, length);
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
			collectionActions2.Random(random)();
			Assert.IsTrue(fhs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(fhs));
		}, () =>
		{
			if (fhs.Length == 0) return;
			int index;
			do
				index = random.Next(fhs.Size);
			while (!fhs.IsValidIndex(index));
			var n = random.Next(16);
			gs.Remove(fhs[index, suppressException: true]);
			fhs.RemoveValue(n);
			fhs[index, suppressException: true] = n;
			gs.Add(n);
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
		if (counter++ < 1000)
			goto l1;
	}
}

[TestClass]
public class ListHashSetTests
{
	[TestMethod]
	public void TestShuffle()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var toShuffle = new G.List<string>(new string[256].ToArray(x => new byte[random.Next(1, 17)].ToString(y => (char)random.Next(65536))));
		var a = toShuffle.ToHashSet();
		var b = a.Copy().Shuffle();
		var c = new G.List<string>(a);
		c = new(c.Shuffle());
		Assert.IsTrue(b.SetEquals(c));
		Assert.IsTrue(E.ToHashSet(b).SetEquals(c));
	}
}

[TestClass]
public class ParallelHashSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var arr = RedStarLinq.FillArray(16, _ => random.Next(16));
		var toInsert = Array.Empty<int>();
		ParallelHashSet<int> phs = new(arr);
		G.HashSet<int> gs = [.. arr];
		var collectionActions = new[] { (int[] arr) =>
		{
			phs.ExceptWith(arr);
			gs.ExceptWith(arr);
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		}, arr =>
		{
			phs.IntersectWith(arr);
			gs.IntersectWith(arr);
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		}, arr =>
		{
			phs.SymmetricExceptWith(arr);
			gs.SymmetricExceptWith(arr);
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		}, arr =>
		{
			phs.UnionWith(arr);
			gs.UnionWith(arr);
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		} };
		var collectionActions2 = new[] { () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(6), _ => random.Next(16));
			phs.AddRange(toInsert);
			gs.UnionWith(toInsert);
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		}, () =>
		{
			var n = random.Next(phs.Length);
			toInsert = RedStarLinq.FillArray(random.Next(6), _ => random.Next(16));
			phs.Insert(n, toInsert);
			gs.UnionWith(toInsert);
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		}, () =>
		{
			var length = Min(random.Next(9), phs.Length);
			if (phs.Length < length)
				return;
			var start = random.Next(phs.Length - length + 1);
			foreach (var item in phs.GetSlice(start, length))
				gs.Remove(item);
			phs.Remove(start, length);
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		} };
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			phs.Add(n);
			gs.Add(n);
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		}, () =>
		{
			if (phs.Length == 0) return;
			if (random.Next(2) == 0)
			{
				int n;
				do
					n = random.Next(phs.Size);
				while (!phs.IsValidIndex(n));
				gs.Remove(phs[n, suppressException : true]);
				phs.RemoveAt(n);
			}
			else
			{
				var n = random.Next(16);
				phs.RemoveValue(n);
				gs.Remove(n);
			}
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		}, () =>
		{
			var arr = RedStarLinq.FillArray(5, _ => random.Next(16));
			collectionActions.Random(random)(arr);
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		}, () =>
		{
			collectionActions2.Random(random)();
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		}, () =>
		{
			if (phs.Length == 0) return;
			int index;
			do
				index = random.Next(phs.Size);
			while (!phs.IsValidIndex(index));
			var n = random.Next(16);
			gs.Remove(phs[index, suppressException: true]);
			phs.RemoveValue(n);
			phs[index, suppressException: true] = n;
			gs.Add(n);
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		}, () =>
		{
			if (phs.Length == 0) return;
			int n;
			do
				n = random.Next(phs.Size);
			while (!phs.IsValidIndex(n));
			Assert.AreEqual(phs.IndexOf(phs[n, suppressException : true]), n);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 1000)
			goto l1;
	}

	[TestMethod]
	public void ConstructionTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var arr = RedStarLinq.FillArray(65536, _ => random.Next(24576));
			var phs = new ParallelHashSet<int>(arr);
			var gs = new G.HashSet<int>(arr);
			Assert.IsTrue(phs.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(phs));
		}
	}

	[TestMethod]
	public void CrashTest()
	{
		var random = RedStarLinq.FillArray(Environment.ProcessorCount, _ =>
			Lock(Global.lockObj, () => new Random(Global.random.Next())));
		var sw = Stopwatch.StartNew();
		var lockObj = RedStarLinq.FillArray(random.Length, _ => new object());
		var arr = RedStarLinq.FillArray(16, _ => random[0].Next(16));
		var toInsert = Array.Empty<int>();
		ParallelHashSet<int> phs = new(arr);
		var collectionActions = new[] { (int[] arr) => phs.ExceptWith(arr), phs.IntersectWith, phs.SymmetricExceptWith, phs.UnionWith };
		var collectionActions2 = new[] { (int tn) =>
		{
			lock (lockObj[tn % random.Length])
				toInsert = RedStarLinq.FillArray(random[tn].Next(6), _ => random[tn].Next(16));
			phs.AddRange(toInsert);
		}, tn =>
		{
			int n;
			lock (lockObj[tn % random.Length])
			{
				n = random[tn].Next(phs.Length);
				toInsert = RedStarLinq.FillArray(random[tn].Next(6), _ => random[tn].Next(16));
			}
			phs.Insert(n, toInsert);
		}, tn =>
		{
			int start, length;
			lock (lockObj[tn % random.Length])
			{
				length = Min(random[tn].Next(9), phs.Length);
				if (phs.Length < length)
					return;
				start = random[tn].Next(phs.Length - length + 1);
			}
			phs.Remove(start, length);
		} };
		var actions = new[] { (int tn) =>
		{
			int n;
			lock (lockObj[tn % random.Length])
				n = random[tn].Next(16);
			phs.Add(n);
		}, tn =>
		{
			int index, n;
			lock (lockObj[tn % random.Length])
			{
				index = random[tn].Next(phs.Size);
				n = random[tn].Next(16);
			}
			phs.Insert(index, n);
		}, tn =>
		{
			if (phs.Length == 0) return;
			int index;
			lock (lockObj[tn % random.Length])
			{
				do
					index = random[tn].Next(phs.Size);
				while (!phs.IsValidIndex(index));
			}
			phs.RemoveAt(index);
		}, tn =>
		{
			int n;
			lock (lockObj[tn % random.Length])
				n = random[tn].Next(16);
			phs.RemoveValue(n);
		}, tn =>
		{
			var arr = RedStarLinq.FillArray(5, _ => random[tn].Next(16));
			collectionActions.Random(random[tn])(arr);
		}, tn => collectionActions2.Random(random[tn])(tn) };
		Task.Factory.StartNew(() =>
		{
			var counter = 0;
		l1:
			Parallel.For(0, 1000, i =>
					{
						var tn = i % random.Length;
						actions.Random(random[tn])(tn);
					});
			if (counter++ < 1000)
				goto l1;
		}, TestContext.CancellationTokenSource.Token);
		Thread.Sleep(60000);
		return;
	}

	public TestContext TestContext { get; set; }
}

[TestClass]
public class TreeHashSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var arr = RedStarLinq.FillArray(16, _ => random.Next(16));
		TreeHashSet<int> ths = new(arr);
		G.HashSet<int> gs = [.. arr];
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
			var index = random.Next(ths.Length);
			var n = random.Next(16);
			if (ths[index] == n)
				return;
			gs.Remove(ths[index]);
			if (ths.TryGetIndexOf(n, out var index2) && index2 < index)
				index--;
			ths.RemoveValue(n);
			ths[index] = n;
			gs.Add(n);
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
		if (counter++ < 1000)
			goto l1;
	}
}
