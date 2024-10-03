namespace Luthetus.Extensions.Git.Models;

public enum GitDirtyReason
{
    None,
    Untracked,
    Added,
    Modified,
    Deleted,
}