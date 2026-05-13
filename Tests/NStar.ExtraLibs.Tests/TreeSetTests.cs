namespace NStar.ExtraLibs.Tests;

[TestClass]
public class TreeSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var arr = RedStarLinq.FillArray(16, _ => random.Next(16));
		TreeSet<int> ts = new(arr);
		G.SortedSet<int> gs = new(arr);
		var collectionActions = new[] { (int[] arr) =>
		{
			ts.ExceptWith(arr);
			gs.ExceptWith(arr);
			Assert.IsTrue(RedStarLinq.Equals(ts, gs));
		}, arr =>
		{
			ts.IntersectWith(arr);
			gs.IntersectWith(arr);
			Assert.IsTrue(RedStarLinq.Equals(ts, gs));
		}, arr =>
		{
			ts.SymmetricExceptWith(arr);
			gs.SymmetricExceptWith(arr);
			Assert.IsTrue(RedStarLinq.Equals(ts, gs));
		}, arr =>
		{
			ts.UnionWith(arr);
			gs.UnionWith(arr);
			Assert.IsTrue(RedStarLinq.Equals(ts, gs));
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
		if (counter++ < 1000)
			goto l1;
	}
}
