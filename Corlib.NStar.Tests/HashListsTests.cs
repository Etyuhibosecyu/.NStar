
namespace Corlib.NStar.Tests;

[TestClass]
public class FastDelHashListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		try
		{
			FastDelHashList<int> fhs = new();
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
			Assert.IsTrue(RedStarLinq.Equals(gs.Sort(x => x).ToList(), fhs.Sort(x => x).ToList()));
			for (int i = 0; i < 10; i++)
			{
				int n = random.Next(16);
				fhs.Add(n);
				gs.Add(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(gs.Sort(x => x).ToList(), fhs.Sort(x => x).ToList()));
			for (int i = 0; i < 100; i++)
			{
				int n = random.Next(fhs.Length);
				Assert.AreEqual(fhs[fhs.IndexOf(fhs[n])], fhs[n]);
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}

[TestClass]
public class HashListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		try
		{
			HashList<int> shs = new();
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
			Assert.IsTrue(RedStarLinq.Equals(gs.Sort(x => x).ToList(), shs.Sort(x => x).ToList()));
			for (int i = 0; i < 10; i++)
			{
				int n = random.Next(16);
				shs.Add(n);
				gs.Add(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(gs.Sort(x => x).ToList(), shs.Sort(x => x).ToList()));
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
			Assert.IsTrue(RedStarLinq.Equals(gs.Sort(x => x).ToList(), shs.Sort(x => x).ToList()));
			for (int i = 0; i < 100; i++)
			{
				int n = random.Next(shs.Length);
				Assert.AreEqual(shs[shs.IndexOf(shs[n])], shs[n]);
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}
