using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using Luthetus.Common.RazorLib.Options.States;
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
using Luthetus.Extensions.DotNet.TestExplorers.States;
using Luthetus.Extensions.DotNet.TestExplorers.Displays.Internals;

namespace Luthetus.Extensions.DotNet.TestExplorers.Displays;

public partial class TestExplorerDisplay : FluxorComponent
{
	[Inject]
	private IState<TestExplorerState> TestExplorerStateWrap { get; set; } = null!;
	[Inject]
	private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
	[Inject]
	private IState<TreeViewState> TreeViewStateWrap { get; set; } = null!;
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
	private IDispatcher Dispatcher { get; set; } = null!;

	private readonly ElementDimensions _treeViewElementDimensions = new();
	private readonly ElementDimensions _detailsElementDimensions = new();

	protected override void OnInitialized()
	{
		// TODO: Supress un-used property on TreeViewStateWrap...
		// ...Its injected so that Fluxor will wire up events to re-render UI...
		// ...Preferably a different approach would be taken here.
		_ = TreeViewStateWrap;

		// TreeView ElementDimensions
		{
			var treeViewWidth = _treeViewElementDimensions.DimensionAttributeList.Single(
				da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

			treeViewWidth.DimensionUnitList.AddRange(new[]
			{
				new DimensionUnit
				{
					Value = 50,
					DimensionUnitKind = DimensionUnitKind.Percentage
				},
				new DimensionUnit
				{
					Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
					DimensionUnitKind = DimensionUnitKind.Pixels,
					DimensionOperatorKind = DimensionOperatorKind.Subtract
				}
			});
		}

		// Details ElementDimensions
		{
			var detailsWidth = _detailsElementDimensions.DimensionAttributeList.Single(
				da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

			detailsWidth.DimensionUnitList.AddRange(new[]
			{
				new DimensionUnit
				{
					Value = 50,
					DimensionUnitKind = DimensionUnitKind.Percentage
				},
				new DimensionUnit
				{
					Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
					DimensionUnitKind = DimensionUnitKind.Pixels,
					DimensionOperatorKind = DimensionOperatorKind.Subtract
				}
			});
		}

		Dispatcher.Dispatch(new TestExplorerState.UserInterfaceWasInitializedEffect());

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
		TextEditorService.PostUnique(
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

				model.CompilerService.RegisterResource(model.ResourceUri);

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
				
				return Task.CompletedTask;
			});
	}
}