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
    
    public Func<TextEditorEditContext, string?, int?, ValueTask> AltF12Func { get; set; } = (_, _, _) => ValueTask.CompletedTask;
    public Func<TextEditorEditContext, TextEditorModel, TextEditorViewModel, CursorModifierBagTextEditor, ValueTask> ShiftF12Func { get; set; } = (_, _, _, _) => ValueTask.CompletedTask;

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

		// An NRE will be caught by the IBackgroundTaskService so don't bother checking 'viewModel is null'.
        var viewModel = editContext.GetViewModelModifier(onKeyDown.ViewModelKey);
        var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModel);
        var primaryCursorModifier = cursorModifierBag.CursorModifier;

        if (modelModifier is null || viewModel is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
            return;

		var menuKind = MenuKind.None;
		var shouldClearTooltip = false;
		var shouldRevealCursor = false;
		var shouldApplySyntaxHighlighting = false;

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
		    		onKeyDown.ComponentData.ThrottleApplySyntaxHighlighting(modelModifier);
		            TextEditorCommandDefaultFunctions.TriggerRemeasure(
		                editContext,
		                viewModel);
		            break;
		    	case "KeyS":
		            TextEditorCommandDefaultFunctions.TriggerSave(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag,
		                onKeyDown.ComponentData.CommonComponentRenderers,
		                onKeyDown.ComponentData.NotificationService);
		            
		            onKeyDown.ComponentData.ThrottleApplySyntaxHighlighting(modelModifier);
		            
		            break;
		        case "KeyC":
		            await TextEditorCommandDefaultFunctions.CopyAsync(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag,
		                onKeyDown.ComponentData.ClipboardService);
		            break;
		        case "KeyV":
		            await TextEditorCommandDefaultFunctions.PasteAsync(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag,
		                onKeyDown.ComponentData.ClipboardService);
		            shouldRevealCursor = true;
		            shouldApplySyntaxHighlighting = true;
		            break;
		        case "KeyX":
		            await TextEditorCommandDefaultFunctions.CutAsync(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag,
		                onKeyDown.ComponentData.ClipboardService);
		            shouldRevealCursor = true;
		            shouldApplySyntaxHighlighting = true;
		            break;
		        case "KeyA":
		            TextEditorCommandDefaultFunctions.SelectAll(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "KeyZ":
		            TextEditorCommandDefaultFunctions.Undo(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            shouldRevealCursor = true;
		            shouldApplySyntaxHighlighting = true;
		            break;
		        case "KeyY":
		            TextEditorCommandDefaultFunctions.Redo(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            shouldRevealCursor = true;
		            shouldApplySyntaxHighlighting = true;
		            break;
		        case "KeyD":
		            TextEditorCommandDefaultFunctions.Duplicate(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            shouldRevealCursor = true;
		            shouldApplySyntaxHighlighting = true;
		            break;
		        case "ArrowDown":
		            TextEditorCommandDefaultFunctions.ScrollLineDown(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "ArrowUp":
		            TextEditorCommandDefaultFunctions.ScrollLineUp(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "PageDown":
					TextEditorCommandDefaultFunctions.CursorMovePageBottom(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "PageUp":
					TextEditorCommandDefaultFunctions.CursorMovePageTop(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "Slash":
					await TextEditorCommandDefaultFunctions.ShowTooltipByCursorPositionAsync(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag,
		                onKeyDown.ComponentData.TextEditorService,
		                onKeyDown.ComponentData,
		                onKeyDown.ComponentData.TextEditorComponentRenderers);
		            shouldRevealCursor = true;
		            break;
		        case "KeyF":
		        	if (onKeyDown.KeymapArgs.ShiftKey)
		        	{
		        		TextEditorCommandDefaultFunctions.PopulateSearchFindAll(
			                editContext,
			                modelModifier,
			                viewModel,
			                cursorModifierBag,
			                primaryCursorModifier,
			                onKeyDown.ComponentData.FindAllService);
		        	}
		        	else
		        	{
						await TextEditorCommandDefaultFunctions.ShowFindOverlay(
			                editContext,
			                modelModifier,
			                viewModel,
			                cursorModifierBag,
			                primaryCursorModifier,
			                onKeyDown.ComponentData.TextEditorService.JsRuntimeCommonApi);
			        }
		            
		            break;
	            case "KeyH":
		        	if (onKeyDown.KeymapArgs.ShiftKey)
		        	{
		        		/*TextEditorCommandDefaultFunctions.PopulateSearchFindAll(
			                editContext,
			                modelModifier,
			                viewModel,
			                cursorModifierBag,
			                primaryCursorModifier,
			                onKeyDown.ComponentData.FindAllService);*/
		        	}
		        	else
		        	{
		        		viewModel.ShowReplaceButtonInFindOverlay = true;
		        	
						await TextEditorCommandDefaultFunctions.ShowFindOverlay(
			                editContext,
			                modelModifier,
			                viewModel,
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
				        viewModel,
				        cursorModifierBag);
				        
				    if (viewModel.MenuKind != MenuKind.None)
				    {
				    	TextEditorCommandDefaultFunctions.RemoveDropdown(
					        editContext,
					        viewModel,
					        onKeyDown.ComponentData.DropdownService);
				    }

	                break;
	            case "Backspace":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModel.DeleteKind.Backspace,
	                    CancellationToken.None);
	                shouldRevealCursor = true;
					break;
				case "Delete":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModel.DeleteKind.Delete,
	                    CancellationToken.None);
	                shouldRevealCursor = true;
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
		
					var indentationLength = _indentationBuilder.Length;
					valueToInsert = _indentationBuilder.ToString() + valueToInsert;
					
					primaryCursorModifier.SelectionAnchorPositionIndex = -1;
					primaryCursorModifier.LineIndex = primaryCursorModifier.LineIndex;
        			primaryCursorModifier.ColumnIndex = 0;
					
					modelModifier.Insert(
			            valueToInsert,
			            cursorModifierBag,
			            cancellationToken: CancellationToken.None);
			            
			        if (primaryCursorModifier.LineIndex > 1)
			        {
			            primaryCursorModifier.LineIndex--;
			            primaryCursorModifier.ColumnIndex = indentationLength;
			        }
			            
	                shouldRevealCursor = true;
		            shouldApplySyntaxHighlighting = true;
	                break;
	            case "BracketRight":
	            	TextEditorCommandDefaultFunctions.GoToMatchingCharacter(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag,
		                shouldSelectText: onKeyDown.KeymapArgs.ShiftKey);
	            	shouldRevealCursor = true;
	            	break;
	            case "Space":
	            	if (onKeyDown.KeymapArgs.ShiftKey)
	            	{
	            		await modelModifier.CompilerService.ShowCallingSignature(
							editContext,
							modelModifier,
							viewModel,
							modelModifier.GetPositionIndex(primaryCursorModifier),
							onKeyDown.ComponentData,
							onKeyDown.ComponentData.TextEditorComponentRenderers,
					        modelModifier.ResourceUri);
	            	}
	            	else
	            	{
		            	shouldRevealCursor = true;
		            	shouldClearTooltip = true;
		            	menuKind = MenuKind.AutoCompleteMenu;
			            
			            // TODO: Fix 'shouldApplySyntaxHighlighting = true' for "Space"...
			            // ...It is causing the autocomplete menu to lose focus.
			            // shouldApplySyntaxHighlighting = true;
		            }
	            	break;
	            case "Period":
	            	await TextEditorCommandDefaultFunctions.QuickActionsSlashRefactor(
				        editContext,
				        modelModifier,
				        viewModel,
				        cursorModifierBag,
				        onKeyDown.ComponentData.TextEditorService.JsRuntimeCommonApi,
				        onKeyDown.ComponentData.TextEditorService,
				        onKeyDown.ComponentData.DropdownService);
	            	break;
	            default:
		    		break;
		    }
		}
		else if (onKeyDown.KeymapArgs.AltKey)
		{
			switch (onKeyDown.KeymapArgs.Code)
			{
				case "F12":
		        	TextEditorCommandDefaultFunctions.GoToDefinition(
		        		editContext,
				        modelModifier,
				        viewModel,
				        cursorModifierBag,
        				new Category("CodeSearchService"));
			        break;
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
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "PageUp":
					TextEditorCommandDefaultFunctions.ScrollPageUp(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
				case "ArrowLeft":
	            case "ArrowDown":
	            case "ArrowUp":
	            case "ArrowRight":
	            case "Home":
	            case "End":
	            	if (("ArrowDown" == onKeyDown.KeymapArgs.Code || "ArrowUp" == onKeyDown.KeymapArgs.Code) &&
	                    viewModel.MenuKind == MenuKind.AutoCompleteMenu)
	                {
	                	// TODO: Focusing the menu from here isn't working?
	                	await editContext.TextEditorService.JsRuntimeCommonApi.FocusHtmlElementById(
	                		AutocompleteMenu.HTML_ELEMENT_ID,
	                		preventScroll: true);
	                		
	                	onKeyDown.ComponentData.MenuShouldTakeFocus = true;
	                	menuKind = MenuKind.AutoCompleteMenu;
	                	
	                	break;
	                }
	                else
	                {
						editContext.TextEditorService.ViewModelApi.MoveCursor(
	                		onKeyDown.KeymapArgs,
					        editContext,
					        modelModifier,
					        viewModel,
					        cursorModifierBag);
					        
					    if (viewModel.MenuKind != MenuKind.None)
					    {
					    	TextEditorCommandDefaultFunctions.RemoveDropdown(
						        editContext,
						        viewModel,
						        onKeyDown.ComponentData.DropdownService);
					    }
					    
					    break;
		            }
		        case "F12":
		        	if (onKeyDown.KeymapArgs.ShiftKey)
		        	{
		        		await ShiftF12Func.Invoke(
		        			editContext,
		        			modelModifier,
		        			viewModel,
		        			cursorModifierBag);
		        	}
		        	else
		        	{
			        	TextEditorCommandDefaultFunctions.GoToDefinition(
			        		editContext,
					        modelModifier,
					        viewModel,
			        		cursorModifierBag,
	        				new Category("main"));
			        }
			        break;
		        case "F10":
		        	if (onKeyDown.KeymapArgs.ShiftKey)
		        	{
		        		menuKind = MenuKind.ContextMenu;
	                	shouldRevealCursor = true;
                		shouldClearTooltip = true;
					    break;
		        	}
		        	break;
		        case "F7":
		        	await TextEditorCommandDefaultFunctions.RelatedFilesQuickPick(
				        editContext,
				        modelModifier,
				        viewModel,
				        cursorModifierBag,
				        onKeyDown.ComponentData.TextEditorService.JsRuntimeCommonApi,
				        onKeyDown.ComponentData.EnvironmentProvider,
				        onKeyDown.ComponentData.FileSystemProvider,
				        onKeyDown.ComponentData.TextEditorService,
				        onKeyDown.ComponentData.DropdownService);
				    break;
		        case "ContextMenu":
		        	menuKind = MenuKind.ContextMenu;
	                shouldRevealCursor = true;
	                shouldClearTooltip = true;
				    break;
				case "CapsLock":
					/*
					On Linux the 'CapsLock' to 'Escape' setting is returning:
						event.code == CapsLock
						event.key == Escape
					*/
					if (onKeyDown.KeymapArgs.Key == "Escape")
					{
						menuKind = MenuKind.None;
						shouldClearTooltip = true;
						break;
					}
					break;
				case "Escape":
					menuKind = MenuKind.None;
					shouldClearTooltip = true;
					break;
				case "Backspace":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModel.DeleteKind.Backspace,
	                    CancellationToken.None);
	                shouldRevealCursor = true;
	                menuKind = MenuKind.None;
	                shouldClearTooltip = true;
					break;
				case "Delete":
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModel.DeleteKind.Delete,
	                    CancellationToken.None);
					shouldRevealCursor = true;
					menuKind = MenuKind.None;
	                shouldClearTooltip = true;
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
					
					if (onKeyDown.KeymapArgs.ShiftKey)
					{
						primaryCursorModifier.SelectionAnchorPositionIndex = -1;
						primaryCursorModifier.LineIndex = primaryCursorModifier.LineIndex;
    					primaryCursorModifier.ColumnIndex = modelModifier.GetLineLength(primaryCursorModifier.LineIndex);
					}
					
					modelModifier.Insert(
			            valueToInsert,
			            cursorModifierBag,
			            cancellationToken: CancellationToken.None);
			            
	                shouldRevealCursor = true;
	                menuKind = MenuKind.None;
	                shouldClearTooltip = true;
		            shouldApplySyntaxHighlighting = true;
	                break;
				case "Tab":
					if (TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier))
		        	{
		        		if (onKeyDown.KeymapArgs.ShiftKey)
			        	{
			        		TextEditorCommandDefaultFunctions.IndentLess(
				                editContext,
				                modelModifier,
				                viewModel,
				                cursorModifierBag);
			        	}
			        	else
			        	{
			        		TextEditorCommandDefaultFunctions.IndentMore(
				                editContext,
				                modelModifier,
				                viewModel,
				                cursorModifierBag);
			        	}
			        	
			        	shouldRevealCursor = true;
			        	break;
		        	}
					else
					{
						if (onKeyDown.KeymapArgs.ShiftKey)
			        	{
			        		TextEditorCommandDefaultFunctions.IndentLess(
				                editContext,
				                modelModifier,
				                viewModel,
				                cursorModifierBag);
			        	}
			        	else
			        	{
			            	modelModifier.Insert(
			                    "\t",
			                    cursorModifierBag,
			                    cancellationToken: CancellationToken.None);
			                    
			                menuKind = MenuKind.None;
			                shouldClearTooltip = true;
		            		shouldApplySyntaxHighlighting = true;
			            }
			            
			            shouldRevealCursor = true;
		            }
	                break;
				case "Space":
	            	modelModifier.Insert(
	                    " ",
	                    cursorModifierBag,
	                    cancellationToken: CancellationToken.None);
	                    
	                shouldRevealCursor = true;
	                shouldClearTooltip = true;
	                menuKind = MenuKind.None;
		            shouldApplySyntaxHighlighting = true;
	                break;
				case "Backquote":
				case "BracketLeft":
				case "BracketRight":
				case "Backslash":
				case "Semicolon":
				case "Quote":
				case "Comma":
				case "Period":
				case "Slash":
				case "Minus":
				case "Equal":
					modelModifier.Insert(
	                    onKeyDown.KeymapArgs.Key,
	                    cursorModifierBag,
	                    cancellationToken: CancellationToken.None);
	                
	                shouldRevealCursor = true;
	                menuKind = MenuKind.None;
	                shouldClearTooltip = true;
	                break;
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
	                shouldRevealCursor = true;
	                menuKind = MenuKind.AutoCompleteMenu;
	                shouldClearTooltip = true;
	                break;
			}
		}
		
		if (viewModel.MenuKind != menuKind)
		{
			switch (menuKind)
			{
				case MenuKind.None:
					TextEditorCommandDefaultFunctions.RemoveDropdown(
				        editContext,
				        viewModel,
			        	onKeyDown.ComponentData.DropdownService);
			        break;
				case MenuKind.ContextMenu:
					TextEditorCommandDefaultFunctions.ShowContextMenu(
				        editContext,
				        modelModifier,
				        viewModel,
				        cursorModifierBag,
				        primaryCursorModifier,
				        onKeyDown.ComponentData.DropdownService,
				        onKeyDown.ComponentData);
			        break;
				case MenuKind.AutoCompleteMenu:
					TextEditorCommandDefaultFunctions.ShowAutocompleteMenu(
		        		editContext,
				        modelModifier,
				        viewModel,
				        cursorModifierBag,
				        primaryCursorModifier,
				        onKeyDown.ComponentData.DropdownService,
				        onKeyDown.ComponentData);
			        break;
			}
		}
		
		if (shouldClearTooltip)
		{
			if (viewModel.TooltipViewModel is not null)
			{
				viewModel.TooltipViewModel = null;
			}
		}
		
		if (shouldRevealCursor)
		{
			viewModel.ShouldRevealCursor = true;
		}
		
		if (shouldApplySyntaxHighlighting)
		{
			onKeyDown.ComponentData.ThrottleApplySyntaxHighlighting(modelModifier);
		}
		
		// TODO: Do this code first so the user gets immediate UI feedback in the event that
		//       their keydown code takes a long time?
		editContext.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
		
		await editContext.TextEditorService
			.FinalizePost(editContext)
			.ConfigureAwait(false);
    }
}
