namespace NStar.BigCollections.Tests;

[TestClass]
public class BigQueueTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var arr = RedStarLinq.FillArray(random.Next(100), _ => random.Next(16));
		BigQueue<int> bq = new(arr, random.Next(2, 7), random.Next(1, 7));
		G.Queue<int> gq = new(arr);
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			if (random.Next(25) == 0)
			{
				bq.Clear();
				gq.Clear();
				if (random.Next(2) == 0)
					bq.TrimExcess();
			}
			else
			{
				var clone = (BigQueue<int>)bq.Clone();
				Assert.IsTrue(RedStarLinq.Equals(clone, gq));
				Assert.IsTrue(E.SequenceEqual(gq, clone));
			}
			Assert.IsTrue(RedStarLinq.Equals(bq, gq));
			Assert.IsTrue(E.SequenceEqual(gq, bq));
		}, () =>
		{
			var n = random.Next(16);
			Assert.AreEqual(gq.Contains(n), bq.Contains(n));
			Assert.IsTrue(RedStarLinq.Equals(bq, gq));
			Assert.IsTrue(E.SequenceEqual(gq, bq));
		}, () =>
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
					Assert.ThrowsExactly<InvalidOperationException>(() => bq.Dequeue());
				else
					Assert.AreEqual(bq.Dequeue(), gq.Dequeue());
				if (random.Next(25) == 0)
					bq.TrimExcess();
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
					Assert.IsEmpty(gq);
				}
				if (random.Next(25) == 0)
					bq.TrimExcess();
				Assert.IsTrue(RedStarLinq.Equals(bq, gq));
				Assert.IsTrue(E.SequenceEqual(gq, bq));
			}
		}, () =>
		{
			if (random.Next(2) == 0)
			{
				if (bq.Length == 0)
					Assert.ThrowsExactly<InvalidOperationException>(() => bq.Peek());
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
					Assert.IsEmpty(gq);
				}
				Assert.IsTrue(RedStarLinq.Equals(bq, gq));
				Assert.IsTrue(E.SequenceEqual(gq, bq));
			}
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 1000)
			goto l1;
	}
}
