var configuration = Argument("Configuration", "Debug");
var target = Argument("Target", "Default");

Task("Default")
    .IsDependentOn("Build")
    ;
Task("Restore")
    .Does(() =>
    {
        MSBuild("MSBuild.Sdk.CMake.slnproj", setting =>
        {
            setting.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Normal)
                .WithTarget("Restore")
                ;
        });
    });
Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        MSBuild("MSBuild.Sdk.CMake.slnproj", setting =>
        {
            setting.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Normal)
                .WithTarget("Build")
                ;
        });
    });
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var regularProjectDir = DirectoryPath.FromString("tests").Combine("Regular");
        var projectFilePath = regularProjectDir.CombineWithFilePath("Regular.cmproj");
        DotNetCoreClean(projectFilePath.ToString());
        DotNetCoreBuild(projectFilePath.ToString());
    });
Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
    {
        MSBuild("MSBuild.Sdk.CMake.slnproj", setting =>
        {
            setting.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Normal)
                .WithTarget("Pack")
                ;
        });
        foreach(var pkgname in new []{ "MSBuild.Sdk.CMake.Template.Console", "MSBuild.Sdk.CMake.Template.Library" })
        {
            string processName;
            ProcessArgumentBuilder args;
            if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                processName = "mono";
                args = new ProcessArgumentBuilder().Append(
                        DirectoryPath.FromString("buildtools").CombineWithFilePath("nuget.exe").ToString())
                    .Append("pack")
                    .Append(
                        DirectoryPath.FromString("templates").Combine(pkgname).CombineWithFilePath($"{pkgname}.nuspec").ToString()
                    )
                    .Append("-OutputDirectory")
                    .Append("nupkg")
                    ;
            }
            else
            {
                processName = DirectoryPath.FromString("buildtools").CombineWithFilePath("nuget.exe").ToString();
                args = new ProcessArgumentBuilder()
                    .Append("pack")
                    .Append(
                        DirectoryPath.FromString("templates").Combine(pkgname).CombineWithFilePath($"{pkgname}.nuspec").ToString()
                    )
                    .Append("-OutputDirectory")
                    .Append("nupkg")
                    ;
            }
            var exitCode = StartProcess(processName, new ProcessSettings(){ Arguments = args });
            if(exitCode != 0)
            {
                throw new Exception($"failed to execute nuget pack({exitCode})");
            }
        }
    });
RunTarget(target);