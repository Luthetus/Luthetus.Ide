using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;
using Luthetus.Extensions.DotNet.Nugets.States;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.Nugets.Displays;

public partial class NugetPackageDisplay : FluxorComponent
{
	[Inject]
	private IState<NuGetPackageManagerState> NuGetPackageManagerStateWrap { get; set; } = null!;
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
	[Inject]
	private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;

	[Parameter, EditorRequired]
	public NugetPackageRecord NugetPackageRecord { get; set; } = null!;

	private static readonly Key<TerminalCommandRequest> AddNugetPackageTerminalCommandRequestKey = Key<TerminalCommandRequest>.NewKey();

	private string _nugetPackageVersionString = string.Empty;

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
        	parentDirectory.Value,
        	AddNugetPackageTerminalCommandRequestKey)
        {
        	ContinueWithFunc = parsedCommand =>
        	{
        		NotificationHelper.DispatchInformative("Add Nuget Package Reference", $"{targetNugetPackage.Title}, {targetNugetVersion} was added to {targetProject.DisplayName}", CommonComponentRenderers, Dispatcher, TimeSpan.FromSeconds(7));
				return Task.CompletedTask;
        	}
        };
        	
        TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
	}
}