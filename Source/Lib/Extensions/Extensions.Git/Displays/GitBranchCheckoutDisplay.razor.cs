using Microsoft.AspNetCore.Components;
using Luthetus.Extensions.Git.Models;
using Luthetus.Extensions.Git.BackgroundTasks.Models;

namespace Luthetus.Extensions.Git.Displays;

public partial class GitBranchCheckoutDisplay : ComponentBase
{
    [Inject]
    private GitBackgroundTaskApi GitBackgroundTaskApi { get; set; } = null!;

    [Parameter, EditorRequired]
    public GitState GitState { get; set; } = null!;

    private void SetActiveBranchOnClick(GitState localGitState, string branchName)
    {
        if (localGitState.Repo is null)
            return;

        GitBackgroundTaskApi.Git.Enqueue(new GitIdeApiWorkArgs
        {
        	WorkKind = GitIdeApiWorkKind.BranchSet,
        	RepoAtTimeOfRequest = localGitState.Repo,
        	BranchName = branchName
    	});
    }
}