using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Git;

public record GitFile(
    IAbsolutePath AbsoluteFilePath,
    GitDirtyReason GitDirtyReason)
{
    public bool IsDirty => GitDirtyReason switch
    {
        GitDirtyReason.None => false,
        GitDirtyReason.Untracked => true,
        GitDirtyReason.Added => true,
        GitDirtyReason.Modified => true,
        GitDirtyReason.Deleted => true,
        _ => throw new ApplicationException(
            GetUnrecognizedGitDirtyReasonExceptionMessage(GitDirtyReason))
    };

    private static string GetUnrecognizedGitDirtyReasonExceptionMessage(GitDirtyReason gitDirtyReason) =>
        $"The {nameof(GitDirtyReason)}: {gitDirtyReason} was unrecognized.";
}