using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Ide.ClassLib.CommandLine;
using Luthetus.Ide.ClassLib.Nuget;
using Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;
using Luthetus.Ide.ClassLib.Store.NugetPackageManagerCase;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.NuGet;

public partial class NugetPackageDisplay : FluxorComponent
{
    [Inject]
    private IState<NuGetPackageManagerState> NuGetPackageManagerStateWrap { get; set; } = null!;
    [Inject]
    private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public NugetPackageRecord NugetPackageRecord { get; set; } = null!;

    private static readonly TerminalCommandKey AddNugetPackageTerminalCommandKey = TerminalCommandKey.NewTerminalCommandKey();

    private string _nugetPackageVersionString;

    private ImmutableArray<NugetPackageVersionRecord> _nugetPackageVersionsOrdered = ImmutableArray<NugetPackageVersionRecord>.Empty;
    private string? _previousNugetPackageId;

    protected override void OnParametersSet()
    {
        if (_previousNugetPackageId is null || _previousNugetPackageId != NugetPackageRecord.Id)
        {
            _previousNugetPackageId = NugetPackageRecord.Id;

            _nugetPackageVersionsOrdered = NugetPackageRecord.Versions
                .OrderByDescending(x => x.Version)
                .ToImmutableArray();

            _nugetPackageVersionString = _nugetPackageVersionsOrdered
                .FirstOrDefault()?
                .Version ?? string.Empty;
        }

        base.OnParametersSet();
    }

    private void SelectedNugetVersionChanged(ChangeEventArgs changeEventArgs)
    {
        _nugetPackageVersionString = changeEventArgs.Value?.ToString() ?? string.Empty;
    }

    private bool ValidateSolutionContainsSelectedProject()
    {
        var dotNetSolutionState = DotNetSolutionStateWrap.Value;
        var nuGetPackageManagerState = NuGetPackageManagerStateWrap.Value;

        if (dotNetSolutionState.DotNetSolutionModel is null ||
            nuGetPackageManagerState.SelectedProjectToModify is null)
        {
            return false;
        }

        return dotNetSolutionState.DotNetSolutionModel.DotNetProjects.Any(x =>
            x.ProjectIdGuid == nuGetPackageManagerState.SelectedProjectToModify.ProjectIdGuid);
    }

    private async Task AddNugetPackageReferenceOnClick()
    {
        var targetProject = NuGetPackageManagerStateWrap.Value.SelectedProjectToModify;
        var targetNugetPackage = NugetPackageRecord;
        var targetNugetVersion = _nugetPackageVersionString;

        if (!ValidateSolutionContainsSelectedProject() ||
            targetProject is null)
        {
            return;
        }

        var parentDirectory = targetProject.AbsoluteFilePath.ParentDirectory;

        if (parentDirectory is null)
            return;

        var formattedCommand = DotNetCliFacts.FormatAddNugetPackageReferenceToProject(
            targetProject.AbsoluteFilePath.FormattedInput,
            targetNugetPackage.Id,
            targetNugetVersion);

        var addNugetPackageReferenceCommand = new TerminalCommand(
            AddNugetPackageTerminalCommandKey,
            formattedCommand,
            parentDirectory.FormattedInput,
            CancellationToken.None, () =>
            {
                if (LuthetusCommonComponentRenderers.InformativeNotificationRendererType is not null)
                {
                    var notificationInformative = new NotificationRecord(
                        NotificationKey.NewNotificationKey(),
                        "Add Nuget Package Reference",
                        LuthetusCommonComponentRenderers.InformativeNotificationRendererType,
                        new Dictionary<string, object?>
                        {
                            {
                                nameof(IInformativeNotificationRendererType.Message),
                                $"{targetNugetPackage.Title}, {targetNugetVersion} was added to {targetProject.DisplayName}"
                            },
                        },
                        TimeSpan.FromSeconds(6),
                        true,
                        null);

                    Dispatcher.Dispatch(new NotificationRegistry.RegisterAction(
                        notificationInformative));
                }

                return Task.CompletedTask;
            });

        var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

        await generalTerminalSession
            .EnqueueCommandAsync(addNugetPackageReferenceCommand);
    }
}