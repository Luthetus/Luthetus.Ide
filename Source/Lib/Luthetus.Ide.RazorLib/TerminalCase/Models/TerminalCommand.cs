using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.Ide.RazorLib.CommandLineCase.Models;

namespace Luthetus.Ide.RazorLib.TerminalCase.Models;

public record TerminalCommand(
    Key<TerminalCommand> TerminalCommandKey,
    FormattedCommand FormattedCommand,
    string? ChangeWorkingDirectoryTo = null,
    CancellationToken CancellationToken = default,
    Func<Task>? ContinueWith = null);