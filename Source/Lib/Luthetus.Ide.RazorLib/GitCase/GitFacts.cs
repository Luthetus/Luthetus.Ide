using Luthetus.Ide.RazorLib.TerminalCase;

namespace Luthetus.Ide.RazorLib.GitCase;

public static class GitFacts
{
    public const string GIT_FOLDER_NAME = ".git";

    public const string UNTRACKED_FILES_TEXT_START = "Untracked files:";
    public const string CHANGES_NOT_STAGED_FOR_COMMIT_TEXT_START = "Changes not staged for commit:";

    public const string GIT_DIRTY_REASON_MODIFIED = "modified:";
    public const string GIT_DIRTY_REASON_DELETED = "deleted:";

    public static readonly TerminalCommandKey GitInitTerminalCommandKey = TerminalCommandKey.NewKey();
    public static readonly TerminalCommandKey GitStatusTerminalCommandKey = TerminalCommandKey.NewKey();
}