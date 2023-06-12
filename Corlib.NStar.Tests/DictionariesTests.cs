
namespace Corlib.NStar.Tests;

[TestClass]
public class MirrorTests
{
	[TestMethod]
	public void ComplexTest()
	{
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
			if (random.Next(2) == 0)
			{
				if (dic2.TryGetValue(n2, out var key))
					dic.Remove(key);
				if (dic.TryGetValue(n, out var value))
				{
					dic2.Remove(n2);
					dic2.Remove(value);
				}
				mir.SetValue(n, n2);
				dic[n] = n2;
				dic2[n2] = n;
			}
			else
			{
				if (dic2.TryGetValue(n2, out var key))
					dic.Remove(key);
				if (dic.TryGetValue(n, out var value))
				{
					dic2.Remove(n2);
					dic2.Remove(value);
				}
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
	}
}
