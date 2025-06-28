using NStar.ParallelHS;

namespace Benchmark;

[MemoryDiagnoser]
public class Benchmark
{
	private static readonly Random random = new(1234567890);
	private static readonly NList<int> list8 = RedStarLinq.NFill(x => random.Next(0, 65536), 100000000);

	public G.HashSet<int> GHashSet { get; set; } = [];
	public ParallelHashSet<int> ParallelHashSet { get; set; } = [];

	[Benchmark]
	public void GHashSetTest() => GHashSet = new(list8);

	[Benchmark]
	public void ParallelHashSetTest() => ParallelHashSet = new(list8);
}
