using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;

namespace Luthetus.Extensions.DotNet.Nugets.Displays;

public partial class NuGetPackageManager : ComponentBase, IDisposable, INuGetPackageManagerRendererType
{
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private INugetPackageManagerProvider NugetPackageManagerProvider { get; set; } = null!;
	[Inject]
	private IAppOptionsService AppOptionsService { get; set; } = null!;

	private bool _performingNugetQuery;
	private Exception? _exceptionFromNugetQuery;

	public string NugetQuery
	{
		get => DotNetBackgroundTaskApi.NuGetPackageManagerService.GetNuGetPackageManagerState().NugetQuery;
		set => DotNetBackgroundTaskApi.NuGetPackageManagerService.ReduceSetNugetQueryAction(value);
	}

	public bool IncludePrerelease
	{
		get => DotNetBackgroundTaskApi.NuGetPackageManagerService.GetNuGetPackageManagerState().IncludePrerelease;
		set => DotNetBackgroundTaskApi.NuGetPackageManagerService.ReduceSetIncludePrereleaseAction(value);
	}

	protected override void OnInitialized()
	{
		DotNetBackgroundTaskApi.NuGetPackageManagerService.NuGetPackageManagerStateChanged += OnNuGetPackageManagerStateChanged;
		DotNetBackgroundTaskApi.DotNetSolutionService.DotNetSolutionStateChanged += OnDotNetSolutionStateChanged;
		base.OnInitialized();
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

		DotNetBackgroundTaskApi.NuGetPackageManagerService.ReduceSetSelectedProjectToModifyAction(selectedProject);
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

			DotNetBackgroundTaskApi.Enqueue(new DotNetBackgroundTaskApiWorkArgs
			{
				WorkKind = DotNetBackgroundTaskApiWorkKind.SubmitNuGetQuery,
				NugetPackageManagerQuery = query,
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
	
	private async void OnNuGetPackageManagerStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	private async void OnDotNetSolutionStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	public void Dispose()
	{
		DotNetBackgroundTaskApi.NuGetPackageManagerService.NuGetPackageManagerStateChanged -= OnNuGetPackageManagerStateChanged;
		DotNetBackgroundTaskApi.DotNetSolutionService.DotNetSolutionStateChanged -= OnDotNetSolutionStateChanged;
	}
}