using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;

public class TextEditorKeymapDefault : ITextEditorKeymap
{
    public string DisplayName { get; } = nameof(TextEditorKeymapDefault);

    public Key<KeymapLayer> GetLayer(bool hasSelection)
    {
        return hasSelection
            ? TextEditorKeymapDefaultFacts.HasSelectionLayer.Key
            : TextEditorKeymapDefaultFacts.DefaultLayer.Key;
    }

    public string GetCursorCssClassString()
    {
        return TextCursorKindFacts.BeamCssClassString;
    }

    public string GetCursorCssStyleString(
        TextEditorModel textEditorModel,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions)
    {
        return string.Empty;
    }

    public async ValueTask HandleEvent(OnKeyDown onKeyDown)
    {
    	var editContext = new TextEditorEditContext(onKeyDown.ComponentData.TextEditorViewModelDisplay.TextEditorService);

        var modelModifier = editContext.GetModelModifier(onKeyDown.ResourceUri);
        var viewModelModifier = editContext.GetViewModelModifier(onKeyDown.ViewModelKey);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

        if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
            return;

		if (onKeyDown.KeymapArgs.MetaKey)
		{
			switch (onKeyDown.KeymapArgs.Code)
			{
				default:
			    	goto finalize;
	    	}
		}
		else if (onKeyDown.KeymapArgs.CtrlKey && onKeyDown.KeymapArgs.AltKey)
		{
			switch (onKeyDown.KeymapArgs.Code)
			{
				default:
			    	goto finalize;
	    	}
		}
		else if (onKeyDown.KeymapArgs.CtrlKey)
		{
		    switch (onKeyDown.KeymapArgs.Code)
		    {
		    	case "KeyR":
		    		modelModifier.CompilerService.ResourceWasModified(
		                modelModifier.ResourceUri,
		                Array.Empty<TextEditorTextSpan>());
		            TextEditorCommandDefaultFunctions.TriggerRemeasure(
		                editContext,
		                viewModelModifier);
		            goto finalize;
		    	case "KeyS":
		            TextEditorCommandDefaultFunctions.TriggerSave(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag,
		                onKeyDown.ComponentData.CommonComponentRenderers,
		                onKeyDown.ComponentData.NotificationService);
		            
		            modelModifier.CompilerService.ResourceWasModified(
		                modelModifier.ResourceUri,
		                Array.Empty<TextEditorTextSpan>());
		            
		            goto finalize;
		        case "KeyC":
		            await TextEditorCommandDefaultFunctions.CopyAsync(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag,
		                onKeyDown.ComponentData.ClipboardService);
		            goto finalize;
		        case "KeyV":
		            await TextEditorCommandDefaultFunctions.PasteAsync(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag,
		                onKeyDown.ComponentData.ClipboardService);
		            goto finalize;
		        case "KeyX":
		            await TextEditorCommandDefaultFunctions.CutAsync(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag,
		                onKeyDown.ComponentData.ClipboardService);
		            goto finalize;
		        case "KeyA":
		            TextEditorCommandDefaultFunctions.SelectAll(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            goto finalize;
		        case "Keyz":
		            TextEditorCommandDefaultFunctions.Undo(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            goto finalize;
		        case "KeyY":
		            TextEditorCommandDefaultFunctions.Redo(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            goto finalize;
		        case "KeyD":
		            TextEditorCommandDefaultFunctions.Duplicate(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            goto finalize;
		        case "ArrowDown":
		            TextEditorCommandDefaultFunctions.ScrollLineDown(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            goto finalize;
		        case "ArrowUp":
		            TextEditorCommandDefaultFunctions.ScrollLineUp(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            goto finalize;
		        case "PageDown":
					TextEditorCommandDefaultFunctions.CursorMovePageBottom(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            goto finalize;
		        case "PageUp":
					TextEditorCommandDefaultFunctions.CursorMovePageTop(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            goto finalize;
		        case "Slash":
					await TextEditorCommandDefaultFunctions.ShowTooltipByCursorPositionAsync(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag,
		                onKeyDown.ComponentData.TextEditorService,
		                onKeyDown.ComponentData,
		                onKeyDown.ComponentData.TextEditorComponentRenderers);
		            goto finalize;
		        case "KeyF":
		        	if (onKeyDown.KeymapArgs.ShiftKey)
		        	{
		        		TextEditorCommandDefaultFunctions.PopulateSearchFindAll(
			                editContext,
			                modelModifier,
			                viewModelModifier,
			                cursorModifierBag,
			                primaryCursorModifier,
			                onKeyDown.ComponentData.FindAllService);
		        	}
		        	else
		        	{
						await TextEditorCommandDefaultFunctions.ShowFindOverlay(
			                editContext,
			                modelModifier,
			                viewModelModifier,
			                cursorModifierBag,
			                primaryCursorModifier,
			                onKeyDown.ComponentData.TextEditorService.JsRuntimeCommonApi);
			        }
		            
		            goto finalize;
		        case "ArrowLeft":
	            case "ArrowRight":
	            case "Home":
	            case "End":
					editContext.TextEditorService.ViewModelApi.MoveCursor(
                		onKeyDown.KeymapArgs,
				        editContext,
				        modelModifier,
				        viewModelModifier,
				        cursorModifierBag);
				        
				    if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
				    {
				    	TextEditorCommandDefaultFunctions.RemoveDropdown(
					        editContext,
					        viewModelModifier,
					        onKeyDown.ComponentData.DropdownService);
				    }

	                goto finalize;
	            case "Backspace":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModelModifier.DeleteKind.Backspace,
	                    CancellationToken.None);
					goto finalize;
				case "Delete":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModelModifier.DeleteKind.Delete,
	                    CancellationToken.None);
	                goto finalize;
	            default:
		    		goto finalize;
		    }
		}
		else if (onKeyDown.KeymapArgs.AltKey)
		{
			switch (onKeyDown.KeymapArgs.Code)
			{
				default:
			    	goto finalize;
	    	}
		}
		else
		{
			switch (onKeyDown.KeymapArgs.Code)
			{
				case "PageDown":
					TextEditorCommandDefaultFunctions.ScrollPageDown(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            goto finalize;
		        case "PageUp":
					TextEditorCommandDefaultFunctions.ScrollPageUp(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            goto finalize;
		        case "Tab":
		        	if (TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier))
		        	{
		        		if (onKeyDown.KeymapArgs.ShiftKey)
			        	{
			        		TextEditorCommandDefaultFunctions.IndentLess(
				                editContext,
				                modelModifier,
				                viewModelModifier,
				                cursorModifierBag);
			        	}
			        	else
			        	{
			        		TextEditorCommandDefaultFunctions.IndentMore(
				                editContext,
				                modelModifier,
				                viewModelModifier,
				                cursorModifierBag);
			        	}
			        	
			        	goto finalize;
		        	}

					break;
				case "ArrowLeft":
	            case "ArrowDown":
	            case "ArrowUp":
	            case "ArrowRight":
	            case "Home":
	            case "End":
	            	if (("ArrowDown" == onKeyDown.KeymapArgs.Code || "ArrowUp" == onKeyDown.KeymapArgs.Code) &&
	                    viewModelModifier.ViewModel.MenuKind == MenuKind.AutoCompleteMenu)
	                {
	                	// TODO: Focusing the menu from here isn't working?
	                	await editContext.TextEditorService.JsRuntimeCommonApi.FocusHtmlElementById(
	                		AutocompleteMenu.HTML_ELEMENT_ID,
	                		preventScroll: true);
	                		
	                	onKeyDown.ComponentData.MenuShouldTakeFocus = true;
	                	
	                	break;
	                }
	                else
	                {
						editContext.TextEditorService.ViewModelApi.MoveCursor(
	                		onKeyDown.KeymapArgs,
					        editContext,
					        modelModifier,
					        viewModelModifier,
					        cursorModifierBag);
					        
					    if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
					    {
					    	TextEditorCommandDefaultFunctions.RemoveDropdown(
						        editContext,
						        viewModelModifier,
						        onKeyDown.ComponentData.DropdownService);
					    }
	
		                goto finalize;
		            }
		        case "F10":
		        	if (onKeyDown.KeymapArgs.ShiftKey)
		        	{
		        		TextEditorCommandDefaultFunctions.ShowContextMenu(
					        editContext,
					        modelModifier,
					        viewModelModifier,
					        cursorModifierBag,
					        primaryCursorModifier,
					        onKeyDown.ComponentData.DropdownService,
					        onKeyDown.ComponentData);
					    goto finalize;
		        	}
		        	break;
		        case "ContextMenu":
		        	TextEditorCommandDefaultFunctions.ShowContextMenu(
				        editContext,
				        modelModifier,
				        viewModelModifier,
				        cursorModifierBag,
				        primaryCursorModifier,
				        onKeyDown.ComponentData.DropdownService,
				        onKeyDown.ComponentData);
				    goto finalize;
				case "CapsLock":
					/*
					On Linux the 'CapsLock' to 'Escape' setting is returning:
						event.code == CapsLock
						event.key == Escape
					*/
					if (onKeyDown.KeymapArgs.Key == "Escape")
					{
						if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
	                	{
							TextEditorCommandDefaultFunctions.RemoveDropdown(
						        editContext,
						        viewModelModifier,
						        onKeyDown.ComponentData.DropdownService);
						}
						goto finalize;
					}
					break;
				case "Escape":
					if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
                	{
						TextEditorCommandDefaultFunctions.RemoveDropdown(
					        editContext,
					        viewModelModifier,
					        onKeyDown.ComponentData.DropdownService);
					    goto finalize;
					}
					break;
				case "Backspace":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModelModifier.DeleteKind.Backspace,
	                    CancellationToken.None);
					goto finalize;
				case "Delete":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModelModifier.DeleteKind.Delete,
	                    CancellationToken.None);
					goto finalize;
				case "Backquote":
				case "Digit1":
				case "Minus":
				case "Equal":
				case "BracketLeft":
				case "BracketRight":
				case "Backslash":
				case "Semicolon":
				case "Quote":
				case "Comma":
				case "Period":
				case "Slash":
	            case "KeyA":
	            case "KeyB":
	            case "KeyC":
	            case "KeyD":
	            case "KeyE":
	            case "KeyF":
	            case "KeyG":
	            case "KeyH":
	            case "KeyI":
	            case "KeyJ":
	            case "KeyK":
	            case "KeyL":
	            case "KeyM":
	            case "KeyN":
	            case "KeyO":
	            case "KeyP":
	            case "KeyQ":
	            case "KeyR":
	            case "KeyS":
	            case "KeyT":
	            case "KeyU":
	            case "KeyV":
	            case "KeyW":
	            case "KeyX":
	            case "KeyY":
	            case "KeyZ":
	            	modelModifier.Insert(
	                    onKeyDown.KeymapArgs.Key,
	                    cursorModifierBag,
	                    cancellationToken: CancellationToken.None);
	                goto finalize;
			}
		}

		var keymapArgs = onKeyDown.KeymapArgs;
		
        var definiteHasSelection = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

        var definiteKeyboardEventArgsKind = EventUtils.GetKeymapArgsKind(
            onKeyDown.ComponentData,
			keymapArgs,
			definiteHasSelection,
			editContext.TextEditorService,
			out var command);

        var shouldInvokeAfterOnKeyDownAsync = false;

        switch (definiteKeyboardEventArgsKind)
        {
            case KeymapArgsKind.Text:
            case KeymapArgsKind.Other:
                shouldInvokeAfterOnKeyDownAsync = true;

				viewModelModifier.ViewModel = viewModelModifier.ViewModel with
				{
					TooltipViewModel = null
				};

				if (definiteKeyboardEventArgsKind == KeymapArgsKind.Text)
				{
					modelModifier.Insert(
	                    keymapArgs.Key,
	                    cursorModifierBag,
	                    cancellationToken: CancellationToken.None);
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
                break;
        }
		
		finalize:
		
		// TODO: Do this code first so the user gets immediate UI feedback in the event that
		//       their keydown code takes a long time?
		editContext.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
		
		await editContext.TextEditorService
			.FinalizePost(editContext)
			.ConfigureAwait(false);
    }
}
