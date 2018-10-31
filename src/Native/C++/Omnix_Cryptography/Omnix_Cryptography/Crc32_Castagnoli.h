#pragma once

class Crc32_Castagnoli
{
public:
    Crc32_Castagnoli();
    ~Crc32_Castagnoli();

    uint32_t compute(uint32_t x, byte* source, int32_t length) const;

private:
    uint32_t _table[256];
};

const Crc32_Castagnoli _crc32_castagnoli;

extern "C" {

uint32_t compute_Crc32_Castagnoli(uint32_t x, byte* source, int32_t length);

}
