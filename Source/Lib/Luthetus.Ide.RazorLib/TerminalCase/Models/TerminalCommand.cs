using Luthetus.Ide.RazorLib.CommandLineCase.Models;

namespace Luthetus.Ide.RazorLib.TerminalCase.Models;

public record TerminalCommand(
    TerminalCommandKey TerminalCommandKey,
    FormattedCommand FormattedCommand,
    string? ChangeWorkingDirectoryTo = null,
    CancellationToken CancellationToken = default,
    Func<Task>? ContinueWith = null);