using System.Text;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Lines.Models;
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
    	var editContext = new TextEditorEditContext(onKeyDown.ComponentData.TextEditorViewModelSlimDisplay.TextEditorService);

		// An NRE will be caught by the IBackgroundTaskService so don't bother checking 'viewModel is null'.
        var viewModel = editContext.GetViewModelModifier(onKeyDown.ViewModelKey);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModel);
        var primaryCursorModifier = cursorModifierBag.CursorModifier;

        if (viewModel is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
            return;

		var menuKind = MenuKind.None;
		var shouldClearTooltip = false;
		var shouldRevealCursor = false;
		var shouldApplySyntaxHighlighting = false;
		
		TextEditorModel? modelModifier;

		if (onKeyDown.KeymapArgs.MetaKey)
		{
			switch (onKeyDown.KeymapArgs.Code)
			{
				default:
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
			    	break;
	    	}
		}
		else if (onKeyDown.KeymapArgs.CtrlKey && onKeyDown.KeymapArgs.AltKey)
		{
			switch (onKeyDown.KeymapArgs.Code)
			{
				default:
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
			    	break;
			}
		}
		else if (onKeyDown.KeymapArgs.CtrlKey)
		{
		    switch (onKeyDown.KeymapArgs.Code)
		    {
		    	case "KeyR":
		    		modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
		    		onKeyDown.ComponentData.ThrottleApplySyntaxHighlighting(modelModifier);
		            TextEditorCommandDefaultFunctions.TriggerRemeasure(
		                editContext,
		                viewModel);
		            break;
		    	case "KeyS":
		    		modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
		            await TextEditorCommandDefaultFunctions.CopyAsync(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag,
		                onKeyDown.ComponentData.ClipboardService);
		            break;
		        case "KeyV":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
		            TextEditorCommandDefaultFunctions.SelectAll(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "KeyZ":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
		            TextEditorCommandDefaultFunctions.Undo(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            shouldRevealCursor = true;
		            shouldApplySyntaxHighlighting = true;
		            break;
		        case "KeyY":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
		            TextEditorCommandDefaultFunctions.Redo(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            shouldRevealCursor = true;
		            shouldApplySyntaxHighlighting = true;
		            break;
		        case "KeyD":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
		            TextEditorCommandDefaultFunctions.Duplicate(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            shouldRevealCursor = true;
		            shouldApplySyntaxHighlighting = true;
		            break;
		        case "ArrowDown":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
		            TextEditorCommandDefaultFunctions.ScrollLineDown(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "ArrowUp":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
		            TextEditorCommandDefaultFunctions.ScrollLineUp(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "PageDown":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
					TextEditorCommandDefaultFunctions.CursorMovePageBottom(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "PageUp":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
					TextEditorCommandDefaultFunctions.CursorMovePageTop(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "Slash":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
	            case "KeyM":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
            		CollapsePoint encompassingCollapsePoint = new CollapsePoint(-1, false, string.Empty, -1);;

					foreach (var collapsePoint in viewModel.AllCollapsePointList)
					{
						for (var lineOffset = 0; lineOffset < collapsePoint.EndExclusiveLineIndex - collapsePoint.AppendToLineIndex; lineOffset++)
						{
							if (primaryCursorModifier.LineIndex == collapsePoint.AppendToLineIndex + lineOffset)
								encompassingCollapsePoint = collapsePoint;
						}
					}
					
	            	if (encompassingCollapsePoint.AppendToLineIndex != -1)
	            	{
	            		_ = TextEditorCommandDefaultFunctions.ToggleCollapsePoint(
		            		encompassingCollapsePoint.AppendToLineIndex,
	            			modelModifier,
	            			viewModel,
	            			primaryCursorModifier);
	            	}
					
		            shouldRevealCursor = true;
		            break;
		        case "KeyF":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModel.DeleteKind.Backspace);
	                shouldRevealCursor = true;
					break;
				case "Delete":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModel.DeleteKind.Delete);
	                shouldRevealCursor = true;
	                break;
	            case "Enter":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
					var valueToInsert = modelModifier.LineEndKindPreference.AsCharacters();
			
					// Match indentation on newline keystroke
					var line = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex);
		
					var cursorPositionIndex = line.Position_StartInclusiveIndex + primaryCursorModifier.ColumnIndex;
					var indentationPositionIndex = line.Position_StartInclusiveIndex;
		
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
			            cursorModifierBag);
			            
			        if (primaryCursorModifier.LineIndex > 1)
			        {
			            primaryCursorModifier.LineIndex--;
			            primaryCursorModifier.ColumnIndex = indentationLength;
			        }
			            
	                shouldRevealCursor = true;
		            shouldApplySyntaxHighlighting = true;
	                break;
	            case "BracketRight":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
	            	TextEditorCommandDefaultFunctions.GoToMatchingCharacter(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag,
		                shouldSelectText: onKeyDown.KeymapArgs.ShiftKey);
	            	shouldRevealCursor = true;
	            	break;
	            case "Space":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
		    		break;
		    }
		}
		else if (onKeyDown.KeymapArgs.AltKey)
		{
			switch (onKeyDown.KeymapArgs.Code)
			{
				case "F12":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
		        	TextEditorCommandDefaultFunctions.GoToDefinition(
		        		editContext,
				        modelModifier,
				        viewModel,
				        cursorModifierBag,
        				new Category("CodeSearchService"));
			        break;
				default:
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
			    	break;
	    	}
		}
		else
		{
			switch (onKeyDown.KeymapArgs.Code)
			{
				case "PageDown":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
					TextEditorCommandDefaultFunctions.ScrollPageDown(
		                editContext,
		                modelModifier,
		                viewModel,
		                cursorModifierBag);
		            break;
		        case "PageUp":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
		        	if (onKeyDown.KeymapArgs.ShiftKey)
		        	{
		        		menuKind = MenuKind.ContextMenu;
	                	shouldRevealCursor = true;
                		shouldClearTooltip = true;
					    break;
		        	}
		        	break;
		        case "F7":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
		        	menuKind = MenuKind.ContextMenu;
	                shouldRevealCursor = true;
	                shouldClearTooltip = true;
				    break;
				case "CapsLock":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
					menuKind = MenuKind.None;
					shouldClearTooltip = true;
					break;
				case "Backspace":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModel.DeleteKind.Backspace);
	                shouldRevealCursor = true;
	                menuKind = MenuKind.None;
	                shouldClearTooltip = true;
					break;
				case "Delete":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
					modelModifier.Delete(
	                    cursorModifierBag,
	                    1,
	                    onKeyDown.KeymapArgs.CtrlKey,
	                    TextEditorModel.DeleteKind.Delete);
					shouldRevealCursor = true;
					menuKind = MenuKind.None;
	                shouldClearTooltip = true;
					break;
				case "Enter":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
					var valueToInsert = modelModifier.LineEndKindPreference.AsCharacters();
			
					// Match indentation on newline keystroke
					var line = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex);
		
					var cursorPositionIndex = line.Position_StartInclusiveIndex + primaryCursorModifier.ColumnIndex;
					var indentationPositionIndex = line.Position_StartInclusiveIndex;
		
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
			            cursorModifierBag);
			            
	                shouldRevealCursor = true;
	                menuKind = MenuKind.None;
	                shouldClearTooltip = true;
		            shouldApplySyntaxHighlighting = true;
	                break;
				case "Tab":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
			                    cursorModifierBag);
			                    
			                menuKind = MenuKind.None;
			                shouldClearTooltip = true;
		            		shouldApplySyntaxHighlighting = true;
			            }
			            
			            shouldRevealCursor = true;
		            }
	                break;
				case "Space":
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
	            	modelModifier.Insert(
	                    " ",
	                    cursorModifierBag);
	                    
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
					modelModifier.Insert(
	                    onKeyDown.KeymapArgs.Key,
	                    cursorModifierBag);
	                
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
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
	            	modelModifier.Insert(
	                    onKeyDown.KeymapArgs.Key,
	                    cursorModifierBag);
	                shouldRevealCursor = true;
	                menuKind = MenuKind.AutoCompleteMenu;
	                shouldClearTooltip = true;
	                break;
				default:
		        	modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
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
