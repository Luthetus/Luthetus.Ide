using Fluxor;
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

        var gitCliOutputParser = new GitCliOutputParser(
            Dispatcher,
            GitState,
            EnvironmentProvider.AbsolutePathFactory(parentDirectory.Value, true),
            EnvironmentProvider);

        var gitStatusCommand = new TerminalCommand(
            NewDotNetSolutionTerminalCommandKey,
            formattedCommand,
            parentDirectory.Value,
            ParseFunc: gitCliOutputParser.Parse);

        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
        await generalTerminal.EnqueueCommandAsync(gitStatusCommand);
    }

    private async Task SubmitOnClick(GitState localGitState)
    {
        if (string.IsNullOrWhiteSpace(_summary))
            return;

        if (localGitState.GitFolderAbsolutePath?.ParentDirectory is null)
            return;

        var parentDirectory = localGitState.GitFolderAbsolutePath.ParentDirectory;

        var filesBuilder =  new StringBuilder();

        foreach (var fileAbsolutePath in localGitState.StagedGitFileMap.Values)
        {
            var relativePathString = PathHelper.GetRelativeFromTwoAbsolutes(
                EnvironmentProvider.AbsolutePathFactory(parentDirectory.Value, true),
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
            NewDotNetSolutionTerminalCommandKey,
            formattedCommand,
            parentDirectory.Value);

        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
        await generalTerminal.EnqueueCommandAsync(gitAddCommand);
    }
}