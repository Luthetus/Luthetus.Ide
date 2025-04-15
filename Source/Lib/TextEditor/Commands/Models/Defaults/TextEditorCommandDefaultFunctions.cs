using System.Text;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public class TextEditorCommandDefaultFunctions
{
    public static void DoNothingDiscard()
    {
        return;
    }

    public static async ValueTask CopyAsync(
	    TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        IClipboardService clipboardService)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
        
        var selectedText = TextEditorSelectionHelper.GetSelectedText(primaryCursorModifier, modelModifier);
        selectedText ??= modelModifier.GetLineTextRange(primaryCursorModifier.LineIndex, 1);

        await clipboardService.SetClipboard(selectedText).ConfigureAwait(false);
        await viewModel.FocusAsync().ConfigureAwait(false);
    }

    public static async ValueTask CutAsync(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        IClipboardService clipboardService)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
    	
    	if (!TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier))
        {
            var positionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
            var lineInformation = modelModifier.GetLineInformationFromPositionIndex(positionIndex);

            primaryCursorModifier.SelectionAnchorPositionIndex = lineInformation.StartPositionIndexInclusive;
            primaryCursorModifier.SelectionEndingPositionIndex = lineInformation.EndPositionIndexExclusive;
        }

        var selectedText = TextEditorSelectionHelper.GetSelectedText(primaryCursorModifier, modelModifier) ?? string.Empty;
        await clipboardService.SetClipboard(selectedText).ConfigureAwait(false);

        await viewModel.FocusAsync().ConfigureAwait(false);

        modelModifier.HandleKeyboardEvent(
            new KeymapArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
            cursorModifierBag,
            CancellationToken.None);
    }

    public static async ValueTask PasteAsync(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        IClipboardService clipboardService)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
    
    	var clipboard = await clipboardService.ReadClipboard().ConfigureAwait(false);
        modelModifier.Insert(clipboard, cursorModifierBag, cancellationToken: CancellationToken.None);
    }

    public static void TriggerSave(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        ICommonComponentRenderers commonComponentRenderers,
        INotificationService notificationService)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
    	
    	if (viewModel.OnSaveRequested is null)
    	{
    		NotificationHelper.DispatchError(
		        nameof(TriggerSave),
		        $"{nameof(TriggerSave)} was null",
		        commonComponentRenderers,
				notificationService,
		        TimeSpan.FromSeconds(7));
    	}
    	else
    	{
    		viewModel.OnSaveRequested.Invoke(modelModifier);
        	modelModifier.SetIsDirtyFalse();
    	}
    }

    public static void SelectAll(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
    	
    	primaryCursorModifier.SelectionAnchorPositionIndex = 0;
        primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.CharCount;
    }

    public static void Undo(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        modelModifier.UndoEditWithUserCursor(cursorModifierBag);
    }

    public static void Redo(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
		modelModifier.RedoEditWithUserCursor(cursorModifierBag);
    }

    public static void TriggerRemeasure(
        TextEditorEditContext editContext,
        TextEditorViewModel viewModel)
    {
        editContext.TextEditorService.OptionsApi.SetRenderStateKey(Key<RenderState>.NewKey());
    }

    public static void ScrollLineDown(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        editContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
    		editContext,
	        viewModel,
	        viewModel.CharAndLineMeasurements.LineHeight);
    }

    public static void ScrollLineUp(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        editContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
            editContext,
	        viewModel,
	        -1 * viewModel.CharAndLineMeasurements.LineHeight);
    }

    public static void ScrollPageDown(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        editContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
            editContext,
	        viewModel,
	        viewModel.TextEditorDimensions.Height);
    }

    public static void ScrollPageUp(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        editContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
            editContext,
	        viewModel,
	        -1 * viewModel.TextEditorDimensions.Height);
    }

    public static void CursorMovePageBottom(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        if (viewModel.VirtualizationResult.EntryList.Any())
        {
			var primaryCursorModifier = cursorModifierBag.CursorModifier;
        
            var lastEntry = viewModel.VirtualizationResult.EntryList.Last();
            var lastEntriesRowLength = modelModifier.GetLineLength(lastEntry.LineIndex);

            primaryCursorModifier.LineIndex = lastEntry.LineIndex;
            primaryCursorModifier.ColumnIndex = lastEntriesRowLength;
        }
    }

    public static void CursorMovePageTop(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        if (viewModel.VirtualizationResult.EntryList.Any())
        {
        	var primaryCursorModifier = cursorModifierBag.CursorModifier;
        
            var firstEntry = viewModel.VirtualizationResult.EntryList.First();

            primaryCursorModifier.LineIndex = firstEntry.LineIndex;
            primaryCursorModifier.ColumnIndex = 0;
        }
    }

    public static void Duplicate(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
    
        var selectedText = TextEditorSelectionHelper.GetSelectedText(primaryCursorModifier, modelModifier);

        TextEditorCursor cursorForInsertion;
        if (selectedText is null)
        {
            // Select line
            selectedText = modelModifier.GetLineTextRange(primaryCursorModifier.LineIndex, 1);

            cursorForInsertion = new TextEditorCursor(
                primaryCursorModifier.LineIndex,
                0,
                primaryCursorModifier.IsPrimaryCursor);
        }
        else
        {
            // Clone the TextEditorCursor to remove the TextEditorSelection otherwise the
            // selected text to duplicate would be overwritten by itself and do nothing
            cursorForInsertion = primaryCursorModifier.ToCursor() with
            {
                Selection = TextEditorSelection.Empty
            };
        }

        modelModifier.Insert(
            selectedText,
            new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, new(cursorForInsertion)),
            cancellationToken: CancellationToken.None);
    }

    public static void IndentMore(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
    
        if (!TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier))
            return;

        var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(primaryCursorModifier);

        var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
            modelModifier,
            selectionBoundsInPositionIndexUnits);

        for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
             i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
             i++)
        {
            var insertionCursor = new TextEditorCursor(i, 0, true);

            var insertionCursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new TextEditorCursorModifier(insertionCursor));

            modelModifier.Insert(
                KeyboardKeyFacts.WhitespaceCharacters.TAB.ToString(),
                insertionCursorModifierBag,
                cancellationToken: CancellationToken.None);
        }

        var lowerBoundPositionIndexChange = 1;

        var upperBoundPositionIndexChange = selectionBoundsInRowIndexUnits.upperRowIndexExclusive -
            selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;

        if (primaryCursorModifier.SelectionAnchorPositionIndex < primaryCursorModifier.SelectionEndingPositionIndex)
        {
            primaryCursorModifier.SelectionAnchorPositionIndex += lowerBoundPositionIndexChange;
            primaryCursorModifier.SelectionEndingPositionIndex += upperBoundPositionIndexChange;
        }
        else
        {
            primaryCursorModifier.SelectionAnchorPositionIndex += upperBoundPositionIndexChange;
            primaryCursorModifier.SelectionEndingPositionIndex += lowerBoundPositionIndexChange;
        }

        primaryCursorModifier.SetColumnIndexAndPreferred(1 + primaryCursorModifier.ColumnIndex);
    }

    public static void IndentLess(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
    	
    	(int lowerRowIndexInclusive, int upperRowIndexExclusive) selectionBoundsInRowIndexUnits;
    
    	if (primaryCursorModifier.SelectionAnchorPositionIndex is null)
    	{
    		selectionBoundsInRowIndexUnits = (primaryCursorModifier.LineIndex, primaryCursorModifier.LineIndex + 1);
    	}
    	else
    	{
	        var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(primaryCursorModifier);
	
	        selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
	            modelModifier,
	            selectionBoundsInPositionIndexUnits);
        }

        bool isFirstLoop = true;

        for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
             i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
             i++)
        {
            var rowPositionIndex = modelModifier.GetPositionIndex(i, 0);
            var characterReadCount = TextEditorModel.TAB_WIDTH;
            var lengthOfRow = modelModifier.GetLineLength(i);

            characterReadCount = Math.Min(lengthOfRow, characterReadCount);

            var readResult = modelModifier.GetString(rowPositionIndex, characterReadCount);
            var removeCharacterCount = 0;

            if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.TAB))
            {
                removeCharacterCount = 1;

                var cursorForDeletion = new TextEditorCursor(i, 0, true);

                modelModifier.DeleteByRange(
                    removeCharacterCount, // Delete a single "Tab" character
                    new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, new(cursorForDeletion)),
                    CancellationToken.None);
            }
            else if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.SPACE))
            {
                var cursorForDeletion = new TextEditorCursor(i, 0, true);
                var contiguousSpaceCount = 0;

                foreach (var character in readResult)
                {
                    if (character == KeyboardKeyFacts.WhitespaceCharacters.SPACE)
                        contiguousSpaceCount++;
                }

                removeCharacterCount = contiguousSpaceCount;

                modelModifier.DeleteByRange(
                    removeCharacterCount,
                    new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, new(cursorForDeletion)),
                    CancellationToken.None);
            }

            // Modify the lower bound of user's text selection
            if (isFirstLoop)
            {
                isFirstLoop = false;

                if (primaryCursorModifier.SelectionAnchorPositionIndex < primaryCursorModifier.SelectionEndingPositionIndex)
                    primaryCursorModifier.SelectionAnchorPositionIndex -= removeCharacterCount;
                else
                    primaryCursorModifier.SelectionEndingPositionIndex -= removeCharacterCount;
            }

            // Modify the upper bound of user's text selection
            if (primaryCursorModifier.SelectionAnchorPositionIndex < primaryCursorModifier.SelectionEndingPositionIndex)
                primaryCursorModifier.SelectionEndingPositionIndex -= removeCharacterCount;
            else
                primaryCursorModifier.SelectionAnchorPositionIndex -= removeCharacterCount;

            // Modify the column index of user's cursor
            if (i == primaryCursorModifier.LineIndex)
            {
                var nextColumnIndex = primaryCursorModifier.ColumnIndex - removeCharacterCount;

                primaryCursorModifier.LineIndex = primaryCursorModifier.LineIndex;
                primaryCursorModifier.ColumnIndex = Math.Max(0, nextColumnIndex);
            }
        }
    }

    public static void ClearTextSelection(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
        primaryCursorModifier.SelectionAnchorPositionIndex = null;
    }

    public static void NewLineBelow(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
        primaryCursorModifier.SelectionAnchorPositionIndex = null;

        var lengthOfRow = modelModifier.GetLineLength(primaryCursorModifier.LineIndex);

        primaryCursorModifier.LineIndex = primaryCursorModifier.LineIndex;
        primaryCursorModifier.ColumnIndex = lengthOfRow;
        
        // NOTE: keep the value to insert as '\n' because this will be changed to the user's
        //       preferred line ending upon insertion.
        var valueToInsert = "\n";
        
        // GOAL: Match indentation on newline keystroke (2024-07-07)
        {
			var line = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex);

			var cursorPositionIndex = line.StartPositionIndexInclusive + primaryCursorModifier.ColumnIndex;
			var indentationPositionIndex = line.StartPositionIndexInclusive;

			var indentationBuilder = new StringBuilder();

			while (indentationPositionIndex < cursorPositionIndex)
			{
				var possibleIndentationChar = modelModifier.RichCharacterList[indentationPositionIndex++].Value;

				if (possibleIndentationChar == '\t' || possibleIndentationChar == ' ')
					indentationBuilder.Append(possibleIndentationChar);
				else
					break;
			}

			valueToInsert += indentationBuilder.ToString();
        }

        modelModifier.Insert(valueToInsert, cursorModifierBag, cancellationToken: CancellationToken.None);
    }

    public static void NewLineAbove(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
        primaryCursorModifier.SelectionAnchorPositionIndex = null;
            
        var originalColumnIndex = primaryCursorModifier.ColumnIndex;

        primaryCursorModifier.LineIndex = primaryCursorModifier.LineIndex;
        primaryCursorModifier.ColumnIndex = 0;
        
        // NOTE: keep the value to insert as '\n' because this will be changed to the user's
        //       preferred line ending upon insertion.
        var valueToInsert = "\n";
        
        var indentationLength = 0;
        
        // GOAL: Match indentation on newline keystroke (2024-07-07)
        {
			var line = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex);

			var cursorPositionIndex = line.StartPositionIndexInclusive + originalColumnIndex;
			var indentationPositionIndex = line.StartPositionIndexInclusive;

			var indentationBuilder = new StringBuilder();

			while (indentationPositionIndex < cursorPositionIndex)
			{
				var possibleIndentationChar = modelModifier.RichCharacterList[indentationPositionIndex++].Value;

				if (possibleIndentationChar == '\t' || possibleIndentationChar == ' ')
					indentationBuilder.Append(possibleIndentationChar);
				else
					break;
			}

			valueToInsert = indentationBuilder.ToString() + valueToInsert;
			indentationLength = indentationBuilder.Length;
        }

        modelModifier.Insert(valueToInsert, cursorModifierBag, cancellationToken: CancellationToken.None);

        if (primaryCursorModifier.LineIndex > 1)
        {
            primaryCursorModifier.LineIndex--;
            primaryCursorModifier.ColumnIndex = indentationLength;
        }
    }
    
    public static void MoveLineDown(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
        var lineIndexOriginal = primaryCursorModifier.LineIndex;
        var columnIndexOriginal = primaryCursorModifier.ColumnIndex;

		var nextLineIndex = lineIndexOriginal + 1;
		var nextLineInformation = modelModifier.GetLineInformation(nextLineIndex);

		// Insert
		{
			var currentLineContent = modelModifier.GetLineTextRange(lineIndexOriginal, 1);
		
			primaryCursorModifier.LineIndex = nextLineIndex + 1;
			primaryCursorModifier.ColumnIndex = 0;
			
			var innerCursorModifierBag = new CursorModifierBagTextEditor(
		        Key<TextEditorViewModel>.Empty,
		        primaryCursorModifier);

			modelModifier.Insert(
				value: currentLineContent,
				cursorModifierBag: innerCursorModifierBag,
				useLineEndKindPreference: false);
		}

		// Delete
		{
			primaryCursorModifier.LineIndex = lineIndexOriginal;
			primaryCursorModifier.ColumnIndex = 0;
			
			var currentLineInformation = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex);
			var columnCount = currentLineInformation.EndPositionIndexExclusive -
				currentLineInformation.StartPositionIndexInclusive;

			var innerCursorModifierBag = new CursorModifierBagTextEditor(
		        Key<TextEditorViewModel>.Empty,
		        primaryCursorModifier);

			modelModifier.Delete(
		        innerCursorModifierBag,
		        columnCount,
		        false,
		        TextEditorModel.DeleteKind.Delete);
		}
		
		primaryCursorModifier.LineIndex = lineIndexOriginal + 1;
		primaryCursorModifier.ColumnIndex = 0;
    }
    
    public static void MoveLineUp(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
        var lineIndexOriginal = primaryCursorModifier.LineIndex;
		var columnIndexOriginal = primaryCursorModifier.ColumnIndex;
			
		var previousLineIndex = lineIndexOriginal - 1;
		var previousLineInformation = modelModifier.GetLineInformation(previousLineIndex);

		// Insert
		{
			var currentLineContent = modelModifier.GetLineTextRange(lineIndexOriginal, 1);
		
			primaryCursorModifier.LineIndex = previousLineIndex;
			primaryCursorModifier.ColumnIndex = 0;

			var innerCursorModifierBag = new CursorModifierBagTextEditor(
		        Key<TextEditorViewModel>.Empty,
		        primaryCursorModifier);

			modelModifier.Insert(
				value: currentLineContent,
				cursorModifierBag: innerCursorModifierBag,
				useLineEndKindPreference: false);
		}

		// Delete
		{
			// Add 1 because a line was inserted
			primaryCursorModifier.LineIndex = lineIndexOriginal + 1;
			primaryCursorModifier.ColumnIndex = 0;
			
			var currentLineInformation = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex);
			var columnCount = currentLineInformation.EndPositionIndexExclusive -
				currentLineInformation.StartPositionIndexInclusive;

			var innerCursorModifierBag = new CursorModifierBagTextEditor(
		        Key<TextEditorViewModel>.Empty,
		        primaryCursorModifier);

			modelModifier.Delete(
		        innerCursorModifierBag,
		        columnCount,
		        false,
		        TextEditorModel.DeleteKind.Delete);
		}
		
		primaryCursorModifier.LineIndex = lineIndexOriginal - 1;
		primaryCursorModifier.ColumnIndex = 0;
    }

    public static void GoToMatchingCharacter(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        bool shouldSelectText)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
        var cursorPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);

        if (shouldSelectText)
        {
            if (!TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier))
                primaryCursorModifier.SelectionAnchorPositionIndex = cursorPositionIndex;
        }
        else
        {
            primaryCursorModifier.SelectionAnchorPositionIndex = null;
        }

        var previousCharacter = modelModifier.GetCharacter(cursorPositionIndex - 1);
        var currentCharacter = modelModifier.GetCharacter(cursorPositionIndex);

        char? characterToMatch = null;
        char? match = null;

        var fallbackToPreviousCharacter = false;

        if (CharacterKindHelper.CharToCharacterKind(currentCharacter) == CharacterKind.Punctuation)
        {
            // Prefer current character
            match = KeyboardKeyFacts.MatchPunctuationCharacter(currentCharacter);

            if (match is not null)
                characterToMatch = currentCharacter;
        }

        if (characterToMatch is null && CharacterKindHelper.CharToCharacterKind(previousCharacter) == CharacterKind.Punctuation)
        {
            // Fallback to the previous current character
            match = KeyboardKeyFacts.MatchPunctuationCharacter(previousCharacter);

            if (match is not null)
            {
                characterToMatch = previousCharacter;
                fallbackToPreviousCharacter = true;
            }
        }

        if (characterToMatch is null || match is null)
            return;

        var directionToFindMatchingPunctuationCharacter = KeyboardKeyFacts.DirectionToFindMatchingPunctuationCharacter(
            characterToMatch.Value);

        if (directionToFindMatchingPunctuationCharacter is null)
            return;

        var unmatchedCharacters = fallbackToPreviousCharacter && -1 == directionToFindMatchingPunctuationCharacter
            ? 0
            : 1;

        while (true)
        {
            KeymapArgs keymapArgs;

            if (directionToFindMatchingPunctuationCharacter == -1)
            {
                keymapArgs = new KeymapArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                    ShiftKey = shouldSelectText,
                };
            }
            else
            {
                keymapArgs = new KeymapArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    ShiftKey = shouldSelectText,
                };
            }

            editContext.TextEditorService.ViewModelApi.MoveCursorUnsafe(
        		keymapArgs,
		        editContext,
		        modelModifier,
		        viewModel,
		        cursorModifierBag,
		        primaryCursorModifier);

            var positionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
            var characterAt = modelModifier.GetCharacter(positionIndex);

            if (characterAt == match)
                unmatchedCharacters--;
            else if (characterAt == characterToMatch)
                unmatchedCharacters++;

            if (unmatchedCharacters == 0)
                break;

            if (positionIndex <= 0 || positionIndex >= modelModifier.CharCount)
                break;
        }

        if (shouldSelectText)
            primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
    }

    public static async ValueTask RelatedFilesQuickPick(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        LuthetusCommonJavaScriptInteropApi jsRuntimeCommonApi,
        IEnvironmentProvider environmentProvider,
        IFileSystemProvider fileSystemProvider,
        ITextEditorService textEditorService,
        IDropdownService dropdownService)
    {
		var cursorDimensions = await jsRuntimeCommonApi
			.MeasureElementById(viewModel.PrimaryCursorContentId)
			.ConfigureAwait(false);

		var resourceAbsolutePath = environmentProvider.AbsolutePathFactory(modelModifier.ResourceUri.Value, false);
		var parentDirectoryAbsolutePath = environmentProvider.AbsolutePathFactory(resourceAbsolutePath.ParentDirectory, true);
	
		var siblingFileStringList = Array.Empty<string>();
		
		try
		{
			siblingFileStringList = await fileSystemProvider.Directory
				.GetFilesAsync(parentDirectoryAbsolutePath.Value)
				.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		var menuOptionList = new List<MenuOptionRecord>();
		
		siblingFileStringList = siblingFileStringList.OrderBy(x => x).ToArray();
		
		var initialActiveMenuOptionRecordIndex = -1;
		
		for (int i = 0; i < siblingFileStringList.Length; i++)
		{
			var file = siblingFileStringList[i];
			
			var siblingAbsolutePath = environmentProvider.AbsolutePathFactory(file, false);
			
			menuOptionList.Add(new MenuOptionRecord(
				siblingAbsolutePath.NameWithExtension,
				MenuOptionKind.Other,
				onClickFunc: async () => 
				{
					textEditorService.WorkerArbitrary.PostUnique(nameof(TextEditorCommandDefaultFunctions), async editContext =>
			    	{
			    		await textEditorService.OpenInEditorAsync(
			    			editContext,
			                file,
							true,
							null,
							new Category("main"),
							Key<TextEditorViewModel>.NewKey());
			    	});
				}));
					
			if (siblingAbsolutePath.NameWithExtension == resourceAbsolutePath.NameWithExtension)
				initialActiveMenuOptionRecordIndex = i;
		}
		
		MenuRecord menu;
		
		if (menuOptionList.Count == 0)
			menu = new MenuRecord(MenuRecord.NoMenuOptionsExistList);
		else
			menu = new MenuRecord(menuOptionList);
		
		var dropdownRecord = new DropdownRecord(
			Key<DropdownRecord>.NewKey(),
			cursorDimensions.LeftInPixels,
			cursorDimensions.TopInPixels + cursorDimensions.HeightInPixels,
			typeof(MenuDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(MenuDisplay.MenuRecord),
					menu
				},
				{
					nameof(MenuDisplay.InitialActiveMenuOptionRecordIndex),
					initialActiveMenuOptionRecordIndex
				}
			},
			// TODO: this callback when the dropdown closes is suspect.
			//       The editContext is supposed to live the lifespan of the
			//       Post. But what if the Post finishes before the dropdown is closed?
			async () => 
			{
				// TODO: Even if this '.single or default' to get the main group works it is bad and I am ashamed...
				//       ...I'm too tired at the moment, need to make this sensible.
				//	   The key is in the IDE project yet its circular reference if I do so, gotta
				//       make groups more sensible I'm not sure what to say here I'm super tired and brain checked out.
				//       |
				//       I ran this and it didn't work. Its for the best that it doesn't.
				//	   maybe when I wake up tomorrow I'll realize what im doing here.
				var mainEditorGroup = textEditorService.GroupApi.GetTextEditorGroupState().GroupList.SingleOrDefault();
				
				if (mainEditorGroup is not null &&
					mainEditorGroup.ActiveViewModelKey != Key<TextEditorViewModel>.Empty)
				{
					var activeViewModel = textEditorService.ViewModelApi.GetOrDefault(mainEditorGroup.ActiveViewModelKey);

					if (activeViewModel is not null)
						await activeViewModel.FocusAsync();
				}
				
				await viewModel.FocusAsync();
			});

        dropdownService.ReduceRegisterAction(dropdownRecord);
    }
    
    public static async ValueTask QuickActionsSlashRefactor(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        LuthetusCommonJavaScriptInteropApi jsRuntimeCommonApi,
        ITextEditorService textEditorService,
        IDropdownService dropdownService)
    {
		var cursorDimensions = await jsRuntimeCommonApi
			.MeasureElementById(viewModel.PrimaryCursorContentId)
			.ConfigureAwait(false);

		var primaryCursorModifier = cursorModifierBag.CursorModifier;
		var compilerService = modelModifier.CompilerService;

		var menu = await compilerService.GetQuickActionsSlashRefactorMenu(
			editContext,
	        modelModifier,
	        viewModel,
	        cursorModifierBag);

		var dropdownRecord = new DropdownRecord(
			Key<DropdownRecord>.NewKey(),
			cursorDimensions.LeftInPixels,
			cursorDimensions.TopInPixels + cursorDimensions.HeightInPixels,
			typeof(MenuDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(MenuDisplay.MenuRecord),
					menu
				}
			},
			// TODO: this callback when the dropdown closes is suspect.
			//       The editContext is supposed to live the lifespan of the
			//       Post. But what if the Post finishes before the dropdown is closed?
			async () => 
			{
				// TODO: Even if this '.single or default' to get the main group works it is bad and I am ashamed...
				//       ...I'm too tired at the moment, need to make this sensible.
				//	   The key is in the IDE project yet its circular reference if I do so, gotta
				//       make groups more sensible I'm not sure what to say here I'm super tired and brain checked out.
				//       |
				//       I ran this and it didn't work. Its for the best that it doesn't.
				//	   maybe when I wake up tomorrow I'll realize what im doing here.
				var mainEditorGroup = textEditorService.GroupApi.GetTextEditorGroupState().GroupList.SingleOrDefault();
				
				if (mainEditorGroup is not null &&
					mainEditorGroup.ActiveViewModelKey != Key<TextEditorViewModel>.Empty)
				{
					var activeViewModel = textEditorService.ViewModelApi.GetOrDefault(mainEditorGroup.ActiveViewModelKey);

					if (activeViewModel is not null)
						await activeViewModel.FocusAsync();
				}
				
				await viewModel.FocusAsync();
			});

        dropdownService.ReduceRegisterAction(dropdownRecord);
    }
    
    public static void GoToDefinition(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        Category category)
    {
    	modelModifier.CompilerService.GoToDefinition(
			editContext,
	        modelModifier,
	        viewModel,
	        cursorModifierBag,
	        category);
    }

    public static void ShowFindAllDialog(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        ITextEditorService textEditorService)
    {
        textEditorService.OptionsApi.ShowFindAllDialog();
    }

    public static async ValueTask ShowTooltipByCursorPositionAsync(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        ITextEditorService textEditorService,
        TextEditorComponentData componentData,
        ILuthetusTextEditorComponentRenderers textEditorComponentRenderers)
    {
        var elementPositionInPixels = await textEditorService.JsRuntimeTextEditorApi
            .GetBoundingClientRect(viewModel.PrimaryCursorContentId)
            .ConfigureAwait(false);

        elementPositionInPixels = elementPositionInPixels with
        {
            Top = elementPositionInPixels.Top +
                (.9 * viewModel.CharAndLineMeasurements.LineHeight)
        };

        await HandleMouseStoppedMovingEventAsync(
        		editContext,
        		modelModifier,
        		viewModel,
				new MouseEventArgs
	            {
	                ClientX = elementPositionInPixels.Left,
	                ClientY = elementPositionInPixels.Top
	            },
				componentData,
				textEditorComponentRenderers,
				modelModifier.ResourceUri)
			.ConfigureAwait(false);
    }

	/// <summary>The default <see cref="AfterOnKeyDownAsync"/> will provide syntax highlighting, and autocomplete.<br/><br/>The syntax highlighting occurs on ';', whitespace, paste, undo, redo<br/><br/>The autocomplete occurs on LetterOrDigit typed or { Ctrl + Space }. Furthermore, the autocomplete is done via <see cref="IAutocompleteService"/> and the one can provide their own implementation when registering the Luthetus.TextEditor services using <see cref="LuthetusTextEditorConfig.AutocompleteServiceFactory"/></summary>
	public static async ValueTask HandleAfterOnKeyDownAsync(
		TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        KeymapArgs keymapArgs,
		TextEditorComponentData componentData)
    {
        // Indexing can be invoked and this method still check for syntax highlighting and such
        if (EventUtils.IsAutocompleteIndexerInvoker(keymapArgs))
        {
            try
            {
				var primaryCursorModifier = cursorModifierBag.CursorModifier;

				if (primaryCursorModifier.ColumnIndex > 0)
				{
					// All keyboardEventArgs that return true from "IsAutocompleteIndexerInvoker"
					// are to be 1 character long, as well either specific whitespace or punctuation.
					// Therefore 1 character behind might be a word that can be indexed.
					var word = modelModifier.ReadPreviousWordOrDefault(
						primaryCursorModifier.LineIndex,
						primaryCursorModifier.ColumnIndex);

					if (word is not null)
					{
						_ = Task.Run(async () =>
						{
							await editContext.TextEditorService.AutocompleteIndexer
								.IndexWordAsync(word)
								.ConfigureAwait(false);
						});
					}
				}
			}
            catch (LuthetusTextEditorException e)
            {
                // Eat this exception
            }
        }

        if (EventUtils.IsAutocompleteMenuInvoker(keymapArgs))
        {
        	ShowAutocompleteMenu(
        		editContext,
		        modelModifier,
		        viewModel,
		        cursorModifierBag,
		        cursorModifierBag.CursorModifier,
		        componentData.DropdownService,
		        componentData);
        }
        else if (EventUtils.IsSyntaxHighlightingInvoker(keymapArgs))
        {
            componentData.ThrottleApplySyntaxHighlighting(modelModifier);
        }
    }

	/// <summary>
	/// This method was being used in the 'OnKeyDownBatch.cs' class, which no longer exists.
	/// The replacement for 'OnKeyDownBatch.cs' is 'OnKeyDownLateBatching.cs'.
	///
	/// But, during the replacement process, this method was overlooked.
	///
	/// One would likely want to use this method when appropriate because
	/// it permits every batched keyboard event to individually be given a chance
	/// to trigger 'HandleAfterOnKeyDownAsyncFactory(...)'
	///
	/// Example: a 'space' keyboard event, batched with the letter 'a' keyboard event.
	/// Depending on what 'OnKeyDownLateBatching.cs' does, perhaps it takes the last keyboard event
	/// and uses that to fire 'HandleAfterOnKeyDownAsyncFactory(...)'.
	///
	/// Well, a 'space' keyboard event would have trigger syntax highlighting to be refreshed.
	/// Whereas, the letter 'a' keyboard event won't do anything beyond inserting the letter.
	/// Therefore, the syntax highlighting was erroneously not refreshed due to batching.
	/// This method is intended to solve this problem, but it was forgotten at some point.
	/// </summary>
	public static async ValueTask HandleAfterOnKeyDownRangeAsync(
		TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        KeymapArgs[] batchKeymapArgsList,
        int batchKeymapArgsListLength,
		TextEditorComponentData componentData,
		ViewModelDisplayOptions viewModelDisplayOptions)
    {
        if (viewModelDisplayOptions.AfterOnKeyDownRangeAsync is not null)
        {
            await viewModelDisplayOptions.AfterOnKeyDownRangeAsync.Invoke(
                editContext,
		        modelModifier,
		        viewModel,
		        cursorModifierBag,
                batchKeymapArgsList,
                batchKeymapArgsListLength,
                componentData);
            return;
        }

        var seenIsAutocompleteIndexerInvoker = false;
        var seenIsAutocompleteMenuInvoker = false;
        var seenIsSyntaxHighlightingInvoker = false;

        for (int i = 0; i < batchKeymapArgsListLength; i++)
        {
            var keymapArgs = batchKeymapArgsList[i];

            if (!seenIsAutocompleteIndexerInvoker && EventUtils.IsAutocompleteIndexerInvoker(keymapArgs))
                seenIsAutocompleteIndexerInvoker = true;

            if (!seenIsAutocompleteMenuInvoker && EventUtils.IsAutocompleteMenuInvoker(keymapArgs))
                seenIsAutocompleteMenuInvoker = true;
            else if (!seenIsSyntaxHighlightingInvoker && EventUtils.IsSyntaxHighlightingInvoker(keymapArgs))
                seenIsSyntaxHighlightingInvoker = true;
        }

        if (seenIsAutocompleteIndexerInvoker)
        {
        	var primaryCursorModifier = cursorModifierBag.CursorModifier;
        	
            if (primaryCursorModifier.ColumnIndex > 0)
            {
                // All keyboardEventArgs that return true from "IsAutocompleteIndexerInvoker"
                // are to be 1 character long, as well either specific whitespace or punctuation.
                // Therefore 1 character behind might be a word that can be indexed.
                var word = modelModifier.ReadPreviousWordOrDefault(
                    primaryCursorModifier.LineIndex,
                    primaryCursorModifier.ColumnIndex);

                if (word is not null)
                {
		            _ = Task.Run(async () =>
		            {
                        await editContext.TextEditorService.AutocompleteIndexer
                            .IndexWordAsync(word)
                            .ConfigureAwait(false);
		            });
                }
            }
        }

        if (seenIsAutocompleteMenuInvoker)
        {
        	ShowAutocompleteMenu(
        		editContext,
		        modelModifier,
		        viewModel,
		        cursorModifierBag,
		        cursorModifierBag.CursorModifier,
		        componentData.DropdownService,
		        componentData);
        }

        if (seenIsSyntaxHighlightingInvoker)
        {
            componentData.ThrottleApplySyntaxHighlighting(modelModifier);
        }
    }

	public static ValueTask HandleMouseStoppedMovingEventAsync(
		TextEditorEditContext editContext,
		TextEditorModel modelModifier,
		TextEditorViewModel viewModel,
		MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
		ILuthetusTextEditorComponentRenderers textEditorComponentRenderers,
        ResourceUri resourceUri)
    {
    	return modelModifier.CompilerService.OnInspect(
			editContext,
			modelModifier,
			viewModel,
			mouseEventArgs,
			componentData,
			textEditorComponentRenderers,
	        resourceUri);
    }
    
    /// <summary>
    /// If left or top is left as null, it will be set to whatever the primary cursor's left or top is respectively.
    /// The left and top values are offsets from the text editor's bounding client rect.
    ///
    /// Use the method <see cref="RemoveDropdown"/> to un-render the dropdown programmatically.
    /// </summary>
    public static void ShowDropdown(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier primaryCursor,
        IDropdownService dropdownService,
        double? leftOffset,
        double? topOffset,
        Type componentType,
        Dictionary<string, object?>? componentParameters)
    {
        var dropdownKey = new Key<DropdownRecord>(viewModel.ViewModelKey.Guid);
        
        if (leftOffset is null)
        {
        	leftOffset = primaryCursor.ColumnIndex * viewModel.CharAndLineMeasurements.CharacterWidth;
	        
	        // Tab key column offset
            var tabsOnSameRowBeforeCursor = modelModifier.GetTabCountOnSameLineBeforeCursor(
                primaryCursor.LineIndex,
                primaryCursor.ColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            leftOffset += extraWidthPerTabKey *
                tabsOnSameRowBeforeCursor *
                viewModel.CharAndLineMeasurements.CharacterWidth;
                
            leftOffset -= viewModel.ScrollbarDimensions.ScrollLeft;
        }
        
        topOffset ??= (primaryCursor.LineIndex + 1) *
        	viewModel.CharAndLineMeasurements.LineHeight -
        	viewModel.ScrollbarDimensions.ScrollTop;
		
		var dropdownRecord = new DropdownRecord(
			dropdownKey,
			viewModel.TextEditorDimensions.BoundingClientRectLeft + leftOffset.Value,
			viewModel.TextEditorDimensions.BoundingClientRectTop + topOffset.Value,
			componentType,
			componentParameters,
			async () => await viewModel.FocusAsync())
		{
			ShouldShowOutOfBoundsClickDisplay = false
		};

        dropdownService.ReduceRegisterAction(dropdownRecord);
	}
	
	public static void RemoveDropdown(
        TextEditorEditContext editContext,
        TextEditorViewModel viewModel,
        IDropdownService dropdownService)
    {
    	viewModel.MenuKind = MenuKind.None;
    
		var dropdownKey = new Key<DropdownRecord>(viewModel.ViewModelKey.Guid);
		dropdownService.ReduceDisposeAction(dropdownKey);
	}
	
	public static void ShowContextMenu(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier primaryCursor,
        IDropdownService dropdownService,
        TextEditorComponentData componentData)
    {
    	viewModel.MenuKind = MenuKind.ContextMenu;
    
    	ShowDropdown(
    		editContext,
	        modelModifier,
	        viewModel,
	        cursorModifierBag,
	        primaryCursor,
	        dropdownService,
	        leftOffset: null,
	        topOffset: null,
	        typeof(Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.ContextMenu),
	        new Dictionary<string, object?>
			{
				{
					nameof(Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.ContextMenu.TextEditorViewModelDisplay),
					componentData.TextEditorViewModelDisplay
				},
			});
	}
	
	public static void ShowAutocompleteMenu(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier primaryCursor,
        IDropdownService dropdownService,
        TextEditorComponentData componentData)
    {
    	viewModel.MenuKind = MenuKind.AutoCompleteMenu;
    
    	ShowDropdown(
    		editContext,
	        modelModifier,
	        viewModel,
	        cursorModifierBag,
	        primaryCursor,
	        dropdownService,
	        leftOffset: null,
	        topOffset: null,
	        typeof(Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.AutocompleteMenu),
	        new Dictionary<string, object?>
			{
				{
					nameof(Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.AutocompleteMenu.TextEditorViewModelDisplay),
					componentData.TextEditorViewModelDisplay
				},
			});
	}
	
	public static async ValueTask ShowFindOverlay(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier primaryCursor,
        LuthetusCommonJavaScriptInteropApi commonJavaScriptInteropApi)
    {
		// If the user has an active text selection,
		// then populate the find overlay with their selection.
		
        var selectedText = TextEditorSelectionHelper.GetSelectedText(primaryCursor, modelModifier);
		if (selectedText is not null)
		{
			viewModel.FindOverlayValue = selectedText;
            viewModel.FindOverlayValueExternallyChangedMarker = !viewModel.FindOverlayValueExternallyChangedMarker;
		}

        if (viewModel.ShowFindOverlay)
        {
            await commonJavaScriptInteropApi
                .FocusHtmlElementById(viewModel.FindOverlayId)
                .ConfigureAwait(false);
        }
        else
        {
            viewModel.ShowFindOverlay = true;
        }
    }
    
    public static void PopulateSearchFindAll(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier primaryCursor,
        IFindAllService findAllService)
    {
		// If the user has an active text selection,
		// then populate the find overlay with their selection.
		
        if (modelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursor is null)
            return;

        var selectedText = TextEditorSelectionHelper.GetSelectedText(primaryCursor, modelModifier);
		if (selectedText is null)
			return;
			
		findAllService.SetSearchQuery(selectedText);
    }
}
