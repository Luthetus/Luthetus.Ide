using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorDevToolsDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	public ITextEditorService TextEditorService { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;
	
	private static readonly TimeSpan ThrottleTimeSpan = TimeSpan.FromMilliseconds(500);

	/// <summary>byte is used as TArgs just as a "throwaway" type. It isn't used.</summary>
	private ThrottleOptimized<byte> _throttleRender;
	
	protected override void OnInitialized()
    {
    	_throttleRender = new(ThrottleTimeSpan, async (_, _) =>
    	{
    		DrawScopeInTextEditor();
    	});
    
        TextEditorViewModelDisplay.RenderBatchChanged += OnRenderBatchChanged;
        OnRenderBatchChanged();
        
        base.OnInitialized();
    }

	private void OnRenderBatchChanged()
    {
    	_throttleRender.Run(0);
    }
    
    private void DrawScopeInTextEditor()
    {
    	var renderBatch = TextEditorViewModelDisplay._storedRenderBatchTuple.Validated;
    	
    	if (renderBatch is null)
    		return;
    	
    	TextEditorService.PostUnique(nameof(TextEditorDevToolsDisplay), editContext =>
    	{
    		var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
            
            if (!viewModelModifier.ViewModel.FirstPresentationLayerKeysList.Contains(
            		TextEditorDevToolsPresentationFacts.PresentationKey))
            {
	            viewModelModifier.ViewModel = viewModelModifier.ViewModel with
	            {
	            	FirstPresentationLayerKeysList = viewModelModifier.ViewModel.FirstPresentationLayerKeysList
	            		.Add(TextEditorDevToolsPresentationFacts.PresentationKey)
	            };
	        }
    	
    		TextEditorService.ModelApi.StartPendingCalculatePresentationModel(
				editContext,
		        modelModifier,
		        TextEditorDevToolsPresentationFacts.PresentationKey,
				TextEditorDevToolsPresentationFacts.EmptyPresentationModel);
	
			var presentationModel = modelModifier.PresentationModelList.First(
				x => x.TextEditorPresentationKey == TextEditorDevToolsPresentationFacts.PresentationKey);
	
			if (presentationModel.PendingCalculation is null)
				throw new LuthetusTextEditorException($"{nameof(presentationModel)}.{nameof(presentationModel.PendingCalculation)} was not expected to be null here.");
	
	        var resourceUri = modelModifier.ResourceUri;
	
			var targetScope = modelModifier.CompilerService.Binder.
				GetScope(resourceUri, modelModifier.GetPositionIndex(primaryCursorModifier));
			
			if (targetScope is null)
				return Task.CompletedTask;
    
    		var textSpanStart = new TextEditorTextSpan(
	            targetScope.StartingIndexInclusive,
	            targetScope.StartingIndexInclusive + 1,
			    (byte)TextEditorDevToolsDecorationKind.Scope,
			    resourceUri,
			    sourceText: string.Empty,
			    getTextPrecalculatedResult: string.Empty);
    		
			var textSpanEnd = new TextEditorTextSpan(
	            (targetScope.EndingIndexExclusive ?? presentationModel.PendingCalculation.ContentAtRequest.Length) - 1,
			    targetScope.EndingIndexExclusive ?? presentationModel.PendingCalculation.ContentAtRequest.Length,
			    (byte)TextEditorDevToolsDecorationKind.Scope,
			    resourceUri,
			    sourceText: string.Empty,
			    getTextPrecalculatedResult: string.Empty);
	
			var diagnosticTextSpans = new [] { textSpanStart, textSpanEnd }
				.ToImmutableArray();

			modelModifier.CompletePendingCalculatePresentationModel(
				TextEditorDevToolsPresentationFacts.PresentationKey,
				TextEditorDevToolsPresentationFacts.EmptyPresentationModel,
				diagnosticTextSpans);
	
    		return Task.CompletedTask;
    	});
    }

	public void Dispose()
    {
    	TextEditorViewModelDisplay.RenderBatchChanged -= OnRenderBatchChanged;
    }
}