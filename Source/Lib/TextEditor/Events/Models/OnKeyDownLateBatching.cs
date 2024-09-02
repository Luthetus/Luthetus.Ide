using System.Text;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
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
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

/// <summary>
/// Use an array with a fixed size to optimize this, usually there are only about 3-5 events that end up batched.
/// To construct a non-struct collection is probably a massive performance hit considering the extremely rapid
/// turn over of this type while holding down a keyboard key.
/// </summary>
public struct OnKeyDownLateBatching : ITextEditorWork
{
	public const int MAX_BATCH_SIZE = 8;
	public const string NAME_ZERO = $"{nameof(OnKeyDownLateBatching)}_0";
	public const string NAME_ONE = $"{nameof(OnKeyDownLateBatching)}_1";
	public const string NAME_TWO = $"{nameof(OnKeyDownLateBatching)}_2";
	public const string NAME_THREE = $"{nameof(OnKeyDownLateBatching)}_3";
	public const string NAME_FOUR = $"{nameof(OnKeyDownLateBatching)}_4";
	public const string NAME_FIVE = $"{nameof(OnKeyDownLateBatching)}_5";
	public const string NAME_SIX = $"{nameof(OnKeyDownLateBatching)}_6";
	public const string NAME_SEVEN = $"{nameof(OnKeyDownLateBatching)}_7";
	public const string NAME_EIGHT = $"{nameof(OnKeyDownLateBatching)}_8";
	public const string NAME_DEFAULT = nameof(OnKeyDownLateBatching);

	public OnKeyDownLateBatching(
		TextEditorComponentData componentData,
	    KeymapArgs keymapArgs,
	    ResourceUri resourceUri,
	    Key<TextEditorViewModel> viewModelKey)
    {
        ComponentData = componentData;

		AddToBatch(keymapArgs);

        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
	public KeymapArgs[] KeymapArgsList { get; } = new KeymapArgs[MAX_BATCH_SIZE];
	public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
	public ITextEditorEditContext EditContext { get; set; }
	public TextEditorComponentData ComponentData { get; set; }
	public int BatchLength { get; set; }
	public bool BatchHasAvailability => BatchLength < MAX_BATCH_SIZE;

    public string Name => BatchLength switch
    {
		0 => NAME_ZERO,
        1 => NAME_ONE,
        2 => NAME_TWO,
		3 => NAME_THREE,
		4 => NAME_FOUR,
		5 => NAME_FIVE,
		6 => NAME_SIX,
		7 => NAME_SEVEN,
		8 => NAME_EIGHT,
		_ => NAME_DEFAULT
    };

	/// <summary>
	/// Global variable used during <see cref="HandleEvent"/> to
	/// iterate over <see cref="KeymapArgsList"/>
	/// </summary>
	private int _index;

	public void AddToBatch(KeymapArgs keymapArgs)
	{
		if (!BatchHasAvailability)
			throw new LuthetusTextEditorException($"{nameof(BatchLength)} >= {nameof(MAX_BATCH_SIZE)}");

		KeymapArgsList[BatchLength] = keymapArgs;
        BatchLength++;
    }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask upstreamEvent)
    {
		if (upstreamEvent is OnKeyDownLateBatching upstreamOnKeyDownLateBatching)
		{
			if (BatchLength == 1 && upstreamOnKeyDownLateBatching.BatchHasAvailability)
				upstreamOnKeyDownLateBatching.AddToBatch(KeymapArgsList[0]);

            return upstreamOnKeyDownLateBatching;
		}

		return null;
    }

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
		try
		{
            var modelModifier = EditContext.GetModelModifier(ResourceUri);
            var viewModelModifier = EditContext.GetViewModelModifier(ViewModelKey);
            var cursorModifierBag = EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return;

			_index = 0;

			for (; _index < BatchLength; _index++)
			{
				var keymapArgs = KeymapArgsList[_index];

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

							for (; innerIndex < BatchLength; innerIndex++)
							{
								var innerKeyboardEventArgs = KeymapArgsList[innerIndex];

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
	
								for (; innerIndex < BatchLength; innerIndex++)
								{
									var innerKeymapArgs = KeymapArgsList[innerIndex];
	
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
			
			if (modelModifier.LineCount < modelModifier.PreviousLineCount)
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
			}
			
			await EditContext.TextEditorService
				.FinalizePost(EditContext)
				.ConfigureAwait(false);
				
			await Task.Delay(ThrottleFacts.TwentyFour_Frames_Per_Second).ConfigureAwait(false);
		}
		catch (Exception e)
		{
			// It was found to be the case (2024-08-13)
			// ========================================
			// that if an exception is thrown after the partitions were modified,
			// that the "only 'FinalizePost' if there were no exceptions" idea is not sufficient.
			// Because the partitions are shared between recreations of the text editor model,
			// and the corrupt state will be spread.
			//
			// So, a follow up idea is that inside the catch, if there is a 'modified' flag on a model,
			// then presume that the partitions are corrupted?
			//
			// Could one then recover from this state?
			Console.WriteLine(e);
		}
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
