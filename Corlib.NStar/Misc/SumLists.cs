using System.Numerics;
using System.Reflection;

namespace Corlib.NStar;

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

	protected virtual IComparer<int> Comparer => G.Comparer<int>.Default;

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
	}

	protected virtual Node? ConstructRootFromSortedArray(T[] arr, int startIndex, int endIndex, Node? redNode)
	{
		// You're given a sorted array... say 1 2 3 4 5 6
		// There are 2 cases:
		// -  If there are odd # of elements, pick the middle element (in this case 4), and compute
		//    its left and right branches
		// -  If there are even # of elements, pick the left middle element, save the right middle element
		//    and call the function on the rest
		//    1 2 3 4 5 6 -> pick 3, save 4 and call the fn on 1,2 and 5,6
		//    now add 4 as a red node to the lowest element on the right branch
		//             3                       3
		//         1       5       ->     1        5
		//           2       6             2     4   6
		//    As we're adding to the leftmost of the right branch, nesting will not hurt the red-black properties
		//    Leaf nodes are red if they have no sibling (if there are 2 nodes or if a node trickles
		//    down to the bottom

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
		}
		root?.ColorBlack();
#if VERIFY
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException();
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
		using var stack = Stack<Node>.GetNew(2 * Log2(Length + 1));
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
			throw new InvalidOperationException();
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
			throw new InvalidOperationException();
		foreach (var x in new[] { node, parent, grandParent, greatGrandParent })
			x?.Verify();
#endif
		// The root node is always black.
		root.ColorBlack();
		++_size;
		return this2;
	}

	protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
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
		}
		root?.ColorBlack();
#if VERIFY
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException();
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
		using var stack = Stack<Node>.GetNew(2 * Log2(Length + 1));
		var current = root;
		while (current != null)
		{
			stack.Push(current);
			if (i++ >= index)
				nodes.Add(current);
			current = current.Left;
		}
		while (stack.Length != 0)
		{
			current = stack.Pop();
			if (i >= index + length)
				break;
			var node = current.Right;
			while (node != null)
			{
				stack.Push(node);
				if (i++ >= index)
					nodes.Add(node);
				node = node.Left;
			}
		}
		using var values = nodes.ToList(x => x.Value).Reverse();
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
				if (Parent != null && Parent.LeavesCount != (Parent._left?.LeavesCount ?? 0) + (Parent._right?.LeavesCount ?? 0) + 1)
					throw new InvalidOperationException();
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
			var isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
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
			var isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
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
			var isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
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
			var isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
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
				throw new InvalidOperationException();
			if (LeavesCount != (Left?.LeavesCount ?? 0) + (Right?.LeavesCount ?? 0) + 1)
				throw new InvalidOperationException();
			if (Left != null && Left.Parent == null)
				throw new InvalidOperationException();
			if (Right != null && Right.Parent == null)
				throw new InvalidOperationException();
		}
#endif
	}
}

internal delegate bool SumWalkPredicate(SumList.Node node);

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class SumList : BaseSumList<int, SumList>
{
	public SumList() { }

	public SumList(IEnumerable<int> collection) : this()
	{
		ArgumentNullException.ThrowIfNull(collection);
		// These are explicit type checks in the mold of HashSet. It would have worked better with
		// something like an ISorted interface. (We could make this work for SortedList.Keys, etc.)
		if (collection is SumList sumList && sumList is not TreeSubSet)
		{
			if (sumList.Length > 0)
			{
				Debug.Assert(sumList.root != null);
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

	public SumList(params int[] array) : this((IEnumerable<int>)array) { }

	public SumList(ReadOnlySpan<int> span) : this((IEnumerable<int>)span.ToArray()) { }

	protected override Func<int, SumList> CapacityCreator => x => [];

	protected override Func<IEnumerable<int>, SumList> CollectionCreator => x => new(x);

	protected override Func<ReadOnlySpan<int>, SumList> SpanCreator => x => new(x);

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

	public override IEnumerator<int> GetEnumerator() => new Enumerator(this);

	protected override int GetInternal(int index, bool invoke = true)
	{
		var current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
			{
				var value = current.Value;
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

	public virtual long GetLeftValuesSum(int index, out int actualValue)
	{
		var current = root as Node;
		long sum = 0;
		while (current != null)
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
		var current = root as Node;
		sumExceedsBy = 0;
		var index = 0;
		while (current != null)
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
		sumExceedsBy += (-sum >> 32 == 0) ? (int)sum : throw new InvalidOperationException();
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

	internal override void SetInternal(int index, int value)
	{
		if (value == 0)
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

	[DebuggerDisplay("{Value.ToString()}, Left = {Left != null ? Left.Value.ToString() : null}, Right = {Right != null ? Right.Value.ToString() : null}, Parent = {Parent != null ? Parent.Value.ToString() : null}")]
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
				if (Parent != null)
					Parent.ValuesSum += value - _valuesSum;
				_valuesSum = value;
				if (Parent != null && Parent.ValuesSum != (Parent.Left?.ValuesSum ?? 0) + (Parent.Right?.ValuesSum ?? 0) + Parent.Value)
					throw new InvalidOperationException();
			}
		}

		internal override Node DeepClone(int length)
		{
#if VERIFY
			Debug.Assert(length == GetCount());
#endif
			var newRoot = ShallowClone();
			using var pendingNodes = Stack<(Node source, Node target)>.GetNew(2 * Log2(length) + 2);
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
		internal Node GetSibling(Node node) => base.GetSibling(node) as Node ?? throw new InvalidOperationException();

		private protected override BaseSumList<int, SumList>.Node Reconstruct(int value, NodeColor color)
		{
			var node = base.Reconstruct(value, color) as Node ?? throw new InvalidOperationException();
			node._valuesSum = value;
			return node;
		}

		internal override Node ShallowClone() => new(Value, Color);

		internal override void Update(int value)
		{
			ValuesSum += value - Value;
			Value = value;
		}

		internal override void UpdateValuesSum(BaseSumList<int, SumList>.Node? newNode, BaseSumList<int, SumList>.Node? oldNode) => ValuesSum += ((newNode as Node)?.ValuesSum ?? 0) - ((oldNode as Node)?.ValuesSum ?? 0);

#if VERIFY
		internal override void Verify()
		{
			base.Verify();
			if (ValuesSum != (Left?.ValuesSum ?? 0) + (Right?.ValuesSum ?? 0) + Value)
				throw new InvalidOperationException();
		}
#endif
	}

	public new struct Enumerator : IEnumerator<int>
	{
		private readonly SumList _list;
		private readonly int _version;

		private readonly Stack<Node> _stack;
		private Node? _current;

		private readonly bool _reverse;

		internal Enumerator(SumList list) : this(list, reverse: false)
		{
		}

		internal Enumerator(SumList list, bool reverse)
		{
			_list = list;
			list.VersionCheck();
			_version = list.version;
			// 2 log(n + 1) is the maximum height.
			_stack = Stack<Node>.GetNew(2 * Log2(list.TotalCount() + 1));
			_current = null;
			_reverse = reverse;
			Initialize();
		}

		public readonly int Current
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
					throw new InvalidOperationException();
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
					_stack.Push(node as Node ?? throw new InvalidOperationException());
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
				throw new InvalidOperationException();
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
				throw new InvalidOperationException();
			_stack.Clear();
			Initialize();
		}

		void IEnumerator.Reset() => Reset();
	}

	internal sealed class TreeSubSet : SumList
	{
		private readonly SumList _underlying;
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

		internal override bool BreadthFirstTreeWalk(BaseSumWalkPredicate<int, SumList> action)
		{
			VersionCheck();
			if (root == null)
				return true;
			using Queue<Node> processQueue = [];
			processQueue.Enqueue(root as Node ?? throw new InvalidOperationException());
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
			NList<int> toRemove = [];
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

		protected override int GetInternal(int index, bool invoke = true) => _underlying.GetInternal(_min + index, invoke);

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

		internal override bool InOrderTreeWalk(BaseSumWalkPredicate<int, SumList> action)
		{
			VersionCheck();
			if (root == null)
				return true;
			// The maximum height of a red-black tree is 2*lg(n+1).
			// See page 264 of "Introduction to algorithms" by Thomas H. Cormen
			using var stack = Stack<Node>.GetNew(2 * Log2(_size + 1)); // this is not exactly right if length is out of date, but the stack can grow
			var current = root;
			using var indexStack = Stack<int>.GetNew(2 * Log2(_size + 1));
			var index = current.Left?.LeavesCount ?? 0;
			using var flagStack = Stack<bool>.GetNew(2 * Log2(_size + 1));
			while (current != null)
			{
				if (IsWithinRange(index))
				{
					stack.Push(current as Node ?? throw new InvalidOperationException());
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
					stack.Push(current as Node ?? throw new InvalidOperationException());
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
						stack.Push(node as Node ?? throw new InvalidOperationException());
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
						stack.Push(node as Node ?? throw new InvalidOperationException());
						indexStack.Push((node.Left?.LeavesCount ?? 0) + index);
						flagStack.Push(false);
						node = node.Left;
						index -= (node?.Right?.LeavesCount ?? 0) + 1;
					}
				}
			}
			return true;
		}

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

		internal override void SetInternal(int index, int value) => _underlying.SetInternal(_min + index, value);

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
			}
		}

#if DEBUG
		internal override bool VersionUpToDate() => version == _underlying.version;
#endif
	}
}

internal delegate bool BigSumWalkPredicate(BigSumList.Node node);

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigSumList : BaseSumList<MpzT, BigSumList>
{
	public BigSumList() { }

	public BigSumList(IEnumerable<MpzT> collection) : this()
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

	public BigSumList(params MpzT[] array) : this((IEnumerable<MpzT>)array) { }

	public BigSumList(ReadOnlySpan<MpzT> span) : this((IEnumerable<MpzT>)span.ToArray()) { }

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

	protected override Func<IEnumerable<MpzT>, BigSumList> CollectionCreator => x => new(x);

	protected override Func<ReadOnlySpan<MpzT>, BigSumList> SpanCreator => x => new(x);

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

	public override IEnumerator<MpzT> GetEnumerator() => new Enumerator(this);

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
		sumExceedsBy = 0;
		var index = 0;
		while (current != null)
		{
			if (sum >= (current.Left?.ValuesSum ?? 0) && sum < (current.Left?.ValuesSum ?? 0) + current.Value)
			{
				sumExceedsBy = sum - (current.Left?.ValuesSum ?? 0);
				return index + (current.Left?.LeavesCount ?? 0);
			}
			else if (sum < (current.Left?.ValuesSum ?? 0))
			{
				sumExceedsBy = new(current.Value);
				current = current.Left;
			}
			else
			{
				index += (current.Left?.LeavesCount ?? 0) + 1;
				sum -= (current.Left?.ValuesSum ?? 0) + current.Value;
				sumExceedsBy = new(current.Value);
				current = current.Right;
			}
		}
		sumExceedsBy += sum;
		return index - 1;
	}

	internal override void SetInternal(int index, MpzT value)
	{
		if (value == 0)
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
				if (Parent != null && Parent.ValuesSum != (Parent.Left?.ValuesSum ?? 0) + (Parent.Right?.ValuesSum ?? 0) + Parent.Value)
					throw new InvalidOperationException();
			}
		}

		internal override Node DeepClone(int length)
		{
#if VERIFY
			Debug.Assert(length == GetCount());
#endif
			var newRoot = ShallowClone();
			using var pendingNodes = Stack<(Node source, Node target)>.GetNew(2 * Log2(length) + 2);
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
			var node = base.Reconstruct(new(value), color) as Node ?? throw new InvalidOperationException();
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
				throw new InvalidOperationException();
		}
#endif
	}

	public new struct Enumerator : IEnumerator<MpzT>
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
			_stack = Stack<Node>.GetNew(2 * Log2(list.TotalCount() + 1));
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
					throw new InvalidOperationException();
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
					_stack.Push(node as Node ?? throw new InvalidOperationException());
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
				throw new InvalidOperationException();
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
				throw new InvalidOperationException();
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
			processQueue.Enqueue(root as Node ?? throw new InvalidOperationException());
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
			NList<int> toRemove = [];
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
			using var stack = Stack<Node>.GetNew(2 * Log2(_size + 1)); // this is not exactly right if length is out of date, but the stack can grow
			var current = root;
			using var indexStack = Stack<int>.GetNew(2 * Log2(_size + 1));
			var index = current.Left?.LeavesCount ?? 0;
			using var flagStack = Stack<bool>.GetNew(2 * Log2(_size + 1));
			while (current != null)
			{
				if (IsWithinRange(index))
				{
					stack.Push(current as Node ?? throw new InvalidOperationException());
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
					stack.Push(current as Node ?? throw new InvalidOperationException());
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
						stack.Push(node as Node ?? throw new InvalidOperationException());
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
						stack.Push(node as Node ?? throw new InvalidOperationException());
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

		internal override void SetInternal(int index, MpzT value) => _underlying.SetInternal(_min + index, value);

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
			}
		}

#if DEBUG
		internal override bool VersionUpToDate() => version == _underlying.version;
#endif
	}
}
