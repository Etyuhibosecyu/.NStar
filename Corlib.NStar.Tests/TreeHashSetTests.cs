
namespace Corlib.NStar.Tests;

[TestClass]
public class TreeHashSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		try
		{
			TreeHashSet<int> ths = new();
			G.HashSet<int> gs = new();
			for (int i = 0; i < 100; i++)
			{
				int n = random.Next(16);
				ths.Add(n);
				gs.Add(n);
			}
			Assert.IsTrue(ths.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(ths));
			for (int i = 0; i < 10; i++)
			{
				int n = random.Next(16);
				ths.RemoveValue(n);
				gs.Remove(n);
			}
			Assert.IsTrue(ths.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(ths));
			for (int i = 0; i < 10; i++)
			{
				int n = random.Next(16);
				ths.Add(n);
				gs.Add(n);
			}
			Assert.IsTrue(ths.SetEquals(gs));
			Assert.IsTrue(gs.SetEquals(ths));
			for (int i = 0; i < 100; i++)
			{
				int n = random.Next(ths.Length);
				Assert.AreEqual(ths.IndexOf(ths[n]), n);
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}
