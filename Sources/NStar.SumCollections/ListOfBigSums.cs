namespace NStar.SumCollections;

internal delegate bool BigSumWalkPredicate(ListOfBigSums.Node node);

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class ListOfBigSums : BaseSumList<MpzT, ListOfBigSums>
{
	public ListOfBigSums() { }

	public ListOfBigSums(G.IEnumerable<MpzT> collection) : this()
	{
		ArgumentNullException.ThrowIfNull(collection);
		// These are explicit type checks in the mold of HashSet. It would have worked better with
		// something like an ISorted interface. (We could make this work for SortedList.Keys, etc.)
		if (collection is ListOfBigSums sumList && sumList is not TreeSubSet)
		{
			if (sumList.Length > 0)
			{
				Debug.Assert(sumList.root is not null);
				_size = sumList._size;
				root = sumList.root.DeepClone(_size);
			}
			return;
		}
		var elements = collection is MpzT[] array ? array : collection.ToArray();
		var length = elements.Length;
		if (length > 0)
		{
			root = ConstructRootFromSortedArray(elements, 0, length - 1, null);
			_size = length;
		}
	}

	public ListOfBigSums(params MpzT[] array) : this((G.IEnumerable<MpzT>)array) { }

	public ListOfBigSums(ReadOnlySpan<MpzT> span) : this((G.IEnumerable<MpzT>)span.ToArray()) { }

	public override MpzT this[Index index, bool invoke = false]
	{
		get => base[index, invoke];
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegative(value);
			base[index, invoke] = value;
		}
	}

	protected override Func<int, ListOfBigSums> CapacityCreator => x => [];

	protected override Func<G.IEnumerable<MpzT>, ListOfBigSums> CollectionCreator { get; } = x => new(x);

	protected override Func<ReadOnlySpan<MpzT>, ListOfBigSums> SpanCreator { get; } = x => new(x);

	public override int Length
	{
		get
		{
			VersionCheck(updateCount: true);
			return _size;
		}
	}

	public virtual MpzT ValuesSum => new((root as Node)?.ValuesSum ?? 0);

	protected override void ClearInternal(int index, int length)
	{
		_size += length;
		using var subset = new TreeSubSet(this, index, index + length - 1, true, true);
		subset.Clear();
	}

	protected override void CopyToInternal(int sourceIndex, ListOfBigSums destination, int destinationIndex, int length)
	{
		if (length == 0)
			return;
		if (length == 1)
		{
			destination.SetOrAdd(destinationIndex, GetInternal(sourceIndex));
			return;
		}
		using TreeSubSet subset = new(this, sourceIndex, sourceIndex + length - 1, true, true);
		using ListOfBigSums list = new(subset);
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

	protected override void CopyToInternal(int index, MpzT[] array, int arrayIndex, int length)
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
			array[arrayIndex++] = new(node.Value);
			return true;
		});
	}

	protected override void DisposeInternal()
	{
		root = null;
		_size = 0;
		version = 0;
		Changed();
	}

	public override G.IEnumerator<MpzT> GetEnumerator() => new Enumerator(this);

	protected override MpzT GetInternal(int index)
	{
		var current = root;
		while (current is not null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
				return new(current.Value);
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

	public virtual MpzT GetLeftValuesSum(int index, out MpzT actualValue)
	{
		var current = root as Node;
		var sum = MpzT.Zero;
		while (current is not null)
		{
			var order = Comparer.Compare(index, current.Left?.LeavesCount ?? 0);
			if (order == 0)
			{
				actualValue = new(current.Value);
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

	public virtual ListOfBigSums GetViewBetween(int lowerValue, int upperValue)
	{
		if (Comparer.Compare(lowerValue, upperValue) > 0)
			throw new ArgumentException("Максимум не может быть меньше минимума!");
		return new TreeSubSet(this, lowerValue, upperValue, true, true);
	}

	public virtual int IndexOfNotGreaterSum(MpzT sum) => IndexOfNotGreaterSum(sum, out _);

	public virtual int IndexOfNotGreaterSum(MpzT sum, out MpzT sumExceedsBy)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(sum);
		if (sum >= ValuesSum)
		{
			sumExceedsBy = sum - ValuesSum;
			return _size;
		}
		sum = new(sum);
		var current = root as Node;
		sumExceedsBy = 0;
		var index = 0;
		while (current is not null)
		{
			var left = current.Left;
			var leftCount = left?.LeavesCount ?? 0;
			var leftSum = left?.ValuesSum ?? 0;
			if (sum < leftSum)
			{
				Mpir.Mpir.MpzSet(sumExceedsBy, current.Value);
				current = left;
			}
			else if (sum < leftSum + current.Value)
			{
				Mpir.Mpir.MpzSub(sumExceedsBy, sum, leftSum);
				return index + leftCount;
			}
			else
			{
				index += leftCount + 1;
				Mpir.Mpir.MpzSub(sum, sum, leftSum + current.Value);
				Mpir.Mpir.MpzSet(sumExceedsBy, current.Value);
				current = current.Right;
			}
		}
		Mpir.Mpir.MpzAdd(sumExceedsBy, sumExceedsBy, sum);
		return index - 1;
	}

	protected override void SetInternal(int index, MpzT value)
	{
		if (value <= 0)
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

	[DebuggerDisplay("{Value.ToString()}, Left = {Left?.Value.ToString()}, Right = {Right?.Value.ToString()}, Parent = {Parent?.Value.ToString()}")]
	internal new sealed class Node : BaseSumList<MpzT, ListOfBigSums>.Node
	{
		private new Node? Parent { get => base.Parent as Node; set => base.Parent = value; }
		private MpzT _valuesSum;

		private Node(MpzT value, NodeColor color) : base(new(value), color) => _valuesSum = new(value);

		internal new Node? Left { get => base.Left as Node; set => base.Left = value; }

		internal new Node? Right { get => base.Right as Node; set => base.Right = value; }

		internal MpzT ValuesSum
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
		/// Gets the rotation this node should undergo during a removal.
		/// </summary>
		internal TreeRotation GetRotation(Node current, Node sibling)
		{
			Debug.Assert(IsNonNullRed(sibling.Left) || IsNonNullRed(sibling.Right));
#if VERIFY
			Debug.Assert(HasChildren(current, sibling));
#endif
			var currentIsLeftChild = Left == current;
			return IsNonNullRed(sibling.Left) ?
				(currentIsLeftChild ? TreeRotation.RightLeft : TreeRotation.Right) :
				(currentIsLeftChild ? TreeRotation.Left : TreeRotation.LeftRight);
		}

		/// <summary>
		/// Gets the sibling of one of this node's children.
		/// </summary>
		internal Node GetSibling(Node node)
		{
			Debug.Assert(node is not null);
			Debug.Assert(node == Left ^ node == Right);
			return node == Left ? Right! : Left!;
		}

		internal static bool IsNonNullBlack(Node? node) => node is not null && node.IsBlack;

		internal static bool IsNonNullRed(Node? node) => node is not null && node.IsRed;

		internal static bool IsNullOrBlack(Node? node) => node is null || node.IsBlack;

		private protected override BaseSumList<MpzT, ListOfBigSums>.Node Reconstruct(MpzT value, NodeColor color)
		{
			var node = base.Reconstruct(new(value), color) as Node
				?? throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
				" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
			node._valuesSum = new(value);
			return node;
		}

		internal override Node ShallowClone() => new(Value, Color);

		internal override void Update(MpzT value)
		{
			ValuesSum += value - Value;
			Value = new(value);
		}

		internal override void UpdateValuesSum(BaseSumList<MpzT, ListOfBigSums>.Node? newNode,
			BaseSumList<MpzT, ListOfBigSums>.Node? oldNode) =>
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

	internal sealed class TreeSubSet : ListOfBigSums
	{
		private readonly ListOfBigSums _underlying;
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
		// used to see if the length is out of date

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

		public TreeSubSet(ListOfBigSums Underlying, int Min, int Max, bool lowerBoundActive, bool upperBoundActive) : base()
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

		internal override bool BreadthFirstTreeWalk(BaseSumWalkPredicate<MpzT, ListOfBigSums> action)
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
			Changed();
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

		protected override MpzT GetInternal(int index) => _underlying.GetInternal(_min + index);

		// This passes functionality down to the underlying tree, clipping edges if necessary
		// There's nothing gained by having a nested subset. May as well draw it from the base
		// Cannot increase the bounds of the subset, can only decrease it
		public override ListOfBigSums GetViewBetween(int lowerValue, int upperValue)
		{
			if (_lBoundActive && Comparer.Compare(_min, lowerValue) > 0)
				throw new ArgumentOutOfRangeException(nameof(lowerValue));
			if (_uBoundActive && Comparer.Compare(_max, upperValue) < 0)
				throw new ArgumentOutOfRangeException(nameof(upperValue));
			return (TreeSubSet)_underlying.GetViewBetween(lowerValue, upperValue);
		}

		internal override bool InOrderTreeWalk(BaseSumWalkPredicate<MpzT, ListOfBigSums> action) =>
			InOrderTreeWalk(this, action);

		public override ListOfBigSums Insert(int index, MpzT value)
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

		protected override void SetInternal(int index, MpzT value) => _underlying.SetInternal(_min + index, value);

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
