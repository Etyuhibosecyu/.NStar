
// Disable warning about missing XML comments.

namespace NStar.Mpir;

public struct MpqT
{
	#region Data

	public nint val;

	#endregion

	#region Creation and destruction

	public MpqT() => val = Mpir.MpqInit();

	public MpqT(string str) : this(str, 10u) { }

	public MpqT(string str, uint Base) => Mpir.MpqSetStr(this, str, Base);

	#endregion
}
