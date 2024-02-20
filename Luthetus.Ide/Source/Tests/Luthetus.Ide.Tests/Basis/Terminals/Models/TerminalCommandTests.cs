using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

public record TerminalCommandTests(
    Key<TerminalCommand> TerminalCommandKey,
    FormattedCommand FormattedCommand,
    string? ChangeWorkingDirectoryTo = null,
    CancellationToken CancellationToken = default,
    Func<Task>? ContinueWith = null,
	Func<Task>? BeginWith = null);