using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

public partial class Build : NukeBuild
{
    [Parameter] static string NuGetSource => $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json";
    [Parameter] [Secret] static string NuGetApiKey => GitHubActions?.Token ?? "";

    Target Publish => _ => _
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey)
        .Executes(() =>
        {
            DotNetNuGetPush(_ => _
                    .SetSource(NuGetSource)
                    .SetApiKey(NuGetApiKey)
                    .EnableSkipDuplicate()
                    .EnableNoSymbols()
                    .CombineWith(PushPackageFiles, (_, v) => _
                        .SetTargetPath(v)),
                PushDegreeOfParallelism,
                PushCompleteOnFailure);
        });

    static IEnumerable<AbsolutePath> PushPackageFiles => PackagesDirectory.GlobFiles(ArtifactsDirectory, "*.nupkg");

    static bool PushCompleteOnFailure => true;

    static int PushDegreeOfParallelism => 1;
}
