using System.Text;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
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

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public class TextEditorCommandDefaultFunctions
{
    public static void DoNothingDiscard()
    {
        return;
    }

    public static async Task CopyAsync(
	    ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        IClipboardService clipboardService)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
        
        var selectedText = TextEditorSelectionHelper.GetSelectedText(primaryCursorModifier, modelModifier);
        selectedText ??= modelModifier.GetLineTextRange(primaryCursorModifier.LineIndex, 1);

        await clipboardService.SetClipboard(selectedText).ConfigureAwait(false);
        await viewModelModifier.ViewModel.FocusAsync().ConfigureAwait(false);
    }

    public static async Task CutAsync(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        IClipboardService clipboardService)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
    	
    	if (!TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier))
        {
            var positionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
            var lineInformation = modelModifier.GetLineInformationFromPositionIndex(positionIndex);

            primaryCursorModifier.SelectionAnchorPositionIndex = lineInformation.StartPositionIndexInclusive;
            primaryCursorModifier.SelectionEndingPositionIndex = lineInformation.EndPositionIndexExclusive;
        }

        var selectedText = TextEditorSelectionHelper.GetSelectedText(primaryCursorModifier, modelModifier) ?? string.Empty;
        await clipboardService.SetClipboard(selectedText).ConfigureAwait(false);

        await viewModelModifier.ViewModel.FocusAsync().ConfigureAwait(false);

        modelModifier.HandleKeyboardEvent(
            new KeymapArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
            cursorModifierBag,
            CancellationToken.None);
    }

    public static async Task PasteAsync(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        IClipboardService clipboardService)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
    
    	var clipboard = await clipboardService.ReadClipboard().ConfigureAwait(false);
        modelModifier.Insert(clipboard, cursorModifierBag, cancellationToken: CancellationToken.None);
    }

    public static void TriggerSave(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
    	
    	if (viewModelModifier.ViewModel.OnSaveRequested is null)
    	{
    		NotificationHelper.DispatchError(
		        nameof(TriggerSave),
		        $"{nameof(TriggerSave)} was null",
		        commandArgs.ServiceProvider.GetRequiredService<ICommonComponentRenderers>(),
				commandArgs.ServiceProvider.GetRequiredService<IDispatcher>(),
		        TimeSpan.FromSeconds(7));
    	}
    	else
    	{
    		viewModelModifier.ViewModel.OnSaveRequested.Invoke(modelModifier);
        	modelModifier.SetIsDirtyFalse();
    	}
    }

    public static void SelectAll(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
    	
    	primaryCursorModifier.SelectionAnchorPositionIndex = 0;
        primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.CharCount;
    }

    public static void Undo(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        modelModifier.UndoEdit();
    }

    public static void Redo(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
		modelModifier.RedoEdit();
    }

    public static void TriggerRemeasure(
        ITextEditorEditContext editContext,
        TextEditorViewModelModifier viewModelModifier,
        TextEditorCommandArgs commandArgs)
    {
        editContext.TextEditorService.OptionsApi.SetRenderStateKey(Key<RenderState>.NewKey());
    }

    public static void ScrollLineDown(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        editContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
    		editContext,
	        viewModelModifier,
	        viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight);
    }

    public static void ScrollLineUp(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        editContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
            editContext,
	        viewModelModifier,
	        -1 * viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight);
    }

    public static void ScrollPageDown(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        editContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
            editContext,
	        viewModelModifier,
	        viewModelModifier.ViewModel.TextEditorDimensions.Height);
    }

    public static void ScrollPageUp(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        editContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
            editContext,
	        viewModelModifier,
	        -1 * viewModelModifier.ViewModel.TextEditorDimensions.Height);
    }

    public static void CursorMovePageBottom(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        if (viewModelModifier.ViewModel.VirtualizationResult?.EntryList.Any() ?? false)
        {
			var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
        
            var lastEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryList.Last();
            var lastEntriesRowLength = modelModifier.GetLineLength(lastEntry.Index);

            primaryCursorModifier.LineIndex = lastEntry.Index;
            primaryCursorModifier.ColumnIndex = lastEntriesRowLength;
        }
    }

    public static void CursorMovePageTop(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        if (viewModelModifier.ViewModel.VirtualizationResult?.EntryList.Any() ?? false)
        {
        	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
        
            var firstEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryList.First();

            primaryCursorModifier.LineIndex = firstEntry.Index;
            primaryCursorModifier.ColumnIndex = 0;
        }
    }

    public static void Duplicate(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
    
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
            new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, new List<TextEditorCursorModifier>() { new(cursorForInsertion) }),
            cancellationToken: CancellationToken.None);
    }

    public static void IndentMore(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
    
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
                new List<TextEditorCursorModifier> { new TextEditorCursorModifier(insertionCursor) });

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
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
    
        var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(primaryCursorModifier);

        var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
            modelModifier,
            selectionBoundsInPositionIndexUnits);

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
                    new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, new List<TextEditorCursorModifier> { new(cursorForDeletion) }),
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
                    new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, new List<TextEditorCursorModifier> { new(cursorForDeletion) }),
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
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
        primaryCursorModifier.SelectionAnchorPositionIndex = null;
    }

    public static void NewLineBelow(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
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
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
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
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
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
		        new List<TextEditorCursorModifier> { primaryCursorModifier });

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
		        new List<TextEditorCursorModifier> { primaryCursorModifier });

			modelModifier.Delete(
		        innerCursorModifierBag,
		        columnCount,
		        false,
		        TextEditorModelModifier.DeleteKind.Delete);
		}
		
		primaryCursorModifier.LineIndex = lineIndexOriginal + 1;
		primaryCursorModifier.ColumnIndex = 0;
    }
    
    public static void MoveLineUp(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
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
		        new List<TextEditorCursorModifier> { primaryCursorModifier });

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
		        new List<TextEditorCursorModifier> { primaryCursorModifier });

			modelModifier.Delete(
		        innerCursorModifierBag,
		        columnCount,
		        false,
		        TextEditorModelModifier.DeleteKind.Delete);
		}
		
		primaryCursorModifier.LineIndex = lineIndexOriginal - 1;
		primaryCursorModifier.ColumnIndex = 0;
    }

    public static void GoToMatchingCharacter(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
        var cursorPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);

        if (commandArgs.ShouldSelectText)
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
                    ShiftKey = commandArgs.ShouldSelectText,
                };
            }
            else
            {
                keymapArgs = new KeymapArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    ShiftKey = commandArgs.ShouldSelectText,
                };
            }

            editContext.TextEditorService.ViewModelApi.MoveCursorUnsafe(
        		keymapArgs,
		        editContext,
		        modelModifier,
		        viewModelModifier,
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

        if (commandArgs.ShouldSelectText)
            primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
    }

    public static async Task RelatedFilesQuickPick(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        var jsRuntime = commandArgs.ServiceProvider.GetRequiredService<IJSRuntime>();
        var jsRuntimeCommonApi = jsRuntime.GetLuthetusCommonApi();
		       
		var cursorDimensions = await jsRuntimeCommonApi
			.MeasureElementById(viewModelModifier.ViewModel.PrimaryCursorContentId)
			.ConfigureAwait(false);

        var environmentProvider = commandArgs.ServiceProvider.GetRequiredService<IEnvironmentProvider>();
        
		var resourceAbsolutePath = environmentProvider.AbsolutePathFactory(modelModifier.ResourceUri.Value, false);
		var parentDirectoryAbsolutePath = environmentProvider.AbsolutePathFactory(resourceAbsolutePath.ParentDirectory.Value, true);
	
		var fileSystemProvider = commandArgs.ServiceProvider.GetRequiredService<IFileSystemProvider>();
		
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
				OnClickFunc: async () => commandArgs.TextEditorService.OpenInEditorAsync(
					file,
					true,
					null,
					new Category("main"),
					Key<TextEditorViewModel>.NewKey())));
					
			if (siblingAbsolutePath.NameWithExtension == resourceAbsolutePath.NameWithExtension)
				initialActiveMenuOptionRecordIndex = i;
		}
		
		MenuRecord menu;
		
		if (menuOptionList.Count == 0)
			menu = MenuRecord.Empty;
		else
			menu = new MenuRecord(menuOptionList.ToImmutableArray());
		
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
			() => 
			{
				// TODO: Even if this '.single or default' to get the main group works it is bad and I am ashamed...
				//       ...I'm too tired at the moment, need to make this sensible.
				//	   The key is in the IDE project yet its circular reference if I do so, gotta
				//       make groups more sensible I'm not sure what to say here I'm super tired and brain checked out.
				//       |
				//       I ran this and it didn't work. Its for the best that it doesn't.
				//	   maybe when I wake up tomorrow I'll realize what im doing here.
				var mainEditorGroup = commandArgs.TextEditorService.GroupStateWrap.Value.GroupList.SingleOrDefault();
				
				if (mainEditorGroup is not null &&
					mainEditorGroup.ActiveViewModelKey != Key<TextEditorViewModel>.Empty)
				{
					var activeViewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(mainEditorGroup.ActiveViewModelKey);

					if (activeViewModel is not null)
						return activeViewModel.FocusAsync();
				}
				
				return viewModelModifier.ViewModel.FocusAsync();
			});

		var dispatcher = commandArgs.ServiceProvider.GetRequiredService<IDispatcher>();
        dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
    }
    
    public static async Task QuickActionsSlashRefactor(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        var jsRuntime = commandArgs.ServiceProvider.GetRequiredService<IJSRuntime>();
        var jsRuntimeCommonApi = jsRuntime.GetLuthetusCommonApi();
		       
		var cursorDimensions = await jsRuntimeCommonApi
			.MeasureElementById(viewModelModifier.ViewModel.PrimaryCursorContentId)
			.ConfigureAwait(false);

        var environmentProvider = commandArgs.ServiceProvider.GetRequiredService<IEnvironmentProvider>();
        
		var resourceAbsolutePath = environmentProvider.AbsolutePathFactory(modelModifier.ResourceUri.Value, false);
		var parentDirectoryAbsolutePath = environmentProvider.AbsolutePathFactory(resourceAbsolutePath.ParentDirectory.Value, true);
	
		var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
		var compilerService = modelModifier.CompilerService;
	
		var compilerServiceResource = viewModelModifier is null
			? null
			: compilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri);

		int? primaryCursorPositionIndex = modelModifier is null || viewModelModifier is null
			? null
			: modelModifier.GetPositionIndex(primaryCursorModifier);

		var syntaxNode = primaryCursorPositionIndex is null || compilerService.Binder is null || compilerServiceResource?.CompilationUnit is null
			? null
			: compilerService.Binder.GetSyntaxNode(primaryCursorPositionIndex.Value, compilerServiceResource.ResourceUri);
			
		var menuOptionList = new List<MenuOptionRecord>();
			
		menuOptionList.Add(new MenuOptionRecord(
			"QuickActionsSlashRefactorMenu",
			MenuOptionKind.Other));
			
		if (syntaxNode is null)
		{
			menuOptionList.Add(new MenuOptionRecord(
				"syntaxNode was null",
				MenuOptionKind.Other,
				OnClickFunc: async () => {}));
		}
		else
		{
			/*
			
			// The ISyntaxNode.Parent property is being removed. (2024-11-11)
	    	// ==============================================================
	    	// TODO: Rewrite this block of code but find the parent node by "querying" the Binder.
			
			menuOptionList.Add(new MenuOptionRecord(
				syntaxNode.SyntaxKind.ToString(),
				MenuOptionKind.Other,
				OnClickFunc: async () => {}));
				
			if (syntaxNode.SyntaxKind == SyntaxKind.PropertyDefinitionNode)
			{
				if (syntaxNode.Parent is null)
				{
					menuOptionList.Add(new MenuOptionRecord(
						"syntaxNode.Parent is null",
						MenuOptionKind.Other,
						OnClickFunc: async () => {}));
				}
				else
				{
					if (syntaxNode.Parent is TypeDefinitionNode typeDefinitionNode)
					{
						menuOptionList.Add(new MenuOptionRecord(
							$"Add to {typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText()}() constructor",
							MenuOptionKind.Other,
							OnClickFunc: () => 
							{
								TextEditorRefactorFacts.GenerateConstructor(
									typeDefinitionNode,
									Array.Empty<IVariableDeclarationNode>(),
									commandArgs.ServiceProvider,
									editContext.TextEditorService,
							        modelModifier.ResourceUri,
							        viewModelModifier.ViewModel.ViewModelKey);
							    return Task.CompletedTask;
							}));
					}
					else
					{
						menuOptionList.Add(new MenuOptionRecord(
							$"Parent is not {nameof(SyntaxKind)}.{nameof(SyntaxKind.TypeDefinitionNode)} it is: {nameof(SyntaxKind)}.{syntaxNode.Parent.SyntaxKind}",
							MenuOptionKind.Other,
							OnClickFunc: async () => {}));
					}
				}
			}
			*/
		}
		
		MenuRecord menu;
		
		if (menuOptionList.Count == 0)
			menu = MenuRecord.Empty;
		else
			menu = new MenuRecord(menuOptionList.ToImmutableArray());
		
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
			() => 
			{
				// TODO: Even if this '.single or default' to get the main group works it is bad and I am ashamed...
				//       ...I'm too tired at the moment, need to make this sensible.
				//	   The key is in the IDE project yet its circular reference if I do so, gotta
				//       make groups more sensible I'm not sure what to say here I'm super tired and brain checked out.
				//       |
				//       I ran this and it didn't work. Its for the best that it doesn't.
				//	   maybe when I wake up tomorrow I'll realize what im doing here.
				var mainEditorGroup = commandArgs.TextEditorService.GroupStateWrap.Value.GroupList.SingleOrDefault();
				
				if (mainEditorGroup is not null &&
					mainEditorGroup.ActiveViewModelKey != Key<TextEditorViewModel>.Empty)
				{
					var activeViewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(mainEditorGroup.ActiveViewModelKey);

					if (activeViewModel is not null)
						return activeViewModel.FocusAsync();
				}
				
				return viewModelModifier.ViewModel.FocusAsync();
			});

		var dispatcher = commandArgs.ServiceProvider.GetRequiredService<IDispatcher>();
        dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
    }
    
    public static void GoToDefinition(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        if (modelModifier.CompilerService.Binder is null)
            return;
            
        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

        var positionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
        
        var wordTextSpan = modelModifier.GetWordTextSpan(positionIndex);
        if (wordTextSpan is null)
            return;

		var compilerServiceResource = modelModifier.CompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri);
		if (compilerServiceResource?.CompilationUnit is null)
			return;

        var definitionTextSpan = modelModifier.CompilerService.Binder.GetDefinitionTextSpan(wordTextSpan.Value, compilerServiceResource);
        if (definitionTextSpan is null)
            return;

        var definitionModel = commandArgs.TextEditorService.ModelApi.GetOrDefault(definitionTextSpan.Value.ResourceUri);
        if (definitionModel is null)
        {
            if (commandArgs.TextEditorService.TextEditorConfig.RegisterModelFunc is not null)
            {
                commandArgs.TextEditorService.TextEditorConfig.RegisterModelFunc.Invoke(
                    new RegisterModelArgs(definitionTextSpan.Value.ResourceUri, commandArgs.ServiceProvider));

                var definitionModelModifier = editContext.GetModelModifier(definitionTextSpan.Value.ResourceUri);

                if (definitionModel is null) // TODO: Should this be null checking instead: 'definitionModelModifier'?
                    return;
            }
            else
            {
                return;
            }
        }

        var definitionViewModels = commandArgs.TextEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.Value.ResourceUri);

        if (!definitionViewModels.Any())
        {
            if (commandArgs.TextEditorService.TextEditorConfig.TryRegisterViewModelFunc is not null)
            {
                commandArgs.TextEditorService.TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
                    Key<TextEditorViewModel>.NewKey(),
                    definitionTextSpan.Value.ResourceUri,
                    new Category("main"),
                    true,
                    commandArgs.ServiceProvider));

                definitionViewModels = commandArgs.TextEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.Value.ResourceUri);

                if (!definitionViewModels.Any())
                    return;
            }
            else
            {
                return;
            }
        }

        var definitionViewModelKey = definitionViewModels.First().ViewModelKey;
        
        var definitionViewModelModifier = editContext.GetViewModelModifier(definitionViewModelKey);
        var definitionCursorModifierBag = editContext.GetCursorModifierBag(definitionViewModelModifier?.ViewModel);
        var definitionPrimaryCursorModifier = editContext.GetPrimaryCursorModifier(definitionCursorModifierBag);

        if (definitionViewModelModifier is null || definitionCursorModifierBag is null || definitionPrimaryCursorModifier is null)
            return;

        var rowData = definitionModel.GetLineInformationFromPositionIndex(definitionTextSpan.Value.StartingIndexInclusive);
        var columnIndex = definitionTextSpan.Value.StartingIndexInclusive - rowData.StartPositionIndexInclusive;

        definitionPrimaryCursorModifier.SelectionAnchorPositionIndex = null;
        definitionPrimaryCursorModifier.LineIndex = rowData.Index;
        definitionPrimaryCursorModifier.ColumnIndex = columnIndex;
        definitionPrimaryCursorModifier.PreferredColumnIndex = columnIndex;

        if (commandArgs.TextEditorService.TextEditorConfig.TryShowViewModelFunc is not null)
        {
            commandArgs.TextEditorService.TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
                definitionViewModelKey,
                Key<TextEditorGroup>.Empty,
                true,
                commandArgs.ServiceProvider));
        }
    }

    public static void ShowFindAllDialog(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        commandArgs.TextEditorService.OptionsApi.ShowFindAllDialog();
    }

    public static async Task ShowTooltipByCursorPositionAsync(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        var elementPositionInPixels = await commandArgs.TextEditorService.JsRuntimeTextEditorApi
            .GetBoundingClientRect(viewModelModifier.ViewModel.PrimaryCursorContentId)
            .ConfigureAwait(false);

        elementPositionInPixels = elementPositionInPixels with
        {
            Top = elementPositionInPixels.Top +
                (.9 * viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight)
        };

        await HandleMouseStoppedMovingEventAsync(
        		editContext,
        		modelModifier,
        		viewModelModifier,
				new MouseEventArgs
	            {
	                ClientX = elementPositionInPixels.Left,
	                ClientY = elementPositionInPixels.Top
	            },
				commandArgs.ComponentData,
				commandArgs.ServiceProvider.GetRequiredService<ILuthetusTextEditorComponentRenderers>(),
				modelModifier.ResourceUri)
			.ConfigureAwait(false);
    }

	/// <summary>The default <see cref="AfterOnKeyDownAsync"/> will provide syntax highlighting, and autocomplete.<br/><br/>The syntax highlighting occurs on ';', whitespace, paste, undo, redo<br/><br/>The autocomplete occurs on LetterOrDigit typed or { Ctrl + Space }. Furthermore, the autocomplete is done via <see cref="IAutocompleteService"/> and the one can provide their own implementation when registering the Luthetus.TextEditor services using <see cref="LuthetusTextEditorConfig.AutocompleteServiceFactory"/></summary>
	public static async Task HandleAfterOnKeyDownAsync(
		ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        KeymapArgs keymapArgs,
		TextEditorComponentData componentData)
    {
        // Indexing can be invoked and this method still check for syntax highlighting and such
        if (EventUtils.IsAutocompleteIndexerInvoker(keymapArgs))
        {
            try
            {
				var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

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
		        viewModelModifier,
		        cursorModifierBag,
		        editContext.GetPrimaryCursorModifier(cursorModifierBag),
		        componentData.Dispatcher,
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
	public static async Task HandleAfterOnKeyDownRangeAsync(
		ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
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
		        viewModelModifier,
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
        	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
        	
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
		        viewModelModifier,
		        cursorModifierBag,
		        editContext.GetPrimaryCursorModifier(cursorModifierBag),
		        componentData.Dispatcher,
		        componentData);
        }

        if (seenIsSyntaxHighlightingInvoker)
        {
            componentData.ThrottleApplySyntaxHighlighting(modelModifier);
        }
    }

	public static async Task HandleMouseStoppedMovingEventAsync(
		ITextEditorEditContext editContext,
		TextEditorModelModifier modelModifier,
		TextEditorViewModelModifier viewModelModifier,
		MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
		ILuthetusTextEditorComponentRenderers textEditorComponentRenderers,
        ResourceUri resourceUri)
    {
    	// Lazily calculate row and column index a second time. Otherwise one has to calculate it every mouse moved event.
        var rowAndColumnIndex = await EventUtils.CalculateRowAndColumnIndex(
				resourceUri,
				viewModelModifier.ViewModel.ViewModelKey,
				mouseEventArgs,
				componentData,
				editContext)
			.ConfigureAwait(false);

		var textEditorDimensions = viewModelModifier.ViewModel.TextEditorDimensions;
		var scrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions;
	
		var relativeCoordinatesOnClick = new RelativeCoordinates(
		    mouseEventArgs.ClientX - textEditorDimensions.BoundingClientRectLeft,
		    mouseEventArgs.ClientY - textEditorDimensions.BoundingClientRectTop,
		    scrollbarDimensions.ScrollLeft,
		    scrollbarDimensions.ScrollTop);

        var cursorPositionIndex = modelModifier.GetPositionIndex(new TextEditorCursor(
            rowAndColumnIndex.rowIndex,
            rowAndColumnIndex.columnIndex,
            true));

        var foundMatch = false;

        var symbols = modelModifier.CompilerService.GetSymbolsFor(modelModifier.ResourceUri);
        var diagnostics = modelModifier.CompilerService.GetDiagnosticsFor(modelModifier.ResourceUri);

        if (diagnostics.Length != 0)
        {
            foreach (var diagnostic in diagnostics)
            {
                if (cursorPositionIndex >= diagnostic.TextSpan.StartingIndexInclusive &&
                    cursorPositionIndex < diagnostic.TextSpan.EndingIndexExclusive)
                {
                    // Prefer showing a diagnostic over a symbol when both exist at the mouse location.
                    foundMatch = true;

                    var parameterMap = new Dictionary<string, object?>
                    {
                        {
                            nameof(ITextEditorDiagnosticRenderer.Diagnostic),
                            diagnostic
                        }
                    };

                    viewModelModifier.ViewModel = viewModelModifier.ViewModel with
					{
						TooltipViewModel = new(
		                    modelModifier.CompilerService.DiagnosticRendererType ?? textEditorComponentRenderers.DiagnosticRendererType,
		                    parameterMap,
		                    relativeCoordinatesOnClick,
		                    null,
		                    componentData.ContinueRenderingTooltipAsync)
					};
                }
            }
        }

        if (!foundMatch && symbols.Length != 0)
        {
            foreach (var symbol in symbols)
            {
                if (cursorPositionIndex >= symbol.TextSpan.StartingIndexInclusive &&
                    cursorPositionIndex < symbol.TextSpan.EndingIndexExclusive)
                {
                    foundMatch = true;

                    var parameters = new Dictionary<string, object?>
                    {
                        {
                            nameof(ITextEditorSymbolRenderer.Symbol),
                            symbol
                        }
                    };

                    viewModelModifier.ViewModel = viewModelModifier.ViewModel with
					{
						TooltipViewModel = new(
	                        modelModifier.CompilerService.SymbolRendererType ?? textEditorComponentRenderers.SymbolRendererType,
	                        parameters,
	                        relativeCoordinatesOnClick,
	                        null,
	                        componentData.ContinueRenderingTooltipAsync)
					};
                }
            }
        }

        if (!foundMatch)
        {
			viewModelModifier.ViewModel = viewModelModifier.ViewModel with
			{
            	TooltipViewModel = null
			};
        }

        // TODO: Measure the tooltip, and reposition if it would go offscreen.
    }
    
    /// <summary>
    /// If left or top is left as null, it will be set to whatever the primary cursor's left or top is respectively.
    /// The left and top values are offsets from the text editor's bounding client rect.
    ///
    /// Use the method <see cref="RemoveDropdown"/> to un-render the dropdown programmatically.
    /// </summary>
    public static void ShowDropdown(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier primaryCursor,
        IDispatcher dispatcher,
        double? leftOffset,
        double? topOffset,
        Type componentType,
        Dictionary<string, object?>? componentParameters)
    {
        var dropdownKey = new Key<DropdownRecord>(viewModelModifier.ViewModel.ViewModelKey.Guid);
        
        if (leftOffset is null)
        {
        	leftOffset = primaryCursor.ColumnIndex * viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth;
	        
	        // Tab key column offset
            var tabsOnSameRowBeforeCursor = modelModifier.GetTabCountOnSameLineBeforeCursor(
                primaryCursor.LineIndex,
                primaryCursor.ColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            leftOffset += extraWidthPerTabKey *
                tabsOnSameRowBeforeCursor *
                viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth;
                
            leftOffset -= viewModelModifier.ViewModel.ScrollbarDimensions.ScrollLeft;
        }
        
        topOffset ??= (primaryCursor.LineIndex + 1) *
        	viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight -
        	viewModelModifier.ViewModel.ScrollbarDimensions.ScrollTop;
		
		var dropdownRecord = new DropdownRecord(
			dropdownKey,
			viewModelModifier.ViewModel.TextEditorDimensions.BoundingClientRectLeft + leftOffset.Value,
			viewModelModifier.ViewModel.TextEditorDimensions.BoundingClientRectTop + topOffset.Value,
			componentType,
			componentParameters,
			viewModelModifier.ViewModel.FocusAsync)
		{
			ShouldShowOutOfBoundsClickDisplay = false
		};

        dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
	}
	
	public static void RemoveDropdown(
        ITextEditorEditContext editContext,
        TextEditorViewModelModifier viewModelModifier,
        IDispatcher dispatcher)
    {
    	viewModelModifier.ViewModel = viewModelModifier.ViewModel with
		{
			MenuKind = MenuKind.None
		};
    
		var dropdownKey = new Key<DropdownRecord>(viewModelModifier.ViewModel.ViewModelKey.Guid);
		dispatcher.Dispatch(new DropdownState.DisposeAction(dropdownKey));
	}
	
	public static void ShowContextMenu(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier primaryCursor,
        IDispatcher dispatcher,
        TextEditorComponentData componentData)
    {
    	viewModelModifier.ViewModel = viewModelModifier.ViewModel with
		{
			MenuKind = MenuKind.ContextMenu
		};
    
    	ShowDropdown(
    		editContext,
	        modelModifier,
	        viewModelModifier,
	        cursorModifierBag,
	        primaryCursor,
	        dispatcher,
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
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier primaryCursor,
        IDispatcher dispatcher,
        TextEditorComponentData componentData)
    {
    	viewModelModifier.ViewModel = viewModelModifier.ViewModel with
		{
			MenuKind = MenuKind.AutoCompleteMenu
		};
    
    	ShowDropdown(
    		editContext,
	        modelModifier,
	        viewModelModifier,
	        cursorModifierBag,
	        primaryCursor,
	        dispatcher,
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
}
