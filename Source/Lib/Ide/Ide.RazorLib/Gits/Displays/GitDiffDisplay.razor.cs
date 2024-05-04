using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitDiffDisplay : ComponentBase
{
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public GitFile GitFile { get; set; } = null!;

    public Key<TerminalCommand> GitLogTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    private async Task ShowOriginalFromGitOnClick()
    {
        var localGitState = GitStateWrap.Value;
        var localGitFile = GitFile;

        if (localGitState.Repo is null)
            return;

        var gitLogArgs = $"log -p {localGitFile.RelativePathString}";
        var formattedCommand = new FormattedCommand(
            GitCliFacts.TARGET_FILE_NAME,
            new string[] { gitLogArgs })
        {
            HACK_ArgumentsString = gitLogArgs
        };

        var gitStatusCommand = new TerminalCommand(
            GitLogTerminalCommandKey,
            formattedCommand,
            localGitState.Repo.RepoFolderAbsolutePath.Value);

        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
        await generalTerminal.EnqueueCommandAsync(gitStatusCommand);
    }
}