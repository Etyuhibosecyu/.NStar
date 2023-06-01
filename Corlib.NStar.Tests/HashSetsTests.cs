
namespace Corlib.NStar.Tests;

[TestClass]
public class FastDelHashSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		FastDelHashSet<int> fhs = new();
		G.HashSet<int> gs = new();
		for (int i = 0; i < 100; i++)
		{
			int n = random.Next(16);
			fhs.Add(n);
			gs.Add(n);
		}
		Assert.IsTrue(fhs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(fhs));
		for (int i = 0; i < 10; i++)
		{
			int n = random.Next(16);
			fhs.RemoveValue(n);
			gs.Remove(n);
		}
		Assert.IsTrue(fhs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(fhs));
		for (int i = 0; i < 10; i++)
		{
			int n = random.Next(16);
			fhs.Add(n);
			gs.Add(n);
		}
		Assert.IsTrue(fhs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(fhs));
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
}

[TestClass]
public class ListHashSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		ListHashSet<int> shs = new();
		G.HashSet<int> gs = new();
		for (int i = 0; i < 100; i++)
		{
			int n = random.Next(16);
			shs.Add(n);
			gs.Add(n);
		}
		Assert.IsTrue(shs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(shs));
		for (int i = 0; i < 10; i++)
		{
			int n = random.Next(16);
			shs.RemoveValue(n);
			gs.Remove(n);
		}
		Assert.IsTrue(shs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(shs));
		for (int i = 0; i < 10; i++)
		{
			int n = random.Next(16);
			shs.Add(n);
			gs.Add(n);
		}
		Assert.IsTrue(shs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(shs));
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
		Assert.IsTrue(shs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(shs));
		for (int i = 0; i < 100; i++)
		{
			int n;
			do
			{
				n = random.Next(shs.Length);
			} while (shs[n] == 0);
			Assert.AreEqual(shs.IndexOf(shs[n]), n);
		}
	}
}

[TestClass]
public class ParallelHashSetTests
{
	[TestMethod]
	public void ComplexTest()
	{
		ParallelHashSet<int> phs = new();
		G.HashSet<int> gs = new();
		for (int i = 0; i < 100; i++)
		{
			int n = random.Next(16);
			phs.Add(n);
			gs.Add(n);
		}
		Assert.IsTrue(phs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(phs));
		for (int i = 0; i < 10; i++)
		{
			int n = random.Next(16);
			phs.RemoveValue(n);
			gs.Remove(n);
		}
		Assert.IsTrue(phs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(phs));
		for (int i = 0; i < 10; i++)
		{
			int n = random.Next(16);
			phs.Add(n);
			gs.Add(n);
		}
		Assert.IsTrue(phs.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(phs));
		for (int i = 0; i < 100; i++)
		{
			int n;
			do
			{
				n = random.Next(phs.Length);
			} while (phs[n] == 0);
			Assert.AreEqual(phs.IndexOf(phs[n]), n);
		}
	}
}

[TestClass]
public class TreeHashSetTests
{
	[TestMethod]
	public void ComplexTest()
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
		G.List<int> list = new(ths);
		for (int i = 0; i < 10; i++)
		{
			int n = random.Next(ths.Length);
			int n2 = random.Next(16);
			if (CreateVar(ths.IndexOf(n2), out int foundIndex) == n || foundIndex == -1)
			{
				ths[n] = n2;
				list[n] = n2;
			}
		}
		gs = new(list);
		Assert.IsTrue(ths.SetEquals(gs));
		Assert.IsTrue(gs.SetEquals(ths));
		for (int i = 0; i < 100; i++)
		{
			int n = random.Next(ths.Length);
			Assert.AreEqual(ths.IndexOf(ths[n]), n);
		}
	}
}
