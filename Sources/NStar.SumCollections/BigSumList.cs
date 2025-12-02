namespace NStar.SumCollections;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigSumList : BaseSumList<MpzT, BigSumList>
{
	public BigSumList() { }

	public BigSumList(G.IEnumerable<MpzT> collection) : this()
	{
		ArgumentNullException.ThrowIfNull(collection);
		// These are explicit type checks in the mold of HashSet. It would have worked better with
		// something like an ISorted interface. (We could make this work for SortedList.Keys, etc.)
		if (collection is BigSumList sumList && sumList is not TreeSubSet)
		{
			if (sumList.Length > 0)
			{
				Debug.Assert(sumList.root != null);
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

	public BigSumList(params MpzT[] array) : this((G.IEnumerable<MpzT>)array) { }

	public BigSumList(ReadOnlySpan<MpzT> span) : this((G.IEnumerable<MpzT>)span.ToArray()) { }

	public override MpzT this[Index index, bool invoke = true]
	{
		get => base[index, invoke];
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegative(value);
			base[index, invoke] = value;
		}
	}

	protected override Func<int, BigSumList> CapacityCreator => x => [];

	protected override Func<G.IEnumerable<MpzT>, BigSumList> CollectionCreator { get; } = x => new(x);

	protected override Func<ReadOnlySpan<MpzT>, BigSumList> SpanCreator { get; } = x => new(x);

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
		Changed();
	}

	protected override void CopyToInternal(int sourceIndex, BigSumList destination, int destinationIndex, int length)
	{
		if (length == 0)
			return;
		if (length == 1)
		{
			destination.SetOrAdd(destinationIndex, GetInternal(sourceIndex));
			return;
		}
		using TreeSubSet subset = new(this, sourceIndex, sourceIndex + length - 1, true, true);
		using BigSumList list = new(subset);
		using var en = list.GetEnumerator();
		if (destinationIndex < destination._size)
		{
			using var destSubSet = new TreeSubSet(destination, destinationIndex, Min(destinationIndex + length, destination._size) - 1, true, true);
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

	public override void Dispose()
	{
		root = null;
		_size = 0;
		version = 0;
		GC.SuppressFinalize(this);
	}

	public override G.IEnumerator<MpzT> GetEnumerator() => new Enumerator(this);

	protected override MpzT GetInternal(int index, bool invoke = true)
	{
		var current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
			{
				MpzT value = new(current.Value);
				if (invoke)
					Changed();
				return value;
			}
			else if (current.Left == null)
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
		MpzT sum = 0;
		while (current != null)
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

	public virtual BigSumList GetViewBetween(int lowerValue, int upperValue)
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
		sumExceedsBy = MpzT.Zero;
		var index = 0;
		while (current != null)
		{
			var left = current.Left;
			var leftCount = left?.LeavesCount ?? 0;
			var leftSum = left?.ValuesSum ?? 0;
			if (sum < leftSum)
			{
				Mpir.NET.Mpir.MpzSet(sumExceedsBy, current.Value);
				current = left;
			}
			else if (sum < leftSum + current.Value)
			{
				Mpir.NET.Mpir.MpzSub(sumExceedsBy, sum, leftSum);
				return index + leftCount;
			}
			else
			{
				index += leftCount + 1;
				Mpir.NET.Mpir.MpzSub(sum, sum, leftSum + current.Value);
				Mpir.NET.Mpir.MpzSet(sumExceedsBy, current.Value);
				current = current.Right;
			}
		}
		Mpir.NET.Mpir.MpzAdd(sumExceedsBy, sumExceedsBy, sum);
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
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
			{
				current.Update(value);
				Changed();
				return;
			}
			else if (current.Left == null)
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
	internal new sealed class Node : BaseSumList<MpzT, BigSumList>.Node
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
				if (Parent != null)
					Parent.ValuesSum += value - _valuesSum;
				_valuesSum = value;
				if (Parent == null || Parent.ValuesSum == (Parent.Left?.ValuesSum ?? 0) + (Parent.Right?.ValuesSum ?? 0) + Parent.Value)
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
			Debug.Assert(pendingNodes != null);
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
			Debug.Assert(node != null);
			Debug.Assert(node == Left ^ node == Right);
			return node == Left ? Right! : Left!;
		}

		internal static bool IsNonNullBlack(Node? node) => node != null && node.IsBlack;

		internal static bool IsNonNullRed(Node? node) => node != null && node.IsRed;

		internal static bool IsNullOrBlack(Node? node) => node == null || node.IsBlack;

		private protected override BaseSumList<MpzT, BigSumList>.Node Reconstruct(MpzT value, NodeColor color)
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

		internal override void UpdateValuesSum(BaseSumList<MpzT, BigSumList>.Node? newNode, BaseSumList<MpzT, BigSumList>.Node? oldNode) => ValuesSum += ((newNode as Node)?.ValuesSum ?? 0) - ((oldNode as Node)?.ValuesSum ?? 0);

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

	public new struct Enumerator : G.IEnumerator<MpzT>
	{
		private readonly BigSumList _list;
		private readonly int _version;

		private readonly Stack<Node> _stack;
		private Node? _current;

		private readonly bool _reverse;

		internal Enumerator(BigSumList list) : this(list, reverse: false)
		{
		}

		internal Enumerator(BigSumList list, bool reverse)
		{
			_list = list;
			list.VersionCheck();
			_version = list.version;
			// 2 log(n + 1) is the maximum height.
			_stack = (Stack<Node>?)typeof(Stack<Node>).GetMethod("GetNew", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, [2 * Log2(list.TotalCount() + 1)])!;
			Debug.Assert(_stack != null);
			_current = null;
			_reverse = reverse;
			Initialize();
		}

		public readonly MpzT Current
		{
			get
			{
				if (_current != null)
					return _current.Value;
				return default!; // Should only happen when accessing Current is undefined behavior
			}
		}

		readonly object? IEnumerator.Current
		{
			get
			{
				if (_current == null)
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				return _current.Value;
			}
		}

		internal readonly bool NotStartedOrEnded => _current == null;

		public readonly void Dispose() => _stack?.Dispose();

		private void Initialize()
		{
			_current = null;
			var node = _list.root;
			Node? next, other;
			while (node != null)
			{
				next = (_reverse ? node.Right : node.Left) as Node;
				other = (_reverse ? node.Left : node.Right) as Node;
				if (_list.IsWithinRange(node.Left?.LeavesCount ?? 0))
				{
					_stack.Push(node as Node
						?? throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
						" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar."));
					node = next;
				}
				else if (next == null || !_list.IsWithinRange(next.Left?.LeavesCount ?? 0))
					node = other;
				else
					node = next;
			}
		}

		public bool MoveNext()
		{
			// Make sure that the underlying subset has not been changed since
			_list.VersionCheck();
			if (_version != _list.version)
				throw new InvalidOperationException("Коллекцию нельзя изменять во время перечисления по ней.");
			if (_stack.Length == 0)
			{
				_current = null;
				return false;
			}
			_current = _stack.Pop();
			var node = _reverse ? _current.Left : _current.Right;
			Node? next, other;
			while (node != null)
			{
				next = _reverse ? node.Right : node.Left;
				other = _reverse ? node.Left : node.Right;
				if (_list.IsWithinRange(node.Left?.LeavesCount ?? 0))
				{
					_stack.Push(node);
					node = next;
				}
				else if (other == null || !_list.IsWithinRange(other.Left?.LeavesCount ?? 0))
					node = next;
				else
					node = other;
			}
			return true;
		}

		internal void Reset()
		{
			if (_version != _list.version)
				throw new InvalidOperationException("Коллекцию нельзя изменять во время перечисления по ней.");
			_stack.Clear();
			Initialize();
		}

		void IEnumerator.Reset() => Reset();
	}

	internal sealed class TreeSubSet : BigSumList
	{
		private readonly BigSumList _underlying;
		private readonly int _min;
		private readonly int _max;
		// keeps track of whether the length variable is up to date
		// up to date -> _countVersion = _underlying.version
		// not up to date -> _countVersion < _underlying.version
		private int _countVersion;
		// these exist for unbounded collections
		// for instance, you could allow this subset to be defined for i > 10. The list will throw if
		// anything <= 10 is added, but there is no upper bound. These features Head(), Tail(), were punted
		// in the spec, and are not available, but the framework is there to make them available at some point.
		private readonly bool _lBoundActive, _uBoundActive;
		// used to see if the length is out of date

		public TreeSubSet(BigSumList Underlying, int Min, int Max, bool lowerBoundActive, bool upperBoundActive) : base()
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

		internal override int MaxInternal
		{
			get
			{
				VersionCheck();
				var current = root;
				int result = default;
				while (current != null)
				{
					var comp = _uBoundActive ? Comparer.Compare(_max, current.Left?.LeavesCount ?? 0) : 1;
					if (comp < 0)
						current = current.Left;
					else
					{
						result = current.Left?.LeavesCount ?? 0;
						if (comp == 0)
							break;
						current = current.Right;
					}
				}
				return result!;
			}
		}

		internal override int MinInternal
		{
			get
			{
				VersionCheck();
				var current = root;
				int result = default;
				while (current != null)
				{
					var comp = _lBoundActive ? Comparer.Compare(_min, current.Left?.LeavesCount ?? 0) : -1;
					if (comp > 0)
						current = current.Right;
					else
					{
						result = current.Left?.LeavesCount ?? 0;
						if (comp == 0)
							break;
						current = current.Left;
					}
				}
				return result!;
			}
		}

		internal override bool BreadthFirstTreeWalk(BaseSumWalkPredicate<MpzT, BigSumList> action)
		{
			VersionCheck();
			if (root == null)
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
				if (current.Left != null && (!_lBoundActive || Comparer.Compare(_min, current.Left.LeavesCount) < 0))
					processQueue.Enqueue(current.Left);
				if (current.Right != null && (!_uBoundActive || Comparer.Compare(_max, current.Left?.LeavesCount ?? 0) > 0))
					processQueue.Enqueue(current.Right);
			}
			return true;
		}

		public override void Clear(bool _)
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

		protected override MpzT GetInternal(int index, bool invoke = true) => _underlying.GetInternal(_min + index, invoke);

		// This passes functionality down to the underlying tree, clipping edges if necessary
		// There's nothing gained by having a nested subset. May as well draw it from the base
		// Cannot increase the bounds of the subset, can only decrease it
		public override BigSumList GetViewBetween(int lowerValue, int upperValue)
		{
			if (_lBoundActive && Comparer.Compare(_min, lowerValue) > 0)
				throw new ArgumentOutOfRangeException(nameof(lowerValue));
			if (_uBoundActive && Comparer.Compare(_max, upperValue) < 0)
				throw new ArgumentOutOfRangeException(nameof(upperValue));
			return (TreeSubSet)_underlying.GetViewBetween(lowerValue, upperValue);
		}

		internal override bool InOrderTreeWalk(BaseSumWalkPredicate<MpzT, BigSumList> action)
		{
			VersionCheck();
			if (root == null)
				return true;
			// The maximum height of a red-black tree is 2*lg(n+1).
			// See page 264 of "Introduction to algorithms" by Thomas H. Cormen
			using var stack = (Stack<Node>?)typeof(Stack<Node>).GetMethod("GetNew", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, [2 * Log2(_size + 1)]);
			Debug.Assert(stack != null);
			var current = root;
			using var indexStack = (Stack<int>?)typeof(Stack<int>).GetMethod("GetNew", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, [2 * Log2(_size + 1)]);
			Debug.Assert(indexStack != null);
			var index = current.Left?.LeavesCount ?? 0;
			using var flagStack = (Stack<bool>?)typeof(Stack<bool>).GetMethod("GetNew", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, [2 * Log2(_size + 1)]);
			Debug.Assert(flagStack != null);
			while (current != null)
			{
				if (IsWithinRange(index))
				{
					stack.Push(current as Node
						?? throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
						" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar."));
					indexStack.Push(index);
					flagStack.Push(true);
					current = current.Left;
					index -= (current?.Right?.LeavesCount ?? 0) + 1;
				}
				else if (_lBoundActive && Comparer.Compare(index, _max) < 0)
				{
					current = current.Right;
					index += (current?.Left?.LeavesCount ?? 0) + 1;
				}
				else
				{
					stack.Push(current as Node
						?? throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
						" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar."));
					indexStack.Push(index);
					flagStack.Push(false);
					current = current.Left;
					index -= (current?.Right?.LeavesCount ?? 0) + 1;
				}
			}
			while (stack.Length != 0)
			{
				current = stack.Pop();
				if (flagStack.Pop() && !action(current))
					return false;
				var node = current.Right;
				index = indexStack.Pop() + (node?.Left?.LeavesCount ?? 0) + 1;
				while (node != null)
				{
					if (IsWithinRange(index))
					{
						stack.Push(node as Node
							?? throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
							" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar."));
						indexStack.Push(index);
						flagStack.Push(true);
						node = node.Left;
						index -= (node?.Right?.LeavesCount ?? 0) + 1;
					}
					else if (_lBoundActive && Comparer.Compare(index, _max) < 0)
					{
						node = node.Right;
						index += (node?.Left?.LeavesCount ?? 0) + 1;
					}
					else
					{
						stack.Push(node as Node
							?? throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
							" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar."));
						indexStack.Push((node.Left?.LeavesCount ?? 0) + index);
						flagStack.Push(false);
						node = node.Left;
						index -= (node?.Right?.LeavesCount ?? 0) + 1;
					}
				}
			}
			return true;
		}

		public override BigSumList Insert(int index, MpzT value)
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
			Debug.Assert(_underlying != null);
			return _underlying.Length;
		}

		/// <summary>
		/// Checks whether this subset is out of date, and updates it if necessary.
		/// </summary>
		/// <param name="updateCount">Updates the length variable if necessary.</param>
		internal override void VersionCheck(bool updateCount = false) => VersionCheckImpl(updateCount);

		private void VersionCheckImpl(bool updateCount)
		{
			Debug.Assert(_underlying != null);
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
				Changed();
			}
		}

#if DEBUG
		internal override bool VersionUpToDate() => version == _underlying.version;
#endif
	}
}

internal delegate bool BigSumWalkPredicate(BigSumList.Node node);
