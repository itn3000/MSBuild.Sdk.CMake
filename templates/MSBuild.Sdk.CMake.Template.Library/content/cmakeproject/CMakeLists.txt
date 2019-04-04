cmake_minimum_required(VERSION 2.8)
set(project_name "cmakeproject")
project("${project_name}")
file(GLOB SRC *.cpp *.h)

include_directories(${PROJECT_BINARY_DIR})

include (GenerateExportHeader)

add_library(${project_name} SHARED ${SRC} ${PROJECT_BINARY_DIR}/${project_name}_export.h)
add_library(${project_name}_static STATIC ${SRC} ${PROJECT_BINARY_DIR}/${project_name}_export.h)

GENERATE_EXPORT_HEADER( ${project_name}
	BASE_NAME ${project_name}
	EXPORT_MACRO_NAME ${project_name}_EXPORT
	EXPORT_FILE_NAME ${project_name}_export.h
	STATIC_DEFINE ${project_name}_BUILT_AS_STATIC
)

add_definitions(-D${project_name}_EXPORTS)
install(TARGETS ${project_name} ${project_name}_static
	RUNTIME DESTINATION bin
	LIBRARY DESTINATION lib
	ARCHIVE DESTINATION lib
	PUBLIC_HEADER DESTINATION include)
# if("${LIB_TYPE}" EQUAL "SHARED")
# 	install(TARGETS ${project_name} DESTINATION bin)
# else()
# 	install(TARGETS ${project_name} DESTINATION lib)
# endif()

install(FILES 
    "${PROJECT_SOURCE_DIR}/${project_name}.h" 
    "${PROJECT_BINARY_DIR}/${project_name}_export.h" 
    DESTINATION include)