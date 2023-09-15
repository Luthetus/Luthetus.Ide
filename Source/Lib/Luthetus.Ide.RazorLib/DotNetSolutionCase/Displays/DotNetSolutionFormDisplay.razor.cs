using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Luthetus.Ide.RazorLib.CommandLineCase.Models;
using Luthetus.Ide.RazorLib.FileSystemCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.States;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.Viewables;
using Luthetus.Ide.RazorLib.InputFileCase.States;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialog.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.Notification.Models;
using Luthetus.Common.RazorLib.Installation.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.Displays;

public partial class DotNetSolutionFormDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalSessionState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public DotNetSolutionFormViewable Viewable { get; set; } = null!;

    private string SolutionName => string.IsNullOrWhiteSpace(Viewable.SolutionName)
        ? "{enter solution name}"
        : Viewable.SolutionName;

    private string ParentDirectoryName => string.IsNullOrWhiteSpace(Viewable.ParentDirectoryName)
        ? "{enter parent directory name}"
        : Viewable.ParentDirectoryName;

    private FormattedCommand FormattedCommand => DotNetCliFacts.FormatDotnetNewSln(Viewable.SolutionName);

    private void RequestInputFileForParentDirectory()
    {
        Dispatcher.Dispatch(new InputFileState.RequestInputFileStateFormAction(
            "Directory for new .NET Solution",
            async afp =>
            {
                if (afp is null)
                    return;

                Viewable.ParentDirectoryName = afp.FormattedInput;

                await InvokeAsync(StateHasChanged);
            },
            afp =>
            {
                if (afp is null || !afp.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                new InputFilePattern("Directory", afp => afp.IsDirectory)
            }.ToImmutableArray()));
    }

    private async Task StartNewDotNetSolutionCommandOnClick()
    {
        var localFormattedCommand = FormattedCommand;
        var localSolutionName = Viewable.SolutionName;
        var localParentDirectoryName = Viewable.ParentDirectoryName;

        if (string.IsNullOrWhiteSpace(localSolutionName) ||
            string.IsNullOrWhiteSpace(localParentDirectoryName))
        {
            return;
        }

        if (LuthetusHostingInformation.LuthetusHostingKind != LuthetusHostingKind.Photino)
        {
            await HackForWebsite_StartNewDotNetSolutionCommandOnClick(
                localSolutionName,
                localParentDirectoryName);
        }
        else
        {
            var newDotNetSolutionCommand = new TerminalCommand(
                Viewable.NewDotNetSolutionTerminalCommandKey,
                localFormattedCommand,
                Viewable.ParentDirectoryName,
                Viewable.NewDotNetSolutionCancellationTokenSource.Token,
                () =>
                {
                    // Close Dialog
                    Dispatcher.Dispatch(new DialogRegistry.DisposeAction(DialogRecord.Key));

                    // Open the created .NET Solution
                    var parentDirectoryAbsolutePath = new AbsolutePath(
                        localParentDirectoryName,
                        true,
                        EnvironmentProvider);

                    var solutionAbsolutePathString =
                        parentDirectoryAbsolutePath.FormattedInput +
                        localSolutionName +
                        EnvironmentProvider.DirectorySeparatorChar +
                        localSolutionName +
                        '.' +
                        ExtensionNoPeriodFacts.DOT_NET_SOLUTION;

                    var solutionAbsolutePath = new AbsolutePath(
                        solutionAbsolutePathString,
                        false,
                        EnvironmentProvider);

                    Dispatcher.Dispatch(new DotNetSolutionState.SetDotNetSolutionTask(
                        solutionAbsolutePath,
                        DotNetSolutionSync));

                    return Task.CompletedTask;
                });

            var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
                TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            await generalTerminalSession
                .EnqueueCommandAsync(newDotNetSolutionCommand);
        }
    }

    private async Task HackForWebsite_StartNewDotNetSolutionCommandOnClick(
        string localSolutionName,
        string localParentDirectoryName)
    {
        var directoryContainingSolution =
            EnvironmentProvider.JoinPaths(localParentDirectoryName, localSolutionName) +
            EnvironmentProvider.DirectorySeparatorChar;

        await FileSystemProvider.Directory.CreateDirectoryAsync(
            directoryContainingSolution);

        var localSolutionFilenameWithExtension =
            localSolutionName +
            '.' +
            ExtensionNoPeriodFacts.DOT_NET_SOLUTION;

        var solutionAbsolutePathString = EnvironmentProvider.JoinPaths(
            directoryContainingSolution,
            localSolutionFilenameWithExtension);

        await FileSystemProvider.File.WriteAllTextAsync(
            solutionAbsolutePathString,
            DotNetSolutionFormViewable.HackForWebsite_NEW_SOLUTION_TEMPLATE);

        // Close Dialog
        Dispatcher.Dispatch(new DialogRegistry.DisposeAction(DialogRecord.Key));

        NotificationHelper.DispatchInformative("Website .sln template was used", "No terminal available", LuthetusCommonComponentRenderers, Dispatcher);

        var solutionAbsolutePath = new AbsolutePath(
            solutionAbsolutePathString,
            false,
            EnvironmentProvider);

        Dispatcher.Dispatch(new DotNetSolutionState.SetDotNetSolutionTask(
            solutionAbsolutePath,
            DotNetSolutionSync));
    }
}