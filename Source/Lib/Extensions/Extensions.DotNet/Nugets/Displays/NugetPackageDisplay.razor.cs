using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.Nugets.Displays;

public partial class NugetPackageDisplay : ComponentBase, IDisposable
{
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private ITerminalService TerminalService { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private INotificationService NotificationService { get; set; } = null!;

	[Parameter, EditorRequired]
	public NugetPackageRecord NugetPackageRecord { get; set; } = null!;

	private static readonly Key<TerminalCommandRequest> AddNugetPackageTerminalCommandRequestKey = Key<TerminalCommandRequest>.NewKey();

	private string _nugetPackageVersionString = string.Empty;

	private List<NugetPackageVersionRecord> _nugetPackageVersionsOrdered = new();
	private string? _previousNugetPackageId;

	protected override void OnInitialized()
	{
		DotNetBackgroundTaskApi.NuGetPackageManagerService.NuGetPackageManagerStateChanged += OnNuGetPackageManagerStateChanged;
		DotNetBackgroundTaskApi.DotNetSolutionService.DotNetSolutionStateChanged += OnDotNetSolutionStateChanged;
		base.OnInitialized();
	}
	
	protected override void OnParametersSet()
	{
		if (_previousNugetPackageId is null || _previousNugetPackageId != NugetPackageRecord.Id)
		{
			_previousNugetPackageId = NugetPackageRecord.Id;

			_nugetPackageVersionsOrdered = NugetPackageRecord.Versions
				.OrderByDescending(x => x.Version)
				.ToList();

			_nugetPackageVersionString = _nugetPackageVersionsOrdered.FirstOrDefault()
				?.Version ?? string.Empty;
		}

		base.OnParametersSet();
	}

	private void SelectedNugetVersionChanged(ChangeEventArgs changeEventArgs)
	{
		_nugetPackageVersionString = changeEventArgs.Value?.ToString() ?? string.Empty;
	}

	private bool ValidateSolutionContainsSelectedProject(
		DotNetSolutionState dotNetSolutionState,
		NuGetPackageManagerState nuGetPackageManagerState)
	{
		if (dotNetSolutionState.DotNetSolutionModel is null || nuGetPackageManagerState.SelectedProjectToModify is null)
			return false;

		return dotNetSolutionState.DotNetSolutionModel.DotNetProjectList.Any(
			x => x.ProjectIdGuid == nuGetPackageManagerState.SelectedProjectToModify.ProjectIdGuid);
	}

	private void AddNugetPackageReferenceOnClick(
		DotNetSolutionState dotNetSolutionState,
		NuGetPackageManagerState nuGetPackageManagerState)
	{
		var targetProject = nuGetPackageManagerState.SelectedProjectToModify;
		var targetNugetPackage = NugetPackageRecord;
		var targetNugetVersion = _nugetPackageVersionString;

		var isValid = ValidateSolutionContainsSelectedProject(dotNetSolutionState, nuGetPackageManagerState);

		if (!isValid || targetProject is null)
		{
			return;
		}

		var parentDirectory = targetProject.AbsolutePath.ParentDirectory;

		if (parentDirectory is null)
			return;

		var formattedCommand = DotNetCliCommandFormatter.FormatAddNugetPackageReferenceToProject(
			targetProject.AbsolutePath.Value,
			targetNugetPackage.Id,
			targetNugetVersion);

		var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	parentDirectory,
        	AddNugetPackageTerminalCommandRequestKey)
        {
        	ContinueWithFunc = parsedCommand =>
        	{
        		NotificationHelper.DispatchInformative("Add Nuget Package Reference", $"{targetNugetPackage.Title}, {targetNugetVersion} was added to {targetProject.DisplayName}", CommonComponentRenderers, NotificationService, TimeSpan.FromSeconds(7));
				return Task.CompletedTask;
        	}
        };
        	
        TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
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