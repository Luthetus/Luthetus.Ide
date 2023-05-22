namespace Luthetus.Ide.ClassLib.Git;

public enum GitDirtyReason
{
    None,
    Untracked,
    Added,
    Modified,
    Deleted,
}