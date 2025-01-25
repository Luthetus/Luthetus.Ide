using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Exceptions;

namespace Luthetus.Extensions.Git.Models;

public record GitFile(AbsolutePath AbsolutePath, string RelativePathString, GitDirtyReason GitDirtyReason)
{
    public bool IsDirty => GitDirtyReason switch
    {
        GitDirtyReason.None => false,
        GitDirtyReason.Untracked => true,
        GitDirtyReason.Added => true,
        GitDirtyReason.Modified => true,
        GitDirtyReason.Deleted => true,
        _ => throw new LuthetusIdeException(GetUnrecognizedGitDirtyReasonExceptionMessage(GitDirtyReason))
    };

    private static string GetUnrecognizedGitDirtyReasonExceptionMessage(GitDirtyReason gitDirtyReason) =>
        $"The {nameof(GitDirtyReason)}: {gitDirtyReason} was unrecognized.";
}