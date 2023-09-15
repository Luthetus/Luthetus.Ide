using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.CompilerServices.Lang.DotNetSolution;
using Luthetus.Ide.RazorLib.DotNetSolutionCase;
using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.NugetCase.NugetPackageManagerCase;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.NugetCase.Models;

namespace Luthetus.Ide.RazorLib.NugetCase.Displays;

public partial class NuGetPackageManager : FluxorComponent, INuGetPackageManagerRendererType
{
    [Inject]
    private IState<NuGetPackageManagerRegistry> NuGetPackageManagerStateWrap { get; set; } = null!;
    [Inject]
    private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private INugetPackageManagerProvider NugetPackageManagerProvider { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

    private bool _performingNugetQuery;
    private Exception? _exceptionFromNugetQuery;

    public string NugetQuery
    {
        get => NuGetPackageManagerStateWrap.Value.NugetQuery;
        set => Dispatcher.Dispatch(new NuGetPackageManagerRegistry.SetNugetQueryAction(
                   value));
    }

    public bool IncludePrerelease
    {
        get => NuGetPackageManagerStateWrap.Value.IncludePrerelease;
        set => Dispatcher.Dispatch(new NuGetPackageManagerRegistry.SetIncludePrereleaseAction(
                   value));
    }

    private void SelectedProjectToModifyChanged(
        ChangeEventArgs changeEventArgs,
        DotNetSolutionState dotNetSolutionState)
    {
        if (changeEventArgs.Value is null || dotNetSolutionState.DotNetSolutionModel is null)
            return;

        var projectIdGuid = Guid.Parse((string)changeEventArgs.Value);

        IDotNetProject? selectedProject = null;

        if (projectIdGuid != Guid.Empty)
        {
            selectedProject = dotNetSolutionState.DotNetSolutionModel.DotNetProjects
                .SingleOrDefault(x => x.ProjectIdGuid == projectIdGuid);
        }

        Dispatcher.Dispatch(new NuGetPackageManagerRegistry.SetSelectedProjectToModifyAction(
            selectedProject));
    }

    private bool CheckIfProjectIsSelected(
        IDotNetProject dotNetProject,
        NuGetPackageManagerRegistry nuGetPackageManagerState)
    {
        if (nuGetPackageManagerState.SelectedProjectToModify is null)
            return false;

        return nuGetPackageManagerState.SelectedProjectToModify.ProjectIdGuid ==
               dotNetProject.ProjectIdGuid;
    }

    private bool ValidateSolutionContainsSelectedProject(
        DotNetSolutionState dotNetSolutionState,
        NuGetPackageManagerRegistry nuGetPackageManagerState)
    {
        if (dotNetSolutionState.DotNetSolutionModel is null ||
            nuGetPackageManagerState.SelectedProjectToModify is null)
        {
            return false;
        }

        return dotNetSolutionState.DotNetSolutionModel.DotNetProjects
            .Any(x =>
                x.ProjectIdGuid == nuGetPackageManagerState.SelectedProjectToModify.ProjectIdGuid);
    }

    private async Task SubmitNuGetQueryOnClick()
    {
        var query = NugetPackageManagerProvider
            .BuildQuery(NugetQuery, IncludePrerelease);

        try
        {
            _exceptionFromNugetQuery = null;

            _performingNugetQuery = true;
            await InvokeAsync(StateHasChanged);

            BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
                "Submit NuGet Query",
                async () =>
                {
                    var localNugetResult =
                        await NugetPackageManagerProvider
                            .QueryForNugetPackagesAsync(query);

                    var setMostRecentQueryResultAction =
                        new NuGetPackageManagerRegistry.SetMostRecentQueryResultAction(
                            localNugetResult);

                    Dispatcher.Dispatch(setMostRecentQueryResultAction);
                });
        }
        catch (Exception e)
        {
            _exceptionFromNugetQuery = e;
        }
        finally
        {
            _performingNugetQuery = false;
            await InvokeAsync(StateHasChanged);
        }
    }
}