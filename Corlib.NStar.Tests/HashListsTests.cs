
namespace Corlib.NStar.Tests;

[TestClass]
public class FastDelHashListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		FastDelHashList<int> fhs = [];
		G.List<int> gs = [];
		for (var i = 0; i < 100; i++)
		{
			var n = random.Next(16);
			fhs.Add(n);
			gs.Add(n);
		}
		Assert.IsTrue(RedStarLinq.Equals(gs, fhs));
		for (var i = 0; i < 10; i++)
		{
			var n = random.Next(16);
			fhs.RemoveValue(n);
			gs.Remove(n);
		}
		Assert.IsTrue(RedStarLinq.Equals(gs.Sort(x => x).ToList(), fhs.Sort(x => x).ToList()));
		for (var i = 0; i < 10; i++)
		{
			var n = random.Next(16);
			fhs.Add(n);
			gs.Add(n);
		}
		Assert.IsTrue(RedStarLinq.Equals(gs.Sort(x => x).ToList(), fhs.Sort(x => x).ToList()));
		for (var i = 0; i < 100; i++)
		{
			var n = random.Next(fhs.Length);
			Assert.AreEqual(fhs[fhs.IndexOf(fhs[n])], fhs[n]);
		}
	}
}

[TestClass]
public class HashListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var arr = RedStarLinq.FillArray(100, _ => random.Next(16));
		HashList<int> hl = new(arr);
		G.List<int> gl = new(arr);
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			hl.Add(n);
			gl.Add(n);
			Assert.IsTrue(hl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, hl));
		}, () =>
		{
			if (hl.Length == 0) return;
			if (random.Next(2) == 0)
			{
				var n = random.Next(hl.Length);
				hl.RemoveAt(n);
				gl.RemoveAt(n);
			}
			else
			{
				var n = random.Next(16);
				hl.RemoveValue(n);
				gl.Remove(n);
			}
			Assert.IsTrue(hl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, hl));
		}, () =>
		{
			if (hl.Length == 0)
				return;
			var n = random.Next(hl.Length);
			var n2 = random.Next(16);
			if (hl[n] == n2)
				return;
			hl[n] = n2;
			gl[n] = n2;
			Assert.IsTrue(hl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, hl));
		}, () =>
		{
			if (hl.Length == 0) return;
			var n = random.Next(hl.Length);
			Assert.AreEqual(hl[hl.IndexOf(hl[n])], hl[n]);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
	}
}
