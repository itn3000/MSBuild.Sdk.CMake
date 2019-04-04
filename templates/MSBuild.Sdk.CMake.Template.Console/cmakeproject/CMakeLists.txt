cmake_minimum_required(VERSION 2.8)
set(project_name "cmakeproject")
project("${project_name}")
file(GLOB SRC *.cpp *.h)
add_executable(${project_name} ${SRC})
install(TARGETS ${project_name} 
    RUNTIME DESTINATION bin)