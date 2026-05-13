namespace NStar.SumCollections;

internal delegate bool SumWalkPredicate(SumList.Node node);

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class SumList : BaseSumList<int, SumList>
{
	public SumList() { }

	public SumList(G.IEnumerable<int> collection) : this()
	{
		ArgumentNullException.ThrowIfNull(collection);
		// These are explicit type checks in the mold of HashSet. It would have worked better with
		// something like an ISorted interface. (We could make this work for SortedList.Keys, etc.)
		if (collection is SumList sumList && sumList is not TreeSubSet)
		{
			if (sumList.Length > 0)
			{
				Debug.Assert(sumList.root is not null);
				_size = sumList._size;
				root = sumList.root.DeepClone(_size);
			}
			return;
		}
		var elements = collection is int[] array ? array : [.. collection];
		var length = elements.Length;
		if (length > 0)
		{
			root = ConstructRootFromSortedArray(elements, 0, length - 1, null);
			_size = length;
		}
	}

	public SumList(params int[] array) : this((G.IEnumerable<int>)array) { }

	public SumList(ReadOnlySpan<int> span) : this((G.IEnumerable<int>)span.ToArray()) { }

	protected override Func<int, SumList> CapacityCreator => x => [];

	protected override Func<G.IEnumerable<int>, SumList> CollectionCreator { get; } = x => new(x);

	protected override Func<ReadOnlySpan<int>, SumList> SpanCreator { get; } = x => new(x);

	public virtual long ValuesSum => (root as Node)?.ValuesSum ?? 0;

	protected override void ClearInternal(int index, int length)
	{
		_size += length;
		using var subset = new TreeSubSet(this, index, index + length - 1, true, true);
		subset.Clear();
	}

	protected override void CopyToInternal(int sourceIndex, SumList destination, int destinationIndex, int length)
	{
		if (length == 0)
			return;
		if (length == 1)
		{
			destination.SetOrAddInternal(destinationIndex, GetInternal(sourceIndex));
			return;
		}
		using TreeSubSet subset = new(this, sourceIndex, sourceIndex + length - 1, true, true);
		using SumList list = new(subset);
		using var en = list.GetEnumerator();
		if (destinationIndex < destination._size)
		{
			using var destSubSet = new TreeSubSet(destination, destinationIndex,
				Min(destinationIndex + length, destination._size) - 1, true, true);
			destSubSet.InOrderTreeWalk(node =>
			{
				var b = en.MoveNext();
				if (b)
					node.Update(en.Current);
				return b;
			});
		}
		while (en.MoveNext())
			destination.Add(en.Current);
	}

	protected override void CopyToInternal(int index, int[] array, int arrayIndex, int length)
	{
		ArgumentNullException.ThrowIfNull(array);
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (length > array.Length - index)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		length += index; // Make `length` the upper bound.
		var i = 0;
		InOrderTreeWalk(node =>
		{
			if (i >= length)
				return false;
			if (i++ < index)
				return true;
			array[arrayIndex++] = node.Value;
			return true;
		});
	}

	public override G.IEnumerator<int> GetEnumerator() => new Enumerator(this);

	protected override int GetInternal(int index)
	{
		var current = root;
		while (current is not null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
				return current.Value;
			else if (current.Left is null)
			{
				index--;
				current = current.Right;
			}
			else if (current.Left.LeavesCount > index)
				current = current.Left;
			else
			{
				index -= current.Left.LeavesCount + 1;
				current = current.Right;
			}
		}
		throw new ArgumentOutOfRangeException(nameof(index));
	}

	public virtual long GetLeftValuesSum(int index, out int actualValue)
	{
		var current = root as Node;
		long sum = 0;
		while (current is not null)
		{
			var order = Comparer.Compare(index, current.Left?.LeavesCount ?? 0);
			if (order == 0)
			{
				actualValue = current.Value;
				return sum + (current.Left?.ValuesSum ?? 0);
			}
			else if (order < 0)
				current = current.Left;
			else
			{
				index -= (current.Left?.LeavesCount ?? 0) + 1;
				sum += (current.Left?.ValuesSum ?? 0) + current.Value;
				current = current.Right;
			}
		}
		actualValue = 0;
		return sum;
	}

	public virtual SumList GetViewBetween(int lowerValue, int upperValue)
	{
		if (Comparer.Compare(lowerValue, upperValue) > 0)
			throw new ArgumentException("Максимум не может быть меньше минимума!");
		return new TreeSubSet(this, lowerValue, upperValue, true, true);
	}

	public virtual int IndexOfNotGreaterSum(long sum) => IndexOfNotGreaterSum(sum, out _);

	public virtual int IndexOfNotGreaterSum(long sum, out int sumExceedsBy)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(sum);
		if (sum >= ValuesSum)
		{
			sumExceedsBy = (int)Min(sum - ValuesSum, int.MaxValue);
			return _size;
		}
		var oldSum = sum;
		var current = root as Node;
		sumExceedsBy = 0;
		var index = 0;
		while (current is not null)
		{
			if (sum == (current.Left?.ValuesSum ?? 0))
				return index + (current.Left?.LeavesCount ?? 0);
			else if (sum < (current.Left?.ValuesSum ?? 0))
			{
				sumExceedsBy = current.Value;
				current = current.Left;
			}
			else
			{
				index += (current.Left?.LeavesCount ?? 0) + 1;
				sum -= (current.Left?.ValuesSum ?? 0) + current.Value;
				sumExceedsBy = current.Value;
				current = current.Right;
			}
		}
		sumExceedsBy += (-sum >> 32 == 0) ? (int)sum
			: throw new InvalidOperationException("Невозможно сопоставить какой-либо индекс сумме " + oldSum
			+ ". Возможные причины:\r\n"
			+ "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
			+ "2. Нарушение целостности структуры исходной коллекции (ошибка в логике - наши коллекции"
			+ " все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
			+ "3. Системная ошибка (память, диск и т. д.).\r\n"
			+ $"Текущее состояние: тип коллекции - {typeof(SumList)}, длина - {Length},"
			+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
		return index - 1;
	}

	protected override void SetAllInternal(int value, int index, int length)
	{
		int oldLength = _size, endIndex = index + length - 1;
		for (var i = index; i < Min(_size, endIndex); i++)
			SetInternal(i, value);
		for (var i = _size; i < endIndex; i++)
			Add(value);
		_size = oldLength;
	}

	protected override void SetInternal(int index, int value)
	{
		if (value == 0)
		{
			RemoveAt(index);
			return;
		}
		var current = root;
		while (current is not null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
			{
				current.Update(value);
				return;
			}
			else if (current.Left is null)
			{
				index--;
				current = current.Right;
			}
			else if (current.Left.LeavesCount >= index)
				current = current.Left;
			else
			{
				index -= current.Left.LeavesCount + 1;
				current = current.Right;
			}
		}
		throw new ArgumentOutOfRangeException(nameof(index));
	}

	[DebuggerDisplay("{Value.ToString()}, Left = {Left is not null ? Left.Value.ToString() : null},"
		+ " Right = {Right is not null ? Right.Value.ToString() : null},"
		+ " Parent = {Parent is not null ? Parent.Value.ToString() : null}")]
	internal new sealed class Node : BaseSumList<int, SumList>.Node
	{
		private new Node? Parent { get => base.Parent as Node; set => base.Parent = value; }
		private long _valuesSum;

		private Node(int value, NodeColor color) : base(value, color) => _valuesSum = value;

		internal new Node? Left { get => base.Left as Node; set => base.Left = value; }

		internal new Node? Right { get => base.Right as Node; set => base.Right = value; }

		internal long ValuesSum
		{
			get => _valuesSum;
			set
			{
				Parent?.ValuesSum += value - _valuesSum;
				_valuesSum = value;
				if (Parent is null
					|| Parent.ValuesSum == (Parent.Left?.ValuesSum ?? 0) + (Parent.Right?.ValuesSum ?? 0) + Parent.Value)
					return;
				throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
			}
		}

		internal override Node DeepClone(int length)
		{
#if VERIFY
			Debug.Assert(length == GetCount());
#endif
			var newRoot = ShallowClone();
			using var pendingNodes = (Stack<(Node source, Node target)>?)typeof(Stack<(Node source, Node target)>)
				.GetMethod("GetNew", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, [2 * Log2(length + 1)]);
			Debug.Assert(pendingNodes is not null);
			pendingNodes.Push((this, newRoot));
			while (pendingNodes.TryPop(out var next))
			{
				Node clonedNode;
				if (next.source.Left is Node left)
				{
					clonedNode = left.ShallowClone();
					next.target.Left = clonedNode;
					pendingNodes.Push((left, clonedNode));
				}
				if (next.source.Right is Node right)
				{
					clonedNode = right.ShallowClone();
					next.target.Right = clonedNode;
					pendingNodes.Push((right, clonedNode));
				}
			}
			return newRoot;
		}

		/// <summary>
		/// Gets the sibling of one of this node's children.
		/// </summary>
		internal Node GetSibling(Node node) => base.GetSibling(node) as Node
			?? throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
			" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");

		private protected override BaseSumList<int, SumList>.Node Reconstruct(int value, NodeColor color)
		{
			var node = base.Reconstruct(value, color) as Node
			?? throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
			" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
			node._valuesSum = value;
			return node;
		}

		internal override Node ShallowClone() => new(Value, Color);

		internal override void Update(int value)
		{
			ValuesSum += value - Value;
			Value = value;
		}

		internal override void UpdateValuesSum(BaseSumList<int, SumList>.Node? newNode,
			BaseSumList<int, SumList>.Node? oldNode) =>
			ValuesSum += ((newNode as Node)?.ValuesSum ?? 0) - ((oldNode as Node)?.ValuesSum ?? 0);

#if VERIFY
		internal override void Verify()
		{
			base.Verify();
			if (ValuesSum != (Left?.ValuesSum ?? 0) + (Right?.ValuesSum ?? 0) + Value)
				throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
		}
#endif
	}

	internal sealed class TreeSubSet : SumList
	{
		private readonly SumList _underlying;
		internal readonly int _min, _max;
		// keeps track of whether the length variable is up to date
		// up to date -> _countVersion = _underlying.version
		// not up to date -> _countVersion < _underlying.version
		private int _countVersion;
		// these exist for unbounded collections
		// for instance, you could allow this subset to be defined for i > 10. The list will throw if
		// anything <= 10 is added, but there is no upper bound. These features Head(), Tail(), were punted
		// in the spec, and are not available, but the framework is there to make them available at some point.
		internal readonly bool _lBoundActive, _uBoundActive;

		public TreeSubSet() : base()
		{
			_underlying = default!;
			version = 0;
			_countVersion = 0;
			_min = 0;
			_max = 0;
			_lBoundActive = false;
			_uBoundActive = false;
			root = default!;
			_size = 0;
		}

		public TreeSubSet(SumList Underlying, int Min, int Max, bool lowerBoundActive, bool upperBoundActive) : base()
		{
			_underlying = Underlying;
			version = Underlying.version;
			_countVersion = Underlying.version;
			_min = Min;
			_max = Max;
			_lBoundActive = lowerBoundActive;
			_uBoundActive = upperBoundActive;
			root = _underlying.root; // root is first element within range
			_size = Max - Min + 1;
		}

		internal override int MaxInternal => MaxInternalStatic(this);
		internal override int MinInternal => MinInternalStatic(this);

		internal override bool BreadthFirstTreeWalk(BaseSumWalkPredicate<int, SumList> action)
		{
			VersionCheck();
			if (root is null)
				return true;
			using Queue<Node> processQueue = [];
			processQueue.Enqueue(root as Node
				?? throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
				" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar."));
			Node current;
			while (processQueue.Length != 0)
			{
				current = processQueue.Dequeue();
				if (IsWithinRange(current.Left?.LeavesCount ?? 0) && !action(current))
					return false;
				if (current.Left is not null && (!_lBoundActive || Comparer.Compare(_min, current.Left.LeavesCount) < 0))
					processQueue.Enqueue(current.Left);
				if (current.Right is not null && (!_uBoundActive || Comparer.Compare(_max, current.Left?.LeavesCount ?? 0) > 0))
					processQueue.Enqueue(current.Right);
			}
			return true;
		}

		public override void Clear(bool deep)
		{
			if (Length == 0)
				return;
			List<int> toRemove = [];
			var i = 0;
			InOrderTreeWalk(n => { toRemove.Add(_min + i++); return true; });
			while (toRemove.Length != 0)
			{
				_underlying.RemoveAt(toRemove[^1]);
				toRemove.RemoveAt(^1);
			}
			root = null;
			_size = 0;
			version = _underlying.version;
		}

		internal override Node? FindNode(int index)
		{
			if (!IsWithinRange(index))
				return null;
			VersionCheck();
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
#endif
			return base.FindNode(index) as Node;
		}

		protected override int GetInternal(int index) => _underlying.GetInternal(_min + index);

		// This passes functionality down to the underlying tree, clipping edges if necessary
		// There's nothing gained by having a nested subset. May as well draw it from the base
		// Cannot increase the bounds of the subset, can only decrease it
		public override SumList GetViewBetween(int lowerValue, int upperValue)
		{
			if (_lBoundActive && Comparer.Compare(_min, lowerValue) > 0)
				throw new ArgumentOutOfRangeException(nameof(lowerValue));
			if (_uBoundActive && Comparer.Compare(_max, upperValue) < 0)
				throw new ArgumentOutOfRangeException(nameof(upperValue));
			return (TreeSubSet)_underlying.GetViewBetween(lowerValue, upperValue);
		}

		internal override bool InOrderTreeWalk(BaseSumWalkPredicate<int, SumList> action) => InOrderTreeWalk(this, action);

		public override SumList Insert(int index, int value)
		{
			if (!IsWithinRange(index))
				throw new ArgumentOutOfRangeException(nameof(value));
			var ret = _underlying.Insert(_min + index, value);
			VersionCheck();
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.root);
#endif
			return ret;
		}

		internal override bool IsWithinRange(int index)
		{
			var comp = _lBoundActive ? Comparer.Compare(_min, index) : -1;
			if (comp > 0)
				return false;
			comp = _uBoundActive ? Comparer.Compare(_max, index) : 1;
			return comp >= 0;
		}

		protected override void SetInternal(int index, int value) => _underlying.SetInternal(_min + index, value);

		/// <summary>
		/// Returns the number of elements <c>length</c> of the parent list.
		/// </summary>
		internal override int TotalCount()
		{
			Debug.Assert(_underlying is not null);
			return _underlying.Length;
		}

		/// <summary>
		/// Checks whether this subset is out of date, and updates it if necessary.
		/// </summary>
		/// <param name="updateCount">Updates the length variable if necessary.</param>
		internal override void VersionCheck(bool updateCount = false) => VersionCheckImpl(updateCount);

		private void VersionCheckImpl(bool updateCount)
		{
			Debug.Assert(_underlying is not null);
			if (version != _underlying.version)
			{
				root = _underlying.FindRange(_min, _max, _lBoundActive, _uBoundActive);
				version = _underlying.version;
			}
			if (updateCount && _countVersion != _underlying.version)
			{
				_size = 0;
				InOrderTreeWalk(n => { _size++; return true; });
				_countVersion = _underlying.version;
			}
		}

#if DEBUG
		internal override bool VersionUpToDate() => version == _underlying.version;
#endif
	}
}
