using System.Text;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

/// <summary>
/// Allocating an array 'public KeymapArgs[] KeymapArgsList { get; } = new KeymapArgs[MAX_BATCH_SIZE];'
/// for every 'OnKeyDownLateBatching' instance, sounds like a bad idea.
///
/// The IBackgroundTaskQueue should have allocated at all times,
/// an array of objects.
///
/// Then this type can add to that always allocated array.
///
/// There would need to be a way to identify
/// what type each object is.
///
/// If many 'OnKeyDownLateBatching' events are coming in
/// and they build up in the queue.
///
/// Then a 'string BatchTag' could be set to 'luth_OnKeyDownLateBatching_KeymapArgs'.
///
/// This way a new 'OnKeyDownLateBatching' can see that there is a batch being built
/// of similar events, and add its 'KeymapArgs' to the 'object[]'.
///
/// This manipulation of the 'object[]'
/// can likely be done from within 'IBackgroundTask.BatchOrDefault(IBackgroundTask upstreamEvent)'
/// in order to ensure thread safety.
///
/// If it is decided to go with a 'List<object>',
/// consider if it is possible that a batch of events
/// results in a list of extremely high capacity.
///
/// Just for on average the capacity is low.
/// (i.e.: some outlier batch sizes cause high allocation for the List
///        just for that capacity to go un-used in the future).
///
/// You don't need to make the List until dequeueing
/// and you find that the "next up" is the same name.
/// 
/// Optimistic vs Pessimistic batching?
/// Early vs Late batching?
/// </summary>
public struct OnKeyDownLateBatching : ITextEditorWork
{
	public OnKeyDownLateBatching(
		TextEditorComponentData componentData,
	    KeymapArgs keymapArgs,
	    ResourceUri resourceUri,
	    Key<TextEditorViewModel> viewModelKey)
    {
        ComponentData = componentData;

		KeymapArgs = keymapArgs;

        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public Key<IBackgroundTask> BackgroundTaskKey => Key<IBackgroundTask>.Empty;
    public Key<IBackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public bool EarlyBatchEnabled { get; set; }
    public bool LateBatchEnabled { get; set; }
    public KeymapArgs KeymapArgs { get; set; }
	public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
	public ITextEditorEditContext? EditContext { get; private set; }
	public TextEditorComponentData ComponentData { get; set; }

    // TODO: I'm uncomfortable as to whether "luth_{nameof(Abc123)}" is a constant interpolated string so I'm just gonna hardcode it.
    public string Name => "luth_OnKeyDownLateBatching";

	/// <summary>
	/// Global variable used during <see cref="HandleEvent"/> to
	/// iterate over <see cref="KeymapArgsList"/>
	/// </summary>
	private int _index;

	/*public void AddToBatch(KeymapArgs keymapArgs)
	{
		if (!BatchHasAvailability)
			throw new LuthetusTextEditorException($"{nameof(BatchLength)} >= {nameof(MAX_BATCH_SIZE)}");

		KeymapArgsList[BatchLength] = keymapArgs;
        BatchLength++;
    }*/

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask upstreamEvent)
    {
    	/*
    	// TODO: What is the overhead of this 'is' cast versus something like an enum, or string?
		if (upstreamEvent is OnKeyDownLateBatching upstreamOnKeyDownLateBatching)
		{
			if (BatchLength == 1 && upstreamOnKeyDownLateBatching.BatchHasAvailability)
				upstreamOnKeyDownLateBatching.AddToBatch(KeymapArgsList[0]);

            return upstreamOnKeyDownLateBatching;
		}
		*/
		
		return null;
    }
    
    public IBackgroundTask? LateBatchOrDefault(IBackgroundTask oldEvent)
    {
    	return null;
    }

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
    	EditContext = new TextEditorService.TextEditorEditContext(
            ComponentData.TextEditorViewModelDisplay.TextEditorService,
            TextEditorService.AuthenticatedActionKey);

        var modelModifier = EditContext.GetModelModifier(ResourceUri);
        var viewModelModifier = EditContext.GetViewModelModifier(ViewModelKey);
        var cursorModifierBag = EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
        var primaryCursorModifier = EditContext.GetPrimaryCursorModifier(cursorModifierBag);

        if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
            return;

		_index = 0;

		for (; _index < 1; _index++)
		{
			var keymapArgs = KeymapArgs;

            var definiteHasSelection = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

            var definiteKeyboardEventArgsKind = EventUtils.GetKeymapArgsKind(
                ComponentData,
				keymapArgs,
				definiteHasSelection,
				EditContext.TextEditorService,
				out var command);

            var shouldInvokeAfterOnKeyDownAsync = false;

            switch (definiteKeyboardEventArgsKind)
            {
                case KeymapArgsKind.Command:
                    shouldInvokeAfterOnKeyDownAsync = true;

					var commandArgs = new TextEditorCommandArgs(
                        modelModifier.ResourceUri,
                        viewModelModifier.ViewModel.ViewModelKey,
						ComponentData,
						EditContext.TextEditorService,
						ComponentData.ServiceProvider,
						EditContext);

                    await command.CommandFunc
                        .Invoke(commandArgs)
                        .ConfigureAwait(false);
                    break;
                case KeymapArgsKind.Movement:
                    if ((KeyboardKeyFacts.MovementKeys.ARROW_DOWN == keymapArgs.Key || KeyboardKeyFacts.MovementKeys.ARROW_UP == keymapArgs.Key) &&
                        viewModelModifier.ViewModel.MenuKind == MenuKind.AutoCompleteMenu)
                    {
                    	// TODO: Focusing the menu from here isn't working?
                    	await EditContext.TextEditorService.JsRuntimeCommonApi.FocusHtmlElementById(
                    		AutocompleteMenu.HTML_ELEMENT_ID,
                    		preventScroll: true);
                    		
                    	ComponentData.MenuShouldTakeFocus = true;
                    }
                    else
                    {
                        EditContext.TextEditorService.ViewModelApi.MoveCursor(
                    		keymapArgs,
					        EditContext,
					        modelModifier,
					        viewModelModifier,
					        cursorModifierBag);
					        
					    if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
					    {
					    	TextEditorCommandDefaultFunctions.RemoveDropdown(
						        EditContext,
						        viewModelModifier,
						        ComponentData.Dispatcher);
					    }
                    }
                    break;
                case KeymapArgsKind.ContextMenu:
                	TextEditorCommandDefaultFunctions.ShowContextMenu(
				        EditContext,
				        modelModifier,
				        viewModelModifier,
				        cursorModifierBag,
				        primaryCursorModifier,
				        ComponentData.Dispatcher,
				        ComponentData);
                    break;
                case KeymapArgsKind.Text:
                case KeymapArgsKind.Other:
                    shouldInvokeAfterOnKeyDownAsync = true;

                    if (!EventUtils.IsAutocompleteMenuInvoker(keymapArgs))
                    {
                        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keymapArgs.Key ||
                            KeyboardKeyFacts.MetaKeys.BACKSPACE == keymapArgs.Key ||
                            KeyboardKeyFacts.MetaKeys.DELETE == keymapArgs.Key ||
                            !KeyboardKeyFacts.IsMetaKey(keymapArgs))
                        {
                        	if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
                        	{
								TextEditorCommandDefaultFunctions.RemoveDropdown(
							        EditContext,
							        viewModelModifier,
							        ComponentData.Dispatcher);
							}
                        }
                    }

					viewModelModifier.ViewModel = viewModelModifier.ViewModel with
					{
						TooltipViewModel = null
					};

					if (definiteKeyboardEventArgsKind == KeymapArgsKind.Text)
					{
						// Batch contiguous insertions
						var contiguousInsertionBuilder = new StringBuilder(keymapArgs.Key);
						var innerIndex = _index + 1;

						for (; innerIndex < 0; innerIndex++)
						{
							var innerKeyboardEventArgs = KeymapArgs;//List[innerIndex];

							var innerKeyboardEventArgsKind = EventUtils.GetKeymapArgsKind(
								ComponentData,
								innerKeyboardEventArgs,
								definiteHasSelection,
								EditContext.TextEditorService,
								out _);

							if (innerKeyboardEventArgsKind == KeymapArgsKind.Text)
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
						if (KeyboardKeyFacts.IsMetaKey(keymapArgs))
		                {
							// Batch contiguous backspace or delete events
							var eventCounter = 1;
							var innerIndex = _index + 1;

							for (; innerIndex < 1; innerIndex++)
							{
								var innerKeymapArgs = KeymapArgs;//List[innerIndex];

								var innerKeymapArgsKind = EventUtils.GetKeymapArgsKind(
									ComponentData,
									innerKeymapArgs,
									definiteHasSelection,
									EditContext.TextEditorService,
									out _);

								// If the user has a text selection, one cannot batch here.
								//
								// CtrlKey does not work here as of this comment (2024-05-27).
								// It should delete-word eventCounter times, but instead it is
								// doing delete-word 1 time regardless of the counter, and
								// eats any events attempted to batch as if they never fired.
								if (!definiteHasSelection && !keymapArgs.CtrlKey && KeyAndModifiersAreEqual(keymapArgs, innerKeymapArgs))
								{
									eventCounter++;
									_index++;
								}
								else
								{
									break;
								}
							}

		                    if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keymapArgs.Key)
		                    {
		                        modelModifier.Delete(
		                            cursorModifierBag,
		                            eventCounter,
		                            keymapArgs.CtrlKey,
		                            TextEditorModelModifier.DeleteKind.Backspace,
		                            CancellationToken.None);
		                    }
		                    else if (KeyboardKeyFacts.MetaKeys.DELETE == keymapArgs.Key)
		                    {
		                        modelModifier.Delete(
		                            cursorModifierBag,
		                            eventCounter,
		                            keymapArgs.CtrlKey,
		                            TextEditorModelModifier.DeleteKind.Delete,
		                            CancellationToken.None);
		                    }
		                }
						else
						{
							EditContext.TextEditorService.ModelApi.HandleKeyboardEvent(
								EditContext,
						        modelModifier,
						        cursorModifierBag,
						        keymapArgs,
						        CancellationToken.None);
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

				if (ComponentData.ViewModelDisplayOptions.AfterOnKeyDownAsync is not null)
		        {
		            await ComponentData.ViewModelDisplayOptions.AfterOnKeyDownAsync.Invoke(
			                EditContext,
					        modelModifier,
					        viewModelModifier,
					        cursorModifierBag,
					        keymapArgs,
							ComponentData)
                        .ConfigureAwait(false);
		        }
				else
				{
					await TextEditorCommandDefaultFunctions.HandleAfterOnKeyDownAsync(
							EditContext,
					        modelModifier,
					        viewModelModifier,
					        cursorModifierBag,
					        keymapArgs,
							ComponentData)
                        .ConfigureAwait(false);
				}
            }
		}
		
		// TODO: Do this code first so the user gets immediate UI feedback in the event that
		//       their keydown code takes a long time?
		EditContext.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
		
		// This code is wrong.
		// It isn't about the line count or the "most characters on a single line"
		// It simply is whether the maximum scrollLeft or scrollTop has been reduced.
		// In which case you match the new and smaller maximum.
		{
			/*if (modelModifier.LineCount < modelModifier.PreviousLineCount)
			{
				var difference = modelModifier.PreviousLineCount - modelModifier.LineCount;
			
				EditContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
		            EditContext,
			        viewModelModifier,
			        -1 * difference * viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight);
			}
			
			if (modelModifier.MostCharactersOnASingleLineTuple.lineLength < modelModifier.PreviousMostCharactersOnASingleLineTuple.lineLength)
			{
				var difference = modelModifier.PreviousMostCharactersOnASingleLineTuple.lineLength - modelModifier.MostCharactersOnASingleLineTuple.lineLength;
				
				EditContext.TextEditorService.ViewModelApi.MutateScrollHorizontalPosition(
		            EditContext,
			        viewModelModifier,
			        -1 * viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth);
			}*/
		}
		
		await EditContext.TextEditorService
			.FinalizePost(EditContext)
			.ConfigureAwait(false);
			
		// await Task.Delay(ThrottleFacts.TwentyFour_Frames_Per_Second).ConfigureAwait(false);
    }

    private bool KeyAndModifiersAreEqual(KeymapArgs x, KeymapArgs y)
    {
        return
            x.Key == y.Key &&
            x.ShiftKey == y.ShiftKey &&
            x.CtrlKey == y.CtrlKey &&
            x.AltKey == y.AltKey;
    }
}
