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

	private string _input = string.Empty;
	
	private string Input
	{
		get => _input;
		set
		{
			_input = value;
			DrawScopeInTextEditor(_input);
		}
	}
	
	protected override void OnInitialized()
    {
    	_throttleRender = new(ThrottleTimeSpan, async (_, _) =>
    	{
    		await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    	});
    
        TextEditorViewModelDisplay.RenderBatchChanged += OnRenderBatchChanged;
        OnRenderBatchChanged();
        
        base.OnInitialized();
    }

	private void OnRenderBatchChanged()
    {
    	_throttleRender.Run(0);
    }
    
    private void DrawScopeInTextEditor(string inputLocal)
    {
    	if (!Guid.TryParse(inputLocal, out var guid))
    		return;
    
    	var renderBatch = TextEditorViewModelDisplay._storedRenderBatchTuple.Validated;
    	
    	if (renderBatch is null)
    		return;
    		
    	var scopeKey = new Key<IScope>(guid);
    	
    	TextEditorService.PostUnique(nameof(TextEditorDevToolsDisplay), editContext =>
    	{
    		var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            viewModelModifier.ViewModel = viewModelModifier.ViewModel with
            {
            	FirstPresentationLayerKeysList = viewModelModifier.ViewModel.FirstPresentationLayerKeysList
            		.Add(TextEditorDevToolsPresentationFacts.PresentationKey)
            };
    	
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
	
			var scopeList = modelModifier.CompilerService.Binder
				.GetScopeList(resourceUri)
				?? Array.Empty<IScope>();
				
			var targetScope = scopeList.FirstOrDefault(x => x.Key == scopeKey);
			
			if (targetScope is null)
				return Task.CompletedTask;
    
			var textSpan = new TextEditorTextSpan(
	            targetScope.StartingIndexInclusive,
			    targetScope.EndingIndexExclusive ?? presentationModel.PendingCalculation.ContentAtRequest.Length,
			    (byte)TextEditorDevToolsDecorationKind.Scope,
			    resourceUri,
			    sourceText: string.Empty,
			    getTextPrecalculatedResult: string.Empty);
	
			var diagnosticTextSpans = new [] { textSpan }
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