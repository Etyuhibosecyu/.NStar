global using NStar.BufferLib;
global using NStar.Core;
global using System;
global using G = System.Collections.Generic;
global using static NStar.Core.Extents;
using NStar.Dictionaries;

namespace NStar.ExtraReplacing;

public static class ReplaceExtents
{
#pragma warning restore CS8714 // Тип не может быть использован как параметр типа в универсальном типе или методе. Допустимость значения NULL для аргумента типа не соответствует ограничению "notnull".

	public static TCertain Replace<T, TCertain>(this BaseList<T, TCertain> source, Dictionary<(T, T), G.IEnumerable<T>>? dic) where TCertain : BaseList<T, TCertain>, new()
	{
		ArgumentNullException.ThrowIfNull(dic);
		if (source.Length < 2)
			return [.. source];
		TCertain result = [];
		result.Capacity = source.Length + GetArrayLength(source.Length, 10);
		using Buffer<T> buffer = new(2) { source[0] };
		for (var i = 1; i < source.Length; i++)
		{
			buffer.Add(source[i]);
			if (!buffer.IsFull)
				continue;
			if (dic.TryGetValue((buffer[0], buffer[1]), out var newCollection))
			{
				result.AddRange(newCollection);
				buffer.Clear(false);
			}
			else
				result.Add(buffer.GetAndRemove(0));
		}
		return result.AddRange(buffer);
	}

	public static TCertain Replace<T, TCertain>(this BaseList<T, TCertain> source, Dictionary<(T, T), TCertain>? dic) where TCertain : BaseList<T, TCertain>, new() => Replace(source, dic?.ToDictionary(x => x.Key, x => (G.IEnumerable<T>)x.Value)!);

	public static TCertain Replace<T, TCertain>(this BaseList<T, TCertain> source, Dictionary<(T, T, T), G.IEnumerable<T>>? dic) where TCertain : BaseList<T, TCertain>, new()
	{
		ArgumentNullException.ThrowIfNull(dic);
		if (source.Length < 3)
			return [.. source];
		TCertain result = [];
		result.Capacity = source.Length + GetArrayLength(source.Length, 10);
		using Buffer<T> buffer = new(3) { source[0], source[1] };
		for (var i = 2; i < source.Length; i++)
		{
			buffer.Add(source[i]);
			if (!buffer.IsFull)
				continue;
			if (dic.TryGetValue((buffer[0], buffer[1], buffer[2]), out var newCollection))
			{
				result.AddRange(newCollection);
				buffer.Clear(false);
			}
			else
				result.Add(buffer.GetAndRemove(0));
		}
		return result.AddRange(buffer);
	}

	public static TCertain Replace<T, TCertain>(this BaseList<T, TCertain> source, Dictionary<(T, T, T), TCertain>? dic) where TCertain : BaseList<T, TCertain>, new() => Replace(source, dic?.ToDictionary(x => x.Key, x => (G.IEnumerable<T>)x.Value)!);
#pragma warning disable CS8714 // Тип не может быть использован как параметр типа в универсальном типе или методе. Допустимость значения NULL для аргумента типа не соответствует ограничению "notnull".

	public static TCertain ReplaceInPlace<T, TCertain>(this BaseList<T, TCertain> source, Dictionary<(T, T), G.IEnumerable<T>>? dic) where TCertain : BaseList<T, TCertain>, new() => source.Replace(Replace(source, dic));

	public static TCertain ReplaceInPlace<T, TCertain>(this BaseList<T, TCertain> source, Dictionary<(T, T), TCertain>? dic) where TCertain : BaseList<T, TCertain>, new() => source.Replace(Replace(source, dic));

	public static TCertain ReplaceInPlace<T, TCertain>(this BaseList<T, TCertain> source, Dictionary<(T, T, T), G.IEnumerable<T>>? dic) where TCertain : BaseList<T, TCertain>, new() => source.Replace(Replace(source, dic));

	public static TCertain ReplaceInPlace<T, TCertain>(this BaseList<T, TCertain> source, Dictionary<(T, T, T), TCertain>? dic) where TCertain : BaseList<T, TCertain>, new() => source.Replace(Replace(source, dic));
}
