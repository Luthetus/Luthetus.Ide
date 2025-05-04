using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public static class EventUtils
{
    public static KeymapArgsKind GetKeymapArgsKind(
		TextEditorComponentData componentData,
        KeymapArgs keymapArgs,
        bool hasSelection,
        ITextEditorService textEditorService,
        out CommandNoType command)
    {
        var eventIsCommand = CheckIfKeymapArgsMapsToCommand(
			componentData,
            keymapArgs,
            hasSelection,
            textEditorService,
            out Key<KeymapLayer> layerKey,
            out var success,
            out command);

        var eventIsMovement = CheckIfKeyboardEventArgsMapsToMovement(keymapArgs, command);

        var eventIsContextMenu = KeyboardKeyFacts.CheckIsContextMenuEvent(keymapArgs);

        if (eventIsMovement)
            return KeymapArgsKind.Movement;

        if (eventIsContextMenu)
            return KeymapArgsKind.ContextMenu;

        if (eventIsCommand)
            return KeymapArgsKind.Command;

        if (keymapArgs.Key.Length == 1)
        {
        	// Only write text if no modifiers (other than shift) were held at the time of the event.
        	if (!keymapArgs.CtrlKey && !keymapArgs.AltKey && !keymapArgs.MetaKey)
        		return KeymapArgsKind.Text;
        	else
        		return KeymapArgsKind.None;
        }

        return KeymapArgsKind.Other;
    }

    public static bool IsKeyboardEventArgsNoise(KeymapArgs keymapArgs)
    {
        if (keymapArgs.Key == "Shift" ||
            keymapArgs.Key == "Control" ||
            keymapArgs.Key == "Alt" ||
            keymapArgs.Key == "Meta")
        {
            return true;
        }

		// TODO: See following code block (its commented out).
		//       The commented out hack was not a good idea,
		//       This comment is here as a reminder not to repeat
		//       this bad solution.
		//       |
		//       i.e.: this is not yet fixed, but don't fix it the way thats commented out below.
		//             Once this is fixed, then delete this comment.
		//       |
		//       The issue was { Ctrl + Alt + (ArrowRight || ArrowLeft) }
		//       to perform "camel case movement of the cursor".
		// {
	        //if (keyboardEventArgs.CtrlKey && keyboardEventArgs.AltKey)
	        //{
	        //    // TODO: This if is a hack to fix the keybind: { Ctrl + Alt + S } causing...
	        //    // ...an 's' to be written out when using Vim keymap.
	        //    return true;
	        //}
        // }

        return false;
    }
    
    public static bool IsKeyboardEventArgsNoise(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == "Shift" ||
            keyboardEventArgs.Key == "Control" ||
            keyboardEventArgs.Key == "Alt" ||
            keyboardEventArgs.Key == "Meta")
        {
            return true;
        }

		// TODO: See following code block (its commented out).
		//       The commented out hack was not a good idea,
		//       This comment is here as a reminder not to repeat
		//       this bad solution.
		//       |
		//       i.e.: this is not yet fixed, but don't fix it the way thats commented out below.
		//             Once this is fixed, then delete this comment.
		//       |
		//       The issue was { Ctrl + Alt + (ArrowRight || ArrowLeft) }
		//       to perform "camel case movement of the cursor".
		// {
	        //if (keyboardEventArgs.CtrlKey && keyboardEventArgs.AltKey)
	        //{
	        //    // TODO: This if is a hack to fix the keybind: { Ctrl + Alt + S } causing...
	        //    // ...an 's' to be written out when using Vim keymap.
	        //    return true;
	        //}
        // }

        return false;
    }

    public static bool CheckIfKeymapArgsMapsToCommand(
		TextEditorComponentData componentData,
        KeymapArgs keymapArgs,
        bool hasSelection,
        ITextEditorService textEditorService,
        out Key<KeymapLayer> layerKey,
        out bool success,
        out CommandNoType command)
    {
    	layerKey = ((ITextEditorKeymap)componentData.Options.Keymap!).GetLayer(hasSelection);
    	command = null;
    	success = false;
    	return false;
    
        /*layerKey = ((ITextEditorKeymap)componentData.Options.Keymap!).GetLayer(hasSelection);

        keymapArgs.LayerKey = layerKey;

        success = ((ITextEditorKeymap)componentData.Options.Keymap!).TryMap(
            keymapArgs,
            componentData,
            out command);

        if (!success && keymapArgs.LayerKey != TextEditorKeymapDefaultFacts.DefaultLayer.Key)
        {
            keymapArgs.LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key;

            _ = ((ITextEditorKeymap)componentData.Options.Keymap!).TryMap(
                keymapArgs,
                componentData,
                out command);
        }

        /*if (KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE == keymapArgs.Code && keymapArgs.ShiftKey)
            command = TextEditorCommandDefaultFunctions.NewLineBelow();*//*

        return command is not null;*/
    }

    public static bool CheckIfKeyboardEventArgsMapsToMovement(KeymapArgs keymapArgs, CommandNoType command)
    {
        return KeyboardKeyFacts.IsMovementKey(keymapArgs.Key) && command is null;
    }

	public static bool IsAutocompleteMenuInvoker(KeymapArgs keymapArgs)
    {
        // LetterOrDigit was hit without Ctrl being held
        return !keymapArgs.CtrlKey &&
               !KeyboardKeyFacts.IsWhitespaceCode(keymapArgs.Code) &&
               !KeyboardKeyFacts.IsMetaKey(keymapArgs);
    }

	public static bool IsSyntaxHighlightingInvoker(KeymapArgs keymapArgs)
    {
    	if (keymapArgs.Key == ";" ||
    		KeyboardKeyFacts.IsWhitespaceCode(keymapArgs.Code))
    	{
    		if (keymapArgs.CtrlKey && (keymapArgs.Key == " " || keymapArgs.Key == "SPACE"))
    		{
    			// Working on using the binder to populate the autocomplete menu with the members of the type
    			// that a variable reference is a type of. (2025-01-01)
    			// ==========================================================================================
    			//
    			//
    			// Introduction to the issue.
    			// --------------------------------------------------------------------------------
    			// When typing the '.' after the variable reference's identifier, the autocomplete
    			// correctly populates with the members using the binder.
    			//
    			// But, if the cursor is immediately after an already existing '.' and then
    			// one presses { 'Ctrl' + 'Space' }, then the autocomplete is empty (with regards to the binder result).
    			//
    			//
    			// Some findings
    			// -------------------------------------------------------------------------------------
    			// In the first case where you type a '.', the node that is found at the cursor position
    			// is a (VariableReferenceNode - need to re-confirm this as I'm speaking from memory), but
    			// the second case of { 'Ctrl' + 'Space' } and an existing '.' then the found
    			// node is an EmptyExpressionNode.
    			//
    			// 
    			// Conclusion
    			// ------------------------------------------------------------------------------------- 
    			// This 'if' statement is being added temporarily in order to stop the re-parsing of the
    			// text file.
    			//
    			// Because, it is presumed to be the 're-parsing' of the text file, and some timing issue
    			// such that the node cannot be found correctly, which results
    			// in no results coming back from binder when asked for the members.
    			//
    			// This 'if' statement fixes the issue for now.
    			// But this is not a good long term solution.
    			//
    			// The code for the member autocompletion is being worked on,
    			// and I don't want to look at the timing issue until I've finished my thoughts
    			// with the member autocompletion.
    			return false;
    		}
    	
    		return true;
    	}
    	
    	if (keymapArgs.CtrlKey)
    	{
    		switch (keymapArgs.Key)
    		{
    			case "s":
    			case "v":
    			case "z":
    			case "y":
    				return true;
    		}
    	}
    	
    	return false;
    }

    /// <summary>
    /// All keyboardEventArgs that return true from "IsAutocompleteIndexerInvoker"
    /// are to be 1 character long, as well either whitespace or punctuation.
    /// Therefore 1 character behind might be a word that can be indexed.
    /// </summary>
    public static bool IsAutocompleteIndexerInvoker(KeymapArgs keymapArgs)
    {
        return (KeyboardKeyFacts.IsWhitespaceCode(keymapArgs.Code) ||
                    KeyboardKeyFacts.IsPunctuationCharacter(keymapArgs.Key.First())) &&
                !keymapArgs.CtrlKey;
    }

	public static async Task<(int LineIndex, int ColumnIndex, double PositionX, double PositionY)> CalculateLineAndColumnIndex(
		ResourceUri resourceUri,
		Key<TextEditorViewModel> viewModelKey,
		MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
		TextEditorEditContext editContext)
    {
        var modelModifier = editContext.GetModelModifier(resourceUri);
        var viewModel = editContext.GetViewModelModifier(viewModelKey);
        var globalTextEditorOptions = editContext.TextEditorService.OptionsApi.GetTextEditorOptionsState().Options;

        if (modelModifier is null || viewModel is null)
            return (0, 0, 0, 0);
    
        var charMeasurements = viewModel.CharAndLineMeasurements;
        var textEditorDimensions = viewModel.TextEditorDimensions;
        var scrollbarDimensions = viewModel.ScrollbarDimensions;
    
        var positionX = mouseEventArgs.ClientX - textEditorDimensions.BoundingClientRectLeft;
        var positionY = mouseEventArgs.ClientY - textEditorDimensions.BoundingClientRectTop;
    
        // Scroll position offset
        positionX += scrollbarDimensions.ScrollLeft;
        positionY += scrollbarDimensions.ScrollTop;
        
        var lineIndex = (int)(positionY / charMeasurements.LineHeight);
        
        var hiddenLineCount = 0;
        
		for (int i = 0; i <= lineIndex; i++)
		{
			if (viewModel.HiddenLineIndexHashSet.Contains(i))
				lineIndex++;
		}
        
        lineIndex = lineIndex > modelModifier.LineCount - 1
            ? modelModifier.LineCount - 1
            : lineIndex;
            
        var columnIndexDouble = positionX / charMeasurements.CharacterWidth;
        int columnIndexInt = (int)Math.Round(columnIndexDouble, MidpointRounding.AwayFromZero);
        
        var inlineUi = default(InlineUi);
        (int lineIndex, int columnIndex) inlineUiLineAndColumnPositionIndices = (-1, -1);
        
        foreach (var inlineUiTuple in viewModel.InlineUiList)
        {
        	inlineUiLineAndColumnPositionIndices = modelModifier.GetLineAndColumnIndicesFromPositionIndex(inlineUiTuple.InlineUi.PositionIndex);
        	
        	if (inlineUiLineAndColumnPositionIndices.lineIndex == lineIndex)
        	{
        		inlineUi = inlineUiTuple.InlineUi;
        		break;
        	}
        }
        
        var lineLength = modelModifier.GetLineLength(lineIndex);
        
        lineIndex = Math.Max(lineIndex, 0);
        columnIndexInt = Math.Max(columnIndexInt, 0);
        
        var lineInformation = modelModifier.GetLineInformation(lineIndex);
        
        int literalLength = 0;
		int visualLength = 0;
		
		var previousCharacterWidth = 1;
		var previousCharacterWidthIsInlineUi = false;
		var previousPosition = -1;
		
		for (int columnIndex = 0; columnIndex < lineLength; columnIndex++)
		{
			if (visualLength >= columnIndexInt)
		    {
		    	if (previousCharacterWidth > 1)
		    	{
		    		var visualLengthPrevious = visualLength - previousCharacterWidth;
		    		
		    		// If distance from left side to event is smaller than distance from right side to event
		    		// then put cursor on the left side
		    		// else put cursor on the right side.
		    		if (columnIndexDouble - visualLengthPrevious < visualLength - columnIndexDouble)
		    		{
		    			// Represent a '\t' with 4 character width as "--->",
		    			// a cursor by "|",
		    			// and the side closest to the event with "==" then:
		    			//
		    			//  ==
		    			// |--->
		    			if (previousCharacterWidthIsInlineUi)
		    			{
		    				viewModel.VirtualAssociativityKind = VirtualAssociativityKind.Left;
		    			}
		    			else
		    			{
		    				literalLength = literalLength - 1;
		    			}
		    			break;
		    		}
		    		else
		    		{
		    			// Represent a '\t' with 4 character width as "--->",
		    			// a cursor by "|",
		    			// and the side closest to the event with "==" then:
		    			//
		    			//    ==
		    			//  --->|
		    			if (previousCharacterWidthIsInlineUi)
		    			{
		    				viewModel.VirtualAssociativityKind = VirtualAssociativityKind.Right;
		    			}
		    			break;
		    		}
		    	}
		    
		    	break;
		    }
		    
		    if (inlineUiLineAndColumnPositionIndices.lineIndex == lineIndex &&
		    	inlineUiLineAndColumnPositionIndices.columnIndex == columnIndex &&
		    	previousPosition != columnIndex)
		    {
		    	previousCharacterWidth = 3;
		    	previousCharacterWidthIsInlineUi = true;
		    	previousPosition = columnIndex;
		    	columnIndex--;
		    	visualLength += previousCharacterWidth;
		    }
			else
			{
			    literalLength += 1;
			    
			    previousCharacterWidth = GetCharacterWidth(
			    	modelModifier.RichCharacterList[
			    		lineInformation.StartPositionIndexInclusive + columnIndex]
			    	.Value);
			    
			    visualLength += previousCharacterWidth;
			    previousCharacterWidthIsInlineUi = false;
		    }
		}
		
		int GetCharacterWidth(char character)
		{
		    if (character == '\t')
		        return 4;
		
		    return 1;
		}
        
        columnIndexInt = columnIndexInt > lineLength
            ? lineLength
            : columnIndexInt;
        
        return (lineIndex, literalLength, positionX, positionY);
    }
}
