// WARNING!!! This file is NOT a part of the original Mpir.NET library, it has been created by Red-Star-Soft!

using System.Runtime.InteropServices;
using gmp_randstate_intptr = nint;
using mpz_intptr = nint;

namespace NStar.Mpir;

public static partial class Mpir
{
	public static unsafe void MpirMpuImport(MpuT rop, uint count, int order, uint size, int endian, uint nails, ReadOnlySpan<byte> op)
	{
		fixed (void* srcPtr = op)
		{
			Mpir_internal_mpz_import(rop.val, count, order, size, endian, nails, srcPtr);
		}
	}
	public static unsafe void MpirMpuImportByOffset(MpuT rop, int startOffset, int endOffset, int order, uint size, int endian, uint nails, ReadOnlySpan<byte> op)
	{
		fixed (byte* srcPtr = op)
		{
			Mpir_internal_mpz_import(rop.val, (uint)(endOffset - startOffset + 1), order, size, endian, nails, srcPtr + startOffset);
		}
	}
	public static unsafe byte[] MpirMpuExport(int order, uint size, int endian, uint nails, MpuT op)
	{
		var bufSize = (int)Min(MpuSizeinbase(op, 256), 2147483647);
		var destBuf = new byte[bufSize];
		var op2 = op;
		if (op < 0)
		{
			op = new(op);
			op += (MpuT)1 << bufSize * 8;
		}
		fixed (void* destPtr = destBuf)
		{
			// null countp argument, because we already know how large the result will be.
			Mpir_internal_mpz_export(destPtr, null, order, size, endian, nails, op.val);
		}
		return destBuf;
	}
	public static mpz_intptr MpzInitSet(MpuT op)
	{
		var __retval = xmpir_mpz_init_set(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpuInit()
	{
		var __retval = xmpir_mpz_init(out var result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpuInit2(ulong n)
	{
		var __retval = xmpir_mpz_init2(out var result, n);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpuInitSet(MpuT op)
	{
		var __retval = xmpir_mpz_init_set(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpuInitSetUi(uint op)
	{
		var __retval = xmpir_mpz_init_set_ui(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpuInitSetSi(int op)
	{
		var __retval = xmpir_mpz_init_set_si(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpuInitSetD(double op)
	{
		var __retval = xmpir_mpz_init_set_d(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpuInitSetStr(string str, uint Base)
	{
		int __retval;
		var __ba_str = System.Text.Encoding.UTF8.GetBytes(str + "\0");
		__retval = xmpir_malloc(out var __str, str.Length + 1);
		if (__retval != 0) HandleError(__retval);
		Marshal.Copy(__ba_str, 0, __str, str.Length + 1);
		__retval = xmpir_mpz_init_set_str(out var result, __str, Base);
		if (__retval != 0) HandleError(__retval);
		__retval = xmpir_free(__str);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuClear(MpuT v)
	{
		var __retval = xmpir_mpz_clear(v.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int XMpirDummy3mpz(MpuT op0, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_xmpir_dummy_3mpz(out var result, op0.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static gmp_randstate_intptr GmpRandinitLc2exp(MpuT a, uint c, ulong m2exp)
	{
		var __retval = xmpir_gmp_randinit_lc_2exp(out var result, a.val, c, m2exp);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void GmpRandseed(GmpRandstateT state, MpuT seed)
	{
		var __retval = xmpir_gmp_randseed(state.val, seed.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuRealloc2(MpuT x, uint n)
	{
		var __retval = xmpir_mpz_realloc2(x.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSet(MpuT rop, MpuT op)
	{
		var __retval = xmpir_mpz_set(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSetUi(MpuT rop, uint op)
	{
		var __retval = xmpir_mpz_set_ui(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSetSi(MpuT rop, int op)
	{
		var __retval = xmpir_mpz_set_si(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSetD(MpuT rop, double op)
	{
		var __retval = xmpir_mpz_set_d(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSetQ(MpuT rop, MpqT op)
	{
		var __retval = xmpir_mpz_set_q(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSetF(MpuT rop, MpfT op)
	{
		var __retval = xmpir_mpz_set_f(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpuSetStr(MpuT rop, string str, uint Base)
	{
		int __retval;
		var __ba_str = System.Text.Encoding.UTF8.GetBytes(str + "\0");
		__retval = xmpir_malloc(out var __str, str.Length + 1);
		if (__retval != 0) HandleError(__retval);
		Marshal.Copy(__ba_str, 0, __str, str.Length + 1);
		__retval = xmpir_mpz_set_str(out var result, rop.val, __str, Base);
		if (__retval != 0) HandleError(__retval);
		__retval = xmpir_free(__str);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuSwap(MpuT rop1, MpuT rop2)
	{
		var __retval = xmpir_mpz_swap(rop1.val, rop2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpuGetUi(MpuT op)
	{
		var __retval = xmpir_mpz_get_ui(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuGetSi(MpuT op)
	{
		var __retval = xmpir_mpz_get_si(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static double MpuGetD(MpuT op)
	{
		var __retval = xmpir_mpz_get_d(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static string? MpuGetString(uint Base, MpuT op)
	{
		int __retval;
		string? result;
		__retval = xmpir_mpz_get_string(out var __result, Base, op.val);
		if (__retval != 0) HandleError(__retval);
		result = Marshal.PtrToStringAnsi(__result);
		__retval = xmpir_free(__result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuAdd(MpuT rop, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_add(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuAddUi(MpuT rop, MpuT op1, uint op2)
	{
		var __retval = xmpir_mpz_add_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSub(MpuT rop, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_sub(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSubUi(MpuT rop, MpuT op1, uint op2)
	{
		var __retval = xmpir_mpz_sub_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuUiSub(MpuT rop, uint op1, MpuT op2)
	{
		var __retval = xmpir_mpz_ui_sub(rop.val, op1, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuMul(MpuT rop, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_mul(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuMulSi(MpuT rop, MpuT op1, int op2)
	{
		var __retval = xmpir_mpz_mul_si(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuMulUi(MpuT rop, MpuT op1, uint op2)
	{
		var __retval = xmpir_mpz_mul_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuAddmul(MpuT rop, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_addmul(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuAddmulUi(MpuT rop, MpuT op1, uint op2)
	{
		var __retval = xmpir_mpz_addmul_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSubmul(MpuT rop, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_submul(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSubmulUi(MpuT rop, MpuT op1, uint op2)
	{
		var __retval = xmpir_mpz_submul_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuMul2exp(MpuT rop, MpuT op1, ulong op2)
	{
		var __retval = xmpir_mpz_mul_2exp(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuNeg(MpuT rop, MpuT op)
	{
		var __retval = xmpir_mpz_neg(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuAbs(MpuT rop, MpuT op)
	{
		var __retval = xmpir_mpz_abs(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuCdivQ(MpuT q, MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_cdiv_q(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuCdivR(MpuT r, MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_cdiv_r(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuCdivQr(MpuT q, MpuT r, MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_cdiv_qr(q.val, r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpuCdivQUi(MpuT q, MpuT n, uint d)
	{
		var __retval = xmpir_mpz_cdiv_q_ui(out var result, q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpuCdivRUi(MpuT r, MpuT n, uint d)
	{
		var __retval = xmpir_mpz_cdiv_r_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpuCdivQrUi(MpuT q, MpuT r, MpuT n, uint d)
	{
		var __retval = xmpir_mpz_cdiv_qr_ui(out var result, q.val, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpuCdivUi(MpuT n, uint d)
	{
		var __retval = xmpir_mpz_cdiv_ui(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuCdivQ2exp(MpuT q, MpuT n, ulong b)
	{
		var __retval = xmpir_mpz_cdiv_q_2exp(q.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuCdivR2exp(MpuT r, MpuT n, ulong b)
	{
		var __retval = xmpir_mpz_cdiv_r_2exp(r.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuFdivQ(MpuT q, MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_fdiv_q(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuFdivR(MpuT r, MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_fdiv_r(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuFdivQr(MpuT q, MpuT r, MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_fdiv_qr(q.val, r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpuFdivQUi(MpuT q, MpuT n, uint d)
	{
		var __retval = xmpir_mpz_fdiv_q_ui(out var result, q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpuFdivRUi(MpuT r, MpuT n, uint d)
	{
		var __retval = xmpir_mpz_fdiv_r_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpuFdivQrUi(MpuT q, MpuT r, MpuT n, uint d)
	{
		var __retval = xmpir_mpz_fdiv_qr_ui(out var result, q.val, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpuFdivUi(MpuT n, uint d)
	{
		var __retval = xmpir_mpz_fdiv_ui(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuFdivQ2exp(MpuT q, MpuT n, ulong b)
	{
		var __retval = xmpir_mpz_fdiv_q_2exp(q.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuFdivR2exp(MpuT r, MpuT n, ulong b)
	{
		var __retval = xmpir_mpz_fdiv_r_2exp(r.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuTdivQ(MpuT q, MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_tdiv_q(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuTdivR(MpuT r, MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_tdiv_r(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuTdivQr(MpuT q, MpuT r, MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_tdiv_qr(q.val, r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpuTdivQUi(MpuT q, MpuT n, uint d)
	{
		var __retval = xmpir_mpz_tdiv_q_ui(out var result, q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpuTdivRUi(MpuT r, MpuT n, uint d)
	{
		var __retval = xmpir_mpz_tdiv_r_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpuTdivQrUi(MpuT q, MpuT r, MpuT n, uint d)
	{
		var __retval = xmpir_mpz_tdiv_qr_ui(out var result, q.val, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpuTdivUi(MpuT n, uint d)
	{
		var __retval = xmpir_mpz_tdiv_ui(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuTdivQ2exp(MpuT q, MpuT n, ulong b)
	{
		var __retval = xmpir_mpz_tdiv_q_2exp(q.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuTdivR2exp(MpuT r, MpuT n, ulong b)
	{
		var __retval = xmpir_mpz_tdiv_r_2exp(r.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuMod(MpuT r, MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_mod(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpuModUi(MpuT r, MpuT n, uint d)
	{
		var __retval = xmpir_mpz_mod_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuDivexact(MpuT q, MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_divexact(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuDivexactUi(MpuT q, MpuT n, uint d)
	{
		var __retval = xmpir_mpz_divexact_ui(q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpuDivisibleP(MpuT n, MpuT d)
	{
		var __retval = xmpir_mpz_divisible_p(out var result, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuDivisibleUiP(MpuT n, uint d)
	{
		var __retval = xmpir_mpz_divisible_ui_p(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuDivisible2expP(MpuT n, ulong b)
	{
		var __retval = xmpir_mpz_divisible_2exp_p(out var result, n.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuCongruentP(MpuT n, MpuT c, MpuT d)
	{
		var __retval = xmpir_mpz_congruent_p(out var result, n.val, c.val, d.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuCongruentUiP(MpuT n, uint c, uint d)
	{
		var __retval = xmpir_mpz_congruent_ui_p(out var result, n.val, c, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuCongruent2expP(MpuT n, MpuT c, ulong b)
	{
		var __retval = xmpir_mpz_congruent_2exp_p(out var result, n.val, c.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuPowm(MpuT rop, MpuT Base, MpuT Exp, MpuT Mod)
	{
		var __retval = xmpir_mpz_powm(rop.val, Base.val, Exp.val, Mod.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuPowmUi(MpuT rop, MpuT Base, uint Exp, MpuT Mod)
	{
		var __retval = xmpir_mpz_powm_ui(rop.val, Base.val, Exp, Mod.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuPowUi(MpuT rop, MpuT Base, uint Exp)
	{
		var __retval = xmpir_mpz_pow_ui(rop.val, Base.val, Exp);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuUiPowUi(MpuT rop, uint Base, uint Exp)
	{
		var __retval = xmpir_mpz_ui_pow_ui(rop.val, Base, Exp);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpuRoot(MpuT rop, MpuT op, uint n)
	{
		var __retval = xmpir_mpz_root(out var result, rop.val, op.val, n);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuRootrem(MpuT root, MpuT rem, MpuT u, uint n)
	{
		var __retval = xmpir_mpz_rootrem(root.val, rem.val, u.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSqrt(MpuT rop, MpuT op)
	{
		var __retval = xmpir_mpz_sqrt(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuSqrtrem(MpuT rop1, MpuT rop2, MpuT op)
	{
		var __retval = xmpir_mpz_sqrtrem(rop1.val, rop2.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpuPerfectPowerP(MpuT op)
	{
		var __retval = xmpir_mpz_perfect_power_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuPerfectSquareP(MpuT op)
	{
		var __retval = xmpir_mpz_perfect_square_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuProbabPrimeP(MpuT n, uint reps)
	{
		var __retval = xmpir_mpz_probab_prime_p(out var result, n.val, reps);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuNextprime(MpuT rop, MpuT op)
	{
		var __retval = xmpir_mpz_nextprime(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuGcd(MpuT rop, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_gcd(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpuGcdUi(MpuT rop, MpuT op1, uint op2)
	{
		var __retval = xmpir_mpz_gcd_ui(out var result, rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuGcdext(MpuT g, MpuT s, MpuT t, MpuT a, MpuT b)
	{
		var __retval = xmpir_mpz_gcdext(g.val, s.val, t.val, a.val, b.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuLcm(MpuT rop, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_lcm(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuLcmUi(MpuT rop, MpuT op1, uint op2)
	{
		var __retval = xmpir_mpz_lcm_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpuInvert(MpuT rop, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_invert(out var result, rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuJacobi(MpuT a, MpuT b)
	{
		var __retval = xmpir_mpz_jacobi(out var result, a.val, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuLegendre(MpuT a, MpuT p)
	{
		var __retval = xmpir_mpz_legendre(out var result, a.val, p.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuKronecker(MpuT a, MpuT b)
	{
		var __retval = xmpir_mpz_kronecker(out var result, a.val, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuKroneckerSi(MpuT a, int b)
	{
		var __retval = xmpir_mpz_kronecker_si(out var result, a.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuKroneckerUi(MpuT a, uint b)
	{
		var __retval = xmpir_mpz_kronecker_ui(out var result, a.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuSiKronecker(int a, MpuT b)
	{
		var __retval = xmpir_mpz_si_kronecker(out var result, a, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuUiKronecker(uint a, MpuT b)
	{
		var __retval = xmpir_mpz_ui_kronecker(out var result, a, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpuRemove(MpuT rop, MpuT op, MpuT f)
	{
		var __retval = xmpir_mpz_remove(out var result, rop.val, op.val, f.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuFacUi(MpuT rop, uint op)
	{
		var __retval = xmpir_mpz_fac_ui(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuBinUi(MpuT rop, MpuT n, uint k)
	{
		var __retval = xmpir_mpz_bin_ui(rop.val, n.val, k);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuBinUiui(MpuT rop, uint n, uint k)
	{
		var __retval = xmpir_mpz_bin_uiui(rop.val, n, k);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuFibUi(MpuT fn, uint n)
	{
		var __retval = xmpir_mpz_fib_ui(fn.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuFib2Ui(MpuT fn, MpuT fnsub1, uint n)
	{
		var __retval = xmpir_mpz_fib2_ui(fn.val, fnsub1.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuLucnumUi(MpuT ln, uint n)
	{
		var __retval = xmpir_mpz_lucnum_ui(ln.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuLucnum2Ui(MpuT ln, MpuT lnsub1, uint n)
	{
		var __retval = xmpir_mpz_lucnum2_ui(ln.val, lnsub1.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpuCmp(MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_cmp(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuCmpD(MpuT op1, double op2)
	{
		var __retval = xmpir_mpz_cmp_d(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuCmpSi(MpuT op1, int op2)
	{
		var __retval = xmpir_mpz_cmp_si(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuCmpUi(MpuT op1, uint op2)
	{
		var __retval = xmpir_mpz_cmp_ui(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuCmpabs(MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_cmpabs(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuCmpabsD(MpuT op1, double op2)
	{
		var __retval = xmpir_mpz_cmpabs_d(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuCmpabsUi(MpuT op1, uint op2)
	{
		var __retval = xmpir_mpz_cmpabs_ui(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuSgn(MpuT op)
	{
		var __retval = xmpir_mpz_sgn(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuAnd(MpuT rop, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_and(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuIor(MpuT rop, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_ior(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuXor(MpuT rop, MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_xor(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuCom(MpuT rop, MpuT op)
	{
		var __retval = xmpir_mpz_com(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static ulong MpuPopcount(MpuT op)
	{
		var __retval = xmpir_mpz_popcount(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpuHamdist(MpuT op1, MpuT op2)
	{
		var __retval = xmpir_mpz_hamdist(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpuScan0(MpuT op, ulong startingBit)
	{
		var __retval = xmpir_mpz_scan0(out var result, op.val, startingBit);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpuScan1(MpuT op, ulong startingBit)
	{
		var __retval = xmpir_mpz_scan1(out var result, op.val, startingBit);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuSetbit(MpuT rop, ulong bitIndex)
	{
		var __retval = xmpir_mpz_setbit(rop.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuClrbit(MpuT rop, ulong bitIndex)
	{
		var __retval = xmpir_mpz_clrbit(rop.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuCombit(MpuT rop, ulong bitIndex)
	{
		var __retval = xmpir_mpz_combit(rop.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpuTstbit(MpuT op, ulong bitIndex)
	{
		var __retval = xmpir_mpz_tstbit(out var result, op.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpuUrandomb(MpuT rop, GmpRandstateT state, ulong n)
	{
		var __retval = xmpir_mpz_urandomb(rop.val, state.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuUrandomm(MpuT rop, GmpRandstateT state, MpuT n)
	{
		var __retval = xmpir_mpz_urandomm(rop.val, state.val, n.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpuRrandomb(MpuT rop, GmpRandstateT state, ulong n)
	{
		var __retval = xmpir_mpz_rrandomb(rop.val, state.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpuFitsUintP(MpuT op)
	{
		var __retval = xmpir_mpz_fits_uint_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuFitsSintP(MpuT op)
	{
		var __retval = xmpir_mpz_fits_sint_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuOddP(MpuT op)
	{
		var __retval = xmpir_mpz_odd_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpuEvenP(MpuT op)
	{
		var __retval = xmpir_mpz_even_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpuSizeinbase(MpuT op, uint Base)
	{
		var __retval = xmpir_mpz_sizeinbase(out var result, op.val, Base);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
}
