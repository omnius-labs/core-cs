#pragma once

extern "C" {

    void BytesOperations_Zero(byte* src, int32_t len);
    void BytesOperations_Copy(byte* src, byte* dst, int32_t len);
    bool BytesOperations_Equals(byte* x, byte* y, int32_t len);
    int32_t BytesOperations_Compare(byte* x, byte* y, int32_t len);
    void BytesOperations_And(byte* x, byte* y, byte* result, int32_t len);
    void BytesOperations_Or(byte* x, byte* y, byte* result, int32_t len);
    void BytesOperations_Xor(byte* x, byte* y, byte* result, int32_t len);

}
