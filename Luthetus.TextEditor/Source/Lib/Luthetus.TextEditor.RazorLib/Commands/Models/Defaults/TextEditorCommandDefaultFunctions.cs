using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public class TextEditorCommandDefaultFunctions
{
    public static TextEditorEdit DoNothingDiscardFactory()
    {
        return (ITextEditorEditContext editContext) =>
        {
            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit CopyFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return async (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(
                primaryCursorModifier,
                modelModifier);

            selectedText ??= modelModifier.GetLinesRange(
                primaryCursorModifier.RowIndex,
                1);

            await commandArgs.ClipboardService.SetClipboard(selectedText);
            viewModelModifier.ViewModel.Focus();
        };
    }

    public static TextEditorEdit CutFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return async (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(
                primaryCursorModifier,
                modelModifier);

            if (selectedText is null)
                return; // Should never occur

            await commandArgs.ClipboardService.SetClipboard(selectedText);
            viewModelModifier.ViewModel.Focus();

            modelModifier.HandleKeyboardEvent(
                new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
                cursorModifierBag,
                CancellationToken.None);
        };
    }

    public static TextEditorEdit PasteFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return async (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return;

            var clipboard = await commandArgs.ClipboardService.ReadClipboard();

            modelModifier.EditByInsertion(
                clipboard,
                cursorModifierBag,
                CancellationToken.None);
        };
    }

    public static TextEditorEdit SaveFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            var onSaveRequestedFunc = viewModelModifier.ViewModel.OnSaveRequested;

            if (onSaveRequestedFunc is not null)
                onSaveRequestedFunc.Invoke(modelModifier);

            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit SelectAllFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            primaryCursorModifier.SelectionAnchorPositionIndex = 0;
            primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.DocumentLength;

            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit UndoFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            return editContext.TextEditorService.ModelApi
                .UndoEditFactory(modelResourceUri)
                .Invoke(editContext);
        };
    }

    public static TextEditorEdit RedoFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            return editContext.TextEditorService.ModelApi
                .RedoEditFactory(modelResourceUri)
                .Invoke(editContext);
        };
    }

    public static TextEditorEdit RemeasureFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            editContext.TextEditorService.OptionsApi.SetRenderStateKey(
                Key<RenderState>.NewKey());

            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit ScrollLineDownFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return async (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return;

            viewModelModifier.ViewModel.MutateScrollVerticalPositionByLines(1);
        };
    }

    public static TextEditorEdit ScrollLineUpFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return async (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return;

            viewModelModifier.ViewModel.MutateScrollVerticalPositionByLines(-1);
        };
    }

    public static TextEditorEdit ScrollPageDownFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return async (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return;

            viewModelModifier.ViewModel.MutateScrollVerticalPositionByPages(1);
        };
    }

    public static TextEditorEdit ScrollPageUpFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return async (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return;

            viewModelModifier.ViewModel.MutateScrollVerticalPositionByPages(-1);
        };
    }

    public static TextEditorEdit CursorMovePageBottomFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            if (viewModelModifier.ViewModel.VirtualizationResult?.EntryBag.Any() ?? false)
            {
                var lastEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryBag.Last();
                var lastEntriesRowLength = modelModifier.GetLengthOfRow(lastEntry.Index);

                primaryCursorModifier.RowIndex = lastEntry.Index;
                primaryCursorModifier.ColumnIndex = lastEntriesRowLength;
            }

            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit CursorMovePageTopFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            if (viewModelModifier.ViewModel.VirtualizationResult?.EntryBag.Any() ?? false)
            {
                var firstEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryBag.First();

                primaryCursorModifier.RowIndex = firstEntry.Index;
                primaryCursorModifier.ColumnIndex = 0;
            }

            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit DuplicateFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(
                primaryCursorModifier,
                modelModifier);

            TextEditorCursor cursorForInsertion;

            if (selectedText is null)
            {
                // Select line
                selectedText = modelModifier.GetLinesRange(primaryCursorModifier.RowIndex, 1);

                cursorForInsertion = new TextEditorCursor(
                    primaryCursorModifier.RowIndex,
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

            modelModifier.EditByInsertion(
                selectedText,
                new TextEditorCursorModifierBag(Key<TextEditorViewModel>.Empty, new List<TextEditorCursorModifier>() { new(cursorForInsertion) }),
                CancellationToken.None);

            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit IndentMoreFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
                primaryCursorModifier);

            var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                modelModifier,
                selectionBoundsInPositionIndexUnits);

            for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
                 i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
                 i++)
            {
                var cursorForInsertion = new TextEditorCursor(i, 0, true);

                modelModifier.EditByInsertion(
                    KeyboardKeyFacts.WhitespaceCharacters.TAB.ToString(),
                    cursorModifierBag,
                    CancellationToken.None);
            }

            var lowerBoundPositionIndexChange = 1;

            var upperBoundPositionIndexChange = selectionBoundsInRowIndexUnits.upperRowIndexExclusive -
                selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;

            if (primaryCursorModifier.SelectionAnchorPositionIndex <
                primaryCursorModifier.SelectionEndingPositionIndex)
            {
                primaryCursorModifier.SelectionAnchorPositionIndex +=
                    lowerBoundPositionIndexChange;

                primaryCursorModifier.SelectionEndingPositionIndex +=
                    upperBoundPositionIndexChange;
            }
            else
            {
                primaryCursorModifier.SelectionAnchorPositionIndex +=
                    upperBoundPositionIndexChange;

                primaryCursorModifier.SelectionEndingPositionIndex +=
                    lowerBoundPositionIndexChange;
            }

            var userCursorRowIndex = primaryCursorModifier.RowIndex;
            var userCursorColumnIndex = primaryCursorModifier.ColumnIndex;

            primaryCursorModifier.RowIndex = userCursorRowIndex;
            primaryCursorModifier.ColumnIndex = userCursorColumnIndex + 1;

            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit IndentLessFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
                primaryCursorModifier);

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
                var lengthOfRow = modelModifier.GetLengthOfRow(i);

                characterReadCount = Math.Min(lengthOfRow, characterReadCount);

                var readResult = modelModifier.GetString(rowPositionIndex, characterReadCount);
                var removeCharacterCount = 0;

                if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.TAB))
                {
                    removeCharacterCount = 1;

                    var cursorForDeletion = new TextEditorCursor(i, 0, true);

                    modelModifier.DeleteByRange(
                        removeCharacterCount, // Delete a single "Tab" character
                        new TextEditorCursorModifierBag(Key<TextEditorViewModel>.Empty, new List<TextEditorCursorModifier> { new(cursorForDeletion) }),
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
                        new TextEditorCursorModifierBag(Key<TextEditorViewModel>.Empty, new List<TextEditorCursorModifier> { new(cursorForDeletion) }),
                        CancellationToken.None);
                }

                // Modify the lower bound of user's text selection
                if (isFirstLoop)
                {
                    isFirstLoop = false;

                    if (primaryCursorModifier.SelectionAnchorPositionIndex <
                        primaryCursorModifier.SelectionEndingPositionIndex)
                    {
                        primaryCursorModifier.SelectionAnchorPositionIndex -=
                            removeCharacterCount;
                    }
                    else
                    {
                        primaryCursorModifier.SelectionEndingPositionIndex -=
                            removeCharacterCount;
                    }
                }

                // Modify the upper bound of user's text selection
                if (primaryCursorModifier.SelectionAnchorPositionIndex <
                    primaryCursorModifier.SelectionEndingPositionIndex)
                {
                    primaryCursorModifier.SelectionEndingPositionIndex -=
                        removeCharacterCount;
                }
                else
                {
                    primaryCursorModifier.SelectionAnchorPositionIndex -=
                        removeCharacterCount;
                }

                // Modify the column index of user's cursor
                if (i == primaryCursorModifier.RowIndex)
                {
                    var nextColumnIndex = primaryCursorModifier.ColumnIndex -
                        removeCharacterCount;

                    primaryCursorModifier.RowIndex = primaryCursorModifier.RowIndex;
                    primaryCursorModifier.ColumnIndex = Math.Max(0, nextColumnIndex);
                }
            }

            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit ClearTextSelectionFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            primaryCursorModifier.SelectionAnchorPositionIndex = null;
            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit NewLineBelowFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            primaryCursorModifier.SelectionAnchorPositionIndex = null;

            var lengthOfRow = modelModifier.GetLengthOfRow(primaryCursorModifier.RowIndex);

            primaryCursorModifier.RowIndex = primaryCursorModifier.RowIndex;
            primaryCursorModifier.ColumnIndex = lengthOfRow;

            modelModifier.EditByInsertion(
                "\n",
                cursorModifierBag,
                CancellationToken.None);

            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit NewLineAboveFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            primaryCursorModifier.SelectionAnchorPositionIndex = null;

            primaryCursorModifier.RowIndex = primaryCursorModifier.RowIndex;
            primaryCursorModifier.ColumnIndex = 0;

            modelModifier.EditByInsertion(
                "\n",
                cursorModifierBag,
                CancellationToken.None);

            if (primaryCursorModifier.RowIndex > 1)
            {
                primaryCursorModifier.RowIndex--;
                primaryCursorModifier.ColumnIndex = 0;
            }

            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit GoToMatchingCharacterFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return async (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return;

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

            var directionToFindMatchingPunctuationCharacter =
                KeyboardKeyFacts.DirectionToFindMatchingPunctuationCharacter(characterToMatch.Value);

            if (directionToFindMatchingPunctuationCharacter is null)
                return;

            var temporaryCursor = new TextEditorCursor(
                primaryCursorModifier.RowIndex,
                primaryCursorModifier.ColumnIndex,
                primaryCursorModifier.IsPrimaryCursor)
            {
                PreferredColumnIndex = primaryCursorModifier.PreferredColumnIndex,
            };

            var unmatchedCharacters =
                fallbackToPreviousCharacter && directionToFindMatchingPunctuationCharacter == -1
                    ? 0
                    : 1;

            while (true)
            {
                KeyboardEventArgs keyboardEventArgs;

                if (directionToFindMatchingPunctuationCharacter == -1)
                {
                    keyboardEventArgs = new KeyboardEventArgs
                    {
                        Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                    };
                }
                else
                {
                    keyboardEventArgs = new KeyboardEventArgs
                    {
                        Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    };
                }

                await editContext.TextEditorService.ViewModelApi.MoveCursorUnsafeFactory(
                        keyboardEventArgs,
                        modelModifier.ResourceUri,
                        viewModelModifier.ViewModel.ViewModelKey,
                        primaryCursorModifier)
                    .Invoke(editContext);

                var temporaryCursorPositionIndex = modelModifier.GetPositionIndex(
                    temporaryCursor);

                var characterAt = modelModifier.GetCharacter(temporaryCursorPositionIndex);

                if (characterAt == match)
                    unmatchedCharacters--;
                else if (characterAt == characterToMatch)
                    unmatchedCharacters++;

                if (unmatchedCharacters == 0)
                    break;

                if (temporaryCursorPositionIndex <= 0 ||
                    temporaryCursorPositionIndex >= modelModifier.DocumentLength)
                    break;
            }

            if (commandArgs.ShouldSelectText)
            {
                primaryCursorModifier.SelectionEndingPositionIndex =
                    modelModifier.GetPositionIndex(temporaryCursor);
            }

            primaryCursorModifier.RowIndex = temporaryCursor.RowIndex;
            primaryCursorModifier.ColumnIndex = temporaryCursor.ColumnIndex;
        };
    }

    public static TextEditorEdit GoToDefinitionFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            if (modelModifier.CompilerService.Binder is null)
                return Task.CompletedTask;

            var positionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
            var wordTextSpan = modelModifier.GetWordAt(positionIndex);

            if (wordTextSpan is null)
                return Task.CompletedTask;

            var definitionTextSpan = modelModifier.CompilerService.Binder.GetDefinition(
                wordTextSpan);

            if (definitionTextSpan is null)
                return Task.CompletedTask;

            var definitionModel = commandArgs.TextEditorService.ModelApi.GetOrDefault(definitionTextSpan.ResourceUri);

            if (definitionModel is null)
            {
                if (commandArgs.RegisterModelAction is not null)
                {
                    commandArgs.RegisterModelAction.Invoke(definitionTextSpan.ResourceUri);
                    definitionModel = commandArgs.TextEditorService.ModelApi.GetOrDefault(definitionTextSpan.ResourceUri);

                    if (definitionModel is null)
                        return Task.CompletedTask;
                }
                else
                {
                    return Task.CompletedTask;
                }
            }

            var definitionViewModels = commandArgs.TextEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

            if (!definitionViewModels.Any())
            {
                if (commandArgs.RegisterViewModelAction is not null)
                {
                    commandArgs.RegisterViewModelAction.Invoke(definitionTextSpan.ResourceUri);
                    definitionViewModels = commandArgs.TextEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

                    if (!definitionViewModels.Any())
                        return Task.CompletedTask;
                }
                else
                {
                    return Task.CompletedTask;
                }
            }

            var firstDefinitionViewModel = definitionViewModels.First();
            var rowData = definitionModel.FindRowInformation(definitionTextSpan.StartingIndexInclusive);
            var columnIndex = definitionTextSpan.StartingIndexInclusive - rowData.rowStartPositionIndex;

            var firstDefinitionViewModelCursorModifier = new TextEditorCursorModifier(firstDefinitionViewModel.PrimaryCursor);

            firstDefinitionViewModelCursorModifier.RowIndex = rowData.rowIndex;
            firstDefinitionViewModelCursorModifier.ColumnIndex = columnIndex;
            firstDefinitionViewModelCursorModifier.PreferredColumnIndex = columnIndex;

            commandArgs.TextEditorService.ViewModelApi.WithValueFactory(
                viewModelModifier.ViewModel.ViewModelKey,
                firstDefinitionInViewModel =>
                {
                    var outCursor = firstDefinitionViewModelCursorModifier.ToCursor();
                    var outCursorBag = firstDefinitionInViewModel.CursorBag.Replace(firstDefinitionInViewModel.PrimaryCursor, outCursor);

                    return firstDefinitionInViewModel with
                    {
                        CursorBag = outCursorBag
                    };
                }).Invoke(editContext);

            if (commandArgs.ShowViewModelAction is not null)
                commandArgs.ShowViewModelAction.Invoke(firstDefinitionViewModel.ViewModelKey);

            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit ShowFindDialogFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            commandArgs.TextEditorService.OptionsApi.ShowFindDialog();
            return Task.CompletedTask;
        };
    }

    public static TextEditorEdit ShowTooltipByCursorPositionFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return async (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return;

            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (cursorModifierBag is null || primaryCursorModifier is null)
                return;

            if (commandArgs.JsRuntime is null || commandArgs.HandleMouseStoppedMovingEventAsyncFunc is null)
                return;

            var elementPositionInPixels = await commandArgs.JsRuntime.InvokeAsync<ElementPositionInPixels>(
                "luthetusTextEditor.getBoundingClientRect",
                viewModelModifier.ViewModel.PrimaryCursorContentId);

            elementPositionInPixels = elementPositionInPixels with
            {
                Top = elementPositionInPixels.Top +
                    (.9 * viewModelModifier.ViewModel.VirtualizationResult.CharAndRowMeasurements.RowHeight)
            };

            await commandArgs.HandleMouseStoppedMovingEventAsyncFunc.Invoke(new MouseEventArgs
            {
                ClientX = elementPositionInPixels.Left,
                ClientY = elementPositionInPixels.Top
            });
        };
    }
}
