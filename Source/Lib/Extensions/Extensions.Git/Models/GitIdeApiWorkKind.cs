namespace Luthetus.Extensions.Git.Models;

public enum GitIdeApiWorkKind
{
	None,
    Status,
    GetActiveBranchName,
    GetOriginName,
    Add,
    Unstage,
    Commit,
    BranchNew,
    BranchGetAll,
    BranchSet,
    PushToOriginWithTracking,
    Pull,
    Fetch,
    LogFile,
    ShowFile,
    DiffFile
}
