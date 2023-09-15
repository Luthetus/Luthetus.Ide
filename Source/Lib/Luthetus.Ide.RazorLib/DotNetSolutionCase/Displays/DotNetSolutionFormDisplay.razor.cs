using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Luthetus.Ide.RazorLib.InputFileCase;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.Views;
using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Ide.RazorLib.CommandLineCase.Models;
using Luthetus.Ide.RazorLib.FileSystemCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.States;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.Displays;

public partial class DotNetSolutionFormDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalSessionRegistry> TerminalSessionsStateWrap { get; set; } = null!;
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
    public DotNetSolutionFormViewable View { get; set; } = null!;

    private string SolutionName => string.IsNullOrWhiteSpace(View.SolutionName)
        ? "{enter solution name}"
        : View.SolutionName;

    private string ParentDirectoryName => string.IsNullOrWhiteSpace(View.ParentDirectoryName)
        ? "{enter parent directory name}"
        : View.ParentDirectoryName;

    private FormattedCommand FormattedCommand => DotNetCliFacts.FormatDotnetNewSln(View.SolutionName);

    private void RequestInputFileForParentDirectory()
    {
        Dispatcher.Dispatch(new InputFileRegistry.RequestInputFileStateFormAction(
            "Directory for new .NET Solution",
            async afp =>
            {
                if (afp is null)
                    return;

                View.ParentDirectoryName = afp.FormattedInput;

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
        var localSolutionName = View.SolutionName;
        var localParentDirectoryName = View.ParentDirectoryName;

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
                View.NewDotNetSolutionTerminalCommandKey,
                localFormattedCommand,
                View.ParentDirectoryName,
                View.NewDotNetSolutionCancellationTokenSource.Token,
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