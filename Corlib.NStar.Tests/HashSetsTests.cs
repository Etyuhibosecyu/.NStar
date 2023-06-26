
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
