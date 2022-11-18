#pragma once
#include "pch.h"

#define WIN32_LEAN_AND_MEAN             // Исключите редко используемые компоненты из заголовков Windows
#include "Windows.h"
#include <string>
#include <vector>
#define EXPORT extern "C" __declspec(dllexport)
using std::wstring;
#define nameof(name) #name

class Radix
{
public:
	//static array<unsigned>^ Sort(array<unsigned>^ source, int index, int count)
	//{
	//	if (index < 0)
	//		throw gcnew ArgumentOutOfRangeException(nameof(index));
	//	if (count < 0)
	//		throw gcnew ArgumentOutOfRangeException(nameof(count));
	//	if (index + count > source->Length)
	//		throw gcnew ArgumentException(nullptr);
	//	if (count <= 1)
	//		return source;
	//	pin_ptr<unsigned> ptr = &source[index];
	//	unsigned* ptr2 = ptr;
	//	radixSortUnsigned(ptr2, count);
	//	return source;
	//}

	//generic<class T> static array<T>^ Sort(array<T>^ source, Func<T, unsigned>^ function, int index, int count)
	//{
	//	if (index < 0)
	//		throw gcnew ArgumentOutOfRangeException(nameof(index));
	//	if (count < 0)
	//		throw gcnew ArgumentOutOfRangeException(nameof(count));
	//	if (index + count > source->Length)
	//		throw gcnew ArgumentException(nullptr);
	//	if (count <= 1)
	//		return source;
	//	unsigned* indexes = new unsigned[count];
	//	for (int i = 0; i < count; i++)
	//		indexes[i] = i;
	//	unsigned* converted = new unsigned[count];
	//	for (int i = 0; i < count; i++)
	//		converted[i] = function(source[index + i]);
	//	radixSortUnsigned(converted, indexes, count);
	//	delete[] converted;
	//	array<T>^ tempArray = gcnew array<T>(count);
	//	for (int i = 0; i < count; i++)
	//		tempArray[i] = source[index + indexes[i]];
	//	for (int i = 0; i < count; i++)
	//		source[index + i] = tempArray[i];
	//	delete[] indexes;
	//	delete[] tempArray;
	//	return source;
	//}

	static void Sort(unsigned* in, int count)
	{
		return radixSortUnsigned(in, count);
	}

	static void Sort(unsigned* in, int* in2, int count)
	{
		return radixSortUnsigned(in, in2, count);
	}

	//generic<class T> where T : value class static void Sort(T* in, unsigned* in2, int n)
	//{
	//	return radixSortUnsigned(in, in2, n);
	//}

	//static array<String^>^ Sort(array<String^>^ source, int index, int count)
	//{
	//	if (index < 0)
	//		throw gcnew ArgumentOutOfRangeException(nameof(index));
	//	if (count < 0)
	//		throw gcnew ArgumentOutOfRangeException(nameof(count));
	//	if (index + count > source->Length)
	//		throw gcnew ArgumentException(nullptr);
	//	if (count <= 1)
	//		return source;
	//	pin_ptr<String^> ptr = &source[index];
	//	wstring* ptr2 = new wstring[count];
	//	for (int i = 0; i < count; i++)
	//	{
	//		pin_ptr<const wchar_t> ptr3 = PtrToStringChars(ptr[i]);
	//		ptr2[i] = ptr3;
	//	}
	//	radixSortWString(ptr2, count);
	//	for (int i = 0; i < count; i++)
	//		ptr[i] = gcnew String(ptr2[i].c_str());
	//	return source;
	//}

private:
	template<class T> static void radixSortUnsigned(T*& in, int n)
	{
		T* out = new T[n];
		int* counters = new int[sizeof(T) * 256], * count;
		createCountersUnsigned(in, counters, n);
		for (USHORT i = 0; i < sizeof(T); i++)
		{
			count = counters + (intptr_t)256 * i;
			if (count[0] == n) continue;
			radixPassUnsigned(i, n, in, out, count);
			std::swap(in, out);
		}
		delete[] out;
		delete[] counters;
	}

	template<class T, class T2> static void radixSortUnsigned(T*& in, T2*& in2, int n)
	{
		T* out = new T[n];
		T2* out2 = new T2[n];
		int* counters = new int[sizeof(T) * 256], * count;
		createCountersUnsigned(in, counters, n);
		for (USHORT i = 0; i < sizeof(T); i++)
		{
			count = counters + (intptr_t)256 * i;
			if (count[0] == n) continue;
			radixPassUnsigned(i, n, in, in2, out, out2, count);
			std::swap(in, out);
			std::swap(in2, out2);
		}
		delete[] in;
		delete[] out;
		delete[] out2;
		delete[] counters;
	}

	static void radixSortWString(wstring*& in, int n)
	{
		wstring* out = new wstring[n];
		size_t* sizes = new size_t[n];
		UCHAR** cstr = new UCHAR * [n];
		size_t max = getMaxWString(in, sizes, cstr, n);
		int* counters = new int[max * 256], * count;
		createCountersWString(max, sizes, cstr, counters, n);
		for (size_t i = max; i > 0; i--)
		{
			count = counters + 256 * (i - 1);
			if (count[0] == n) continue;
			radixPassWString(i - 1, n, sizes, cstr, in, out, count);
			std::swap(in, out);
		}
		delete[] out;
		delete[] counters;
	}

	static size_t getMaxWString(wstring* in, size_t* sizes, UCHAR** cstr, int n)
	{
		size_t max = sizes[0] = in[0].size() * sizeof(wchar_t);
		cstr[0] = (UCHAR*)in[0].c_str();
		for (int i = 1; i < n; i++)
		{
			sizes[i] = in[i].size() * sizeof(wchar_t);
			cstr[i] = (UCHAR*)in[i].c_str();
			if (sizes[i] > max)
				max = sizes[i];
		}
		return max;
	}

	static void createCountersWString(size_t max, size_t* sizes, UCHAR** cstr, int* counters, int n)
	{
		memset(counters, 0, 256 * max * sizeof(int));
		int* count, * cp;
		size_t i, j, size;
		for (i = 0; i < n; ++i)
		{
			count = counters;
			size = sizes[i];
			UCHAR* sp = cstr[i];
			for (j = 0; j < size; ++j, ++sp)
			{
				cp = (count += 256) + (int)*sp;
				++(*cp);
				*cp = *cp;
			}
			for (j = size; j < max; ++j)
			{
				cp = count += 256;
				++(*cp);
			}
		}
	}

	static void radixPassWString(size_t offset, int n, size_t* sizes, UCHAR** cstr, wstring* in, wstring* out, int* count)
	{
		int s, c, i, * cp;
		s = 0;
		cp = count;
		for (i = 0; i < 256; ++i, ++cp)
		{
			c = *cp;
			*cp = s;
			s += c;
		}
		for (i = 0; i < n; ++i)
			out[count[offset < sizes[i] ? (int)*(cstr[i] + offset) : 0]++] = in[i];
	}

	template<class T> static void createCountersUnsigned(T* data, int* counters, int n)
	{
		memset(counters, 0, 256 * sizeof(T) * sizeof(int));
		UCHAR* bp = (UCHAR*)data;
		UCHAR* dataEnd = (UCHAR*)(data + n);
		USHORT i;
		while (bp != dataEnd)
			for (i = 0; i < sizeof(T); i++)
				counters[256 * i + *bp++]++;
	}

	template<class T> static void radixPassUnsigned(short offset, int n, T* in, T* out, int* count)
	{
		T* sp;
		int s, c, i, * cp;
		UCHAR* bp;
		s = 0;
		cp = count;
		for (i = 256; i > 0; --i, ++cp)
		{
			c = *cp;
			*cp = s;
			s += c;
		}
		bp = (UCHAR*)in + offset;
		sp = in;
		for (i = n; i > 0; --i, bp += sizeof(T), ++sp)
		{
			cp = count + *bp;
			out[*cp] = *sp;
			++(*cp);
		}
	}

	template<class T, class T2> static void createCountersUnsigned(T* data, int* counters, int n)
	{
		memset(counters, 0, 256 * sizeof(T) * sizeof(int));
		UCHAR* bp = (UCHAR*)data;
		UCHAR* dataEnd = (UCHAR*)(data + n);
		USHORT i;
		while (bp != dataEnd)
			for (i = 0; i < sizeof(T); i++)
				counters[256 * i + *bp++]++;
	}

	template<class T, class T2> static void radixPassUnsigned(short offset, int n, T* in, T2* in2, T* out, T2* out2, int* count)
	{
		T* sp;
		T2* sp2;
		int s, c, i, * cp;
		UCHAR* bp;
		s = 0;
		cp = count;
		for (i = 256; i > 0; --i, ++cp)
		{
			c = *cp;
			*cp = s;
			s += c;
		}
		bp = (UCHAR*)in + offset;
		sp = in;
		sp2 = in2;
		for (i = n; i > 0; --i, bp += sizeof(T), ++sp, ++sp2)
		{
			cp = count + *bp;
			out[*cp] = *sp;
			out2[*cp] = *sp2;
			++(*cp);
		}
	}
};

EXPORT void RadixSort(unsigned* in, int index, int count)
{
	return Radix::Sort(in + index, count);
}

EXPORT void RadixSort2(unsigned* in, int* in2, int index, int count)
{
	return Radix::Sort(in + index, in2 + index, count);
}
