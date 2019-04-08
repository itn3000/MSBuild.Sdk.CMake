# MSBuild helper for CMake project

[![Build status](https://ci.appveyor.com/api/projects/status/8mg77qa079jkia26/branch/master?svg=true)](https://ci.appveyor.com/project/itn3000/msbuild-sdk-cmake/branch/master)

This is the [MSBuild SDK](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk?view=vs-2019) which does configure,build,and other task for [CMake](https://cmake.org) project.

## packages

* [MSBuild.Sdk.CMake](https://www.nuget.org/packages/MSBuild.Sdk.CMake/)
* [MSBUild.Sdk.CMake.Template.Console](https://www.nuget.org/packages/MSBuild.Sdk.CMake.Template.Console/)
* [MSBUild.Sdk.CMake.Template.Library](https://www.nuget.org/packages/MSBuild.Sdk.CMake.Template.Library/)

# How to use

## Requirements

* [cmake](https://cmake.org)
    * you should add path to `cmake` to "PATH" environment variable
* [dotnet core sdk 2.1.300 or later](https://dotnet.microsoft.com/download)
* build tools for C/C++
    * **Depends on CMake**

## Steps from scratch

1. [creating initial cmake project](https://cmake.org/cmake-tutorial/)
2. create msbuild project file like following next to CMakeLists.txt
```xml
<Project Sdk="MSBuild.Sdk.CMake/[version of this package]">
</Project>
```
3. execute `dotnet build [project file]`
    * if you want to get release build, `dotnet build -c Release [project file]`
    * if you do not want to add cmake to PATH, `CMakeExe` property can be used
        * property can be spcified by `dotnet build -p:CMakeExe=/path/to/cmake`

Then you will get built binary in `/dir/to/project/builddir/[Configuration]/[Platform(default is 'any')]/[Configuration]`

## from template

if you want to create project from `dotnet new`, you can do it by installing templates for cmake project.

1. do `dotnet new -i MSBuild.Sdk.CMake.Template.Library` to install library template
2. do `dotnet new -i MSBuild.Sdk.CMake.Template.Console` to install console template
3. do `dotnet new cmlib --name [project name]`, then cmake library project will be created
4. do `dotnet new cmconsole --name [project name]`, then cmake console project will be created
5. after project creation, you can build project by `dotnet build`

## configure only

By default, `build` task execute `Configure` task before main build.
If you want to configure project only, execute `dotnet msbuild [path to project file] /t:Configure`

## clean project

if you want to clean up the built binary, execute `dotnet clean [path to project file]`.
Note this target only delete CMake cache file(CMakeCache.txt) and some intermediate files.
if you want to delete whole directory including all intermediated objects, execute `dotnet msbuild /t:AllClean`

## executing custom target

if you want to custom cmake target(like `install`), execute `dotnet msbuild [path to project file] /t:ExecuteTarget /p:CMakeTarget=[your custom target name]`.
Or you can add custom target by edit project file.

```xml
<Project Sdk="MSBuild.Sdk.CMake/0.1.0">
    <Target Name="Install">
        <MSBuild Projects="$(MSBuildThisFileFullPath)" 
            Targets="ExecuteTarget" 
            Properties="CMakeTarget=install;Configuration=$(Configuration);Platform=$(Platform)"/>
    </Target>
</Project>
```

and then, execute `dotnet msbuild /t:Install`

# References

## Supported Items

Following item can be added to `ItemGroup`

### CMakeDefine

Definitions passed when `Configure` task.
you must set `Value` metadata.

for example;

```
<ItemGroup>
    <CMakeDefine Include="A" Value="B"/>
</ItemGroup>
```

then passed `-D"A=B"` to `Configure` task.

## Supported Properties

### CMakeExe

Path to `cmake` executable file,default is `cmake`

### CMakeGenerator

Name of cmake generator(available items can be seen by `cmake --help`)

### CMakePlatform

build platform, default using msbuild's `$(Platform)` value, or `default` if not set `$(Platform)`

### CMakeBuildType

build configuration, default using msbuild's `$(Configuration)` value

### CMakeInstallPrefix

the value passed as CMAKE_INSTALL_PREFIX when configure task.

## Supported Targets

### Configure

do cmake configure task(`cd $(CMakeBuildRootDirectory) && cmake $(CMakeRootDirectory)`)

### Build

do cmake's default build target.
depends on `Congiure` task.

### BuildClean

do cmake's `clean` target.
This task will be skipped if `Configure` task is not executed.

### ConfigClean

delete `$(CMakeBuildDirectory)/CMakeCache.txt`.

### Clean

do `BuildClean` and `ConfigClean`

### Rebuild

do `Clean` and `Build`

### AllClean

delete `$(CMakeBuildRootDirectory)`

### ExecuteTarget

do custom cmake target.

### Install

do cmake `install` target
