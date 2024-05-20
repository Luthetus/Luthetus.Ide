using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Ide.RazorLib.TestExplorers.States;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.TestExplorers.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays;

public partial class TestExplorerDisplay : FluxorComponent
{
	[Inject]
    private IState<TestExplorerState> TestExplorerStateWrap { get; set; } = null!;
	[Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
	[Inject]
    private IState<TreeViewState> TreeViewStateWrap { get; set; } = null!;
	[Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

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
				model = new TextEditorModel(
					ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri,
					DateTime.UtcNow,
					ExtensionNoPeriodFacts.TERMINAL,
					"initialContent:TestExplorerDetailsTextEditorResourceUri",
					null,
					null);

				TextEditorService.ModelApi.RegisterCustom(model);

				TextEditorService.ViewModelApi.Register(
					TestExplorerDetailsDisplay.DetailsTextEditorViewModelKey,
					ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri,
					new Category("terminal"));

				await RegisterDetailsTextEditor(model);
				
				await InvokeAsync(StateHasChanged);
			}
		}

		await base.OnAfterRenderAsync(firstRender);
	}

	private async Task RegisterDetailsTextEditor(TextEditorModel model)
	{
		await TextEditorService.PostSimpleBatch(
			nameof(TextEditorService.ModelApi.AddPresentationModelFactory),
			string.Empty,
			async editContext =>
			{
				await TextEditorService.ModelApi.AddPresentationModelFactory(
						model.ResourceUri,
						TerminalPresentationFacts.EmptyPresentationModel)
					.Invoke(editContext)
					.ConfigureAwait(false);

				await TextEditorService.ModelApi.AddPresentationModelFactory(
						model.ResourceUri,
						CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel)
					.Invoke(editContext)
					.ConfigureAwait(false);

				await TextEditorService.ModelApi.AddPresentationModelFactory(
						model.ResourceUri,
						FindOverlayPresentationFacts.EmptyPresentationModel)
					.Invoke(editContext)
					.ConfigureAwait(false);

				model.CompilerService.RegisterResource(model.ResourceUri);

				var viewModelModifier = editContext.GetViewModelModifier(TestExplorerDetailsDisplay.DetailsTextEditorViewModelKey);

				if (viewModelModifier is null)
					throw new NullReferenceException();

				var layerFirstPresentationKeys = new[]
				{
					TerminalPresentationFacts.PresentationKey,
					CompilerServiceDiagnosticPresentationFacts.PresentationKey,
					FindOverlayPresentationFacts.PresentationKey,
				}.ToImmutableArray();

				viewModelModifier.ViewModel = viewModelModifier.ViewModel with
				{
					FirstPresentationLayerKeysList = layerFirstPresentationKeys.ToImmutableList()
				};
			});
	}
}