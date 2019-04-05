cmake_minimum_required(VERSION 2.8)

add_executable(installtest main.cpp)

install (TARGETS installtest DESTINATION bin)