namespace SortedSets.NStar;

public static unsafe class SortedSetsExtents
{
	public static int NthAbsent<TCertain>(this BaseSortedSet<int, TCertain> set, int n) where TCertain : BaseSortedSet<int, TCertain>, new()
	{
		ArgumentNullException.ThrowIfNull(set);
		if (set.Comparer != G.Comparer<int>.Default)
			throw new ArgumentException("Множество должно иметь стандартный для int компаратор.", nameof(set));
		if (set.Length == 0)
			return n;
		if (set[0] < 0)
			throw new ArgumentException("Не допускается множество, содержащее отрицательные значения.", nameof(set));
		var lo = 0;
		var hi = set.Length - 1;
		var comparer = G.Comparer<int>.Default;
		while (lo <= hi)
		{
			// i might overflow if lo and hi are both large positive numbers. 
			var i = lo + ((hi - lo) >> 1);
			int c;
			try
			{
				c = comparer.Compare(set[i] - i, n);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(null, ex);
			}
			if (c == 0)
			{
				var result = n + i + 1;
				while (++i < set.Length && set[i] == result)
					result++;
				return result;
			}
			if (c < 0)
				lo = i + 1;
			else
				hi = i - 1;
		}
		return n + lo;
	}
}
