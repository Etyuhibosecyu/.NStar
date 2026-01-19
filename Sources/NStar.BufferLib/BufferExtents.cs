namespace NStar.BufferLib;

internal static class BufferExtents
{
	private static G.KeyValuePair<int, T>? FindMinGreaterOrEqual<T>(this G.SortedSet<G.KeyValuePair<int, T>> set, int value)
	{
		if (value > set.Max.Key)
			return default;
		// Получаем представление набора от value до максимального элемента
		var view = set.GetViewBetween(new(value, default!), set.Max);
		// Возвращаем первый элемент, если он есть
		var en = view.GetEnumerator();
		return en.MoveNext() ? en.Current : default;
	}

	internal static T[] GetAndRemove<T>(this G.SortedDictionary<int, G.List<T[]>> dic, int value, bool exact = true)
	{
		if (dic.Count == 0)
			return GC.AllocateUninitializedArray<T>(value);
		if (exact)
		{
			if (!dic.TryGetValue(value, out var exactList))
				return GC.AllocateUninitializedArray<T>(value);
			var exactResult = exactList[^1];
			exactList.RemoveAt(exactList.Count - 1);
			if (exactList.Count == 0)
				dic.Remove(value);
			return exactResult;
		}
		if (dic.GetType().GetField("_set", System.Reflection.BindingFlags.Instance
			| System.Reflection.BindingFlags.NonPublic)?.GetValue(dic) is not G.SortedSet<G.KeyValuePair<int, G.List<T[]>>> set)
			return GC.AllocateUninitializedArray<T>(value);
		if (set.FindMinGreaterOrEqual(value) is not G.KeyValuePair<int, G.List<T[]>> found)
			return GC.AllocateUninitializedArray<T>(value);
		var index = found.Key;
		var list = dic[index];
		var result = list[^1];
		list.RemoveAt(list.Count - 1);
		if (list.Count == 0)
			dic.Remove(index);
		return result;
	}
}
