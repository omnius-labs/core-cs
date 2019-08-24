#include "stdafx.h"
#include "BytesOperations.h"

#if _WIN64 || __amd64__
#define PORTABLE_64_BIT
#else
#define PORTABLE_32_BIT
#endif

#include "xmmintrin.h" //SSE
#include "emmintrin.h" //SSE2
//#include "pmmintrin.h" //SSE3
//#include "tmmintrin.h" //SSSE3
//#include "smmintrin.h" //SSE4.1
//#include "nmmintrin.h" //SSE4.2
//#include "wmmintrin.h" //AES
//#include "immintrin.h" //AVX

void BytesOperations_Zero(byte* src, int32_t len)
{
	memset(src, 0x00, len * sizeof(byte));
}

void BytesOperations_Copy(byte* src, byte* dst, int32_t len)
{
#if defined (PORTABLE_64_BIT)
	if (len >= 16)
	{
		for (int32_t i = (len / 16) - 1; i >= 0; i--, src += 16, dst += 16)
		{
			__m128i xmm_target = _mm_loadu_si128((__m128i*)src);
			_mm_storeu_si128((__m128i*)dst, xmm_target);
		}
	}

	if ((len & 8) != 0)
	{
		*((uint64_t*)dst) = *((uint64_t*)src);
		dst += 8; src += 8;
	}

	if ((len & 4) != 0)
	{
		*((uint32_t*)dst) = *((uint32_t*)src);
		dst += 4; src += 4;
	}

	if ((len & 2) != 0)
	{
		*((uint16_t*)dst) = *((uint16_t*)src);
		dst += 2; src += 2;
	}

	if ((len & 1) != 0)
	{
		*((byte*)dst) = *((byte*)src);
	}
#elif defined (PORTABLE_32_BIT)
	if (len >= 16)
	{
		for (int32_t i = (len / 16) - 1; i >= 0; i--, src += 16, dst += 16)
		{
			__m128i xmm_target = _mm_loadu_si128((__m128i*)src);
			_mm_storeu_si128((__m128i*)dst, xmm_target);
		}
	}

	if ((len & 8) != 0)
	{
		*((uint32_t*)dst) = *((uint32_t*)src);
		dst += 4; src += 4;
		*((uint32_t*)dst) = *((uint32_t*)src);
		dst += 4; src += 4;
	}

	if ((len & 4) != 0)
	{
		*((uint32_t*)dst) = *((uint32_t*)src);
		dst += 4; src += 4;
	}

	if ((len & 2) != 0)
	{
		*((uint16_t*)dst) = *((uint16_t*)src);
		dst += 2; src += 2;
	}

	if ((len & 1) != 0)
	{
		*((byte*)dst) = *((byte*)src);
	}
#endif
}

// https://gist.github.com/karthick18/1361842
bool BytesOperations_Equals(byte* x, byte* y, int32_t len)
{
#if defined (PORTABLE_64_BIT)
	if (len >= 16)
	{
		for (int32_t i = (len / 16) - 1; i >= 0; i--, x += 16, y += 16)
		{
			__m128i xmm_x = _mm_loadu_si128((__m128i*)x);
			__m128i xmm_y = _mm_loadu_si128((__m128i*)y);
			__m128i xmm_cmp = _mm_cmpeq_epi16(xmm_x, xmm_y);
			if ((uint16_t)_mm_movemask_epi8(xmm_cmp) != (uint16_t)0xffff) return false;
		}
	}

	if ((len & 8) != 0)
	{
		if (*((uint64_t*)x) != *((uint64_t*)y)) return false;
		x += 8; y += 8;
	}

	if ((len & 4) != 0)
	{
		if (*((uint32_t*)x) != *((uint32_t*)y)) return false;
		x += 4; y += 4;
	}

	if ((len & 2) != 0)
	{
		if (*((uint16_t*)x) != *((uint16_t*)y)) return false;
		x += 2; y += 2;
	}

	if ((len & 1) != 0)
	{
		if (*((byte*)x) != *((byte*)y)) return false;
	}

	return true;
#elif defined (PORTABLE_32_BIT)
	if (len >= 16)
	{
		for (int32_t i = (len / 16) - 1; i >= 0; i--, x += 16, y += 16)
		{
			__m128i xmm_x = _mm_loadu_si128((__m128i*)x);
			__m128i xmm_y = _mm_loadu_si128((__m128i*)y);
			__m128i xmm_cmp = _mm_cmpeq_epi16(xmm_x, xmm_y);
			if ((uint16_t)_mm_movemask_epi8(xmm_cmp) != (uint16_t)0xffff) return false;
		}
	}

	if ((len & 8) != 0)
	{
		if (*((uint32_t*)x) != *((uint32_t*)y)) return false;
		x += 4; y += 4;
		if (*((uint32_t*)x) != *((uint32_t*)y)) return false;
		x += 4; y += 4;
	}

	if ((len & 4) != 0)
	{
		if (*((uint32_t*)x) != *((uint32_t*)y)) return false;
		x += 4; y += 4;
	}

	if ((len & 2) != 0)
	{
		if (*((uint16_t*)x) != *((uint16_t*)y)) return false;
		x += 2; y += 2;
	}

	if ((len & 1) != 0)
	{
		if (*((byte*)x) != *((byte*)y)) return false;
	}

	return true;
#endif
}

int32_t BytesOperations_Compare(byte* x, byte* y, int32_t len)
{
	int32_t c = 0;

	for (; len > 0; len--)
	{
		c = (int32_t)*x++ - (int32_t)*y++;
		if (c != 0) return c;
	}

	return 0;
}

void BytesOperations_And(byte* x, byte* y, byte* result, int32_t len)
{
#if defined (PORTABLE_64_BIT)
	if (len >= 16)
	{
		for (int32_t i = (len / 16) - 1; i >= 0; i--, x += 16, y += 16, result += 16)
		{
			__m128i xmm_x = _mm_loadu_si128((__m128i*)x);
			__m128i xmm_y = _mm_loadu_si128((__m128i*)y);
			__m128i xmm_res = _mm_and_si128(xmm_x, xmm_y);
			_mm_storeu_si128((__m128i*)result, xmm_res);
		}
	}

	if ((len & 8) != 0)
	{
		*((uint64_t*)result) = *((uint64_t*)x) & *((uint64_t*)y);
		x += 8; y += 8; result += 8;
	}

	if ((len & 4) != 0)
	{
		*((uint32_t*)result) = *((uint32_t*)x) & *((uint32_t*)y);
		x += 4; y += 4; result += 4;
	}

	if ((len & 2) != 0)
	{
		*((uint16_t*)result) = *((uint16_t*)x) & *((uint16_t*)y);
		x += 2; y += 2; result += 2;
	}

	if ((len & 1) != 0)
	{
		*((byte*)result) = (byte)(*((byte*)x) & *((byte*)y));
	}
#elif defined (PORTABLE_32_BIT)
	if (len >= 16)
	{
		for (int32_t i = (len / 16) - 1; i >= 0; i--, x += 16, y += 16, result += 16)
		{
			__m128i xmm_x = _mm_loadu_si128((__m128i*)x);
			__m128i xmm_y = _mm_loadu_si128((__m128i*)y);
			__m128i xmm_res = _mm_and_si128(xmm_x, xmm_y);
			_mm_storeu_si128((__m128i*)result, xmm_res);
		}
	}

	if ((len & 8) != 0)
	{
		*((uint32_t*)result) = *((uint32_t*)x) & *((uint32_t*)y);
		x += 4; y += 4; result += 4;
		*((uint32_t*)result) = *((uint32_t*)x) & *((uint32_t*)y);
		x += 4; y += 4; result += 4;
	}

	if ((len & 4) != 0)
	{
		*((uint32_t*)result) = *((uint32_t*)x) & *((uint32_t*)y);
		x += 4; y += 4; result += 4;
	}

	if ((len & 2) != 0)
	{
		*((uint16_t*)result) = *((uint16_t*)x) & *((uint16_t*)y);
		x += 2; y += 2; result += 2;
	}

	if ((len & 1) != 0)
	{
		*((byte*)result) = (byte)(*((byte*)x) & *((byte*)y));
	}
#endif
}

void BytesOperations_Or(byte* x, byte* y, byte* result, int32_t len)
{
#if defined (PORTABLE_64_BIT)
	if (len >= 16)
	{
		for (int32_t i = (len / 16) - 1; i >= 0; i--, x += 16, y += 16, result += 16)
		{
			__m128i xmm_x = _mm_loadu_si128((__m128i*)x);
			__m128i xmm_y = _mm_loadu_si128((__m128i*)y);
			__m128i xmm_res = _mm_or_si128(xmm_x, xmm_y);
			_mm_storeu_si128((__m128i*)result, xmm_res);
		}
	}

	if ((len & 8) != 0)
	{
		*((uint64_t*)result) = *((uint64_t*)x) | *((uint64_t*)y);
		x += 8; y += 8; result += 8;
	}

	if ((len & 4) != 0)
	{
		*((uint32_t*)result) = *((uint32_t*)x) | *((uint32_t*)y);
		x += 4; y += 4; result += 4;
	}

	if ((len & 2) != 0)
	{
		*((uint16_t*)result) = *((uint16_t*)x) | *((uint16_t*)y);
		x += 2; y += 2; result += 2;
	}

	if ((len & 1) != 0)
	{
		*((byte*)result) = (byte)(*((byte*)x) | *((byte*)y));
	}
#elif defined (PORTABLE_32_BIT)
	if (len >= 16)
	{
		for (int32_t i = (len / 16) - 1; i >= 0; i--, x += 16, y += 16, result += 16)
		{
			__m128i xmm_x = _mm_loadu_si128((__m128i*)x);
			__m128i xmm_y = _mm_loadu_si128((__m128i*)y);
			__m128i xmm_res = _mm_or_si128(xmm_x, xmm_y);
			_mm_storeu_si128((__m128i*)result, xmm_res);
		}
	}

	if ((len & 8) != 0)
	{
		*((uint32_t*)result) = *((uint32_t*)x) | *((uint32_t*)y);
		x += 4; y += 4; result += 4;
		*((uint32_t*)result) = *((uint32_t*)x) | *((uint32_t*)y);
		x += 4; y += 4; result += 4;
	}

	if ((len & 4) != 0)
	{
		*((uint32_t*)result) = *((uint32_t*)x) | *((uint32_t*)y);
		x += 4; y += 4; result += 4;
	}

	if ((len & 2) != 0)
	{
		*((uint16_t*)result) = *((uint16_t*)x) | *((uint16_t*)y);
		x += 2; y += 2; result += 2;
	}

	if ((len & 1) != 0)
	{
		*((byte*)result) = (byte)(*((byte*)x) | *((byte*)y));
	}
#endif
}

void BytesOperations_Xor(byte* x, byte* y, byte* result, int32_t len)
{
#if defined (PORTABLE_64_BIT)
	if (len >= 16)
	{
		for (int32_t i = (len / 16) - 1; i >= 0; i--, x += 16, y += 16, result += 16)
		{
			__m128i xmm_x = _mm_loadu_si128((__m128i*)x);
			__m128i xmm_y = _mm_loadu_si128((__m128i*)y);
			__m128i xmm_res = _mm_xor_si128(xmm_x, xmm_y);
			_mm_storeu_si128((__m128i*)result, xmm_res);
		}
	}

	if ((len & 8) != 0)
	{
		*((uint64_t*)result) = *((uint64_t*)x) ^ *((uint64_t*)y);
		x += 8; y += 8; result += 8;
	}

	if ((len & 4) != 0)
	{
		*((uint32_t*)result) = *((uint32_t*)x) ^ *((uint32_t*)y);
		x += 4; y += 4; result += 4;
	}

	if ((len & 2) != 0)
	{
		*((uint16_t*)result) = *((uint16_t*)x) ^ *((uint16_t*)y);
		x += 2; y += 2; result += 2;
	}

	if ((len & 1) != 0)
	{
		*((byte*)result) = (byte)(*((byte*)x) ^ *((byte*)y));
	}
#elif defined (PORTABLE_32_BIT)
	if (len >= 16)
	{
		for (int32_t i = (len / 16) - 1; i >= 0; i--, x += 16, y += 16, result += 16)
		{
			__m128i xmm_x = _mm_loadu_si128((__m128i*)x);
			__m128i xmm_y = _mm_loadu_si128((__m128i*)y);
			__m128i xmm_res = _mm_xor_si128(xmm_x, xmm_y);
			_mm_storeu_si128((__m128i*)result, xmm_res);
		}
	}

	if ((len & 8) != 0)
	{
		*((uint32_t*)result) = *((uint32_t*)x) ^ *((uint32_t*)y);
		x += 4; y += 4; result += 4;
		*((uint32_t*)result) = *((uint32_t*)x) ^ *((uint32_t*)y);
		x += 4; y += 4; result += 4;
	}

	if ((len & 4) != 0)
	{
		*((uint32_t*)result) = *((uint32_t*)x) ^ *((uint32_t*)y);
		x += 4; y += 4; result += 4;
	}

	if ((len & 2) != 0)
	{
		*((uint16_t*)result) = *((uint16_t*)x) ^ *((uint16_t*)y);
		x += 2; y += 2; result += 2;
	}

	if ((len & 1) != 0)
	{
		*((byte*)result) = (byte)(*((byte*)x) ^ *((byte*)y));
	}
#endif
}
