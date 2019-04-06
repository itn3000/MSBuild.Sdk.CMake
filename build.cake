var configuration = Argument("Configuration", "Debug");
var target = Argument("Target", "Default");

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack")
    ;
Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore("MSBuild.Sdk.CMake.slnproj");
    });
Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var buildSettings = new DotNetCoreBuildSettings();
        buildSettings.Configuration = configuration;
        DotNetCoreBuild("MSBuild.Sdk.CMake.slnproj", buildSettings);
    });
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        foreach(var projectName in new []{ "Install", "Regular" })
        {
            var regularProjectDir = DirectoryPath.FromString("tests").Combine(projectName);
            var projectFilePath = regularProjectDir.CombineWithFilePath($"{projectName}.cmproj");
            var cleanSettings = new DotNetCoreCleanSettings();
            cleanSettings.Configuration = configuration;
            DotNetCoreClean(projectFilePath.ToString(), cleanSettings);
            var buildSettings = new DotNetCoreBuildSettings();
            buildSettings.Configuration = configuration;
            DotNetCoreBuild(projectFilePath.ToString(), buildSettings);
        }
    });
Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
    {
        DotNetCorePack("MSBuild.Sdk.CMake.slnproj");
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