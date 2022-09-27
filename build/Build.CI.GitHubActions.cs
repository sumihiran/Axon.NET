using Nuke.Common.CI.GitHubActions;

[GitHubActions(
    "build",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    OnPushBranches = new []{ DevelopBranch, MainBranch, $"{FeatureBranchPrefix}/*" },
    OnPullRequestBranches = new[] { DevelopBranch, MainBranch },
    PublishArtifacts = true,
    CacheKeyFiles = new[] { "global.json", "src/**/*.csproj" },
    EnableGitHubToken = true,
    AutoGenerate = false)]
partial class Build
{
    static GitHubActions GitHubActions => GitHubActions.Instance;
}
