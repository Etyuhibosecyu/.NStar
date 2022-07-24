#pragma once
#include "NativeFunctions.h"
using NativeFunctions::Radix;

namespace NativeFunctions
{
	public ref class LZ abstract sealed
	{

	};

	//[DebuggerDisplay("({Lower}, {Length} : {Base})")]
	//public value struct Interval : IEquatable<Interval>
	//{
	//public:
	//	property unsigned Lower;
	//	property unsigned Length;
	//	property unsigned Base;

	//	Interval(unsigned base_)
	//	{
	//		Lower = 0;
	//		Length = 1;
	//		if (base_ == 0)
	//			Base = 1;
	//		else
	//			Base = base_;
	//	}

	//	Interval(unsigned lower, unsigned length, unsigned base_)
	//	{
	//		if (lower + length <= base_)
	//		{
	//			Lower = lower;
	//			Length = length;
	//		}
	//		else
	//		{
	//			Lower = 0;
	//			Length = 1;
	//		}
	//		if (base_ == 0)
	//			Base = 1;
	//		else
	//			Base = base_;
	//	}

	//	bool Equals(Object^ obj) override
	//	{
	//		if (obj == nullptr)
	//			return false;
	//		Interval^ m = as<Interval^>(obj);
	//		if (m == nullptr)
	//			return false;
	//		return Lower == m->Lower && Length == m->Length && Base == m->Base;
	//	}

	//	virtual bool Equals(Interval obj) = IEquatable<Interval>::Equals
	//	{
	//		return Equals((Object^)obj);
	//	}

	//	int GetHashCode() override
	//	{
	//		return Lower.GetHashCode() ^ Length.GetHashCode() ^ Base.GetHashCode();
	//	}

	//	static bool operator ==(Interval x, Interval y)
	//	{
	//		return x.Lower == y.Lower && x.Length == y.Length && x.Base == y.Base;
	//	}

	//	static bool operator !=(Interval x, Interval y)
	//	{
	//		return !(x == y);
	//	}
	//};

	//[DebuggerDisplay("Count = {Count}")]
	//public ref class ShortIntervalList : IList<Interval>
	//{
	//private:
	//	Interval Item1 = default, ^Item2 = default, ^Item3 = default, ^Item4 = default;
	//	SecondPart^ secondPart = null;
	//	byte _size = 0;

	//public:
	//	ShortIntervalList()
	//	{
	//	}

	//	ShortIntervalList(IEnumerable<Interval>^ collection)
	//	{
	//		foreach(auto item in collection)
	//			Add(item);
	//	}

	//	int Count = > _size;

	//	bool IsReadOnly = > false;

	//	Interval this[int index]
	//	{
	//		get = > index >= 0 && index < _size GetInternal(index) : throw new ArgumentOutOfRangeException(nameof(index));
	//		set
	//		{
	//			if (index < 0 || index >= _size)
	//				throw new ArgumentOutOfRangeException(nameof(index));
	//			SetInternal(index, value);
	//		}
	//	}

	//	Interval IList<Interval>.this[int index]{ get = > this[index]; set = > this[index] = value; }

	//	void Add(Interval item) = > SetInternal(_size++, item);

	//	void ICollection<Interval>.Add(Interval item) = > Add(item);

	//	void Clear() = > _size = 0;

	//	bool Contains(Interval item)
	//	{
	//		if ((_size >= 1 && Item1 == item) || (_size >= 2 && Item2 == item) || (_size >= 3 && Item3 == item) || (_size >= 4 && Item4 == item) || (_size >= 5 && secondPart!.Item1 == item) || (_size >= 6 && secondPart!.Item2 == item) || (_size >= 7 && secondPart!.Item3 == item) || (_size >= 8 && secondPart!.Item4 == item))
	//			return true;
	//		return false;
	//	}

	//	bool ICollection<Interval>.Contains(Interval item) = > Contains(item);

	//	void CopyTo(array<Interval>^ array_, int arrayIndex)
	//	{
	//		if (_size >= 1)
	//			array_[arrayIndex++] = Item1;
	//		else
	//			return;
	//		if (_size >= 2)
	//			array_[arrayIndex++] = Item2;
	//		else
	//			return;
	//		if (_size >= 3)
	//			array_[arrayIndex++] = Item3;
	//		else
	//			return;
	//		if (_size >= 4)
	//			array_[arrayIndex++] = Item4;
	//		else
	//			return;
	//		if (_size >= 5)
	//			array_[arrayIndex++] = secondPart!.Item1;
	//		else
	//			return;
	//		if (_size >= 6)
	//			array_[arrayIndex++] = secondPart!.Item2;
	//		else
	//			return;
	//		if (_size >= 7)
	//			array_[arrayIndex++] = secondPart!.Item3;
	//		else
	//			return;
	//		if (_size >= 8)
	//			array_[arrayIndex++] = secondPart!.Item4;
	//		else
	//			return;
	//	}

	//	void ICollection<Interval>.CopyTo(array<Interval>^ array_, int arrayIndex) = > CopyTo(array_, arrayIndex);

	//	Enumerator GetEnumerator() = > new(this);

	//	IEnumerator<Interval> IEnumerable<Interval>.GetEnumerator() = > GetEnumerator();

	//	IEnumerator IEnumerable.GetEnumerator() = > GetEnumerator();

	//	private Interval GetInternal(int index) = > index switch
	//	{
	//		0 = > Item1,
	//			1 = > Item2,
	//			2 = > Item3,
	//			3 = > Item4,
	//			4 = > secondPart!.Item1,
	//			5 = > secondPart!.Item2,
	//			6 = > secondPart!.Item3,
	//			7 = > secondPart!.Item4,
	//			_ = > throw new ArgumentOutOfRangeException(nameof(index)),
	//	};

	//	int IndexOf(Interval item)
	//	{
	//		if (_size >= 1 && Item1 == item)
	//			return 0;
	//		if (_size >= 2 && Item2 == item)
	//			return 1;
	//		if (_size >= 3 && Item3 == item)
	//			return 2;
	//		if (_size >= 4 && Item4 == item)
	//			return 3;
	//		if (_size >= 5 && secondPart!.Item1 == item)
	//			return 4;
	//		if (_size >= 6 && secondPart!.Item2 == item)
	//			return 5;
	//		if (_size >= 7 && secondPart!.Item3 == item)
	//			return 6;
	//		if (_size >= 8 && secondPart!.Item4 == item)
	//			return 7;
	//		return -1;
	//	}

	//	int IList<Interval>.IndexOf(Interval item) = > IndexOf(item);

	//	void Insert(int index, Interval item)
	//	{
	//		if (index < 0 || index > _size)
	//			throw new ArgumentOutOfRangeException(nameof(index));
	//		if (_size >= 4 && secondPart == null)
	//			secondPart = new();
	//		if (index < 7 && _size >= 7)
	//			secondPart!.Item4 = secondPart!.Item3;
	//		if (index < 6 && _size >= 6)
	//			secondPart!.Item3 = secondPart!.Item2;
	//		if (index < 5 && _size >= 5)
	//			secondPart!.Item2 = secondPart!.Item1;
	//		if (index < 4 && _size >= 4)
	//			secondPart!.Item1 = Item4;
	//		if (index < 3 && _size >= 3)
	//			Item4 = Item3;
	//		if (index < 2 && _size >= 2)
	//			Item3 = Item2;
	//		if (index < 1 && _size >= 1)
	//			Item2 = Item1;
	//		SetInternal(index, item);
	//		_size++;
	//	}

	//	void IList<Interval>.Insert(int index, Interval item) = > Insert(index, item);

	//	bool Remove(Interval item)
	//	{
	//		int index = IndexOf(item);
	//		if (index >= 0)
	//			RemoveAt(index);
	//		return index >= 0;
	//	}

	//	bool ICollection<Interval>.Remove(Interval item) = > Remove(item);

	//	void RemoveAt(int index)
	//	{
	//		if (index < 0 || index >= _size)
	//			throw new ArgumentOutOfRangeException(nameof(index));
	//		if (index >= 0)
	//			Item1 = Item2;
	//		if (index >= 1)
	//			Item2 = Item3;
	//		if (index >= 2)
	//			Item3 = Item4;
	//		if (index >= 3)
	//			Item4 = secondPart!.Item1;
	//		if (index >= 4)
	//			secondPart!.Item1 = secondPart!.Item2;
	//		if (index >= 5)
	//			secondPart!.Item2 = secondPart!.Item3;
	//		if (index >= 6)
	//			secondPart!.Item3 = secondPart!.Item4;
	//		_size--;
	//	}

	//	void IList<Interval>.RemoveAt(int index) = > RemoveAt(index);

	//	private void SetInternal(int index, Interval value)
	//	{
	//		if (index >= 4 && secondPart == null)
	//			secondPart = new();
	//		switch (index)
	//		{
	//		case 0: Item1 = value; break;
	//		case 1: Item2 = value; break;
	//		case 2: Item3 = value; break;
	//		case 3: Item4 = value; break;
	//		case 4: secondPart!.Item1 = value; break;
	//		case 5: secondPart!.Item2 = value; break;
	//		case 6: secondPart!.Item3 = value; break;
	//		case 7: secondPart!.Item4 = value; break;
	//		default: throw new ArgumentOutOfRangeException(nameof(index));
	//		}
	//	}

	//	struct Enumerator : IEnumerator<Interval>
	//	{
	//		private ShortIntervalList _list;
	//		private byte _index = 0;
	//		private Interval _current = default;

	//		Enumerator(ShortIntervalList list) = > _list = list;

	//		Interval Current = > _current;

	//		object IEnumerator.Current = > Current;

	//		void Dispose()
	//		{
	//		}

	//		bool MoveNext()
	//		{
	//			if (_index >= _list._size)
	//				return false;
	//			_current = _index switch
	//			{
	//				0 = > _list.Item1,
	//					1 = > _list.Item2,
	//					2 = > _list.Item3,
	//					3 = > _list.Item4,
	//					4 = > _list.secondPart!.Item1,
	//					5 = > _list.secondPart!.Item2,
	//					6 = > _list.secondPart!.Item3,
	//					7 = > _list.secondPart!.Item4,
	//					_ = > default,
	//			};
	//			_index++;
	//			return true;
	//		}

	//		void Reset()
	//		{
	//			_current = default;
	//			_index = 0;
	//		}
	//	}

	//	private class SecondPart
	//	{
	//		internal Interval Item1 = default, Item2 = default, Item3 = default, Item4 = default;
	//	}
	//};
}
