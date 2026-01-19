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

using System.Runtime.InteropServices;
using mpz_intptr = nint;
using mpq_intptr = nint;
using mpf_intptr = nint;
using gmp_randstate_intptr = nint;

// Disable warning about missing XML comments.

namespace Mpir.NET;

public static partial class Mpir
{
	//
	// Automatically generated code: functions
	//
	public static mpz_intptr MpzInit()
	{
		var __retval = xmpir_mpz_init(out var result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpzInit2(ulong n)
	{
		var __retval = xmpir_mpz_init2(out var result, n);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpzInitSet(MpzT op)
	{
		var __retval = xmpir_mpz_init_set(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpzInitSetUi(uint op)
	{
		var __retval = xmpir_mpz_init_set_ui(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpzInitSetSi(int op)
	{
		var __retval = xmpir_mpz_init_set_si(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpzInitSetD(double op)
	{
		var __retval = xmpir_mpz_init_set_d(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpzInitSetStr(string str, uint Base)
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
	public static mpq_intptr MpqInit()
	{
		var __retval = xmpir_mpq_init(out var result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpf_intptr MpfInit2(uint prec)
	{
		var __retval = xmpir_mpf_init2(out var result, prec);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpf_intptr MpfInitSet(MpfT op)
	{
		var __retval = xmpir_mpf_init_set(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpf_intptr MpfInitSetUi(uint op)
	{
		var __retval = xmpir_mpf_init_set_ui(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpf_intptr MpfInitSetSi(int op)
	{
		var __retval = xmpir_mpf_init_set_si(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpf_intptr MpfInitSetD(double op)
	{
		var __retval = xmpir_mpf_init_set_d(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpf_intptr MpfInitSetStr(string str, uint Base)
	{
		int __retval;
		var __ba_str = System.Text.Encoding.UTF8.GetBytes(str + "\0");
		__retval = xmpir_malloc(out var __str, str.Length + 1);
		if (__retval != 0) HandleError(__retval);
		Marshal.Copy(__ba_str, 0, __str, str.Length + 1);
		__retval = xmpir_mpf_init_set_str(out var result, __str, Base);
		if (__retval != 0) HandleError(__retval);
		__retval = xmpir_free(__str);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzClear(MpzT v)
	{
		var __retval = xmpir_mpz_clear(v.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqClear(MpqT v)
	{
		var __retval = xmpir_mpq_clear(v.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfClear(MpfT v)
	{
		var __retval = xmpir_mpf_clear(v.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void XMpirDummy()
	{
		var __retval = xmpir_xmpir_dummy();
		if (__retval != 0) HandleError(__retval);
	}
	public static int XMpirDummyAdd(int a, int b)
	{
		var __retval = xmpir_xmpir_dummy_add(out var result, a, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int XMpirDummy3mpz(MpzT op0, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_xmpir_dummy_3mpz(out var result, op0.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static gmp_randstate_intptr GmpRandinitDefault()
	{
		var __retval = xmpir_gmp_randinit_default(out var result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static gmp_randstate_intptr GmpRandinitMt()
	{
		var __retval = xmpir_gmp_randinit_mt(out var result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static gmp_randstate_intptr GmpRandinitLc2exp(MpzT a, uint c, ulong m2exp)
	{
		var __retval = xmpir_gmp_randinit_lc_2exp(out var result, a.val, c, m2exp);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static gmp_randstate_intptr GmpRandinitSet(GmpRandstateT op)
	{
		var __retval = xmpir_gmp_randinit_set(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void GmpRandclear(GmpRandstateT v)
	{
		var __retval = xmpir_gmp_randclear(v.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void GmpRandseed(GmpRandstateT state, MpzT seed)
	{
		var __retval = xmpir_gmp_randseed(state.val, seed.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void GmpRandseedUi(GmpRandstateT state, uint seed)
	{
		var __retval = xmpir_gmp_randseed_ui(state.val, seed);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint GmpUrandombUi(GmpRandstateT state, uint n)
	{
		var __retval = xmpir_gmp_urandomb_ui(out var result, state.val, n);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint GmpUrandommUi(GmpRandstateT state, uint n)
	{
		var __retval = xmpir_gmp_urandomm_ui(out var result, state.val, n);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzRealloc2(MpzT x, uint n)
	{
		var __retval = xmpir_mpz_realloc2(x.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetDefaultPrec(ulong prec)
	{
		var __retval = xmpir_mpf_set_default_prec(prec);
		if (__retval != 0) HandleError(__retval);
	}
	public static ulong MpfGetDefaultPrec()
	{
		var __retval = xmpir_mpf_get_default_prec(out var result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzSet(MpzT rop, MpzT op)
	{
		var __retval = xmpir_mpz_set(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSetUi(MpzT rop, uint op)
	{
		var __retval = xmpir_mpz_set_ui(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSetSi(MpzT rop, int op)
	{
		var __retval = xmpir_mpz_set_si(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSetD(MpzT rop, double op)
	{
		var __retval = xmpir_mpz_set_d(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSetQ(MpzT rop, MpqT op)
	{
		var __retval = xmpir_mpz_set_q(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSetF(MpzT rop, MpfT op)
	{
		var __retval = xmpir_mpz_set_f(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzSetStr(MpzT rop, string str, uint Base)
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
	public static void MpzSwap(MpzT rop1, MpzT rop2)
	{
		var __retval = xmpir_mpz_swap(rop1.val, rop2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzGetUi(MpzT op)
	{
		var __retval = xmpir_mpz_get_ui(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzGetSi(MpzT op)
	{
		var __retval = xmpir_mpz_get_si(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static double MpzGetD(MpzT op)
	{
		var __retval = xmpir_mpz_get_d(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static string? MpzGetString(uint Base, MpzT op)
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
	public static void MpzAdd(MpzT rop, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_add(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzAddUi(MpzT rop, MpzT op1, uint op2)
	{
		var __retval = xmpir_mpz_add_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSub(MpzT rop, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_sub(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSubUi(MpzT rop, MpzT op1, uint op2)
	{
		var __retval = xmpir_mpz_sub_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzUiSub(MpzT rop, uint op1, MpzT op2)
	{
		var __retval = xmpir_mpz_ui_sub(rop.val, op1, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzMul(MpzT rop, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_mul(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzMulSi(MpzT rop, MpzT op1, int op2)
	{
		var __retval = xmpir_mpz_mul_si(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzMulUi(MpzT rop, MpzT op1, uint op2)
	{
		var __retval = xmpir_mpz_mul_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzAddmul(MpzT rop, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_addmul(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzAddmulUi(MpzT rop, MpzT op1, uint op2)
	{
		var __retval = xmpir_mpz_addmul_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSubmul(MpzT rop, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_submul(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSubmulUi(MpzT rop, MpzT op1, uint op2)
	{
		var __retval = xmpir_mpz_submul_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzMul2exp(MpzT rop, MpzT op1, ulong op2)
	{
		var __retval = xmpir_mpz_mul_2exp(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzNeg(MpzT rop, MpzT op)
	{
		var __retval = xmpir_mpz_neg(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzAbs(MpzT rop, MpzT op)
	{
		var __retval = xmpir_mpz_abs(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCdivQ(MpzT q, MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_cdiv_q(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCdivR(MpzT r, MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_cdiv_r(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCdivQr(MpzT q, MpzT r, MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_cdiv_qr(q.val, r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzCdivQUi(MpzT q, MpzT n, uint d)
	{
		var __retval = xmpir_mpz_cdiv_q_ui(out var result, q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzCdivRUi(MpzT r, MpzT n, uint d)
	{
		var __retval = xmpir_mpz_cdiv_r_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzCdivQrUi(MpzT q, MpzT r, MpzT n, uint d)
	{
		var __retval = xmpir_mpz_cdiv_qr_ui(out var result, q.val, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzCdivUi(MpzT n, uint d)
	{
		var __retval = xmpir_mpz_cdiv_ui(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzCdivQ2exp(MpzT q, MpzT n, ulong b)
	{
		var __retval = xmpir_mpz_cdiv_q_2exp(q.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCdivR2exp(MpzT r, MpzT n, ulong b)
	{
		var __retval = xmpir_mpz_cdiv_r_2exp(r.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFdivQ(MpzT q, MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_fdiv_q(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFdivR(MpzT r, MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_fdiv_r(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFdivQr(MpzT q, MpzT r, MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_fdiv_qr(q.val, r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzFdivQUi(MpzT q, MpzT n, uint d)
	{
		var __retval = xmpir_mpz_fdiv_q_ui(out var result, q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzFdivRUi(MpzT r, MpzT n, uint d)
	{
		var __retval = xmpir_mpz_fdiv_r_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzFdivQrUi(MpzT q, MpzT r, MpzT n, uint d)
	{
		var __retval = xmpir_mpz_fdiv_qr_ui(out var result, q.val, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzFdivUi(MpzT n, uint d)
	{
		var __retval = xmpir_mpz_fdiv_ui(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzFdivQ2exp(MpzT q, MpzT n, ulong b)
	{
		var __retval = xmpir_mpz_fdiv_q_2exp(q.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFdivR2exp(MpzT r, MpzT n, ulong b)
	{
		var __retval = xmpir_mpz_fdiv_r_2exp(r.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzTdivQ(MpzT q, MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_tdiv_q(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzTdivR(MpzT r, MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_tdiv_r(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzTdivQr(MpzT q, MpzT r, MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_tdiv_qr(q.val, r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzTdivQUi(MpzT q, MpzT n, uint d)
	{
		var __retval = xmpir_mpz_tdiv_q_ui(out var result, q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzTdivRUi(MpzT r, MpzT n, uint d)
	{
		var __retval = xmpir_mpz_tdiv_r_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzTdivQrUi(MpzT q, MpzT r, MpzT n, uint d)
	{
		var __retval = xmpir_mpz_tdiv_qr_ui(out var result, q.val, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzTdivUi(MpzT n, uint d)
	{
		var __retval = xmpir_mpz_tdiv_ui(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzTdivQ2exp(MpzT q, MpzT n, ulong b)
	{
		var __retval = xmpir_mpz_tdiv_q_2exp(q.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzTdivR2exp(MpzT r, MpzT n, ulong b)
	{
		var __retval = xmpir_mpz_tdiv_r_2exp(r.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzMod(MpzT r, MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_mod(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzModUi(MpzT r, MpzT n, uint d)
	{
		var __retval = xmpir_mpz_mod_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzDivexact(MpzT q, MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_divexact(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzDivexactUi(MpzT q, MpzT n, uint d)
	{
		var __retval = xmpir_mpz_divexact_ui(q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzDivisibleP(MpzT n, MpzT d)
	{
		var __retval = xmpir_mpz_divisible_p(out var result, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzDivisibleUiP(MpzT n, uint d)
	{
		var __retval = xmpir_mpz_divisible_ui_p(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzDivisible2expP(MpzT n, ulong b)
	{
		var __retval = xmpir_mpz_divisible_2exp_p(out var result, n.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCongruentP(MpzT n, MpzT c, MpzT d)
	{
		var __retval = xmpir_mpz_congruent_p(out var result, n.val, c.val, d.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCongruentUiP(MpzT n, uint c, uint d)
	{
		var __retval = xmpir_mpz_congruent_ui_p(out var result, n.val, c, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCongruent2expP(MpzT n, MpzT c, ulong b)
	{
		var __retval = xmpir_mpz_congruent_2exp_p(out var result, n.val, c.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzPowm(MpzT rop, MpzT Base, MpzT Exp, MpzT Mod)
	{
		var __retval = xmpir_mpz_powm(rop.val, Base.val, Exp.val, Mod.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzPowmUi(MpzT rop, MpzT Base, uint Exp, MpzT Mod)
	{
		var __retval = xmpir_mpz_powm_ui(rop.val, Base.val, Exp, Mod.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzPowUi(MpzT rop, MpzT Base, uint Exp)
	{
		var __retval = xmpir_mpz_pow_ui(rop.val, Base.val, Exp);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzUiPowUi(MpzT rop, uint Base, uint Exp)
	{
		var __retval = xmpir_mpz_ui_pow_ui(rop.val, Base, Exp);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzRoot(MpzT rop, MpzT op, uint n)
	{
		var __retval = xmpir_mpz_root(out var result, rop.val, op.val, n);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzRootrem(MpzT root, MpzT rem, MpzT u, uint n)
	{
		var __retval = xmpir_mpz_rootrem(root.val, rem.val, u.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSqrt(MpzT rop, MpzT op)
	{
		var __retval = xmpir_mpz_sqrt(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSqrtrem(MpzT rop1, MpzT rop2, MpzT op)
	{
		var __retval = xmpir_mpz_sqrtrem(rop1.val, rop2.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzPerfectPowerP(MpzT op)
	{
		var __retval = xmpir_mpz_perfect_power_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzPerfectSquareP(MpzT op)
	{
		var __retval = xmpir_mpz_perfect_square_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzProbabPrimeP(MpzT n, uint reps)
	{
		var __retval = xmpir_mpz_probab_prime_p(out var result, n.val, reps);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzNextprime(MpzT rop, MpzT op)
	{
		var __retval = xmpir_mpz_nextprime(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzGcd(MpzT rop, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_gcd(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzGcdUi(MpzT rop, MpzT op1, uint op2)
	{
		var __retval = xmpir_mpz_gcd_ui(out var result, rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzGcdext(MpzT g, MpzT s, MpzT t, MpzT a, MpzT b)
	{
		var __retval = xmpir_mpz_gcdext(g.val, s.val, t.val, a.val, b.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzLcm(MpzT rop, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_lcm(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzLcmUi(MpzT rop, MpzT op1, uint op2)
	{
		var __retval = xmpir_mpz_lcm_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzInvert(MpzT rop, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_invert(out var result, rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzJacobi(MpzT a, MpzT b)
	{
		var __retval = xmpir_mpz_jacobi(out var result, a.val, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzLegendre(MpzT a, MpzT p)
	{
		var __retval = xmpir_mpz_legendre(out var result, a.val, p.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzKronecker(MpzT a, MpzT b)
	{
		var __retval = xmpir_mpz_kronecker(out var result, a.val, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzKroneckerSi(MpzT a, int b)
	{
		var __retval = xmpir_mpz_kronecker_si(out var result, a.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzKroneckerUi(MpzT a, uint b)
	{
		var __retval = xmpir_mpz_kronecker_ui(out var result, a.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzSiKronecker(int a, MpzT b)
	{
		var __retval = xmpir_mpz_si_kronecker(out var result, a, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzUiKronecker(uint a, MpzT b)
	{
		var __retval = xmpir_mpz_ui_kronecker(out var result, a, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpzRemove(MpzT rop, MpzT op, MpzT f)
	{
		var __retval = xmpir_mpz_remove(out var result, rop.val, op.val, f.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzFacUi(MpzT rop, uint op)
	{
		var __retval = xmpir_mpz_fac_ui(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzBinUi(MpzT rop, MpzT n, uint k)
	{
		var __retval = xmpir_mpz_bin_ui(rop.val, n.val, k);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzBinUiui(MpzT rop, uint n, uint k)
	{
		var __retval = xmpir_mpz_bin_uiui(rop.val, n, k);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFibUi(MpzT fn, uint n)
	{
		var __retval = xmpir_mpz_fib_ui(fn.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFib2Ui(MpzT fn, MpzT fnsub1, uint n)
	{
		var __retval = xmpir_mpz_fib2_ui(fn.val, fnsub1.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzLucnumUi(MpzT ln, uint n)
	{
		var __retval = xmpir_mpz_lucnum_ui(ln.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzLucnum2Ui(MpzT ln, MpzT lnsub1, uint n)
	{
		var __retval = xmpir_mpz_lucnum2_ui(ln.val, lnsub1.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzCmp(MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_cmp(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpD(MpzT op1, double op2)
	{
		var __retval = xmpir_mpz_cmp_d(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpSi(MpzT op1, int op2)
	{
		var __retval = xmpir_mpz_cmp_si(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpUi(MpzT op1, uint op2)
	{
		var __retval = xmpir_mpz_cmp_ui(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpabs(MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_cmpabs(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpabsD(MpzT op1, double op2)
	{
		var __retval = xmpir_mpz_cmpabs_d(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpabsUi(MpzT op1, uint op2)
	{
		var __retval = xmpir_mpz_cmpabs_ui(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzSgn(MpzT op)
	{
		var __retval = xmpir_mpz_sgn(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzAnd(MpzT rop, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_and(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzIor(MpzT rop, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_ior(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzXor(MpzT rop, MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_xor(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCom(MpzT rop, MpzT op)
	{
		var __retval = xmpir_mpz_com(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static ulong MpzPopcount(MpzT op)
	{
		var __retval = xmpir_mpz_popcount(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpzHamdist(MpzT op1, MpzT op2)
	{
		var __retval = xmpir_mpz_hamdist(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpzScan0(MpzT op, ulong startingBit)
	{
		var __retval = xmpir_mpz_scan0(out var result, op.val, startingBit);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpzScan1(MpzT op, ulong startingBit)
	{
		var __retval = xmpir_mpz_scan1(out var result, op.val, startingBit);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzSetbit(MpzT rop, ulong bitIndex)
	{
		var __retval = xmpir_mpz_setbit(rop.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzClrbit(MpzT rop, ulong bitIndex)
	{
		var __retval = xmpir_mpz_clrbit(rop.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCombit(MpzT rop, ulong bitIndex)
	{
		var __retval = xmpir_mpz_combit(rop.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzTstbit(MpzT op, ulong bitIndex)
	{
		var __retval = xmpir_mpz_tstbit(out var result, op.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzUrandomb(MpzT rop, GmpRandstateT state, ulong n)
	{
		var __retval = xmpir_mpz_urandomb(rop.val, state.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzUrandomm(MpzT rop, GmpRandstateT state, MpzT n)
	{
		var __retval = xmpir_mpz_urandomm(rop.val, state.val, n.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzRrandomb(MpzT rop, GmpRandstateT state, ulong n)
	{
		var __retval = xmpir_mpz_rrandomb(rop.val, state.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzFitsUintP(MpzT op)
	{
		var __retval = xmpir_mpz_fits_uint_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzFitsSintP(MpzT op)
	{
		var __retval = xmpir_mpz_fits_sint_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzOddP(MpzT op)
	{
		var __retval = xmpir_mpz_odd_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzEvenP(MpzT op)
	{
		var __retval = xmpir_mpz_even_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzSizeinbase(MpzT op, uint Base)
	{
		var __retval = xmpir_mpz_sizeinbase(out var result, op.val, Base);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpqCanonicalize(MpqT op)
	{
		var __retval = xmpir_mpq_canonicalize(op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSet(MpqT rop, MpqT op)
	{
		var __retval = xmpir_mpq_set(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetZ(MpqT rop, MpzT op)
	{
		var __retval = xmpir_mpq_set_z(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetUi(MpqT rop, uint op1, uint op2)
	{
		var __retval = xmpir_mpq_set_ui(rop.val, op1, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetSi(MpqT rop, int op1, uint op2)
	{
		var __retval = xmpir_mpq_set_si(rop.val, op1, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpqSetStr(MpqT rop, string str, uint Base)
	{
		int __retval;
		var __ba_str = System.Text.Encoding.UTF8.GetBytes(str + "\0");
		__retval = xmpir_malloc(out var __str, str.Length + 1);
		if (__retval != 0) HandleError(__retval);
		Marshal.Copy(__ba_str, 0, __str, str.Length + 1);
		__retval = xmpir_mpq_set_str(out var result, rop.val, __str, Base);
		if (__retval != 0) HandleError(__retval);
		__retval = xmpir_free(__str);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpqSwap(MpqT rop1, MpqT rop2)
	{
		var __retval = xmpir_mpq_swap(rop1.val, rop2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static double MpqGetD(MpqT op)
	{
		var __retval = xmpir_mpq_get_d(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpqSetD(MpqT rop, double op)
	{
		var __retval = xmpir_mpq_set_d(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetF(MpqT rop, MpfT op)
	{
		var __retval = xmpir_mpq_set_f(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static string? MpqGetString(uint Base, MpqT op)
	{
		int __retval;
		string? result;
		__retval = xmpir_mpq_get_string(out var __result, Base, op.val);
		if (__retval != 0) HandleError(__retval);
		result = Marshal.PtrToStringAnsi(__result);
		__retval = xmpir_free(__result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpqAdd(MpqT sum, MpqT addend1, MpqT addend2)
	{
		var __retval = xmpir_mpq_add(sum.val, addend1.val, addend2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSub(MpqT difference, MpqT minuend, MpqT subtrahend)
	{
		var __retval = xmpir_mpq_sub(difference.val, minuend.val, subtrahend.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqMul(MpqT product, MpqT multiplier, MpqT multiplicand)
	{
		var __retval = xmpir_mpq_mul(product.val, multiplier.val, multiplicand.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqMul2exp(MpqT rop, MpqT op1, ulong op2)
	{
		var __retval = xmpir_mpq_mul_2exp(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqDiv(MpqT quotient, MpqT dividend, MpqT divisor)
	{
		var __retval = xmpir_mpq_div(quotient.val, dividend.val, divisor.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqDiv2exp(MpqT rop, MpqT op1, ulong op2)
	{
		var __retval = xmpir_mpq_div_2exp(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqNeg(MpqT negatedOperand, MpqT operand)
	{
		var __retval = xmpir_mpq_neg(negatedOperand.val, operand.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqAbs(MpqT rop, MpqT op)
	{
		var __retval = xmpir_mpq_abs(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqInv(MpqT invertedNumber, MpqT number)
	{
		var __retval = xmpir_mpq_inv(invertedNumber.val, number.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpqCmp(MpqT op1, MpqT op2)
	{
		var __retval = xmpir_mpq_cmp(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpqCmpUi(MpqT op1, uint num2, uint den2)
	{
		var __retval = xmpir_mpq_cmp_ui(out var result, op1.val, num2, den2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpqCmpSi(MpqT op1, int num2, uint den2)
	{
		var __retval = xmpir_mpq_cmp_si(out var result, op1.val, num2, den2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpqSgn(MpqT op)
	{
		var __retval = xmpir_mpq_sgn(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpqEqual(MpqT op1, MpqT op2)
	{
		var __retval = xmpir_mpq_equal(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpqGetNum(MpzT numerator, MpqT rational)
	{
		var __retval = xmpir_mpq_get_num(numerator.val, rational.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqGetDen(MpzT denominator, MpqT rational)
	{
		var __retval = xmpir_mpq_get_den(denominator.val, rational.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetNum(MpqT rational, MpzT numerator)
	{
		var __retval = xmpir_mpq_set_num(rational.val, numerator.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetDen(MpqT rational, MpzT denominator)
	{
		var __retval = xmpir_mpq_set_den(rational.val, denominator.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static ulong MpfGetPrec(MpfT op)
	{
		var __retval = xmpir_mpf_get_prec(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpfSetPrec(MpfT rop, ulong prec)
	{
		var __retval = xmpir_mpf_set_prec(rop.val, prec);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSet(MpfT rop, MpfT op)
	{
		var __retval = xmpir_mpf_set(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetUi(MpfT rop, uint op)
	{
		var __retval = xmpir_mpf_set_ui(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetSi(MpfT rop, int op)
	{
		var __retval = xmpir_mpf_set_si(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetD(MpfT rop, double op)
	{
		var __retval = xmpir_mpf_set_d(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetZ(MpfT rop, MpzT op)
	{
		var __retval = xmpir_mpf_set_z(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetQ(MpfT rop, MpqT op)
	{
		var __retval = xmpir_mpf_set_q(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpfSetStr(MpfT rop, string str, uint Base)
	{
		int __retval;
		var __ba_str = System.Text.Encoding.UTF8.GetBytes(str + "\0");
		__retval = xmpir_malloc(out var __str, str.Length + 1);
		if (__retval != 0) HandleError(__retval);
		Marshal.Copy(__ba_str, 0, __str, str.Length + 1);
		__retval = xmpir_mpf_set_str(out var result, rop.val, __str, Base);
		if (__retval != 0) HandleError(__retval);
		__retval = xmpir_free(__str);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpfSwap(MpfT rop1, MpfT rop2)
	{
		var __retval = xmpir_mpf_swap(rop1.val, rop2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static double MpfGetD(MpfT op)
	{
		var __retval = xmpir_mpf_get_d(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static double MpfGetD2exp(out long expptr, MpfT op)
	{
		var __retval = xmpir_mpf_get_d_2exp(out var result, out expptr, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfGetSi(MpfT op)
	{
		var __retval = xmpir_mpf_get_si(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpfGetUi(MpfT op)
	{
		var __retval = xmpir_mpf_get_ui(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static string? MpfGetString(out long expptr, uint Base, uint nDigits, MpfT op)
	{
		int __retval;
		string? result;
		__retval = xmpir_mpf_get_string(out var __result, out expptr, Base, nDigits, op.val);
		if (__retval != 0) HandleError(__retval);
		result = Marshal.PtrToStringAnsi(__result);
		__retval = xmpir_free(__result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpfAdd(MpfT rop, MpfT op1, MpfT op2)
	{
		var __retval = xmpir_mpf_add(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfAddUi(MpfT rop, MpfT op1, uint op2)
	{
		var __retval = xmpir_mpf_add_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSub(MpfT rop, MpfT op1, MpfT op2)
	{
		var __retval = xmpir_mpf_sub(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfUiSub(MpfT rop, uint op1, MpfT op2)
	{
		var __retval = xmpir_mpf_ui_sub(rop.val, op1, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSubUi(MpfT rop, MpfT op1, uint op2)
	{
		var __retval = xmpir_mpf_sub_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfMul(MpfT rop, MpfT op1, MpfT op2)
	{
		var __retval = xmpir_mpf_mul(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfMulUi(MpfT rop, MpfT op1, uint op2)
	{
		var __retval = xmpir_mpf_mul_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfDiv(MpfT rop, MpfT op1, MpfT op2)
	{
		var __retval = xmpir_mpf_div(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfUiDiv(MpfT rop, uint op1, MpfT op2)
	{
		var __retval = xmpir_mpf_ui_div(rop.val, op1, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfDivUi(MpfT rop, MpfT op1, uint op2)
	{
		var __retval = xmpir_mpf_div_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSqrt(MpfT rop, MpfT op)
	{
		var __retval = xmpir_mpf_sqrt(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSqrtUi(MpfT rop, uint op)
	{
		var __retval = xmpir_mpf_sqrt_ui(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfPowUi(MpfT rop, MpfT op1, uint op2)
	{
		var __retval = xmpir_mpf_pow_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfNeg(MpfT rop, MpfT op)
	{
		var __retval = xmpir_mpf_neg(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfAbs(MpfT rop, MpfT op)
	{
		var __retval = xmpir_mpf_abs(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfMul2exp(MpfT rop, MpfT op1, ulong op2)
	{
		var __retval = xmpir_mpf_mul_2exp(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfDiv2exp(MpfT rop, MpfT op1, ulong op2)
	{
		var __retval = xmpir_mpf_div_2exp(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpfCmp(MpfT op1, MpfT op2)
	{
		var __retval = xmpir_mpf_cmp(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfCmpD(MpfT op1, double op2)
	{
		var __retval = xmpir_mpf_cmp_d(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfCmpUi(MpfT op1, uint op2)
	{
		var __retval = xmpir_mpf_cmp_ui(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfCmpSi(MpfT op1, int op2)
	{
		var __retval = xmpir_mpf_cmp_si(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfEq(MpfT op1, MpfT op2, ulong op3)
	{
		var __retval = xmpir_mpf_eq(out var result, op1.val, op2.val, op3);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpfReldiff(MpfT rop, MpfT op1, MpfT op2)
	{
		var __retval = xmpir_mpf_reldiff(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpfSgn(MpfT op)
	{
		var __retval = xmpir_mpf_sgn(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpfCeil(MpfT rop, MpfT op)
	{
		var __retval = xmpir_mpf_ceil(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfFloor(MpfT rop, MpfT op)
	{
		var __retval = xmpir_mpf_floor(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfTrunc(MpfT rop, MpfT op)
	{
		var __retval = xmpir_mpf_trunc(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpfIntegerP(MpfT op)
	{
		var __retval = xmpir_mpf_integer_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfFitsUintP(MpfT op)
	{
		var __retval = xmpir_mpf_fits_uint_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfFitsSintP(MpfT op)
	{
		var __retval = xmpir_mpf_fits_sint_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpfUrandomb(MpfT rop, GmpRandstateT state, ulong nbits)
	{
		var __retval = xmpir_mpf_urandomb(rop.val, state.val, nbits);
		if (__retval != 0) HandleError(__retval);
	}
}
