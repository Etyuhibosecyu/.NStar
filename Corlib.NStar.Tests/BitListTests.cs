
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
				length = random.Next(96);
				sourceIndex = random.Next(bytes.Length * 8 - length);
				destinationIndex = random.Next(bytes.Length * 8 - length);
				bitList = new(bytes);
				bitList2 = new(bitList);
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

	[TestMethod]
	public void TestGetSmallRange()
	{
		try
		{
			byte[] bytes;
			BitList bitList;
			int length;
			int sourceIndex;
			uint range;
			for (int i = 0; i < 1000; i++)
			{
				bytes = new byte[40];
				random.NextBytes(bytes);
				length = random.Next(33);
				sourceIndex = random.Next(bytes.Length * 8 - length);
				bitList = new(bytes);
				range = bitList.GetSmallRange(sourceIndex, length);
				Assert.IsTrue(bitList[sourceIndex..(sourceIndex + length)].Equals(new BitList(new[] { range })[..length]));
			}
			length = 32;
			for (int i = 0; i < 100; i++)
			{
				bytes = new byte[40];
				random.NextBytes(bytes);
				sourceIndex = random.Next(bytes.Length * 8 - length);
				bitList = new(bytes);
				range = bitList.GetSmallRange(sourceIndex, length);
				Assert.IsTrue(bitList[sourceIndex..(sourceIndex + length)].Equals(new BitList(new[] { range })[..length]));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}
