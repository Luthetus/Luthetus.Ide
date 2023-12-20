using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Microsoft.AspNetCore.Components.Web;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public class TextEditorCommandDefaultFunctions
{
    public static TextEditorEdit DoNothingDiscardAsync = (ITextEditorEditContext editContext) =>
    {
        return Task.CompletedTask;
    };

    public static TextEditorEdit CopyAsync = async (ITextEditorEditContext editContext) =>
    {
        var selectedText = TextEditorSelectionHelper.GetSelectedText(
            editContext.PrimaryCursor,
            editContext.Model);

        selectedText ??= editContext.Model.GetLinesRange(
            editContext.PrimaryCursor.RowIndex,
            1);

        await editContext.CommandArgs.ClipboardService.SetClipboard(selectedText);
        await editContext.ViewModel.FocusAsync();
    };

    public static TextEditorEdit CutAsync = async (ITextEditorEditContext editContext) =>
    {
        var selectedText = TextEditorSelectionHelper.GetSelectedText(
            editContext.PrimaryCursor,
            editContext.Model);

        if (selectedText is null)
            return; // Should never occur

        await editContext.CommandArgs.ClipboardService.SetClipboard(selectedText);
        await editContext.ViewModel.FocusAsync();

        editContext.CommandArgs.Dispatcher.Dispatch(new KeyboardEventAction(
            editContext.Model.ResourceUri,
            editContext.ViewModel.ViewModelKey,
            editContext.RefreshCursorsRequest.CursorBag,
            new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
            CancellationToken.None,
            editContext.AuthenticatedActionKey));
    };

    public static TextEditorEdit PasteAsync = async (ITextEditorEditContext editContext) =>
    {
        var clipboard = await editContext.CommandArgs.ClipboardService.ReadClipboard();

        editContext.CommandArgs.Dispatcher.Dispatch(new InsertTextAction(
            editContext.Model.ResourceUri,
            editContext.ViewModel.ViewModelKey,
            editContext.RefreshCursorsRequest.CursorBag,
            clipboard,
            CancellationToken.None,
            editContext.AuthenticatedActionKey));
    };

    public static TextEditorEdit SaveAsync = (ITextEditorEditContext editContext) =>
    {
        var onSaveRequestedFunc = editContext.ViewModel.OnSaveRequested;

        if (onSaveRequestedFunc is not null)
        {
            onSaveRequestedFunc.Invoke(editContext.Model);

            editContext.CommandArgs.Dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                editContext.ViewModel.ViewModelKey,
                inState => inState with { })); // "with { }" is a Hack to re-render
        }

        return Task.CompletedTask;
    };

    public static TextEditorEdit SelectAllAsync = (ITextEditorEditContext editContext) =>
    {
        editContext.PrimaryCursor.SelectionAnchorPositionIndex = 0;
        editContext.PrimaryCursor.SelectionEndingPositionIndex = editContext.Model.DocumentLength;

        return Task.CompletedTask;
    };

    public static TextEditorEdit UndoAsync = (ITextEditorEditContext editContext) =>
    {
        return editContext.CommandArgs.TextEditorService.ModelApi
            .UndoEdit(editContext.CommandArgs.ModelResourceUri)
            .Invoke(editContext);
    };

    public static TextEditorEdit RedoAsync = (ITextEditorEditContext editContext) =>
    {
        return editContext.CommandArgs.TextEditorService.ModelApi
            .RedoEdit(editContext.CommandArgs.ModelResourceUri)
            .Invoke(editContext);
    };

    public static TextEditorEdit RemeasureAsync = (ITextEditorEditContext editContext) =>
    {
        editContext.CommandArgs.TextEditorService.OptionsApi.SetRenderStateKey(
            Key<RenderState>.NewKey());

        return Task.CompletedTask;
    };

    public static TextEditorEdit ScrollLineDownAsync = async (ITextEditorEditContext editContext) =>
    {
        await editContext.ViewModel.MutateScrollVerticalPositionByLinesAsync(1);
    };

    public static TextEditorEdit ScrollLineUpAsync = async (ITextEditorEditContext editContext) =>
    {
        await editContext.ViewModel.MutateScrollVerticalPositionByLinesAsync(-1);
    };

    public static TextEditorEdit ScrollPageDownAsync = async (ITextEditorEditContext editContext) =>
    {
        await editContext.ViewModel.MutateScrollVerticalPositionByPagesAsync(1);
    };

    public static TextEditorEdit ScrollPageUpAsync = async (ITextEditorEditContext editContext) =>
    {
        await editContext.ViewModel.MutateScrollVerticalPositionByPagesAsync(-1);
    };

    public static TextEditorEdit CursorMovePageBottomAsync = (ITextEditorEditContext editContext) =>
    {
        if (editContext.ViewModel.VirtualizationResult?.EntryBag.Any() ?? false)
        {
            var lastEntry = editContext.ViewModel.VirtualizationResult.EntryBag.Last();
            var lastEntriesRowLength = editContext.Model.GetLengthOfRow(lastEntry.Index);

            editContext.PrimaryCursor.RowIndex = lastEntry.Index;
            editContext.PrimaryCursor.ColumnIndex = lastEntriesRowLength;
        }

        return Task.CompletedTask;
    };

    public static TextEditorEdit CursorMovePageTopAsync = (ITextEditorEditContext editContext) =>
    {
        if (editContext.ViewModel.VirtualizationResult?.EntryBag.Any() ?? false)
        {
            var firstEntry = editContext.ViewModel.VirtualizationResult.EntryBag.First();

            editContext.PrimaryCursor.RowIndex = firstEntry.Index;
            editContext.PrimaryCursor.ColumnIndex = 0;
        }

        return Task.CompletedTask;
    };

    public static TextEditorEdit DuplicateAsync = (ITextEditorEditContext editContext) =>
    {
        var selectedText = TextEditorSelectionHelper.GetSelectedText(
            editContext.PrimaryCursor,
            editContext.Model);

        TextEditorCursor cursorForInsertion;

        if (selectedText is null)
        {
            // Select line
            selectedText = editContext.Model.GetLinesRange(editContext.PrimaryCursor.RowIndex, 1);

            cursorForInsertion = new TextEditorCursor(
                editContext.PrimaryCursor.RowIndex,
                0,
                editContext.PrimaryCursor.IsPrimaryCursor);
        }
        else
        {
            // Clone the TextEditorCursor to remove the TextEditorSelection otherwise the
            // selected text to duplicate would be overwritten by itself and do nothing
            cursorForInsertion = editContext.PrimaryCursor.ToCursor() with
            {
                Selection = TextEditorSelection.Empty
            };
        }

        editContext.CommandArgs.Dispatcher.Dispatch(new InsertTextAction(
            editContext.Model.ResourceUri,
            editContext.ViewModel.ViewModelKey,
            new TextEditorCursorModifier[] { new(cursorForInsertion) }.ToList(),
            selectedText,
            CancellationToken.None,
            editContext.AuthenticatedActionKey));

        return Task.CompletedTask;
    };

    public static TextEditorEdit IndentMoreAsync = (ITextEditorEditContext editContext) =>
    {
        var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
            editContext.PrimaryCursor);

        var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
            editContext.Model,
            selectionBoundsInPositionIndexUnits);

        for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
             i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
             i++)
        {
            var cursorForInsertion = new TextEditorCursor(i, 0, true);

            editContext.CommandArgs.Dispatcher.Dispatch(new InsertTextAction(
                editContext.Model.ResourceUri,
                editContext.ViewModel.ViewModelKey,
                editContext.RefreshCursorsRequest.CursorBag,
                KeyboardKeyFacts.WhitespaceCharacters.TAB.ToString(),
                CancellationToken.None,
                editContext.AuthenticatedActionKey));
        }

        var lowerBoundPositionIndexChange = 1;

        var upperBoundPositionIndexChange = selectionBoundsInRowIndexUnits.upperRowIndexExclusive -
            selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;

        if (editContext.PrimaryCursor.SelectionAnchorPositionIndex <
            editContext.PrimaryCursor.SelectionEndingPositionIndex)
        {
            editContext.PrimaryCursor.SelectionAnchorPositionIndex +=
                lowerBoundPositionIndexChange;

            editContext.PrimaryCursor.SelectionEndingPositionIndex +=
                upperBoundPositionIndexChange;
        }
        else
        {
            editContext.PrimaryCursor.SelectionAnchorPositionIndex +=
                upperBoundPositionIndexChange;

            editContext.PrimaryCursor.SelectionEndingPositionIndex +=
                lowerBoundPositionIndexChange;
        }

        var userCursorRowIndex = editContext.PrimaryCursor.RowIndex;
        var userCursorColumnIndex = editContext.PrimaryCursor.ColumnIndex;

        editContext.PrimaryCursor.RowIndex = userCursorRowIndex;
        editContext.PrimaryCursor.ColumnIndex = userCursorColumnIndex + 1;

        return Task.CompletedTask;
    };

    public static TextEditorEdit IndentLessAsync = (ITextEditorEditContext editContext) =>
    {
        var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
            editContext.PrimaryCursor);

        var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
            editContext.Model,
            selectionBoundsInPositionIndexUnits);

        bool isFirstLoop = true;

        for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
             i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
             i++)
        {
            var rowPositionIndex = editContext.Model.GetPositionIndex(i, 0);
            var characterReadCount = TextEditorModel.TAB_WIDTH;
            var lengthOfRow = editContext.Model.GetLengthOfRow(i);

            characterReadCount = Math.Min(lengthOfRow, characterReadCount);

            var readResult = editContext.Model.GetTextRange(rowPositionIndex, characterReadCount);
            var removeCharacterCount = 0;

            if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.TAB))
            {
                removeCharacterCount = 1;

                var cursorForDeletion = new TextEditorCursor(i, 0, true);

                editContext.CommandArgs.Dispatcher.Dispatch(new DeleteTextByRangeAction(
                    editContext.Model.ResourceUri,
                    editContext.ViewModel.ViewModelKey,
                    new TextEditorCursorModifier[] { new(cursorForDeletion) }.ToList(),
                    removeCharacterCount, // Delete a single "Tab" character
                    CancellationToken.None,
                    editContext.AuthenticatedActionKey));
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

                editContext.CommandArgs.Dispatcher.Dispatch(new DeleteTextByRangeAction(
                    editContext.Model.ResourceUri,
                    editContext.ViewModel.ViewModelKey,
                    new TextEditorCursorModifier[] { new(cursorForDeletion) }.ToList(),
                    removeCharacterCount,
                    CancellationToken.None,
                    editContext.AuthenticatedActionKey));
            }

            // Modify the lower bound of user's text selection
            if (isFirstLoop)
            {
                isFirstLoop = false;

                if (editContext.PrimaryCursor.SelectionAnchorPositionIndex <
                    editContext.PrimaryCursor.SelectionEndingPositionIndex)
                {
                    editContext.PrimaryCursor.SelectionAnchorPositionIndex -=
                        removeCharacterCount;
                }
                else
                {
                    editContext.PrimaryCursor.SelectionEndingPositionIndex -=
                        removeCharacterCount;
                }
            }

            // Modify the upper bound of user's text selection
            if (editContext.PrimaryCursor.SelectionAnchorPositionIndex <
                editContext.PrimaryCursor.SelectionEndingPositionIndex)
            {
                editContext.PrimaryCursor.SelectionEndingPositionIndex -=
                    removeCharacterCount;
            }
            else
            {
                editContext.PrimaryCursor.SelectionAnchorPositionIndex -=
                    removeCharacterCount;
            }

            // Modify the column index of user's cursor
            if (i == editContext.PrimaryCursor.RowIndex)
            {
                var nextColumnIndex = editContext.PrimaryCursor.ColumnIndex -
                    removeCharacterCount;

                editContext.PrimaryCursor.RowIndex = editContext.PrimaryCursor.RowIndex;
                editContext.PrimaryCursor.ColumnIndex = Math.Max(0, nextColumnIndex);
            }
        }

        return Task.CompletedTask;
    };

    public static TextEditorEdit ClearTextSelectionAsync = (ITextEditorEditContext editContext) =>
    {
        editContext.PrimaryCursor.SelectionAnchorPositionIndex = null;
        return Task.CompletedTask;
    };

    public static TextEditorEdit NewLineBelowAsync = (ITextEditorEditContext editContext) =>
    {
        editContext.PrimaryCursor.SelectionAnchorPositionIndex = null;

        var lengthOfRow = editContext.Model.GetLengthOfRow(editContext.PrimaryCursor.RowIndex);

        editContext.PrimaryCursor.RowIndex = editContext.PrimaryCursor.RowIndex;
        editContext.PrimaryCursor.ColumnIndex = lengthOfRow;

        editContext.CommandArgs.Dispatcher.Dispatch(new InsertTextAction(
            editContext.Model.ResourceUri,
            editContext.ViewModel.ViewModelKey,
            editContext.RefreshCursorsRequest.CursorBag,
            "\n",
            CancellationToken.None,
            editContext.AuthenticatedActionKey));

        return Task.CompletedTask;
    };

    public static TextEditorEdit NewLineAboveAsync = (ITextEditorEditContext editContext) =>
    {
        editContext.PrimaryCursor.SelectionAnchorPositionIndex = null;

        editContext.PrimaryCursor.RowIndex = editContext.PrimaryCursor.RowIndex;
        editContext.PrimaryCursor.ColumnIndex = 0;

        editContext.CommandArgs.Dispatcher.Dispatch(new InsertTextAction(
            editContext.Model.ResourceUri,
            editContext.ViewModel.ViewModelKey,
            new TextEditorCursorModifier[] { editContext.PrimaryCursor }.ToList(),
            "\n",
            CancellationToken.None,
            editContext.AuthenticatedActionKey));

        if (editContext.PrimaryCursor.RowIndex > 1)
        {
            editContext.PrimaryCursor.RowIndex--;
            editContext.PrimaryCursor.ColumnIndex = 0;
        }

        return Task.CompletedTask;
    };

    public static TextEditorEdit GoToMatchingCharacterFactoryAsync = async (ITextEditorEditContext editContext) =>
    {
        var cursorPositionIndex = editContext.Model.GetCursorPositionIndex(
            editContext.PrimaryCursor);

        if (editContext.CommandArgs.ShouldSelectText)
        {
            if (!TextEditorSelectionHelper.HasSelectedText(editContext.PrimaryCursor))
                editContext.PrimaryCursor.SelectionAnchorPositionIndex = cursorPositionIndex;
        }
        else
        {
            editContext.PrimaryCursor.SelectionAnchorPositionIndex = null;
        }

        var previousCharacter = editContext.Model.GetTextAt(cursorPositionIndex - 1);
        var currentCharacter = editContext.Model.GetTextAt(cursorPositionIndex);

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
            editContext.PrimaryCursor.RowIndex,
            editContext.PrimaryCursor.ColumnIndex,
            editContext.PrimaryCursor.IsPrimaryCursor)
        {
            PreferredColumnIndex = editContext.PrimaryCursor.PreferredColumnIndex,
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

            await editContext.CommandArgs.TextEditorService.ViewModelApi.GetMoveCursorTask(
                    keyboardEventArgs,
                    editContext.Model,
                    editContext.ViewModel.ViewModelKey,
                    editContext.PrimaryCursor)
                .Invoke(editContext);

            var temporaryCursorPositionIndex = editContext.Model.GetCursorPositionIndex(
                temporaryCursor);

            var characterAt = editContext.Model.GetTextAt(temporaryCursorPositionIndex);

            if (characterAt == match)
                unmatchedCharacters--;
            else if (characterAt == characterToMatch)
                unmatchedCharacters++;

            if (unmatchedCharacters == 0)
                break;

            if (temporaryCursorPositionIndex <= 0 ||
                temporaryCursorPositionIndex >= editContext.Model.DocumentLength)
                break;
        }

        if (editContext.CommandArgs.ShouldSelectText)
        {
            editContext.PrimaryCursor.SelectionEndingPositionIndex =
                editContext.Model.GetCursorPositionIndex(temporaryCursor);
        }

        editContext.PrimaryCursor.RowIndex = temporaryCursor.RowIndex;
        editContext.PrimaryCursor.ColumnIndex = temporaryCursor.ColumnIndex;
    };

    public static TextEditorEdit GoToDefinitionAsync = (ITextEditorEditContext editContext) =>
    {
        if (editContext.Model.CompilerService.Binder is null)
            return Task.CompletedTask;

        var positionIndex = editContext.Model.GetCursorPositionIndex(editContext.PrimaryCursor);
        var wordTextSpan = editContext.Model.GetWordAt(positionIndex);

        if (wordTextSpan is null)
            return Task.CompletedTask;

        var definitionTextSpan = editContext.Model.CompilerService.Binder.GetDefinition(
            wordTextSpan);

        if (definitionTextSpan is null)
            return Task.CompletedTask;

        var definitionModel = editContext.CommandArgs.TextEditorService.ModelApi.GetOrDefault(definitionTextSpan.ResourceUri);

        if (definitionModel is null)
        {
            if (editContext.CommandArgs.RegisterModelAction is not null)
            {
                editContext.CommandArgs.RegisterModelAction.Invoke(definitionTextSpan.ResourceUri);
                definitionModel = editContext.CommandArgs.TextEditorService.ModelApi.GetOrDefault(definitionTextSpan.ResourceUri);

                if (definitionModel is null)
                    return Task.CompletedTask;
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        var definitionViewModels = editContext.CommandArgs.TextEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

        if (!definitionViewModels.Any())
        {
            if (editContext.CommandArgs.RegisterViewModelAction is not null)
            {
                editContext.CommandArgs.RegisterViewModelAction.Invoke(definitionTextSpan.ResourceUri);
                definitionViewModels = editContext.CommandArgs.TextEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

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

        editContext.CommandArgs.Dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
            editContext.ViewModel.ViewModelKey,
            firstDefinitionInViewModel =>
            {
                var outCursor = firstDefinitionViewModelCursorModifier.ToCursor();
                var outCursorBag = firstDefinitionInViewModel.CursorBag.Replace(firstDefinitionInViewModel.PrimaryCursor, outCursor);

                return firstDefinitionInViewModel with
                {
                    CursorBag = outCursorBag
                };
            }));

        if (editContext.CommandArgs.ShowViewModelAction is not null)
            editContext.CommandArgs.ShowViewModelAction.Invoke(firstDefinitionViewModel.ViewModelKey);

        return Task.CompletedTask;
    };

    public static TextEditorEdit ShowFindDialogAsync = (ITextEditorEditContext editContext) =>
    {
        editContext.CommandArgs.TextEditorService.OptionsApi.ShowFindDialog();
        return Task.CompletedTask;
    };

    public static TextEditorEdit ShowTooltipByCursorPositionAsync = async (ITextEditorEditContext editContext) =>
    {
        if (editContext.CommandArgs.JsRuntime is null || editContext.CommandArgs.HandleMouseStoppedMovingEventAsyncFunc is null)
            return;

        var elementPositionInPixels = await editContext.CommandArgs.JsRuntime.InvokeAsync<ElementPositionInPixels>(
            "luthetusTextEditor.getBoundingClientRect",
            editContext.ViewModel.PrimaryCursorContentId);

        elementPositionInPixels = elementPositionInPixels with
        {
            Top = elementPositionInPixels.Top +
                (.9 * editContext.ViewModel.VirtualizationResult.CharAndRowMeasurements.RowHeight)
        };

        await editContext.CommandArgs.HandleMouseStoppedMovingEventAsyncFunc.Invoke(new MouseEventArgs
        {
            ClientX = elementPositionInPixels.Left,
            ClientY = elementPositionInPixels.Top
        });
    };
}
