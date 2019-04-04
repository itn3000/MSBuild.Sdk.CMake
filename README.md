# MSBuild helper for CMake project

This is the [MSBuild SDK](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk?view=vs-2019) which does configure,build,and other task for [CMake](https://cmake.org) project.

# How to use

## Requirements

* [cmake](https://cmake.org)
* [dotnet core sdk 2.1.300 or later](https://dotnet.microsoft.com/download)

## Steps from scratch

**Warning:the nuget package has not been released yet**

1. [creating initial cmake project](https://cmake.org/cmake-tutorial/)
2. create msbuild project file like following next to CMakeLists.txt
```xml
<Project Sdk="MSBuild.Sdk.CMake/[version of this package]">
</Project>
```
3. execute `dotnet build [project file]`
    * if you want to get release build, `dotnet build -c Release [project file]`

Then you will get built binary in `/dir/to/project/builddir/[Configuration]/[Platform(default is 'any')]/[Configuration]`

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