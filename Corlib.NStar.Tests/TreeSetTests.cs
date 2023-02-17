
namespace Corlib.NStar.Tests;

[TestClass]
public class TreeSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		try
		{
			TreeSet<int> ts = new();
			G.SortedSet<int> gs = new();
			for (int i = 0; i < 100; i++)
			{
				int n = random.Next(16);
				ts.Add(n);
				gs.Add(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(ts, gs));
			for (int i = 0; i < 10; i++)
			{
				int n = random.Next(16);
				ts.RemoveValue(n);
				gs.Remove(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(ts, gs));
			for (int i = 0; i < 10; i++)
			{
				int n = random.Next(16);
				ts.Add(n);
				gs.Add(n);
			}
			Assert.IsTrue(RedStarLinq.Equals(ts, gs));
			for (int i = 0; i < 100; i++)
			{
				int n = random.Next(ts.Length);
				Assert.AreEqual(ts.IndexOf(ts[n]), n);
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}
