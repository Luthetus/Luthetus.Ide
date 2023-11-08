using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Displays;

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
    [Inject]
    private InputFileSync InputFileSync { get; set; } = null!;

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private string _solutionName = string.Empty;
    private string _parentDirectoryName = string.Empty;

    public Key<TerminalCommand> NewDotNetSolutionTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
    public CancellationTokenSource NewDotNetSolutionCancellationTokenSource { get; set; } = new();

    private string DisplaySolutionName => string.IsNullOrWhiteSpace(_solutionName)
        ? "{enter solution name}"
        : _solutionName;

    private string DisplayParentDirectoryName => string.IsNullOrWhiteSpace(_parentDirectoryName)
        ? "{enter parent directory name}"
        : _parentDirectoryName;

    private FormattedCommand FormattedCommand => DotNetCliCommandFormatter.FormatDotnetNewSln(_solutionName);

    private void RequestInputFileForParentDirectory()
    {
        InputFileSync.RequestInputFileStateForm("Directory for new .NET Solution",
            async afp =>
            {
                if (afp is null)
                    return;

                _parentDirectoryName = afp.FormattedInput;

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
            }.ToImmutableArray());
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

        if (LuthetusHostingInformation.LuthetusHostingKind != LuthetusHostingKind.Photino)
        {
            await HackForWebsite_StartNewDotNetSolutionCommandOnClick(
                localSolutionName,
                localParentDirectoryName);
        }
        else
        {
            var newDotNetSolutionCommand = new TerminalCommand(
                NewDotNetSolutionTerminalCommandKey,
                localFormattedCommand,
                _parentDirectoryName,
                NewDotNetSolutionCancellationTokenSource.Token,
                () =>
                {
                    // Close Dialog
                    Dispatcher.Dispatch(new DialogState.DisposeAction(DialogRecord.Key));

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

                    DotNetSolutionSync.SetDotNetSolution(solutionAbsolutePath);
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
            HackForWebsite_NEW_SOLUTION_TEMPLATE);

        // Close Dialog
        Dispatcher.Dispatch(new DialogState.DisposeAction(DialogRecord.Key));

        NotificationHelper.DispatchInformative("Website .sln template was used", "No terminal available", LuthetusCommonComponentRenderers, Dispatcher, TimeSpan.FromSeconds(7));

        var solutionAbsolutePath = new AbsolutePath(
            solutionAbsolutePathString,
            false,
            EnvironmentProvider);

        DotNetSolutionSync.SetDotNetSolution(solutionAbsolutePath);
    }

    public const string HackForWebsite_NEW_SOLUTION_TEMPLATE = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.7.34018.315
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{EC571C96-8996-402C-B44A-264F84598795}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{EC571C96-8996-402C-B44A-264F84598795}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{EC571C96-8996-402C-B44A-264F84598795}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{EC571C96-8996-402C-B44A-264F84598795}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {CC0E8FC7-3D42-4480-BAF6-86D1E2F2289E}
	EndGlobalSection
EndGlobal
";
}