using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitControlsDisplay : ComponentBase
{
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;

    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    public Key<TerminalCommand> NewDotNetSolutionTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    private async Task ExecuteGitStatusTerminalCommandOnClick()
    {
        var localGitState = GitState;

        if (localGitState.GitFolderAbsolutePath?.ParentDirectory is null)
            return;

        var parentDirectory = localGitState.GitFolderAbsolutePath.ParentDirectory;

        var formattedCommand = new FormattedCommand(
            GitCliFacts.TARGET_FILE_NAME,
            new string[] { GitCliFacts.STATUS_COMMAND });

        var gitStatusCommand = new TerminalCommand(
            NewDotNetSolutionTerminalCommandKey,
            formattedCommand,
            parentDirectory.Value,
            CancellationToken.None);

        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
        await generalTerminal.EnqueueCommandAsync(gitStatusCommand);
    }
}