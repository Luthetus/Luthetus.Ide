using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Displays.Internals;

namespace Luthetus.Extensions.DotNet.TestExplorers.Displays;

public partial class TestExplorerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
	private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;

	protected override void OnInitialized()
	{
		DotNetBackgroundTaskApi.TestExplorerService.TestExplorerStateChanged += OnTestExplorerStateChanged;
		TreeViewService.TreeViewStateChanged += OnTreeViewStateChanged;

		DotNetBackgroundTaskApi.TestExplorerService.HandleUserInterfaceWasInitializedEffect();

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
				var terminalDecorationMapper = DecorationMapperRegistry.GetDecorationMapper(ExtensionNoPeriodFacts.TERMINAL);
				var terminalCompilerService = CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL);

				model = new TextEditorModel(
					ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri,
					DateTime.UtcNow,
					ExtensionNoPeriodFacts.TERMINAL,
					"initialContent:TestExplorerDetailsTextEditorResourceUri",
                    terminalDecorationMapper,
                    terminalCompilerService);

				TextEditorService.ModelApi.RegisterCustom(model);

				TextEditorService.ViewModelApi.Register(
					TestExplorerDetailsDisplay.DetailsTextEditorViewModelKey,
					ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri,
					new Category("terminal"));

				RegisterDetailsTextEditor(model);

				await InvokeAsync(StateHasChanged);
			}
		}

		await base.OnAfterRenderAsync(firstRender);
	}

	private void RegisterDetailsTextEditor(TextEditorModel model)
	{
		TextEditorService.TextEditorWorker.PostUnique(
			nameof(TextEditorService.ModelApi.AddPresentationModel),
			editContext =>
			{
				var modelModifier = editContext.GetModelModifier(model.ResourceUri);
			
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

				model.CompilerService.RegisterResource(
					model.ResourceUri,
					shouldTriggerResourceWasModified: true);

				var viewModelModifier = editContext.GetViewModelModifier(TestExplorerDetailsDisplay.DetailsTextEditorViewModelKey);

				if (viewModelModifier is null)
					throw new NullReferenceException();

				var firstPresentationLayerKeys = new[]
				{
					TerminalPresentationFacts.PresentationKey,
					CompilerServiceDiagnosticPresentationFacts.PresentationKey,
					FindOverlayPresentationFacts.PresentationKey,
				}.ToImmutableArray();

				viewModelModifier.ViewModel = viewModelModifier.ViewModel with
				{
					FirstPresentationLayerKeysList = firstPresentationLayerKeys.ToImmutableList()
				};
				
				return ValueTask.CompletedTask;
			});
	}
	
	private void DispatchShouldDiscoverTestsEffect()
	{
		DotNetBackgroundTaskApi.TestExplorerService.HandleShouldDiscoverTestsEffect();
	}
	
	private async void OnTestExplorerStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	private async void OnTreeViewStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	public void Dispose()
	{
		DotNetBackgroundTaskApi.TestExplorerService.TestExplorerStateChanged -= OnTestExplorerStateChanged;
		TreeViewService.TreeViewStateChanged -= OnTreeViewStateChanged;
	}
}