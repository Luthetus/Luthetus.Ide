using System.Text;
using Luthetus.TextEditor.RazorLib.Rows.Models;
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
	private readonly StringBuilder _indentationBuilder = new();

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
			    	break;
	    	}
		}
		else if (onKeyDown.KeymapArgs.CtrlKey && onKeyDown.KeymapArgs.AltKey)
		{
			switch (onKeyDown.KeymapArgs.Code)
			{
				default:
			    	break;
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
		            break;
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
		            
		            break;
		        case "KeyC":
		            await TextEditorCommandDefaultFunctions.CopyAsync(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag,
		                onKeyDown.ComponentData.ClipboardService);
		            break;
		        case "KeyV":
		            await TextEditorCommandDefaultFunctions.PasteAsync(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag,
		                onKeyDown.ComponentData.ClipboardService);
		            break;
		        case "KeyX":
		            await TextEditorCommandDefaultFunctions.CutAsync(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag,
		                onKeyDown.ComponentData.ClipboardService);
		            break;
		        case "KeyA":
		            TextEditorCommandDefaultFunctions.SelectAll(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            break;
		        case "Keyz":
		            TextEditorCommandDefaultFunctions.Undo(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            break;
		        case "KeyY":
		            TextEditorCommandDefaultFunctions.Redo(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            break;
		        case "KeyD":
		            TextEditorCommandDefaultFunctions.Duplicate(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            break;
		        case "ArrowDown":
		            TextEditorCommandDefaultFunctions.ScrollLineDown(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            break;
		        case "ArrowUp":
		            TextEditorCommandDefaultFunctions.ScrollLineUp(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            break;
		        case "PageDown":
					TextEditorCommandDefaultFunctions.CursorMovePageBottom(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            break;
		        case "PageUp":
					TextEditorCommandDefaultFunctions.CursorMovePageTop(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
		            break;
		        case "Slash":
					await TextEditorCommandDefaultFunctions.ShowTooltipByCursorPositionAsync(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag,
		                onKeyDown.ComponentData.TextEditorService,
		                onKeyDown.ComponentData,
		                onKeyDown.ComponentData.TextEditorComponentRenderers);
		            break;
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
		            
		            break;
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

	                break;
	            case "Backspace":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModelModifier.DeleteKind.Backspace,
	                    CancellationToken.None);
					break;
				case "Delete":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModelModifier.DeleteKind.Delete,
	                    CancellationToken.None);
	                break;
	            default:
		    		break;
		    }
		}
		else if (onKeyDown.KeymapArgs.AltKey)
		{
			switch (onKeyDown.KeymapArgs.Code)
			{
				default:
			    	break;
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
		            break;
		        case "PageUp":
					TextEditorCommandDefaultFunctions.ScrollPageUp(
		                editContext,
		                modelModifier,
		                viewModelModifier,
		                cursorModifierBag);
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
					    
					    break;
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
					    break;
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
				    break;
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
						break;
					}
					break;
				case "Escape":
					if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
                	{
						TextEditorCommandDefaultFunctions.RemoveDropdown(
					        editContext,
					        viewModelModifier,
					        onKeyDown.ComponentData.DropdownService);
					    break;
					}
					break;
				case "Backspace":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModelModifier.DeleteKind.Backspace,
	                    CancellationToken.None);
					break;
				case "Delete":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModelModifier.DeleteKind.Delete,
	                    CancellationToken.None);
					break;
				case "Enter":
					var valueToInsert = modelModifier.LineEndKindPreference.AsCharacters();
			
					// Match indentation on newline keystroke
					var line = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex);
		
					var cursorPositionIndex = line.StartPositionIndexInclusive + primaryCursorModifier.ColumnIndex;
					var indentationPositionIndex = line.StartPositionIndexInclusive;
		
					_indentationBuilder.Clear();
					
					while (indentationPositionIndex < cursorPositionIndex)
					{
						var possibleIndentationChar = modelModifier.RichCharacterList[indentationPositionIndex++].Value;
		
						if (possibleIndentationChar == '\t' || possibleIndentationChar == ' ')
							_indentationBuilder.Append(possibleIndentationChar);
						else
							break;
					}
		
					valueToInsert += _indentationBuilder.ToString();
					
					modelModifier.Insert(
			            valueToInsert,
			            cursorModifierBag,
			            cancellationToken: CancellationToken.None);
			            
	                break;
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
			        	
			        	break;
		        	}
					else
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
			            	modelModifier.Insert(
			                    "\t",
			                    cursorModifierBag,
			                    cancellationToken: CancellationToken.None);
			            }
		            }
	                break;
				case "Space":
	            	modelModifier.Insert(
	                    " ",
	                    cursorModifierBag,
	                    cancellationToken: CancellationToken.None);
	                break;
				case "Backquote":
				case "Digit0":
				case "Digit1":
				case "Digit2":
				case "Digit3":
				case "Digit4":
				case "Digit5":
				case "Digit6":
				case "Digit7":
				case "Digit8":
				case "Digit9":
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
	                break;
			}
		}
		
		// TODO: Do this code first so the user gets immediate UI feedback in the event that
		//       their keydown code takes a long time?
		editContext.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
		
		await editContext.TextEditorService
			.FinalizePost(editContext)
			.ConfigureAwait(false);
    }
}
