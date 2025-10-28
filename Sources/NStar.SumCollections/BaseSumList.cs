using System.Numerics;

namespace NStar.SumCollections;

internal delegate bool BaseSumWalkPredicate<T, TCertain>(BaseSumList<T, TCertain>.Node node) where T : INumber<T> where TCertain : BaseSumList<T, TCertain>, new();

public abstract class BaseSumList<T, TCertain> : BaseList<T, TCertain> where T : INumber<T> where TCertain : BaseSumList<T, TCertain>, new()
{
	private protected Node? root;
	private protected int version;
	private protected static readonly Queue<Node> nodePool = new(256);
	private protected static readonly object globalLockObj = new();

	public override int Capacity
	{
		get => _size;
		set
		{
		}
	}

	protected virtual G.IComparer<int> Comparer => G.Comparer<int>.Default;

	private protected virtual Func<T, NodeColor, Node> NodeCreator => Node.GetNew;

	public override int Length
	{
		get
		{
			VersionCheck(updateCount: true);
			return _size;
		}
	}

	public virtual int Max => MaxInternal;

	internal virtual int MaxInternal => _size - 1;

	public virtual int Min => MinInternal;

	internal virtual int MinInternal => 0;

	public override TCertain Add(T value) => Insert(_size, value);

	public override Span<T> AsSpan(int index, int length) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
		+ " Используйте GetSlice() вместо него.");

	/// <summary>
	/// Does a left-to-right breadth-first tree walk and calls the delegate for each node.
	/// </summary>
	/// <param name="action">
	/// The delegate to invoke on each node.
	/// If the delegate returns <c>false</c>, the walk is stopped.
	/// </param>
	/// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
	internal virtual bool BreadthFirstTreeWalk(BaseSumWalkPredicate<T, TCertain> action)
	{
		if (root == null)
			return true;
		using Queue<Node> processQueue = [];
		processQueue.Enqueue(root);
		Node current;
		while (processQueue.Length != 0)
		{
			current = processQueue.Dequeue();
			if (!action(current))
				return false;
			if (current.Left != null)
				processQueue.Enqueue(current.Left);
			if (current.Right != null)
				processQueue.Enqueue(current.Right);
		}
		return true;
	}

	public override void Clear(bool _)
	{
		root?.Dispose();
		root = null;
		_size = 0;
		++version;
		Changed();
	}

	protected virtual Node? ConstructRootFromSortedArray(T[] arr, int startIndex, int endIndex, Node? redNode)
	{
		// You're given a sorted array... say 1 2 3 4 5 6
		// There are 2 cases:
		// -  If there are odd # of elements, pick the middle element (in this case 4), and compute
		//	its left and right branches
		// -  If there are even # of elements, pick the left middle element, save the right middle element
		//	and call the function on the rest
		//	1 2 3 4 5 6 -> pick 3, save 4 and call the fn on 1,2 and 5,6
		//	now add 4 as a red node to the lowest element on the right branch
		//			   3					   3
		//		   1	   5	   ->	  1		   5
		//			 2		 6			   2	 4   6
		//	As we're adding to the leftmost of the right branch, nesting will not hurt the red-black properties
		//	Leaf nodes are red if they have no sibling (if there are 2 nodes or if a node trickles
		//	down to the bottom

		// This is done recursively because the iterative way to do this ends up wasting more space than it saves in stack frames
		// Only some base cases are handled below.
		var size = endIndex - startIndex + 1;
		Node root;
		switch (size)
		{
			case 0:
			return null;
			case 1:
			root = NodeCreator(arr[startIndex], NodeColor.Black);
			if (redNode != null)
				root.Left = redNode;
			break;
			case 2:
			root = NodeCreator(arr[startIndex], NodeColor.Black);
			root.Right = NodeCreator(arr[endIndex], NodeColor.Black);
			root.Right.ColorRed();
			if (redNode != null)
				root.Left = redNode;
			break;
			case 3:
			root = NodeCreator(arr[startIndex + 1], NodeColor.Black);
			root.Left = NodeCreator(arr[startIndex], NodeColor.Black);
			root.Right = NodeCreator(arr[endIndex], NodeColor.Black);
			if (redNode != null)
				root.Left.Left = redNode;
			break;
			default:
			var midpt = (startIndex + endIndex) / 2;
			root = NodeCreator(arr[midpt], NodeColor.Black);
			root.Left = ConstructRootFromSortedArray(arr, startIndex, midpt - 1, redNode);
			root.Right = size % 2 == 0 ?
				ConstructRootFromSortedArray(arr, midpt + 2, endIndex, NodeCreator(arr[midpt + 1], NodeColor.Red)) :
				ConstructRootFromSortedArray(arr, midpt + 1, endIndex, null);
			break;
		}
		return root;
	}

	protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is T[] array2)
			CopyToInternal(0, array2, arrayIndex, _size);
		else
			throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
	}

	public virtual bool Decrease(int index) => Update(index, (dynamic?)GetInternal(index) - 1);

	public override void Dispose()
	{
		root = null;
		_size = 0;
		version = 0;
		GC.SuppressFinalize(this);
	}

	protected virtual void FindForRemove(int index, out Node? parent, out Node? grandParent, out Node? match, out Node? parentOfMatch)
	{
		// Search for a node and then find its successor.
		// Then copy the value from the successor to the matching node, and delete the successor.
		// If a node doesn't have a successor, we can replace it with its left child (if not empty),
		// or delete the matching node.
		//
		// In top-down implementation, it is important to make sure the node to be deleted is not a 2-node.
		// Following code will make sure the node on the path is not a 2-node.

		// Even if we don't actually remove from the list, we may be altering its structure (by doing rotations
		// and such). So update our version to disable any enumerators/subsets working on it.
		version++;
		var current = root;
		parent = null;
		grandParent = null;
		match = null;
		parentOfMatch = null;
		var foundMatch = false;
		while (current != null)
		{
			if (current.Is2Node)
			{
				// Fix up 2-node
				if (parent == null)
					current.ColorRed();
				else if (parent.Left != null && parent.Right != null)
				{
					var sibling = parent.GetSibling(current);
					Debug.Assert(sibling != null, "parent must have two children");
					if (sibling.IsRed)
					{
						// If parent is a 3-node, flip the orientation of the red link.
						// We can achieve this by a single rotation.
						// This case is converted to one of the other cases below.
						Debug.Assert(parent.IsBlack);
						if (parent.Right == sibling)
							parent.RotateLeft();
						else
							parent.RotateRight();
						parent.ColorRed();
						sibling.ColorBlack(); // The red parent can't have black children.
											  // `sibling` becomes the child of `grandParent` or `root` after rotation. Update the link from that node.
						ReplaceChildOrRoot(grandParent, parent, sibling);
						// `sibling` will become the grandparent of `current`.
						grandParent = sibling;
						if (parent == match)
							parentOfMatch = sibling;
						sibling = parent.GetSibling(current);
					}
					Debug.Assert(Node.IsNonNullBlack(sibling));
					if (sibling.Is2Node)
						parent.Merge2Nodes();
					else
					{
						// `current` is a 2-node and `sibling` is either a 3-node or a 4-node.
						// We can change the color of `current` to red by some rotation.
						var newGrandParent = parent.Rotate(parent.GetRotation(current, sibling))!;
						newGrandParent.Color = parent.Color;
						parent.ColorBlack();
						current.ColorRed();
						ReplaceChildOrRoot(grandParent, parent, newGrandParent);
						if (parent == match)
							parentOfMatch = newGrandParent;
					}
				}
			}
			grandParent = parent;
			parent = current;
			if (foundMatch)
				current = current.Left;
			else if ((current.Left?.LeavesCount ?? 0) == index)
			{
				// Save the matching node.
				foundMatch = true;
				match = current;
				parentOfMatch = grandParent;
				current = current.Right;
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
	}

	internal virtual Node? FindNode(int index)
	{
		var current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
				return current;
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
		return null;
	}

	internal virtual Node? FindRange(int from, int to) => FindRange(from, to, true, true);

	internal virtual Node? FindRange(int from, int to, bool lowerBoundActive, bool upperBoundActive)
	{
		var current = root;
		while (current != null)
		{
			if (lowerBoundActive && Comparer.Compare(from, current.Left?.LeavesCount ?? 0) > 0)
			{
				from -= (current.Left?.LeavesCount ?? 0) + 1;
				to -= (current.Left?.LeavesCount ?? 0) + 1;
				current = current.Right;
			}
			else if (upperBoundActive && Comparer.Compare(to, current.Left?.LeavesCount ?? 0) < 0)
				current = current.Left;
			else
				return current;
		}
		return null;
	}

	public override T GetAndRemove(Index index)
	{
		var index2 = index.GetOffset(_size);
		if (root == null)
			return default!;
		FindForRemove(index2, out var parent, out var grandParent, out var match, out var parentOfMatch);
		T found = default!;
		// Move successor to the matching node position and replace links.
		if (match != null)
		{
			found = match.Value;
			ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
			--_size;
			Changed();
		}
		root?.ColorBlack();
#if VERIFY
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
				" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
		foreach (var x in new[] { parentOfMatch, parent, grandParent })
			x?.Verify();
#endif
		return found;
	}

	public virtual bool Increase(int index) => Update(index, (dynamic?)GetInternal(index) + 1);

	protected override int IndexOfInternal(T value, int index, int length) =>
		throw new NotSupportedException("Этот метод временно не поддерживается в этой коллекции."
			+ " Если он нужен вам прямо сейчас, используйте другой вид списка, например, NList<int> или NList<MpzT>.");

	/// <summary>
	/// Does an in-order tree walk and calls the delegate for each node.
	/// </summary>
	/// <param name="action">
	/// The delegate to invoke on each node.
	/// If the delegate returns <c>false</c>, the walk is stopped.
	/// </param>
	/// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
	internal virtual bool InOrderTreeWalk(BaseSumWalkPredicate<T, TCertain> action)
	{
		if (root == null)
			return true;
		// The maximum height of a red-black tree is 2 * log2(n+1).
		// See page 264 of "Introduction to algorithms" by Thomas H. Cormen
		// Note: It's not strictly necessary to provide the stack capacity, but we don't
		// want the stack to unnecessarily allocate arrays as it grows.
		using var stack = (Stack<Node>?)typeof(Stack<Node>).GetMethod("GetNew", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, [2 * Log2(Length + 1)]);
		Debug.Assert(stack != null);
		var current = root;
		while (current != null)
		{
			stack.Push(current);
			current = current.Left;
		}
		while (stack.Length != 0)
		{
			current = stack.Pop();
			if (!action(current))
				return false;
			var node = current.Right;
			while (node != null)
			{
				stack.Push(node);
				node = node.Left;
			}
		}
		return true;
	}

	public override TCertain Insert(int index, T value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(value);
		var this2 = (TCertain)this;
		if (value == T.Zero)
			return this2;
		if (root == null)
		{
			// The tree is empty and this is the first value.
			root = NodeCreator(value, NodeColor.Black);
			_size = 1;
			version++;
			Changed();
			return this2;
		}
		// Search for a node at bottom to insert the new node.
		// If we can guarantee the node we found is not a 4-node, it would be easy to do insertion.
		// We split 4-nodes along the search path.
		var current = root;
		Node? parent = null;
		Node? grandParent = null;
		Node? greatGrandParent = null;
		// Even if we don't actually add to the list, we may be altering its structure (by doing rotations and such).
		// So update `_version` to disable any instances of Enumerator/TreeSubSet from working on it.
		version++;
		var oldIndex = index;
		var order = 0;
		var foundMatch = false;
		while (current != null)
		{
			order = foundMatch ? 1 : Comparer.Compare(index, current.Left?.LeavesCount ?? 0);
			if (order == 0)
			{
				// We could have changed root node to red during the search process.
				// We need to set it to black before we return.
				root.ColorBlack();
				foundMatch = true;
			}
			// Split a 4-node into two 2-nodes.
			if (current.Is4Node)
			{
				current.Split4Node();
				// We could have introduced two consecutive red nodes after split. Fix that by rotation.
				if (Node.IsNonNullRed(parent))
					InsertionBalance(current, ref parent!, grandParent!, greatGrandParent!);
				index = oldIndex;
				current = root;
				greatGrandParent = grandParent = parent = null;
				foundMatch = false;
				continue;
			}
			greatGrandParent = grandParent;
			grandParent = parent;
			parent = current;
			if (order <= 0)
				current = current.Left;
			else
			{
				index -= (current.Left?.LeavesCount ?? 0) + 1;
				current = current.Right;
			}
		}
#if VERIFY
		if (index != 0)
			throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
				" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
#endif
		Debug.Assert(parent != null);
		// We're ready to insert the new node.
		var node = NodeCreator(value, NodeColor.Red);
		if (order <= 0)
			parent.Left = node;
		else
			parent.Right = node;
		// The new node will be red, so we will need to adjust colors if its parent is also red.
		if (parent.IsRed)
			InsertionBalance(node, ref parent!, grandParent!, greatGrandParent!);
#if VERIFY
		if (_size + 1 != root.LeavesCount)
			throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
				" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
		foreach (var x in new[] { node, parent, grandParent, greatGrandParent })
			x?.Verify();
#endif
		// The root node is always black.
		root.ColorBlack();
		++_size;
		Changed();
		return this2;
	}

	protected override TCertain InsertInternal(int index, G.IEnumerable<T> collection)
	{
		foreach (var item in collection)
			Insert(index++, item);
		return (TCertain)this;
	}

	protected override TCertain InsertInternal(int index, ReadOnlySpan<T> span)
	{
		for (var i = 0; i < span.Length; i++)
			Insert(index++, span[i]);
		return (TCertain)this;
	}

	// After calling InsertionBalance, we need to make sure `current` and `parent` are up-to-date.
	// It doesn't matter if we keep `grandParent` and `greatGrandParent` up-to-date, because we won't
	// need to split again in the next node.
	// By the time we need to split again, everything will be correctly set.
	private protected void InsertionBalance(Node current, ref Node parent, Node grandParent, Node greatGrandParent)
	{
		Debug.Assert(parent != null);
		Debug.Assert(grandParent != null);
		var parentIsOnRight = grandParent.Right == parent;
		var currentIsOnRight = parent.Right == current;
		Node newChildOfGreatGrandParent;
		if (parentIsOnRight == currentIsOnRight)
			newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeft() : grandParent.RotateRight();
		else
		{
			// Different orientation, double rotation
			newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeftRight() : grandParent.RotateRightLeft();
			// Current node now becomes the child of `greatGrandParent`
			parent = greatGrandParent;
		}
		// `grandParent` will become a child of either `parent` of `current`.
		grandParent.ColorRed();
		newChildOfGreatGrandParent.ColorBlack();
		ReplaceChildOrRoot(greatGrandParent, grandParent, newChildOfGreatGrandParent);
#if VERIFY
		foreach (var x in new[] { current, parent, grandParent, greatGrandParent })
			x?.Verify();
#endif
	}

	// Virtual function for TreeSubSet, which may need to do range checks.
	internal virtual bool IsWithinRange(int index) => true;

	protected override int LastIndexOfInternal(T value, int index, int length) =>
		throw new NotSupportedException("Этот метод временно не поддерживается в этой коллекции."
			+ " Если он нужен вам прямо сейчас, используйте другой вид списка, например, NList<int> или NList<MpzT>.");

	// Used for set checking operations (using enumerables) that rely on counting
	private protected static int Log2(int value) => BitOperations.Log2((uint)value);

	public override TCertain RemoveAt(int index)
	{
		var this2 = (TCertain)this;
		if (root == null)
			return this2;
		FindForRemove(index, out var parent, out var grandParent, out var match, out var parentOfMatch);
		// Move successor to the matching node position and replace links.
		if (match != null)
		{
			ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
			--_size;
			Changed();
		}
		root?.ColorBlack();
#if VERIFY
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
				" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
		foreach (var x in new[] { parentOfMatch, parent, grandParent })
			x?.Verify();
#endif
		return this2;
	}

	/// <summary>
	/// Replaces the child of a parent node, or replaces the root if the parent is <c>null</c>.
	/// </summary>
	/// <param name="parent">The (possibly <c>null</c>) parent.</param>
	/// <param name="child">The child node to replace.</param>
	/// <param name="newChild">The node to replace <paramref name="child"/> with.</param>
	private protected void ReplaceChildOrRoot(Node? parent, Node child, Node newChild)
	{
		if (parent != null)
			parent.ReplaceChild(child, newChild);
		else
		{
			root = newChild;
			root?.Isolate();
		}
	}

	/// <summary>
	/// Replaces the matching node with its successor.
	/// </summary>
	private protected void ReplaceNode(Node match, Node parentOfMatch, Node successor, Node parentOfSuccessor)
	{
		Debug.Assert(match != null);
		if (successor == match)
		{
			// This node has no successor. This can only happen if the right child of the match == null.
			Debug.Assert(match.Right == null);
			successor = match.Left!;
		}
		else
		{
			Debug.Assert(parentOfSuccessor != null);
			Debug.Assert(successor.Left == null);
			Debug.Assert(successor.Right == null ? successor.IsRed : successor.Right.IsRed && successor.IsBlack);
			successor.Right?.ColorBlack();
			if (parentOfSuccessor != match)
			{
				// Detach the successor from its parent and set its right child.
				parentOfSuccessor.Left = successor.Right;
				successor.Right = match.Right;
				parentOfSuccessor.FixUp();
			}
			successor.Left = match.Left;
		}
		if (successor != null)
			successor.Color = match.Color;
		ReplaceChildOrRoot(parentOfMatch, match, successor!);
#if VERIFY
		foreach (var x in new[] { parentOfMatch, successor, parentOfSuccessor })
			x?.Verify();
#endif
	}

	protected override void ReverseInternal(int index, int length)
	{
		using List<Node> nodes = [];
		var i = 0;
		using var stack = (Stack<Node>?)typeof(Stack<Node>).GetMethod("GetNew", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, [2 * Log2(Length + 1)]);
		Debug.Assert(stack != null);
		var current = root;
		while (current != null)
		{
			stack.Push(current);
			current = current.Left;
		}
		while (stack.Length != 0)
		{
			current = stack.Pop();
			if (i++ >= index)
				nodes.Add(current);
			if (i >= index + length)
				break;
			var node = current.Right;
			while (node != null)
			{
				stack.Push(node);
				node = node.Left;
			}
		}
		if (nodes.Length == 0)
			return;
		var values = nodes.ToArray(x => x.Value);
		Array.Reverse(values);
		for (i = 0; i < nodes.Length; i++)
			nodes[i].Update(values[i]);
	}

	// Virtual function for TreeSubSet, which may need the length variable of the parent list.
	internal virtual int TotalCount() => Length;

	public virtual bool Update(int index, T value)
	{
		if ((dynamic?)value <= 0)
		{
			RemoveAt(index);
			return true;
		}
		var node = FindNode(index);
		if (node != null)
		{
			node.Update(value);
#if VERIFY
			foreach (var x in new[] { node, root })
				x?.Verify();
#endif
			return true;
		}
		else
			return false;
	}

	public virtual T UpdateIfGreater(int index, T value)
	{
		var node = FindNode(index);
		if (node != null)
		{
			if ((dynamic?)value <= 0)
				return node.Value;
			if (value > node.Value)
				node.Update(value);
#if VERIFY
			foreach (var x in new[] { node, root })
				x?.Verify();
#endif
			return node.Value;
		}
		else
			return T.Zero;
	}

	internal virtual void UpdateVersion() => ++version;

	// Virtual function for TreeSubSet, which may need to update its length.
	internal virtual void VersionCheck(bool updateCount = false) { }

#if DEBUG
	/// <summary>
	/// debug status to be checked whenever any operation is called
	/// </summary>
	/// <returns></returns>
	internal virtual bool VersionUpToDate() => true;
#endif

	protected internal abstract class Node : IDisposable
	{
		private protected Node? _left;
		private protected Node? _right;
		protected virtual Node? Parent { get; set; }

		internal virtual T Value { get; private protected set; }
		private protected int _leavesCount;

		private protected Node(T value, NodeColor color)
		{
			Value = value;
			Color = color;
			_leavesCount = 1;
		}

		internal virtual Node? Left
		{
			get => _left;
			set
			{
				if (_left == value)
					return;
				if (_left != null && _left.Parent != value)
					_left.Parent = null;
				LeavesCount += (value?.LeavesCount ?? 0) - (_left?.LeavesCount ?? 0);
				UpdateValuesSum(value, _left);
				_left = value;
				if (_left != null)
					_left.Parent = this;
#if VERIFY
				foreach (var x in new[] { this, _left, _right, Parent })
					x?.Verify();
#endif
			}
		}

		internal virtual Node? Right
		{
			get => _right;
			set
			{
				if (_right == value)
					return;
				if (_right != null && _right.Parent != value)
					_right.Parent = null;
				LeavesCount += (value?.LeavesCount ?? 0) - (_right?.LeavesCount ?? 0);
				UpdateValuesSum(value, _right);
				_right = value;
				if (_right != null)
					_right.Parent = this;
#if VERIFY
				foreach (var x in new[] { this, _left, _right, Parent })
					x?.Verify();
#endif
			}
		}

		internal int LeavesCount
		{
			get => _leavesCount;
			set
			{
				if (Parent != null)
					Parent.LeavesCount += value - _leavesCount;
				_leavesCount = value;
				if (Parent == null || Parent.LeavesCount == (Parent._left?.LeavesCount ?? 0) + (Parent._right?.LeavesCount ?? 0) + 1)
					return;
				throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
			}
		}

		internal virtual NodeColor Color { get; set; }

		internal virtual bool IsBlack => Color == NodeColor.Black;

		internal virtual bool IsRed => Color == NodeColor.Red;

		internal virtual bool Is2Node => IsBlack && IsNullOrBlack(Left) && IsNullOrBlack(Right);

		internal virtual bool Is4Node => IsNonNullRed(Left) && IsNonNullRed(Right);

		internal virtual void ColorBlack() => Color = NodeColor.Black;

		internal virtual void ColorRed() => Color = NodeColor.Red;

		internal abstract Node DeepClone(int length);

		public void Dispose()
		{
			lock (globalLockObj)
				DisposeLocked();
			GC.SuppressFinalize(this);
		}

		private void DisposeLocked()
		{
			nodePool.Enqueue(this);
			Left?.DisposeLocked();
			Right?.DisposeLocked();
		}

		internal virtual void FixUp()
		{
			if (Left != null)
				Left.Parent = this;
			if (Right != null)
				Right.Parent = this;
		}

		internal static Node GetNew(T value, NodeColor color)
		{
			lock (globalLockObj)
				return nodePool.TryDequeue(out var node) ? node!.Reconstruct(value, color)
					: typeof(TCertain)?.GetNestedType("Node", BindingFlags.NonPublic)
					?.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
					?[0]?.Invoke([value, color]) as Node ?? throw new NullReferenceException();
		}

		/// <summary>
		/// Gets the rotation this node should undergo during a removal.
		/// </summary>
		internal virtual TreeRotation GetRotation(Node current, Node sibling)
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
		internal virtual Node GetSibling(Node node)
		{
			Debug.Assert(node != null);
			Debug.Assert(node == Left ^ node == Right);
			return node == Left ? Right! : Left!;
		}

		internal static bool IsNonNullBlack(Node? node) => node != null && node.IsBlack;

		internal static bool IsNonNullRed(Node? node) => node != null && node.IsRed;

		internal static bool IsNullOrBlack(Node? node) => node == null || node.IsBlack;

		internal virtual void Isolate()
		{
			if (Parent != null && Parent.Left == this)
				Parent.Left = null;
			if (Parent != null && Parent.Right == this)
				Parent.Right = null;
		}

		/// <summary>
		/// Combines two 2-nodes into a 4-node.
		/// </summary>
		internal virtual void Merge2Nodes()
		{
			Debug.Assert(IsRed);
			Debug.Assert(Left!.Is2Node);
			Debug.Assert(Right!.Is2Node);
			// Combine two 2-nodes into a 4-node.
			ColorBlack();
			Left.ColorRed();
			Right.ColorRed();
		}

		private protected virtual Node Reconstruct(T value, NodeColor color)
		{
			Value = value;
			Color = color;
			_left = null;
			_right = null;
			Parent = null;
			_leavesCount = 1;
			return this;
		}

		/// <summary>
		/// Replaces a child of this node with a new node.
		/// </summary>
		/// <param name="child">The child to replace.</param>
		/// <param name="newChild">The node to replace <paramref name="child"/> with.</param>
		internal virtual void ReplaceChild(Node child, Node newChild)
		{
			if (Left == child)
				Left = newChild;
			else if (Right == child)
				Right = newChild;
		}

		/// <summary>
		/// Does a rotation on this tree. May change the color of a grandchild from red to black.
		/// </summary>
		internal Node? Rotate(TreeRotation rotation)
		{
			Node removeRed;
			switch (rotation)
			{
				case TreeRotation.Right:
				removeRed = Left!.Left!;
				Debug.Assert(removeRed.IsRed);
				removeRed.ColorBlack();
				return RotateRight();
				case TreeRotation.Left:
				removeRed = Right!.Right!;
				Debug.Assert(removeRed.IsRed);
				removeRed.ColorBlack();
				return RotateLeft();
				case TreeRotation.RightLeft:
				Debug.Assert(Right!.Left!.IsRed);
				return RotateRightLeft();
				case TreeRotation.LeftRight:
				Debug.Assert(Left!.Right!.IsRed);
				return RotateLeftRight();
				default:
				Debug.Fail($"{nameof(rotation)}: {rotation} is not a defined {nameof(TreeRotation)} value.");
				return null;
			}
		}

		/// <summary>
		/// Does a left rotation on this tree, making this this the new left child of the current right child.
		/// </summary>
		internal virtual Node RotateLeft()
		{
			var child = Right!;
			var parent = Parent;
			var isRight = parent != null && (parent.Right == this || (parent.Left == this ? false
				: throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.")));
			Right = child.Left;
			child.Left = this;
			if (parent != null)
			{
				if (isRight)
					parent.Right = child;
				else
					parent.Left = child;
			}
#if VERIFY
			Verify();
			foreach (var x in new[] { parent, child })
				x?.Verify();
#endif
			return child;
		}

		/// <summary>
		/// Does a left-right rotation on this tree. The left child is rotated left, then this this is rotated right.
		/// </summary>
		internal virtual Node RotateLeftRight()
		{
			var child = Left!;
			var grandChild = child.Right!;
			var parent = Parent;
			var isRight = parent != null && (parent.Right == this || (parent.Left == this ? false
				: throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.")));
			Left = grandChild.Right;
			grandChild.Right = this;
			child.Right = grandChild.Left;
			grandChild.Left = child;
			if (parent != null)
			{
				if (isRight)
					parent.Right = grandChild;
				else
					parent.Left = grandChild;
			}
#if VERIFY
			Verify();
			foreach (var x in new[] { parent, child, grandChild })
				x?.Verify();
#endif
			return grandChild;
		}

		/// <summary>
		/// Does a right rotation on this tree, making this this the new right child of the current left child.
		/// </summary>
		internal virtual Node RotateRight()
		{
			var child = Left!;
			var parent = Parent;
			var isRight = parent != null && (parent.Right == this || (parent.Left == this ? false
				: throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.")));
			Left = child.Right;
			child.Right = this;
			if (parent != null)
			{
				if (isRight)
					parent.Right = child;
				else
					parent.Left = child;
			}
#if VERIFY
			Verify();
			foreach (var x in new[] { parent, child })
				x?.Verify();
#endif
			return child;
		}

		/// <summary>
		/// Does a right-left rotation on this tree. The right child is rotated right, then this this is rotated left.
		/// </summary>
		internal virtual Node RotateRightLeft()
		{
			var child = Right!;
			var grandChild = child.Left!;
			var parent = Parent;
			var isRight = parent != null && (parent.Right == this || (parent.Left == this ? false
				: throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.")));
			Right = grandChild.Left;
			grandChild.Left = this;
			child.Left = grandChild.Right;
			grandChild.Right = child;
			if (parent != null)
			{
				if (isRight)
					parent.Right = grandChild;
				else
					parent.Left = grandChild;
			}
#if VERIFY
			Verify();
			foreach (var x in new[] { parent, child, grandChild })
				x?.Verify();
#endif
			return grandChild;
		}

		internal abstract Node ShallowClone();

		internal virtual void Split4Node()
		{
			Debug.Assert(Left != null);
			Debug.Assert(Right != null);
			ColorRed();
			Left.ColorBlack();
			Right.ColorBlack();
		}

		internal abstract void Update(T value);

		internal abstract void UpdateValuesSum(Node? newNode, Node? oldNode);

#if VERIFY
		protected virtual int GetCount() => 1 + (Left?.GetCount() ?? 0) + (Right?.GetCount() ?? 0);

		protected virtual bool HasChild(Node child) => child == Left || child == Right;

		protected virtual bool HasChildren(Node child1, Node child2)
		{
			Debug.Assert(child1 != child2);
			return Left == child1 && Right == child2
				|| Left == child2 && Right == child1;
		}

		internal virtual void Verify()
		{
			if (Right != null && Right == Left)
				throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
			if (LeavesCount != (Left?.LeavesCount ?? 0) + (Right?.LeavesCount ?? 0) + 1)
				throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
			if (Left != null && Left.Parent == null)
				throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
			if (Right != null && Right.Parent == null)
				throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
		}
#endif
	}
}
