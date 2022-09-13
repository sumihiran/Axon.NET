using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Git;
using Nuke.Common.Tools.Coverlet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.IO.FileSystemTasks;

[DisableDefaultOutput(DefaultOutput.Logo)]
partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Default);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? global::Configuration.Debug : global::Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    static AbsolutePath SourceDirectory => RootDirectory / "src";
    static AbsolutePath TestsDirectory => RootDirectory / "tests";

    static AbsolutePath OutputDirectory => RootDirectory / "output";

    const string MainBranch = "main";
    const string DevelopBranch = "develop";
    const string FeatureBranchPrefix = "feature";

    Target Clean => _ => _
        .Description("Cleans the output, bin and obj directories.")
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(OutputDirectory);
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
        .Description("Restores NuGet packages.")
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .Description("Compile the solution.")
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetNoRestore(InvokedTargets.Contains(Restore))
                .SetConfiguration(Configuration));
        });

    static AbsolutePath TestResultDirectory => OutputDirectory / "test-results";

    static AbsolutePath CoverageResultDirectory => OutputDirectory / "test-coverage";
    IEnumerable<Project> TestProjects => Partition.GetCurrent(Solution.GetProjects("*.Tests"));

    Target Test => _ => _
        .Description("Runs unit tests and outputs test results to the output directory.")
        .DependsOn(Compile)
        .Partition(2)
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetConfiguration(Configuration)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .ResetVerbosity()
                .SetResultsDirectory(TestResultDirectory)
                .EnableCollectCoverage()
                .SetCoverletOutput(CoverageResultDirectory / "coverage")
                .SetCoverletOutputFormat("json%2copencover")
                .AddProperty("MergeWith", CoverageResultDirectory / "coverage.json")
                .CombineWith(TestProjects, (_, v) => _
                    .SetProjectFile(v)
                    .SetLoggers($"trx;LogFileName={v.Name}.trx")));
        });

    Target Default => _ => _.Description("Cleans, restores NuGet packages, compile the solution, runs unit tests.")
        .DependsOn(Clean)
        .DependsOn(Compile)
        .DependsOn(Test);
}
