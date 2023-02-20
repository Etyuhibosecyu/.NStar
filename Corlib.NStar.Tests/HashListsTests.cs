
namespace Corlib.NStar.Tests;

[TestClass]
public class FakeIndAftDelHashListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		try
		{
			FakeIndAftDelHashList<int> fhs = new();
			G.List<int> gs = new();
			for (int i = 0; i < 100; i++)
			{
				int n = random.Next(16);
				fhs.Add(n);
				gs.Add(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(gs, fhs));
			for (int i = 0; i < 10; i++)
			{
				int n = random.Next(16);
				fhs.RemoveValue(n);
				gs.Remove(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(gs, fhs));
			for (int i = 0; i < 10; i++)
			{
				int n = random.Next(16);
				fhs.Add(n);
				gs.Add(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(gs, fhs));
			for (int i = 0; i < 100; i++)
			{
				int n;
				do
				{
					n = random.Next(fhs.Length);
				} while (fhs[n] == 0);
				Assert.AreEqual(fhs.IndexOf(fhs[n]), n);
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}

[TestClass]
public class SlowDeletionHashListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		try
		{
			SlowDeletionHashList<int> shs = new();
			G.List<int> gs = new();
			for (int i = 0; i < 100; i++)
			{
				int n = random.Next(16);
				shs.Add(n);
				gs.Add(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(gs, shs));
			for (int i = 0; i < 10; i++)
			{
				int n = random.Next(16);
				shs.RemoveValue(n);
				gs.Remove(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(gs, shs));
			for (int i = 0; i < 10; i++)
			{
				int n = random.Next(16);
				shs.Add(n);
				gs.Add(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(gs, shs));
			G.List<int> list = new(shs);
			for (int i = 0; i < 10; i++)
			{
				int n = random.Next(shs.Length);
				int n2 = random.Next(16);
				if (CreateVar(shs.IndexOf(n2), out int foundIndex) == n || foundIndex == -1)
				{
					shs[n] = n2;
					list[n] = n2;
				}
			}
			gs = new(list);
			Assert.IsTrue(RedStarLinq.Equals(gs, shs));
			for (int i = 0; i < 100; i++)
			{
				int n;
				do
				{
					n = random.Next(shs.Length);
				} while (shs[n] == 0);
				Assert.AreEqual(shs[shs.IndexOf(shs[n])], shs[n]);
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}
