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

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using mpz_intptr = nint;
using mpq_intptr = nint;
using mpf_intptr = nint;
using mpfr_intptr = nint;
using gmp_randstate_intptr = nint;

// Disable warning about missing XML comments.

namespace Mpir.NET;

public static partial class Mpir
{
	//
	// xMPIR library handle
	private static nint hxmpir = InitializeHxmpir();

	//
	// Automatically generated code: pointers to functions
	//
	private static readonly nint __ptr__xmpir_mpz_init = GetProcAddressSafe(hxmpir, "xmpir_mpz_init");
	private static readonly nint __ptr__xmpir_mpz_init2 = GetProcAddressSafe(hxmpir, "xmpir_mpz_init2");
	private static readonly nint __ptr__xmpir_mpz_init_set = GetProcAddressSafe(hxmpir, "xmpir_mpz_init_set");
	private static readonly nint __ptr__xmpir_mpz_init_set_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_init_set_ui");
	private static readonly nint __ptr__xmpir_mpz_init_set_si = GetProcAddressSafe(hxmpir, "xmpir_mpz_init_set_si");
	private static readonly nint __ptr__xmpir_mpz_init_set_d = GetProcAddressSafe(hxmpir, "xmpir_mpz_init_set_d");
	private static readonly nint __ptr__xmpir_mpz_init_set_str = GetProcAddressSafe(hxmpir, "xmpir_mpz_init_set_str");
	private static readonly nint __ptr__xmpir_mpq_init = GetProcAddressSafe(hxmpir, "xmpir_mpq_init");
	private static readonly nint __ptr__xmpir_mpf_init2 = GetProcAddressSafe(hxmpir, "xmpir_mpf_init2");
	private static readonly nint __ptr__xmpir_mpf_init_set = GetProcAddressSafe(hxmpir, "xmpir_mpf_init_set");
	private static readonly nint __ptr__xmpir_mpf_init_set_ui = GetProcAddressSafe(hxmpir, "xmpir_mpf_init_set_ui");
	private static readonly nint __ptr__xmpir_mpf_init_set_si = GetProcAddressSafe(hxmpir, "xmpir_mpf_init_set_si");
	private static readonly nint __ptr__xmpir_mpf_init_set_d = GetProcAddressSafe(hxmpir, "xmpir_mpf_init_set_d");
	private static readonly nint __ptr__xmpir_mpf_init_set_str = GetProcAddressSafe(hxmpir, "xmpir_mpf_init_set_str");
	private static readonly nint __ptr__xmpir_mpz_clear = GetProcAddressSafe(hxmpir, "xmpir_mpz_clear");
	private static readonly nint __ptr__xmpir_mpq_clear = GetProcAddressSafe(hxmpir, "xmpir_mpq_clear");
	private static readonly nint __ptr__xmpir_mpf_clear = GetProcAddressSafe(hxmpir, "xmpir_mpf_clear");
	private static readonly nint __ptr__xmpir_xmpir_dummy = GetProcAddressSafe(hxmpir, "xmpir_xmpir_dummy");
	private static readonly nint __ptr__xmpir_xmpir_dummy_add = GetProcAddressSafe(hxmpir, "xmpir_xmpir_dummy_add");
	private static readonly nint __ptr__xmpir_xmpir_dummy_3mpz = GetProcAddressSafe(hxmpir, "xmpir_xmpir_dummy_3mpz");
	private static readonly nint __ptr__xmpir_gmp_randinit_default = GetProcAddressSafe(hxmpir, "xmpir_gmp_randinit_default");
	private static readonly nint __ptr__xmpir_gmp_randinit_mt = GetProcAddressSafe(hxmpir, "xmpir_gmp_randinit_mt");
	private static readonly nint __ptr__xmpir_gmp_randinit_lc_2exp = GetProcAddressSafe(hxmpir, "xmpir_gmp_randinit_lc_2exp");
	private static readonly nint __ptr__xmpir_gmp_randinit_set = GetProcAddressSafe(hxmpir, "xmpir_gmp_randinit_set");
	private static readonly nint __ptr__xmpir_gmp_randclear = GetProcAddressSafe(hxmpir, "xmpir_gmp_randclear");
	private static readonly nint __ptr__xmpir_gmp_randseed = GetProcAddressSafe(hxmpir, "xmpir_gmp_randseed");
	private static readonly nint __ptr__xmpir_gmp_randseed_ui = GetProcAddressSafe(hxmpir, "xmpir_gmp_randseed_ui");
	private static readonly nint __ptr__xmpir_gmp_urandomb_ui = GetProcAddressSafe(hxmpir, "xmpir_gmp_urandomb_ui");
	private static readonly nint __ptr__xmpir_gmp_urandomm_ui = GetProcAddressSafe(hxmpir, "xmpir_gmp_urandomm_ui");
	private static readonly nint __ptr__xmpir_mpz_realloc2 = GetProcAddressSafe(hxmpir, "xmpir_mpz_realloc2");
	private static readonly nint __ptr__xmpir_mpf_set_default_prec = GetProcAddressSafe(hxmpir, "xmpir_mpf_set_default_prec");
	private static readonly nint __ptr__xmpir_mpf_get_default_prec = GetProcAddressSafe(hxmpir, "xmpir_mpf_get_default_prec");
	private static readonly nint __ptr__xmpir_mpz_set = GetProcAddressSafe(hxmpir, "xmpir_mpz_set");
	private static readonly nint __ptr__xmpir_mpz_set_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_set_ui");
	private static readonly nint __ptr__xmpir_mpz_set_si = GetProcAddressSafe(hxmpir, "xmpir_mpz_set_si");
	private static readonly nint __ptr__xmpir_mpz_set_d = GetProcAddressSafe(hxmpir, "xmpir_mpz_set_d");
	private static readonly nint __ptr__xmpir_mpz_set_q = GetProcAddressSafe(hxmpir, "xmpir_mpz_set_q");
	private static readonly nint __ptr__xmpir_mpz_set_f = GetProcAddressSafe(hxmpir, "xmpir_mpz_set_f");
	private static readonly nint __ptr__xmpir_mpz_set_str = GetProcAddressSafe(hxmpir, "xmpir_mpz_set_str");
	private static readonly nint __ptr__xmpir_mpz_swap = GetProcAddressSafe(hxmpir, "xmpir_mpz_swap");
	private static readonly nint __ptr__xmpir_mpz_get_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_get_ui");
	private static readonly nint __ptr__xmpir_mpz_get_si = GetProcAddressSafe(hxmpir, "xmpir_mpz_get_si");
	private static readonly nint __ptr__xmpir_mpz_get_d = GetProcAddressSafe(hxmpir, "xmpir_mpz_get_d");
	private static readonly nint __ptr__xmpir_mpz_get_string = GetProcAddressSafe(hxmpir, "xmpir_mpz_get_string");
	private static readonly nint __ptr__xmpir_mpz_add = GetProcAddressSafe(hxmpir, "xmpir_mpz_add");
	private static readonly nint __ptr__xmpir_mpz_add_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_add_ui");
	private static readonly nint __ptr__xmpir_mpz_sub = GetProcAddressSafe(hxmpir, "xmpir_mpz_sub");
	private static readonly nint __ptr__xmpir_mpz_sub_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_sub_ui");
	private static readonly nint __ptr__xmpir_mpz_ui_sub = GetProcAddressSafe(hxmpir, "xmpir_mpz_ui_sub");
	private static readonly nint __ptr__xmpir_mpz_mul = GetProcAddressSafe(hxmpir, "xmpir_mpz_mul");
	private static readonly nint __ptr__xmpir_mpz_mul_si = GetProcAddressSafe(hxmpir, "xmpir_mpz_mul_si");
	private static readonly nint __ptr__xmpir_mpz_mul_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_mul_ui");
	private static readonly nint __ptr__xmpir_mpz_addmul = GetProcAddressSafe(hxmpir, "xmpir_mpz_addmul");
	private static readonly nint __ptr__xmpir_mpz_addmul_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_addmul_ui");
	private static readonly nint __ptr__xmpir_mpz_submul = GetProcAddressSafe(hxmpir, "xmpir_mpz_submul");
	private static readonly nint __ptr__xmpir_mpz_submul_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_submul_ui");
	private static readonly nint __ptr__xmpir_mpz_mul_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpz_mul_2exp");
	private static readonly nint __ptr__xmpir_mpz_neg = GetProcAddressSafe(hxmpir, "xmpir_mpz_neg");
	private static readonly nint __ptr__xmpir_mpz_abs = GetProcAddressSafe(hxmpir, "xmpir_mpz_abs");
	private static readonly nint __ptr__xmpir_mpz_cdiv_q = GetProcAddressSafe(hxmpir, "xmpir_mpz_cdiv_q");
	private static readonly nint __ptr__xmpir_mpz_cdiv_r = GetProcAddressSafe(hxmpir, "xmpir_mpz_cdiv_r");
	private static readonly nint __ptr__xmpir_mpz_cdiv_qr = GetProcAddressSafe(hxmpir, "xmpir_mpz_cdiv_qr");
	private static readonly nint __ptr__xmpir_mpz_cdiv_q_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_cdiv_q_ui");
	private static readonly nint __ptr__xmpir_mpz_cdiv_r_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_cdiv_r_ui");
	private static readonly nint __ptr__xmpir_mpz_cdiv_qr_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_cdiv_qr_ui");
	private static readonly nint __ptr__xmpir_mpz_cdiv_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_cdiv_ui");
	private static readonly nint __ptr__xmpir_mpz_cdiv_q_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpz_cdiv_q_2exp");
	private static readonly nint __ptr__xmpir_mpz_cdiv_r_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpz_cdiv_r_2exp");
	private static readonly nint __ptr__xmpir_mpz_fdiv_q = GetProcAddressSafe(hxmpir, "xmpir_mpz_fdiv_q");
	private static readonly nint __ptr__xmpir_mpz_fdiv_r = GetProcAddressSafe(hxmpir, "xmpir_mpz_fdiv_r");
	private static readonly nint __ptr__xmpir_mpz_fdiv_qr = GetProcAddressSafe(hxmpir, "xmpir_mpz_fdiv_qr");
	private static readonly nint __ptr__xmpir_mpz_fdiv_q_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_fdiv_q_ui");
	private static readonly nint __ptr__xmpir_mpz_fdiv_r_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_fdiv_r_ui");
	private static readonly nint __ptr__xmpir_mpz_fdiv_qr_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_fdiv_qr_ui");
	private static readonly nint __ptr__xmpir_mpz_fdiv_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_fdiv_ui");
	private static readonly nint __ptr__xmpir_mpz_fdiv_q_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpz_fdiv_q_2exp");
	private static readonly nint __ptr__xmpir_mpz_fdiv_r_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpz_fdiv_r_2exp");
	private static readonly nint __ptr__xmpir_mpz_tdiv_q = GetProcAddressSafe(hxmpir, "xmpir_mpz_tdiv_q");
	private static readonly nint __ptr__xmpir_mpz_tdiv_r = GetProcAddressSafe(hxmpir, "xmpir_mpz_tdiv_r");
	private static readonly nint __ptr__xmpir_mpz_tdiv_qr = GetProcAddressSafe(hxmpir, "xmpir_mpz_tdiv_qr");
	private static readonly nint __ptr__xmpir_mpz_tdiv_q_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_tdiv_q_ui");
	private static readonly nint __ptr__xmpir_mpz_tdiv_r_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_tdiv_r_ui");
	private static readonly nint __ptr__xmpir_mpz_tdiv_qr_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_tdiv_qr_ui");
	private static readonly nint __ptr__xmpir_mpz_tdiv_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_tdiv_ui");
	private static readonly nint __ptr__xmpir_mpz_tdiv_q_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpz_tdiv_q_2exp");
	private static readonly nint __ptr__xmpir_mpz_tdiv_r_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpz_tdiv_r_2exp");
	private static readonly nint __ptr__xmpir_mpz_mod = GetProcAddressSafe(hxmpir, "xmpir_mpz_mod");
	private static readonly nint __ptr__xmpir_mpz_mod_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_mod_ui");
	private static readonly nint __ptr__xmpir_mpz_divexact = GetProcAddressSafe(hxmpir, "xmpir_mpz_divexact");
	private static readonly nint __ptr__xmpir_mpz_divexact_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_divexact_ui");
	private static readonly nint __ptr__xmpir_mpz_divisible_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_divisible_p");
	private static readonly nint __ptr__xmpir_mpz_divisible_ui_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_divisible_ui_p");
	private static readonly nint __ptr__xmpir_mpz_divisible_2exp_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_divisible_2exp_p");
	private static readonly nint __ptr__xmpir_mpz_congruent_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_congruent_p");
	private static readonly nint __ptr__xmpir_mpz_congruent_ui_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_congruent_ui_p");
	private static readonly nint __ptr__xmpir_mpz_congruent_2exp_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_congruent_2exp_p");
	private static readonly nint __ptr__xmpir_mpz_powm = GetProcAddressSafe(hxmpir, "xmpir_mpz_powm");
	private static readonly nint __ptr__xmpir_mpz_powm_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_powm_ui");
	private static readonly nint __ptr__xmpir_mpz_pow_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_pow_ui");
	private static readonly nint __ptr__xmpir_mpz_ui_pow_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_ui_pow_ui");
	private static readonly nint __ptr__xmpir_mpz_root = GetProcAddressSafe(hxmpir, "xmpir_mpz_root");
	private static readonly nint __ptr__xmpir_mpz_rootrem = GetProcAddressSafe(hxmpir, "xmpir_mpz_rootrem");
	private static readonly nint __ptr__xmpir_mpz_sqrt = GetProcAddressSafe(hxmpir, "xmpir_mpz_sqrt");
	private static readonly nint __ptr__xmpir_mpz_sqrtrem = GetProcAddressSafe(hxmpir, "xmpir_mpz_sqrtrem");
	private static readonly nint __ptr__xmpir_mpz_perfect_power_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_perfect_power_p");
	private static readonly nint __ptr__xmpir_mpz_perfect_square_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_perfect_square_p");
	private static readonly nint __ptr__xmpir_mpz_probab_prime_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_probab_prime_p");
	private static readonly nint __ptr__xmpir_mpz_nextprime = GetProcAddressSafe(hxmpir, "xmpir_mpz_nextprime");
	private static readonly nint __ptr__xmpir_mpz_gcd = GetProcAddressSafe(hxmpir, "xmpir_mpz_gcd");
	private static readonly nint __ptr__xmpir_mpz_gcd_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_gcd_ui");
	private static readonly nint __ptr__xmpir_mpz_gcdext = GetProcAddressSafe(hxmpir, "xmpir_mpz_gcdext");
	private static readonly nint __ptr__xmpir_mpz_lcm = GetProcAddressSafe(hxmpir, "xmpir_mpz_lcm");
	private static readonly nint __ptr__xmpir_mpz_lcm_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_lcm_ui");
	private static readonly nint __ptr__xmpir_mpz_invert = GetProcAddressSafe(hxmpir, "xmpir_mpz_invert");
	private static readonly nint __ptr__xmpir_mpz_jacobi = GetProcAddressSafe(hxmpir, "xmpir_mpz_jacobi");
	private static readonly nint __ptr__xmpir_mpz_legendre = GetProcAddressSafe(hxmpir, "xmpir_mpz_legendre");
	private static readonly nint __ptr__xmpir_mpz_kronecker = GetProcAddressSafe(hxmpir, "xmpir_mpz_kronecker");
	private static readonly nint __ptr__xmpir_mpz_kronecker_si = GetProcAddressSafe(hxmpir, "xmpir_mpz_kronecker_si");
	private static readonly nint __ptr__xmpir_mpz_kronecker_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_kronecker_ui");
	private static readonly nint __ptr__xmpir_mpz_si_kronecker = GetProcAddressSafe(hxmpir, "xmpir_mpz_si_kronecker");
	private static readonly nint __ptr__xmpir_mpz_ui_kronecker = GetProcAddressSafe(hxmpir, "xmpir_mpz_ui_kronecker");
	private static readonly nint __ptr__xmpir_mpz_remove = GetProcAddressSafe(hxmpir, "xmpir_mpz_remove");
	private static readonly nint __ptr__xmpir_mpz_fac_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_fac_ui");
	private static readonly nint __ptr__xmpir_mpz_bin_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_bin_ui");
	private static readonly nint __ptr__xmpir_mpz_bin_uiui = GetProcAddressSafe(hxmpir, "xmpir_mpz_bin_uiui");
	private static readonly nint __ptr__xmpir_mpz_fib_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_fib_ui");
	private static readonly nint __ptr__xmpir_mpz_fib2_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_fib2_ui");
	private static readonly nint __ptr__xmpir_mpz_lucnum_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_lucnum_ui");
	private static readonly nint __ptr__xmpir_mpz_lucnum2_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_lucnum2_ui");
	private static readonly nint __ptr__xmpir_mpz_cmp = GetProcAddressSafe(hxmpir, "xmpir_mpz_cmp");
	private static readonly nint __ptr__xmpir_mpz_cmp_d = GetProcAddressSafe(hxmpir, "xmpir_mpz_cmp_d");
	private static readonly nint __ptr__xmpir_mpz_cmp_si = GetProcAddressSafe(hxmpir, "xmpir_mpz_cmp_si");
	private static readonly nint __ptr__xmpir_mpz_cmp_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_cmp_ui");
	private static readonly nint __ptr__xmpir_mpz_cmpabs = GetProcAddressSafe(hxmpir, "xmpir_mpz_cmpabs");
	private static readonly nint __ptr__xmpir_mpz_cmpabs_d = GetProcAddressSafe(hxmpir, "xmpir_mpz_cmpabs_d");
	private static readonly nint __ptr__xmpir_mpz_cmpabs_ui = GetProcAddressSafe(hxmpir, "xmpir_mpz_cmpabs_ui");
	private static readonly nint __ptr__xmpir_mpz_sgn = GetProcAddressSafe(hxmpir, "xmpir_mpz_sgn");
	private static readonly nint __ptr__xmpir_mpz_and = GetProcAddressSafe(hxmpir, "xmpir_mpz_and");
	private static readonly nint __ptr__xmpir_mpz_ior = GetProcAddressSafe(hxmpir, "xmpir_mpz_ior");
	private static readonly nint __ptr__xmpir_mpz_xor = GetProcAddressSafe(hxmpir, "xmpir_mpz_xor");
	private static readonly nint __ptr__xmpir_mpz_com = GetProcAddressSafe(hxmpir, "xmpir_mpz_com");
	private static readonly nint __ptr__xmpir_mpz_popcount = GetProcAddressSafe(hxmpir, "xmpir_mpz_popcount");
	private static readonly nint __ptr__xmpir_mpz_hamdist = GetProcAddressSafe(hxmpir, "xmpir_mpz_hamdist");
	private static readonly nint __ptr__xmpir_mpz_scan0 = GetProcAddressSafe(hxmpir, "xmpir_mpz_scan0");
	private static readonly nint __ptr__xmpir_mpz_scan1 = GetProcAddressSafe(hxmpir, "xmpir_mpz_scan1");
	private static readonly nint __ptr__xmpir_mpz_setbit = GetProcAddressSafe(hxmpir, "xmpir_mpz_setbit");
	private static readonly nint __ptr__xmpir_mpz_clrbit = GetProcAddressSafe(hxmpir, "xmpir_mpz_clrbit");
	private static readonly nint __ptr__xmpir_mpz_combit = GetProcAddressSafe(hxmpir, "xmpir_mpz_combit");
	private static readonly nint __ptr__xmpir_mpz_tstbit = GetProcAddressSafe(hxmpir, "xmpir_mpz_tstbit");
	private static readonly nint __ptr__xmpir_mpz_urandomb = GetProcAddressSafe(hxmpir, "xmpir_mpz_urandomb");
	private static readonly nint __ptr__xmpir_mpz_urandomm = GetProcAddressSafe(hxmpir, "xmpir_mpz_urandomm");
	private static readonly nint __ptr__xmpir_mpz_rrandomb = GetProcAddressSafe(hxmpir, "xmpir_mpz_rrandomb");
	private static readonly nint __ptr__xmpir_mpz_fits_uint_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_fits_uint_p");
	private static readonly nint __ptr__xmpir_mpz_fits_sint_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_fits_sint_p");
	private static readonly nint __ptr__xmpir_mpz_odd_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_odd_p");
	private static readonly nint __ptr__xmpir_mpz_even_p = GetProcAddressSafe(hxmpir, "xmpir_mpz_even_p");
	private static readonly nint __ptr__xmpir_mpz_sizeinbase = GetProcAddressSafe(hxmpir, "xmpir_mpz_sizeinbase");
	private static readonly nint __ptr__xmpir_mpq_canonicalize = GetProcAddressSafe(hxmpir, "xmpir_mpq_canonicalize");
	private static readonly nint __ptr__xmpir_mpq_set = GetProcAddressSafe(hxmpir, "xmpir_mpq_set");
	private static readonly nint __ptr__xmpir_mpq_set_z = GetProcAddressSafe(hxmpir, "xmpir_mpq_set_z");
	private static readonly nint __ptr__xmpir_mpq_set_ui = GetProcAddressSafe(hxmpir, "xmpir_mpq_set_ui");
	private static readonly nint __ptr__xmpir_mpq_set_si = GetProcAddressSafe(hxmpir, "xmpir_mpq_set_si");
	private static readonly nint __ptr__xmpir_mpq_set_str = GetProcAddressSafe(hxmpir, "xmpir_mpq_set_str");
	private static readonly nint __ptr__xmpir_mpq_swap = GetProcAddressSafe(hxmpir, "xmpir_mpq_swap");
	private static readonly nint __ptr__xmpir_mpq_get_d = GetProcAddressSafe(hxmpir, "xmpir_mpq_get_d");
	private static readonly nint __ptr__xmpir_mpq_set_d = GetProcAddressSafe(hxmpir, "xmpir_mpq_set_d");
	private static readonly nint __ptr__xmpir_mpq_set_f = GetProcAddressSafe(hxmpir, "xmpir_mpq_set_f");
	private static readonly nint __ptr__xmpir_mpq_get_string = GetProcAddressSafe(hxmpir, "xmpir_mpq_get_string");
	private static readonly nint __ptr__xmpir_mpq_add = GetProcAddressSafe(hxmpir, "xmpir_mpq_add");
	private static readonly nint __ptr__xmpir_mpq_sub = GetProcAddressSafe(hxmpir, "xmpir_mpq_sub");
	private static readonly nint __ptr__xmpir_mpq_mul = GetProcAddressSafe(hxmpir, "xmpir_mpq_mul");
	private static readonly nint __ptr__xmpir_mpq_mul_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpq_mul_2exp");
	private static readonly nint __ptr__xmpir_mpq_div = GetProcAddressSafe(hxmpir, "xmpir_mpq_div");
	private static readonly nint __ptr__xmpir_mpq_div_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpq_div_2exp");
	private static readonly nint __ptr__xmpir_mpq_neg = GetProcAddressSafe(hxmpir, "xmpir_mpq_neg");
	private static readonly nint __ptr__xmpir_mpq_abs = GetProcAddressSafe(hxmpir, "xmpir_mpq_abs");
	private static readonly nint __ptr__xmpir_mpq_inv = GetProcAddressSafe(hxmpir, "xmpir_mpq_inv");
	private static readonly nint __ptr__xmpir_mpq_cmp = GetProcAddressSafe(hxmpir, "xmpir_mpq_cmp");
	private static readonly nint __ptr__xmpir_mpq_cmp_ui = GetProcAddressSafe(hxmpir, "xmpir_mpq_cmp_ui");
	private static readonly nint __ptr__xmpir_mpq_cmp_si = GetProcAddressSafe(hxmpir, "xmpir_mpq_cmp_si");
	private static readonly nint __ptr__xmpir_mpq_sgn = GetProcAddressSafe(hxmpir, "xmpir_mpq_sgn");
	private static readonly nint __ptr__xmpir_mpq_equal = GetProcAddressSafe(hxmpir, "xmpir_mpq_equal");
	private static readonly nint __ptr__xmpir_mpq_get_num = GetProcAddressSafe(hxmpir, "xmpir_mpq_get_num");
	private static readonly nint __ptr__xmpir_mpq_get_den = GetProcAddressSafe(hxmpir, "xmpir_mpq_get_den");
	private static readonly nint __ptr__xmpir_mpq_set_num = GetProcAddressSafe(hxmpir, "xmpir_mpq_set_num");
	private static readonly nint __ptr__xmpir_mpq_set_den = GetProcAddressSafe(hxmpir, "xmpir_mpq_set_den");
	private static readonly nint __ptr__xmpir_mpf_get_prec = GetProcAddressSafe(hxmpir, "xmpir_mpf_get_prec");
	private static readonly nint __ptr__xmpir_mpf_set_prec = GetProcAddressSafe(hxmpir, "xmpir_mpf_set_prec");
	private static readonly nint __ptr__xmpir_mpf_set = GetProcAddressSafe(hxmpir, "xmpir_mpf_set");
	private static readonly nint __ptr__xmpir_mpf_set_ui = GetProcAddressSafe(hxmpir, "xmpir_mpf_set_ui");
	private static readonly nint __ptr__xmpir_mpf_set_si = GetProcAddressSafe(hxmpir, "xmpir_mpf_set_si");
	private static readonly nint __ptr__xmpir_mpf_set_d = GetProcAddressSafe(hxmpir, "xmpir_mpf_set_d");
	private static readonly nint __ptr__xmpir_mpf_set_z = GetProcAddressSafe(hxmpir, "xmpir_mpf_set_z");
	private static readonly nint __ptr__xmpir_mpf_set_q = GetProcAddressSafe(hxmpir, "xmpir_mpf_set_q");
	private static readonly nint __ptr__xmpir_mpf_set_str = GetProcAddressSafe(hxmpir, "xmpir_mpf_set_str");
	private static readonly nint __ptr__xmpir_mpf_swap = GetProcAddressSafe(hxmpir, "xmpir_mpf_swap");
	private static readonly nint __ptr__xmpir_mpf_get_d = GetProcAddressSafe(hxmpir, "xmpir_mpf_get_d");
	private static readonly nint __ptr__xmpir_mpf_get_d_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpf_get_d_2exp");
	private static readonly nint __ptr__xmpir_mpf_get_si = GetProcAddressSafe(hxmpir, "xmpir_mpf_get_si");
	private static readonly nint __ptr__xmpir_mpf_get_ui = GetProcAddressSafe(hxmpir, "xmpir_mpf_get_ui");
	private static readonly nint __ptr__xmpir_mpf_get_string = GetProcAddressSafe(hxmpir, "xmpir_mpf_get_string");
	private static readonly nint __ptr__xmpir_mpf_add = GetProcAddressSafe(hxmpir, "xmpir_mpf_add");
	private static readonly nint __ptr__xmpir_mpf_add_ui = GetProcAddressSafe(hxmpir, "xmpir_mpf_add_ui");
	private static readonly nint __ptr__xmpir_mpf_sub = GetProcAddressSafe(hxmpir, "xmpir_mpf_sub");
	private static readonly nint __ptr__xmpir_mpf_ui_sub = GetProcAddressSafe(hxmpir, "xmpir_mpf_ui_sub");
	private static readonly nint __ptr__xmpir_mpf_sub_ui = GetProcAddressSafe(hxmpir, "xmpir_mpf_sub_ui");
	private static readonly nint __ptr__xmpir_mpf_mul = GetProcAddressSafe(hxmpir, "xmpir_mpf_mul");
	private static readonly nint __ptr__xmpir_mpf_mul_ui = GetProcAddressSafe(hxmpir, "xmpir_mpf_mul_ui");
	private static readonly nint __ptr__xmpir_mpf_div = GetProcAddressSafe(hxmpir, "xmpir_mpf_div");
	private static readonly nint __ptr__xmpir_mpf_ui_div = GetProcAddressSafe(hxmpir, "xmpir_mpf_ui_div");
	private static readonly nint __ptr__xmpir_mpf_div_ui = GetProcAddressSafe(hxmpir, "xmpir_mpf_div_ui");
	private static readonly nint __ptr__xmpir_mpf_sqrt = GetProcAddressSafe(hxmpir, "xmpir_mpf_sqrt");
	private static readonly nint __ptr__xmpir_mpf_sqrt_ui = GetProcAddressSafe(hxmpir, "xmpir_mpf_sqrt_ui");
	private static readonly nint __ptr__xmpir_mpf_pow_ui = GetProcAddressSafe(hxmpir, "xmpir_mpf_pow_ui");
	private static readonly nint __ptr__xmpir_mpf_neg = GetProcAddressSafe(hxmpir, "xmpir_mpf_neg");
	private static readonly nint __ptr__xmpir_mpf_abs = GetProcAddressSafe(hxmpir, "xmpir_mpf_abs");
	private static readonly nint __ptr__xmpir_mpf_mul_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpf_mul_2exp");
	private static readonly nint __ptr__xmpir_mpf_div_2exp = GetProcAddressSafe(hxmpir, "xmpir_mpf_div_2exp");
	private static readonly nint __ptr__xmpir_mpf_cmp = GetProcAddressSafe(hxmpir, "xmpir_mpf_cmp");
	private static readonly nint __ptr__xmpir_mpf_cmp_d = GetProcAddressSafe(hxmpir, "xmpir_mpf_cmp_d");
	private static readonly nint __ptr__xmpir_mpf_cmp_ui = GetProcAddressSafe(hxmpir, "xmpir_mpf_cmp_ui");
	private static readonly nint __ptr__xmpir_mpf_cmp_si = GetProcAddressSafe(hxmpir, "xmpir_mpf_cmp_si");
	private static readonly nint __ptr__xmpir_mpf_eq = GetProcAddressSafe(hxmpir, "xmpir_mpf_eq");
	private static readonly nint __ptr__xmpir_mpf_reldiff = GetProcAddressSafe(hxmpir, "xmpir_mpf_reldiff");
	private static readonly nint __ptr__xmpir_mpf_sgn = GetProcAddressSafe(hxmpir, "xmpir_mpf_sgn");
	private static readonly nint __ptr__xmpir_mpf_ceil = GetProcAddressSafe(hxmpir, "xmpir_mpf_ceil");
	private static readonly nint __ptr__xmpir_mpf_floor = GetProcAddressSafe(hxmpir, "xmpir_mpf_floor");
	private static readonly nint __ptr__xmpir_mpf_trunc = GetProcAddressSafe(hxmpir, "xmpir_mpf_trunc");
	private static readonly nint __ptr__xmpir_mpf_integer_p = GetProcAddressSafe(hxmpir, "xmpir_mpf_integer_p");
	private static readonly nint __ptr__xmpir_mpf_fits_uint_p = GetProcAddressSafe(hxmpir, "xmpir_mpf_fits_uint_p");
	private static readonly nint __ptr__xmpir_mpf_fits_sint_p = GetProcAddressSafe(hxmpir, "xmpir_mpf_fits_sint_p");
	private static readonly nint __ptr__xmpir_mpf_urandomb = GetProcAddressSafe(hxmpir, "xmpir_mpf_urandomb");

	//
	// Automatically generated code: definitions
	//
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_init(out nint result);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_init2(out nint result, ulong n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_init_set(out nint result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_init_set_ui(out nint result, uint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_init_set_si(out nint result, int op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_init_set_d(out nint result, double op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_init_set_str(out nint result, nint str, uint _base);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_init(out nint result);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_init2(out nint result, uint prec);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_init_set(out nint result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_init_set_ui(out nint result, uint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_init_set_si(out nint result, int op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_init_set_d(out nint result, double op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_init_set_str(out nint result, nint str, uint _base);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_clear(nint v);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_clear(nint v);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_clear(nint v);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_xmpir_dummy();
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_xmpir_dummy_add(out int result, int a, int b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_xmpir_dummy_3mpz(out int result, nint op0, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_gmp_randinit_default(out nint result);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_gmp_randinit_mt(out nint result);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_gmp_randinit_lc_2exp(out nint result, nint a, uint c, ulong m2exp);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_gmp_randinit_set(out nint result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_gmp_randclear(nint v);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_gmp_randseed(nint state, nint seed);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_gmp_randseed_ui(nint state, uint seed);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_gmp_urandomb_ui(out uint result, nint state, uint n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_gmp_urandomm_ui(out uint result, nint state, uint n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_realloc2(nint x, uint n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_set_default_prec(ulong prec);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_get_default_prec(out ulong result);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_set(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_set_ui(nint rop, uint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_set_si(nint rop, int op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_set_d(nint rop, double op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_set_q(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_set_f(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_set_str(out int result, nint rop, nint str, uint _base);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_swap(nint rop1, nint rop2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_get_ui(out uint result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_get_si(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_get_d(out double result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_get_string(out nint result, uint _base, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_add(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_add_ui(nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_sub(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_sub_ui(nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_ui_sub(nint rop, uint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_mul(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_mul_si(nint rop, nint op1, int op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_mul_ui(nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_addmul(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_addmul_ui(nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_submul(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_submul_ui(nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_mul_2exp(nint rop, nint op1, ulong op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_neg(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_abs(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cdiv_q(nint q, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cdiv_r(nint r, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cdiv_qr(nint q, nint r, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cdiv_q_ui(out uint result, nint q, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cdiv_r_ui(out uint result, nint r, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cdiv_qr_ui(out uint result, nint q, nint r, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cdiv_ui(out uint result, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cdiv_q_2exp(nint q, nint n, ulong b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cdiv_r_2exp(nint r, nint n, ulong b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fdiv_q(nint q, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fdiv_r(nint r, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fdiv_qr(nint q, nint r, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fdiv_q_ui(out uint result, nint q, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fdiv_r_ui(out uint result, nint r, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fdiv_qr_ui(out uint result, nint q, nint r, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fdiv_ui(out uint result, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fdiv_q_2exp(nint q, nint n, ulong b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fdiv_r_2exp(nint r, nint n, ulong b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_tdiv_q(nint q, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_tdiv_r(nint r, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_tdiv_qr(nint q, nint r, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_tdiv_q_ui(out uint result, nint q, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_tdiv_r_ui(out uint result, nint r, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_tdiv_qr_ui(out uint result, nint q, nint r, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_tdiv_ui(out uint result, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_tdiv_q_2exp(nint q, nint n, ulong b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_tdiv_r_2exp(nint r, nint n, ulong b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_mod(nint r, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_mod_ui(out uint result, nint r, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_divexact(nint q, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_divexact_ui(nint q, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_divisible_p(out int result, nint n, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_divisible_ui_p(out int result, nint n, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_divisible_2exp_p(out int result, nint n, ulong b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_congruent_p(out int result, nint n, nint c, nint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_congruent_ui_p(out int result, nint n, uint c, uint d);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_congruent_2exp_p(out int result, nint n, nint c, ulong b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_powm(nint rop, nint _base, nint _exp, nint _mod);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_powm_ui(nint rop, nint _base, uint _exp, nint _mod);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_pow_ui(nint rop, nint _base, uint _exp);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_ui_pow_ui(nint rop, uint _base, uint _exp);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_root(out int result, nint rop, nint op, uint n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_rootrem(nint root, nint rem, nint u, uint n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_sqrt(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_sqrtrem(nint rop1, nint rop2, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_perfect_power_p(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_perfect_square_p(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_probab_prime_p(out int result, nint n, uint reps);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_nextprime(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_gcd(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_gcd_ui(out uint result, nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_gcdext(nint g, nint s, nint t, nint a, nint b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_lcm(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_lcm_ui(nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_invert(out int result, nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_jacobi(out int result, nint a, nint b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_legendre(out int result, nint a, nint p);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_kronecker(out int result, nint a, nint b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_kronecker_si(out int result, nint a, int b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_kronecker_ui(out int result, nint a, uint b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_si_kronecker(out int result, int a, nint b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_ui_kronecker(out int result, uint a, nint b);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_remove(out ulong result, nint rop, nint op, nint f);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fac_ui(nint rop, uint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_bin_ui(nint rop, nint n, uint k);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_bin_uiui(nint rop, uint n, uint k);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fib_ui(nint fn, uint n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fib2_ui(nint fn, nint fnsub1, uint n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_lucnum_ui(nint ln, uint n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_lucnum2_ui(nint ln, nint lnsub1, uint n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cmp(out int result, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cmp_d(out int result, nint op1, double op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cmp_si(out int result, nint op1, int op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cmp_ui(out int result, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cmpabs(out int result, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cmpabs_d(out int result, nint op1, double op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_cmpabs_ui(out int result, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_sgn(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_and(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_ior(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_xor(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_com(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_popcount(out ulong result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_hamdist(out ulong result, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_scan0(out ulong result, nint op, ulong starting_bit);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_scan1(out ulong result, nint op, ulong starting_bit);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_setbit(nint rop, ulong bit_index);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_clrbit(nint rop, ulong bit_index);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_combit(nint rop, ulong bit_index);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_tstbit(out int result, nint op, ulong bit_index);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_urandomb(nint rop, nint state, ulong n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_urandomm(nint rop, nint state, nint n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_rrandomb(nint rop, nint state, ulong n);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fits_uint_p(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_fits_sint_p(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_odd_p(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_even_p(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpz_sizeinbase(out uint result, nint op, uint _base);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_canonicalize(nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_set(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_set_z(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_set_ui(nint rop, uint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_set_si(nint rop, int op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_set_str(out int result, nint rop, nint str, uint _base);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_swap(nint rop1, nint rop2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_get_d(out double result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_set_d(nint rop, double op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_set_f(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_get_string(out nint result, uint _base, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_add(nint sum, nint addend1, nint addend2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_sub(nint difference, nint minuend, nint subtrahend);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_mul(nint product, nint multiplier, nint multiplicand);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_mul_2exp(nint rop, nint op1, ulong op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_div(nint quotient, nint dividend, nint divisor);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_div_2exp(nint rop, nint op1, ulong op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_neg(nint negated_operand, nint operand);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_abs(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_inv(nint inverted_number, nint number);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_cmp(out int result, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_cmp_ui(out int result, nint op1, uint num2, uint den2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_cmp_si(out int result, nint op1, int num2, uint den2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_sgn(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_equal(out int result, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_get_num(nint numerator, nint rational);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_get_den(nint denominator, nint rational);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_set_num(nint rational, nint numerator);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpq_set_den(nint rational, nint denominator);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_get_prec(out ulong result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_set_prec(nint rop, ulong prec);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_set(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_set_ui(nint rop, uint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_set_si(nint rop, int op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_set_d(nint rop, double op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_set_z(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_set_q(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_set_str(out int result, nint rop, nint str, uint _base);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_swap(nint rop1, nint rop2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_get_d(out double result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_get_d_2exp(out double result, out long expptr, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_get_si(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_get_ui(out uint result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_get_string(out nint result, out long expptr, uint _base, uint n_digits, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_add(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_add_ui(nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_sub(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_ui_sub(nint rop, uint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_sub_ui(nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_mul(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_mul_ui(nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_div(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_ui_div(nint rop, uint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_div_ui(nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_sqrt(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_sqrt_ui(nint rop, uint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_pow_ui(nint rop, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_neg(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_abs(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_mul_2exp(nint rop, nint op1, ulong op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_div_2exp(nint rop, nint op1, ulong op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_cmp(out int result, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_cmp_d(out int result, nint op1, double op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_cmp_ui(out int result, nint op1, uint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_cmp_si(out int result, nint op1, int op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_eq(out int result, nint op1, nint op2, ulong op3);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_reldiff(nint rop, nint op1, nint op2);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_sgn(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_ceil(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_floor(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_trunc(nint rop, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_integer_p(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_fits_uint_p(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_fits_sint_p(out int result, nint op);
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int __xmpir_mpf_urandomb(nint rop, nint state, ulong nbits);

	//
	// Automatically generated code: delegates
	//
	private static readonly __xmpir_mpz_init xmpir_mpz_init = (__xmpir_mpz_init)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_init, typeof(__xmpir_mpz_init));
	private static readonly __xmpir_mpz_init2 xmpir_mpz_init2 = (__xmpir_mpz_init2)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_init2, typeof(__xmpir_mpz_init2));
	private static readonly __xmpir_mpz_init_set xmpir_mpz_init_set = (__xmpir_mpz_init_set)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_init_set, typeof(__xmpir_mpz_init_set));
	private static readonly __xmpir_mpz_init_set_ui xmpir_mpz_init_set_ui = (__xmpir_mpz_init_set_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_init_set_ui, typeof(__xmpir_mpz_init_set_ui));
	private static readonly __xmpir_mpz_init_set_si xmpir_mpz_init_set_si = (__xmpir_mpz_init_set_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_init_set_si, typeof(__xmpir_mpz_init_set_si));
	private static readonly __xmpir_mpz_init_set_d xmpir_mpz_init_set_d = (__xmpir_mpz_init_set_d)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_init_set_d, typeof(__xmpir_mpz_init_set_d));
	private static readonly __xmpir_mpz_init_set_str xmpir_mpz_init_set_str = (__xmpir_mpz_init_set_str)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_init_set_str, typeof(__xmpir_mpz_init_set_str));
	private static readonly __xmpir_mpq_init xmpir_mpq_init = (__xmpir_mpq_init)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_init, typeof(__xmpir_mpq_init));
	private static readonly __xmpir_mpf_init2 xmpir_mpf_init2 = (__xmpir_mpf_init2)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_init2, typeof(__xmpir_mpf_init2));
	private static readonly __xmpir_mpf_init_set xmpir_mpf_init_set = (__xmpir_mpf_init_set)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_init_set, typeof(__xmpir_mpf_init_set));
	private static readonly __xmpir_mpf_init_set_ui xmpir_mpf_init_set_ui = (__xmpir_mpf_init_set_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_init_set_ui, typeof(__xmpir_mpf_init_set_ui));
	private static readonly __xmpir_mpf_init_set_si xmpir_mpf_init_set_si = (__xmpir_mpf_init_set_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_init_set_si, typeof(__xmpir_mpf_init_set_si));
	private static readonly __xmpir_mpf_init_set_d xmpir_mpf_init_set_d = (__xmpir_mpf_init_set_d)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_init_set_d, typeof(__xmpir_mpf_init_set_d));
	private static readonly __xmpir_mpf_init_set_str xmpir_mpf_init_set_str = (__xmpir_mpf_init_set_str)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_init_set_str, typeof(__xmpir_mpf_init_set_str));
	private static readonly __xmpir_mpz_clear xmpir_mpz_clear = (__xmpir_mpz_clear)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_clear, typeof(__xmpir_mpz_clear));
	private static readonly __xmpir_mpq_clear xmpir_mpq_clear = (__xmpir_mpq_clear)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_clear, typeof(__xmpir_mpq_clear));
	private static readonly __xmpir_mpf_clear xmpir_mpf_clear = (__xmpir_mpf_clear)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_clear, typeof(__xmpir_mpf_clear));
	private static readonly __xmpir_xmpir_dummy xmpir_xmpir_dummy = (__xmpir_xmpir_dummy)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_xmpir_dummy, typeof(__xmpir_xmpir_dummy));
	private static readonly __xmpir_xmpir_dummy_add xmpir_xmpir_dummy_add = (__xmpir_xmpir_dummy_add)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_xmpir_dummy_add, typeof(__xmpir_xmpir_dummy_add));
	private static readonly __xmpir_xmpir_dummy_3mpz xmpir_xmpir_dummy_3mpz = (__xmpir_xmpir_dummy_3mpz)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_xmpir_dummy_3mpz, typeof(__xmpir_xmpir_dummy_3mpz));
	private static readonly __xmpir_gmp_randinit_default xmpir_gmp_randinit_default = (__xmpir_gmp_randinit_default)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_gmp_randinit_default, typeof(__xmpir_gmp_randinit_default));
	private static readonly __xmpir_gmp_randinit_mt xmpir_gmp_randinit_mt = (__xmpir_gmp_randinit_mt)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_gmp_randinit_mt, typeof(__xmpir_gmp_randinit_mt));
	private static readonly __xmpir_gmp_randinit_lc_2exp xmpir_gmp_randinit_lc_2exp = (__xmpir_gmp_randinit_lc_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_gmp_randinit_lc_2exp, typeof(__xmpir_gmp_randinit_lc_2exp));
	private static readonly __xmpir_gmp_randinit_set xmpir_gmp_randinit_set = (__xmpir_gmp_randinit_set)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_gmp_randinit_set, typeof(__xmpir_gmp_randinit_set));
	private static readonly __xmpir_gmp_randclear xmpir_gmp_randclear = (__xmpir_gmp_randclear)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_gmp_randclear, typeof(__xmpir_gmp_randclear));
	private static readonly __xmpir_gmp_randseed xmpir_gmp_randseed = (__xmpir_gmp_randseed)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_gmp_randseed, typeof(__xmpir_gmp_randseed));
	private static readonly __xmpir_gmp_randseed_ui xmpir_gmp_randseed_ui = (__xmpir_gmp_randseed_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_gmp_randseed_ui, typeof(__xmpir_gmp_randseed_ui));
	private static readonly __xmpir_gmp_urandomb_ui xmpir_gmp_urandomb_ui = (__xmpir_gmp_urandomb_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_gmp_urandomb_ui, typeof(__xmpir_gmp_urandomb_ui));
	private static readonly __xmpir_gmp_urandomm_ui xmpir_gmp_urandomm_ui = (__xmpir_gmp_urandomm_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_gmp_urandomm_ui, typeof(__xmpir_gmp_urandomm_ui));
	private static readonly __xmpir_mpz_realloc2 xmpir_mpz_realloc2 = (__xmpir_mpz_realloc2)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_realloc2, typeof(__xmpir_mpz_realloc2));
	private static readonly __xmpir_mpf_set_default_prec xmpir_mpf_set_default_prec = (__xmpir_mpf_set_default_prec)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_set_default_prec, typeof(__xmpir_mpf_set_default_prec));
	private static readonly __xmpir_mpf_get_default_prec xmpir_mpf_get_default_prec = (__xmpir_mpf_get_default_prec)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_get_default_prec, typeof(__xmpir_mpf_get_default_prec));
	private static readonly __xmpir_mpz_set xmpir_mpz_set = (__xmpir_mpz_set)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_set, typeof(__xmpir_mpz_set));
	private static readonly __xmpir_mpz_set_ui xmpir_mpz_set_ui = (__xmpir_mpz_set_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_set_ui, typeof(__xmpir_mpz_set_ui));
	private static readonly __xmpir_mpz_set_si xmpir_mpz_set_si = (__xmpir_mpz_set_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_set_si, typeof(__xmpir_mpz_set_si));
	private static readonly __xmpir_mpz_set_d xmpir_mpz_set_d = (__xmpir_mpz_set_d)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_set_d, typeof(__xmpir_mpz_set_d));
	private static readonly __xmpir_mpz_set_q xmpir_mpz_set_q = (__xmpir_mpz_set_q)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_set_q, typeof(__xmpir_mpz_set_q));
	private static readonly __xmpir_mpz_set_f xmpir_mpz_set_f = (__xmpir_mpz_set_f)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_set_f, typeof(__xmpir_mpz_set_f));
	private static readonly __xmpir_mpz_set_str xmpir_mpz_set_str = (__xmpir_mpz_set_str)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_set_str, typeof(__xmpir_mpz_set_str));
	private static readonly __xmpir_mpz_swap xmpir_mpz_swap = (__xmpir_mpz_swap)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_swap, typeof(__xmpir_mpz_swap));
	private static readonly __xmpir_mpz_get_ui xmpir_mpz_get_ui = (__xmpir_mpz_get_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_get_ui, typeof(__xmpir_mpz_get_ui));
	private static readonly __xmpir_mpz_get_si xmpir_mpz_get_si = (__xmpir_mpz_get_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_get_si, typeof(__xmpir_mpz_get_si));
	private static readonly __xmpir_mpz_get_d xmpir_mpz_get_d = (__xmpir_mpz_get_d)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_get_d, typeof(__xmpir_mpz_get_d));
	private static readonly __xmpir_mpz_get_string xmpir_mpz_get_string = (__xmpir_mpz_get_string)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_get_string, typeof(__xmpir_mpz_get_string));
	private static readonly __xmpir_mpz_add xmpir_mpz_add = (__xmpir_mpz_add)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_add, typeof(__xmpir_mpz_add));
	private static readonly __xmpir_mpz_add_ui xmpir_mpz_add_ui = (__xmpir_mpz_add_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_add_ui, typeof(__xmpir_mpz_add_ui));
	private static readonly __xmpir_mpz_sub xmpir_mpz_sub = (__xmpir_mpz_sub)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_sub, typeof(__xmpir_mpz_sub));
	private static readonly __xmpir_mpz_sub_ui xmpir_mpz_sub_ui = (__xmpir_mpz_sub_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_sub_ui, typeof(__xmpir_mpz_sub_ui));
	private static readonly __xmpir_mpz_ui_sub xmpir_mpz_ui_sub = (__xmpir_mpz_ui_sub)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_ui_sub, typeof(__xmpir_mpz_ui_sub));
	private static readonly __xmpir_mpz_mul xmpir_mpz_mul = (__xmpir_mpz_mul)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_mul, typeof(__xmpir_mpz_mul));
	private static readonly __xmpir_mpz_mul_si xmpir_mpz_mul_si = (__xmpir_mpz_mul_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_mul_si, typeof(__xmpir_mpz_mul_si));
	private static readonly __xmpir_mpz_mul_ui xmpir_mpz_mul_ui = (__xmpir_mpz_mul_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_mul_ui, typeof(__xmpir_mpz_mul_ui));
	private static readonly __xmpir_mpz_addmul xmpir_mpz_addmul = (__xmpir_mpz_addmul)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_addmul, typeof(__xmpir_mpz_addmul));
	private static readonly __xmpir_mpz_addmul_ui xmpir_mpz_addmul_ui = (__xmpir_mpz_addmul_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_addmul_ui, typeof(__xmpir_mpz_addmul_ui));
	private static readonly __xmpir_mpz_submul xmpir_mpz_submul = (__xmpir_mpz_submul)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_submul, typeof(__xmpir_mpz_submul));
	private static readonly __xmpir_mpz_submul_ui xmpir_mpz_submul_ui = (__xmpir_mpz_submul_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_submul_ui, typeof(__xmpir_mpz_submul_ui));
	private static readonly __xmpir_mpz_mul_2exp xmpir_mpz_mul_2exp = (__xmpir_mpz_mul_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_mul_2exp, typeof(__xmpir_mpz_mul_2exp));
	private static readonly __xmpir_mpz_neg xmpir_mpz_neg = (__xmpir_mpz_neg)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_neg, typeof(__xmpir_mpz_neg));
	private static readonly __xmpir_mpz_abs xmpir_mpz_abs = (__xmpir_mpz_abs)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_abs, typeof(__xmpir_mpz_abs));
	private static readonly __xmpir_mpz_cdiv_q xmpir_mpz_cdiv_q = (__xmpir_mpz_cdiv_q)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cdiv_q, typeof(__xmpir_mpz_cdiv_q));
	private static readonly __xmpir_mpz_cdiv_r xmpir_mpz_cdiv_r = (__xmpir_mpz_cdiv_r)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cdiv_r, typeof(__xmpir_mpz_cdiv_r));
	private static readonly __xmpir_mpz_cdiv_qr xmpir_mpz_cdiv_qr = (__xmpir_mpz_cdiv_qr)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cdiv_qr, typeof(__xmpir_mpz_cdiv_qr));
	private static readonly __xmpir_mpz_cdiv_q_ui xmpir_mpz_cdiv_q_ui = (__xmpir_mpz_cdiv_q_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cdiv_q_ui, typeof(__xmpir_mpz_cdiv_q_ui));
	private static readonly __xmpir_mpz_cdiv_r_ui xmpir_mpz_cdiv_r_ui = (__xmpir_mpz_cdiv_r_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cdiv_r_ui, typeof(__xmpir_mpz_cdiv_r_ui));
	private static readonly __xmpir_mpz_cdiv_qr_ui xmpir_mpz_cdiv_qr_ui = (__xmpir_mpz_cdiv_qr_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cdiv_qr_ui, typeof(__xmpir_mpz_cdiv_qr_ui));
	private static readonly __xmpir_mpz_cdiv_ui xmpir_mpz_cdiv_ui = (__xmpir_mpz_cdiv_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cdiv_ui, typeof(__xmpir_mpz_cdiv_ui));
	private static readonly __xmpir_mpz_cdiv_q_2exp xmpir_mpz_cdiv_q_2exp = (__xmpir_mpz_cdiv_q_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cdiv_q_2exp, typeof(__xmpir_mpz_cdiv_q_2exp));
	private static readonly __xmpir_mpz_cdiv_r_2exp xmpir_mpz_cdiv_r_2exp = (__xmpir_mpz_cdiv_r_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cdiv_r_2exp, typeof(__xmpir_mpz_cdiv_r_2exp));
	private static readonly __xmpir_mpz_fdiv_q xmpir_mpz_fdiv_q = (__xmpir_mpz_fdiv_q)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fdiv_q, typeof(__xmpir_mpz_fdiv_q));
	private static readonly __xmpir_mpz_fdiv_r xmpir_mpz_fdiv_r = (__xmpir_mpz_fdiv_r)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fdiv_r, typeof(__xmpir_mpz_fdiv_r));
	private static readonly __xmpir_mpz_fdiv_qr xmpir_mpz_fdiv_qr = (__xmpir_mpz_fdiv_qr)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fdiv_qr, typeof(__xmpir_mpz_fdiv_qr));
	private static readonly __xmpir_mpz_fdiv_q_ui xmpir_mpz_fdiv_q_ui = (__xmpir_mpz_fdiv_q_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fdiv_q_ui, typeof(__xmpir_mpz_fdiv_q_ui));
	private static readonly __xmpir_mpz_fdiv_r_ui xmpir_mpz_fdiv_r_ui = (__xmpir_mpz_fdiv_r_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fdiv_r_ui, typeof(__xmpir_mpz_fdiv_r_ui));
	private static readonly __xmpir_mpz_fdiv_qr_ui xmpir_mpz_fdiv_qr_ui = (__xmpir_mpz_fdiv_qr_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fdiv_qr_ui, typeof(__xmpir_mpz_fdiv_qr_ui));
	private static readonly __xmpir_mpz_fdiv_ui xmpir_mpz_fdiv_ui = (__xmpir_mpz_fdiv_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fdiv_ui, typeof(__xmpir_mpz_fdiv_ui));
	private static readonly __xmpir_mpz_fdiv_q_2exp xmpir_mpz_fdiv_q_2exp = (__xmpir_mpz_fdiv_q_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fdiv_q_2exp, typeof(__xmpir_mpz_fdiv_q_2exp));
	private static readonly __xmpir_mpz_fdiv_r_2exp xmpir_mpz_fdiv_r_2exp = (__xmpir_mpz_fdiv_r_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fdiv_r_2exp, typeof(__xmpir_mpz_fdiv_r_2exp));
	private static readonly __xmpir_mpz_tdiv_q xmpir_mpz_tdiv_q = (__xmpir_mpz_tdiv_q)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_tdiv_q, typeof(__xmpir_mpz_tdiv_q));
	private static readonly __xmpir_mpz_tdiv_r xmpir_mpz_tdiv_r = (__xmpir_mpz_tdiv_r)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_tdiv_r, typeof(__xmpir_mpz_tdiv_r));
	private static readonly __xmpir_mpz_tdiv_qr xmpir_mpz_tdiv_qr = (__xmpir_mpz_tdiv_qr)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_tdiv_qr, typeof(__xmpir_mpz_tdiv_qr));
	private static readonly __xmpir_mpz_tdiv_q_ui xmpir_mpz_tdiv_q_ui = (__xmpir_mpz_tdiv_q_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_tdiv_q_ui, typeof(__xmpir_mpz_tdiv_q_ui));
	private static readonly __xmpir_mpz_tdiv_r_ui xmpir_mpz_tdiv_r_ui = (__xmpir_mpz_tdiv_r_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_tdiv_r_ui, typeof(__xmpir_mpz_tdiv_r_ui));
	private static readonly __xmpir_mpz_tdiv_qr_ui xmpir_mpz_tdiv_qr_ui = (__xmpir_mpz_tdiv_qr_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_tdiv_qr_ui, typeof(__xmpir_mpz_tdiv_qr_ui));
	private static readonly __xmpir_mpz_tdiv_ui xmpir_mpz_tdiv_ui = (__xmpir_mpz_tdiv_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_tdiv_ui, typeof(__xmpir_mpz_tdiv_ui));
	private static readonly __xmpir_mpz_tdiv_q_2exp xmpir_mpz_tdiv_q_2exp = (__xmpir_mpz_tdiv_q_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_tdiv_q_2exp, typeof(__xmpir_mpz_tdiv_q_2exp));
	private static readonly __xmpir_mpz_tdiv_r_2exp xmpir_mpz_tdiv_r_2exp = (__xmpir_mpz_tdiv_r_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_tdiv_r_2exp, typeof(__xmpir_mpz_tdiv_r_2exp));
	private static readonly __xmpir_mpz_mod xmpir_mpz_mod = (__xmpir_mpz_mod)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_mod, typeof(__xmpir_mpz_mod));
	private static readonly __xmpir_mpz_mod_ui xmpir_mpz_mod_ui = (__xmpir_mpz_mod_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_mod_ui, typeof(__xmpir_mpz_mod_ui));
	private static readonly __xmpir_mpz_divexact xmpir_mpz_divexact = (__xmpir_mpz_divexact)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_divexact, typeof(__xmpir_mpz_divexact));
	private static readonly __xmpir_mpz_divexact_ui xmpir_mpz_divexact_ui = (__xmpir_mpz_divexact_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_divexact_ui, typeof(__xmpir_mpz_divexact_ui));
	private static readonly __xmpir_mpz_divisible_p xmpir_mpz_divisible_p = (__xmpir_mpz_divisible_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_divisible_p, typeof(__xmpir_mpz_divisible_p));
	private static readonly __xmpir_mpz_divisible_ui_p xmpir_mpz_divisible_ui_p = (__xmpir_mpz_divisible_ui_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_divisible_ui_p, typeof(__xmpir_mpz_divisible_ui_p));
	private static readonly __xmpir_mpz_divisible_2exp_p xmpir_mpz_divisible_2exp_p = (__xmpir_mpz_divisible_2exp_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_divisible_2exp_p, typeof(__xmpir_mpz_divisible_2exp_p));
	private static readonly __xmpir_mpz_congruent_p xmpir_mpz_congruent_p = (__xmpir_mpz_congruent_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_congruent_p, typeof(__xmpir_mpz_congruent_p));
	private static readonly __xmpir_mpz_congruent_ui_p xmpir_mpz_congruent_ui_p = (__xmpir_mpz_congruent_ui_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_congruent_ui_p, typeof(__xmpir_mpz_congruent_ui_p));
	private static readonly __xmpir_mpz_congruent_2exp_p xmpir_mpz_congruent_2exp_p = (__xmpir_mpz_congruent_2exp_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_congruent_2exp_p, typeof(__xmpir_mpz_congruent_2exp_p));
	private static readonly __xmpir_mpz_powm xmpir_mpz_powm = (__xmpir_mpz_powm)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_powm, typeof(__xmpir_mpz_powm));
	private static readonly __xmpir_mpz_powm_ui xmpir_mpz_powm_ui = (__xmpir_mpz_powm_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_powm_ui, typeof(__xmpir_mpz_powm_ui));
	private static readonly __xmpir_mpz_pow_ui xmpir_mpz_pow_ui = (__xmpir_mpz_pow_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_pow_ui, typeof(__xmpir_mpz_pow_ui));
	private static readonly __xmpir_mpz_ui_pow_ui xmpir_mpz_ui_pow_ui = (__xmpir_mpz_ui_pow_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_ui_pow_ui, typeof(__xmpir_mpz_ui_pow_ui));
	private static readonly __xmpir_mpz_root xmpir_mpz_root = (__xmpir_mpz_root)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_root, typeof(__xmpir_mpz_root));
	private static readonly __xmpir_mpz_rootrem xmpir_mpz_rootrem = (__xmpir_mpz_rootrem)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_rootrem, typeof(__xmpir_mpz_rootrem));
	private static readonly __xmpir_mpz_sqrt xmpir_mpz_sqrt = (__xmpir_mpz_sqrt)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_sqrt, typeof(__xmpir_mpz_sqrt));
	private static readonly __xmpir_mpz_sqrtrem xmpir_mpz_sqrtrem = (__xmpir_mpz_sqrtrem)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_sqrtrem, typeof(__xmpir_mpz_sqrtrem));
	private static readonly __xmpir_mpz_perfect_power_p xmpir_mpz_perfect_power_p = (__xmpir_mpz_perfect_power_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_perfect_power_p, typeof(__xmpir_mpz_perfect_power_p));
	private static readonly __xmpir_mpz_perfect_square_p xmpir_mpz_perfect_square_p = (__xmpir_mpz_perfect_square_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_perfect_square_p, typeof(__xmpir_mpz_perfect_square_p));
	private static readonly __xmpir_mpz_probab_prime_p xmpir_mpz_probab_prime_p = (__xmpir_mpz_probab_prime_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_probab_prime_p, typeof(__xmpir_mpz_probab_prime_p));
	private static readonly __xmpir_mpz_nextprime xmpir_mpz_nextprime = (__xmpir_mpz_nextprime)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_nextprime, typeof(__xmpir_mpz_nextprime));
	private static readonly __xmpir_mpz_gcd xmpir_mpz_gcd = (__xmpir_mpz_gcd)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_gcd, typeof(__xmpir_mpz_gcd));
	private static readonly __xmpir_mpz_gcd_ui xmpir_mpz_gcd_ui = (__xmpir_mpz_gcd_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_gcd_ui, typeof(__xmpir_mpz_gcd_ui));
	private static readonly __xmpir_mpz_gcdext xmpir_mpz_gcdext = (__xmpir_mpz_gcdext)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_gcdext, typeof(__xmpir_mpz_gcdext));
	private static readonly __xmpir_mpz_lcm xmpir_mpz_lcm = (__xmpir_mpz_lcm)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_lcm, typeof(__xmpir_mpz_lcm));
	private static readonly __xmpir_mpz_lcm_ui xmpir_mpz_lcm_ui = (__xmpir_mpz_lcm_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_lcm_ui, typeof(__xmpir_mpz_lcm_ui));
	private static readonly __xmpir_mpz_invert xmpir_mpz_invert = (__xmpir_mpz_invert)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_invert, typeof(__xmpir_mpz_invert));
	private static readonly __xmpir_mpz_jacobi xmpir_mpz_jacobi = (__xmpir_mpz_jacobi)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_jacobi, typeof(__xmpir_mpz_jacobi));
	private static readonly __xmpir_mpz_legendre xmpir_mpz_legendre = (__xmpir_mpz_legendre)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_legendre, typeof(__xmpir_mpz_legendre));
	private static readonly __xmpir_mpz_kronecker xmpir_mpz_kronecker = (__xmpir_mpz_kronecker)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_kronecker, typeof(__xmpir_mpz_kronecker));
	private static readonly __xmpir_mpz_kronecker_si xmpir_mpz_kronecker_si = (__xmpir_mpz_kronecker_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_kronecker_si, typeof(__xmpir_mpz_kronecker_si));
	private static readonly __xmpir_mpz_kronecker_ui xmpir_mpz_kronecker_ui = (__xmpir_mpz_kronecker_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_kronecker_ui, typeof(__xmpir_mpz_kronecker_ui));
	private static readonly __xmpir_mpz_si_kronecker xmpir_mpz_si_kronecker = (__xmpir_mpz_si_kronecker)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_si_kronecker, typeof(__xmpir_mpz_si_kronecker));
	private static readonly __xmpir_mpz_ui_kronecker xmpir_mpz_ui_kronecker = (__xmpir_mpz_ui_kronecker)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_ui_kronecker, typeof(__xmpir_mpz_ui_kronecker));
	private static readonly __xmpir_mpz_remove xmpir_mpz_remove = (__xmpir_mpz_remove)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_remove, typeof(__xmpir_mpz_remove));
	private static readonly __xmpir_mpz_fac_ui xmpir_mpz_fac_ui = (__xmpir_mpz_fac_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fac_ui, typeof(__xmpir_mpz_fac_ui));
	private static readonly __xmpir_mpz_bin_ui xmpir_mpz_bin_ui = (__xmpir_mpz_bin_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_bin_ui, typeof(__xmpir_mpz_bin_ui));
	private static readonly __xmpir_mpz_bin_uiui xmpir_mpz_bin_uiui = (__xmpir_mpz_bin_uiui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_bin_uiui, typeof(__xmpir_mpz_bin_uiui));
	private static readonly __xmpir_mpz_fib_ui xmpir_mpz_fib_ui = (__xmpir_mpz_fib_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fib_ui, typeof(__xmpir_mpz_fib_ui));
	private static readonly __xmpir_mpz_fib2_ui xmpir_mpz_fib2_ui = (__xmpir_mpz_fib2_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fib2_ui, typeof(__xmpir_mpz_fib2_ui));
	private static readonly __xmpir_mpz_lucnum_ui xmpir_mpz_lucnum_ui = (__xmpir_mpz_lucnum_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_lucnum_ui, typeof(__xmpir_mpz_lucnum_ui));
	private static readonly __xmpir_mpz_lucnum2_ui xmpir_mpz_lucnum2_ui = (__xmpir_mpz_lucnum2_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_lucnum2_ui, typeof(__xmpir_mpz_lucnum2_ui));
	private static readonly __xmpir_mpz_cmp xmpir_mpz_cmp = (__xmpir_mpz_cmp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cmp, typeof(__xmpir_mpz_cmp));
	private static readonly __xmpir_mpz_cmp_d xmpir_mpz_cmp_d = (__xmpir_mpz_cmp_d)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cmp_d, typeof(__xmpir_mpz_cmp_d));
	private static readonly __xmpir_mpz_cmp_si xmpir_mpz_cmp_si = (__xmpir_mpz_cmp_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cmp_si, typeof(__xmpir_mpz_cmp_si));
	private static readonly __xmpir_mpz_cmp_ui xmpir_mpz_cmp_ui = (__xmpir_mpz_cmp_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cmp_ui, typeof(__xmpir_mpz_cmp_ui));
	private static readonly __xmpir_mpz_cmpabs xmpir_mpz_cmpabs = (__xmpir_mpz_cmpabs)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cmpabs, typeof(__xmpir_mpz_cmpabs));
	private static readonly __xmpir_mpz_cmpabs_d xmpir_mpz_cmpabs_d = (__xmpir_mpz_cmpabs_d)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cmpabs_d, typeof(__xmpir_mpz_cmpabs_d));
	private static readonly __xmpir_mpz_cmpabs_ui xmpir_mpz_cmpabs_ui = (__xmpir_mpz_cmpabs_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_cmpabs_ui, typeof(__xmpir_mpz_cmpabs_ui));
	private static readonly __xmpir_mpz_sgn xmpir_mpz_sgn = (__xmpir_mpz_sgn)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_sgn, typeof(__xmpir_mpz_sgn));
	private static readonly __xmpir_mpz_and xmpir_mpz_and = (__xmpir_mpz_and)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_and, typeof(__xmpir_mpz_and));
	private static readonly __xmpir_mpz_ior xmpir_mpz_ior = (__xmpir_mpz_ior)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_ior, typeof(__xmpir_mpz_ior));
	private static readonly __xmpir_mpz_xor xmpir_mpz_xor = (__xmpir_mpz_xor)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_xor, typeof(__xmpir_mpz_xor));
	private static readonly __xmpir_mpz_com xmpir_mpz_com = (__xmpir_mpz_com)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_com, typeof(__xmpir_mpz_com));
	private static readonly __xmpir_mpz_popcount xmpir_mpz_popcount = (__xmpir_mpz_popcount)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_popcount, typeof(__xmpir_mpz_popcount));
	private static readonly __xmpir_mpz_hamdist xmpir_mpz_hamdist = (__xmpir_mpz_hamdist)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_hamdist, typeof(__xmpir_mpz_hamdist));
	private static readonly __xmpir_mpz_scan0 xmpir_mpz_scan0 = (__xmpir_mpz_scan0)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_scan0, typeof(__xmpir_mpz_scan0));
	private static readonly __xmpir_mpz_scan1 xmpir_mpz_scan1 = (__xmpir_mpz_scan1)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_scan1, typeof(__xmpir_mpz_scan1));
	private static readonly __xmpir_mpz_setbit xmpir_mpz_setbit = (__xmpir_mpz_setbit)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_setbit, typeof(__xmpir_mpz_setbit));
	private static readonly __xmpir_mpz_clrbit xmpir_mpz_clrbit = (__xmpir_mpz_clrbit)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_clrbit, typeof(__xmpir_mpz_clrbit));
	private static readonly __xmpir_mpz_combit xmpir_mpz_combit = (__xmpir_mpz_combit)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_combit, typeof(__xmpir_mpz_combit));
	private static readonly __xmpir_mpz_tstbit xmpir_mpz_tstbit = (__xmpir_mpz_tstbit)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_tstbit, typeof(__xmpir_mpz_tstbit));
	private static readonly __xmpir_mpz_urandomb xmpir_mpz_urandomb = (__xmpir_mpz_urandomb)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_urandomb, typeof(__xmpir_mpz_urandomb));
	private static readonly __xmpir_mpz_urandomm xmpir_mpz_urandomm = (__xmpir_mpz_urandomm)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_urandomm, typeof(__xmpir_mpz_urandomm));
	private static readonly __xmpir_mpz_rrandomb xmpir_mpz_rrandomb = (__xmpir_mpz_rrandomb)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_rrandomb, typeof(__xmpir_mpz_rrandomb));
	private static readonly __xmpir_mpz_fits_uint_p xmpir_mpz_fits_uint_p = (__xmpir_mpz_fits_uint_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fits_uint_p, typeof(__xmpir_mpz_fits_uint_p));
	private static readonly __xmpir_mpz_fits_sint_p xmpir_mpz_fits_sint_p = (__xmpir_mpz_fits_sint_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_fits_sint_p, typeof(__xmpir_mpz_fits_sint_p));
	private static readonly __xmpir_mpz_odd_p xmpir_mpz_odd_p = (__xmpir_mpz_odd_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_odd_p, typeof(__xmpir_mpz_odd_p));
	private static readonly __xmpir_mpz_even_p xmpir_mpz_even_p = (__xmpir_mpz_even_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_even_p, typeof(__xmpir_mpz_even_p));
	private static readonly __xmpir_mpz_sizeinbase xmpir_mpz_sizeinbase = (__xmpir_mpz_sizeinbase)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpz_sizeinbase, typeof(__xmpir_mpz_sizeinbase));
	private static readonly __xmpir_mpq_canonicalize xmpir_mpq_canonicalize = (__xmpir_mpq_canonicalize)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_canonicalize, typeof(__xmpir_mpq_canonicalize));
	private static readonly __xmpir_mpq_set xmpir_mpq_set = (__xmpir_mpq_set)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_set, typeof(__xmpir_mpq_set));
	private static readonly __xmpir_mpq_set_z xmpir_mpq_set_z = (__xmpir_mpq_set_z)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_set_z, typeof(__xmpir_mpq_set_z));
	private static readonly __xmpir_mpq_set_ui xmpir_mpq_set_ui = (__xmpir_mpq_set_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_set_ui, typeof(__xmpir_mpq_set_ui));
	private static readonly __xmpir_mpq_set_si xmpir_mpq_set_si = (__xmpir_mpq_set_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_set_si, typeof(__xmpir_mpq_set_si));
	private static readonly __xmpir_mpq_set_str xmpir_mpq_set_str = (__xmpir_mpq_set_str)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_set_str, typeof(__xmpir_mpq_set_str));
	private static readonly __xmpir_mpq_swap xmpir_mpq_swap = (__xmpir_mpq_swap)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_swap, typeof(__xmpir_mpq_swap));
	private static readonly __xmpir_mpq_get_d xmpir_mpq_get_d = (__xmpir_mpq_get_d)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_get_d, typeof(__xmpir_mpq_get_d));
	private static readonly __xmpir_mpq_set_d xmpir_mpq_set_d = (__xmpir_mpq_set_d)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_set_d, typeof(__xmpir_mpq_set_d));
	private static readonly __xmpir_mpq_set_f xmpir_mpq_set_f = (__xmpir_mpq_set_f)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_set_f, typeof(__xmpir_mpq_set_f));
	private static readonly __xmpir_mpq_get_string xmpir_mpq_get_string = (__xmpir_mpq_get_string)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_get_string, typeof(__xmpir_mpq_get_string));
	private static readonly __xmpir_mpq_add xmpir_mpq_add = (__xmpir_mpq_add)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_add, typeof(__xmpir_mpq_add));
	private static readonly __xmpir_mpq_sub xmpir_mpq_sub = (__xmpir_mpq_sub)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_sub, typeof(__xmpir_mpq_sub));
	private static readonly __xmpir_mpq_mul xmpir_mpq_mul = (__xmpir_mpq_mul)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_mul, typeof(__xmpir_mpq_mul));
	private static readonly __xmpir_mpq_mul_2exp xmpir_mpq_mul_2exp = (__xmpir_mpq_mul_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_mul_2exp, typeof(__xmpir_mpq_mul_2exp));
	private static readonly __xmpir_mpq_div xmpir_mpq_div = (__xmpir_mpq_div)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_div, typeof(__xmpir_mpq_div));
	private static readonly __xmpir_mpq_div_2exp xmpir_mpq_div_2exp = (__xmpir_mpq_div_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_div_2exp, typeof(__xmpir_mpq_div_2exp));
	private static readonly __xmpir_mpq_neg xmpir_mpq_neg = (__xmpir_mpq_neg)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_neg, typeof(__xmpir_mpq_neg));
	private static readonly __xmpir_mpq_abs xmpir_mpq_abs = (__xmpir_mpq_abs)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_abs, typeof(__xmpir_mpq_abs));
	private static readonly __xmpir_mpq_inv xmpir_mpq_inv = (__xmpir_mpq_inv)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_inv, typeof(__xmpir_mpq_inv));
	private static readonly __xmpir_mpq_cmp xmpir_mpq_cmp = (__xmpir_mpq_cmp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_cmp, typeof(__xmpir_mpq_cmp));
	private static readonly __xmpir_mpq_cmp_ui xmpir_mpq_cmp_ui = (__xmpir_mpq_cmp_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_cmp_ui, typeof(__xmpir_mpq_cmp_ui));
	private static readonly __xmpir_mpq_cmp_si xmpir_mpq_cmp_si = (__xmpir_mpq_cmp_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_cmp_si, typeof(__xmpir_mpq_cmp_si));
	private static readonly __xmpir_mpq_sgn xmpir_mpq_sgn = (__xmpir_mpq_sgn)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_sgn, typeof(__xmpir_mpq_sgn));
	private static readonly __xmpir_mpq_equal xmpir_mpq_equal = (__xmpir_mpq_equal)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_equal, typeof(__xmpir_mpq_equal));
	private static readonly __xmpir_mpq_get_num xmpir_mpq_get_num = (__xmpir_mpq_get_num)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_get_num, typeof(__xmpir_mpq_get_num));
	private static readonly __xmpir_mpq_get_den xmpir_mpq_get_den = (__xmpir_mpq_get_den)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_get_den, typeof(__xmpir_mpq_get_den));
	private static readonly __xmpir_mpq_set_num xmpir_mpq_set_num = (__xmpir_mpq_set_num)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_set_num, typeof(__xmpir_mpq_set_num));
	private static readonly __xmpir_mpq_set_den xmpir_mpq_set_den = (__xmpir_mpq_set_den)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpq_set_den, typeof(__xmpir_mpq_set_den));
	private static readonly __xmpir_mpf_get_prec xmpir_mpf_get_prec = (__xmpir_mpf_get_prec)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_get_prec, typeof(__xmpir_mpf_get_prec));
	private static readonly __xmpir_mpf_set_prec xmpir_mpf_set_prec = (__xmpir_mpf_set_prec)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_set_prec, typeof(__xmpir_mpf_set_prec));
	private static readonly __xmpir_mpf_set xmpir_mpf_set = (__xmpir_mpf_set)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_set, typeof(__xmpir_mpf_set));
	private static readonly __xmpir_mpf_set_ui xmpir_mpf_set_ui = (__xmpir_mpf_set_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_set_ui, typeof(__xmpir_mpf_set_ui));
	private static readonly __xmpir_mpf_set_si xmpir_mpf_set_si = (__xmpir_mpf_set_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_set_si, typeof(__xmpir_mpf_set_si));
	private static readonly __xmpir_mpf_set_d xmpir_mpf_set_d = (__xmpir_mpf_set_d)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_set_d, typeof(__xmpir_mpf_set_d));
	private static readonly __xmpir_mpf_set_z xmpir_mpf_set_z = (__xmpir_mpf_set_z)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_set_z, typeof(__xmpir_mpf_set_z));
	private static readonly __xmpir_mpf_set_q xmpir_mpf_set_q = (__xmpir_mpf_set_q)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_set_q, typeof(__xmpir_mpf_set_q));
	private static readonly __xmpir_mpf_set_str xmpir_mpf_set_str = (__xmpir_mpf_set_str)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_set_str, typeof(__xmpir_mpf_set_str));
	private static readonly __xmpir_mpf_swap xmpir_mpf_swap = (__xmpir_mpf_swap)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_swap, typeof(__xmpir_mpf_swap));
	private static readonly __xmpir_mpf_get_d xmpir_mpf_get_d = (__xmpir_mpf_get_d)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_get_d, typeof(__xmpir_mpf_get_d));
	private static readonly __xmpir_mpf_get_d_2exp xmpir_mpf_get_d_2exp = (__xmpir_mpf_get_d_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_get_d_2exp, typeof(__xmpir_mpf_get_d_2exp));
	private static readonly __xmpir_mpf_get_si xmpir_mpf_get_si = (__xmpir_mpf_get_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_get_si, typeof(__xmpir_mpf_get_si));
	private static readonly __xmpir_mpf_get_ui xmpir_mpf_get_ui = (__xmpir_mpf_get_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_get_ui, typeof(__xmpir_mpf_get_ui));
	private static readonly __xmpir_mpf_get_string xmpir_mpf_get_string = (__xmpir_mpf_get_string)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_get_string, typeof(__xmpir_mpf_get_string));
	private static readonly __xmpir_mpf_add xmpir_mpf_add = (__xmpir_mpf_add)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_add, typeof(__xmpir_mpf_add));
	private static readonly __xmpir_mpf_add_ui xmpir_mpf_add_ui = (__xmpir_mpf_add_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_add_ui, typeof(__xmpir_mpf_add_ui));
	private static readonly __xmpir_mpf_sub xmpir_mpf_sub = (__xmpir_mpf_sub)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_sub, typeof(__xmpir_mpf_sub));
	private static readonly __xmpir_mpf_ui_sub xmpir_mpf_ui_sub = (__xmpir_mpf_ui_sub)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_ui_sub, typeof(__xmpir_mpf_ui_sub));
	private static readonly __xmpir_mpf_sub_ui xmpir_mpf_sub_ui = (__xmpir_mpf_sub_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_sub_ui, typeof(__xmpir_mpf_sub_ui));
	private static readonly __xmpir_mpf_mul xmpir_mpf_mul = (__xmpir_mpf_mul)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_mul, typeof(__xmpir_mpf_mul));
	private static readonly __xmpir_mpf_mul_ui xmpir_mpf_mul_ui = (__xmpir_mpf_mul_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_mul_ui, typeof(__xmpir_mpf_mul_ui));
	private static readonly __xmpir_mpf_div xmpir_mpf_div = (__xmpir_mpf_div)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_div, typeof(__xmpir_mpf_div));
	private static readonly __xmpir_mpf_ui_div xmpir_mpf_ui_div = (__xmpir_mpf_ui_div)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_ui_div, typeof(__xmpir_mpf_ui_div));
	private static readonly __xmpir_mpf_div_ui xmpir_mpf_div_ui = (__xmpir_mpf_div_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_div_ui, typeof(__xmpir_mpf_div_ui));
	private static readonly __xmpir_mpf_sqrt xmpir_mpf_sqrt = (__xmpir_mpf_sqrt)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_sqrt, typeof(__xmpir_mpf_sqrt));
	private static readonly __xmpir_mpf_sqrt_ui xmpir_mpf_sqrt_ui = (__xmpir_mpf_sqrt_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_sqrt_ui, typeof(__xmpir_mpf_sqrt_ui));
	private static readonly __xmpir_mpf_pow_ui xmpir_mpf_pow_ui = (__xmpir_mpf_pow_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_pow_ui, typeof(__xmpir_mpf_pow_ui));
	private static readonly __xmpir_mpf_neg xmpir_mpf_neg = (__xmpir_mpf_neg)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_neg, typeof(__xmpir_mpf_neg));
	private static readonly __xmpir_mpf_abs xmpir_mpf_abs = (__xmpir_mpf_abs)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_abs, typeof(__xmpir_mpf_abs));
	private static readonly __xmpir_mpf_mul_2exp xmpir_mpf_mul_2exp = (__xmpir_mpf_mul_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_mul_2exp, typeof(__xmpir_mpf_mul_2exp));
	private static readonly __xmpir_mpf_div_2exp xmpir_mpf_div_2exp = (__xmpir_mpf_div_2exp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_div_2exp, typeof(__xmpir_mpf_div_2exp));
	private static readonly __xmpir_mpf_cmp xmpir_mpf_cmp = (__xmpir_mpf_cmp)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_cmp, typeof(__xmpir_mpf_cmp));
	private static readonly __xmpir_mpf_cmp_d xmpir_mpf_cmp_d = (__xmpir_mpf_cmp_d)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_cmp_d, typeof(__xmpir_mpf_cmp_d));
	private static readonly __xmpir_mpf_cmp_ui xmpir_mpf_cmp_ui = (__xmpir_mpf_cmp_ui)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_cmp_ui, typeof(__xmpir_mpf_cmp_ui));
	private static readonly __xmpir_mpf_cmp_si xmpir_mpf_cmp_si = (__xmpir_mpf_cmp_si)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_cmp_si, typeof(__xmpir_mpf_cmp_si));
	private static readonly __xmpir_mpf_eq xmpir_mpf_eq = (__xmpir_mpf_eq)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_eq, typeof(__xmpir_mpf_eq));
	private static readonly __xmpir_mpf_reldiff xmpir_mpf_reldiff = (__xmpir_mpf_reldiff)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_reldiff, typeof(__xmpir_mpf_reldiff));
	private static readonly __xmpir_mpf_sgn xmpir_mpf_sgn = (__xmpir_mpf_sgn)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_sgn, typeof(__xmpir_mpf_sgn));
	private static readonly __xmpir_mpf_ceil xmpir_mpf_ceil = (__xmpir_mpf_ceil)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_ceil, typeof(__xmpir_mpf_ceil));
	private static readonly __xmpir_mpf_floor xmpir_mpf_floor = (__xmpir_mpf_floor)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_floor, typeof(__xmpir_mpf_floor));
	private static readonly __xmpir_mpf_trunc xmpir_mpf_trunc = (__xmpir_mpf_trunc)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_trunc, typeof(__xmpir_mpf_trunc));
	private static readonly __xmpir_mpf_integer_p xmpir_mpf_integer_p = (__xmpir_mpf_integer_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_integer_p, typeof(__xmpir_mpf_integer_p));
	private static readonly __xmpir_mpf_fits_uint_p xmpir_mpf_fits_uint_p = (__xmpir_mpf_fits_uint_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_fits_uint_p, typeof(__xmpir_mpf_fits_uint_p));
	private static readonly __xmpir_mpf_fits_sint_p xmpir_mpf_fits_sint_p = (__xmpir_mpf_fits_sint_p)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_fits_sint_p, typeof(__xmpir_mpf_fits_sint_p));
	private static readonly __xmpir_mpf_urandomb xmpir_mpf_urandomb = (__xmpir_mpf_urandomb)Marshal.GetDelegateForFunctionPointer(__ptr__xmpir_mpf_urandomb, typeof(__xmpir_mpf_urandomb));

	//
	// Automatically generated code: functions
	//
	public static mpz_intptr MpzInit()
	{
		int __retval;
		__retval = xmpir_mpz_init(out var result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpzInit2(ulong n)
	{
		int __retval;
		__retval = xmpir_mpz_init2(out var result, n);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpzInitSet(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_init_set(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpzInitSetUi(uint op)
	{
		int __retval;
		__retval = xmpir_mpz_init_set_ui(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpzInitSetSi(int op)
	{
		int __retval;
		__retval = xmpir_mpz_init_set_si(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpz_intptr MpzInitSetD(double op)
	{
		int __retval;
		__retval = xmpir_mpz_init_set_d(out var result, op);
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
		int __retval;
		__retval = xmpir_mpq_init(out var result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpf_intptr MpfInit2(uint prec)
	{
		int __retval;
		__retval = xmpir_mpf_init2(out var result, prec);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpf_intptr MpfInitSet(MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_init_set(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpf_intptr MpfInitSetUi(uint op)
	{
		int __retval;
		__retval = xmpir_mpf_init_set_ui(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpf_intptr MpfInitSetSi(int op)
	{
		int __retval;
		__retval = xmpir_mpf_init_set_si(out var result, op);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static mpf_intptr MpfInitSetD(double op)
	{
		int __retval;
		__retval = xmpir_mpf_init_set_d(out var result, op);
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
		int __retval;
		__retval = xmpir_mpz_clear(v.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqClear(MpqT v)
	{
		int __retval;
		__retval = xmpir_mpq_clear(v.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfClear(MpfT v)
	{
		int __retval;
		__retval = xmpir_mpf_clear(v.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void XMpirDummy()
	{
		int __retval;
		__retval = xmpir_xmpir_dummy();
		if (__retval != 0) HandleError(__retval);
	}
	public static int XMpirDummyAdd(int a, int b)
	{
		int __retval;
		__retval = xmpir_xmpir_dummy_add(out var result, a, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int XMpirDummy3mpz(MpzT op0, MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_xmpir_dummy_3mpz(out var result, op0.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static gmp_randstate_intptr GmpRandinitDefault()
	{
		int __retval;
		__retval = xmpir_gmp_randinit_default(out var result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static gmp_randstate_intptr GmpRandinitMt()
	{
		int __retval;
		__retval = xmpir_gmp_randinit_mt(out var result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static gmp_randstate_intptr GmpRandinitLc2exp(MpzT a, uint c, ulong m2exp)
	{
		int __retval;
		__retval = xmpir_gmp_randinit_lc_2exp(out var result, a.val, c, m2exp);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static gmp_randstate_intptr GmpRandinitSet(GmpRandstateT op)
	{
		int __retval;
		__retval = xmpir_gmp_randinit_set(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void GmpRandclear(GmpRandstateT v)
	{
		int __retval;
		__retval = xmpir_gmp_randclear(v.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void GmpRandseed(GmpRandstateT state, MpzT seed)
	{
		int __retval;
		__retval = xmpir_gmp_randseed(state.val, seed.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void GmpRandseedUi(GmpRandstateT state, uint seed)
	{
		int __retval;
		__retval = xmpir_gmp_randseed_ui(state.val, seed);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint GmpUrandombUi(GmpRandstateT state, uint n)
	{
		int __retval;
		__retval = xmpir_gmp_urandomb_ui(out var result, state.val, n);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint GmpUrandommUi(GmpRandstateT state, uint n)
	{
		int __retval;
		__retval = xmpir_gmp_urandomm_ui(out var result, state.val, n);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzRealloc2(MpzT x, uint n)
	{
		int __retval;
		__retval = xmpir_mpz_realloc2(x.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetDefaultPrec(ulong prec)
	{
		int __retval;
		__retval = xmpir_mpf_set_default_prec(prec);
		if (__retval != 0) HandleError(__retval);
	}
	public static ulong MpfGetDefaultPrec()
	{
		int __retval;
		__retval = xmpir_mpf_get_default_prec(out var result);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzSet(MpzT rop, MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_set(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSetUi(MpzT rop, uint op)
	{
		int __retval;
		__retval = xmpir_mpz_set_ui(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSetSi(MpzT rop, int op)
	{
		int __retval;
		__retval = xmpir_mpz_set_si(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSetD(MpzT rop, double op)
	{
		int __retval;
		__retval = xmpir_mpz_set_d(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSetQ(MpzT rop, MpqT op)
	{
		int __retval;
		__retval = xmpir_mpz_set_q(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSetF(MpzT rop, MpfT op)
	{
		int __retval;
		__retval = xmpir_mpz_set_f(rop.val, op.val);
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
		int __retval;
		__retval = xmpir_mpz_swap(rop1.val, rop2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzGetUi(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_get_ui(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzGetSi(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_get_si(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static double MpzGetD(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_get_d(out var result, op.val);
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
		int __retval;
		__retval = xmpir_mpz_add(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzAddUi(MpzT rop, MpzT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpz_add_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSub(MpzT rop, MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_sub(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSubUi(MpzT rop, MpzT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpz_sub_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzUiSub(MpzT rop, uint op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_ui_sub(rop.val, op1, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzMul(MpzT rop, MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_mul(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzMulSi(MpzT rop, MpzT op1, int op2)
	{
		int __retval;
		__retval = xmpir_mpz_mul_si(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzMulUi(MpzT rop, MpzT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpz_mul_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzAddmul(MpzT rop, MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_addmul(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzAddmulUi(MpzT rop, MpzT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpz_addmul_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSubmul(MpzT rop, MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_submul(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSubmulUi(MpzT rop, MpzT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpz_submul_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzMul2exp(MpzT rop, MpzT op1, ulong op2)
	{
		int __retval;
		__retval = xmpir_mpz_mul_2exp(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzNeg(MpzT rop, MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_neg(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzAbs(MpzT rop, MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_abs(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCdivQ(MpzT q, MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_cdiv_q(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCdivR(MpzT r, MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_cdiv_r(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCdivQr(MpzT q, MpzT r, MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_cdiv_qr(q.val, r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzCdivQUi(MpzT q, MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_cdiv_q_ui(out var result, q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzCdivRUi(MpzT r, MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_cdiv_r_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzCdivQrUi(MpzT q, MpzT r, MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_cdiv_qr_ui(out var result, q.val, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzCdivUi(MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_cdiv_ui(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzCdivQ2exp(MpzT q, MpzT n, ulong b)
	{
		int __retval;
		__retval = xmpir_mpz_cdiv_q_2exp(q.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCdivR2exp(MpzT r, MpzT n, ulong b)
	{
		int __retval;
		__retval = xmpir_mpz_cdiv_r_2exp(r.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFdivQ(MpzT q, MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_fdiv_q(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFdivR(MpzT r, MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_fdiv_r(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFdivQr(MpzT q, MpzT r, MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_fdiv_qr(q.val, r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzFdivQUi(MpzT q, MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_fdiv_q_ui(out var result, q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzFdivRUi(MpzT r, MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_fdiv_r_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzFdivQrUi(MpzT q, MpzT r, MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_fdiv_qr_ui(out var result, q.val, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzFdivUi(MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_fdiv_ui(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzFdivQ2exp(MpzT q, MpzT n, ulong b)
	{
		int __retval;
		__retval = xmpir_mpz_fdiv_q_2exp(q.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFdivR2exp(MpzT r, MpzT n, ulong b)
	{
		int __retval;
		__retval = xmpir_mpz_fdiv_r_2exp(r.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzTdivQ(MpzT q, MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_tdiv_q(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzTdivR(MpzT r, MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_tdiv_r(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzTdivQr(MpzT q, MpzT r, MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_tdiv_qr(q.val, r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzTdivQUi(MpzT q, MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_tdiv_q_ui(out var result, q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzTdivRUi(MpzT r, MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_tdiv_r_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzTdivQrUi(MpzT q, MpzT r, MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_tdiv_qr_ui(out var result, q.val, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzTdivUi(MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_tdiv_ui(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzTdivQ2exp(MpzT q, MpzT n, ulong b)
	{
		int __retval;
		__retval = xmpir_mpz_tdiv_q_2exp(q.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzTdivR2exp(MpzT r, MpzT n, ulong b)
	{
		int __retval;
		__retval = xmpir_mpz_tdiv_r_2exp(r.val, n.val, b);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzMod(MpzT r, MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_mod(r.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzModUi(MpzT r, MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_mod_ui(out var result, r.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzDivexact(MpzT q, MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_divexact(q.val, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzDivexactUi(MpzT q, MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_divexact_ui(q.val, n.val, d);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzDivisibleP(MpzT n, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_divisible_p(out var result, n.val, d.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzDivisibleUiP(MpzT n, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_divisible_ui_p(out var result, n.val, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzDivisible2expP(MpzT n, ulong b)
	{
		int __retval;
		__retval = xmpir_mpz_divisible_2exp_p(out var result, n.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCongruentP(MpzT n, MpzT c, MpzT d)
	{
		int __retval;
		__retval = xmpir_mpz_congruent_p(out var result, n.val, c.val, d.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCongruentUiP(MpzT n, uint c, uint d)
	{
		int __retval;
		__retval = xmpir_mpz_congruent_ui_p(out var result, n.val, c, d);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCongruent2expP(MpzT n, MpzT c, ulong b)
	{
		int __retval;
		__retval = xmpir_mpz_congruent_2exp_p(out var result, n.val, c.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzPowm(MpzT rop, MpzT Base, MpzT Exp, MpzT Mod)
	{
		int __retval;
		__retval = xmpir_mpz_powm(rop.val, Base.val, Exp.val, Mod.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzPowmUi(MpzT rop, MpzT Base, uint Exp, MpzT Mod)
	{
		int __retval;
		__retval = xmpir_mpz_powm_ui(rop.val, Base.val, Exp, Mod.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzPowUi(MpzT rop, MpzT Base, uint Exp)
	{
		int __retval;
		__retval = xmpir_mpz_pow_ui(rop.val, Base.val, Exp);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzUiPowUi(MpzT rop, uint Base, uint Exp)
	{
		int __retval;
		__retval = xmpir_mpz_ui_pow_ui(rop.val, Base, Exp);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzRoot(MpzT rop, MpzT op, uint n)
	{
		int __retval;
		__retval = xmpir_mpz_root(out var result, rop.val, op.val, n);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzRootrem(MpzT root, MpzT rem, MpzT u, uint n)
	{
		int __retval;
		__retval = xmpir_mpz_rootrem(root.val, rem.val, u.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSqrt(MpzT rop, MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_sqrt(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzSqrtrem(MpzT rop1, MpzT rop2, MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_sqrtrem(rop1.val, rop2.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzPerfectPowerP(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_perfect_power_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzPerfectSquareP(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_perfect_square_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzProbabPrimeP(MpzT n, uint reps)
	{
		int __retval;
		__retval = xmpir_mpz_probab_prime_p(out var result, n.val, reps);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzNextprime(MpzT rop, MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_nextprime(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzGcd(MpzT rop, MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_gcd(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static uint MpzGcdUi(MpzT rop, MpzT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpz_gcd_ui(out var result, rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzGcdext(MpzT g, MpzT s, MpzT t, MpzT a, MpzT b)
	{
		int __retval;
		__retval = xmpir_mpz_gcdext(g.val, s.val, t.val, a.val, b.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzLcm(MpzT rop, MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_lcm(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzLcmUi(MpzT rop, MpzT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpz_lcm_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzInvert(MpzT rop, MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_invert(out var result, rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzJacobi(MpzT a, MpzT b)
	{
		int __retval;
		__retval = xmpir_mpz_jacobi(out var result, a.val, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzLegendre(MpzT a, MpzT p)
	{
		int __retval;
		__retval = xmpir_mpz_legendre(out var result, a.val, p.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzKronecker(MpzT a, MpzT b)
	{
		int __retval;
		__retval = xmpir_mpz_kronecker(out var result, a.val, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzKroneckerSi(MpzT a, int b)
	{
		int __retval;
		__retval = xmpir_mpz_kronecker_si(out var result, a.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzKroneckerUi(MpzT a, uint b)
	{
		int __retval;
		__retval = xmpir_mpz_kronecker_ui(out var result, a.val, b);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzSiKronecker(int a, MpzT b)
	{
		int __retval;
		__retval = xmpir_mpz_si_kronecker(out var result, a, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzUiKronecker(uint a, MpzT b)
	{
		int __retval;
		__retval = xmpir_mpz_ui_kronecker(out var result, a, b.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpzRemove(MpzT rop, MpzT op, MpzT f)
	{
		int __retval;
		__retval = xmpir_mpz_remove(out var result, rop.val, op.val, f.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzFacUi(MpzT rop, uint op)
	{
		int __retval;
		__retval = xmpir_mpz_fac_ui(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzBinUi(MpzT rop, MpzT n, uint k)
	{
		int __retval;
		__retval = xmpir_mpz_bin_ui(rop.val, n.val, k);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzBinUiui(MpzT rop, uint n, uint k)
	{
		int __retval;
		__retval = xmpir_mpz_bin_uiui(rop.val, n, k);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFibUi(MpzT fn, uint n)
	{
		int __retval;
		__retval = xmpir_mpz_fib_ui(fn.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzFib2Ui(MpzT fn, MpzT fnsub1, uint n)
	{
		int __retval;
		__retval = xmpir_mpz_fib2_ui(fn.val, fnsub1.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzLucnumUi(MpzT ln, uint n)
	{
		int __retval;
		__retval = xmpir_mpz_lucnum_ui(ln.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzLucnum2Ui(MpzT ln, MpzT lnsub1, uint n)
	{
		int __retval;
		__retval = xmpir_mpz_lucnum2_ui(ln.val, lnsub1.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzCmp(MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_cmp(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpD(MpzT op1, double op2)
	{
		int __retval;
		__retval = xmpir_mpz_cmp_d(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpSi(MpzT op1, int op2)
	{
		int __retval;
		__retval = xmpir_mpz_cmp_si(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpUi(MpzT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpz_cmp_ui(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpabs(MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_cmpabs(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpabsD(MpzT op1, double op2)
	{
		int __retval;
		__retval = xmpir_mpz_cmpabs_d(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzCmpabsUi(MpzT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpz_cmpabs_ui(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzSgn(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_sgn(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzAnd(MpzT rop, MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_and(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzIor(MpzT rop, MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_ior(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzXor(MpzT rop, MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_xor(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCom(MpzT rop, MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_com(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static ulong MpzPopcount(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_popcount(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpzHamdist(MpzT op1, MpzT op2)
	{
		int __retval;
		__retval = xmpir_mpz_hamdist(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpzScan0(MpzT op, ulong startingBit)
	{
		int __retval;
		__retval = xmpir_mpz_scan0(out var result, op.val, startingBit);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static ulong MpzScan1(MpzT op, ulong startingBit)
	{
		int __retval;
		__retval = xmpir_mpz_scan1(out var result, op.val, startingBit);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzSetbit(MpzT rop, ulong bitIndex)
	{
		int __retval;
		__retval = xmpir_mpz_setbit(rop.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzClrbit(MpzT rop, ulong bitIndex)
	{
		int __retval;
		__retval = xmpir_mpz_clrbit(rop.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzCombit(MpzT rop, ulong bitIndex)
	{
		int __retval;
		__retval = xmpir_mpz_combit(rop.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzTstbit(MpzT op, ulong bitIndex)
	{
		int __retval;
		__retval = xmpir_mpz_tstbit(out var result, op.val, bitIndex);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpzUrandomb(MpzT rop, GmpRandstateT state, ulong n)
	{
		int __retval;
		__retval = xmpir_mpz_urandomb(rop.val, state.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzUrandomm(MpzT rop, GmpRandstateT state, MpzT n)
	{
		int __retval;
		__retval = xmpir_mpz_urandomm(rop.val, state.val, n.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpzRrandomb(MpzT rop, GmpRandstateT state, ulong n)
	{
		int __retval;
		__retval = xmpir_mpz_rrandomb(rop.val, state.val, n);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpzFitsUintP(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_fits_uint_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzFitsSintP(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_fits_sint_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzOddP(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_odd_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpzEvenP(MpzT op)
	{
		int __retval;
		__retval = xmpir_mpz_even_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpzSizeinbase(MpzT op, uint Base)
	{
		int __retval;
		__retval = xmpir_mpz_sizeinbase(out var result, op.val, Base);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpqCanonicalize(MpqT op)
	{
		int __retval;
		__retval = xmpir_mpq_canonicalize(op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSet(MpqT rop, MpqT op)
	{
		int __retval;
		__retval = xmpir_mpq_set(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetZ(MpqT rop, MpzT op)
	{
		int __retval;
		__retval = xmpir_mpq_set_z(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetUi(MpqT rop, uint op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpq_set_ui(rop.val, op1, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetSi(MpqT rop, int op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpq_set_si(rop.val, op1, op2);
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
		int __retval;
		__retval = xmpir_mpq_swap(rop1.val, rop2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static double MpqGetD(MpqT op)
	{
		int __retval;
		__retval = xmpir_mpq_get_d(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpqSetD(MpqT rop, double op)
	{
		int __retval;
		__retval = xmpir_mpq_set_d(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetF(MpqT rop, MpfT op)
	{
		int __retval;
		__retval = xmpir_mpq_set_f(rop.val, op.val);
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
		int __retval;
		__retval = xmpir_mpq_add(sum.val, addend1.val, addend2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSub(MpqT difference, MpqT minuend, MpqT subtrahend)
	{
		int __retval;
		__retval = xmpir_mpq_sub(difference.val, minuend.val, subtrahend.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqMul(MpqT product, MpqT multiplier, MpqT multiplicand)
	{
		int __retval;
		__retval = xmpir_mpq_mul(product.val, multiplier.val, multiplicand.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqMul2exp(MpqT rop, MpqT op1, ulong op2)
	{
		int __retval;
		__retval = xmpir_mpq_mul_2exp(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqDiv(MpqT quotient, MpqT dividend, MpqT divisor)
	{
		int __retval;
		__retval = xmpir_mpq_div(quotient.val, dividend.val, divisor.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqDiv2exp(MpqT rop, MpqT op1, ulong op2)
	{
		int __retval;
		__retval = xmpir_mpq_div_2exp(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqNeg(MpqT negatedOperand, MpqT operand)
	{
		int __retval;
		__retval = xmpir_mpq_neg(negatedOperand.val, operand.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqAbs(MpqT rop, MpqT op)
	{
		int __retval;
		__retval = xmpir_mpq_abs(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqInv(MpqT invertedNumber, MpqT number)
	{
		int __retval;
		__retval = xmpir_mpq_inv(invertedNumber.val, number.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpqCmp(MpqT op1, MpqT op2)
	{
		int __retval;
		__retval = xmpir_mpq_cmp(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpqCmpUi(MpqT op1, uint num2, uint den2)
	{
		int __retval;
		__retval = xmpir_mpq_cmp_ui(out var result, op1.val, num2, den2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpqCmpSi(MpqT op1, int num2, uint den2)
	{
		int __retval;
		__retval = xmpir_mpq_cmp_si(out var result, op1.val, num2, den2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpqSgn(MpqT op)
	{
		int __retval;
		__retval = xmpir_mpq_sgn(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpqEqual(MpqT op1, MpqT op2)
	{
		int __retval;
		__retval = xmpir_mpq_equal(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpqGetNum(MpzT numerator, MpqT rational)
	{
		int __retval;
		__retval = xmpir_mpq_get_num(numerator.val, rational.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqGetDen(MpzT denominator, MpqT rational)
	{
		int __retval;
		__retval = xmpir_mpq_get_den(denominator.val, rational.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetNum(MpqT rational, MpzT numerator)
	{
		int __retval;
		__retval = xmpir_mpq_set_num(rational.val, numerator.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpqSetDen(MpqT rational, MpzT denominator)
	{
		int __retval;
		__retval = xmpir_mpq_set_den(rational.val, denominator.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static ulong MpfGetPrec(MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_get_prec(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpfSetPrec(MpfT rop, ulong prec)
	{
		int __retval;
		__retval = xmpir_mpf_set_prec(rop.val, prec);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSet(MpfT rop, MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_set(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetUi(MpfT rop, uint op)
	{
		int __retval;
		__retval = xmpir_mpf_set_ui(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetSi(MpfT rop, int op)
	{
		int __retval;
		__retval = xmpir_mpf_set_si(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetD(MpfT rop, double op)
	{
		int __retval;
		__retval = xmpir_mpf_set_d(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetZ(MpfT rop, MpzT op)
	{
		int __retval;
		__retval = xmpir_mpf_set_z(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSetQ(MpfT rop, MpqT op)
	{
		int __retval;
		__retval = xmpir_mpf_set_q(rop.val, op.val);
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
		int __retval;
		__retval = xmpir_mpf_swap(rop1.val, rop2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static double MpfGetD(MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_get_d(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static double MpfGetD2exp(out long expptr, MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_get_d_2exp(out var result, out expptr, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfGetSi(MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_get_si(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static uint MpfGetUi(MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_get_ui(out var result, op.val);
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
		int __retval;
		__retval = xmpir_mpf_add(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfAddUi(MpfT rop, MpfT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpf_add_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSub(MpfT rop, MpfT op1, MpfT op2)
	{
		int __retval;
		__retval = xmpir_mpf_sub(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfUiSub(MpfT rop, uint op1, MpfT op2)
	{
		int __retval;
		__retval = xmpir_mpf_ui_sub(rop.val, op1, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSubUi(MpfT rop, MpfT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpf_sub_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfMul(MpfT rop, MpfT op1, MpfT op2)
	{
		int __retval;
		__retval = xmpir_mpf_mul(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfMulUi(MpfT rop, MpfT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpf_mul_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfDiv(MpfT rop, MpfT op1, MpfT op2)
	{
		int __retval;
		__retval = xmpir_mpf_div(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfUiDiv(MpfT rop, uint op1, MpfT op2)
	{
		int __retval;
		__retval = xmpir_mpf_ui_div(rop.val, op1, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfDivUi(MpfT rop, MpfT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpf_div_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSqrt(MpfT rop, MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_sqrt(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfSqrtUi(MpfT rop, uint op)
	{
		int __retval;
		__retval = xmpir_mpf_sqrt_ui(rop.val, op);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfPowUi(MpfT rop, MpfT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpf_pow_ui(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfNeg(MpfT rop, MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_neg(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfAbs(MpfT rop, MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_abs(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfMul2exp(MpfT rop, MpfT op1, ulong op2)
	{
		int __retval;
		__retval = xmpir_mpf_mul_2exp(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfDiv2exp(MpfT rop, MpfT op1, ulong op2)
	{
		int __retval;
		__retval = xmpir_mpf_div_2exp(rop.val, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpfCmp(MpfT op1, MpfT op2)
	{
		int __retval;
		__retval = xmpir_mpf_cmp(out var result, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfCmpD(MpfT op1, double op2)
	{
		int __retval;
		__retval = xmpir_mpf_cmp_d(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfCmpUi(MpfT op1, uint op2)
	{
		int __retval;
		__retval = xmpir_mpf_cmp_ui(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfCmpSi(MpfT op1, int op2)
	{
		int __retval;
		__retval = xmpir_mpf_cmp_si(out var result, op1.val, op2);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfEq(MpfT op1, MpfT op2, ulong op3)
	{
		int __retval;
		__retval = xmpir_mpf_eq(out var result, op1.val, op2.val, op3);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpfReldiff(MpfT rop, MpfT op1, MpfT op2)
	{
		int __retval;
		__retval = xmpir_mpf_reldiff(rop.val, op1.val, op2.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpfSgn(MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_sgn(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpfCeil(MpfT rop, MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_ceil(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfFloor(MpfT rop, MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_floor(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static void MpfTrunc(MpfT rop, MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_trunc(rop.val, op.val);
		if (__retval != 0) HandleError(__retval);
	}
	public static int MpfIntegerP(MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_integer_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfFitsUintP(MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_fits_uint_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static int MpfFitsSintP(MpfT op)
	{
		int __retval;
		__retval = xmpir_mpf_fits_sint_p(out var result, op.val);
		if (__retval != 0) HandleError(__retval);
		return result;
	}
	public static void MpfUrandomb(MpfT rop, GmpRandstateT state, ulong nbits)
	{
		int __retval;
		__retval = xmpir_mpf_urandomb(rop.val, state.val, nbits);
		if (__retval != 0) HandleError(__retval);
	}
}
