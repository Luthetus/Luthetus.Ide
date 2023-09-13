using Luthetus.Ide.RazorLib.CommandLineCase;

namespace Luthetus.Ide.RazorLib.TerminalCase;

public record TerminalCommand(
    TerminalCommandKey TerminalCommandKey,
    FormattedCommand FormattedCommand,
    string? ChangeWorkingDirectoryTo = null,
    CancellationToken CancellationToken = default,
    Func<Task>? ContinueWith = null);