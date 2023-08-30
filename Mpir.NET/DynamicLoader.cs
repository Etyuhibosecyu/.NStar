using System;
using System.Runtime.InteropServices;

namespace Mpir.NET;

partial class MpirDynamicLoader
{
	// Windows imports
	[LibraryImport("kernel32", EntryPoint = "LoadLibraryA", StringMarshalling = StringMarshalling.Utf8)]
	private static partial nint LoadLibrary(string libraryName);
	[LibraryImport("kernel32", EntryPoint = "GetProcAddress", StringMarshalling = StringMarshalling.Utf8)]
	private static partial nint GetProcAddress(nint hwnd, string procedureName);

	// Linux imports
	const int RTLD_NOW = 2;
	[LibraryImport("libdl.so", StringMarshalling = StringMarshalling.Utf8)]
	private static partial nint dlopen(string filename, int flags);
	[LibraryImport("libdl.so", StringMarshalling = StringMarshalling.Utf8)]
	private static partial nint dlsym(nint handle, string symbol);

	public static bool IsWindows() => Path.DirectorySeparatorChar == '\\';

	public static nint LoadLibrarySafe(string name)
	{
		if (IsWindows())
			return LoadLibrary(name);
		else
			return dlopen(name, RTLD_NOW);
	}

	public static nint GetProcAddressSafe(nint hLib, string name)
	{
		if (IsWindows())
			return GetProcAddress(hLib, name);
		else
			return dlsym(hLib, name);
	}
}
