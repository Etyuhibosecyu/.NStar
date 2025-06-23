namespace ExtraLibs.NStar.Tests;

public static class BaseDictionaryTests<TKey, TValue, TCertain> where TKey : notnull where TValue : IEquatable<TValue> where TCertain : BaseDictionary<TKey, TValue, TCertain>, new()
{
	public static void ComplexTest(Func<(TCertain, (TKey Key, TValue Value)[])> create, Func<TKey> newKeyFunc, Func<TValue> newValueFunc, Func<(TKey Key, TValue Value)> newKeyAndValueFunc)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var (dic, arr) = create();
		var dic2 = E.ToDictionary(arr.RemoveDoubles(x => x.Key), x => x.Key, x => x.Value);
		var bytes = new byte[100];
		var collectionActions = new[] { ((TKey Key, TValue Value)[] arr) =>
		{
			dic.ExceptWith(arr);
			foreach (var x in arr)
				if (dic2.TryGetValue(x.Key, out var x2) && x.Value.Equals(x2))
					dic2.Remove(x.Key);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value.Equals(value)));
		}, arr =>
		{
			dic.IntersectWith(arr);
			foreach (var x in dic2)
				if (Array.IndexOf(arr, (x.Key, x.Value)) < 0)
					dic2.Remove(x.Key);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value.Equals(value)));
		//}, arr =>
		//{
		//	dic.SymmetricExceptWith(arr);
		//	foreach (var x in E.DistinctBy(arr, x => x.Key))
		//	{
		//		var result = dic2.ContainsKey(x.Key) ? dic2.Remove(x.Key) : dic2.TryAdd(x.Key, x.Value);
		//		Assert.IsTrue(result);
		//	}
		//	Assert.AreEqual(dic.Length, dic2.Count);
		//	Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value.Equals(value)));
		}, arr =>
		{
			dic.UnionWith(arr);
			foreach (var x in arr)
				dic2[x.Key] = x.Value;
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value.Equals(value)));
		} };
		var actions = new[] { () =>
		{
			var (index, n) = newKeyAndValueFunc();
			if (dic.TryAdd(index, n))
				dic2.Add(index, n);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value.Equals(value)));
		}, () =>
		{
			if (dic.Length == 0) return;
			var (index, n) = newKeyAndValueFunc();
			dic[index] = n;
			dic2[index] = n;
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value.Equals(value)));
		}, () =>
		{
			if (dic.Length == 0) return;
			var (n, _) = newKeyAndValueFunc();
			var b = dic.TryGetValue(n, out var value);
			if (!b) return;
			dic.Remove(n);
			dic2.Remove(n);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value.Equals(value)));
		}, () =>
		{
			var arr = RedStarLinq.FillArray(25, _ => (CreateVar(newKeyFunc(), out var key), dic.TryGetValue(key, out var value) ? value : newValueFunc()));
			collectionActions.Random(random)(arr);
			Assert.AreEqual(dic.Length, dic2.Count);
			Assert.IsTrue(dic.All(x => dic2.TryGetValue(x.Key, out var value) && x.Value.Equals(value)));
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 1000)
			goto l1;
	}
}

[TestClass]
public class DictionaryTests
{
	[TestMethod]
	public void ComplexTest() => BaseDictionaryTests<int, int, Dictionary<int, int>>.ComplexTest(() =>
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var arr = RedStarLinq.FillArray(random.Next(1, 100), _ => (random.Next(100), random.Next(1, 100)));
		Dictionary<int, int> dic = new(arr);
		return (dic, arr);
	}, () => random.Next(16), () => random.Next(1, 16), () => (random.Next(100), random.Next(100)));
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
			var index = random.Next(16);
			var n = random.Next(16);
			if (mir.TryAdd(index, n))
			{
				if (mir.TryGetValue(index, out var value))
					dic2.Remove(value);
				if (mir.TryGetKey(n, out var key))
					dic.Remove(key);
				dic.Add(index, n);
				dic2.Add(n, index);
			}
			Assert.AreEqual(mir.Length, dic.Count);
			Assert.AreEqual(mir.Length, dic2.Count);
			Assert.IsTrue(RedStarLinq.Equals(mir, dic, (x, y) => x.Key == y.Key && x.Value == y.Value));
			Assert.IsTrue(RedStarLinq.Equals(mir, dic2, (x, y) => x.Key == y.Value && x.Value == y.Key));
		}, () =>
		{
			if (mir.Length == 0) return;
			var index = random.Next(16);
			var n = random.Next(16);
			if (dic2.TryGetValue(n, out var key))
				dic.Remove(key);
			if (dic.TryGetValue(index, out var value))
			{
				dic2.Remove(n);
				dic2.Remove(value);
			}
			if (random.Next(2) == 0)
			{
				mir.SetValue(index, n);
				dic[index] = n;
				dic2[n] = index;
			}
			else
			{
				mir.SetKey(n, index);
				dic[index] = n;
				dic2[n] = index;
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
	public void ComplexTest() => BaseDictionaryTests<int, int, SortedDictionary<int, int>>.ComplexTest(() =>
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var arr = RedStarLinq.FillArray(random.Next(1, 100), _ => (random.Next(100), random.Next(1, 100)));
		SortedDictionary<int, int> dic = new(arr);
		return (dic, arr);
	}, () => random.Next(16), () => random.Next(1, 16), () => (random.Next(100), random.Next(100)));
}
