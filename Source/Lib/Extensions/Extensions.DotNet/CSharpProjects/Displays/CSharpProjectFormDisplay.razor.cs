using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Extensions.DotNet.CSharpProjects.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;
using Luthetus.Extensions.DotNet.Websites;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;

namespace Luthetus.Extensions.DotNet.CSharpProjects.Displays;

public partial class CSharpProjectFormDisplay : ComponentBase, IDisposable
{
	[Inject]
	private ITerminalService TerminalService { get; set; } = null!;
	[Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
	private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
	[Inject]
	private IFileSystemProvider FileSystemProvider { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
	[Inject]
	private LuthetusIdeConfig IdeConfig { get; set; } = null!;
	[Inject]
	private TextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private IDialogService DialogService { get; set; } = null!;
	[Inject]
	private INotificationService NotificationService { get; set; } = null!;
	[Inject]
	private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
	[Inject]
	private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;

	[CascadingParameter]
	public IDialog DialogRecord { get; set; } = null!;

	[Parameter]
	public Key<DotNetSolutionModel> DotNetSolutionModelKey { get; set; }

	private CSharpProjectFormViewModel _viewModel = null!;

	private DotNetSolutionModel? DotNetSolutionModel => DotNetBackgroundTaskApi.DotNetSolutionService.GetDotNetSolutionState().DotNetSolutionsList.FirstOrDefault(
		x => x.Key == DotNetSolutionModelKey);

	protected override void OnInitialized()
	{
		_viewModel = new(DotNetSolutionModel, EnvironmentProvider);
		
		DotNetBackgroundTaskApi.DotNetSolutionService.DotNetSolutionStateChanged += OnDotNetSolutionStateChanged;
		TerminalService.TerminalStateChanged += OnTerminalStateChanged;
		
		base.OnInitialized();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await ReadProjectTemplates().ConfigureAwait(false);
		}

		await base.OnAfterRenderAsync(firstRender);
	}

	private string GetIsActiveCssClassString(CSharpProjectFormPanelKind panelKind) =>
		_viewModel.ActivePanelKind == panelKind ? "luth_active" : string.Empty;

	private void RequestInputFileForParentDirectory(string message)
	{
		IdeBackgroundTaskApi.Enqueue(new IdeBackgroundTaskApiWorkArgs
		{
			WorkKind = IdeBackgroundTaskApiWorkKind.RequestInputFileStateForm,
			Message = message,
			OnAfterSubmitFunc = async absolutePath =>
			{
				if (absolutePath.ExactInput is null)
					return;

				_viewModel.ParentDirectoryNameValue = absolutePath.Value;
				await InvokeAsync(StateHasChanged);
			},
			SelectionIsValidFunc = absolutePath =>
			{
				if (absolutePath.ExactInput is null || !absolutePath.IsDirectory)
					return Task.FromResult(false);

				return Task.FromResult(true);
			},
			InputFilePatterns = new()
			{
				new InputFilePattern("Directory", absolutePath => absolutePath.IsDirectory)
			}
		});
	}

	private async Task ReadProjectTemplates()
	{
		if (LuthetusHostingInformation.LuthetusHostingKind != LuthetusHostingKind.Photino)
		{
			_viewModel.ProjectTemplateList = WebsiteProjectTemplateFacts.WebsiteProjectTemplatesContainer.ToList();
			await InvokeAsync(StateHasChanged);
		}
		else
		{
			await EnqueueDotNetNewListAsync().ConfigureAwait(false);
		}
	}

	private async Task EnqueueDotNetNewListAsync()
	{
		try
		{
			// Render UI loading icon
			_viewModel.IsReadingProjectTemplates = true;
			await InvokeAsync(StateHasChanged);

			var formattedCommand = DotNetCliCommandFormatter.FormatDotnetNewList();
			var generalTerminal = TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY];
				
			var terminalCommandRequest = new TerminalCommandRequest(
				formattedCommand.Value,
				EnvironmentProvider.HomeDirectoryAbsolutePath.Value,
				new Key<TerminalCommandRequest>(_viewModel.LoadProjectTemplatesTerminalCommandRequestKey.Guid))
			{
				ContinueWithFunc = parsedTerminalCommand =>
				{
					DotNetCliOutputParser.ParseOutputLineDotNetNewList(parsedTerminalCommand.OutputCache.ToString());
					_viewModel.ProjectTemplateList = DotNetCliOutputParser.ProjectTemplateList ?? new();
					return InvokeAsync(StateHasChanged);
				}
			};

			generalTerminal.EnqueueCommand(terminalCommandRequest);
		}
		finally
		{
			// UI loading message
			_viewModel.IsReadingProjectTemplates = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	/// <summary>If the non-deprecated version of the command fails, then try the deprecated one.</summary>
	private async Task EnqueueDotnetNewListDeprecatedAsync()
	{
		try
		{
			// UI loading message
			_viewModel.IsReadingProjectTemplates = true;
			await InvokeAsync(StateHasChanged);

			var formattedCommand = DotNetCliCommandFormatter.FormatDotnetNewListDeprecated();
			var generalTerminal = TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY];

			var terminalCommandRequest = new TerminalCommandRequest(
	        	formattedCommand.Value,
	        	EnvironmentProvider.HomeDirectoryAbsolutePath.Value,
	        	new Key<TerminalCommandRequest>(_viewModel.LoadProjectTemplatesTerminalCommandRequestKey.Guid))
	        {
	        	ContinueWithFunc = parsedCommand =>
	        	{
		        	DotNetCliOutputParser.ParseOutputLineDotNetNewList(parsedCommand.OutputCache.ToString());
					_viewModel.ProjectTemplateList = DotNetCliOutputParser.ProjectTemplateList ?? new();
					return InvokeAsync(StateHasChanged);
				}
	        };
	        	
	        TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		}
		finally
		{
			// UI loading message
			_viewModel.IsReadingProjectTemplates = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private string GetCssClassForActivePanelKind(CSharpProjectFormPanelKind localActivePanelKind)
	{
		return localActivePanelKind switch
		{
			CSharpProjectFormPanelKind.Graphical => "luth_ide_c-sharp-project-form-graphical-panel",
			CSharpProjectFormPanelKind.Manual => "luth_ide_c-sharp-project-form-manual-panel",
			_ => throw new NotImplementedException($"The {nameof(CSharpProjectFormPanelKind)}: '{localActivePanelKind}' was unrecognized."),
		};
	}

	private async Task StartNewCSharpProjectCommandOnClick()
	{
		if (!_viewModel.TryTakeSnapshot(out var immutableView) ||
			immutableView is null)
		{
			return;
		}

		if (string.IsNullOrWhiteSpace(immutableView.ProjectTemplateShortNameValue) ||
			string.IsNullOrWhiteSpace(immutableView.CSharpProjectNameValue) ||
			string.IsNullOrWhiteSpace(immutableView.ParentDirectoryNameValue))
		{
			return;
		}

		if (LuthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.Photino)
		{
			var generalTerminal = TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY];

			var terminalCommandRequest = new TerminalCommandRequest(
	        	immutableView.FormattedNewCSharpProjectCommand.Value,
	        	immutableView.ParentDirectoryNameValue,
	        	new Key<TerminalCommandRequest>(immutableView.NewCSharpProjectTerminalCommandRequestKey.Guid))
	        {
	        	ContinueWithFunc = parsedCommand =>
	        	{
					var terminalCommandRequest = new TerminalCommandRequest(
			        	immutableView.FormattedAddExistingProjectToSolutionCommand.Value,
			        	immutableView.ParentDirectoryNameValue,
			        	new Key<TerminalCommandRequest>(immutableView.AddCSharpProjectToSolutionTerminalCommandRequestKey.Guid))
			        {
			        	ContinueWithFunc = parsedCommand =>
			        	{
				        	DialogService.ReduceDisposeAction(DialogRecord.DynamicViewModelKey);
	
							DotNetBackgroundTaskApi.Enqueue(new DotNetBackgroundTaskApiWorkArgs
							{
								WorkKind = DotNetBackgroundTaskApiWorkKind.SetDotNetSolution,
								DotNetSolutionAbsolutePath = immutableView.DotNetSolutionModel.NamespacePath.AbsolutePath,
							});
							return Task.CompletedTask;
						}
			        };
			        	
			        generalTerminal.EnqueueCommand(terminalCommandRequest);
					return Task.CompletedTask;
	        	}
	        };
	        	
	        generalTerminal.EnqueueCommand(terminalCommandRequest);
		}
		else
		{
			await WebsiteDotNetCliHelper.StartNewCSharpProjectCommand(
					immutableView,
					EnvironmentProvider,
					FileSystemProvider,
					DotNetBackgroundTaskApi,
					NotificationService,
					DialogService,
					DialogRecord,
					LuthetusCommonComponentRenderers)
				.ConfigureAwait(false);
		}
	}
	
	public async void OnDotNetSolutionStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	public async void OnTerminalStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	public void Dispose()
	{
		DotNetBackgroundTaskApi.DotNetSolutionService.DotNetSolutionStateChanged -= OnDotNetSolutionStateChanged;
		TerminalService.TerminalStateChanged -= OnTerminalStateChanged;
	}
}
