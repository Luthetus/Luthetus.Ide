using Luthetus.Ide.ClassLib.CommandLineCase;

namespace Luthetus.Ide.ClassLib.TerminalCase;

public record TerminalCommand(
    TerminalCommandKey TerminalCommandKey,
    FormattedCommand FormattedCommand,
    string? ChangeWorkingDirectoryTo = null,
    CancellationToken CancellationToken = default,
    Func<Task>? ContinueWith = null);