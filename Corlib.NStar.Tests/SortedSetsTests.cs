
namespace Corlib.NStar.Tests;

[TestClass]
public class SumSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		try
		{
			var arr = RedStarLinq.FillArray(16, _ => (random.Next(16), random.Next(16)));
			SumSet<int> ss = new(arr);
			G.SortedSet<int> gs = new(arr.ToArray(x => x.Item1));
			G.List<int> gl = new(ss.Convert(x => x.Value));
			var collectionActions = new[] { ((int, int)[] arr) =>
			{
				gl = new(E.Where(gl, (x, index) => !Array.Exists(arr, x => x.Item1 == ss[index].Key)));
				ss.ExceptWith(arr);
				gs.ExceptWith(arr.ToArray(x => x.Item1));
			}, arr =>
			{
				gl = new(E.Where(gl, (x, index) => Array.Exists(arr, x => x.Item1 == ss[index].Key)));
				ss.IntersectWith(arr);
				gs.IntersectWith(arr.ToArray(x => x.Item1));
			}, arr =>
			{
				ss.SymmetricExceptWith(arr);
				gs.SymmetricExceptWith(arr.ToArray(x => x.Item1));
				gl = new(ss.Convert(x => x.Value));
			}, arr =>
			{
				ss.UnionWith(arr);
				gs.UnionWith(arr.ToArray(x => x.Item1));
				gl = new(ss.Convert(x => x.Value));
			} };
			var updateActions = new[] { (int key) =>
			{
				int newValue = random.Next(16);
				int index = ss.IndexOf(key);
				if (index != -1)
					gl[index] = newValue;
				ss.Update(key, newValue);
			}, key =>
			{
				int index = ss.Search(key);
				if (index >= 0)
					gl[index]++;
				else
					gl.Insert(~index, 1);
				ss.Increase(key);
				gs.Add(key);
			}, key =>
			{
				if (ss.Decrease(key))
					gl[ss.IndexOf(key)]--;
			} };
			var actions = new[] { () =>
			{
				int n = random.Next(16);
				int n2 = random.Next(16);
				int index = ss.Search(n);
				if (index < 0)
					gl.Insert(~index, n2);
				ss.Add(n, n2);
				gs.Add(n);
				Assert.IsTrue(RedStarLinq.Equals(ss, gs, (x, y) => x.Key == y));
				Assert.IsTrue(RedStarLinq.Equals(ss, gl, (x, y) => x.Value == y));
			}, () =>
			{
				if (ss.Length == 0) return;
				if (random.Next(2) == 0)
				{
					int n = random.Next(ss.Length);
					gs.Remove(ss[n].Key);
					ss.RemoveAt(n);
					gl.RemoveAt(n);
				}
				else
				{
					int n = random.Next(16);
					int index = ss.IndexOf(n);
					if (index != -1) gl.RemoveAt(index);
					ss.RemoveValue(n);
					gs.Remove(n);
				}
				Assert.IsTrue(RedStarLinq.Equals(ss, gs, (x, y) => x.Key == y));
				Assert.IsTrue(RedStarLinq.Equals(ss, gl, (x, y) => x.Value == y));
			}, () =>
			{
				var arr = RedStarLinq.FillArray(5, _ => (CreateVar(random.Next(16), out int key), ss.TryGetValue(key, out int value) ? value : random.Next(16)));
				collectionActions.Random(random)(arr);
				Assert.IsTrue(RedStarLinq.Equals(ss, gs, (x, y) => x.Key == y));
				Assert.IsTrue(RedStarLinq.Equals(ss, gl, (x, y) => x.Value == y));
			}, () =>
			{
				int n = random.Next(16);
				updateActions.Random(random)(n);
				Assert.IsTrue(RedStarLinq.Equals(ss, gs, (x, y) => x.Key == y));
				Assert.IsTrue(RedStarLinq.Equals(ss, gl, (x, y) => x.Value == y));
				Assert.AreEqual(ss.GetLeftValuesSum(n, out int value), E.Sum(E.Take(gl, ss.IndexOfNotLess(n))));
			}, () =>
			{
				if (ss.Length == 0) return;
				int n = random.Next(ss.Length);
				Assert.AreEqual(ss.IndexOf(ss[n]), n);
				Assert.AreEqual(ss[n].Value, gl[n]);
			} };
			for (int i = 0; i < 1000; i++)
				actions.Random(random)();
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}

[TestClass]
public class TreeSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		try
		{
			var arr = RedStarLinq.FillArray(16, _ => random.Next(16));
			TreeSet<int> ts = new(arr);
			G.SortedSet<int> gs = new(arr);
			var collectionActions = new[] { (int[] arr) =>
			{
				ts.ExceptWith(arr);
				gs.ExceptWith(arr);
			}, arr =>
			{
				ts.IntersectWith(arr);
				gs.IntersectWith(arr);
			}, arr =>
			{
				ts.SymmetricExceptWith(arr);
				gs.SymmetricExceptWith(arr);
			}, arr =>
			{
				ts.UnionWith(arr);
				gs.UnionWith(arr);
			} };
			var actions = new[] { () =>
			{
				int n = random.Next(16);
				ts.Add(n);
				gs.Add(n);
				Assert.IsTrue(RedStarLinq.Equals(ts, gs));
			}, () =>
			{
				if (ts.Length == 0) return;
				if (random.Next(2) == 0)
				{
					int n = random.Next(ts.Length);
					gs.Remove(ts[n]);
					ts.RemoveAt(n);
				}
				else
				{
					int n = random.Next(16);
					ts.RemoveValue(n);
					gs.Remove(n);
				}
				Assert.IsTrue(RedStarLinq.Equals(ts, gs));
			}, () =>
			{
				var arr = RedStarLinq.FillArray(5, _ => random.Next(16));
				collectionActions.Random(random)(arr);
				Assert.IsTrue(RedStarLinq.Equals(ts, gs));
			}, () =>
			{
				if (ts.Length == 0) return;
				int n = random.Next(ts.Length);
				Assert.AreEqual(ts.IndexOf(ts[n]), n);
			} };
			for (int i = 0; i < 1000; i++)
				actions.Random(random)();
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}
