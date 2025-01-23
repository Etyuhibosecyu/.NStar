﻿
namespace Corlib.NStar.Tests;

[TestClass]
public class ExtentsTests
{
	[TestMethod]
	public void TestNSort()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var a = RedStarLinq.FillArray(random.Next(1000), _ => random.Next());
			var b = a.ToArray(x => (uint)x);
			b.NSort();
			CheckSort(b);
			b = a.ToArray(x => (uint)x & 0xff00ff);
			b.NSort();
			CheckSort(b);
			b = a.ToArray(x => (uint)x & 0xff00ff00);
			b.NSort();
			CheckSort(b);
			b = a.ToArray(x => (uint)x & 0xff0000ff);
			b.NSort();
			CheckSort(b);
			b = a.ToArray(x => (uint)x & 0xffff00);
			b.NSort();
			CheckSort(b);
			b = a.ToArray(x => (uint)x);
			b.NSort(x => x);
			CheckSort(b.ToArray(x => x));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => uint.MaxValue - x);
			CheckSort(b.ToArray(x => uint.MaxValue - x));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => x & 0xff00ff);
			CheckSort(b.ToArray(x => x & 0xff00ff));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => uint.MaxValue - x & 0xff00ff);
			CheckSort(b.ToArray(x => uint.MaxValue - x & 0xff00ff));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => x & 0xff00ff00);
			CheckSort(b.ToArray(x => x & 0xff00ff00));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => uint.MaxValue - x & 0xff00ff00);
			CheckSort(b.ToArray(x => uint.MaxValue - x & 0xff00ff00));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => x & 0xff0000ff);
			CheckSort(b.ToArray(x => x & 0xff0000ff));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => uint.MaxValue - x & 0xff0000ff);
			CheckSort(b.ToArray(x => uint.MaxValue - x & 0xff0000ff));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => x & 0xffff00);
			CheckSort(b.ToArray(x => x & 0xffff00));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => uint.MaxValue - x & 0xffff00);
			CheckSort(b.ToArray(x => uint.MaxValue - x & 0xffff00));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => (ushort)x);
			CheckSort(b.ToArray(x => (ushort)x));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => (ushort)(ushort.MaxValue - (ushort)x));
			CheckSort(b.ToArray(x => (ushort)(ushort.MaxValue - (ushort)x)));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => (byte)x);
			CheckSort(b.ToArray(x => (byte)x));
			b = a.ToArray(x => (uint)x);
			b.NSort(x => (byte)(byte.MaxValue - (byte)x));
			CheckSort(b.ToArray(x => (byte)(byte.MaxValue - (byte)x)));
			var b2 = a.ToArray(x => (ushort)x);
			b2.NSort();
			CheckSort(b2);
			b2 = a.ToArray(x => (ushort)x);
			b2.NSort(x => uint.MaxValue - x);
			CheckSort(b2.ToArray(x => uint.MaxValue - x));
			b2 = a.ToArray(x => (ushort)x);
			b2.NSort(x => x);
			CheckSort(b2.ToArray(x => x));
			b2 = a.ToArray(x => (ushort)x);
			b2.NSort(x => (ushort)(ushort.MaxValue - x));
			CheckSort(b2.ToArray(x => (ushort)(ushort.MaxValue - x)));
			b2 = a.ToArray(x => (ushort)x);
			b2.NSort(x => (byte)x);
			CheckSort(b2.ToArray(x => (byte)x));
			b2 = a.ToArray(x => (ushort)x);
			b2.NSort(x => (byte)(byte.MaxValue - (byte)x));
			CheckSort(b2.ToArray(x => (byte)(byte.MaxValue - (byte)x)));
			var b3 = a.ToArray(x => (byte)x);
			b3.NSort();
			CheckSort(b3);
			b3 = a.ToArray(x => (byte)x);
			b3.NSort(x => (uint)x);
			CheckSort(b3.ToArray(x => (uint)x));
			b3 = a.ToArray(x => (byte)x);
			b3.NSort(x => uint.MaxValue - x);
			CheckSort(b3.ToArray(x => uint.MaxValue - x));
			b3 = a.ToArray(x => (byte)x);
			b3.NSort(x => (ushort)x);
			CheckSort(b3.ToArray(x => (ushort)x));
			b3 = a.ToArray(x => (byte)x);
			b3.NSort(x => (ushort)(ushort.MaxValue - x));
			CheckSort(b3.ToArray(x => (ushort)(ushort.MaxValue - x)));
			b3 = a.ToArray(x => (byte)x);
			b3.NSort(x => x);
			CheckSort(b3.ToArray(x => x));
			b3 = a.ToArray(x => (byte)x);
			b3.NSort(x => (byte)(byte.MaxValue - x));
			CheckSort(b3.ToArray(x => (byte)(byte.MaxValue - x)));
			var c = a.ToList(x => (uint)x);
			c.NSort();
			CheckSort(c);
			c = a.ToList(x => (uint)x);
			c.NSort(x => x);
			CheckSort(c.Convert(x => x));
			c = a.ToList(x => (uint)x);
			c.NSort(x => uint.MaxValue - x);
			CheckSort(c.Convert(x => uint.MaxValue - x));
			c = a.ToList(x => (uint)x);
			c.NSort(x => (ushort)x);
			CheckSort(c.Convert(x => (ushort)x));
			c = a.ToList(x => (uint)x);
			c.NSort(x => (ushort)(ushort.MaxValue - (ushort)x));
			CheckSort(c.Convert(x => (ushort)(ushort.MaxValue - (ushort)x)));
			c = a.ToList(x => (uint)x);
			c.NSort(x => (byte)x);
			CheckSort(c.Convert(x => (byte)x));
			c = a.ToList(x => (uint)x);
			c.NSort(x => (byte)(byte.MaxValue - (byte)x));
			CheckSort(c.Convert(x => (byte)(byte.MaxValue - (byte)x)));
			var c2 = a.ToList(x => (ushort)x);
			c2.NSort();
			CheckSort(c2);
			c2 = a.ToList(x => (ushort)x);
			c2.NSort(x => uint.MaxValue - x);
			CheckSort(c2.Convert(x => uint.MaxValue - x));
			c2 = a.ToList(x => (ushort)x);
			c2.NSort(x => x);
			CheckSort(c2.Convert(x => x));
			c2 = a.ToList(x => (ushort)x);
			c2.NSort(x => (ushort)(ushort.MaxValue - x));
			CheckSort(c2.Convert(x => (ushort)(ushort.MaxValue - x)));
			c2 = a.ToList(x => (ushort)x);
			c2.NSort(x => (byte)x);
			CheckSort(c2.Convert(x => (byte)x));
			c2 = a.ToList(x => (ushort)x);
			c2.NSort(x => (byte)(byte.MaxValue - (byte)x));
			CheckSort(c2.Convert(x => (byte)(byte.MaxValue - (byte)x)));
			var c3 = a.ToList(x => (byte)x);
			c3.NSort();
			CheckSort(c3);
			c3 = a.ToList(x => (byte)x);
			c3.NSort(x => x);
			CheckSort(c3.Convert(x => (uint)x));
			c3 = a.ToList(x => (byte)x);
			c3.NSort(x => uint.MaxValue - x);
			CheckSort(c3.Convert(x => uint.MaxValue - x));
			c3 = a.ToList(x => (byte)x);
			c3.NSort(x => x);
			CheckSort(c3.Convert(x => (ushort)x));
			c3 = a.ToList(x => (byte)x);
			c3.NSort(x => (ushort)(ushort.MaxValue - x));
			CheckSort(c3.Convert(x => (ushort)(ushort.MaxValue - x)));
			c3 = a.ToList(x => (byte)x);
			c3.NSort(x => x);
			CheckSort(c3.Convert(x => x));
			c3 = a.ToList(x => (byte)x);
			c3.NSort(x => (byte)(byte.MaxValue - x));
			CheckSort(c3.Convert(x => (byte)(byte.MaxValue - x)));
			var d = a.ToNList(x => (uint)x);
			d.Sort();
			CheckSort(d);
			d = a.ToNList(x => (uint)x);
			d.Sort(x => x);
			CheckSort(d.ToNList(x => x));
			d = a.ToNList(x => (uint)x);
			d.Sort(x => uint.MaxValue - x);
			CheckSort(d.ToNList(x => uint.MaxValue - x));
			d = a.ToNList(x => (uint)x);
			d.Sort(x => (ushort)x);
			CheckSort(d.ToNList(x => (ushort)x));
			d = a.ToNList(x => (uint)x);
			d.Sort(x => (ushort)(ushort.MaxValue - (ushort)x));
			CheckSort(d.ToNList(x => (ushort)(ushort.MaxValue - (ushort)x)));
			d = a.ToNList(x => (uint)x);
			d.Sort(x => (byte)x);
			CheckSort(d.ToNList(x => (byte)x));
			d = a.ToNList(x => (uint)x);
			d.Sort(x => (byte)(byte.MaxValue - (byte)x));
			CheckSort(d.ToNList(x => (byte)(byte.MaxValue - (byte)x)));
			var d2 = a.ToNList(x => (ushort)x);
			d2.Sort();
			CheckSort(d2);
			d2 = a.ToNList(x => (ushort)x);
			d2.Sort(x => uint.MaxValue - x);
			CheckSort(d2.ToNList(x => uint.MaxValue - x));
			d2 = a.ToNList(x => (ushort)x);
			d2.Sort(x => x);
			CheckSort(d2.ToNList(x => x));
			d2 = a.ToNList(x => (ushort)x);
			d2.Sort(x => (ushort)(ushort.MaxValue - x));
			CheckSort(d2.ToNList(x => (ushort)(ushort.MaxValue - x)));
			d2 = a.ToNList(x => (ushort)x);
			d2.Sort(x => (byte)x);
			CheckSort(d2.ToNList(x => (byte)x));
			d2 = a.ToNList(x => (ushort)x);
			d2.Sort(x => (byte)(byte.MaxValue - (byte)x));
			CheckSort(d2.ToNList(x => (byte)(byte.MaxValue - (byte)x)));
			var d3 = a.ToNList(x => (byte)x);
			d3.Sort();
			CheckSort(d3);
			d3 = a.ToNList(x => (byte)x);
			d3.Sort(x => x);
			CheckSort(d3.ToNList(x => (uint)x));
			d3 = a.ToNList(x => (byte)x);
			d3.Sort(x => uint.MaxValue - x);
			CheckSort(d3.ToNList(x => uint.MaxValue - x));
			d3 = a.ToNList(x => (byte)x);
			d3.Sort(x => x);
			CheckSort(d3.ToNList(x => (ushort)x));
			d3 = a.ToNList(x => (byte)x);
			d3.Sort(x => (ushort)(ushort.MaxValue - x));
			CheckSort(d3.ToNList(x => (ushort)(ushort.MaxValue - x)));
			d3 = a.ToNList(x => (byte)x);
			d3.Sort(x => x);
			CheckSort(d3.ToNList(x => x));
			d3 = a.ToNList(x => (byte)x);
			d3.Sort(x => (byte)(byte.MaxValue - x));
			CheckSort(d3.ToNList(x => (byte)(byte.MaxValue - x)));
		}
		static void CheckSort<T>(G.IReadOnlyList<T> list) where T : IComparable<T>
		{
			for (var i = 1; i < list.Count; i++)
				Assert.IsTrue(list[i].CompareTo(list[i - 1]) >= 0);
		}
	}
}
