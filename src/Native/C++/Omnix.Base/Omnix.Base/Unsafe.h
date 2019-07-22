#pragma once

extern "C" {

void zero(byte* src, int32_t len);
void copy(byte* src, byte* dst, int32_t len);
bool equals(byte* x, byte* y, int32_t len);
int32_t compare(byte* x, byte* y, int32_t len);
void math_and(byte* x, byte* y, byte* result, int32_t len);
void math_or(byte* x, byte* y, byte* result, int32_t len);
void math_xor(byte* x, byte* y, byte* result, int32_t len);

}
