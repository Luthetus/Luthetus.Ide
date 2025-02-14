namespace Luthetus.Extensions.Git.Models;

public enum GitIdeApiWorkKind
{
    Status,
    GetActiveBranchName,
    GetOriginName,
    Add,
    Unstage,
    Commit,
    BranchNew,
    BranchGetAll,
    BranchSet
}
