namespace Corlib.NStar.Tests;

[TestClass]
public class CorlibNStarTest
{
	private static readonly Random random = new(1234567890);

	[TestMethod]
	public void ClearTest()
	{
		BigBitList bitList = new(OptimizedLinq.Fill(x => (byte)random.Next(), 2500000));
		BigBitList bitList2 = new(bitList);
		int x = random.Next(20000000), y = random.Next(20000000);
		(int index, int endIndex) = (x < y) ? (x, y) : (y, x);
		int count = endIndex - index;
		bitList.Clear(index, count);
		Assert.IsTrue(Enumerable.SequenceEqual(bitList.GetRange(0, index).ToUIntBigList(), bitList2.GetRange(0, index).ToUIntBigList()));
		Assert.IsTrue(Enumerable.SequenceEqual(bitList.GetRange(index, count).ToUIntBigList(), new BigBitList(count, false).ToUIntBigList()));
		Assert.IsTrue(Enumerable.SequenceEqual(bitList.GetRange(endIndex, 20000000 - endIndex).ToUIntBigList(), bitList2.GetRange(endIndex, 20000000 - endIndex).ToUIntBigList()));
	}
}