using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

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
public struct OnKeyDownLateBatching
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
        
        #if DEBUG
        LuthetusDebugSomething.OnKeyDownLateBatchingCountSent++;
        #endif
    }

    public KeymapArgs KeymapArgs { get; set; }
	public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
	public TextEditorComponentData ComponentData { get; set; }
	
	/// <summary>
	/// Global variable used during <see cref="HandleEvent"/> to
	/// iterate over <see cref="KeymapArgsList"/>
	/// </summary>
	private int _index;

	/// <summary>
	/// CONFUSING: '0 == 0' used to be 'BatchLength == 0'. The batching code is being removed but is a bit of a mess at the moment.
	/// </summary>
    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	#if DEBUG
    	LuthetusDebugSomething.OnKeyDownLateBatchingCountHandled++;
    	#endif
    
    	var editContext = new TextEditorEditContext(ComponentData.TextEditorViewModelDisplay.TextEditorService);

        var modelModifier = editContext.GetModelModifier(ResourceUri);
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

        if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
            return;

		_index = 0;
		
		var batchLengthFake = 0;
		
		if (0 == 0)
			batchLengthFake = 1;

		for (; _index < batchLengthFake; _index++)
		{
			KeymapArgs keymapArgs;
			
			if (0 == 0)
				keymapArgs = KeymapArgs;

            var definiteHasSelection = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

            var definiteKeyboardEventArgsKind = EventUtils.GetKeymapArgsKind(
                ComponentData,
				keymapArgs,
				definiteHasSelection,
				editContext.TextEditorService,
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
						editContext.TextEditorService,
						ComponentData.ServiceProvider,
						editContext);

                    await command.CommandFunc
                        .Invoke(commandArgs)
                        .ConfigureAwait(false);
                    break;
                case KeymapArgsKind.Movement:
                    if ((KeyboardKeyFacts.MovementKeys.ARROW_DOWN == keymapArgs.Key || KeyboardKeyFacts.MovementKeys.ARROW_UP == keymapArgs.Key) &&
                        viewModelModifier.ViewModel.MenuKind == MenuKind.AutoCompleteMenu)
                    {
                    	// TODO: Focusing the menu from here isn't working?
                    	await editContext.TextEditorService.CommonApi.LuthetusCommonJavaScriptInteropApi.FocusHtmlElementById(
                    		AutocompleteMenu.HTML_ELEMENT_ID,
                    		preventScroll: true);
                    		
                    	ComponentData.MenuShouldTakeFocus = true;
                    }
                    else
                    {
                        editContext.TextEditorService.ViewModelApi.MoveCursor(
                    		keymapArgs,
					        editContext,
					        modelModifier,
					        viewModelModifier,
					        cursorModifierBag);
					        
					    if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
					    {
					    	TextEditorCommandDefaultFunctions.RemoveDropdown(
						        editContext,
						        viewModelModifier,
						        ComponentData.DropdownService);
					    }
                    }
                    break;
                case KeymapArgsKind.ContextMenu:
                	TextEditorCommandDefaultFunctions.ShowContextMenu(
				        editContext,
				        modelModifier,
				        viewModelModifier,
				        cursorModifierBag,
				        primaryCursorModifier,
				        ComponentData.DropdownService,
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
							        editContext,
							        viewModelModifier,
							        ComponentData.DropdownService);
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
						// var contiguousInsertionBuilder = new StringBuilder(keymapArgs.Key);
						// var innerIndex = _index + 1;

						/*for (; innerIndex < 0; innerIndex++)
						{
							var innerKeyboardEventArgs = KeymapArgsList[innerIndex];

							var innerKeyboardEventArgsKind = EventUtils.GetKeymapArgsKind(
								ComponentData,
								innerKeyboardEventArgs,
								definiteHasSelection,
								editContext.TextEditorService,
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
						}*/

						modelModifier.Insert(
		                    keymapArgs.Key,
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

							/*for (; innerIndex < 0; innerIndex++)
							{
								var innerKeymapArgs = KeymapArgsList[innerIndex];

								var innerKeymapArgsKind = EventUtils.GetKeymapArgsKind(
									ComponentData,
									innerKeymapArgs,
									definiteHasSelection,
									editContext.TextEditorService,
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
							}*/

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
							editContext.TextEditorService.ModelApi.HandleKeyboardEvent(
								editContext,
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
			                editContext,
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
							editContext,
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
		editContext.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
		
		// This code is wrong.
		// It isn't about the line count or the "most characters on a single line"
		// It simply is whether the maximum scrollLeft or scrollTop has been reduced.
		// In which case you match the new and smaller maximum.
		{
			/*if (modelModifier.LineCount < modelModifier.PreviousLineCount)
			{
				var difference = modelModifier.PreviousLineCount - modelModifier.LineCount;
			
				editContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
		            editContext,
			        viewModelModifier,
			        -1 * difference * viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight);
			}
			
			if (modelModifier.MostCharactersOnASingleLineTuple.lineLength < modelModifier.PreviousMostCharactersOnASingleLineTuple.lineLength)
			{
				var difference = modelModifier.PreviousMostCharactersOnASingleLineTuple.lineLength - modelModifier.MostCharactersOnASingleLineTuple.lineLength;
				
				editContext.TextEditorService.ViewModelApi.MutateScrollHorizontalPosition(
		            editContext,
			        viewModelModifier,
			        -1 * viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth);
			}*/
		}
		
		await editContext.TextEditorService
			.FinalizePost(editContext)
			.ConfigureAwait(false);
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
