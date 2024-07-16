using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Nugets.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Nugets.Models;

namespace Luthetus.CompilerServices.RazorLib.Nugets.Displays;

public partial class NuGetPackageManager : FluxorComponent, INuGetPackageManagerRendererType
{
    [Inject]
    private IState<NuGetPackageManagerState> NuGetPackageManagerStateWrap { get; set; } = null!;
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
        set => Dispatcher.Dispatch(new NuGetPackageManagerState.SetNugetQueryAction(value));
    }

    public bool IncludePrerelease
    {
        get => NuGetPackageManagerStateWrap.Value.IncludePrerelease;
        set => Dispatcher.Dispatch(new NuGetPackageManagerState.SetIncludePrereleaseAction(value));
    }

    private void SelectedProjectToModifyChanged(ChangeEventArgs changeEventArgs, DotNetSolutionState dotNetSolutionState)
    {
        if (changeEventArgs.Value is null || dotNetSolutionState.DotNetSolutionModel is null)
            return;

        var projectIdGuid = Guid.Parse((string)changeEventArgs.Value);

        IDotNetProject? selectedProject = null;

        if (projectIdGuid != Guid.Empty)
        {
            selectedProject = dotNetSolutionState.DotNetSolutionModel.DotNetProjectList
                .SingleOrDefault(x => x.ProjectIdGuid == projectIdGuid);
        }

        Dispatcher.Dispatch(new NuGetPackageManagerState.SetSelectedProjectToModifyAction(selectedProject));
    }

    private bool CheckIfProjectIsSelected(IDotNetProject dotNetProject, NuGetPackageManagerState nuGetPackageManagerState)
    {
        if (nuGetPackageManagerState.SelectedProjectToModify is null)
            return false;

        return nuGetPackageManagerState.SelectedProjectToModify.ProjectIdGuid == dotNetProject.ProjectIdGuid;
    }

    private bool ValidateSolutionContainsSelectedProject(DotNetSolutionState dotNetSolutionState, NuGetPackageManagerState nuGetPackageManagerState)
    {
        if (dotNetSolutionState.DotNetSolutionModel is null || nuGetPackageManagerState.SelectedProjectToModify is null)
            return false;

        return dotNetSolutionState.DotNetSolutionModel.DotNetProjectList.Any(
            x => x.ProjectIdGuid == nuGetPackageManagerState.SelectedProjectToModify.ProjectIdGuid);
    }

    private async Task SubmitNuGetQueryOnClick()
    {
        var query = NugetPackageManagerProvider.BuildQuery(NugetQuery, IncludePrerelease);

        try
        {
            // UI
            {
                _exceptionFromNugetQuery = null;
                _performingNugetQuery = true;
                await InvokeAsync(StateHasChanged);
            }

            BackgroundTaskService.Enqueue(
                Key<IBackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                "Submit NuGet Query",
                async () =>
                {
                    var localNugetResult = await NugetPackageManagerProvider
                        .QueryForNugetPackagesAsync(query)
                        .ConfigureAwait(false);

                    var setMostRecentQueryResultAction =
                        new NuGetPackageManagerState.SetMostRecentQueryResultAction(
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