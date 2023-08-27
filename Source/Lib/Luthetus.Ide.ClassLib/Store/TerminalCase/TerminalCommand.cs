using Luthetus.Ide.ClassLib.CommandLine;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

public record TerminalCommand(
    TerminalCommandKey TerminalCommandKey,
    FormattedCommand FormattedCommand,
    string? ChangeWorkingDirectoryTo = null,
    CancellationToken CancellationToken = default,
    Func<Task>? ContinueWith = null);