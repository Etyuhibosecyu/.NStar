namespace Benchmark;

[MemoryDiagnoser]
public class Benchmark
{
	private static readonly Random random = new(1234567890);
	private static readonly NList<int> list7 = RedStarLinq.NFill(x => random.Next(0, 65536), 10000000);
	private static readonly NList<int> list8 = RedStarLinq.NFill(x => random.Next(0, 65536), 100000000);

	public G.HashSet<int> GHashSet { get; set; } = [];
	public ParallelHashSet<int> ParallelHashSet { get; set; } = [];
	public double OldComplex { get; set; }
	public double MyComplex { get; set; }
	public G.List<int> OldIndexesOfMax { get; set; } = [];
	public NList<int> MyIndexesOfMax { get; set; } = [];
	public G.List<int> OldSort { get; set; } = [];
	public List<int> MySort { get; set; } = [];

	[Benchmark]
	public void GHashSetTest() => GHashSet = new(list8);

	[Benchmark]
	public void ParallelHashSetTest() => ParallelHashSet = new(list8);

	[Benchmark]
	public void OldComplexTest() => OldComplex = E.Average(E.Where(E.Zip(E.Skip(list8, 1), E.Skip(list8, 2), (x, y) => (Item1: x, Item2: y)), x => Math.Abs(x.Item1 - x.Item2) < 4096), x => Math.Abs(x.Item1 - x.Item2));

	[Benchmark]
	public void MyComplexTest() => MyComplex = list8.AsSpan(1).Combine(list8.AsSpan(2), (x, y) => (Item1: x, Item2: y)).FilterInPlace(x => Math.Abs(x.Item1 - x.Item2) < 4096).Mean(x => Math.Abs(x.Item1 - x.Item2));

	[Benchmark]
	public void OldIndexesOfMaxTest()
	{
		var max = E.Max(list8, x => x);
		OldIndexesOfMax = E.ToList(E.Select(E.Where(E.Select(list8, (elem, index) => (elem, index)), x => x.elem == max), x => x.index));
	}

	[Benchmark]
	public void MyIndexesOfMaxTest() => MyIndexesOfMax = list8.IndexesOfMax();

	[Benchmark]
	public void OldSortTest() => OldSort = E.ToList(E.OrderBy(new List<int>(list7), x => x));

	[Benchmark]
	public void MySortTest() => MySort = new List<int>(list7).Sort(x => x);
}
