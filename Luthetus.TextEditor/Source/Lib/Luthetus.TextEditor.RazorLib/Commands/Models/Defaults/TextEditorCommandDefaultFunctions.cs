using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Microsoft.AspNetCore.Components.Web;
using static Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorService;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Characters.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public class TextEditorCommandDefaultFunctions
{
    public static Task DoNothingDiscardAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        TextEditorService.RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        return Task.CompletedTask;
    }

    public static async Task CopyAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        TextEditorService.RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        var selectedText = TextEditorSelectionHelper.GetSelectedText(
            primaryCursor,
            model);

        selectedText ??= model.GetLinesRange(
            primaryCursor.RowIndex,
            1);

        await commandArgs.ClipboardService.SetClipboard(selectedText);
        await viewModel.FocusAsync();
    }

    public static async Task CutAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        var selectedText = TextEditorSelectionHelper.GetSelectedText(
            primaryCursor,
            model);

        if (selectedText is null)
            return; // Should never occur

        await commandArgs.ClipboardService.SetClipboard(selectedText);
        await viewModel.FocusAsync();

        commandArgs.Dispatcher.Dispatch(new KeyboardEventAction(
            model.ResourceUri,
            viewModel.ViewModelKey,
            refreshCursorsRequest.CursorBag,
            new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
            CancellationToken.None));
    }

    public static async Task PasteAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        var clipboard = await commandArgs.ClipboardService.ReadClipboard();

        commandArgs.Dispatcher.Dispatch(new InsertTextAction(
            model.ResourceUri,
            viewModel.ViewModelKey,
            refreshCursorsRequest.CursorBag,
            clipboard,
            CancellationToken.None));
    }

    public static Task SaveAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        var onSaveRequestedFunc = viewModel.OnSaveRequested;

        if (onSaveRequestedFunc is not null)
        {
            onSaveRequestedFunc.Invoke(model);

            commandArgs.Dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                viewModel.ViewModelKey,
                inState => inState with { })); // "with { }" is a Hack to re-render
        }

        return Task.CompletedTask;
    }

    public static Task SelectAllAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        primaryCursor.SelectionAnchorPositionIndex = 0;
        primaryCursor.SelectionEndingPositionIndex = model.DocumentLength;

        return Task.CompletedTask;
    }

    public static Task UndoAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        commandArgs.TextEditorService.ModelApi.UndoEditAsync(commandArgs.ModelResourceUri);
        return Task.CompletedTask;
    }

    public static Task RedoAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        commandArgs.TextEditorService.ModelApi.RedoEditAsync(commandArgs.ModelResourceUri);
        return Task.CompletedTask;
    }

    public static Task RemeasureAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        commandArgs.TextEditorService.OptionsApi.SetRenderStateKey(Key<RenderState>.NewKey());
        return Task.CompletedTask;
    }

    public static async Task ScrollLineDownAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        await viewModel.MutateScrollVerticalPositionByLinesAsync(1);
    }

    public static async Task ScrollLineUpAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        await viewModel.MutateScrollVerticalPositionByLinesAsync(-1);
    }

    public static async Task ScrollPageDownAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        await viewModel.MutateScrollVerticalPositionByPagesAsync(1);
    }

    public static async Task ScrollPageUpAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        await viewModel.MutateScrollVerticalPositionByPagesAsync(-1);
    }

    public static Task CursorMovePageBottomAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        if (viewModel.VirtualizationResult?.EntryBag.Any() ?? false)
        {
            var lastEntry = viewModel.VirtualizationResult.EntryBag.Last();
            var lastEntriesRowLength = model.GetLengthOfRow(lastEntry.Index);

            primaryCursor.RowIndex = lastEntry.Index;
            primaryCursor.ColumnIndex = lastEntriesRowLength;
        }

        return Task.CompletedTask;
    }

    public static Task CursorMovePageTopAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        if (viewModel.VirtualizationResult?.EntryBag.Any() ?? false)
        {
            var firstEntry = viewModel.VirtualizationResult.EntryBag.First();

            primaryCursor.RowIndex = firstEntry.Index;
            primaryCursor.ColumnIndex = 0;
        }

        return Task.CompletedTask;
    }

    public static Task DuplicateAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        var selectedText = TextEditorSelectionHelper.GetSelectedText(
            primaryCursor,
            model);

        TextEditorCursor cursorForInsertion;

        if (selectedText is null)
        {
            // Select line
            selectedText = model.GetLinesRange(primaryCursor.RowIndex, 1);

            cursorForInsertion = new TextEditorCursor(
                primaryCursor.RowIndex,
                0,
                primaryCursor.IsPrimaryCursor);
        }
        else
        {
            // Clone the TextEditorCursor to remove the TextEditorSelection otherwise the
            // selected text to duplicate would be overwritten by itself and do nothing
            cursorForInsertion = primaryCursor.ToCursor() with
            {
                Selection = TextEditorSelection.Empty
            };
        }

        commandArgs.Dispatcher.Dispatch(new InsertTextAction(
            model.ResourceUri,
            viewModel.ViewModelKey,
            new TextEditorCursorModifier[] { new(cursorForInsertion) }.ToList(),
            selectedText,
            CancellationToken.None));

        return Task.CompletedTask;
    }

    public static Task IndentMoreAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
            primaryCursor);

        var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
            model,
            selectionBoundsInPositionIndexUnits);

        for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
             i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
             i++)
        {
            var cursorForInsertion = new TextEditorCursor(i, 0, true);

            commandArgs.Dispatcher.Dispatch(new InsertTextAction(
                model.ResourceUri,
                viewModel.ViewModelKey,
                refreshCursorsRequest.CursorBag,
                KeyboardKeyFacts.WhitespaceCharacters.TAB.ToString(),
                CancellationToken.None));
        }

        var lowerBoundPositionIndexChange = 1;

        var upperBoundPositionIndexChange = selectionBoundsInRowIndexUnits.upperRowIndexExclusive -
            selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;

        if (primaryCursor.SelectionAnchorPositionIndex <
            primaryCursor.SelectionEndingPositionIndex)
        {
            primaryCursor.SelectionAnchorPositionIndex +=
                lowerBoundPositionIndexChange;

            primaryCursor.SelectionEndingPositionIndex +=
                upperBoundPositionIndexChange;
        }
        else
        {
            primaryCursor.SelectionAnchorPositionIndex +=
                upperBoundPositionIndexChange;

            primaryCursor.SelectionEndingPositionIndex +=
                lowerBoundPositionIndexChange;
        }

        var userCursorRowIndex = primaryCursor.RowIndex;
        var userCursorColumnIndex = primaryCursor.ColumnIndex;

        primaryCursor.RowIndex = userCursorRowIndex;
        primaryCursor.ColumnIndex = userCursorColumnIndex + 1;

        return Task.CompletedTask;
    }

    public static Task IndentLessAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
            primaryCursor);

        var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
            model,
            selectionBoundsInPositionIndexUnits);

        bool isFirstLoop = true;

        for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
             i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
             i++)
        {
            var rowPositionIndex = model.GetPositionIndex(i, 0);
            var characterReadCount = TextEditorModel.TAB_WIDTH;
            var lengthOfRow = model.GetLengthOfRow(i);

            characterReadCount = Math.Min(lengthOfRow, characterReadCount);

            var readResult = model.GetTextRange(rowPositionIndex, characterReadCount);
            var removeCharacterCount = 0;

            if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.TAB))
            {
                removeCharacterCount = 1;

                var cursorForDeletion = new TextEditorCursor(i, 0, true);

                commandArgs.Dispatcher.Dispatch(new DeleteTextByRangeAction(
                    model.ResourceUri,
                    viewModel.ViewModelKey,
                    new TextEditorCursorModifier[] { new(cursorForDeletion) }.ToList(),
                    removeCharacterCount, // Delete a single "Tab" character
                    CancellationToken.None));
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

                commandArgs.Dispatcher.Dispatch(new DeleteTextByRangeAction(
                    model.ResourceUri,
                    viewModel.ViewModelKey,
                    new TextEditorCursorModifier[] { new(cursorForDeletion) }.ToList(),
                    removeCharacterCount,
                    CancellationToken.None));
            }

            // Modify the lower bound of user's text selection
            if (isFirstLoop)
            {
                isFirstLoop = false;

                if (primaryCursor.SelectionAnchorPositionIndex <
                    primaryCursor.SelectionEndingPositionIndex)
                {
                    primaryCursor.SelectionAnchorPositionIndex -=
                        removeCharacterCount;
                }
                else
                {
                    primaryCursor.SelectionEndingPositionIndex -=
                        removeCharacterCount;
                }
            }

            // Modify the upper bound of user's text selection
            if (primaryCursor.SelectionAnchorPositionIndex <
                primaryCursor.SelectionEndingPositionIndex)
            {
                primaryCursor.SelectionEndingPositionIndex -=
                    removeCharacterCount;
            }
            else
            {
                primaryCursor.SelectionAnchorPositionIndex -=
                    removeCharacterCount;
            }

            // Modify the column index of user's cursor
            if (i == primaryCursor.RowIndex)
            {
                var nextColumnIndex = primaryCursor.ColumnIndex -
                    removeCharacterCount;

                primaryCursor.RowIndex = primaryCursor.RowIndex;
                primaryCursor.ColumnIndex = Math.Max(0, nextColumnIndex);
            }
        }

        return Task.CompletedTask;
    }

    public static Task ClearTextSelectionAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        primaryCursor.SelectionAnchorPositionIndex = null;

        return Task.CompletedTask;
    }

    public static Task NewLineBelowAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        primaryCursor.SelectionAnchorPositionIndex = null;

        var lengthOfRow = model.GetLengthOfRow(primaryCursor.RowIndex);

        primaryCursor.RowIndex = primaryCursor.RowIndex;
        primaryCursor.ColumnIndex = lengthOfRow;

        commandArgs.Dispatcher.Dispatch(new InsertTextAction(
            model.ResourceUri,
            viewModel.ViewModelKey,
            refreshCursorsRequest.CursorBag,
            "\n",
            CancellationToken.None));

        return Task.CompletedTask;
    }

    public static Task NewLineAboveAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        primaryCursor.SelectionAnchorPositionIndex = null;

        primaryCursor.RowIndex = primaryCursor.RowIndex;
        primaryCursor.ColumnIndex = 0;

        commandArgs.Dispatcher.Dispatch(new InsertTextAction(
            model.ResourceUri,
            viewModel.ViewModelKey,
            new TextEditorCursorModifier[] { primaryCursor }.ToList(),
            "\n",
            CancellationToken.None));

        if (primaryCursor.RowIndex > 1)
        {
            primaryCursor.RowIndex--;
            primaryCursor.ColumnIndex = 0;
        }

        return Task.CompletedTask;
    }

    public static Task GoToMatchingCharacterFactoryAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        var cursorPositionIndex = model.GetCursorPositionIndex(
            primaryCursor);

        if (commandArgs.ShouldSelectText)
        {
            if (!TextEditorSelectionHelper.HasSelectedText(primaryCursor))
                primaryCursor.SelectionAnchorPositionIndex = cursorPositionIndex;
        }
        else
        {
            primaryCursor.SelectionAnchorPositionIndex = null;
        }

        var previousCharacter = model.GetTextAt(cursorPositionIndex - 1);
        var currentCharacter = model.GetTextAt(cursorPositionIndex);

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
            return Task.CompletedTask;

        var directionToFindMatchingPunctuationCharacter =
            KeyboardKeyFacts.DirectionToFindMatchingPunctuationCharacter(characterToMatch.Value);

        if (directionToFindMatchingPunctuationCharacter is null)
            return Task.CompletedTask;

        var temporaryCursor = new TextEditorCursor(
            primaryCursor.RowIndex,
            primaryCursor.ColumnIndex,
            primaryCursor.IsPrimaryCursor)
        {
            PreferredColumnIndex = primaryCursor.PreferredColumnIndex,
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

            commandArgs.TextEditorService.ViewModelApi.MoveCursorAsync(
                keyboardEventArgs,
                model.ResourceUri,
                viewModel.ViewModelKey,
                primaryCursor.Key);

            var temporaryCursorPositionIndex = model.GetCursorPositionIndex(
                temporaryCursor);

            var characterAt = model.GetTextAt(temporaryCursorPositionIndex);

            if (characterAt == match)
                unmatchedCharacters--;
            else if (characterAt == characterToMatch)
                unmatchedCharacters++;

            if (unmatchedCharacters == 0)
                break;

            if (temporaryCursorPositionIndex <= 0 ||
                temporaryCursorPositionIndex >= model.DocumentLength)
                break;
        }

        if (commandArgs.ShouldSelectText)
        {
            primaryCursor.SelectionEndingPositionIndex =
                model.GetCursorPositionIndex(temporaryCursor);
        }

        primaryCursor.RowIndex = temporaryCursor.RowIndex;
        primaryCursor.ColumnIndex = temporaryCursor.ColumnIndex;

        return Task.CompletedTask;
    }

    public static Task GoToDefinitionAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        if (model.CompilerService.Binder is null)
            return Task.CompletedTask;

        var positionIndex = model.GetCursorPositionIndex(primaryCursor);
        var wordTextSpan = model.GetWordAt(positionIndex);

        if (wordTextSpan is null)
            return Task.CompletedTask;

        var definitionTextSpan = model.CompilerService.Binder.GetDefinition(
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

        commandArgs.Dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
            viewModel.ViewModelKey,
            firstDefinitionInViewModel =>
            {
                var outCursor = firstDefinitionViewModelCursorModifier.ToCursor();
                var outCursorBag = firstDefinitionInViewModel.CursorBag.Replace(firstDefinitionInViewModel.PrimaryCursor, outCursor);

                return firstDefinitionInViewModel with
                {
                    CursorBag = outCursorBag
                };
            }));

        if (commandArgs.ShowViewModelAction is not null)
            commandArgs.ShowViewModelAction.Invoke(firstDefinitionViewModel.ViewModelKey);

        return Task.CompletedTask;
    }

    public static Task ShowFindDialogAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        commandArgs.TextEditorService.OptionsApi.ShowFindDialog();
        return Task.CompletedTask;
    }

    public static async Task ShowTooltipByCursorPositionAsync(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        RefreshCursorsRequest refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor)
    {
        if (commandArgs.JsRuntime is null || commandArgs.HandleMouseStoppedMovingEventAsyncFunc is null)
            return;

        var elementPositionInPixels = await commandArgs.JsRuntime.InvokeAsync<ElementPositionInPixels>(
            "luthetusTextEditor.getBoundingClientRect",
            viewModel.PrimaryCursorContentId);

        elementPositionInPixels = elementPositionInPixels with
        {
            Top = elementPositionInPixels.Top +
                (.9 * viewModel.VirtualizationResult.CharAndRowMeasurements.RowHeight)
        };

        await commandArgs.HandleMouseStoppedMovingEventAsyncFunc.Invoke(new MouseEventArgs
        {
            ClientX = elementPositionInPixels.Left,
            ClientY = elementPositionInPixels.Top
        });
    }
}
