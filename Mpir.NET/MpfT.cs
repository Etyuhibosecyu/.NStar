namespace Mpir.NET;

public struct MpfT
{
	#region Data

	public nint val;

	#endregion

	#region Creation and destruction

	public MpfT(MpfT op) => val = Mpir.MpfInitSet(op);
	public MpfT(int op) => val = Mpir.MpfInitSetSi(op);
	public MpfT(uint op) => val = Mpir.MpfInitSetUi(op);
	public MpfT(double op) => val = Mpir.MpfInitSetD(op);
	public MpfT(string s, uint Base) => val = Mpir.MpfInitSetStr(s, Base);
	public MpfT(string s) : this(s, 10u) { }

	// Initialization with MpfInit2 should not be confused with MpfT construction
	// from a uint. Thus, so we use a static construction function instead, and add
	// the dummy type init2Type to enable us to write a ctor with a unique signature.
	public static MpfT Init2(uint prec) => new(Init2Type.init2, prec);
	private enum Init2Type { init2 }
	private MpfT(Init2Type _, uint arg) => val = Mpir.MpfInit2(arg);

	#endregion
}
