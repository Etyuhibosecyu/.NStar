using System;
using System.Collections.Generic;

// Disable warning about missing XML comments.

namespace Mpir.NET;

public class GmpRandstateT : IDisposable
{
	#region Data

	public nint val;
	private bool disposed = false;

	#endregion

	#region Creation and destruction

	public GmpRandstateT() => val = Mpir.GmpRandinitDefault();
	public GmpRandstateT(MpzT a, uint c, ulong m2exp) => val = Mpir.GmpRandinitLc2exp(a, c, m2exp);
	public GmpRandstateT(GmpRandstateT op) => val = Mpir.GmpRandinitSet(op);

	public static GmpRandstateT RandinitMt() => new(true);
	private GmpRandstateT(bool _) => val = Mpir.GmpRandinitMt();

	~GmpRandstateT()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		// There are no managed resources, so <disposing> is not used.
		if (!disposed)
		{
			// dispose unmanaged resources
			Mpir.GmpRandclear(this);
			disposed = true;
		}
	}

	#endregion
}
