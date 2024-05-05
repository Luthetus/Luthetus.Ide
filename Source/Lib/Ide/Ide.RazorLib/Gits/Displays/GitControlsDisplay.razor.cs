using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitControlsDisplay : ComponentBase
{
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    private string _summary = string.Empty;

    public Key<TerminalCommand> GitStatusTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
    public Key<TerminalCommand> GitCommitTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    private async Task ExecuteGitStatusTerminalCommandOnClick()
    {
        var localGitState = GitState;

        if (localGitState.Repo is null)
            return;

        var gitStatusDashUCommand = $"{GitCliFacts.STATUS_COMMAND} -u";
        var formattedCommand = new FormattedCommand(
            GitCliFacts.TARGET_FILE_NAME,
            new string[] { gitStatusDashUCommand })
        {
            HACK_ArgumentsString = gitStatusDashUCommand
        };

        var gitCliOutputParser = new GitCliOutputParser(
            Dispatcher,
            localGitState,
            EnvironmentProvider);

        var gitStatusCommand = new TerminalCommand(
            GitStatusTerminalCommandKey,
            formattedCommand,
            localGitState.Repo.AbsolutePath.Value,
            OutputParser: gitCliOutputParser);

        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
        await generalTerminal.EnqueueCommandAsync(gitStatusCommand);
    }

    private async Task SubmitOnClick(GitState localGitState)
    {
        var localSummary = _summary;
        if (string.IsNullOrWhiteSpace(localSummary))
            return;

        if (localGitState.Repo is null)
            return;

        var filesBuilder =  new StringBuilder();

        foreach (var fileAbsolutePath in localGitState.StagedFileMap.Values)
        {
            var relativePathString = PathHelper.GetRelativeFromTwoAbsolutes(
                localGitState.Repo.AbsolutePath,
                fileAbsolutePath.AbsolutePath,
                EnvironmentProvider);

            if (EnvironmentProvider.DirectorySeparatorChar == '\\')
            {
                // The following fails:
                //     git add ".\MyApp\"
                //
                // Whereas the following succeeds
                //     git add "./MyApp/"
                relativePathString = relativePathString.Replace(
                    EnvironmentProvider.DirectorySeparatorChar,
                    EnvironmentProvider.AltDirectorySeparatorChar);
            }

            filesBuilder.Append($"\"{relativePathString}\" ");
        }

        var argumentsString = "add " + filesBuilder.ToString();

        var formattedCommand = new FormattedCommand(
            GitCliFacts.TARGET_FILE_NAME,
            new string[] { argumentsString })
        {
            HACK_ArgumentsString = argumentsString
        };
        
        var gitAddCommand = new TerminalCommand(
            GitStatusTerminalCommandKey,
            formattedCommand,
            localGitState.Repo.AbsolutePath.Value,
            ContinueWith: () => CommitChanges(localGitState, localSummary));

        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
        await generalTerminal.EnqueueCommandAsync(gitAddCommand);
    }

    private async Task CommitChanges(GitState localGitState, string localSummary)
    {
        if (localGitState.Repo is null)
            return;

        var argumentsString = $"commit -m \"{localSummary}\"";

        var formattedCommand = new FormattedCommand(
            GitCliFacts.TARGET_FILE_NAME,
            new string[] { argumentsString })
        {
            HACK_ArgumentsString = argumentsString
        };

        var gitCommitCommand = new TerminalCommand(
            GitCommitTerminalCommandKey,
            formattedCommand,
            localGitState.Repo.AbsolutePath.Value,
            ContinueWith: ExecuteGitStatusTerminalCommandOnClick);

        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
        await generalTerminal.EnqueueCommandAsync(gitCommitCommand);
    }

    private void ShowGitOriginDialogOnClick()
    {
        var dialogViewModel = new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
            $"Git Origin",
            typeof(GitOriginDisplay),
            null,
            null,
            true);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogViewModel));
    }
}