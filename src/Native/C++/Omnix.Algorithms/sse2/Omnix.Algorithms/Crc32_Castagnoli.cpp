#include "stdafx.h"
#include "Crc32_Castagnoli.h"

Crc32_Castagnoli::Crc32_Castagnoli()
{
    const uint32_t poly = 0x82F63B78;

    for (uint32_t i = 0; i < 256; i++)
    {
        uint32_t x = i;

        for (uint32_t j = 0; j < 8; j++)
        {
            if ((x & 1) != 0)
            {
                x = (x >> 1) ^ poly;
            }
            else
            {
                x >>= 1;
            }
        }

        _table[i] = x;
    }
}

Crc32_Castagnoli::~Crc32_Castagnoli()
{

}

uint32_t Crc32_Castagnoli::Compute(const uint32_t x, const byte* source, const int32_t length) const
{
    int v = x;

    for (int32_t i = 0; i < length; i++)
    {
        v = (v >> 8) ^ _table[((byte)(v & 0xff)) ^ source[i]];
    }

    return v;
}

uint32_t Crc32_Castagnoli_Compute(const uint32_t x, const byte* source, const int32_t length)
{
    return _crc32_castagnoli.Compute(x, source, length);
}
