cmake_minimum_required(VERSION 2.8)
set(project_name "cmakeproject")
project("${project_name}")
file(GLOB SRC *.cpp *.h)

include_directories(${PROJECT_BINARY_DIR})

add_library(${project_name} ${SRC})

include (GenerateExportHeader)
GENERATE_EXPORT_HEADER( ${project_name}
	BASE_NAME ${project_name}
	EXPORT_MACRO_NAME ${project_name}_EXPORT
	EXPORT_FILE_NAME ${project_name}_Export.h
	STATIC_DEFINE ${project_name}_BUILT_AS_STATIC
)

install(TARGETS ${project_name} DESTINATION bin)
install(FILES 
    "${PROJECT_SOURCE_DIR}/${project_name}.h" 
    "${PROJECT_BINARY_DIR}/${project_name}_Export.h" 
    DESTINATION include)