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
    private readonly object _linksLock = new();
    private readonly ITextEditorService _textEditorService;
	private readonly ResourceUri _resourceUri;
    private readonly Key<TextEditorViewModel> _viewModelKey;

    public DisplayTracker(
        ITextEditorService textEditorService,
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
    /// <see cref="Links"/> refers to a Blazor TextEditorViewModelDisplay having had its OnParametersSet invoked
    /// and the ViewModelKey that was passed as a parameter matches this encompasing ViewModel's key. In this situation
    /// <see cref="Links"/> would be incremented by 1 in a concurrency safe manner.<br/><br/>
    /// 
    /// As well OnParametersSet includes the case where the ViewModelKey that was passed as a parameter is changed.
    /// In this situation the previous ViewModel would have its <see cref="Links"/> decremented by 1 in a concurrency safe manner.<br/><br/>
    /// 
    /// TextEditorViewModelDisplay implements IDisposable. In the Dispose implementation,
    /// the active ViewModel would have its <see cref="Links"/> decremented by 1 in a concurrency safe manner.
    /// </summary>
    public int Links { get; private set; }
	/// <summary>
    /// Since the UI logic is lazily calculated only for ViewModels which are currently rendered to the UI,
    /// when a ViewModel becomes rendered it needs to have its calculations performed so it is up to date.
    /// </summary>
    public bool IsFirstDisplay { get; private set; } = true;

    public void IncrementLinks()
    {
		var becameDisplayed = false;

        lock (_linksLock)
        {
            Links++;

            if (Links == 1)
            {
                // This ViewModel was not being displayed until this point.
				//
                // The goal is to lazily update the UI, i.e.: only when a given view model is being displayed on the UI.
				//
				// Therefore, presume that the UI this newly rendered view model is outdated,
				// (perhaps the font-size was changed for eample)
				IsFirstDisplay = true;
                becameDisplayed = true;

                // Furthermore, subscribe to the events which indicate that the UI has changed (for example font-size)
				// so that these events are immediately handled, considering that this view model is being rendered on the UI.
                _textEditorService.AppDimensionService.AppDimensionStateChanged += AppDimensionStateWrap_StateChanged;
            }
			else if (Links > 1)
			{
				// TODO: This exception is getting commented out for now because dragging a text editor
				//       tab off the text editor group display is using the same view model for the dialog
				//       as it was in the group tab.
				//
				//throw new LuthetusFatalException($"{nameof(DisplayTracker)} detected a {nameof(TextEditorViewModel)}" +
				//								 " was being displayed in two places simultaneously." +
				//								 " A {nameof(TextEditorViewModel)} can only be displayed by a single" +
				//								 " {nameof(TextEditorViewModelDisplay)} at a time.");
			}
        }

		if (becameDisplayed)
		{
			// TODO: If the 'AppDimensionStateWrap_StateChanged(...)' is invoking 'PostScrollAndRemeasure()' too...
			//       ... and it is believed that it is being done there to measure the UI, as the
			//       newly sized font wouldn't be guaranteed to have been rendered in time,
			//       then why is this invoked here.
			//       |
			//       The response is that the very first render of the text editor will ignore the view model,
			//       and only serves to measure the font size.
			//       |
			//       As a result when a view model becomes displayed, the text editor will already know the font-size.
			//       And it will just "be available" and this invocation is less of a re-measure, and more of a
			//       setting of what the current font measurements are on the view model being displayed.
			
			// Tell the view model what the (already known) font-size measurements and text-editor measurements are.
			PostScrollAndRemeasure();
			
			if (!_hasBeenDisplayedAtLeastOnceBefore)
			{
				_hasBeenDisplayedAtLeastOnceBefore = true;
				
				var uniqueTextEditorWork = new UniqueTextEditorWork(
		            nameof(_hasBeenDisplayedAtLeastOnceBefore),
		            _textEditorService,
		            editContext =>
		            {
						var modelModifier = editContext.GetModelModifier(_resourceUri);
		
						if (modelModifier is null)
							return ValueTask.CompletedTask;
						
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
						
						if (modelModifier.CompilerService is not null)	
							modelModifier.CompilerService.ResourceWasModified(_resourceUri, Array.Empty<TextEditorTextSpan>());

						return ValueTask.CompletedTask;
		            });
				
				_textEditorService.WorkerArbitrary.EnqueueUniqueTextEditorWork(uniqueTextEditorWork);
			}
		}
    }

    public void DecrementLinks()
    {
        lock (_linksLock)
        {
            Links--;

            if (Links == 0)
            {
                // This ViewModel will NO LONGER be rendered.
				//
				// The goal is to lazily update the UI, i.e.: only when a given view model is being displayed on the UI.
				//
				// Therefore, proceed to unsubscribe from presume that the UI this newly rendered view model is outdated,
				// (perhaps the font-size was changed for eample)
				//
                // Due to lazily updating the UI, proceed to unsubscribe from the events which indicate that the UI has changed (for example font-size).
				_textEditorService.AppDimensionService.AppDimensionStateChanged -= AppDimensionStateWrap_StateChanged;
            }
			else if (Links < 0)
			{
				throw new LuthetusFatalException($"{nameof(DisplayTracker)} has {nameof(Links)} at a value < 0: '{Links}'.");
			}
        }
    }

	public bool ConsumeIsFirstDisplay()
    {
        lock (_linksLock)
        {
            var localIsFirstDisplay = IsFirstDisplay;
            IsFirstDisplay = false;

            return localIsFirstDisplay;
        }
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
            return;

		_textEditorService.WorkerArbitrary.PostRedundant(
			nameof(AppDimensionStateWrap_StateChanged),
			model.ResourceUri,
            viewModel.ViewModelKey,
			async editContext =>
			{
				var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
				var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);
				
	            if (modelModifier is null || viewModelModifier is null)
	                return;
				
				var textEditorMeasurements = await _textEditorService.ViewModelApi
					.GetTextEditorMeasurementsAsync(viewModelModifier.BodyElementId)
					.ConfigureAwait(false);
		
				viewModelModifier.TextEditorDimensions = textEditorMeasurements;
				
				viewModelModifier.ShouldReloadVirtualizationResult = true;
				
				// TODO: Where does the method: 'ValidateMaximumScrollLeftAndScrollTop(...)' belong?
				((TextEditorService)_textEditorService).ValidateMaximumScrollLeftAndScrollTop(editContext, viewModelModifier, textEditorDimensionsChanged: true);
			});
	}

    public void Dispose()
    {
        lock (_linksLock)
		{
        	_textEditorService.AppDimensionService.AppDimensionStateChanged -= AppDimensionStateWrap_StateChanged;
		}
    }
}