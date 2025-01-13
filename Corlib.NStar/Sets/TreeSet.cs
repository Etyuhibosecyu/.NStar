using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class TreeSet<T> : BaseSortedSet<T, TreeSet<T>>
{
	private protected Node? root;
	private protected int version;

	private protected const string ComparerName = "Comparer"; // Do not rename (binary serialization)
	private protected const string CountName = "Length"; // Do not rename (binary serialization)
	private protected const string ItemsName = "Items"; // Do not rename (binary serialization)
	private protected const string VersionName = "Version"; // Do not rename (binary serialization)

	internal const int StackAllocThreshold = 100;

	public TreeSet() => Comparer = G.Comparer<T>.Default;

	public TreeSet(IComparer<T>? comparer) => Comparer = comparer ?? G.Comparer<T>.Default;

	public TreeSet(Func<T, T, int> compareFunction) : this(new Comparer<T>(compareFunction)) { }

	public TreeSet(IEnumerable<T> collection) : this(collection, G.Comparer<T>.Default) { }

	public TreeSet(IEnumerable<T> collection, IComparer<T>? comparer) : this(comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		// These are explicit type checks in the mold of HashSet. It would have worked better with
		// something like an ISorted<T> interface. (We could make this work for SortedList.Keys, etc.)
		if (collection is TreeSet<T> sortedSet && sortedSet is not TreeSubSet && HasEqualComparer(sortedSet))
		{
			if (sortedSet.Length > 0)
			{
				Debug.Assert(sortedSet.root != null);
				_size = sortedSet._size;
				root = sortedSet.root.DeepClone(_size);
			}
			return;
		}
		int length;
		var elements = collection.ToArray();
		length = elements.Length;
		if (length > 0)
		{
			// If `comparer` == null, sets it to G.Comparer<T>.Default. We checked for this condition in the G.IComparer<T> constructor.
			// Array.Sort handles null comparers, but we need this later when we use `comparer.Compare` directly.
			comparer = Comparer;
			Array.Sort(elements, 0, length, Comparer);
			// Overwrite duplicates while shifting the distinct elements towards
			// the front of the array.
			var index = 1;
			for (var i = 1; i < length; i++)
			{
				if (comparer.Compare(elements[i], elements[i - 1]) != 0)
					elements[index++] = elements[i];
			}
			length = index;
			root = ConstructRootFromSortedArray(elements, 0, length - 1, null);
			_size = length;
		}
	}

	public TreeSet(IEnumerable<T> collection, Func<T, T, int> compareFunction) : this(collection, new Comparer<T>(compareFunction)) { }

	public TreeSet(params T[] array) : this((IEnumerable<T>)array) { }

	public TreeSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray()) { }

	public override int Capacity
	{
		get => _size;
		set
		{
		}
	}

	private protected override Func<int, TreeSet<T>> CapacityCreator => x => [];

	private protected override Func<IEnumerable<T>, TreeSet<T>> CollectionCreator => x => new(x);

	public override IComparer<T> Comparer { get; }

	public override int Length
	{
		get
		{
			VersionCheck(updateCount: true);
			return _size;
		}
	}

	public virtual T? Max => MaxInternal;

	internal virtual T? MaxInternal
	{
		get
		{
			if (root == null)
				return default;
			var current = root;
			while (current.Right != null)
				current = current.Right;
			return current.Item;
		}
	}

	public virtual T? Min => MinInternal;

	internal virtual T? MinInternal
	{
		get
		{
			if (root == null)
				return default;
			var current = root;
			while (current.Left != null)
				current = current.Left;
			return current.Item;
		}
	}

	private protected void AddAllElements(IEnumerable<T> collection)
	{
		foreach (var item in collection)
		{
			if (!Contains(item))
				TryAdd(item);
		}
	}

	/// <summary>
	/// Does a left-to-right breadth-first tree walk and calls the delegate for each node.
	/// </summary>
	/// <param name="action">
	/// The delegate to invoke on each node.
	/// If the delegate returns <c>false</c>, the walk is stopped.
	/// </param>
	/// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
	internal virtual bool BreadthFirstTreeWalk(TreeWalkPredicate<T> action)
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

	/// <summary>
	/// This works similar to HashSet's CheckUniqueAndUnfound (description below), except that the bit
	/// array maps differently than in the HashSet. We can only use this for the bulk boolean checks.
	///
	/// Determines counts that can be used to determine equality, subset, and superset. This
	/// is only used when other is an IEnumerable and not a HashSet. If other is a HashSet
	/// these properties can be checked faster without use of marking because we can assume
	/// other has no duplicates.
	///
	/// The following length checks are performed by callers:
	/// 1. Equals: checks if UnfoundCount = 0 and uniqueFoundCount = Length; i.e. everything
	/// in other is in this and everything in this is in other
	/// 2. Subset: checks if UnfoundCount >= 0 and uniqueFoundCount = Length; i.e. other may
	/// have elements not in this and everything in this is in other
	/// 3. Proper subset: checks if UnfoundCount > 0 and uniqueFoundCount = Length; i.e
	/// other must have at least one element not in this and everything in this is in other
	/// 4. Proper superset: checks if unfound length = 0 and uniqueFoundCount strictly less
	/// than Length; i.e. everything in other was in this and this had at least one element
	/// not contained in other.
	///
	/// An earlier implementation used delegates to perform these checks rather than returning
	/// an ElementCount struct; however this was changed due to the perf overhead of delegates.
	/// </summary>
	private protected unsafe ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
	{
		ElementCount result;
		// need special case in case this has no elements.
		if (Length == 0)
		{
			var numElementsInOther = 0;
			foreach (var x in other)
			{
				numElementsInOther++;
				// break right away, all we want to know is whether other has 0 or 1 elements
				break;
			}
			result.UniqueCount = 0;
			result.UnfoundCount = numElementsInOther;
			return result;
		}
		var originalLastIndex = Length;
		var intArrayLength = BitHelper.ToIntArrayLength(originalLastIndex);
		Span<int> span = stackalloc int[StackAllocThreshold];
		var bitHelper = intArrayLength <= StackAllocThreshold ?
			new BitHelper(span[..intArrayLength], clear: true) :
			new BitHelper(new int[intArrayLength], clear: false);
		// length of items in other not found in this
		var UnfoundCount = 0;
		// length of unique items in other found in this
		var uniqueFoundCount = 0;
		foreach (var x in other)
		{
			var index = InternalIndexOf(x);
			if (index >= 0)
			{
				if (!bitHelper.IsMarked(index))
				{
					// item hasn't been seen yet
					bitHelper.MarkBit(index);
					uniqueFoundCount++;
				}
			}
			else
			{
				UnfoundCount++;
				if (returnIfUnfound)
					break;
			}
		}
		result.UniqueCount = uniqueFoundCount;
		result.UnfoundCount = UnfoundCount;
		return result;
	}

	public override void Clear()
	{
		root = null;
		_size = 0;
		++version;
	}

	private protected static Node? ConstructRootFromSortedArray(T[] arr, int startIndex, int endIndex, Node? redNode)
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
			root = new Node(arr[startIndex], NodeColor.Black);
			if (redNode != null)
				root.Left = redNode;
			break;
			case 2:
			root = new Node(arr[startIndex], NodeColor.Black)
			{
				Right = new Node(arr[endIndex], NodeColor.Black)
			};
			root.Right.ColorRed();
			if (redNode != null)
				root.Left = redNode;
			break;
			case 3:
			root = new Node(arr[startIndex + 1], NodeColor.Black)
			{
				Left = new Node(arr[startIndex], NodeColor.Black),
				Right = new Node(arr[endIndex], NodeColor.Black)
			};
			if (redNode != null)
				root.Left.Left = redNode;
			break;
			default:
			var midpt = (startIndex + endIndex) / 2;
			root = new Node(arr[midpt], NodeColor.Black)
			{
				Left = ConstructRootFromSortedArray(arr, startIndex, midpt - 1, redNode),
				Right = size % 2 == 0 ?
				ConstructRootFromSortedArray(arr, midpt + 2, endIndex, new Node(arr[midpt + 1], NodeColor.Red)) :
				ConstructRootFromSortedArray(arr, midpt + 1, endIndex, null)
			};
			break;
		}
		return root;
	}

	private protected bool ContainsAllElements(IEnumerable<T> collection)
	{
		foreach (var item in collection)
		{
			if (!Contains(item))
				return false;
		}
		return true;
	}

	private protected override void Copy(TreeSet<T> source, int sourceIndex, TreeSet<T> destination, int destinationIndex, int length)
	{
		if (length == 0)
			return;
		if (length == 1)
		{
			destination.SetInternal(destinationIndex, source.GetInternal(sourceIndex));
			return;
		}
		TreeSubSet subset = new(source, source.GetInternal(sourceIndex), source.GetInternal(sourceIndex + length - 1), true, true);
		var en = subset.GetEnumerator();
		if (destinationIndex < destination._size)
			new TreeSubSet(destination, destination.GetInternal(destinationIndex), destination.GetInternal(Min(destinationIndex + length, destination._size) - 1), true, true).InOrderTreeWalk(node =>
			{
				var b = en.MoveNext();
				if (b)
					node.Item = en.Current;
				return b;
			});
		while (en.MoveNext())
			destination.TryAdd(en.Current);
	}

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		ArgumentNullException.ThrowIfNull(array);
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (length > array.Length - index)
			throw new ArgumentException(null);
		length += index; // Make `length` the upper bound.
		var i = 0;
		InOrderTreeWalk(node =>
		{
			if (i >= length)
				return false;
			if (i++ < index)
				return true;
			array[arrayIndex++] = node.Item;
			return true;
		});
	}

	/// <summary>
	/// Returns an <see cref="IEqualityComparer{T}"/> object that can be used to create a collection that contains individual sets.
	/// </summary>
	public static IEqualityComparer<TreeSet<T>> CreateSetComparer() => CreateSetComparer(memberEqualityComparer: null);

	/// <summary>
	/// Returns an <see cref="IEqualityComparer{T}"/> object, according to a specified comparer, that can be used to create a collection that contains individual sets.
	/// </summary>
	public static IEqualityComparer<TreeSet<T>> CreateSetComparer(IEqualityComparer<T>? memberEqualityComparer) => new TreeSetEqualityComparer<T>(memberEqualityComparer);

	public override void Dispose()
	{
		root = null;
		_size = 0;
		version = 0;
		GC.SuppressFinalize(this);
	}

	public override TreeSet<T> ExceptWith(IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (_size == 0)
			return this;
		if (other == this)
		{
			Clear();
			return this;
		}
		if (other is TreeSet<T> asSorted && HasEqualComparer(asSorted))
		{
			// Outside range, no point in doing anything
			if (Comparer.Compare(asSorted.Max, Min) >= 0 && Comparer.Compare(asSorted.Min, Max) <= 0)
			{
				var min = Min;
				var max = Max;
				foreach (var item in other)
				{
					if (Comparer.Compare(item, min) < 0)
						continue;
					if (Comparer.Compare(item, max) > 0)
						break;
					RemoveValue(item);
				}
			}
		}
		else
			RemoveAllElements(other);
		return this;
	}

	private protected void FindForRemove(int index2, out Node? parent, out Node? grandParent, out Node? match, out Node? parentOfMatch)
	{
		// Search for a node and then find its successor.
		// Then copy the item from the successor to the matching node, and delete the successor.
		// If a node doesn't have a successor, we can replace it with its left child (if not empty),
		// or delete the matching node.
		//
		// In top-down implementation, it is important to make sure the node to be deleted is not a 2-node.
		// Following code will make sure the node on the path is not a 2-node.

		// Even if we don't actually remove from the set, we may be altering its structure (by doing rotations
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
				else
				{
					var sibling = parent.GetSibling(current);
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
			else if ((current.Left?.LeavesCount ?? 0) == index2)
			{
				// Save the matching node.
				foundMatch = true;
				match = current;
				parentOfMatch = grandParent;
				current = current.Right;
			}
			else if (current.Left == null)
			{
				index2--;
				current = current.Right;
			}
			else if (current.Left.LeavesCount >= index2)
				current = current.Left;
			else
			{
				index2 -= current.Left.LeavesCount + 1;
				current = current.Right;
			}
		}
	}

	internal virtual Node? FindNode(T item)
	{
		var current = root;
		while (current != null)
		{
			var order = Comparer.Compare(item, current.Item);
			if (order == 0)
				return current;
			current = order < 0 ? current.Left : current.Right;
		}
		return null;
	}

	internal Node? FindRange(T? from, T? to) => FindRange(from, to, true, true);

	internal Node? FindRange(T? from, T? to, bool lowerBoundActive, bool upperBoundActive)
	{
		var current = root;
		while (current != null)
		{
			if (lowerBoundActive && Comparer.Compare(from, current.Item) > 0)
				current = current.Right;
			else if (upperBoundActive && Comparer.Compare(to, current.Item) < 0)
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
			found = match.Item;
			ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
			--_size;
		}
		root?.ColorBlack();
#if DEBUG
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException();
		foreach (var x in new[] { match, parentOfMatch, parent, grandParent })
		{
			if (x?.Right != null && x?.Right == x?.Left)
				throw new InvalidOperationException();
			if (x != null && x.LeavesCount != (x.Left?.LeavesCount ?? 0) + (x.Right?.LeavesCount ?? 0) + 1)
				throw new InvalidOperationException();
			if (x?.Left != null && x?.Left.Parent == null)
				throw new InvalidOperationException();
			if (x?.Right != null && x?.Right.Parent == null)
				throw new InvalidOperationException();
		}
#endif
		return found;
	}

	public override IEnumerator<T> GetEnumerator() => new Enumerator(this);

	internal override T GetInternal(int index, bool invoke = true)
	{
		var current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
			{
				var item = current.Item;
				if (invoke)
					Changed();
				return item;
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

	public virtual TreeSet<T> GetViewBetween(T? lowerValue, T? upperValue)
	{
		if (Comparer.Compare(lowerValue, upperValue) > 0)
			throw new ArgumentException(null, nameof(lowerValue));
		return new TreeSubSet(this, lowerValue, upperValue, true, true);
	}

	/// <summary>
	/// Determines whether two <see cref="TreeSet{T}"/> instances have the same comparer.
	/// </summary>
	/// <param name="other">The other <see cref="TreeSet{T}"/>.</param>
	/// <returns>A value indicating whether both sets have the same comparer.</returns>
	private protected bool HasEqualComparer(TreeSet<T> other) => Comparer == other.Comparer || Comparer.Equals(other.Comparer);
	// Commonly, both comparers will be the default comparer (and reference-equal). Avoid a virtual method call to Equals() in that case.

	/// <summary>
	/// Does an in-order tree walk and calls the delegate for each node.
	/// </summary>
	/// <param name="action">
	/// The delegate to invoke on each node.
	/// If the delegate returns <c>false</c>, the walk is stopped.
	/// </param>
	/// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
	internal virtual bool InOrderTreeWalk(TreeWalkPredicate<T> action)
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

	internal override void InsertInternal(int index, T item) => Add(item);

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
#if DEBUG
		foreach (var x in new[] { current, parent, grandParent, greatGrandParent })
		{
			if (x?.Right != null && x?.Right == x?.Left)
				throw new InvalidOperationException();
			if (x != null && x.LeavesCount != (x.Left?.LeavesCount ?? 0) + (x.Right?.LeavesCount ?? 0) + 1)
				throw new InvalidOperationException();
			if (x?.Left != null && x?.Left.Parent == null)
				throw new InvalidOperationException();
			if (x?.Right != null && x?.Right.Parent == null)
				throw new InvalidOperationException();
		}
#endif
	}

	/// <summary>
	/// Searches for an item and returns its zero-based index in this set.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <returns>The item's zero-based index in this set, or -1 if it isn't found.</returns>
	/// <remarks>
	/// <para>
	/// This implementation is based off of http://en.wikipedia.org/wiki/Binary_Tree#Methods_for_storing_binary_trees.
	/// </para>
	/// <para>
	/// This method is used with the <see cref="BitHelper"/> class. Note that this implementation is
	/// completely different from <see cref="TreeSubSet"/>'s, and that the two should not be mixed.
	/// </para>
	/// </remarks>
	internal virtual int InternalIndexOf(T item)
	{
		var current = root;
		var length = 0;
		while (current != null)
		{
			var order = Comparer.Compare(item, current.Item);
			if (order == 0)
				return length;
			current = order < 0 ? current.Left : current.Right;
			length = order < 0 ? (2 * length + 1) : (2 * length + 2);
		}
		return -1;
	}

	public override TreeSet<T> IntersectWith(IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (Length == 0)
			return this;
		if (other == this)
			return this;
		// HashSet<T> optimizations can't be done until equality comparers and comparers are related
		// Technically, this would work as well with an ISorted<T>
		var treeSubset = this as TreeSubSet;
		if (treeSubset != null)
			VersionCheck();
		if (other is TreeSet<T> asSorted && treeSubset == null && HasEqualComparer(asSorted))
		{
			// First do a merge sort to an array.
			var merged = new T[Length];
			var c = 0;
			var mine = GetEnumerator();
			var theirs = asSorted.GetEnumerator();
			bool mineEnded = !mine.MoveNext(), theirsEnded = !theirs.MoveNext();
			var max = Max;
			while (!mineEnded && !theirsEnded && Comparer.Compare(theirs.Current, max) <= 0)
			{
				var comp = Comparer.Compare(mine.Current, theirs.Current);
				if (comp < 0)
					mineEnded = !mine.MoveNext();
				else if (comp == 0)
				{
					merged[c++] = theirs.Current;
					mineEnded = !mine.MoveNext();
					theirsEnded = !theirs.MoveNext();
				}
				else
					theirsEnded = !theirs.MoveNext();
			}
			// now merged has all c elements
			// safe to gc the root, we  have all the elements
			root = null;
			root = ConstructRootFromSortedArray(merged, 0, c - 1, null);
			_size = c;
			version++;
		}
		else
			IntersectWithEnumerable(other);
		return this;
	}

	internal virtual void IntersectWithEnumerable(IEnumerable<T> other)
	{
		// TODO: Perhaps a more space-conservative way to do this
		List<T> toSave = new(Length);
		foreach (var item in other)
		{
			if (Contains(item))
				toSave.Add(item);
		}
		Clear();
		foreach (var item in toSave)
			TryAdd(item);
	}

	public override bool IsProperSubsetOf(IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (other is System.Collections.ICollection c)
		{
			if (Length == 0)
				return c.Count > 0;
		}
		// another for sorted sets with the same comparer
		if (other is TreeSet<T> asSorted && HasEqualComparer(asSorted))
		{
			return Length < asSorted.Length && IsSubsetOfSortedSetWithSameComparer(asSorted);
		}
		// Worst case: I mark every element in my set and see if I've counted all of them. O(M log N).
		var result = CheckUniqueAndUnfoundElements(other, false);
		return result.UniqueCount == Length && result.UnfoundCount > 0;
	}

	public override bool IsProperSupersetOf(IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (Length == 0)
			return false;
		if (other is System.Collections.ICollection c && c.Count == 0)
			return true;
		// another way for sorted sets
		if (other is TreeSet<T> asSorted && HasEqualComparer(asSorted))
		{
			if (asSorted.Length >= Length)
				return false;
			var pruned = GetViewBetween(asSorted.Min, asSorted.Max);
			foreach (var item in asSorted)
			{
				if (!pruned.Contains(item))
					return false;
			}
			return true;
		}
		// Worst case: I mark every element in my set and see if I've counted all of them. O(M log N).
		// slight optimization, put it into a HashSet and then check can do it in O(N+M)
		// but slower in better cases + wastes space
		var result = CheckUniqueAndUnfoundElements(other, true);
		return result.UniqueCount < Length && result.UnfoundCount == 0;
	}

	public override bool IsSubsetOf(IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (Length == 0)
			return true;
		if (other is TreeSet<T> asSorted && HasEqualComparer(asSorted))
		{
			return Length <= asSorted.Length && IsSubsetOfSortedSetWithSameComparer(asSorted);
		}
		else
		{
			// Worst case: I mark every element in my set and see if I've counted all of them. O(M log N).
			var result = CheckUniqueAndUnfoundElements(other, false);
			return result.UniqueCount == Length && result.UnfoundCount >= 0;
		}
	}

	private protected bool IsSubsetOfSortedSetWithSameComparer(TreeSet<T> asSorted)
	{
		var prunedOther = asSorted.GetViewBetween(Min, Max);
		foreach (var item in this)
		{
			if (!prunedOther.Contains(item))
				return false;
		}
		return true;
	}

	public override bool IsSupersetOf(IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (other is System.Collections.ICollection c && c.Count == 0)
			return true;
		// do it one way for HashSets
		// another for sorted sets with the same comparer
		if (other is TreeSet<T> asSorted && HasEqualComparer(asSorted))
		{
			if (Length < asSorted.Length)
				return false;
			var pruned = GetViewBetween(asSorted.Min, asSorted.Max);
			foreach (var item in asSorted)
				if (!pruned.Contains(item))
					return false;
			return true;
		}
		// and a third for everything else
		return ContainsAllElements(other);
	}

	// Virtual function for TreeSubSet, which may need to do range checks.
	internal virtual bool IsWithinRange(T item) => true;

	// Used for set checking operations (using enumerables) that rely on counting
	private protected static int Log2(int value) => BitOperations.Log2((uint)value);

	public override bool Overlaps(IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (Length == 0)
			return false;
		if (other is G.ICollection<T> c && c.Count == 0)
			return false;
		if (other is TreeSet<T> asSorted && HasEqualComparer(asSorted) && (Comparer.Compare(Min, asSorted.Max) > 0 || Comparer.Compare(Max, asSorted.Min) < 0))
			return false;
		foreach (var item in other)
			if (Contains(item))
				return true;
		return false;
	}

	private protected void RemoveAllElements(IEnumerable<T> collection)
	{
		var min = Min;
		var max = Max;
		foreach (var item in collection)
		{
			if (!(Comparer.Compare(item, min) < 0 || Comparer.Compare(item, max) > 0) && Contains(item))
				RemoveValue(item);
		}
	}

	public override TreeSet<T> RemoveAt(int index)
	{
		if (root == null)
			return this;
		FindForRemove(index, out var parent, out var grandParent, out var match, out var parentOfMatch);
		// Move successor to the matching node position and replace links.
		if (match != null)
		{
			ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
			--_size;
		}
		root?.ColorBlack();
#if DEBUG
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException();
		foreach (var x in new[] { match, parentOfMatch, parent, grandParent })
		{
			if (x?.Right != null && x?.Right == x?.Left)
				throw new InvalidOperationException();
			if (x != null && x.LeavesCount != (x.Left?.LeavesCount ?? 0) + (x.Right?.LeavesCount ?? 0) + 1)
				throw new InvalidOperationException();
			if (x?.Left != null && x?.Left.Parent == null)
				throw new InvalidOperationException();
			if (x?.Right != null && x?.Right.Parent == null)
				throw new InvalidOperationException();
		}
#endif
		return this;
	}

	public override bool RemoveValue(T item)
	{
		if (root == null)
			return false;
		// Search for a node and then find its successor.
		// Then copy the item from the successor to the matching node, and delete the successor.
		// If a node doesn't have a successor, we can replace it with its left child (if not empty),
		// or delete the matching node.
		//
		// In top-down implementation, it is important to make sure the node to be deleted is not a 2-node.
		// Following code will make sure the node on the path is not a 2-node.

		// Even if we don't actually remove from the set, we may be altering its structure (by doing rotations
		// and such). So update our version to disable any enumerators/subsets working on it.
		version++;
		var current = root;
		Node? parent = null;
		Node? grandParent = null;
		Node? match = null;
		Node? parentOfMatch = null;
		var foundMatch = false;
		while (current != null)
		{
			if (current.Is2Node)
			{
				// Fix up 2-node
				if (parent == null)
					current.ColorRed();
				else
				{
					var sibling = parent.GetSibling(current);
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
			// We don't need to compare after we find the match.
			var order = foundMatch ? -1 : Comparer.Compare(item, current.Item);
			if (order == 0)
			{
				// Save the matching node.
				foundMatch = true;
				match = current;
				parentOfMatch = parent;
			}
			grandParent = parent;
			parent = current;
			// If we found a match, continue the search in the right sub-tree.
			current = order < 0 ? current.Left : current.Right;
		}
		// Move successor to the matching node position and replace links.
		if (match != null)
		{
			ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
			--_size;
		}
#if DEBUG
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException();
		foreach (var x in new[] { match, parentOfMatch, parent, grandParent })
		{
			if (x?.Right != null && x?.Right == x?.Left)
				throw new InvalidOperationException();
			if (x != null && x.LeavesCount != (x.Left?.LeavesCount ?? 0) + (x.Right?.LeavesCount ?? 0) + 1)
				throw new InvalidOperationException();
			if (x?.Left != null && x?.Left.Parent == null)
				throw new InvalidOperationException();
			if (x?.Right != null && x?.Right.Parent == null)
				throw new InvalidOperationException();
		}
#endif
		root?.ColorBlack();
		return foundMatch;
	}

	public virtual int RemoveWhere(Predicate<T> match)
	{
		ArgumentNullException.ThrowIfNull(match);
		List<T> matches = new(Length);
		BreadthFirstTreeWalk(n =>
		{
			if (match(n.Item))
				matches.Add(n.Item);
			return true;
		});
		// Enumerate the results of the breadth-first walk in reverse in an attempt to lower cost.
		var actuallyRemoved = 0;
		for (var i = matches.Length - 1; i >= 0; i--)
		{
			if (RemoveValue(matches[i]))
				actuallyRemoved++;
		}
		return actuallyRemoved;
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
			Debug.Assert(successor.Right == null && successor.IsRed || successor.Right!.IsRed && successor.IsBlack);
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
#if DEBUG
		foreach (var x in new[] { match, parentOfMatch, successor, parentOfSuccessor })
		{
			if (x?.Right != null && x?.Right == x?.Left)
				throw new InvalidOperationException();
			if (x != null && x.LeavesCount != (x.Left?.LeavesCount ?? 0) + (x.Right?.LeavesCount ?? 0) + 1)
				throw new InvalidOperationException();
			if (x?.Left != null && x?.Left.Parent == null)
				throw new InvalidOperationException();
			if (x?.Right != null && x?.Right.Parent == null)
				throw new InvalidOperationException();
		}
#endif
	}

	public override int Search(T item)
	{
		var current = root;
		var n = 0;
		while (current != null)
		{
			var order = Comparer.Compare(item, current.Item);
			if (order == 0)
				return (current.Left?.LeavesCount ?? 0) + n;
			else if (order < 0)
				current = current.Left;
			else
			{
				n += (current.Left?.LeavesCount ?? 0) + 1;
				current = current.Right;
			}
		}
		return ~n;
	}

	public override bool SetEquals(IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (other is TreeSet<T> asSorted && HasEqualComparer(asSorted))
		{
			var mine = GetEnumerator();
			var theirs = asSorted.GetEnumerator();
			var mineEnded = !mine.MoveNext();
			var theirsEnded = !theirs.MoveNext();
			while (!mineEnded && !theirsEnded)
			{
				if (Comparer.Compare(mine.Current, theirs.Current) != 0)
					return false;
				mineEnded = !mine.MoveNext();
				theirsEnded = !theirs.MoveNext();
			}
			return mineEnded && theirsEnded;
		}
		// Worst case: I mark every element in my set and see if I've counted all of them. O(size of the other collection).
		var result = CheckUniqueAndUnfoundElements(other, true);
		return result.UniqueCount == Length && result.UnfoundCount == 0;
	}

	internal override void SetInternal(int index, T value)
	{
		var current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
			{
				current.Item = value;
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

	/// <summary>
	/// Decides whether two sets have equal contents, using a fallback comparer if the sets do not have equivalent equality comparers.
	/// </summary>
	/// <param name="set1">The first set.</param>
	/// <param name="set2">The second set.</param>
	/// <param name="comparer">The fallback comparer to use if the sets do not have equal comparers.</param>
	/// <returns><c>true</c> if the sets have equal contents; otherwise, <c>false</c>.</returns>
	internal static bool SortedSetEquals(TreeSet<T>? set1, TreeSet<T>? set2, IComparer<T> comparer)
	{
		if (set1 == null)
			return set2 == null;
		if (set2 == null)
		{
			Debug.Assert(set1 != null);
			return false;
		}
		if (set1.HasEqualComparer(set2))
			return set1.Length == set2.Length && set1.SetEquals(set2);
		bool found;
		foreach (var x in set1)
		{
			found = false;
			foreach (var item2 in set2)
			{
				if (comparer.Compare(x, item2) == 0)
				{
					found = true;
					break;
				}
			}
			if (!found)
				return false;
		}
		return true;
	}

	public override TreeSet<T> SymmetricExceptWith(IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (Length == 0)
		{
			UnionWith(other);
			return this;
		}
		if (other == this)
		{
			Clear();
			return this;
		}
		if (other is TreeSet<T> asSorted && HasEqualComparer(asSorted))
			return SymmetricExceptWithSameComparer(asSorted);
		else
		{
			var elements = other.ToArray();
			var length = elements.Length;
			Array.Sort(elements, 0, length, Comparer);
			return SymmetricExceptWithSameComparer(elements, length);
		}
	}

	private protected TreeSet<T> SymmetricExceptWithSameComparer(TreeSet<T> other)
	{
		Debug.Assert(other != null);
		Debug.Assert(HasEqualComparer(other));
		foreach (var item in other)
		{
			var result = Contains(item) ? RemoveValue(item) : TryAdd(item);
			Debug.Assert(result);
		}
		return this;
	}

	private protected TreeSet<T> SymmetricExceptWithSameComparer(T[] other, int length)
	{
		Debug.Assert(other != null);
		Debug.Assert(length >= 0 && length <= other.Length);
		if (length == 0)
			return this;
		var previous = other[0];
		for (var i = 0; i < length; i++)
		{
			while (i < length && i != 0 && Comparer.Compare(other[i], previous) == 0)
				i++;
			if (i >= length)
				break;
			var current = other[i];
			var result = Contains(current) ? RemoveValue(current) : TryAdd(current);
			Debug.Assert(result);
			previous = current;
		}
		return this;
	}

	// Virtual function for TreeSubSet, which may need the length variable of the parent set.
	internal virtual int TotalCount() => Length;

	public override bool TryAdd(T item)
	{
		if (root == null)
		{
			// The tree is empty and this is the first item.
			root = new Node(item, NodeColor.Black);
			_size = 1;
			version++;
			return true;
		}
		// Search for a node at bottom to insert the new node.
		// If we can guarantee the node we found is not a 4-node, it would be easy to do insertion.
		// We split 4-nodes along the search path.
		var current = root;
		Node? parent = null;
		Node? grandParent = null;
		Node? greatGrandParent = null;
		// Even if we don't actually add to the set, we may be altering its structure (by doing rotations and such).
		// So update `_version` to disable any instances of Enumerator/TreeSubSet from working on it.
		version++;
		var order = 0;
		while (current != null)
		{
			order = Comparer.Compare(item, current.Item);
			if (order == 0)
			{
				// We could have changed root node to red during the search process.
				// We need to set it to black before we return.
				root.ColorBlack();
				return false;
			}
			// Split a 4-node into two 2-nodes.
			if (current.Is4Node)
			{
				current.Split4Node();
				// We could have introduced two consecutive red nodes after split. Fix that by rotation.
				if (Node.IsNonNullRed(parent))
					InsertionBalance(current, ref parent!, grandParent!, greatGrandParent!);
			}
			greatGrandParent = grandParent;
			grandParent = parent;
			parent = current;
			current = (order < 0) ? current.Left : current.Right;
		}
		Debug.Assert(parent != null);
		// We're ready to insert the new node.
		Node node = new(item, NodeColor.Red);
		if (order < 0)
			parent.Left = node;
		else
			parent.Right = node;
		// The new node will be red, so we will need to adjust colors if its parent is also red.
		if (parent.IsRed)
			InsertionBalance(node, ref parent!, grandParent!, greatGrandParent!);
#if DEBUG
		if (_size + 1 != root.LeavesCount)
			throw new InvalidOperationException();
		foreach (var x in new[] { node, parent, grandParent, greatGrandParent })
		{
			if (x?.Right != null && x?.Right == x?.Left)
				throw new InvalidOperationException();
			if (x != null && x.LeavesCount != (x.Left?.LeavesCount ?? 0) + (x.Right?.LeavesCount ?? 0) + 1)
				throw new InvalidOperationException();
			if (x?.Left != null && x?.Left.Parent == null)
				throw new InvalidOperationException();
			if (x?.Right != null && x?.Right.Parent == null)
				throw new InvalidOperationException();
		}
#endif
		// The root node is always black.
		root.ColorBlack();
		++_size;
		return true;
	}

	/// <summary>
	/// Searches the set for a given value and returns the equal value it finds, if any.
	/// </summary>
	/// <param name="equalValue">The value to search for.</param>
	/// <param name="frequency">The value from the set that the search found, or the default value of <typeparamref name="T"/> when the search yielded no match.</param>
	/// <returns>A value indicating whether the search was successful.</returns>
	/// <remarks>
	/// This can be useful when you want to reuse a previously stored reference instead of
	/// a newly constructed one (so that more sharing of references can occur) or to look up
	/// a value that has more complete data than the value you currently have, although their
	/// comparer functions indicate they are equal.
	/// </remarks>
	public virtual bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue)
	{
		var node = FindNode(equalValue);
		if (node != null)
		{
			actualValue = node.Item;
			return true;
		}
		actualValue = default;
		return false;
	}

	public override TreeSet<T> UnionWith(IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		var asSorted = other as TreeSet<T>;
		var treeSubset = this as TreeSubSet;
		if (treeSubset != null)
			VersionCheck();
		if (asSorted != null && treeSubset == null && Length == 0)
		{
			TreeSet<T> dummy = new(asSorted, Comparer);
			root = dummy.root;
			_size = dummy._size;
			version++;
			return this;
		}
		// This actually hurts if N is much greater than M. The / 2 is arbitrary.
		if (asSorted != null && treeSubset == null && HasEqualComparer(asSorted) && asSorted.Length > Length / 2)
		{
			// First do a merge sort to an array.
			var merged = new T[asSorted.Length + Length];
			var c = 0;
			var mine = GetEnumerator();
			var theirs = asSorted.GetEnumerator();
			bool mineEnded = !mine.MoveNext(), theirsEnded = !theirs.MoveNext();
			while (!mineEnded && !theirsEnded)
			{
				var comp = Comparer.Compare(mine.Current, theirs.Current);
				if (comp < 0)
				{
					merged[c++] = mine.Current;
					mineEnded = !mine.MoveNext();
				}
				else if (comp == 0)
				{
					merged[c++] = theirs.Current;
					mineEnded = !mine.MoveNext();
					theirsEnded = !theirs.MoveNext();
				}
				else
				{
					merged[c++] = theirs.Current;
					theirsEnded = !theirs.MoveNext();
				}
			}
			if (!mineEnded || !theirsEnded)
			{
				var remaining = mineEnded ? theirs : mine;
				do
					merged[c++] = remaining.Current;
				while (remaining.MoveNext());
			}
			// now merged has all c elements
			// safe to gc the root, we  have all the elements
			root = null;
			root = ConstructRootFromSortedArray(merged, 0, c - 1, null);
			_size = c;
			version++;
		}
		else
			AddAllElements(other);
		return this;
	}

	internal void UpdateVersion() => ++version;

	// Virtual function for TreeSubSet, which may need to update its length.
	internal virtual void VersionCheck(bool updateCount = false) { }

#if DEBUG
	/// <summary>
	/// debug status to be checked whenever any operation is called
	/// </summary>
	/// <returns></returns>
	internal virtual bool VersionUpToDate() => true;
#endif

	[DebuggerDisplay("{Item.ToString()}, Left = {Left?.Item.ToString()}, Right = {Right?.Item.ToString()}, Parent = {Parent?.Item.ToString()}")]
	internal sealed class Node(T item, NodeColor color)
	{
		private Node? _left;
		private Node? _right;
		internal Node? Parent { get; private set; }
		private int _leavesCount = 1;

		public T Item { get; set; } = item;

		internal Node? Left
		{
			get => _left;
			set
			{
				if (_left == value)
					return;
				if (_left != null && _left.Parent != value)
					_left.Parent = null;
				LeavesCount += (value?.LeavesCount ?? 0) - (_left?.LeavesCount ?? 0);
				_left = value;
				if (_left != null)
					_left.Parent = this;
#if DEBUG
				foreach (var x in new[] { this, _left, _right, Parent })
				{
					if (x?.Right != null && x?.Right == x?.Left)
						throw new InvalidOperationException();
					if (x != null && x.LeavesCount != (x.Left?.LeavesCount ?? 0) + (x.Right?.LeavesCount ?? 0) + 1)
						throw new InvalidOperationException();
					if (x?.Left != null && x?.Left.Parent == null)
						throw new InvalidOperationException();
					if (x?.Right != null && x?.Right.Parent == null)
						throw new InvalidOperationException();
				}
#endif
			}
		}

		internal Node? Right
		{
			get => _right;
			set
			{
				if (_right == value)
					return;
				if (_right != null && _right.Parent != value)
					_right.Parent = null;
				LeavesCount += (value?.LeavesCount ?? 0) - (_right?.LeavesCount ?? 0);
				_right = value;
				if (_right != null)
					_right.Parent = this;
#if DEBUG
				foreach (var x in new[] { this, _left, _right, Parent })
				{
					if (x?.Right != null && x?.Right == x?.Left)
						throw new InvalidOperationException();
					if (x != null && x.LeavesCount != (x.Left?.LeavesCount ?? 0) + (x.Right?.LeavesCount ?? 0) + 1)
						throw new InvalidOperationException();
					if (x?.Left != null && x?.Left.Parent == null)
						throw new InvalidOperationException();
					if (x?.Right != null && x?.Right.Parent == null)
						throw new InvalidOperationException();
				}
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

		public NodeColor Color { get; set; } = color;

		public bool IsBlack => Color == NodeColor.Black;

		public bool IsRed => Color == NodeColor.Red;

		public bool Is2Node => IsBlack && IsNullOrBlack(Left) && IsNullOrBlack(Right);

		public bool Is4Node => IsNonNullRed(Left) && IsNonNullRed(Right);

		public void ColorBlack() => Color = NodeColor.Black;

		public void ColorRed() => Color = NodeColor.Red;

		public Node DeepClone(int length)
		{
#if DEBUG
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

		public void FixUp()
		{
			if (Left != null)
				Left.Parent = this;
			if (Right != null)
				Right.Parent = this;
		}

		/// <summary>
		/// Gets the rotation this node should undergo during a removal.
		/// </summary>
		public TreeRotation GetRotation(Node current, Node sibling)
		{
			Debug.Assert(IsNonNullRed(sibling.Left) || IsNonNullRed(sibling.Right));
#if DEBUG
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
		public Node GetSibling(Node node)
		{
			Debug.Assert(node != null);
			Debug.Assert(node == Left ^ node == Right);
			return node == Left ? Right! : Left!;
		}

		public static bool IsNonNullBlack(Node? node) => node != null && node.IsBlack;

		public static bool IsNonNullRed(Node? node) => node != null && node.IsRed;

		public static bool IsNullOrBlack(Node? node) => node == null || node.IsBlack;

		public void Isolate()
		{
			if (Parent != null && Parent.Left == this)
				Parent.Left = null;
			if (Parent != null && Parent.Right == this)
				Parent.Right = null;
		}

		/// <summary>
		/// Combines two 2-nodes into a 4-node.
		/// </summary>
		public void Merge2Nodes()
		{
			Debug.Assert(IsRed);
			Debug.Assert(Left!.Is2Node);
			Debug.Assert(Right!.Is2Node);
			// Combine two 2-nodes into a 4-node.
			ColorBlack();
			Left.ColorRed();
			Right.ColorRed();
		}

		/// <summary>
		/// Replaces a child of this node with a new node.
		/// </summary>
		/// <param name="child">The child to replace.</param>
		/// <param name="newChild">The node to replace <paramref name="child"/> with.</param>
		public void ReplaceChild(Node child, Node newChild)
		{
			if (Left == child)
				Left = newChild;
			else if (Right == child)
				Right = newChild;
		}

		/// <summary>
		/// Does a rotation on this tree. May change the color of a grandchild from red to black.
		/// </summary>
		public Node? Rotate(TreeRotation rotation)
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
		public Node RotateLeft()
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
			return child;
		}

		/// <summary>
		/// Does a left-right rotation on this tree. The left child is rotated left, then this this is rotated right.
		/// </summary>
		public Node RotateLeftRight()
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
			return grandChild;
		}

		/// <summary>
		/// Does a right rotation on this tree, making this this the new right child of the current left child.
		/// </summary>
		public Node RotateRight()
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
			return child;
		}

		/// <summary>
		/// Does a right-left rotation on this tree. The right child is rotated right, then this this is rotated left.
		/// </summary>
		public Node RotateRightLeft()
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
			return grandChild;
		}

		public Node ShallowClone() => new(Item, Color);

		public void Split4Node()
		{
			Debug.Assert(Left != null);
			Debug.Assert(Right != null);
			ColorRed();
			Left.ColorBlack();
			Right.ColorBlack();
		}

#if DEBUG
		private int GetCount() => 1 + (Left?.GetCount() ?? 0) + (Right?.GetCount() ?? 0);

		private bool HasChild(Node child) => child == Left || child == Right;

		private bool HasChildren(Node child1, Node child2)
		{
			Debug.Assert(child1 != child2);
			return Left == child1 && Right == child2
				|| Left == child2 && Right == child1;
		}
#endif
	}

	public new struct Enumerator : IEnumerator<T>
	{
		private readonly TreeSet<T> _set;
		private readonly int _version;

		private readonly Stack<Node> _stack;
		private Node? _current;

		private readonly bool _reverse;

		internal Enumerator(TreeSet<T> set) : this(set, reverse: false)
		{
		}

		internal Enumerator(TreeSet<T> set, bool reverse)
		{
			_set = set;
			set.VersionCheck();
			_version = set.version;
			// 2 log(n + 1) is the maximum height.
			_stack = Stack<Node>.GetNew(2 * Log2(set.TotalCount() + 1));
			_current = null;
			_reverse = reverse;
			Initialize();
		}

		public readonly T Current
		{
			get
			{
				if (_current != null)
					return _current.Item;
				return default!; // Should only happen when accessing Current is undefined behavior
			}
		}

		readonly object? IEnumerator.Current
		{
			get
			{
				if (_current == null)
					throw new InvalidOperationException();
				return _current.Item;
			}
		}

		internal readonly bool NotStartedOrEnded => _current == null;

		public readonly void Dispose() => _stack?.Dispose();

		private void Initialize()
		{
			_current = null;
			var node = _set.root;
			Node? next, other;
			while (node != null)
			{
				next = _reverse ? node.Right : node.Left;
				other = _reverse ? node.Left : node.Right;
				if (_set.IsWithinRange(node.Item))
				{
					_stack.Push(node);
					node = next;
				}
				else if (next == null || !_set.IsWithinRange(next.Item))
					node = other;
				else
					node = next;
			}
		}

		public bool MoveNext()
		{
			// Make sure that the underlying subset has not been changed since
			_set.VersionCheck();
			if (_version != _set.version)
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
				if (_set.IsWithinRange(node.Item))
				{
					_stack.Push(node);
					node = next;
				}
				else if (other == null || !_set.IsWithinRange(other.Item))
					node = next;
				else
					node = other;
			}
			return true;
		}

		internal void Reset()
		{
			if (_version != _set.version)
				throw new InvalidOperationException();
			_stack.Clear();
			Initialize();
		}

		void IEnumerator.Reset() => Reset();
	}

	internal struct ElementCount
	{
		internal int UniqueCount;
		internal int UnfoundCount;
	}

	internal sealed class TreeSubSet : TreeSet<T>
	{
		private readonly TreeSet<T> _underlying;
		private readonly T? _min;
		private readonly T? _max;
		// keeps track of whether the length variable is up to date
		// up to date -> _countVersion = _underlying.version
		// not up to date -> _countVersion < _underlying.version
		private int _countVersion;
		// these exist for unbounded collections
		// for instance, you could allow this subset to be defined for i > 10. The set will throw if
		// anything <= 10 is added, but there is no upper bound. These features Head(), Tail(), were punted
		// in the spec, and are not available, but the framework is there to make them available at some point.
		private readonly bool _lBoundActive, _uBoundActive;
		// used to see if the length is out of date

		public TreeSubSet(TreeSet<T> Underlying, T? Min, T? Max, bool lowerBoundActive, bool upperBoundActive) : base(Underlying.Comparer)
		{
			_underlying = Underlying;
			_min = Min;
			_max = Max;
			_lBoundActive = lowerBoundActive;
			_uBoundActive = upperBoundActive;
			root = _underlying.FindRange(_min, _max, _lBoundActive, _uBoundActive); // root is first element within range
			_size = 0;
			version = -1;
			_countVersion = -1;
		}

		internal override T MaxInternal
		{
			get
			{
				VersionCheck();
				var current = root;
				T? result = default;
				while (current != null)
				{
					var comp = _uBoundActive ? Comparer.Compare(_max, current.Item) : 1;
					if (comp < 0)
						current = current.Left;
					else
					{
						result = current.Item;
						if (comp == 0)
							break;
						current = current.Right;
					}
				}
				return result!;
			}
		}

		internal override T MinInternal
		{
			get
			{
				VersionCheck();
				var current = root;
				T? result = default;
				while (current != null)
				{
					var comp = _lBoundActive ? Comparer.Compare(_min, current.Item) : -1;
					if (comp > 0)
						current = current.Right;
					else
					{
						result = current.Item;
						if (comp == 0)
							break;
						current = current.Left;
					}
				}
				return result!;
			}
		}

		internal override bool BreadthFirstTreeWalk(TreeWalkPredicate<T> action)
		{
			VersionCheck();
			if (root == null)
				return true;
			using Queue<Node> processQueue = [];
			processQueue.Enqueue(root);
			Node current;
			while (processQueue.Length != 0)
			{
				current = processQueue.Dequeue();
				if (IsWithinRange(current.Item) && !action(current))
					return false;
				if (current.Left != null && (!_lBoundActive || Comparer.Compare(_min, current.Item) < 0))
					processQueue.Enqueue(current.Left);
				if (current.Right != null && (!_uBoundActive || Comparer.Compare(_max, current.Item) > 0))
					processQueue.Enqueue(current.Right);
			}
			return true;
		}

		public override void Clear()
		{
			if (Length == 0)
				return;
			List<T> toRemove = [];
			BreadthFirstTreeWalk(n => { toRemove.Add(n.Item); return true; });
			while (toRemove.Length != 0)
			{
				_underlying.RemoveValue(toRemove[^1]);
				toRemove.RemoveAt(^1);
			}
			root = null;
			_size = 0;
			version = _underlying.version;
		}

		public override bool Contains(T? item)
		{
			VersionCheck();
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
#endif
			return base.Contains(item);
		}

		internal override Node? FindNode(T item)
		{
			if (!IsWithinRange(item))
				return null;
			VersionCheck();
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
#endif
			return base.FindNode(item);
		}

		// This passes functionality down to the underlying tree, clipping edges if necessary
		// There's nothing gained by having a nested subset. May as well draw it from the base
		// Cannot increase the bounds of the subset, can only decrease it
		public override TreeSet<T> GetViewBetween(T? lowerValue, T? upperValue)
		{
			if (_lBoundActive && Comparer.Compare(_min, lowerValue) > 0)
				throw new ArgumentOutOfRangeException(nameof(lowerValue));
			if (_uBoundActive && Comparer.Compare(_max, upperValue) < 0)
				throw new ArgumentOutOfRangeException(nameof(upperValue));
			return (TreeSubSet)_underlying.GetViewBetween(lowerValue, upperValue);
		}

		internal override bool InOrderTreeWalk(TreeWalkPredicate<T> action)
		{
			VersionCheck();
			if (root == null)
				return true;
			// The maximum height of a red-black tree is 2*lg(n+1).
			// See page 264 of "Introduction to algorithms" by Thomas H. Cormen
			using var stack = Stack<Node>.GetNew(2 * Log2(_size + 1)); // this is not exactly right if length is out of date, but the stack can grow
			var current = root;
			while (current != null)
			{
				if (IsWithinRange(current.Item))
				{
					stack.Push(current);
					current = current.Left;
				}
				else if (_lBoundActive && Comparer.Compare(_min, current.Item) > 0)
					current = current.Right;
				else
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
					if (IsWithinRange(node.Item))
					{
						stack.Push(node);
						node = node.Left;
					}
					else if (_lBoundActive && Comparer.Compare(_min, node.Item) > 0)
						node = node.Right;
					else
						node = node.Left;
				}
			}
			return true;
		}

		// this does indexing in an inefficient way compared to the actual sortedset, but it saves a
		// lot of space
		internal override int InternalIndexOf(T item)
		{
			var length = -1;
			foreach (var x in this)
			{
				length++;
				if (Comparer.Compare(item, x) == 0)
					return length;
			}
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
#endif
			return -1;
		}

#if DEBUG
		internal override void IntersectWithEnumerable(IEnumerable<T> other)
		{
			base.IntersectWithEnumerable(other);
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
		}
#endif

		internal override bool IsWithinRange(T item)
		{
			var comp = _lBoundActive ? Comparer.Compare(_min, item) : -1;
			if (comp > 0)
				return false;
			comp = _uBoundActive ? Comparer.Compare(_max, item) : 1;
			return comp >= 0;
		}

		public override bool RemoveValue(T item)
		{
			if (!IsWithinRange(item))
				return false;
			var ret = _underlying.RemoveValue(item);
			VersionCheck();
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
#endif
			return ret;
		}

		/// <summary>
		/// Returns the number of elements <c>length</c> of the parent set.
		/// </summary>
		internal override int TotalCount()
		{
			Debug.Assert(_underlying != null);
			return _underlying.Length;
		}

		public override bool TryAdd(T item)
		{
			if (!IsWithinRange(item))
				throw new ArgumentOutOfRangeException(nameof(item));
			var ret = _underlying.TryAdd(item);
			VersionCheck();
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
#endif
			return ret;
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

/// <summary>
/// A class that generates an G.IEqualityComparer for this SortedSet. Requires that the definition of
/// equality defined by the G.IComparer for this SortedSet be consistent with the default G.IEqualityComparer
/// for the type T. If not, such an G.IEqualityComparer should be provided through the constructor.
/// </summary>    
internal class TreeSetEqualityComparer<T> : IEqualityComparer<TreeSet<T>>
{
	private protected readonly IComparer<T> comparer;
	private protected readonly IEqualityComparer<T> e_comparer;

	public TreeSetEqualityComparer() : this(null, null) { }

	public TreeSetEqualityComparer(IComparer<T>? comparer) : this(comparer, null) { }

	public TreeSetEqualityComparer(IEqualityComparer<T>? memberEqualityComparer) : this(null, memberEqualityComparer) { }

	/// <summary>
	/// Create a new SetEqualityComparer, given a comparer for member order and another for member equality (these
	/// must be consistent in their definition of equality)
	/// </summary>        
	public TreeSetEqualityComparer(IComparer<T>? comparer, IEqualityComparer<T>? memberEqualityComparer)
	{
		if (comparer == null)
			this.comparer = G.Comparer<T>.Default;
		else
			this.comparer = comparer;
		if (memberEqualityComparer == null)
			e_comparer = EqualityComparer<T>.Default;
		else
			e_comparer = memberEqualityComparer;
	}

	// using comparer to keep equals properties in tact; don't want to choose one of the comparers
	public virtual bool Equals(TreeSet<T>? x, TreeSet<T>? y) => TreeSet<T>.SortedSetEquals(x, y, comparer);

	// Equals method for the comparer itself. 
	public override bool Equals(object? obj)
	{
		if (obj is not TreeSetEqualityComparer<T> comparer)
			return false;
		return this.comparer == comparer.comparer;
	}

	// IMPORTANT: this part uses the fact that GetHashCode() is consistent with the notion of equality in
	// the set
	public virtual int GetHashCode(TreeSet<T>? obj)
	{
		var hashCode = 0;
		if (obj != null)
		{
			foreach (var x in obj)
				hashCode ^= e_comparer.GetHashCode(x!) & 0x7FFFFFFF;
		} // else returns hashcode of 0 for null HashSets
		return hashCode;
	}

	public override int GetHashCode() => comparer.GetHashCode() ^ e_comparer.GetHashCode();
}
