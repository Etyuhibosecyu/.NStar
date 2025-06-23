namespace BigCollections.NStar.Tests;

[TestClass]
public class BigQueueTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var arr = RedStarLinq.FillArray(random.Next(17), _ => random.Next(16));
		BigQueue<int> bq = new(arr);
		G.Queue<int> gq = new(arr);
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			bq.Enqueue(n);
			gq.Enqueue(n);
			Assert.IsTrue(RedStarLinq.Equals(bq, gq));
			Assert.IsTrue(E.SequenceEqual(gq, bq));
		}, () =>
		{
			if (random.Next(2) == 0)
			{
				if (bq.Length == 0)
					Assert.ThrowsException<InvalidOperationException>(() => bq.Dequeue());
				else
					Assert.AreEqual(bq.Dequeue(), gq.Dequeue());
				Assert.IsTrue(RedStarLinq.Equals(bq, gq));
				Assert.IsTrue(E.SequenceEqual(gq, bq));
			}
			else
			{
				if (bq.TryDequeue(out var value))
					Assert.AreEqual(value, gq.Dequeue());
				else
				{
					Assert.AreEqual(0, bq.Length);
					Assert.AreEqual(0, gq.Count);
				}
				Assert.IsTrue(RedStarLinq.Equals(bq, gq));
				Assert.IsTrue(E.SequenceEqual(gq, bq));
			}
		}, () =>
		{
			if (random.Next(2) == 0)
			{
				if (bq.Length == 0)
					Assert.ThrowsException<InvalidOperationException>(() => bq.Peek());
				else
					Assert.AreEqual(bq.Peek(), gq.Peek());
				Assert.IsTrue(RedStarLinq.Equals(bq, gq));
				Assert.IsTrue(E.SequenceEqual(gq, bq));
			}
			else
			{
				if (bq.TryPeek(out var value))
					Assert.AreEqual(value, gq.Peek());
				else
				{
					Assert.AreEqual(0, bq.Length);
					Assert.AreEqual(0, gq.Count);
				}
				Assert.IsTrue(RedStarLinq.Equals(bq, gq));
				Assert.IsTrue(E.SequenceEqual(gq, bq));
			}
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 10)
			goto l1;
	}
}
