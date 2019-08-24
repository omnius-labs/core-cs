#!/bin/make
CC = gcc
CXX = g++

SRCS = Crc32_Castagnoli.cpp ReedSolomon8.cpp
TARGET = omnix-algorithms.so
BUILD = bin

$(TARGET):
	mkdir -p $(BUILD)
	$(CXX) $(CXXFLAGS) -O3 -D UNIX -shared -fPIC -o $(BUILD)/$(TARGET) $(SRCS)

clean:
	rm -R $(BUILD)
