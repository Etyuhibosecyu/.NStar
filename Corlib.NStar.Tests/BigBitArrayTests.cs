
namespace Corlib.NStar.Tests;

[TestClass]
public class BigBitArrayTests
{
	[TestMethod]
	public void ComplexTest()
	{
			byte[] bytes = new byte[150];
			random.NextBytes(bytes);
			BigBitArray bitArray = new(bytes, 2, 6);
			BigBitArray bitArray2 = new(bitArray, 2, 6);
			Assert.IsTrue(E.SequenceEqual(bitArray, bitArray2));
			int length = 20;
			int sourceIndex = 72;
			int destinationIndex = 123;
			bitArray.SetRange(destinationIndex, bitArray.GetRange(sourceIndex, length));
			Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(0, destinationIndex), E.Take(bitArray2, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(destinationIndex, length), E.Take(E.Skip(bitArray2, sourceIndex), length)));
			Assert.IsTrue(RedStarLinq.Equals(bitArray.GetRange(destinationIndex + length), E.Skip(bitArray2, destinationIndex + length)));
			for (int i = 0; i < 1000; i++)
			{
				random.NextBytes(bytes);
				length = random.Next(257);
				sourceIndex = random.Next(bytes.Length * 8 - length + 1);
				destinationIndex = random.Next(bytes.Length * 8 - length + 1);
				bitArray = new(bytes, 2, 6);
				bitArray2 = new(bitArray, 2, 6);
				bitArray.SetRange(destinationIndex, bitArray.GetRange(sourceIndex, length));
				Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(0, destinationIndex), E.Take(bitArray2, destinationIndex)));
				Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(destinationIndex, length), E.Take(E.Skip(bitArray2, sourceIndex), length)));
				Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(destinationIndex + length), E.Skip(bitArray2, destinationIndex + length)));
			}
	}

	[TestMethod]
	public void TestGetSmallRange()
	{
			byte[] bytes;
			BigBitArray bitArray;
			G.List<bool> bitArray2;
			int length;
			int sourceIndex;
			uint range;
			for (int i = 0; i < 1000; i++)
			{
				bytes = new byte[40];
				random.NextBytes(bytes);
				length = random.Next(33);
				sourceIndex = random.Next(bytes.Length * 8 - length);
				bitArray = new(bytes, 2, 6);
				bitArray2 = new(bitArray);
				range = bitArray.GetSmallRange(sourceIndex, length);
				Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitArray.GetRange(sourceIndex, length)));
				Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitArray2.GetRange(sourceIndex, length)));
				Assert.IsTrue(E.SequenceEqual(bitArray2.GetRange(sourceIndex, length), new BitList(new[] { range })[..length]));
			}
			length = 32;
			for (int i = 0; i < 100; i++)
			{
				bytes = new byte[40];
				random.NextBytes(bytes);
				sourceIndex = random.Next(bytes.Length * 8 - length);
				bitArray = new(bytes, 2, 6);
				bitArray2 = new(bitArray);
				range = bitArray.GetSmallRange(sourceIndex, length);
				Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitArray.GetRange(sourceIndex, length)));
				Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitArray2.GetRange(sourceIndex, length)));
				Assert.IsTrue(E.SequenceEqual(bitArray2.GetRange(sourceIndex, length), new BitList(new[] { range })[..length]));
			}
	}
}
