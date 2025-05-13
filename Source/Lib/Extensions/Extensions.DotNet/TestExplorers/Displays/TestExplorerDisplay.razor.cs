using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Displays.Internals;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.DotNet.TestExplorers.Displays;

public partial class TestExplorerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
	private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
	private TextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private ITerminalService TerminalService { get; set; } = null!;

	protected override void OnInitialized()
	{
		DotNetBackgroundTaskApi.TestExplorerService.TestExplorerStateChanged += OnTestExplorerStateChanged;
		TreeViewService.TreeViewStateChanged += OnTreeViewStateChanged;
		TerminalService.TerminalStateChanged += OnTerminalStateChanged;

		_ = Task.Run(async () =>
		{
			await DotNetBackgroundTaskApi.TestExplorerService
				.HandleUserInterfaceWasInitializedEffect()
				.ConfigureAwait(false);
		});

		base.OnInitialized();
	}
	
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			var model = TextEditorService.ModelApi.GetOrDefault(
				ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri);

			if (model is null)
			{
				TextEditorService.WorkerArbitrary.PostUnique(nameof(TestExplorerDisplay), async editContext =>
				{
					var terminalDecorationMapper = DecorationMapperRegistry.GetDecorationMapper(ExtensionNoPeriodFacts.TERMINAL);
					var terminalCompilerService = CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL);
	
					model = new TextEditorModel(
						ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri,
						DateTime.UtcNow,
						ExtensionNoPeriodFacts.TERMINAL,
						"initialContent:TestExplorerDetailsTextEditorResourceUri",
	                    terminalDecorationMapper,
	                    terminalCompilerService,
	                    TextEditorService);
	
					TextEditorService.ModelApi.RegisterCustom(editContext, model);
	
					TextEditorService.ViewModelApi.Register(
						editContext,
						TestExplorerDetailsDisplay.DetailsTextEditorViewModelKey,
						ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri,
						new Category("terminal"));
	
					RegisterDetailsTextEditor(model);
	
					await InvokeAsync(StateHasChanged);
				});
			}
		}

		await base.OnAfterRenderAsync(firstRender);
	}

	private void RegisterDetailsTextEditor(TextEditorModel model)
	{
		TextEditorService.WorkerArbitrary.PostUnique(
			nameof(TextEditorService.ModelApi.AddPresentationModel),
			editContext =>
			{
				var modelModifier = editContext.GetModelModifier(model.PersistentState.ResourceUri);
			
				TextEditorService.ModelApi.AddPresentationModel(
					editContext,
					modelModifier,
					TerminalPresentationFacts.EmptyPresentationModel);

				TextEditorService.ModelApi.AddPresentationModel(
					editContext,
					modelModifier,
					CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);

				TextEditorService.ModelApi.AddPresentationModel(
					editContext,
					modelModifier,
					FindOverlayPresentationFacts.EmptyPresentationModel);

				model.PersistentState.CompilerService.RegisterResource(
					model.PersistentState.ResourceUri,
					shouldTriggerResourceWasModified: true);

				var viewModelModifier = editContext.GetViewModelModifier(TestExplorerDetailsDisplay.DetailsTextEditorViewModelKey);

				if (viewModelModifier is null)
					throw new NullReferenceException();

				var firstPresentationLayerKeys = new List<Key<TextEditorPresentationModel>>
				{
					TerminalPresentationFacts.PresentationKey,
					CompilerServiceDiagnosticPresentationFacts.PresentationKey,
					FindOverlayPresentationFacts.PresentationKey,
				};

				viewModelModifier.PersistentState.FirstPresentationLayerKeysList = firstPresentationLayerKeys;
				
				return ValueTask.CompletedTask;
			});
	}
	
	private void DispatchShouldDiscoverTestsEffect()
	{
		_ = Task.Run(async () =>
		{
			await DotNetBackgroundTaskApi.TestExplorerService
				.HandleShouldDiscoverTestsEffect()
				.ConfigureAwait(false);
		});
	}
	
	private void KillExecutionProcessOnClick()
	{
		var terminalState = TerminalService.GetTerminalState();
		var executionTerminal = terminalState.TerminalMap[TerminalFacts.EXECUTION_KEY];
		executionTerminal.KillProcess();
	}
	
	private bool GetIsKillProcessDisabled()
	{
		var terminalState = TerminalService.GetTerminalState();
		var executionTerminal = terminalState.TerminalMap[TerminalFacts.EXECUTION_KEY];
		return !executionTerminal.HasExecutingProcess;
	}
	
	private async void OnTestExplorerStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	private async void OnTreeViewStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	private async void OnTerminalStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	public void Dispose()
	{
		DotNetBackgroundTaskApi.TestExplorerService.TestExplorerStateChanged -= OnTestExplorerStateChanged;
		TreeViewService.TreeViewStateChanged -= OnTreeViewStateChanged;
		TerminalService.TerminalStateChanged -= OnTerminalStateChanged;
	}
}