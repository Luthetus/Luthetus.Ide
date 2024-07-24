using Luthetus.Common.RazorLib.Exceptions;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// One must track whether the ViewModel is currently being rendered.<br/><br/>
/// 
/// The reason for this is that the UI logic is lazily invoked.
/// That is to say, if a ViewModel has its underlying Model change, BUT the ViewModel is not currently being rendered.
/// Then that ViewModel does not react to the Model having changed.
/// </summary>
public class DisplayTracker : IDisposable
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
                _textEditorService.AppDimensionStateWrap.StateChanged += AppDimensionStateWrap_StateChanged;
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
			_ = Task.Run(PostScrollAndRemeasure);
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
				_textEditorService.AppDimensionStateWrap.StateChanged -= AppDimensionStateWrap_StateChanged;
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

    private async void AppDimensionStateWrap_StateChanged(object? sender, EventArgs e)
    {
		await PostScrollAndRemeasure();
    }

	private Task PostScrollAndRemeasure()
	{
		var model = _textEditorService.ModelApi.GetOrDefault(_resourceUri);
        var viewModel = _textEditorService.ViewModelApi.GetOrDefault(_viewModelKey);

        if (model is null || viewModel is null)
            return Task.CompletedTask;

		// The 'Remeasure' command as of this comment
		// does not use the 'commandArgs' parameter
        var commandArgs = (TextEditorCommandArgs?)null;

		_textEditorService.PostRedundant(
			nameof(AppDimensionStateWrap_StateChanged),
			model.ResourceUri,
            viewModel.ViewModelKey,
			editContext =>
			{
				var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
				var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);
				
	            if (modelModifier is null || viewModelModifier is null)
	                return Task.CompletedTask;
	
				viewModelModifier.ScrollWasModified = true;
				
				TextEditorCommandDefaultFunctions.TriggerRemeasure(
	                editContext,
	                viewModelModifier,
	                commandArgs);

				// This virtualization result calculation is intentionally posted from within a post,
				// in order to ensure that the preceeding remeasure is executed and the state is updated first
				_textEditorService.PostRedundant(
	                nameof(TextEditorService.ViewModelApi.CalculateVirtualizationResult),
					model.ResourceUri,
	                viewModel.ViewModelKey,
	                editContext =>
	                {
	                	_textEditorService.ViewModelApi.CalculateVirtualizationResult(
	                		editContext,
					        modelModifier,
					        viewModelModifier,
					        CancellationToken.None);
		                return Task.CompletedTask;
		            });

				return Task.CompletedTask;
			});
		return Task.CompletedTask;
	}

    public void Dispose()
    {
        lock (_linksLock)
		{
        	_textEditorService.AppDimensionStateWrap.StateChanged -= AppDimensionStateWrap_StateChanged;
		}
    }
}