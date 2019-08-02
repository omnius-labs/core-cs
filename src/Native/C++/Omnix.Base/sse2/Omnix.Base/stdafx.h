// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include <stdio.h>
#include <stdint.h>

#ifdef WINDOWS
#include "targetver.h"
#include <windows.h>
#endif

#ifdef UNIX
typedef unsigned char byte;
#endif
