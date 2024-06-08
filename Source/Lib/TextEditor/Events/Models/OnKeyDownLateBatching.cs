using System.Text;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.AspNetCore.Components.Web;
using static Luthetus.TextEditor.RazorLib.TextEditors.Displays.TextEditorViewModelDisplay;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public class OnKeyDownLateBatching : ITextEditorTask
{
    private readonly TextEditorEvents _events;

    public OnKeyDownLateBatching(
			ViewModelDisplayOptions viewModelDisplayOptions,
	        TextEditorEvents events,
	        KeyboardEventArgs keyboardEventArgs,
	        ResourceUri resourceUri,
	        Key<TextEditorViewModel> viewModelKey)
		: this(viewModelDisplayOptions, events, new List<KeyboardEventArgs>() { keyboardEventArgs }, resourceUri, viewModelKey)
    {
    }

	public OnKeyDownLateBatching(
		ViewModelDisplayOptions viewModelDisplayOptions,
        TextEditorEvents events,
        List<KeyboardEventArgs> keyboardEventArgsList,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
		ViewModelDisplayOptions = viewModelDisplayOptions;
        _events = events;

        KeyboardEventArgsList = keyboardEventArgsList;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; private set; } = nameof(OnKeyDownLateBatching);
    public Task? WorkProgress { get; }
    public TimeSpan ThrottleTimeSpan => TextEditorEvents.ThrottleDelayDefault;
    public List<KeyboardEventArgs> KeyboardEventArgsList { get; }
	public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
	public IEditContext EditContext { get; set; }
	public ViewModelDisplayOptions ViewModelDisplayOptions { get; set; }

	/// <summary>
	/// Global variable used during <see cref="HandleEvent"/> to
	/// iterate over <see cref="KeyboardEventArgsList"/>
	/// </summary>
	private int _index;

    public IBackgroundTask? BatchOrDefault(IBackgroundTask upstreamEvent)
    {
		if (upstreamEvent is OnKeyDownLateBatching upstreamOnKeyDownLateBatching)
		{
			upstreamOnKeyDownLateBatching.KeyboardEventArgsList.AddRange(KeyboardEventArgsList);
			return upstreamOnKeyDownLateBatching;
		}

		return null;
    }

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
		try
		{
			Name += $"_{KeyboardEventArgsList.Count}";

            var modelModifier = EditContext.GetModelModifier(ResourceUri);
            var viewModelModifier = EditContext.GetViewModelModifier(ViewModelKey);
            var cursorModifierBag = EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return;

			_index = 0;

			for (; _index < KeyboardEventArgsList.Count; _index++)
			{
				var keyboardEventArgs = KeyboardEventArgsList[_index];

	            var definiteHasSelection = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);
	
	            var definiteKeyboardEventArgsKind = EventUtils.GetKeyboardEventArgsKind(
	                _events, keyboardEventArgs, definiteHasSelection, _events.TextEditorService, out var command);
	
	            var shouldInvokeAfterOnKeyDownAsync = false;
	
	            switch (definiteKeyboardEventArgsKind)
	            {
	                case KeyboardEventArgsKind.Command:
	                    shouldInvokeAfterOnKeyDownAsync = true;

						var commandArgs = new TextEditorCommandArgs(
                            modelModifier.ResourceUri,
                            viewModelModifier.ViewModel.ViewModelKey,
                            TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier),
                            _events.ClipboardService,
                            _events.TextEditorService,
                            _events.Options,
                            _events,
                            _events.HandleMouseStoppedMovingEventAsync,
                            _events.JsRuntime,
                            _events.Dispatcher,
                            _events.ServiceProvider,
                            _events.TextEditorConfig);

                        if (command is TextEditorCommand textEditorCommand &&
                            textEditorCommand.TextEditorEditFactory is not null)
                        {
							// Avoid invoking ITextEditorService.Post(...) since we already have an IEditContext.
                            await textEditorCommand.TextEditorEditFactory
                                .Invoke(commandArgs)
                                .Invoke(EditContext)
                                .ConfigureAwait(false);
                        }
                        else
                        {
							// This isn't desirable, it will re-post using 'ITextEditorService.Post(...)'
                            await command.CommandFunc
                                .Invoke(commandArgs)
                                .ConfigureAwait(false);
                        }
	                    break;
	                case KeyboardEventArgsKind.Movement:
	                    if ((KeyboardKeyFacts.MovementKeys.ARROW_DOWN == keyboardEventArgs.Key || KeyboardKeyFacts.MovementKeys.ARROW_UP == keyboardEventArgs.Key) &&
	                        _events.CursorDisplay is not null && _events.CursorDisplay.MenuKind == MenuKind.AutoCompleteMenu)
	                    {
							// Labeling any IEditContext -> JavaScript interop or Blazor StateHasChanged.
							// Reason being, these are likely to be huge optimizations (2024-05-29).
	                        await _events.CursorDisplay.SetFocusToActiveMenuAsync().ConfigureAwait(false);
	                    }
	                    else
	                    {
	                        await _events.TextEditorService.ViewModelApi.MoveCursorFactory(
	                                keyboardEventArgs,
	                                modelModifier.ResourceUri,
	                                viewModelModifier.ViewModel.ViewModelKey)
	                            .Invoke(EditContext)
	                            .ConfigureAwait(false);
	
							// Labeling any IEditContext -> JavaScript interop or Blazor StateHasChanged.
							// Reason being, these are likely to be huge optimizations (2024-05-29).
	                        await (_events.CursorDisplay?.SetShouldDisplayMenuAsync(MenuKind.None) ?? Task.CompletedTask)
	                            .ConfigureAwait(false);
	                    }
	                    break;
	                case KeyboardEventArgsKind.ContextMenu:
						// Labeling any IEditContext -> JavaScript interop or Blazor StateHasChanged.
						// Reason being, these are likely to be huge optimizations (2024-05-29).
	                    await (_events.CursorDisplay?.SetShouldDisplayMenuAsync(MenuKind.ContextMenu) ?? Task.CompletedTask)
	                        .ConfigureAwait(false);
	                    break;
	                case KeyboardEventArgsKind.Text:
	                case KeyboardEventArgsKind.Other:
	                    shouldInvokeAfterOnKeyDownAsync = true;
	
	                    if (!_events.IsAutocompleteMenuInvoker(keyboardEventArgs))
	                    {
	                        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key ||
	                            KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventArgs.Key ||
	                            KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventArgs.Key ||
	                            !KeyboardKeyFacts.IsMetaKey(keyboardEventArgs))
	                        {
								// Labeling any IEditContext -> JavaScript interop or Blazor StateHasChanged.
								// Reason being, these are likely to be huge optimizations (2024-05-29).
	                            await (_events.CursorDisplay?.SetShouldDisplayMenuAsync(MenuKind.None) ?? Task.CompletedTask)
	                                .ConfigureAwait(false);
	                        }
	                    }
	
	                    _events.TooltipViewModel = null;

						if (definiteKeyboardEventArgsKind == KeyboardEventArgsKind.Text)
						{
							// Batch contiguous insertions
							var contiguousInsertionBuilder = new StringBuilder(keyboardEventArgs.Key);
							var innerIndex = _index + 1;

							for (; innerIndex < KeyboardEventArgsList.Count; innerIndex++)
							{
								var innerKeyboardEventArgs = KeyboardEventArgsList[innerIndex];

								var innerKeyboardEventArgsKind = EventUtils.GetKeyboardEventArgsKind(
									_events,
									innerKeyboardEventArgs,
									definiteHasSelection,
									_events.TextEditorService,
									out _);

								if (innerKeyboardEventArgsKind == KeyboardEventArgsKind.Text)
								{
									contiguousInsertionBuilder.Append(innerKeyboardEventArgs.Key);
									_index++;
								}
								else
								{
									break;
								}
							}

							modelModifier.Insert(
			                    contiguousInsertionBuilder.ToString(),
			                    cursorModifierBag,
			                    cancellationToken: CancellationToken.None);
						}
						else
						{
							if (KeyboardKeyFacts.IsMetaKey(keyboardEventArgs))
			                {
								// Batch contiguous backspace or delete events
								var eventCounter = 1;
								var innerIndex = _index + 1;
	
								for (; innerIndex < KeyboardEventArgsList.Count; innerIndex++)
								{
									var innerKeyboardEventArgs = KeyboardEventArgsList[innerIndex];
	
									var innerKeyboardEventArgsKind = EventUtils.GetKeyboardEventArgsKind(
										_events,
										innerKeyboardEventArgs,
										definiteHasSelection,
										_events.TextEditorService,
										out _);
	
									// If the user has a text selection, one cannot batch here.
									//
									// CtrlKey does not work here as of this comment (2024-05-27).
									// It should delete-word eventCounter times, but instead it is
									// doing delete-word 1 time regardless of the counter, and
									// eats any events attempted to batch as if they never fired.
									if (!definiteHasSelection && !keyboardEventArgs.CtrlKey && KeyAndModifiersAreEqual(keyboardEventArgs, innerKeyboardEventArgs))
									{
										eventCounter++;
										_index++;
									}
									else
									{
										break;
									}
								}

			                    if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventArgs.Key)
			                    {
			                        modelModifier.Delete(
			                            cursorModifierBag,
			                            eventCounter,
			                            keyboardEventArgs.CtrlKey,
			                            TextEditorModelModifier.DeleteKind.Backspace,
			                            CancellationToken.None);
			                    }
			                    else if (KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventArgs.Key)
			                    {
			                        modelModifier.Delete(
			                            cursorModifierBag,
			                            eventCounter,
			                            keyboardEventArgs.CtrlKey,
			                            TextEditorModelModifier.DeleteKind.Delete,
			                            CancellationToken.None);
			                    }
			                }
							else
							{
								await _events.TextEditorService.ModelApi.HandleKeyboardEventFactory(
				                        ResourceUri,
				                        ViewModelKey,
				                        keyboardEventArgs,
				                        CancellationToken.None)
				                    .Invoke(EditContext)
				                    .ConfigureAwait(false);
							}
						}
	                    break;
	            }
	
	            if (shouldInvokeAfterOnKeyDownAsync)
	            {
	                if (command is null ||
	                    command is TextEditorCommand commandTextEditor && commandTextEditor.ShouldScrollCursorIntoView)
	                {
	                    viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = true;
	                }
	
	                var cursorDisplay = _events.CursorDisplay;
	
	                if (cursorDisplay is not null)
	                {
						if (_viewModelDisplay.ViewModelDisplayOptions.AfterOnKeyDownAsyncFactory is not null)
				        {
				            await ViewModelDisplayOptions.AfterOnKeyDownAsyncFactory.Invoke(
					                resourceUri,
					                viewModelKey,
					                keyboardEventArgs,
					                cursorDisplay.SetShouldDisplayMenuAsync)
								.Invoke(EditContext)
		                        .ConfigureAwait(false);
;
				        }
						else
						{
							await TextEditorCommandDefaultFunctions.HandleAfterOnKeyDownAsyncFactory(
		                            modelModifier.ResourceUri,
		                            viewModelModifier.ViewModel.ViewModelKey,
		                            keyboardEventArgs,
		                            cursorDisplay.SetShouldDisplayMenuAsync)
		                        .Invoke(EditContext)
		                        .ConfigureAwait(false);
						}
	                }
	            }
			}
		}
		finally
		{
			await EditContext.TextEditorService.FinalizePost(EditContext);
		}
    }

    private bool KeyAndModifiersAreEqual(KeyboardEventArgs x, KeyboardEventArgs y)
    {
        return
            x.Key == y.Key &&
            x.ShiftKey == y.ShiftKey &&
            x.CtrlKey == y.CtrlKey &&
            x.AltKey == y.AltKey;
    }
}
