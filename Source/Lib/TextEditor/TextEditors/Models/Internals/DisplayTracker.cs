using Luthetus.Common.RazorLib.Exceptions;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// One must track whether the ViewModel is currently being rendered.<br/><br/>
/// 
/// The reason for this is that the UI logic is lazily invoked.
/// That is to say, if a ViewModel has its underlying Model change, BUT the ViewModel is not currently being rendered.
/// Then that ViewModel does not react to the Model having changed.
/// </summary>
public sealed class DisplayTracker : IDisposable
{
    private readonly TextEditorService _textEditorService;
	private readonly ResourceUri _resourceUri;
    private readonly Key<TextEditorViewModel> _viewModelKey;

    public DisplayTracker(
        TextEditorService textEditorService,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
        _textEditorService = textEditorService;
        _resourceUri = resourceUri;
        _viewModelKey = viewModelKey;
    }
    
    /// <summary>
    /// The initial solution wide parse will no longer apply syntax highlighting.
    ///
    /// As a result, the first time a view model is rendered, it needs to
    /// trigger the syntax highlighting to be applied.
    ///
    /// Preferably this would work with a 'SyntaxHighlightingIsDirty'
    /// sort of pattern.
    ///
    /// I couldn't get 'SyntaxHighlightingIsDirty' working and am tired.
    /// This is too big of an optimization to miss out on
    /// so I'll do the easier answer and leave this note.
    /// </summary>
    private bool _hasBeenDisplayedAtLeastOnceBefore;

    /// <summary>
    /// <see cref="Links"/> refers to a Blazor TextEditorViewModelSlimDisplay having had its OnParametersSet invoked
    /// and the ViewModelKey that was passed as a parameter matches this encompasing ViewModel's key. In this situation
    /// <see cref="Links"/> would be incremented by 1 in a concurrency safe manner.<br/><br/>
    /// 
    /// As well OnParametersSet includes the case where the ViewModelKey that was passed as a parameter is changed.
    /// In this situation the previous ViewModel would have its <see cref="Links"/> decremented by 1 in a concurrency safe manner.<br/><br/>
    /// 
    /// TextEditorViewModelSlimDisplay implements IDisposable. In the Dispose implementation,
    /// the active ViewModel would have its <see cref="Links"/> decremented by 1 in a concurrency safe manner.
    /// </summary>
    public TextEditorComponentData? ComponentData { get; private set; }

    public void RegisterComponentData(TextEditorEditContext editContext, TextEditorComponentData componentData)
    {
    	if (ComponentData is not null)
    	{
    		if (componentData.TextEditorHtmlElementId == ComponentData.TextEditorHtmlElementId)
	    	{
	    		Console.WriteLine($"TODO: {nameof(DisplayTracker)} {nameof(RegisterComponentData)} - ComponentData is not null (same component tried registering twice)");
	    		return;
	    	}
	    	
    		Console.WriteLine($"TODO: {nameof(DisplayTracker)} {nameof(RegisterComponentData)} - ComponentData is not null");
    		return;
    	}
    
        ComponentData = componentData;
		_textEditorService.AppDimensionService.AppDimensionStateChanged += AppDimensionStateWrap_StateChanged;

		// Tell the view model what the (already known) font-size measurements and text-editor measurements are.
		PostScrollAndRemeasure();
		
		if (!_hasBeenDisplayedAtLeastOnceBefore)
		{
			_hasBeenDisplayedAtLeastOnceBefore = true;
			
			var modelModifier = editContext.GetModelModifier(_resourceUri);
			if (modelModifier is not null)
			{
				// If this 'ApplySyntaxHighlighting(...)' isn't redundantly invoked prior to
				// the upcoming 'ResourceWasModified(...)' invocation,
				// then there is an obnoxious "flicker" upon opening a file for the first time.
				//
				// This is because it initially opens with 'plain text' syntax highlighting
				// for all the text.
				//
				// Then very soon after it gets the correct syntax highlighting applied.
				// The issue is specifically how quickly it gets the correct syntax highlighting.
				//
				// It is the same issue as putting a 'loading...' icon or text
				// for an asynchronous event, but that event finishes in sub 200ms so the user
				// sees a "flicker" of the 'loading...' text and it just is disorienting to see.
				editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
					editContext,
					modelModifier);
				
				if (modelModifier.PersistentState.CompilerService is not null)	
					modelModifier.PersistentState.CompilerService.ResourceWasModified(_resourceUri, Array.Empty<TextEditorTextSpan>());
			}
		}
    }

    public void DisposeComponentData(TextEditorEditContext editContext, TextEditorComponentData componentData)
    {
    	if (componentData is null || ComponentData is null)
    	{
    		Console.WriteLine($"TODO: {nameof(DisplayTracker)} {nameof(DisposeComponentData)} - componentData is null || ComponentData is null.");
			return;
    	}
    	else if (componentData.TextEditorHtmlElementId != ComponentData.TextEditorHtmlElementId)
    	{
    		Console.WriteLine($"TODO: {nameof(DisplayTracker)} {nameof(DisposeComponentData)} - ComponentData.TextEditorHtmlElementId does not match.");
			return;
    	}
    
        ComponentData = null;
		_textEditorService.AppDimensionService.AppDimensionStateChanged -= AppDimensionStateWrap_StateChanged;
    }

    private void AppDimensionStateWrap_StateChanged()
    {
    	// The UI was resized, and therefore the text-editor measurements need to be re-measured.
    	//
    	// The font-size is theoretically un-changed,
    	// but will be measured anyway just because its part of the same method that does the text-editor measurements.
		PostScrollAndRemeasure();
    }

	public void PostScrollAndRemeasure()
	{
		var model = _textEditorService.ModelApi.GetOrDefault(_resourceUri);
        var viewModel = _textEditorService.ViewModelApi.GetOrDefault(_viewModelKey);

        if (model is null || viewModel is null)
        {
        	Console.WriteLine("FAIL:PostScrollAndRemeasure()");
            return;
        }

		_textEditorService.WorkerArbitrary.PostUnique(async editContext =>
		{
			var modelModifier = editContext.GetModelModifier(viewModel.PersistentState.ResourceUri);
			var viewModelModifier = editContext.GetViewModelModifier(viewModel.PersistentState.ViewModelKey);
            if (modelModifier is null || viewModelModifier is null)
            {
            	Console.WriteLine("FAIL:PostScrollAndRemeasure()");
                return;
            }
            
            var componentData = viewModel.PersistentState.DisplayTracker.ComponentData;
            if (componentData is null)
            	return;
			
			var textEditorDimensions = await _textEditorService.ViewModelApi
				.GetTextEditorMeasurementsAsync(componentData.RowSectionElementId)
				.ConfigureAwait(false);
	
			viewModelModifier.TextEditorDimensions = textEditorDimensions;
			viewModelModifier.ShouldCalculateVirtualizationResult = true;
			
			// TODO: Where does the method: 'ValidateMaximumScrollLeftAndScrollTop(...)' belong?
			((TextEditorService)_textEditorService).ValidateMaximumScrollLeftAndScrollTop(editContext, modelModifier, viewModelModifier, textEditorDimensionsChanged: true);
			
			componentData.Virtualized_LineIndexCache_IsInvalid = true;
		});
	}

    public void Dispose()
    {
    	_textEditorService.WorkerArbitrary.PostUnique(async editContext =>
    	{
    		DisposeComponentData(editContext, ComponentData);
    	});
    }
}