using NStar.ExtraReplacing;
using System.Numerics;

namespace NStar.ExtraLibs.Tests;

[TestClass]
public class BufferTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var arr = RedStarLinq.FillArray(100, _ => random.Next(16));
		var toInsert = Array.Empty<int>();
		Buffer<int> buf = new(16, arr);
		G.List<int> gl = [.. arr[^16..]];
		var collectionActions = new[] { () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(6), _ => random.Next(16));
			buf.AddRange(toInsert);
			gl.AddRange(toInsert);
			gl.RemoveRange(0, Max(gl.Count - 16, 0));
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			var n = random.Next(buf.Length);
			toInsert = RedStarLinq.FillArray(random.Next(6), _ => random.Next(16));
			buf.Insert(n, toInsert);
			gl.InsertRange(n, toInsert);
			gl.RemoveRange(0, Max(gl.Count - 16, 0));
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			var length = Min(random.Next(9), buf.Length);
			if (buf.Length < length)
				return;
			var start = random.Next(buf.Length - length + 1);
			buf.Remove(start, length);
			gl.RemoveRange(start, length);
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		} };
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			if (random.Next(2) == 0)
			{
				var index = random.Next(buf.Length);
				buf.Insert(index, n);
				gl.Insert(index, n);
			}
			else
			{
				buf.Add(n);
				gl.Add(n);
			}
			if (gl.Count > 16)
				gl.RemoveAt(0);
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			if (buf.Length == 0) return;
			if (random.Next(2) == 0)
			{
				var n = random.Next(buf.Length);
				buf.RemoveAt(n);
				gl.RemoveAt(n);
			}
			else
			{
				var n = random.Next(16);
				buf.RemoveValue(n);
				gl.Remove(n);
			}
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			collectionActions.Random(random)();
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			if (buf.Length == 0)
				return;
			var index = random.Next(buf.Length);
			var n = random.Next(16);
			buf[index] = n;
			gl[index] = n;
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			if (buf.Length == 0) return;
			var n = random.Next(buf.Length);
			Assert.AreEqual(buf[buf.IndexOf(buf[n])], buf[n]);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		arr = RedStarLinq.FillArray(14, _ => random.Next(16));
		var n = 1;
		toInsert = RedStarLinq.FillArray(5, _ => random.Next(16));
		buf = new(16, arr);
		gl = [.. arr];
		buf.Insert(n, toInsert);
		gl.InsertRange(n, toInsert);
		gl.RemoveRange(0, Max(gl.Count - 16, 0));
		Assert.IsTrue(buf.Equals(gl));
		Assert.IsTrue(E.SequenceEqual(gl, buf));
	}
}

[TestClass]
public class ListTests
{
	[TestMethod]
	public void TestAddRange()
	{
		var a = list.ToList().AddRange(defaultCollection);
		var b = new G.List<string>(list);
		b.AddRange(defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(RedStarLinq.ToList(defaultCollection));
		b = [.. list, .. defaultCollection];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(RedStarLinq.ToArray(defaultCollection));
		b = [.. list, .. defaultCollection];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(E.ToList(defaultCollection));
		b = [.. list, .. defaultCollection];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(defaultCollection.ToList().Copy().Insert(0, "XXX").GetSlice(1));
		b = [.. list, .. defaultCollection.ToList().Copy().Insert(0, "XXX").GetSlice(1)];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(defaultCollection.Prepend("XXX"));
		b = [.. list, .. defaultCollection.Prepend("XXX")];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(enumerable);
		b = [.. list, .. enumerable];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(enumerable2);
		b = [.. list, .. enumerable2];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(defaultCollection.AsSpan(2, 3));
		b = [.. list, .. defaultCollection.Skip(2).Take(3)];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).AddRange(defaultCollection);
		b = [.. list, .. defaultCollection];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).AddRange(RedStarLinq.ToList(defaultCollection));
		b = [.. list, .. defaultCollection];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).AddRange(RedStarLinq.ToArray(defaultCollection));
		b = [.. list, .. defaultCollection];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).AddRange(E.ToList(defaultCollection));
		b = [.. list, .. defaultCollection];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).AddRange(defaultCollection.ToList().Copy().Insert(0, "XXX").GetSlice(1));
		b = [.. list, .. defaultCollection.ToList().Copy().Insert(0, "XXX").GetSlice(1)];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).AddRange(defaultCollection.Prepend("XXX"));
		b = [.. list, .. defaultCollection.Prepend("XXX")];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).AddRange(enumerable);
		b = [.. list, .. enumerable];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).AddRange(enumerable2);
		b = [.. list, .. enumerable2];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).AddRange(defaultCollection.AsSpan(2, 3));
		b = [.. list, .. defaultCollection.Skip(2).Take(3)];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestInsert()
	{
		var a = list.ToList().Insert(3, defaultString);
		var b = new G.List<string>(list);
		b.Insert(3, defaultString);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(list.Length, defaultString);
		b = [.. list];
		b.Insert(list.Length, defaultString);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(4, defaultCollection);
		b = [.. list];
		b.InsertRange(4, defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(4, RedStarLinq.ToList(defaultCollection));
		b = [.. list];
		b.InsertRange(4, defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(4, RedStarLinq.ToArray(defaultCollection));
		b = [.. list];
		b.InsertRange(4, defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(4, E.ToList(defaultCollection));
		b = [.. list];
		b.InsertRange(4, defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(4, defaultCollection.ToList().Copy().Insert(0, "XXX").GetSlice(1));
		b = [.. list];
		b.InsertRange(4, defaultCollection.ToList().Copy().Insert(0, "XXX").GetSlice(1));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(4, defaultCollection.Prepend("XXX"));
		b = [.. list];
		b.InsertRange(4, defaultCollection.Prepend("XXX"));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(4, enumerable);
		b = [.. list];
		b.InsertRange(4, enumerable);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(4, enumerable2);
		b = [.. list];
		b.InsertRange(4, enumerable2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(2, defaultCollection.AsSpan(2, 3));
		b = [.. list];
		b.InsertRange(2, defaultCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).Insert(list.Length, defaultString);
		b = [.. list];
		b.Insert(list.Length, defaultString);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).Insert(4, defaultCollection);
		b = [.. list];
		b.InsertRange(4, defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).Insert(4, RedStarLinq.ToList(defaultCollection));
		b = [.. list];
		b.InsertRange(4, defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).Insert(4, RedStarLinq.ToArray(defaultCollection));
		b = [.. list];
		b.InsertRange(4, defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).Insert(4, E.ToList(defaultCollection));
		b = [.. list];
		b.InsertRange(4, defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).Insert(4, defaultCollection.ToList().Copy().Insert(0, "XXX").GetSlice(1));
		b = [.. list];
		b.InsertRange(4, defaultCollection.ToList().Copy().Insert(0, "XXX").GetSlice(1));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).Insert(4, defaultCollection.Prepend("XXX"));
		b = [.. list];
		b.InsertRange(4, defaultCollection.Prepend("XXX"));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).Insert(4, enumerable);
		b = [.. list];
		b.InsertRange(4, enumerable);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).Insert(4, enumerable2);
		b = [.. list];
		b.InsertRange(4, enumerable2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).Insert(2, defaultCollection.AsSpan(2, 3));
		b = [.. list];
		b.InsertRange(2, defaultCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a = list.ToList().Insert(1000, defaultString));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => list.ToList().Insert(-1, defaultCollection));
		Assert.ThrowsExactly<ArgumentNullException>(() => list.ToList().Insert(5, (List<string>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => list.ToList().Insert(5, (G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestReplace2()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(33, 127);
			string s = new(arr);
			String a = s;
			var oldItem = (char)random.Next(33, 127);
			var newItem = (char)random.Next(33, 127);
			var b = a.Replace(oldItem, newItem);
			var c = s.Replace(oldItem, newItem);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(33, 127);
			string s = new(arr);
			String a = s;
			var oldCollection = a.GetRange(random.Next(991), random.Next(1, 10)).ToString();
			var newArray = new char[random.Next(10)];
			for (var j = 0; j < newArray.Length; j++)
				newArray[j] = (char)random.Next(33, 127);
			string newCollection = new(newArray);
			var b = a.Replace(oldCollection, newArray);
			var c = s.Replace(oldCollection, newCollection);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace(null!, null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace("925", null!));
		var backConverter = new Chain(48, 10).ToArray(x => ((char)(x - 48 + 'A'), (char)x));
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(48, 58);
			string s = new(arr);
			String a = s;
			var dic = new Chain(48, 10).ToList().Shuffle().Take(random.Next(2, 11)).ToDictionary(x => (char)x, x => (char)random.Next(48, 58));
			var b = a.Replace(dic);
			string c = new(s);
			var replace = dic.ToArray(x => (x.Key, (char)(x.Value - 48 + 'A')));
			for (var j = 0; j < replace.Length; j++)
				c = c.Replace(replace[j].Key, replace[j].Item2);
			for (var j = 0; j < backConverter.Length; j++)
				c = c.Replace(backConverter[j].Item1, backConverter[j].Item2);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, char>)null!));
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(48, 58);
			string s = new(arr);
			String a = s;
			var dic = new Chain(48, 10).ToList().Shuffle().Take(random.Next(2, 11)).ToDictionary(x => (char)x, x => (G.IEnumerable<char>)new Chain(random.Next(0, 5)).ToString(y => (char)random.Next(48, 58)));
			var b = a.Replace(dic);
			string c = new(s);
			var replace = dic.ToArray(x => (x.Key, x.Value.ToString(y => (char)(y - 48 + 'A'))));
			for (var j = 0; j < replace.Length; j++)
				c = c.Replace("" + replace[j].Key, replace[j].Item2);
			for (var j = 0; j < backConverter.Length; j++)
				c = c.Replace(backConverter[j].Item1, backConverter[j].Item2);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, G.IEnumerable<char>>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, String>)null!));
		ProcessPairs("3692581470", "3692581470", []);
		ProcessPairs("3692581470", "3696925814070", new() { { ('6', '9'), "6969" }, { ('1', '4'), "140" } });
		ProcessPairs("3692581470", "3122007370", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("", "", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("6", "6", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("3232323232!", "256256256256256!", new() { { ('3', '2'), "256" }, { ('2', '3'), "128" }, { ('3', '1'), "888" } });
		ProcessPairs("3232323232!", "256256256256256!", new() { { ('2', '3'), "128" }, { ('3', '2'), "256" }, { ('3', '1'), "888" } });
		ProcessPairs("77777", "777777777", new() { { ('7', '7'), "7777" }, { ('8', '8'), "8888" } });
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char), G.IEnumerable<char>>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char), String>)null!));
		ProcessTriples("3692581470", "3692581470", []);
		ProcessTriples("3692581470", "36969581400", new() { { ('6', '9', '2'), "6969" }, { ('1', '4', '7'), "140" } });
		ProcessTriples("3692581470", "391114641470", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("", "", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("6", "6", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("256256256256256!", "40964096409640964096!", new() { { ('2', '5', '6'), "4096" }, { ('5', '6', '2'), "2048" }, { ('6', '2', '5'), "888" } });
		ProcessTriples("3232323232!", "2048409620482!", new() { { ('2', '3', '2'), "4096" }, { ('3', '2', '3'), "2048" }, { ('3', '1', '2'), "888" } });
		ProcessTriples("77777777", "777777777777", new() { { ('7', '7', '7'), "77777" }, { ('8', '8', '8'), "88888" } });
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char, char), G.IEnumerable<char>>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char, char), String>)null!));
		static void ProcessPairs(string s, string c, Dictionary<(char, char), G.IEnumerable<char>> dic)
		{
			String a = s;
			var b = a.Replace(dic);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		static void ProcessTriples(string s, string c, Dictionary<(char, char, char), G.IEnumerable<char>> dic)
		{
			String a = s;
			var b = a.Replace(dic);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestReplaceInPlace()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(33, 127);
			string s = new(arr);
			String a = s;
			var oldItem = (char)random.Next(33, 127);
			var newItem = (char)random.Next(33, 127);
			var b = a.ReplaceInPlace(oldItem, newItem);
			var c = s.Replace(oldItem, newItem);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(33, 127);
			string s = new(arr);
			String a = s;
			var oldCollection = a.GetRange(random.Next(991), random.Next(1, 10)).ToString();
			var newArray = new char[random.Next(10)];
			for (var j = 0; j < newArray.Length; j++)
				newArray[j] = (char)random.Next(33, 127);
			string newCollection = new(newArray);
			var b = a.ReplaceInPlace(oldCollection, newArray);
			var c = s.Replace(oldCollection, newCollection);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace(null!, null!));
		var backConverter = new Chain(48, 10).ToArray(x => ((char)(x - 48 + 'A'), (char)x));
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(48, 58);
			string s = new(arr);
			String a = s;
			var dic = new Chain(48, 10).ToList().Shuffle().Take(random.Next(2, 11)).ToDictionary(x => (char)x, x => (char)random.Next(48, 58));
			var b = a.ReplaceInPlace(dic);
			string c = new(s);
			var replace = dic.ToArray(x => (x.Key, (char)(x.Value - 48 + 'A')));
			for (var j = 0; j < replace.Length; j++)
				c = c.Replace(replace[j].Key, replace[j].Item2);
			for (var j = 0; j < backConverter.Length; j++)
				c = c.Replace(backConverter[j].Item1, backConverter[j].Item2);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, char>)null!));
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(48, 58);
			string s = new(arr);
			String a = s;
			var dic = new Chain(48, 10).ToList().Shuffle().Take(random.Next(2, 11)).ToDictionary(x => (char)x, x => (G.IEnumerable<char>)new Chain(random.Next(0, 5)).ToString(y => (char)random.Next(48, 58)));
			var b = a.ReplaceInPlace(dic);
			string c = new(s);
			var replace = dic.ToArray(x => (x.Key, x.Value.ToString(y => (char)(y - 48 + 'A'))));
			for (var j = 0; j < replace.Length; j++)
				c = c.Replace("" + replace[j].Key, replace[j].Item2);
			for (var j = 0; j < backConverter.Length; j++)
				c = c.Replace(backConverter[j].Item1, backConverter[j].Item2);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, G.IEnumerable<char>>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, String>)null!));
		ProcessPairs("3692581470", "3692581470", []);
		ProcessPairs("3692581470", "3696925814070", new() { { ('6', '9'), "6969" }, { ('1', '4'), "140" } });
		ProcessPairs("3692581470", "3122007370", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("", "", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("6", "6", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("3232323232!", "256256256256256!", new() { { ('3', '2'), "256" }, { ('2', '3'), "128" }, { ('3', '1'), "888" } });
		ProcessPairs("3232323232!", "256256256256256!", new() { { ('2', '3'), "128" }, { ('3', '2'), "256" }, { ('3', '1'), "888" } });
		ProcessPairs("77777", "777777777", new() { { ('7', '7'), "7777" }, { ('8', '8'), "8888" } });
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char), G.IEnumerable<char>>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char), String>)null!));
		ProcessTriples("3692581470", "3692581470", []);
		ProcessTriples("3692581470", "36969581400", new() { { ('6', '9', '2'), "6969" }, { ('1', '4', '7'), "140" } });
		ProcessTriples("3692581470", "391114641470", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("", "", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("6", "6", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("256256256256256!", "40964096409640964096!", new() { { ('2', '5', '6'), "4096" }, { ('5', '6', '2'), "2048" }, { ('6', '2', '5'), "888" } });
		ProcessTriples("3232323232!", "2048409620482!", new() { { ('2', '3', '2'), "4096" }, { ('3', '2', '3'), "2048" }, { ('3', '1', '2'), "888" } });
		ProcessTriples("77777777", "777777777777", new() { { ('7', '7', '7'), "77777" }, { ('8', '8', '8'), "88888" } });
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char, char), G.IEnumerable<char>>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char, char), String>)null!));
		static void ProcessPairs(string s, string c, Dictionary<(char, char), G.IEnumerable<char>> dic)
		{
			String a = s;
			var b = a.ReplaceInPlace(dic);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		static void ProcessTriples(string s, string c, Dictionary<(char, char, char), G.IEnumerable<char>> dic)
		{
			String a = s;
			var b = a.ReplaceInPlace(dic);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}
}

public class BaseSumListTests<T, TCertain> where T : INumber<T> where TCertain : BaseSumList<T, TCertain>, new()
{
	public static void ComplexTest(Func<(BaseSumList<T, TCertain>, G.List<T>, byte[])> create, Func<T> newValueFunc, Action<int> check, Action<byte[]> check2)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
		var toInsert = Array.Empty<T>();
	l1:
		var (sl, gl, bytes) = create();
		var collectionActions = new[] { () =>
		{
			if (random.Next(2) == 0)
			{
				toInsert = RedStarLinq.FillArray(random.Next(6), _ => newValueFunc());
				sl.AddRange(toInsert);
				gl.AddRange(toInsert);
				Assert.IsTrue(sl.Equals(gl));
				Assert.IsTrue(E.SequenceEqual(gl, sl));
			}
			else
			{
				var n = random.Next(sl.Length);
				toInsert = RedStarLinq.FillArray(random.Next(6), _ => newValueFunc());
				sl.Insert(n, toInsert);
				gl.InsertRange(n, toInsert);
				Assert.IsTrue(sl.Equals(gl));
				Assert.IsTrue(E.SequenceEqual(gl, sl));
			}
		}, () =>
		{
			var length = Min(random.Next(9), sl.Length);
			if (sl.Length < length)
				return;
			var start = random.Next(sl.Length - length + 1);
			sl.Remove(start, length);
			gl.RemoveRange(start, length);
			Assert.IsTrue(sl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, sl));
		} };
		var updateActions = new[] { (int key) =>
		{
			var newValue = newValueFunc();
			sl.Update(key, newValue);
			if (newValue <= T.Zero)
				gl.RemoveAt(key);
			else
				gl[key] = newValue;
			Assert.IsTrue(sl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, sl));
		}, key =>
		{
			sl.Increase(key);
			gl[key]++;
			Assert.IsTrue(sl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, sl));
		}, key =>
		{
			if (sl[key] <= T.One)
				gl.RemoveAt(key);
			else
				gl[key]--;
			sl.Decrease(key);
			Assert.IsTrue(sl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, sl));
		} };
		var actions = new[] { () =>
		{
			var n = newValueFunc();
			while (n < T.One)
				n = newValueFunc();
			if (random.Next(2) == 0)
			{
				sl.Add(n);
				gl.Add(n);
			}
			else
			{
				var index = random.Next(sl.Length + 1);
				sl.Insert(index, n);
				gl.Insert(index, n);
			}
			Assert.IsTrue(RedStarLinq.Equals(sl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, sl));
		}, () =>
		{
			if (sl.Length == 0) return;
			var index = random.Next(sl.Length);
			gl.RemoveAt(index);
			sl.RemoveAt(index);
			Assert.IsTrue(RedStarLinq.Equals(sl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, sl));
		}, () =>
		{
			var sl2 = sl.Reverse();
			gl.Reverse();
			Assert.IsTrue(sl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, sl));
			Assert.AreEqual(sl2, sl);
		}, () =>
		{
			var length = Min(random.Next(17), (int)sl.Length);
			if (sl.Length < length)
				return;
			var start = random.Next((int)sl.Length - length + 1);
			var sl2 = sl.Reverse(start, length);
			gl.Reverse(start, length);
			Assert.IsTrue(sl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, sl));
			Assert.AreEqual(sl2, sl);
		}, () =>
		{
			collectionActions.Random(random)();
			Assert.IsTrue(RedStarLinq.Equals(sl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, sl));
		}, () =>
		{
			if (sl.Length == 0) return;
			var index = random.Next(sl.Length);
			updateActions.Random(random)(index);
			Assert.IsTrue(RedStarLinq.Equals(sl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, sl));
			check(index);
		}, () =>
		{
			random.NextBytes(bytes);
			check2(bytes);
		}, () =>
		{
			if (sl.Length == 0) return;
			var index = random.Next(sl.Length);
			Assert.AreEqual(sl[index], gl[index]);
			Assert.IsTrue(E.SequenceEqual(gl, sl));
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 1000)
			goto l1;
	}
}

[TestClass]
public class SumListTests
{
	private SumList sl = default!;
	private G.List<int> gl = default!;

	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		BaseSumListTests<int, SumList>.ComplexTest(() =>
	{
		var arr = RedStarLinq.FillArray(16, _ => random.Next(1, 16));
		sl = [.. arr];
		gl = [.. arr];
		var bytes = new byte[16];
		return (sl, gl, bytes);
	}, () => random.Next(1, 16), index => Assert.AreEqual(sl.GetLeftValuesSum(index, out var value), E.Sum(E.Take(gl, index))), bytes =>
	{
		var index = sl.IndexOfNotGreaterSum(CreateVar((long)(new MpzT(bytes, 1) % (sl.ValuesSum + 1)), out var sum));
		Assert.IsTrue(index == gl.Count && sum == E.Sum(gl) || CreateVar(E.Sum(E.Take(gl, index)), out var sum2) <= sum && (gl[index] == 0 || sum2 + gl[index] > sum));
	});
	}
}

[TestClass]
public class BigSumListTests
{
	private BigSumList sl = default!;
	private G.List<MpzT> gl = default!;
	private readonly byte[] bytes = new byte[20], bytes2 = new byte[48];

	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		BaseSumListTests<MpzT, BigSumList>.ComplexTest(() =>
	{
		var arr = RedStarLinq.FillArray(16, _ =>
		{
			random.NextBytes(bytes);
			return new MpzT([0, .. bytes], 1);
		});
		sl = [.. arr];
		gl = [.. arr];
		return (sl, gl, bytes2);
	}, () =>
	{
		random.NextBytes(bytes);
		return new([0, .. bytes], 1);
	}, index => Assert.AreEqual(sl.GetLeftValuesSum(index, out var value), index == 0 ? 0 : E.Aggregate(E.Take(gl, index), (x, y) => x + y)), bytes =>
	{
		var index = sl.IndexOfNotGreaterSum(CreateVar(new MpzT(bytes, 1) % (sl.ValuesSum + 1), out var sum));
		Assert.IsTrue(index == 0 && (gl.Count == 0 || sum < gl[0]) || index == gl.Count && sum == E.Aggregate(gl, (x, y) => x + y) || CreateVar(E.Aggregate(E.Take(gl, index + 1), (x, y) => x + y), out var sum2) > sum && (gl[index] == 0 || sum2 + gl[index] > sum));
	});
	}
}
