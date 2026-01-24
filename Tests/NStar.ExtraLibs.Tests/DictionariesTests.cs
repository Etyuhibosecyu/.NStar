namespace NStar.ExtraLibs.Tests;

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

	[TestMethod]
	public void ConstructionTest()
	{
		var seed = Lock(lockObj, Global.random.Next);
		var random = Lock(lockObj, () => new Random(seed));
		var random2 = Lock(lockObj, () => new Random(seed));
		var funcs = new[]
		{
			(Random random) => new Dictionary<int, int>(), random => new(1600), random => new(1600),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next()))),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.FillArray(random.Next(5, 50000), _ => (random.Next(), random.Next())).RemoveDoubles(x => x.Item1 / 2).ToArray(), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new(RedStarLinq.Fill(random.Next(50), _ => (random.Next(), random.Next()))),
			random => new(RedStarLinq.Fill(random.Next(50), _ => (random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.Fill(random.Next(5, 50000), _ => (random.Next(), random.Next())).RemoveDoubles(x => x.Item1 / 2), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new((G.IEnumerable<(int, int)>)RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next()))),
			random => new((G.IEnumerable<(int, int)>)RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new((G.IEnumerable<(int, int)>)RedStarLinq.FillArray(random.Next(5, 50000), _ => (random.Next(), random.Next())).RemoveDoubles(x => x.Item1 / 2).ToArray(), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new((G.IEnumerable<(int, int)>)RedStarLinq.Fill(random.Next(50), _ => (random.Next(), random.Next()))),
			random => new((G.IEnumerable<(int, int)>)RedStarLinq.Fill(random.Next(50), _ => (random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new((G.IEnumerable<(int, int)>)RedStarLinq.Fill(random.Next(5, 50000), _ => (random.Next(), random.Next())).RemoveDoubles(x => x.Item1 / 2), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), x => x)),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), x => x), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(5, 50000), _ => (random.Next(), random.Next())), x => x).RemoveDoubles(x => x.Item1 / 2), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), _ => random.Next(10) == -1)),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(5, 50000), _ => (random.Next(), random.Next())).RemoveDoubles(x => x.Item1 / 2), _ => random.Next(10) == -1), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2).ToArray(), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2).ToArray(), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x)),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), x => x), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1)),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), _ => random.Next(10) == -1), (x, y) => x / 2 == y / 2, x => x / 2),
			random => new(new Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())))),
			random => new(new Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>(RedStarLinq.FillArray(random.Next(5, 50000), _ => (random.Next(), random.Next())).RemoveDoubles(x => x.Item1 / 2).ToArray(), (x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => (random.Next(), random.Next())))),
			random => new(new Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => (random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>(RedStarLinq.Fill(random.Next(5, 50000), _ => (random.Next(), random.Next())).RemoveDoubles(x => x.Item1 / 2), (x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new Dictionary<int, int>((G.IEnumerable<(int, int)>)RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())))),
			random => new(new Dictionary<int, int>((G.IEnumerable<(int, int)>)RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>((G.IEnumerable<(int, int)>)RedStarLinq.FillArray(random.Next(5, 50000), _ => (random.Next(), random.Next())).RemoveDoubles(x => x.Item1 / 2).ToArray(), (x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new Dictionary<int, int>((G.IEnumerable<(int, int)>)RedStarLinq.Fill(random.Next(50), _ => (random.Next(), random.Next())))),
			random => new(new Dictionary<int, int>((G.IEnumerable<(int, int)>)RedStarLinq.Fill(random.Next(50), _ => (random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>((G.IEnumerable<(int, int)>)RedStarLinq.Fill(random.Next(5, 50000), _ => (random.Next(), random.Next())).RemoveDoubles(x => x.Item1 / 2), (x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), x => x))),
			random => new(new Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), x => x), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(5, 50000), _ => (random.Next(), random.Next())), x => x).RemoveDoubles(x => x.Item1 / 2), (x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), _ => random.Next(10) == -1))),
			random => new(new Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => (random.Next(), random.Next())), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(5, 50000), _ => (random.Next(), random.Next())).RemoveDoubles(x => x.Item1 / 2), _ => random.Next(10) == -1), (x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2).ToArray(), (x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>(RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), (x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new Dictionary<int, int>((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new Dictionary<int, int>((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2).ToArray(), (x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new Dictionary<int, int>((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new Dictionary<int, int>((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>((G.IEnumerable<G.KeyValuePair<int, int>>)RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), (x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x))),
			random => new(new Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), x => x), (x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1))),
			random => new(new Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), _ => random.Next(10) == -1), (x, y) => x / 2 == y / 2, x => x / 2)),
		};
		var funcs2 = new[]
		{
			(Random random) => new G.Dictionary<int, int>(), random => new(1600), random => new(1600),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x)),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), x => x), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1)),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next()))),
			random => new(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x)),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.Select(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1)),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x))),
			random => new(new G.Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), x => x), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1))),
			random => new(new G.Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(RedStarLinq.Fill(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x))),
			random => new(new G.Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), x => x), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(E.Select(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), x => x), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1))),
			random => new(new G.Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(50), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
			random => new(new G.Dictionary<int, int>(E.SkipWhile(RedStarLinq.FillArray(random.Next(5, 50000), _ => new G.KeyValuePair<int, int>(random.Next(), random.Next())).RemoveDoubles(x => x.Key / 2), _ => random.Next(10) == -1), new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2))),
		};
		for (var i = 0; i < 5000; i++)
		{
			var a = funcs.Random(random)(random);
			var b = funcs.Random(random2)(random2);
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a.TryAdd(random.Next(), random.Next());
			b.TryAdd(random2.Next(), random2.Next());
		}
	}

	[TestMethod]
	public void TestTuples()
	{
		var a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"));
		var b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK"), ("LLL", "LLL"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK"), ("LLL", "LLL")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK"), ("LLL", "LLL"), ("MMM", "MMM"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK"), ("LLL", "LLL"), ("MMM", "MMM")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK"), ("LLL", "LLL"), ("MMM", "MMM"), ("NNN", "NNN"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK"), ("LLL", "LLL"), ("MMM", "MMM"), ("NNN", "NNN")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK"), ("LLL", "LLL"), ("MMM", "MMM"), ("NNN", "NNN"), ("OOO", "OOO"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK"), ("LLL", "LLL"), ("MMM", "MMM"), ("NNN", "NNN"), ("OOO", "OOO")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (Dictionary<string, string>)(("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK"), ("LLL", "LLL"), ("MMM", "MMM"), ("NNN", "NNN"), ("OOO", "OOO"), ("PPP", "PPP"));
		b = new Dictionary<string, string>([("AAA", "AAA"), ("BBB", "BBB"), ("CCC", "CCC"), ("DDD", "DDD"), ("EEE", "EEE"), ("FFF", "FFF"), ("GGG", "GGG"), ("HHH", "HHH"), ("III", "III"), ("JJJ", "JJJ"), ("KKK", "KKK"), ("LLL", "LLL"), ("MMM", "MMM"), ("NNN", "NNN"), ("OOO", "OOO"), ("PPP", "PPP")]);
		Assert.IsTrue(RedStarLinq.Equals(a, b));
		Assert.IsTrue(E.SequenceEqual(b, a));
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
		var actions = new[] { () =>
		{
			if (random.Next(25) == 0)
			{
				mir.Clear();
				dic.Clear();
				dic2.Clear();
			}
			Assert.AreEqual(mir.Length, dic.Count);
			Assert.AreEqual(mir.Length, dic2.Count);
			Assert.IsTrue(RedStarLinq.Equals(mir, dic, (x, y) => x.Key == y.Key && x.Value == y.Value));
			Assert.IsTrue(RedStarLinq.Equals(mir, dic2, (x, y) => x.Key == y.Value && x.Value == y.Key));
		}, () =>
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
			if (random.Next(2) == 0)
			{
				var n = random.Next(16);
				var b = mir.TryGetValue(n, out var value);
				if (!b) return;
				mir.RemoveKey(n, out var value2);
				Assert.AreEqual(value, value2);
				dic.Remove(n);
				dic2.Remove(value);
			}
			else
			{
				var n = random.Next(16);
				var b = mir.TryGetKey(n, out var key);
				if (!b) return;
				mir.RemoveValue(n, out var key2);
				Assert.AreEqual(key, key2);
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
