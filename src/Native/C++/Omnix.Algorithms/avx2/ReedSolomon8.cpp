#include "stdafx.h"
#include "ReedSolomon8.h"

#include "xmmintrin.h" //SSE
#include "emmintrin.h" //SSE2
#include "pmmintrin.h" //SSE3
#include "tmmintrin.h" //SSSE3
#include "smmintrin.h" //SSE4.1
#include "nmmintrin.h" //SSE4.2
#include "wmmintrin.h" //AES
#include "immintrin.h" //AVX

void ReedSolomon8_Mul(byte* src, byte* dst, byte* table, int32_t len)
{
    __m256i ymm0;
    __m256i ymm1;
    __m256i ymm2;
    __m256i ymm3;
    __m256i ymm4;
    __m256i ymm5;
    __m256i ymm6;
    __m256i ymm7;

    int32_t i = 0;

    // アライメントを揃える。
    for (; i < len; i++)
    {
        if (((uintptr_t)dst % 256) == 0) break;

        *dst++ ^= table[*src++];
    }

    for (int32_t count = ((len - i) / 128) - 1; count >= 0; count--)
    {
        ymm0 = _mm256_setr_epi8
        (
            table[*(src + 0)],
            table[*(src + 1)],
            table[*(src + 2)],
            table[*(src + 3)],

            table[*(src + 4)],
            table[*(src + 5)],
            table[*(src + 6)],
            table[*(src + 7)],

            table[*(src + 8)],
            table[*(src + 9)],
            table[*(src + 10)],
            table[*(src + 11)],

            table[*(src + 12)],
            table[*(src + 13)],
            table[*(src + 14)],
            table[*(src + 15)],

            table[*(src + 16)],
            table[*(src + 17)],
            table[*(src + 18)],
            table[*(src + 19)],

            table[*(src + 20)],
            table[*(src + 21)],
            table[*(src + 22)],
            table[*(src + 23)],

            table[*(src + 24)],
            table[*(src + 25)],
            table[*(src + 26)],
            table[*(src + 27)],

            table[*(src + 28)],
            table[*(src + 29)],
            table[*(src + 30)],
            table[*(src + 31)]
        );
        src += 32;

        ymm1 = _mm256_setr_epi8
        (
            table[*(src + 0)],
            table[*(src + 1)],
            table[*(src + 2)],
            table[*(src + 3)],

            table[*(src + 4)],
            table[*(src + 5)],
            table[*(src + 6)],
            table[*(src + 7)],

            table[*(src + 8)],
            table[*(src + 9)],
            table[*(src + 10)],
            table[*(src + 11)],

            table[*(src + 12)],
            table[*(src + 13)],
            table[*(src + 14)],
            table[*(src + 15)],

            table[*(src + 16)],
            table[*(src + 17)],
            table[*(src + 18)],
            table[*(src + 19)],

            table[*(src + 20)],
            table[*(src + 21)],
            table[*(src + 22)],
            table[*(src + 23)],

            table[*(src + 24)],
            table[*(src + 25)],
            table[*(src + 26)],
            table[*(src + 27)],

            table[*(src + 28)],
            table[*(src + 29)],
            table[*(src + 30)],
            table[*(src + 31)]
        );
        src += 32;

        ymm2 = _mm256_setr_epi8
        (
            table[*(src + 0)],
            table[*(src + 1)],
            table[*(src + 2)],
            table[*(src + 3)],

            table[*(src + 4)],
            table[*(src + 5)],
            table[*(src + 6)],
            table[*(src + 7)],

            table[*(src + 8)],
            table[*(src + 9)],
            table[*(src + 10)],
            table[*(src + 11)],

            table[*(src + 12)],
            table[*(src + 13)],
            table[*(src + 14)],
            table[*(src + 15)],

            table[*(src + 16)],
            table[*(src + 17)],
            table[*(src + 18)],
            table[*(src + 19)],

            table[*(src + 20)],
            table[*(src + 21)],
            table[*(src + 22)],
            table[*(src + 23)],

            table[*(src + 24)],
            table[*(src + 25)],
            table[*(src + 26)],
            table[*(src + 27)],

            table[*(src + 28)],
            table[*(src + 29)],
            table[*(src + 30)],
            table[*(src + 31)]
        );
        src += 32;

        ymm3 = _mm256_setr_epi8
        (
            table[*(src + 0)],
            table[*(src + 1)],
            table[*(src + 2)],
            table[*(src + 3)],

            table[*(src + 4)],
            table[*(src + 5)],
            table[*(src + 6)],
            table[*(src + 7)],

            table[*(src + 8)],
            table[*(src + 9)],
            table[*(src + 10)],
            table[*(src + 11)],

            table[*(src + 12)],
            table[*(src + 13)],
            table[*(src + 14)],
            table[*(src + 15)],

            table[*(src + 16)],
            table[*(src + 17)],
            table[*(src + 18)],
            table[*(src + 19)],

            table[*(src + 20)],
            table[*(src + 21)],
            table[*(src + 22)],
            table[*(src + 23)],

            table[*(src + 24)],
            table[*(src + 25)],
            table[*(src + 26)],
            table[*(src + 27)],

            table[*(src + 28)],
            table[*(src + 29)],
            table[*(src + 30)],
            table[*(src + 31)]
        );
        src += 32;

        ymm4 = _mm256_loadu_si256((__m256i*)(dst + (16 * 0)));
        ymm5 = _mm256_loadu_si256((__m256i*)(dst + (16 * 1)));
        ymm6 = _mm256_loadu_si256((__m256i*)(dst + (16 * 2)));
        ymm7 = _mm256_loadu_si256((__m256i*)(dst + (16 * 3)));

        ymm0 = _mm256_xor_si256(ymm0, ymm4);
        ymm1 = _mm256_xor_si256(ymm1, ymm5);
        ymm2 = _mm256_xor_si256(ymm2, ymm6);
        ymm3 = _mm256_xor_si256(ymm3, ymm7);

        _mm256_storeu_si256((__m256i*)(dst + (16 * 0)), ymm0);
        _mm256_storeu_si256((__m256i*)(dst + (16 * 1)), ymm1);
        _mm256_storeu_si256((__m256i*)(dst + (16 * 2)), ymm2);
        _mm256_storeu_si256((__m256i*)(dst + (16 * 3)), ymm3);

        dst += 128;
        i += 128;
    }

    for (; i < len; i++)
    {
        *dst++ ^= table[*src++];
    }
}
