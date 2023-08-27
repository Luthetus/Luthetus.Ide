using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.CompilerServices.Lang.DotNetSolution;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.Nuget;
using Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;
using Luthetus.Ide.ClassLib.Store.NugetPackageManagerCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.NuGet;

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
    private ICommonBackgroundTaskQueue CommonBackgroundTaskQueue { get; set; } = null!;

    private bool _performingNugetQuery;
    private Exception? _exceptionFromNugetQuery;

    public string NugetQuery
    {
        get => NuGetPackageManagerStateWrap.Value.NugetQuery;
        set => Dispatcher.Dispatch(new NuGetPackageManagerState.SetNugetQueryAction(
                   value));
    }

    public bool IncludePrerelease
    {
        get => NuGetPackageManagerStateWrap.Value.IncludePrerelease;
        set => Dispatcher.Dispatch(new NuGetPackageManagerState.SetIncludePrereleaseAction(
                   value));
    }

    private void SelectedProjectToModifyChanged(
        ChangeEventArgs changeEventArgs,
        DotNetSolutionState dotNetSolutionState)
    {
        if (changeEventArgs.Value is null ||
            dotNetSolutionState.DotNetSolution is null)
        {
            return;
        }

        var projectIdGuid = Guid.Parse((string)changeEventArgs.Value);

        IDotNetProject? selectedProject = null;

        if (projectIdGuid != Guid.Empty)
        {
            selectedProject = dotNetSolutionState.DotNetSolution.DotNetProjects
                .SingleOrDefault(x => x.ProjectIdGuid == projectIdGuid);
        }

        Dispatcher.Dispatch(new NuGetPackageManagerState.SetSelectedProjectToModifyAction(
            selectedProject));
    }

    private bool CheckIfProjectIsSelected(
        IDotNetProject dotNetProject,
        NuGetPackageManagerState nuGetPackageManagerState)
    {
        if (nuGetPackageManagerState.SelectedProjectToModify is null)
            return false;

        return nuGetPackageManagerState.SelectedProjectToModify.ProjectIdGuid ==
               dotNetProject.ProjectIdGuid;
    }

    private bool ValidateSolutionContainsSelectedProject(
        DotNetSolutionState dotNetSolutionState,
        NuGetPackageManagerState nuGetPackageManagerState)
    {
        if (dotNetSolutionState.DotNetSolution is null ||
            nuGetPackageManagerState.SelectedProjectToModify is null)
            return false;

        return dotNetSolutionState.DotNetSolution.DotNetProjects
            .Any(x =>
                x.ProjectIdGuid == nuGetPackageManagerState.SelectedProjectToModify.ProjectIdGuid);
    }

    private async Task SubmitNugetQueryOnClick()
    {
        var query = NugetPackageManagerProvider
            .BuildQuery(NugetQuery, IncludePrerelease);

        try
        {
            _exceptionFromNugetQuery = null;

            _performingNugetQuery = true;
            await InvokeAsync(StateHasChanged);

            var backgroundTask = new BackgroundTask(
                async cancellationToken =>
                {
                    var localNugetResult =
                        await NugetPackageManagerProvider
                            .QueryForNugetPackagesAsync(query);

                    var setMostRecentQueryResultAction =
                        new NuGetPackageManagerState.SetMostRecentQueryResultAction(
                            localNugetResult);

                    Dispatcher.Dispatch(setMostRecentQueryResultAction);
                },
                "SubmitNugetQueryOnClickTask",
                "TODO: Describe this task",
                false,
                _ => Task.CompletedTask,
                Dispatcher,
                CancellationToken.None);

            CommonBackgroundTaskQueue.QueueBackgroundWorkItem(backgroundTask);
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