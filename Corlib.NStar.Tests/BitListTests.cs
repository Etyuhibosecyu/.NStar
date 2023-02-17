
namespace Corlib.NStar.Tests;

[TestClass]
public class BitListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		try
		{
			byte[] bytes = new byte[40];
			random.NextBytes(bytes);
			BitList bitList = new(bytes);
			BitList bitList2 = new(bitList);
			int length = 10;
			int sourceIndex = 36;
			int destinationIndex = 61;
			bitList.SetRange(destinationIndex, bitList.GetRange(sourceIndex, length));
			Assert.IsTrue(bitList[..destinationIndex].Equals(bitList2[..destinationIndex]));
			Assert.IsTrue(bitList[destinationIndex..(destinationIndex + length)].Equals(bitList2[sourceIndex..(sourceIndex + length)]));
			Assert.IsTrue(bitList[(destinationIndex + length)..].Equals(bitList2[(destinationIndex + length)..]));
			for (int i = 0; i < 1000; i++)
			{
				bytes = new byte[40];
				random.NextBytes(bytes);
				bitList = new(bytes);
				bitList2 = new(bitList);
				length = random.Next(96);
				sourceIndex = random.Next(bitList.Length - length);
				destinationIndex = random.Next(bitList.Length - length);
				bitList.SetRange(destinationIndex, bitList.GetRange(sourceIndex, length));
				Assert.IsTrue(bitList[..destinationIndex].Equals(bitList2[..destinationIndex]));
				Assert.IsTrue(bitList[destinationIndex..(destinationIndex + length)].Equals(bitList2[sourceIndex..(sourceIndex + length)]));
				Assert.IsTrue(bitList[(destinationIndex + length)..].Equals(bitList2[(destinationIndex + length)..]));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}
