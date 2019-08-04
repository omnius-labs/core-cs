#pragma once

class Crc32_Castagnoli
{
public:
    Crc32_Castagnoli();
    ~Crc32_Castagnoli();

    uint32_t Compute(const uint32_t x, const byte* source, const int32_t length) const;

private:
    uint32_t _table[256];
};

const Crc32_Castagnoli _crc32_castagnoli;

extern "C" {

    uint32_t Crc32_Castagnoli_Compute(const uint32_t x, const byte* source, const int32_t length);

}
