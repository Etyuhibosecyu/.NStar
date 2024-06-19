/*
Copyright 2010 Sergey Bochkanov.

The X-MPIR is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 3 of the License, or (at your
option) any later version.

The X-MPIR is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
License for more details.

You should have received a copy of the GNU Lesser General Public License
along with the X-MPIR; see the file COPYING.LIB.  If not, write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
MA 02110-1301, USA.
*/

/*
Modifications by John Reynolds, to provide disposal of unmanaged resources,
binary import/export functions etc.
*/

using System.Reflection;
using System.Runtime.InteropServices;

// Disable warning about missing XML comments.

namespace Mpir.NET;

public static partial class Mpir
{
	// <dummy> makes sure that <hxmpir> is initialized before it's used in the 
	// field assignments later in this file. hxmpir is declared in xmpir.cs,
	// making sure it's initialized before it's used in that file.
	// So, regardless of whether the CLR initializes the static fields of
	// mpir.cs or xmpir.cs first, <hxmpir> should always be initialized first.
	private static readonly nint dummy = InitializeHxmpir();

	private static nint InitializeHxmpir()
	{
		if (hxmpir == nint.Zero)
			hxmpir = LoadLibrarySafe(GetXMPIRLibraryPath());
		return hxmpir;
	}

	#region Static MpzT functions.

	/// Returns the largest number of a and b.
	public static MpzT Max(MpzT a, MpzT b) => a > b ? a : b;

	/// Returns the smallest number of a and b.
	public static MpzT Min(MpzT a, MpzT b) => a < b ? a : b;

	#endregion

	#region Wrappers for dynamic loading functions

	public static string GetOSString()
	{
		if (Path.DirectorySeparatorChar == '/')
			return "linux";
		return "windows";
	}
	public static string LocateLibrary(string name)
	{
		//
		// try to locate file using one of two methods:
		// * GetExecutingAssembly().CodeBase property for assemblies NOT in the GAC
		// * GetEntryAssembly().CodeBase property (if previous attempt failed)
		//
		var libpath = "";
		if (Assembly.GetExecutingAssembly().Location != "")
		{
			var codeBase = Assembly.GetExecutingAssembly().Location;
			var uri = new UriBuilder(codeBase);
			var path = Uri.UnescapeDataString(uri.Path);
			libpath = Path.GetDirectoryName(path);
			if (!File.Exists(libpath + Path.DirectorySeparatorChar + name))
				libpath = "";
		}
		if (libpath == "")
			if (Assembly.GetEntryAssembly() != null)
			{
				var codeBase = Assembly.GetEntryAssembly()?.Location ?? "";
				var uri = new UriBuilder(codeBase);
				var path = Uri.UnescapeDataString(uri.Path);
				libpath = Path.GetDirectoryName(path);
				if (!File.Exists(libpath + Path.DirectorySeparatorChar + name))
					libpath = "";
			}
		if (libpath == "")
			throw new Exception("MPIR: can't determine path to the " + name);
		return libpath + Path.DirectorySeparatorChar + name;
	}
	public static string GetXMPIRLibraryPath()
	{
		var os = GetOSString();
		var libname = "";
		if (os == "linux")
			libname = "xmpir.so";
		if (os == "windows")
			libname = "xmpir" + (nint.Size * 8).ToString() + ".dll";
		if (libname == "")
			throw new Exception("MPIR: unknown OS - '" + os + "'");
		return LocateLibrary(libname);
	}
	private static void HandleError(int ErrorCode)
	{
		//Environment.Exit(-1);
		if (ErrorCode == 1)
			throw new OutOfMemoryException("MPIR: out of memory!");
		if (ErrorCode == 2)
			throw new Exception("MPIR: division by zero!");
		if (ErrorCode == 3)
			throw new Exception("MPIR: 64-bit index in 32-bit mode!");
		throw new Exception("MPIR: unknown error!");
	}

	private static nint LoadLibrarySafe(string name)
	{
		var hResult = MpirDynamicLoader.LoadLibrarySafe(name);
		if (hResult.Equals(nint.Zero))
			throw new Exception("MPIR: unable to dlopen('" + name + "')");
		return hResult;
	}
	private static nint GetProcAddressSafe(nint hLib, string name)
	{
		var hResult = MpirDynamicLoader.GetProcAddressSafe(hLib, name);
		if (hResult.Equals(nint.Zero))
			throw new Exception("MPIR: unable to dlsym('" + name + "')");
		return hResult;
	}

	#endregion

	//
	// memory management functions
	//
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_malloc(out nint p, int size);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_free(nint p);
	private static readonly nint __ptr__xmpir_malloc = GetProcAddressSafe(hxmpir, "xmpir_malloc");
	private static readonly nint __ptr__xmpir_free = GetProcAddressSafe(hxmpir, "xmpir_free");
	private static readonly __xmpir_malloc xmpir_malloc = (__xmpir_malloc)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_malloc, typeof(__xmpir_malloc));
	private static readonly __xmpir_free xmpir_free = (__xmpir_free)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_free, typeof(__xmpir_free));

	#region Import and export functions

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private unsafe delegate void __Mpir_internal_mpz_import(nint rop, uint count, int order, uint size, int endian, uint nails, void* op);
	private static readonly nint __ptr__Mpir_internal_mpz_import = GetProcAddressSafe(hxmpir, "Mpir_internal_mpz_import");
	private static readonly __Mpir_internal_mpz_import Mpir_internal_mpz_import = (__Mpir_internal_mpz_import)Marshal.GetDelegateForFunctionPointer(__ptr__Mpir_internal_mpz_import, typeof(__Mpir_internal_mpz_import));
	public static unsafe void MpirMpzImport(MpzT rop, uint count, int order, uint size, int endian, uint nails, byte[] op)
	{
		fixed (void* srcPtr = op)
		{
			Mpir_internal_mpz_import(rop.val, count, order, size, endian, nails, srcPtr);
		}
	}
	public static unsafe void MpirMpzImportByOffset(MpzT rop, int startOffset, int endOffset, int order, uint size, int endian, uint nails, byte[] op)
	{
		fixed (byte* srcPtr = op)
		{
			Mpir_internal_mpz_import(rop.val, (uint)(endOffset - startOffset + 1), order, size, endian, nails, srcPtr + startOffset);
		}
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private unsafe delegate nint __Mpir_internal_mpz_export(void* rop, uint* countp, int order, uint size, int endian, uint nails, nint op);
	private static readonly nint __ptr__Mpir_internal_mpz_export = GetProcAddressSafe(hxmpir, "Mpir_internal_mpz_export");
	private static readonly __Mpir_internal_mpz_export Mpir_internal_mpz_export = (__Mpir_internal_mpz_export)Marshal.GetDelegateForFunctionPointer(__ptr__Mpir_internal_mpz_export, typeof(__Mpir_internal_mpz_export));
	public static unsafe byte[] MpirMpzExport(int order, uint size, int endian, uint nails, MpzT op)
	{
		var bufSize = (int)Min(MpzSizeinbase(op, 256), 2147483647);
		var destBuf = new byte[bufSize];
		var op2 = op;
		if (op < 0)
		{
			op = new(op);
			op += (MpzT)1 << bufSize * 8;
		}
		fixed (void* destPtr = destBuf)
		{
			// null countp argument, because we already know how large the result will be.
			Mpir_internal_mpz_export(destPtr, null, order, size, endian, nails, op.val);
		}
		return destBuf[order == 1 ? 0 : ^1] < 128 || op2 < 0 ? destBuf : order == 1 ? [0, .. destBuf] : [.. destBuf, 0];
	}
	#endregion
}
