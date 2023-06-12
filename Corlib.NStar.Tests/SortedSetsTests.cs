
namespace Corlib.NStar.Tests;

[TestClass]
public class SumSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var arr = RedStarLinq.FillArray(16, _ => (random.Next(16), random.Next(1, 16)));
		SumSet<int> ss = new(arr);
		G.SortedSet<int> gs = new(arr.ToArray(x => x.Item1));
		G.List<int> gl = new(ss.Convert(x => x.Value));
		var bytes = new byte[16];
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
			var newValue = random.Next(16);
			var index = ss.IndexOf(key);
			if (index != -1 && newValue <= 0)
			{
				gs.Remove(key);
				gl.RemoveAt(index);
			}
			else if (index != -1)
				gl[index] = newValue;
			ss.Update(key, newValue);
		}, key =>
		{
			var index = ss.Search(key);
			if (index >= 0)
				gl[index]++;
			else
				gl.Insert(~index, 1);
			ss.Increase(key);
			gs.Add(key);
		}, key =>
		{
			var index = ss.IndexOf(key);
			if (ss.TryGetValue(key, out var value) && value == 1)
			{
				gs.Remove(key);
				gl.RemoveAt(index);
			}
			else if (index != -1)
				gl[index]--;
			ss.Decrease(key);
		} };
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			var n2 = random.Next(1, 16);
			var index = ss.Search(n);
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
				var n = random.Next(ss.Length);
				gs.Remove(ss[n].Key);
				ss.RemoveAt(n);
				gl.RemoveAt(n);
			}
			else
			{
				var n = random.Next(16);
				var index = ss.IndexOf(n);
				if (index != -1) gl.RemoveAt(index);
				ss.RemoveValue(n);
				gs.Remove(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(ss, gs, (x, y) => x.Key == y));
			Assert.IsTrue(RedStarLinq.Equals(ss, gl, (x, y) => x.Value == y));
		}, () =>
		{
			var arr = RedStarLinq.FillArray(5, _ => (CreateVar(random.Next(16), out var key), ss.TryGetValue(key, out var value) ? value : random.Next(1, 16)));
			collectionActions.Random(random)(arr);
			Assert.IsTrue(RedStarLinq.Equals(ss, gs, (x, y) => x.Key == y));
			Assert.IsTrue(RedStarLinq.Equals(ss, gl, (x, y) => x.Value == y));
		}, () =>
		{
			var n = random.Next(16);
			updateActions.Random(random)(n);
			Assert.IsTrue(RedStarLinq.Equals(ss, gs, (x, y) => x.Key == y));
			Assert.IsTrue(RedStarLinq.Equals(ss, gl, (x, y) => x.Value == y));
			Assert.AreEqual(ss.GetLeftValuesSum(n, out var value), E.Sum(E.Take(gl, ss.IndexOfNotLess(n))));
		}, () =>
		{
			random.NextBytes(bytes);
			var index = ss.IndexOfNotGreaterSum(CreateVar((long)(new mpz_t(bytes, 1) % (ss.ValuesSum + 1)), out var sum));
			Assert.IsTrue(index == gl.Count && sum == E.Sum(gl) || CreateVar(E.Sum(E.Take(gl, index)), out var sum2) <= sum && (gl[index] == 0 || sum2 + gl[index] > sum));
		}, () =>
		{
			if (ss.Length == 0) return;
			var n = random.Next(ss.Length);
			Assert.AreEqual(ss.IndexOf(ss[n]), n);
			Assert.AreEqual(ss[n].Value, gl[n]);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
	}
}

[TestClass]
public class TreeSetTests
{
	[TestMethod]
	public void ComplexTest()
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
			var n = random.Next(16);
			ts.Add(n);
			gs.Add(n);
			Assert.IsTrue(RedStarLinq.Equals(ts, gs));
		}, () =>
		{
			if (ts.Length == 0) return;
			if (random.Next(2) == 0)
			{
				var n = random.Next(ts.Length);
				gs.Remove(ts[n]);
				ts.RemoveAt(n);
			}
			else
			{
				var n = random.Next(16);
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
			var n = random.Next(ts.Length);
			Assert.AreEqual(ts.IndexOf(ts[n]), n);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
	}
}
