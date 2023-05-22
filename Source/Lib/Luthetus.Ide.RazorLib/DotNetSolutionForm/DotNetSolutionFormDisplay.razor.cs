using System.Collections.Immutable;
using System.Text;
using BlazorCommon.RazorLib.Dialog;
using BlazorCommon.RazorLib.Store.DialogCase;
using BlazorStudio.ClassLib.InputFile;
using BlazorStudio.ClassLib.Store.DotNetSolutionCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.ClassLib.CommandLine;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.DotNetSolutionForm;

public partial class DotNetSolutionFormDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private readonly TerminalCommandKey _newDotNetSolutionTerminalCommandKey =
        TerminalCommandKey.NewTerminalCommandKey();

    private readonly CancellationTokenSource _newDotNetSolutionCancellationTokenSource = new();

    private string _solutionName = string.Empty;
    private string _parentDirectoryName = string.Empty;

    private string SolutionName => string.IsNullOrWhiteSpace(_solutionName)
        ? "{enter solution name}"
        : _solutionName;

    private string ParentDirectoryName => string.IsNullOrWhiteSpace(_parentDirectoryName)
        ? "{enter parent directory name}"
        : _parentDirectoryName;

    private (string targetFileName, IEnumerable<string> arguments) FormattedCommand =>
        DotNetCliFacts.FormatDotnetNewSln(_solutionName);

    private string InterpolatedCommand =>
        FormattedCommandToStringHelper(FormattedCommand);

    private string FormattedCommandToStringHelper(
        (string targetFileName, IEnumerable<string> arguments) formattedCommand)
    {
        var interpolatedCommandBuilder = new StringBuilder(
            formattedCommand.targetFileName);

        foreach (var argument in formattedCommand.arguments)
        {
            interpolatedCommandBuilder.Append($" {argument}");
        }

        return interpolatedCommandBuilder.ToString();
    }

    private void RequestInputFileForParentDirectory()
    {
        Dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "Directory for new .NET Solution",
                async afp =>
                {
                    if (afp is null)
                        return;

                    _parentDirectoryName = afp.GetAbsoluteFilePathString();

                    await InvokeAsync(StateHasChanged);
                },
                afp =>
                {
                    if (afp is null ||
                        !afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(true);
                },
                new[]
                {
                    new InputFilePattern(
                        "Directory",
                        afp => afp.IsDirectory)
                }.ToImmutableArray()));
    }

    private async Task StartNewDotNetSolutionCommandOnClick()
    {
        var localFormattedCommand = FormattedCommand;
        var localSolutionName = _solutionName;
        var localParentDirectoryName = _parentDirectoryName;

        if (string.IsNullOrWhiteSpace(localSolutionName) ||
            string.IsNullOrWhiteSpace(localParentDirectoryName))
        {
            return;
        }

        var newDotNetSolutionCommand = new TerminalCommand(
            _newDotNetSolutionTerminalCommandKey,
            localFormattedCommand.targetFileName,
            localFormattedCommand.arguments,
            _parentDirectoryName,
            _newDotNetSolutionCancellationTokenSource.Token,
            async () =>
            {
                // Close Dialog
                Dispatcher.Dispatch(
                    new DialogRecordsCollection.DisposeAction(
                        DialogRecord.DialogKey));

                // Open the created .NET Solution
                localParentDirectoryName = FilePathHelper.StripEndingDirectorySeparatorIfExists(
                    localParentDirectoryName,
                    EnvironmentProvider);

                var parentDirectoryAbsoluteFilePath = new AbsoluteFilePath(
                    localParentDirectoryName,
                    true,
                    EnvironmentProvider);

                var solutionAbsoluteFilePathString =
                    parentDirectoryAbsoluteFilePath.GetAbsoluteFilePathString() +
                    localSolutionName +
                    EnvironmentProvider.DirectorySeparatorChar +
                    localSolutionName +
                    '.' +
                    ExtensionNoPeriodFacts.DOT_NET_SOLUTION;

                await DotNetSolutionState.SetActiveSolutionAsync(
                    solutionAbsoluteFilePathString,
                    FileSystemProvider,
                    EnvironmentProvider,
                    Dispatcher);
            });

        var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

        await generalTerminalSession
            .EnqueueCommandAsync(newDotNetSolutionCommand);
    }
}