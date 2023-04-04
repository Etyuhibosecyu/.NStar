
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
			Assert.IsTrue(E.SequenceEqual(bitList[..destinationIndex], E.Take(bitList2, destinationIndex)));
			Assert.IsTrue(bitList[destinationIndex..(destinationIndex + length)].Equals(bitList2[sourceIndex..(sourceIndex + length)]));
			Assert.IsTrue(E.SequenceEqual(bitList[destinationIndex..(destinationIndex + length)], E.Take(E.Skip(bitList2, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(bitList[(destinationIndex - 1)..(destinationIndex + length)].Equals(bitList2[(sourceIndex - 1)..(sourceIndex + length)]), E.SequenceEqual(bitList[(destinationIndex - 1)..(destinationIndex + length)], E.Take(E.Skip(bitList2, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length * 8 - 1 && destinationIndex + length < bytes.Length * 8 - 1)
				Assert.AreEqual(bitList[destinationIndex..(destinationIndex + length + 1)].Equals(bitList2[sourceIndex..(sourceIndex + length + 1)]), E.SequenceEqual(bitList[destinationIndex..(destinationIndex + length + 1)], E.Take(E.Skip(bitList2, sourceIndex), length + 1)));
			Assert.IsTrue(bitList[(destinationIndex + length)..].Equals(bitList2[(destinationIndex + length)..]));
			Assert.IsTrue(E.SequenceEqual(bitList[(destinationIndex + length)..], E.Skip(bitList2, destinationIndex + length)));
			for (int i = 0; i < 1000; i++)
			{
				random.NextBytes(bytes);
				length = random.Next(97);
				sourceIndex = random.Next(bytes.Length * 8 - length + 1);
				destinationIndex = random.Next(bytes.Length * 8 - length + 1);
				bitList = new(bytes);
				bitList2 = new(bitList);
				bitList.SetRange(destinationIndex, bitList.GetRange(sourceIndex, length));
				Assert.IsTrue(bitList[..destinationIndex].Equals(bitList2[..destinationIndex]));
				Assert.IsTrue(E.SequenceEqual(bitList[..destinationIndex], E.Take(bitList2, destinationIndex)));
				Assert.IsTrue(bitList[destinationIndex..(destinationIndex + length)].Equals(bitList2[sourceIndex..(sourceIndex + length)]));
				Assert.IsTrue(E.SequenceEqual(bitList[destinationIndex..(destinationIndex + length)], E.Take(E.Skip(bitList2, sourceIndex), length)));
				if (sourceIndex >= 1 && destinationIndex >= 1)
					Assert.AreEqual(bitList[(destinationIndex - 1)..(destinationIndex + length)].Equals(bitList2[(sourceIndex - 1)..(sourceIndex + length)]), E.SequenceEqual(bitList[(destinationIndex - 1)..(destinationIndex + length)], E.Take(E.Skip(bitList2, sourceIndex - 1), length + 1)));
				if (sourceIndex + length < bytes.Length * 8 - 1 && destinationIndex + length < bytes.Length * 8 - 1)
					Assert.AreEqual(bitList[destinationIndex..(destinationIndex + length + 1)].Equals(bitList2[sourceIndex..(sourceIndex + length + 1)]), E.SequenceEqual(bitList[destinationIndex..(destinationIndex + length + 1)], E.Take(E.Skip(bitList2, sourceIndex), length + 1)));
				Assert.IsTrue(bitList[(destinationIndex + length)..].Equals(bitList2[(destinationIndex + length)..]));
				Assert.IsTrue(E.SequenceEqual(bitList[(destinationIndex + length)..], E.Skip(bitList2, destinationIndex + length)));
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
			G.List<bool> bitList2;
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
				bitList2 = new(bitList);
				range = bitList.GetSmallRange(sourceIndex, length);
				Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitList[sourceIndex..(sourceIndex + length)]));
				Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitList2.GetRange(sourceIndex, length)));
				Assert.IsTrue(E.SequenceEqual(bitList2.GetRange(sourceIndex, length), new BitList(new[] { range })[..length]));
			}
			length = 32;
			for (int i = 0; i < 100; i++)
			{
				bytes = new byte[40];
				random.NextBytes(bytes);
				sourceIndex = random.Next(bytes.Length * 8 - length);
				bitList = new(bytes);
				bitList2 = new(bitList);
				range = bitList.GetSmallRange(sourceIndex, length);
				Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitList[sourceIndex..(sourceIndex + length)]));
				Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitList2.GetRange(sourceIndex, length)));
				Assert.IsTrue(E.SequenceEqual(bitList2.GetRange(sourceIndex, length), new BitList(new[] { range })[..length]));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSetAll()
	{
		try
		{
			byte[] bytes = new byte[40];
			BitList bitList;
			G.List<bool> boolList;
			int count;
			int index;
			bool value;
			int endIndex;
			for (int i = 0; i < 1000; i++)
			{
				random.NextBytes(bytes);
				count = random.Next(97);
				index = random.Next(bytes.Length * 8 - count + 1);
				value = random.Next(2) == 1;
				bitList = new(bytes);
				boolList = new(bitList);
				bitList.SetAll(value, index, count);
				endIndex = index + count;
				for (int j = index; j < endIndex; j++)
					boolList[j] = value;
				Assert.IsTrue(bitList.Equals(boolList));
				Assert.IsTrue(E.SequenceEqual(boolList, bitList));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}
