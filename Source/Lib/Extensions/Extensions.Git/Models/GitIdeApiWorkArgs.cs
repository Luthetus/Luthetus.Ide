namespace Luthetus.Extensions.Git.Models;

public struct GitIdeApiWorkArgs
{
	public GitIdeApiWorkKind WorkKind { get; set; }
    public GitRepo RepoAtTimeOfRequest { get; set; }
    public string CommitSummary { get; set; }
    public string BranchName { get; set; }
    public string RelativePathToFile { get; set; }
    public Func<GitCliOutputParser, string, Task> NoIntCallback { get; set; }
    public Func<GitCliOutputParser, string, List<int>, Task> WithIntCallback { get; set; }
}
