#addin "nuget:?package=Cake.ExtendedNuGet&version=1.0.0.27"
#addin "nuget:?package=NuGet.Core&version=2.14.0"
#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"

var target = Argument("Target", "Build");

FilePath SolutionFile = MakeAbsolute(File("HubSpotSdk.sln"));

var testFolder = SolutionFile.GetDirectory().Combine("tests");
var outputFolder = SolutionFile.GetDirectory().Combine("outputs");
var testOutputFolder = outputFolder.Combine("tests");
var coverageOutputFile = testOutputFolder.CombineWithFilePath("coverage.dcvr");
var dotCoverFolder = MakeAbsolute(Context.Tools.Resolve("dotcover.exe").GetDirect‌​ory());


Setup(context => 
{
    CleanDirectory(outputFolder);
});

Task("Restore")
    .Does(() =>
{
    var apiKey = EnvironmentVariable("EMGPrivateApiKey") ?? throw new ArgumentNullException("EMGPrivateApiKey");
    
    var settings = new DotNetCoreRestoreSettings {
        Sources = new []{
            "https://api.nuget.org/v3/index.json",
            "https://www.myget.org/F/emg/api/v3/index.json",
            $"https://www.myget.org/F/emgprivate/auth/{apiKey}/api/v3/index.json"
        }
    };

    DotNetCoreRestore(SolutionFile.FullPath, settings);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = "Debug"
    };

    DotNetCoreBuild(SolutionFile.FullPath, settings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() => 
{
    Information($"Looking for test projects in {testFolder.FullPath}");

    var testProjects = GetFiles(testFolder, "*.csproj", SearchOption.AllDirectories);

    var dotCoverSettings = new DotCoverCoverSettings()
                                    .WithFilter("+:HubSpot*")
                                    .WithFilter("+:EMG.*")
                                    .WithFilter("-:Tests.*");

    foreach (var project in testProjects)
    {
        Information($"Testing {project.FullPath}");

        var testResultFile = testOutputFolder.CombineWithFilePath(project.GetFilenameWithoutExtension() + ".trx");
        var coverageResultFile = testOutputFolder.CombineWithFilePath(project.GetFilenameWithoutExtension() + ".dvcr");
        
        Verbose($"Saving test results on {testResultFile.FullPath}");

        var settings = new DotNetCoreTestSettings
        {
            NoBuild = true,
            NoRestore = true,
            Logger = $"trx;LogFileName={testResultFile.FullPath}"
        };

        DotCoverCover(context => 
        {
                context.DotNetCoreTest(project.FullPath, settings);
        }, coverageResultFile, dotCoverSettings);

        if (BuildSystem.IsRunningOnTeamCity)
        {
            TeamCity.ImportData("mstest", testResultFile);
        }
    }

    var coverageFiles = GetFiles(testOutputFolder, "*.dvcr");
    DotCoverMerge(coverageFiles, coverageOutputFile);
    DeleteFiles(coverageFiles);

    if (BuildSystem.IsRunningOnTeamCity)
    {
        TeamCity.ImportDotCoverCoverage(coverageOutputFile, dotCoverFolder);
    }
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
    var packSettings = new DotNetCorePackSettings 
    {
        Configuration = "Release",
        OutputDirectory = outputFolder
    };

    DotNetCorePack(SolutionFile.FullPath, packSettings);
});

Task("Push")
    .IsDependentOn("Pack")
    .Does(() =>
{
    var apiKey = EnvironmentVariable("EMGPrivateApiKey");
    var source = $"https://www.myget.org/F/emgprivate/auth/{apiKey}/api/v3/index.json";

    var settings = new DotNetCoreNuGetPushSettings
    {
        Source = source,
        ApiKey = apiKey
    };

    var files = GetFiles(outputFolder, "*.nupkg");

    foreach (var file in files)
    {
        var fileName = file.GetFilename();

        if (!IsNuGetPublished(file, $"https://www.myget.org/F/emgprivate/auth/{apiKey}/api/v2"))
        {
            Information($"Pushing {fileName}");

            DotNetCoreNuGetPush(file.FullPath, settings);
            Information($"{fileName} pushed!");
        }
        else
        {
            Warning($"{fileName} already published, removing from artifacts");
            DeleteFile(file);
        }
    }
});

Task("Full")
    .IsDependentOn("Pack");

RunTarget(target);

public static IEnumerable<FilePath> GetFiles(DirectoryPath directory, string pattern = "*.*", SearchOption option = SearchOption.TopDirectoryOnly)
{
    var files = System.IO.Directory.GetFiles(directory.FullPath, pattern, option);
    return files.Select(file => (FilePath)file);
}
