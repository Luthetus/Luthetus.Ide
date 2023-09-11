using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.ClassLib.CommandLine;
using Luthetus.Ide.ClassLib.Store.ProgramExecutionCase;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.StartupControls;

public partial class StartupControlsDisplay : FluxorComponent
{
    [Inject]
    private IState<ProgramExecutionRegistry> ProgramExecutionStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionRegistry> TerminalSessionsStateWrap { get; set; } = null!;

    private readonly TerminalCommandKey _newDotNetSolutionTerminalCommandKey =
        TerminalCommandKey.NewKey();

    private readonly CancellationTokenSource _newDotNetSolutionCancellationTokenSource = new();

    private async Task StartProgramWithoutDebuggingOnClick()
    {
        var programExecutionState = ProgramExecutionStateWrap.Value;

        if (programExecutionState.StartupProjectAbsoluteFilePath is null)
            return;

        var parentDirectoryAbsoluteFilePathString = programExecutionState.StartupProjectAbsoluteFilePath.ParentDirectory?.FormattedInput;

        if (parentDirectoryAbsoluteFilePathString is null)
            return;

        var formattedCommand = DotNetCliFacts
            .FormatStartProjectWithoutDebugging(
                programExecutionState.StartupProjectAbsoluteFilePath);

        var startProgramWithoutDebuggingCommand = new TerminalCommand(
            _newDotNetSolutionTerminalCommandKey,
            formattedCommand,
            parentDirectoryAbsoluteFilePathString,
            _newDotNetSolutionCancellationTokenSource.Token);

        var executionTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.EXECUTION_TERMINAL_SESSION_KEY];

        await executionTerminalSession
            .EnqueueCommandAsync(startProgramWithoutDebuggingCommand);
    }
}