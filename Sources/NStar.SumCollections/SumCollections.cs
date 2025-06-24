global using Mpir.NET;
global using NStar.Core;
global using NStar.SortedSets;
global using NStar.TreeSets;
global using System;
global using System.Collections;
global using System.Diagnostics;
global using System.Reflection;
global using System.Runtime.InteropServices;
global using G = System.Collections.Generic;
global using static System.Math;
global using String = NStar.Core.String;
using System.Numerics;

namespace NStar.SumCollections;

internal delegate bool SumWalkPredicate<T>(SumSet<T>.Node node);

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class SumSet<T> : BaseSortedSet<(T Key, int Value), SumSet<T>>
{
	private protected Node? root;
	private protected int version;
	private protected static readonly Queue<Node> nodePool = new(256);
	private protected static readonly object globalLockObj = new();

	private protected const string ComparerName = "Comparer"; // Do not rename (binary serialization)
	private protected const string CountName = "Length"; // Do not rename (binary serialization)
	private protected const string ItemsName = "Items"; // Do not rename (binary serialization)
	private protected const string VersionName = "Version"; // Do not rename (binary serialization)

	internal const int StackAllocThreshold = 100;

	public SumSet() => Comparer2 = G.Comparer<T>.Default;

	public SumSet(G.IComparer<T>? comparer) => Comparer2 = comparer ?? G.Comparer<T>.Default;

	public SumSet(Func<T, T, int> compareFunction) : this(new Comparer<T>(compareFunction)) { }

	public SumSet(G.IEnumerable<(T Key, int Value)> collection) : this(collection, G.Comparer<T>.Default) { }

	public SumSet(G.IEnumerable<(T Key, int Value)> collection, G.IComparer<T>? comparer) : this(comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		// These are explicit type checks in the mold of HashSet. It would have worked better with
		// something like an ISorted<T> interface. (We could make this work for SortedList.Keys, etc.)
		if (collection is SumSet<T> sortedSet && sortedSet is not TreeSubSet && HasEqualComparer(sortedSet))
		{
			if (sortedSet.Length > 0)
			{
				Debug.Assert(sortedSet.root != null);
				_size = sortedSet._size;
				root = sortedSet.root.DeepClone(_size);
			}
			return;
		}
		if (TryToUniqueArray(collection, out var elements, out var length))
		{
			root = ConstructRootFromSortedArray(elements, 0, length - 1, null);
			_size = length;
		}
	}

	public SumSet(G.IEnumerable<(T Key, int Value)> collection, Func<T, T, int> compareFunction) : this(collection, new Comparer<T>(compareFunction)) { }

	public SumSet(params (T Key, int Value)[] array) : this((G.IEnumerable<(T Key, int Value)>)array) { }

	public SumSet(ReadOnlySpan<(T Key, int Value)> span) : this((G.IEnumerable<(T Key, int Value)>)span.ToArray()) { }

	public override int Capacity
	{
		get => _size;
		set
		{
		}
	}

	protected override Func<int, SumSet<T>> CapacityCreator => x => [];

	protected override Func<G.IEnumerable<(T Key, int Value)>, SumSet<T>> CollectionCreator => x => new(x);

	public override G.IComparer<(T Key, int Value)> Comparer => new Comparer<(T Key, int Value)>((x, y) => Comparer2.Compare(x.Key, y.Key));

	protected virtual G.IComparer<T> Comparer2 { get; }

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
			return current.Item.Key;
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
			return current.Item.Key;
		}
	}

	protected override Func<ReadOnlySpan<(T Key, int Value)>, SumSet<T>> SpanCreator => x => new(x);

	public virtual long ValuesSum => root?.ValuesSum ?? 0;

	public virtual SumSet<T> Add(T key, int value) => Add((key, value));

	protected virtual void AddAllElements(G.IEnumerable<(T Key, int Value)> collection)
	{
		if (!(collection is SumSet<T> asSorted && HasEqualComparer(asSorted)))
		{
			foreach (var item in collection)
				TryAdd(item);
			return;
		}
		// Outside range, no point in doing anything
		var min = Min;
		var max = Max;
		asSorted.InOrderTreeWalk(node =>
		{
			TryAdd(node.Item);
			return true;
		});
	}

	/// <summary>
	/// Does a left-to-right breadth-first tree walk and calls the delegate for each node.
	/// </summary>
	/// <param name="action">
	/// The delegate to invoke on each node.
	/// If the delegate returns <c>false</c>, the walk is stopped.
	/// </param>
	/// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
	internal virtual bool BreadthFirstTreeWalk(SumWalkPredicate<T> action)
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
	protected virtual unsafe ElementCount CheckUniqueAndUnfoundElements(G.IEnumerable<(T Key, int Value)> other, bool returnIfUnfound)
	{
		ElementCount result;
		// need special case in case this has no elements.
		if (Length == 0)
		{
			var numElementsInOther = 0;
			foreach (var (Key, Value) in other)
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
		foreach (var (Key, Value) in other)
		{
			var index = InternalIndexOf(Key);
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

	public override void Clear(bool _)
	{
		root?.Dispose();
		root = null;
		_size = 0;
		++version;
		Changed();
	}

	private protected static Node? ConstructRootFromSortedArray((T Key, int Value)[] arr, int startIndex, int endIndex, Node? redNode)
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
			root = Node.GetNew(arr[startIndex], NodeColor.Black);
			if (redNode != null)
				root.Left = redNode;
			break;
			case 2:
			root = Node.GetNew(arr[startIndex], NodeColor.Black);
			root.Right = Node.GetNew(arr[endIndex], NodeColor.Black);
			root.Right.ColorRed();
			if (redNode != null)
				root.Left = redNode;
			break;
			case 3:
			root = Node.GetNew(arr[startIndex + 1], NodeColor.Black);
			root.Left = Node.GetNew(arr[startIndex], NodeColor.Black);
			root.Right = Node.GetNew(arr[endIndex], NodeColor.Black);
			if (redNode != null)
				root.Left.Left = redNode;
			break;
			default:
			var midpt = (startIndex + endIndex) / 2;
			root = Node.GetNew(arr[midpt], NodeColor.Black);
			root.Left = ConstructRootFromSortedArray(arr, startIndex, midpt - 1, redNode);
			root.Right = size % 2 == 0 ?
			ConstructRootFromSortedArray(arr, midpt + 2, endIndex, Node.GetNew(arr[midpt + 1], NodeColor.Red)) :
			ConstructRootFromSortedArray(arr, midpt + 1, endIndex, null);
			break;
		}
		return root;
	}

	public virtual bool Contains(T? item) => item != null && TryGetValue(item, out var _);

	protected virtual bool ContainsAllElements(G.IEnumerable<(T Key, int Value)> collection)
	{
		if (!(collection is SumSet<T> asSorted && HasEqualComparer(asSorted)))
		{
			foreach (var item in collection)
				if (!Contains(item))
					return false;
			return true;
		}
		// Outside range, no point in doing anything
		if (Comparer2.Compare(asSorted.Max, Min) >= 0 && Comparer2.Compare(asSorted.Min, Max) <= 0)
		{
			var min = Min;
			var max = Max;
			return asSorted.InOrderTreeWalk(node =>
			{
				if (Comparer2.Compare(node.Item.Key, min) < 0)
					return true;
				if (Comparer2.Compare(node.Item.Key, max) > 0)
					return false;
				return Contains(node.Item);
			});
		}
		return true;
	}

	protected override void CopyToInternal(int sourceIndex, SumSet<T> destination, int destinationIndex, int length)
	{
		if (length == 0)
			return;
		if (length == 1)
		{
			destination.SetOrAdd(destinationIndex, GetInternal(sourceIndex));
			return;
		}
		TreeSubSet subset = new(this, GetInternal(sourceIndex).Key, GetInternal(sourceIndex + length - 1).Key, true, true);
		using var en = subset.GetEnumerator();
		if (destinationIndex < destination._size)
			new TreeSubSet(destination, destination.GetInternal(destinationIndex).Key, destination.GetInternal(Min(destinationIndex + length, destination._size) - 1).Key, true, true).InOrderTreeWalk(node =>
			{
				var b = en.MoveNext();
				if (b)
					node.Item = en.Current;
				return b;
			});
		while (en.MoveNext())
			destination.TryAdd(en.Current);
	}

	protected override void CopyToInternal(int index, (T Key, int Value)[] array, int arrayIndex, int length)
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
			array[arrayIndex++] = node.Item;
			return true;
		});
	}

	/// <summary>
	/// Returns an <see cref="G.IEqualityComparer{T}"/> object that can be used to create a collection that contains individual sets.
	/// </summary>
	public static G.IEqualityComparer<SumSet<T>> CreateSetComparer() => CreateSetComparer(memberEqualityComparer: null);

	/// <summary>
	/// Returns an <see cref="G.IEqualityComparer{T}"/> object, according to a specified comparer, that can be used to create a collection that contains individual sets.
	/// </summary>
	public static G.IEqualityComparer<SumSet<T>> CreateSetComparer(G.IEqualityComparer<T>? memberEqualityComparer) => new SumSetEqualityComparer<T>(memberEqualityComparer);

	public virtual bool Decrease(T key) => TryGetValue(key, out var value) && Update(key, value - 1);

	public override void Dispose()
	{
		root?.Dispose();
		root = null;
		_size = 0;
		version = 0;
		GC.SuppressFinalize(this);
	}

	public override SumSet<T> ExceptWith(G.IEnumerable<(T Key, int Value)> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (_size == 0)
			return this;
		if (other == this)
		{
			Clear();
			return this;
		}
		if (other is SumSet<T> asSorted && HasEqualComparer(asSorted))
		{
			// Outside range, no point in doing anything
			if (Comparer2.Compare(asSorted.Max, Min) >= 0 && Comparer2.Compare(asSorted.Min, Max) <= 0)
			{
				var min = Min;
				var max = Max;
				asSorted.InOrderTreeWalk(node =>
				{
					if (Comparer2.Compare(node.Item.Key, min) < 0)
						return true;
					if (Comparer2.Compare(node.Item.Key, max) > 0)
						return false;
					RemoveValue(node.Item);
					return true;
				});
			}
		}
		else
			RemoveAllElements(other);
		return this;
	}

	protected virtual void FindForRemove(int index, out Node? parent, out Node? grandParent, out Node? match, out Node? parentOfMatch)
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

	internal virtual Node? FindNode(T item)
	{
		var current = root;
		while (current != null)
		{
			var order = Comparer2.Compare(item, current.Item.Key);
			if (order == 0)
				return current;
			current = order < 0 ? current.Left : current.Right;
		}
		return null;
	}

	internal virtual Node? FindRange(T? from, T? to) => FindRange(from, to, true, true);

	internal virtual Node? FindRange(T? from, T? to, bool lowerBoundActive, bool upperBoundActive)
	{
		var current = root;
		while (current != null)
		{
			if (lowerBoundActive && Comparer2.Compare(from, current.Item.Key) > 0)
				current = current.Right;
			else if (upperBoundActive && Comparer2.Compare(to, current.Item.Key) < 0)
				current = current.Left;
			else
				return current;
		}
		return null;
	}

	public override (T Key, int Value) GetAndRemove(Index index)
	{
		var index2 = index.GetOffset(_size);
		if (root == null)
			return default!;
		FindForRemove(index2, out var parent, out var grandParent, out var match, out var parentOfMatch);
		(T Key, int Value) found = default!;
		// Move successor to the matching node position and replace links.
		if (match != null)
		{
			found = match.Item;
			ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
			--_size;
			Changed();
		}
		root?.ColorBlack();
#if VERIFY
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
				" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
		foreach (var x in new[] { match, parentOfMatch, parent, grandParent })
			x?.Verify();
#endif
		return found;
	}

	public override G.IEnumerator<(T Key, int Value)> GetEnumerator() => new Enumerator(this);

	protected override (T Key, int Value) GetInternal(int index, bool invoke = true)
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

	public virtual long GetLeftValuesSum(T item, out int actualValue)
	{
		var current = root;
		long sum = 0;
		while (current != null)
		{
			var order = Comparer2.Compare(item, current.Item.Key);
			if (order == 0)
			{
				actualValue = current.Item.Value;
				return sum + (current.Left?.ValuesSum ?? 0);
			}
			else if (order < 0)
				current = current.Left;
			else
			{
				sum += (current.Left?.ValuesSum ?? 0) + current.Item.Value;
				current = current.Right;
			}
		}
		actualValue = 0;
		return sum;
	}

	public virtual SumSet<T> GetViewBetween(T? lowerValue, T? upperValue)
	{
		if (Comparer2.Compare(lowerValue, upperValue) > 0)
			throw new ArgumentException("Максимум не может быть меньше минимума!");
		return new TreeSubSet(this, lowerValue, upperValue, true, true);
	}

	/// <summary>
	/// Determines whether two <see cref="SumSet{T}"/> instances have the same comparer.
	/// </summary>
	/// <param name="other">The other <see cref="SumSet{T}"/>.</param>
	/// <returns>A value indicating whether both sets have the same comparer.</returns>
	protected virtual bool HasEqualComparer(SumSet<T> other) => Comparer2 == other.Comparer2 || Comparer2.Equals(other.Comparer2);
	// Commonly, both comparers will be the default comparer (and reference-equal). Avoid a virtual method call to Equals() in that case.

	public virtual bool Increase(T key)
	{
		var node = FindNode(key);
		if (node != null)
		{
			node.Update(node.Item.Value + 1);
#if VERIFY
			foreach (var x in new[] { node, root })
				x?.Verify();
#endif
			return true;
		}
		else
			return TryAdd(key, 1);
	}

	public virtual int IndexOf(T item) => IndexOf(item, 0, _size);

	public virtual int IndexOf(T item, int index) => IndexOf(item, index, _size - index);

	public virtual int IndexOf(T item, int index, int length)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		var ret = Search(item);
		return ret >= index && ret < index + length ? ret : -1;
	}

	public virtual int IndexOfNotGreaterSum(long sum)
	{
		if (sum == ValuesSum)
			return _size;
		var current = root;
		var index = 0;
		while (current != null)
		{
			if (sum == (current.Left?.ValuesSum ?? 0))
				return index + (current.Left?.LeavesCount ?? 0);
			else if (sum < (current.Left?.ValuesSum ?? 0))
				current = current.Left;
			else
			{
				index += (current.Left?.LeavesCount ?? 0) + 1;
				sum -= (current.Left?.ValuesSum ?? 0) + current.Item.Value;
				current = current.Right;
			}
		}
		return index - 1;
	}

	public virtual int IndexOfNotLess(T item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		var ret = Search(item);
		return ret >= 0 ? ret : ~ret;
	}

	/// <summary>
	/// Does an in-order tree walk and calls the delegate for each node.
	/// </summary>
	/// <param name="action">
	/// The delegate to invoke on each node.
	/// If the delegate returns <c>false</c>, the walk is stopped.
	/// </param>
	/// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
	internal virtual bool InOrderTreeWalk(SumWalkPredicate<T> action)
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

	protected override void InsertInternal(int index, (T Key, int Value) item) => Add(item);

	// After calling InsertionBalance, we need to make sure `current` and `parent` are up-to-date.
	// It doesn't matter if we keep `grandParent` and `greatGrandParent` up-to-date, because we won't
	// need to split again in the next node.
	// By the time we need to split again, everything will be correctly set.
	protected virtual void InsertionBalance(Node current, ref Node parent, Node grandParent, Node greatGrandParent)
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
			var order = Comparer2.Compare(item, current.Item.Key);
			if (order == 0)
				return length;
			current = order < 0 ? current.Left : current.Right;
			length = order < 0 ? (2 * length + 1) : (2 * length + 2);
		}
		return -1;
	}

	public override SumSet<T> IntersectWith(G.IEnumerable<(T Key, int Value)> other)
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
		if (other is SumSet<T> asSorted && treeSubset == null && HasEqualComparer(asSorted))
		{
			// First do a merge sort to an array.
			var merged = new (T Key, int Value)[Length];
			var c = 0;
			using var mine = GetEnumerator();
			using var theirs = asSorted.GetEnumerator();
			bool mineEnded = !mine.MoveNext(), theirsEnded = !theirs.MoveNext();
			var max = Max;
			while (!mineEnded && !theirsEnded && Comparer2.Compare(theirs.Current.Key, max) <= 0)
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
			root?.Dispose();
			root = null;
			root = ConstructRootFromSortedArray(merged, 0, c - 1, null);
			_size = c;
			version++;
			Changed();
		}
		else
			IntersectWithEnumerable(other);
		return this;
	}

	internal virtual void IntersectWithEnumerable(G.IEnumerable<(T Key, int Value)> other)
	{
		// TODO: Perhaps a more space-conservative way to do this
		List<(T Key, int Value)> toSave = new(Length);
		foreach (var item in other)
		{
			if (Contains(item))
				toSave.Add(item);
		}
		Clear();
		foreach (var item in toSave)
			TryAdd(item);
	}

	public override bool IsProperSubsetOf(G.IEnumerable<(T Key, int Value)> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (other is System.Collections.ICollection c)
		{
			if (Length == 0)
				return c.Count > 0;
		}
		// another for sorted sets with the same comparer
		if (other is SumSet<T> asSorted && HasEqualComparer(asSorted))
		{
			if (Length >= asSorted.Length)
				return false;
			return IsSubsetOfSortedSetWithSameComparer(asSorted);
		}
		// Worst case: I mark every element in my set and see if I've counted all of them. O(M log N).
		var result = CheckUniqueAndUnfoundElements(other, false);
		return result.UniqueCount == Length && result.UnfoundCount > 0;
	}

	public override bool IsProperSupersetOf(G.IEnumerable<(T Key, int Value)> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (Length == 0)
			return false;
		if (other is System.Collections.ICollection c && c.Count == 0)
			return true;
		// another way for sorted sets
		if (other is SumSet<T> asSorted && HasEqualComparer(asSorted))
		{
			if (asSorted.Length >= Length)
				return false;
			var pruned = GetViewBetween(asSorted.Min, asSorted.Max);
			foreach (var item in asSorted)
				if (!pruned.Contains(item))
					return false;
			return true;
		}
		// Worst case: I mark every element in my set and see if I've counted all of them. O(M log N).
		// slight optimization, put it into a HashSet and then check can do it in O(N+M)
		// but slower in better cases + wastes space
		var result = CheckUniqueAndUnfoundElements(other, true);
		return result.UniqueCount < Length && result.UnfoundCount == 0;
	}

	public override bool IsSubsetOf(G.IEnumerable<(T Key, int Value)> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (Length == 0)
			return true;
		if (other is SumSet<T> asSorted && HasEqualComparer(asSorted))
		{
			if (Length > asSorted.Length)
				return false;
			return IsSubsetOfSortedSetWithSameComparer(asSorted);
		}
		else
		{
			// Worst case: I mark every element in my set and see if I've counted all of them. O(M log N).
			var result = CheckUniqueAndUnfoundElements(other, false);
			return result.UniqueCount == Length && result.UnfoundCount >= 0;
		}
	}

	protected virtual bool IsSubsetOfSortedSetWithSameComparer(SumSet<T> asSorted)
	{
		var prunedOther = asSorted.GetViewBetween(Min, Max);
		foreach (var item in this)
			if (!prunedOther.Contains(item))
				return false;
		return true;
	}

	public override bool IsSupersetOf(G.IEnumerable<(T Key, int Value)> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (other is System.Collections.ICollection c && c.Count == 0)
			return true;
		// do it one way for HashSets
		// another for sorted sets with the same comparer
		if (other is SumSet<T> asSorted && HasEqualComparer(asSorted))
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

	public override bool Overlaps(G.IEnumerable<(T Key, int Value)> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (Length == 0)
			return false;
		if (other is G.ICollection<T> c && c.Count == 0)
			return false;
		if (other is SumSet<T> asSorted && HasEqualComparer(asSorted) && (Comparer2.Compare(Min, asSorted.Max) > 0 || Comparer2.Compare(Max, asSorted.Min) < 0))
			return false;
		foreach (var item in other)
			if (Contains(item))
				return true;
		return false;
	}

	protected virtual void RemoveAllElements(G.IEnumerable<(T Key, int Value)> collection)
	{
		var min = Min;
		var max = Max;
		foreach (var item in collection)
		{
			if (!(Comparer2.Compare(item.Key, min) < 0 || Comparer2.Compare(item.Key, max) > 0) && Contains(item))
				RemoveValue(item);
		}
	}

	public override SumSet<T> RemoveAt(int index)
	{
		if (root == null)
			return this;
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
		return this;
	}

	public virtual bool RemoveValue(T item)
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
			// We don't need to compare after we find the match.
			var order = foundMatch ? -1 : Comparer2.Compare(item, current.Item.Key);
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
			Changed();
		}
#if VERIFY
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
				" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
		foreach (var x in new[] { parentOfMatch, parent, grandParent })
			x?.Verify();
#endif
		root?.ColorBlack();
		return foundMatch;
	}

	public override bool RemoveValue((T Key, int Value) item) => RemoveValue(item.Key);

	public virtual int RemoveWhere(Predicate<T> match)
	{
		ArgumentNullException.ThrowIfNull(match);
		List<T> matches = new(Length);
		BreadthFirstTreeWalk(n =>
		{
			if (match(n.Item.Key))
				matches.Add(n.Item.Key);
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
	protected virtual void ReplaceChildOrRoot(Node? parent, Node child, Node newChild)
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
	protected virtual void ReplaceNode(Node match, Node parentOfMatch, Node successor, Node parentOfSuccessor)
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
		lock (globalLockObj)
			nodePool.Enqueue(match);
#if VERIFY
		foreach (var x in new[] { parentOfMatch, successor, parentOfSuccessor })
			x?.Verify();
#endif
	}

	public virtual int Search(T item)
	{
		var current = root;
		var n = 0;
		while (current != null)
		{
			var order = Comparer2.Compare(item, current.Item.Key);
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

	public override int Search((T Key, int Value) item) => Search(item.Key);

	public override bool SetEquals(G.IEnumerable<(T Key, int Value)> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (other is SumSet<T> asSorted && HasEqualComparer(asSorted))
		{
			using var mine = GetEnumerator();
			using var theirs = asSorted.GetEnumerator();
			var mineEnded = !mine.MoveNext();
			var theirsEnded = !theirs.MoveNext();
			while (!mineEnded && !theirsEnded)
			{
				if (Comparer2.Compare(mine.Current.Key, theirs.Current.Key) != 0)
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

	protected override void SetInternal(int index, (T Key, int Value) value)
	{
		if (value.Value == 0)
		{
			RemoveAt(index);
			return;
		}
		var current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
			{
				current.ValuesSum += value.Value - current.Item.Value;
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
	internal static bool SortedSetEquals(SumSet<T>? set1, SumSet<T>? set2, G.IComparer<T> comparer)
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
		foreach (var (Key, Value) in set1)
		{
			found = false;
			foreach (var item2 in set2)
			{
				if (comparer.Compare(Key, item2.Key) == 0)
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

	public override SumSet<T> SymmetricExceptWith(G.IEnumerable<(T Key, int Value)> other)
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
		if (other is SumSet<T> asSorted && HasEqualComparer(asSorted))
			return SymmetricExceptWithSameComparer(asSorted);
		else if (TryToUniqueArray(other, out var elements, out var length))
		{
			foreach (var item in elements.GetSlice(0, length))
			{
				var result = Contains(item) ? RemoveValue(item) : TryAdd(item);
				Debug.Assert(result);
			}
			return this;
		}
		else
			return this;
	}

	protected virtual SumSet<T> SymmetricExceptWithSameComparer(SumSet<T> other)
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

	// Virtual function for TreeSubSet, which may need the length variable of the parent set.
	internal virtual int TotalCount() => Length;

	public override bool TryAdd((T Key, int Value) item)
	{
		if (root == null)
		{
			// The tree is empty and this is the first item.
			root = Node.GetNew(item, NodeColor.Black);
			_size = 1;
			version++;
			Changed();
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
			order = Comparer2.Compare(item.Key, current.Item.Key);
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
		var node = Node.GetNew(item, NodeColor.Red);
		if (order < 0)
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
		return true;
	}

	public virtual bool TryAdd(T key, int value) => TryAdd((key, value));

	public virtual bool TryGetValue(T key, out int value)
	{
		var node = FindNode(key);
		if (node != null)
		{
			value = node.Item.Value;
			return true;
		}
		value = default;
		return false;
	}

	protected virtual bool TryToUniqueArray(G.IEnumerable<(T Key, int Value)> collection, out (T Key, int Value)[] elements, out int length)
	{
		elements = collection is (T Key, int Value)[] array ? array : [.. collection];
		length = elements.Length;
		if (length > 0)
		{
			// If `comparer` == null, sets it to G.Comparer<T>.Default. We checked for this condition in the G.IComparer<T> constructor.
			// Array.Sort handles null comparers, but we need this later when we use `comparer.Compare` directly.
			Array.Sort(elements, 0, length, Comparer);
			// Overwrite duplicates while shifting the distinct elements towards
			// the front of the array.
			var index = 1;
			for (var i = 1; i < length; i++)
			{
				if (Comparer2.Compare(elements[i].Key, elements[i - 1].Key) != 0)
					elements[index++] = elements[i];
			}
			length = index;
			return true;
		}
		return false;
	}

	public override SumSet<T> UnionWith(G.IEnumerable<(T Key, int Value)> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		var asSorted = other as SumSet<T>;
		var treeSubset = this as TreeSubSet;
		if (treeSubset != null)
			VersionCheck();
		if (asSorted != null && treeSubset == null && Length == 0)
		{
			root = asSorted.root?.DeepClone(asSorted._size);
			_size = asSorted._size;
			version++;
			Changed();
			return this;
		}
		// This actually hurts if N is much greater than M. The / 2 is arbitrary.
		if (asSorted != null && treeSubset == null && HasEqualComparer(asSorted) && asSorted.Length > Length / 2)
		{
			// First do a merge sort to an array.
			var merged = new (T Key, int Value)[asSorted.Length + Length];
			var c = 0;
			using var mine = GetEnumerator();
			using var theirs = asSorted.GetEnumerator();
			bool mineEnded = !mine.MoveNext(), theirsEnded = !theirs.MoveNext();
			while (!mineEnded && !theirsEnded)
			{
				var comp = Comparer2.Compare(mine.Current.Key, theirs.Current.Key);
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
			root?.Dispose();
			root = null;
			root = ConstructRootFromSortedArray(merged, 0, c - 1, null);
			_size = c;
			version++;
			Changed();
		}
		else
			AddAllElements(other);
		return this;
	}

	public virtual bool Update((T Key, int Value) item)
	{
		if (item.Value <= 0)
			return RemoveValue(item.Key);
		var node = FindNode(item.Key);
		if (node != null)
		{
			node.Update(item.Value);
#if VERIFY
			foreach (var x in new[] { node, root })
				x?.Verify();
#endif
			return true;
		}
		else
			return false;
	}

	public virtual bool Update(T key, int value) => Update((key, value));

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

	[DebuggerDisplay("{Item.ToString()}, Left = {Left?.Item.ToString()}, Right = {Right?.Item.ToString()}, Parent = {Parent?.Item.ToString()}")]
	protected internal sealed class Node : IDisposable
	{
		private Node? _left;
		private Node? _right;
		private Node? Parent { get; set; }
		private long _valuesSum;
		internal (T Key, int Value) Item { get; set; }
		private int _leavesCount;

		private Node((T Key, int Value) item, NodeColor color)
		{
			Item = item;
			Color = color;
			_leavesCount = 1;
			_valuesSum = item.Value;
		}

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
				ValuesSum += (value?.ValuesSum ?? 0) - (_left?.ValuesSum ?? 0);
				_left = value;
				if (_left != null)
					_left.Parent = this;
#if VERIFY
				foreach (var x in new[] { this, _left, _right, Parent })
					x?.Verify();
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
				ValuesSum += (value?.ValuesSum ?? 0) - (_right?.ValuesSum ?? 0);
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

		internal long ValuesSum
		{
			get => _valuesSum;
			set
			{
				if (Parent != null)
					Parent.ValuesSum += value - _valuesSum;
				_valuesSum = value;
				if (Parent == null || Parent.ValuesSum == (Parent._left?.ValuesSum ?? 0) + (Parent._right?.ValuesSum ?? 0) + Parent.Item.Value)
					return;
				throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
			}
		}

		internal NodeColor Color { get; set; }

		internal bool IsBlack => Color == NodeColor.Black;

		internal bool IsRed => Color == NodeColor.Red;

		internal bool Is2Node => IsBlack && IsNullOrBlack(Left) && IsNullOrBlack(Right);

		internal bool Is4Node => IsNonNullRed(Left) && IsNonNullRed(Right);

		internal void ColorBlack() => Color = NodeColor.Black;

		internal void ColorRed() => Color = NodeColor.Red;

		internal Node DeepClone(int length)
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

		internal void FixUp()
		{
			if (Left != null)
				Left.Parent = this;
			if (Right != null)
				Right.Parent = this;
		}

		internal static Node GetNew((T Key, int Value) item, NodeColor color)
		{
			lock (globalLockObj)
				return nodePool.TryDequeue(out var node) ? node!.Reconstruct(item, color) : new(item, color);
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

		internal void Isolate()
		{
			if (Parent != null && Parent.Left == this)
				Parent.Left = null;
			if (Parent != null && Parent.Right == this)
				Parent.Right = null;
		}

		/// <summary>
		/// Combines two 2-nodes into a 4-node.
		/// </summary>
		internal void Merge2Nodes()
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
		internal void ReplaceChild(Node child, Node newChild)
		{
			if (Left == child)
				Left = newChild;
			else if (Right == child)
				Right = newChild;
		}

		private Node Reconstruct((T Key, int Value) item, NodeColor color)
		{
			Item = item;
			Color = color;
			_left = null;
			_right = null;
			Parent = null;
			_leavesCount = 1;
			_valuesSum = item.Value;
			return this;
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
		internal Node RotateLeft()
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
		internal Node RotateLeftRight()
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
		internal Node RotateRight()
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
		internal Node RotateRightLeft()
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

		internal Node ShallowClone() => GetNew(Item, Color);

		internal void Split4Node()
		{
			Debug.Assert(Left != null);
			Debug.Assert(Right != null);
			ColorRed();
			Left.ColorBlack();
			Right.ColorBlack();
		}

		internal void Update(int value)
		{
			ValuesSum += value - Item.Value;
			Item = (Item.Key, value);
		}

#if VERIFY
		private int GetCount() => 1 + (Left?.GetCount() ?? 0) + (Right?.GetCount() ?? 0);

		private bool HasChild(Node child) => child == Left || child == Right;

		private bool HasChildren(Node child1, Node child2)
		{
			Debug.Assert(child1 != child2);
			return Left == child1 && Right == child2
				|| Left == child2 && Right == child1;
		}

		internal void Verify()
		{
			if (Right != null && Right == Left)
				throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
			if (LeavesCount != (Left?.LeavesCount ?? 0) + (Right?.LeavesCount ?? 0) + 1)
				throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
					" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
			if (ValuesSum != (Left?.ValuesSum ?? 0) + (Right?.ValuesSum ?? 0) + Item.Value)
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

	public new struct Enumerator : G.IEnumerator<(T Key, int Value)>
	{
		private readonly SumSet<T> _tree;
		private readonly int _version;

		private readonly Stack<Node> _stack;
		private Node? _current;

		private readonly bool _reverse;

		internal Enumerator(SumSet<T> set) : this(set, reverse: false)
		{
		}

		internal Enumerator(SumSet<T> set, bool reverse)
		{
			_tree = set;
			set.VersionCheck();
			_version = set.version;
			// 2 log(n + 1) is the maximum height.
			_stack = (Stack<Node>?)typeof(Stack<Node>).GetMethod("GetNew", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, [2 * Log2(set.TotalCount() + 1)])!;
			Debug.Assert(_stack != null);
			_current = null;
			_reverse = reverse;
			Initialize();
		}

		public readonly (T Key, int Value) Current
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
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				return _current.Item;
			}
		}

		internal readonly bool NotStartedOrEnded => _current == null;

		public readonly void Dispose() => _stack?.Dispose();

		private void Initialize()
		{
			_current = null;
			var node = _tree.root;
			Node? next, other;
			while (node != null)
			{
				next = _reverse ? node.Right : node.Left;
				other = _reverse ? node.Left : node.Right;
				if (_tree.IsWithinRange(node.Item.Key))
				{
					_stack.Push(node);
					node = next;
				}
				else if (next == null || !_tree.IsWithinRange(next.Item.Key))
					node = other;
				else
					node = next;
			}
		}

		public bool MoveNext()
		{
			// Make sure that the underlying subset has not been changed since
			_tree.VersionCheck();
			if (_version != _tree.version)
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
				if (_tree.IsWithinRange(node.Item.Key))
				{
					_stack.Push(node);
					node = next;
				}
				else if (other == null || !_tree.IsWithinRange(other.Item.Key))
					node = next;
				else
					node = other;
			}
			return true;
		}

		internal void Reset()
		{
			if (_version != _tree.version)
				throw new InvalidOperationException("Коллекцию нельзя изменять во время перечисления по ней.");
			_stack.Clear();
			Initialize();
		}

		void IEnumerator.Reset() => Reset();
	}

	protected internal struct ElementCount
	{
		internal int UniqueCount;
		internal int UnfoundCount;
	}

	protected internal sealed class TreeSubSet : SumSet<T>
	{
		private readonly SumSet<T> _underlying;
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

		public TreeSubSet(SumSet<T> Underlying, T? Min, T? Max, bool lowerBoundActive, bool upperBoundActive) : base(Underlying.Comparer2)
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
					var comp = _uBoundActive ? Comparer2.Compare(_max, current.Item.Key) : 1;
					if (comp < 0)
						current = current.Left;
					else
					{
						result = current.Item.Key;
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
					var comp = _lBoundActive ? Comparer2.Compare(_min, current.Item.Key) : -1;
					if (comp > 0)
						current = current.Right;
					else
					{
						result = current.Item.Key;
						if (comp == 0)
							break;
						current = current.Left;
					}
				}
				return result!;
			}
		}

		internal override bool BreadthFirstTreeWalk(SumWalkPredicate<T> action)
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
				if (IsWithinRange(current.Item.Key) && !action(current))
					return false;
				if (current.Left != null && (!_lBoundActive || Comparer2.Compare(_min, current.Item.Key) < 0))
					processQueue.Enqueue(current.Left);
				if (current.Right != null && (!_uBoundActive || Comparer2.Compare(_max, current.Item.Key) > 0))
					processQueue.Enqueue(current.Right);
			}
			return true;
		}

		public override void Clear(bool _)
		{
			if (Length == 0)
				return;
			List<T> toRemove = [];
			BreadthFirstTreeWalk(n => { toRemove.Add(n.Item.Key); return true; });
			while (toRemove.Length != 0)
			{
				_underlying.RemoveValue(toRemove[^1]);
				toRemove.RemoveAt(^1);
			}
			root = null;
			_size = 0;
			version = _underlying.version;
			Changed();
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
		public override SumSet<T> GetViewBetween(T? lowerValue, T? upperValue)
		{
			if (_lBoundActive && Comparer2.Compare(_min, lowerValue) > 0)
				throw new ArgumentOutOfRangeException(nameof(lowerValue));
			if (_uBoundActive && Comparer2.Compare(_max, upperValue) < 0)
				throw new ArgumentOutOfRangeException(nameof(upperValue));
			return (TreeSubSet)_underlying.GetViewBetween(lowerValue, upperValue);
		}

		internal override bool InOrderTreeWalk(SumWalkPredicate<T> action)
		{
			VersionCheck();
			if (root == null)
				return true;
			// The maximum height of a red-black tree is 2*lg(n+1).
			// See page 264 of "Introduction to algorithms" by Thomas H. Cormen
			using var stack = (Stack<Node>?)typeof(Stack<Node>).GetMethod("GetNew", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null,
				[2 * Log2(_size + 1)]); // this is not exactly right if length is out of date, but the stack can grow
			Debug.Assert(stack != null);
			var current = root;
			while (current != null)
			{
				if (IsWithinRange(current.Item.Key))
				{
					stack.Push(current);
					current = current.Left;
				}
				else if (_lBoundActive && Comparer2.Compare(_min, current.Item.Key) > 0)
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
					if (IsWithinRange(node.Item.Key))
					{
						stack.Push(node);
						node = node.Left;
					}
					else if (_lBoundActive && Comparer2.Compare(_min, node.Item.Key) > 0)
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
			foreach (var (Key, Value) in this)
			{
				length++;
				if (Comparer2.Compare(item, Key) == 0)
					return length;
			}
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
#endif
			return -1;
		}

#if DEBUG
		internal override void IntersectWithEnumerable(G.IEnumerable<(T Key, int Value)> other)
		{
			base.IntersectWithEnumerable(other);
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
		}
#endif

		internal override bool IsWithinRange(T item)
		{
			var comp = _lBoundActive ? Comparer2.Compare(_min, item) : -1;
			if (comp > 0)
				return false;
			comp = _uBoundActive ? Comparer2.Compare(_max, item) : 1;
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

		public override bool TryAdd((T Key, int Value) item)
		{
			if (!IsWithinRange(item.Key))
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
				Changed();
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
internal class SumSetEqualityComparer<T> : G.IEqualityComparer<SumSet<T>>
{
	private protected readonly G.IComparer<T> comparer;
	private protected readonly G.IEqualityComparer<T> e_comparer;

	public SumSetEqualityComparer() : this(null, null) { }

	public SumSetEqualityComparer(G.IComparer<T>? comparer) : this(comparer, null) { }

	public SumSetEqualityComparer(G.IEqualityComparer<T>? memberEqualityComparer) : this(null, memberEqualityComparer) { }

	/// <summary>
	/// Create a new SetEqualityComparer, given a comparer for member order and another for member equality (these
	/// must be consistent in their definition of equality)
	/// </summary>        
	public SumSetEqualityComparer(G.IComparer<T>? comparer, G.IEqualityComparer<T>? memberEqualityComparer)
	{
		if (comparer == null)
			this.comparer = G.Comparer<T>.Default;
		else
			this.comparer = comparer;
		if (memberEqualityComparer == null)
			e_comparer = G.EqualityComparer<T>.Default;
		else
			e_comparer = memberEqualityComparer;
	}

	// using comparer to keep equals properties in tact; don't want to choose one of the comparers
	public virtual bool Equals(SumSet<T>? x, SumSet<T>? y) => SumSet<T>.SortedSetEquals(x, y, comparer);

	// Equals method for the comparer itself. 
	public override bool Equals(object? obj)
	{
		if (obj is not SumSetEqualityComparer<T> comparer)
			return false;
		return this.comparer == comparer.comparer;
	}

	// IMPORTANT: this part uses the fact that GetHashCode() is consistent with the notion of equality in
	// the set
	public virtual int GetHashCode(SumSet<T>? obj)
	{
		var hashCode = 0;
		if (obj != null)
		{
			foreach (var (Key, Value) in obj)
				hashCode ^= e_comparer.GetHashCode(Key!) & 0x7FFFFFFF;
		} // else returns hashcode of 0 for null HashSets
		return hashCode;
	}

	public override int GetHashCode() => comparer.GetHashCode() ^ e_comparer.GetHashCode();
}
