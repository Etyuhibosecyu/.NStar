
namespace Corlib.NStar.Tests;

[TestClass]
public class DictionaryTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var arr = RedStarLinq.FillArray(random.Next(1, 100), _ => (random.Next(100), random.Next(1, 100)));
		Dictionary<int, int> dic = new(arr);
		var dic2 = E.ToDictionary(arr.RemoveDoubles(x => x.Item1), x => x.Item1, x => x.Item2);
		var bytes = new byte[100];
		var collectionActions = new[] { ((int, int)[] arr) =>
		{
			dic.ExceptWith(arr);
			foreach (var x in arr)
				if (dic2.TryGetValue(x.Item1, out var x2) && x.Item2 == x2)
					dic2.Remove(x.Item1);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}, arr =>
		{
			dic.IntersectWith(arr);
			foreach (var x in dic2)
				if (Array.IndexOf(arr, (x.Key, x.Value)) < 0)
					dic2.Remove(x.Key);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}/*, arr =>
		{
			dic.SymmetricExceptWith(arr);
			dic2.SymmetricExceptWith(arr.ToArray(x => x.Item1));
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}*/, arr =>
		{
			dic.UnionWith(arr);
			foreach (var x in arr)
				dic2[x.Item1] = x.Item2;
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		} };
		var actions = new[] { () =>
		{
			var n = random.Next(100);
			var n2 = random.Next(100);
			if (dic.TryAdd(n, n2))
				dic2.Add(n, n2);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}, () =>
		{
			if (dic.Length == 0) return;
			var n = random.Next(100);
			var n2 = random.Next(100);
			dic[n] = n2;
			dic2[n] = n2;
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}, () =>
		{
			if (dic.Length == 0) return;
			var n = random.Next(100);
			var b = dic.TryGetValue(n, out var value);
			if (!b) return;
			dic.Remove(n);
			dic2.Remove(n);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}, () =>
		{
			var arr = RedStarLinq.FillArray(25, _ => (CreateVar(random.Next(16), out var key), dic.TryGetValue(key, out var value) ? value : random.Next(1, 16)));
			collectionActions.Random(random)(arr);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 1000)
			goto l1;
	}
}

[TestClass]
public class MirrorTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var arr = RedStarLinq.Fill(16, _ => (random.Next(16), random.Next(1, 16))).RemoveDoubles(x => x.Item1).RemoveDoubles(x => x.Item2).ToArray();
		Mirror<int, int> mir = new(arr);
		var dic = E.ToDictionary(arr, x => x.Item1, x => x.Item2);
		var dic2 = E.ToDictionary(arr, x => x.Item2, x => x.Item1);
		var bytes = new byte[16];
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			var n2 = random.Next(16);
			if (mir.TryAdd(n, n2))
			{
				if (mir.TryGetValue(n, out var value))
					dic2.Remove(value);
				if (mir.TryGetKey(n2, out var key))
					dic.Remove(key);
				dic.Add(n, n2);
				dic2.Add(n2, n);
			}
			Assert.AreEqual(mir.Length, dic.Count);
			Assert.AreEqual(mir.Length, dic2.Count);
			Assert.IsTrue(RedStarLinq.Equals(mir, dic, (x, y) => x.Key == y.Key && x.Value == y.Value));
			Assert.IsTrue(RedStarLinq.Equals(mir, dic2, (x, y) => x.Key == y.Value && x.Value == y.Key));
		}, () =>
		{
			if (mir.Length == 0) return;
			var n = random.Next(16);
			var n2 = random.Next(16);
			if (dic2.TryGetValue(n2, out var key))
				dic.Remove(key);
			if (dic.TryGetValue(n, out var value))
			{
				dic2.Remove(n2);
				dic2.Remove(value);
			}
			if (random.Next(2) == 0)
			{
				mir.SetValue(n, n2);
				dic[n] = n2;
				dic2[n2] = n;
			}
			else
			{
				mir.SetKey(n2, n);
				dic[n] = n2;
				dic2[n2] = n;
			}
			Assert.AreEqual(mir.Length, dic.Count);
			Assert.AreEqual(mir.Length, dic2.Count);
			Assert.IsTrue(RedStarLinq.Equals(mir, dic, (x, y) => x.Key == y.Key && x.Value == y.Value));
			Assert.IsTrue(RedStarLinq.Equals(mir, dic2, (x, y) => x.Key == y.Value && x.Value == y.Key));
		}, () =>
		{
			if (mir.Length == 0) return;
			if (random.Next(2) == 0)
			{
				var n = random.Next(16);
				var b = mir.TryGetValue(n, out var value);
				if (!b) return;
				mir.RemoveKey(n);
				dic.Remove(n);
				dic2.Remove(value);
			}
			else
			{
				var n = random.Next(16);
				var b = mir.TryGetKey(n, out var key);
				if (!b) return;
				mir.RemoveValue(n);
				dic.Remove(key);
				dic2.Remove(n);
			}
			Assert.AreEqual(mir.Length, dic.Count);
			Assert.AreEqual(mir.Length, dic2.Count);
			Assert.IsTrue(RedStarLinq.Equals(mir, dic, (x, y) => x.Key == y.Key && x.Value == y.Value));
			Assert.IsTrue(RedStarLinq.Equals(mir, dic2, (x, y) => x.Key == y.Value && x.Value == y.Key));
		}, () =>
		{
			if (mir.Length == 0) return;
			var n = random.Next(mir.Length);
			Assert.AreEqual(mir.IndexOf(mir.ElementAt(n)), n);
			Assert.AreEqual(mir.ElementAt(n).Key, dic2.ElementAt(n).Value);
			Assert.AreEqual(mir.ElementAt(n).Value, dic2.ElementAt(n).Key);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 1000)
			goto l1;
	}
}

[TestClass]
public class SortedDictionaryTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var arr = RedStarLinq.FillArray(random.Next(1, 100), _ => (random.Next(100), random.Next(1, 100)));
		SortedDictionary<int, int> dic = new(arr);
		var dic2 = E.ToDictionary(arr.RemoveDoubles(x => x.Item1), x => x.Item1, x => x.Item2);
		var bytes = new byte[100];
		var collectionActions = new[] { ((int, int)[] arr) =>
		{
			dic.ExceptWith(arr);
			foreach (var x in arr)
				if (dic2.TryGetValue(x.Item1, out var x2) && x.Item2 == x2)
					dic2.Remove(x.Item1);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}, arr =>
		{
			dic.IntersectWith(arr);
			foreach (var x in dic2)
				if (Array.IndexOf(arr, (x.Key, x.Value)) < 0)
					dic2.Remove(x.Key);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}/*, arr =>
		{
			dic.SymmetricExceptWith(arr);
			dic2.SymmetricExceptWith(arr.ToArray(x => x.Item1));
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}*/, arr =>
		{
			dic.UnionWith(arr);
			foreach (var x in arr)
				dic2[x.Item1] = x.Item2;
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		} };
		var actions = new[] { () =>
		{
			var n = random.Next(100);
			var n2 = random.Next(100);
			if (dic.TryAdd(n, n2))
				dic2.Add(n, n2);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}, () =>
		{
			if (dic.Length == 0) return;
			var n = random.Next(100);
			var n2 = random.Next(100);
			dic[n] = n2;
			dic2[n] = n2;
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}, () =>
		{
			if (dic.Length == 0) return;
			var n = random.Next(100);
			var b = dic.TryGetValue(n, out var value);
			if (!b) return;
			dic.Remove(n);
			dic2.Remove(n);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		}, () =>
		{
			var arr = RedStarLinq.FillArray(25, _ => (CreateVar(random.Next(16), out var key), dic.TryGetValue(key, out var value) ? value : random.Next(1, 16)));
			collectionActions.Random(random)(arr);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value == value));
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 1000)
			goto l1;
	}
}
