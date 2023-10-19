using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Microsoft.JSInterop;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public static class TextEditorCommandDefaultFacts
{
    public static readonly CommandTextEditor DoNothingDiscard = new(
        _ => Task.CompletedTask,
        false,
        "DoNothingDiscard",
        "defaults_do-nothing-discard");

    public static readonly CommandTextEditor Copy = new(
        async interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(
                commandParameter
                    .PrimaryCursorSnapshot
                    .ImmutableCursor
                    .ImmutableSelection,
                commandParameter.Model);

            selectedText ??= commandParameter.Model.GetLinesRange(
                commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                1);

            await commandParameter.ClipboardService.SetClipboard(selectedText);
            await commandParameter.ViewModel.FocusAsync();
        },
        false,
        "Copy",
        "defaults_copy");

    public static readonly CommandTextEditor Cut = new(
        async interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(
                commandParameter
                    .PrimaryCursorSnapshot
                    .ImmutableCursor
                    .ImmutableSelection,
                commandParameter.Model);

            var cursorSnapshotsBag = commandParameter.CursorSnapshotsBag;

            if (selectedText is null)
            {
                var cursor = TextEditorSelectionHelper.SelectLinesRange(
                    commandParameter.Model,
                    commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                    1);

                cursorSnapshotsBag = TextEditorCursorSnapshot.TakeSnapshots(cursor);

                var primaryCursorSnapshot = cursorSnapshotsBag.FirstOrDefault();

                if (primaryCursorSnapshot is null)
                    return; // Should never occur

                selectedText = TextEditorSelectionHelper.GetSelectedText(
                    primaryCursorSnapshot.ImmutableCursor.ImmutableSelection,
                    commandParameter.Model);
            }

            if (selectedText is null)
                return; // Should never occur

            await commandParameter.ClipboardService.SetClipboard(selectedText);
            await commandParameter.ViewModel.FocusAsync();

            commandParameter.TextEditorService.Model.HandleKeyboardEvent(
                new TextEditorModelState.KeyboardEventAction(
                    commandParameter.Model.ResourceUri,
                    cursorSnapshotsBag,
                    new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
                    CancellationToken.None));
        },
        true,
        "Cut",
        "defaults_cut");

    public static readonly CommandTextEditor Paste = new(
        async interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

            var clipboard = await commandParameter.ClipboardService.ReadClipboard();

            commandParameter.TextEditorService.Model.InsertText(
                new TextEditorModelState.InsertTextAction(
                    commandParameter.Model.ResourceUri,
                    new[] { commandParameter.PrimaryCursorSnapshot }.ToImmutableArray(),
                    clipboard,
                    CancellationToken.None));
        },
        true,
        "Paste",
        "defaults_paste",
        TextEditKind.Other,
        "defaults_paste");

    public static readonly CommandTextEditor Save = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

            var onSaveRequestedFunc = commandParameter.ViewModel.OnSaveRequested;

            if (onSaveRequestedFunc is not null)
            {
                onSaveRequestedFunc.Invoke(commandParameter.Model);

                commandParameter.TextEditorService.ViewModel.With(
                    commandParameter.ViewModel.ViewModelKey,
                    previousViewModel => previousViewModel with { }); // "with { }" is a Hack to re-render
            }

            return Task.CompletedTask;
        },
        false,
        "Save",
        "defaults_save");

    public static readonly CommandTextEditor SelectAll = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

            var primaryCursor = commandParameter.PrimaryCursorSnapshot.UserCursor;

            primaryCursor.Selection.AnchorPositionIndex = 0;

            primaryCursor.Selection.EndingPositionIndex = commandParameter.Model.DocumentLength;

            commandParameter.TextEditorService.ViewModel.With(
                commandParameter.ViewModel.ViewModelKey,
                previousViewModel => previousViewModel with { }); // "with { }" is a Hack to re-render

            return Task.CompletedTask;
        },
        false,
        "Select All",
        "defaults_select-all");

    public static readonly CommandTextEditor Undo = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            commandParameter.TextEditorService.Model.UndoEdit(commandParameter.Model.ResourceUri);
            return Task.CompletedTask;
        },
        true,
        "Undo",
        "defaults_undo");

    public static readonly CommandTextEditor Redo = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            commandParameter.TextEditorService.Model.RedoEdit(commandParameter.Model.ResourceUri);
            return Task.CompletedTask;
        },
        true,
        "Redo",
        "defaults_redo");

    public static readonly CommandTextEditor Remeasure = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            commandParameter.TextEditorService.Options.SetRenderStateKey(Key<RenderState>.NewKey());
            return Task.CompletedTask;
        },
        false,
        "Remeasure",
        "defaults_remeasure");

    public static readonly CommandTextEditor ScrollLineDown = new(
        async interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            await commandParameter.ViewModel.MutateScrollVerticalPositionByLinesAsync(1);
        },
        false,
        "Scroll Line Down",
        "defaults_scroll-line-down");

    public static readonly CommandTextEditor ScrollLineUp = new(
        async interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            await commandParameter.ViewModel.MutateScrollVerticalPositionByLinesAsync(-1);
        },
        false,
        "Scroll Line Up",
        "defaults_scroll-line-up");

    public static readonly CommandTextEditor ScrollPageDown = new(
        async interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            await commandParameter.ViewModel.MutateScrollVerticalPositionByPagesAsync(1);
        },
        false,
        "Scroll Page Down",
        "defaults_scroll-page-down");

    public static readonly CommandTextEditor ScrollPageUp = new(
        async interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            await commandParameter.ViewModel.MutateScrollVerticalPositionByPagesAsync(-1);
        },
        false,
        "Scroll Page Up",
        "defaults_scroll-page-up");

    public static readonly CommandTextEditor CursorMovePageBottom = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            commandParameter.ViewModel.CursorMovePageBottom();
            return Task.CompletedTask;
        },
        false,
        "Move Cursor to Bottom of the Page",
        "defaults_cursor-move-page-bottom");

    public static readonly CommandTextEditor CursorMovePageTop = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            commandParameter.ViewModel.CursorMovePageTop();
            return Task.CompletedTask;
        },
        false,
        "Move Cursor to Top of the Page",
        "defaults_cursor-move-page-top");

    public static readonly CommandTextEditor Duplicate = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(
                commandParameter
                    .PrimaryCursorSnapshot
                    .ImmutableCursor
                    .ImmutableSelection,
                commandParameter.Model);

            TextEditorCursor cursorForInsertion;

            if (selectedText is null)
            {
                selectedText = commandParameter.Model.GetLinesRange(
                    commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                    1);

                cursorForInsertion = new TextEditorCursor(
                    (commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                        0),
                    commandParameter.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);
            }
            else
            {
                // Clone the TextEditorCursor to remove the TextEditorSelection otherwise the
                // selected text to duplicate would be overwritten by itself and do nothing
                cursorForInsertion = new TextEditorCursor(
                    (commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                        commandParameter.PrimaryCursorSnapshot.ImmutableCursor.ColumnIndex),
                    commandParameter.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);
            }

            var insertTextAction = new TextEditorModelState.InsertTextAction(
                commandParameter.Model.ResourceUri,
                TextEditorCursorSnapshot.TakeSnapshots(cursorForInsertion),
                selectedText,
                CancellationToken.None);

            commandParameter.TextEditorService.Model.InsertText(insertTextAction);
            return Task.CompletedTask;
        },
        false,
        "Duplicate",
        "defaults_duplicate");

    public static readonly CommandTextEditor IndentMore = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

            var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
                commandParameter
                    .PrimaryCursorSnapshot
                    .ImmutableCursor
                    .ImmutableSelection);

            var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                commandParameter.Model,
                selectionBoundsInPositionIndexUnits);

            for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
                 i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
                 i++)
            {
                var cursorForInsertion = new TextEditorCursor((i, 0), true);

                var insertTextAction = new TextEditorModelState.InsertTextAction(
                    commandParameter.Model.ResourceUri,
                    TextEditorCursorSnapshot.TakeSnapshots(cursorForInsertion),
                    KeyboardKeyFacts.WhitespaceCharacters.TAB.ToString(),
                    CancellationToken.None);

                commandParameter.TextEditorService.Model.InsertText(insertTextAction);
            }

            var lowerBoundPositionIndexChange = 1;

            var upperBoundPositionIndexChange = selectionBoundsInRowIndexUnits.upperRowIndexExclusive -
                selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;

            if (commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex <
                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex)
            {
                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex +=
                    lowerBoundPositionIndexChange;

                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex +=
                    upperBoundPositionIndexChange;
            }
            else
            {
                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex +=
                    upperBoundPositionIndexChange;

                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex +=
                    lowerBoundPositionIndexChange;
            }

            var userCursorIndexCoordinates = commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

            commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (userCursorIndexCoordinates.rowIndex,
                    userCursorIndexCoordinates.columnIndex + 1);

            return Task.CompletedTask;
        },
        false,
        "Indent More",
        "defaults_indent-more");

    public static readonly CommandTextEditor IndentLess = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
                commandParameter
                    .PrimaryCursorSnapshot
                    .ImmutableCursor
                    .ImmutableSelection);

            var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                commandParameter.Model,
                selectionBoundsInPositionIndexUnits);

            bool isFirstLoop = true;

            for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
                 i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
                 i++)
            {
                var rowPositionIndex = commandParameter.Model.GetPositionIndex(i, 0);

                var characterReadCount = TextEditorModel.TAB_WIDTH;

                var lengthOfRow = commandParameter.Model.GetLengthOfRow(i);

                characterReadCount = Math.Min(lengthOfRow, characterReadCount);

                var readResult = commandParameter.Model.GetTextRange(rowPositionIndex, characterReadCount);

                int removeCharacterCount = 0;

                if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.TAB))
                {
                    removeCharacterCount = 1;

                    var cursorForDeletion = new TextEditorCursor((i, 0), true);

                    var deleteTextAction = new TextEditorModelState.DeleteTextByRangeAction(
                        commandParameter.Model.ResourceUri,
                        TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                        removeCharacterCount, // Delete a single "Tab" character
                        CancellationToken.None);

                    commandParameter.TextEditorService.Model.DeleteTextByRange(deleteTextAction);
                }
                else if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.SPACE))
                {
                    var cursorForDeletion = new TextEditorCursor((i, 0), true);

                    var contiguousSpaceCount = 0;

                    foreach (var character in readResult)
                    {
                        if (character == KeyboardKeyFacts.WhitespaceCharacters.SPACE)
                            contiguousSpaceCount++;
                    }

                    removeCharacterCount = contiguousSpaceCount;

                    var deleteTextAction = new TextEditorModelState.DeleteTextByRangeAction(
                        commandParameter.Model.ResourceUri,
                        TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                        removeCharacterCount,
                        CancellationToken.None);

                    commandParameter.TextEditorService.Model.DeleteTextByRange(deleteTextAction);
                }

                // Modify the lower bound of user's text selection
                if (isFirstLoop)
                {
                    isFirstLoop = false;

                    if (commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex <
                        commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex)
                    {
                        commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex -=
                            removeCharacterCount;
                    }
                    else
                    {
                        commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex -=
                            removeCharacterCount;
                    }
                }

                // Modify the upper bound of user's text selection
                if (commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex <
                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex)
                {
                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex -=
                        removeCharacterCount;
                }
                else
                {
                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex -=
                        removeCharacterCount;
                }

                // Modify the column index of user's cursor
                if (i == commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex)
                {
                    var nextColumnIndex = commandParameter.PrimaryCursorSnapshot.ImmutableCursor.ColumnIndex -
                        removeCharacterCount;

                    var userCursorIndexCoordinates =
                        commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

                    commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                        (userCursorIndexCoordinates.rowIndex, Math.Max(0, nextColumnIndex));
                }
            }

            return Task.CompletedTask;
        },
        false,
        "Indent Less",
        "defaults_indent-less");

    public static readonly CommandTextEditor ClearTextSelection = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;
            return Task.CompletedTask;
        },
        false,
        "ClearTextSelection",
        "defaults_clear-text-selection");

    public static readonly CommandTextEditor NewLineBelow = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;

            var lengthOfRow = commandParameter.Model.GetLengthOfRow(
                commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.rowIndex);

            var temporaryIndexCoordinates = commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

            commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (temporaryIndexCoordinates.rowIndex, lengthOfRow);

            commandParameter.TextEditorService.Model.InsertText(
                new TextEditorModelState.InsertTextAction(
                    commandParameter.Model.ResourceUri,
                    TextEditorCursorSnapshot.TakeSnapshots(commandParameter.PrimaryCursorSnapshot.UserCursor),
                    "\n",
                    CancellationToken.None));

            return Task.CompletedTask;
        },
        false,
        "NewLineBelow",
        "defaults_new-line-below");

    public static readonly CommandTextEditor NewLineAbove = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;

            var temporaryIndexCoordinates = commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

            commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (temporaryIndexCoordinates.rowIndex, 0);

            commandParameter.TextEditorService.Model.InsertText(
                new TextEditorModelState.InsertTextAction(
                    commandParameter.Model.ResourceUri,
                    TextEditorCursorSnapshot.TakeSnapshots(commandParameter.PrimaryCursorSnapshot.UserCursor),
                    "\n",
                    CancellationToken.None));

            temporaryIndexCoordinates = commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

            if (temporaryIndexCoordinates.rowIndex > 1)
            {
                commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                    (temporaryIndexCoordinates.rowIndex - 1, 0);
            }

            return Task.CompletedTask;
        },
        false,
        "NewLineBelow",
        "defaults_new-line-below");

    public static CommandTextEditor GoToMatchingCharacterFactory(bool shouldSelectText) => new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            var cursorPositionIndex = commandParameter.Model.GetCursorPositionIndex(
                commandParameter.PrimaryCursorSnapshot.UserCursor);

            if (shouldSelectText)
            {
                if (!TextEditorSelectionHelper.HasSelectedText(commandParameter.PrimaryCursorSnapshot.UserCursor.Selection))
                {
                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex =
                        cursorPositionIndex;
                }
            }
            else
            {
                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;
            }

            var previousCharacter = commandParameter.Model.GetTextAt(cursorPositionIndex - 1);

            var currentCharacter = commandParameter.Model.GetTextAt(cursorPositionIndex);

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

            var directionToFindMatchMatchingPunctuationCharacter =
                KeyboardKeyFacts.DirectionToFindMatchMatchingPunctuationCharacter(characterToMatch.Value);

            if (directionToFindMatchMatchingPunctuationCharacter is null)
                return Task.CompletedTask;

            var temporaryCursor = new TextEditorCursor(
                (commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.rowIndex,
                    commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.columnIndex),
                commandParameter.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);

            var unmatchedCharacters =
                fallbackToPreviousCharacter && directionToFindMatchMatchingPunctuationCharacter == -1
                    ? 0
                    : 1;

            while (true)
            {
                KeyboardEventArgs keyboardEventArgs;

                if (directionToFindMatchMatchingPunctuationCharacter == -1)
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

                TextEditorCursor.MoveCursor(keyboardEventArgs, temporaryCursor, commandParameter.Model);

                var temporaryCursorPositionIndex = commandParameter.Model.GetCursorPositionIndex(
                    temporaryCursor);

                var characterAt = commandParameter.Model.GetTextAt(temporaryCursorPositionIndex);

                if (characterAt == match)
                    unmatchedCharacters--;
                else if (characterAt == characterToMatch)
                    unmatchedCharacters++;

                if (unmatchedCharacters == 0)
                    break;

                if (temporaryCursorPositionIndex <= 0 ||
                    temporaryCursorPositionIndex >= commandParameter.Model.DocumentLength)
                    break;
            }

            if (shouldSelectText)
            {
                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex =
                    commandParameter.Model.GetCursorPositionIndex(temporaryCursor);
            }

            commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                temporaryCursor.IndexCoordinates;

            return Task.CompletedTask;
        },
        true,
        "GoToMatchingCharacter",
        "defaults_go-to-matching-character");

    public static readonly CommandTextEditor GoToDefinition = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

            if (commandParameter.Model.CompilerService.Binder is null)
                return Task.CompletedTask;

            var positionIndex = commandParameter.Model.GetCursorPositionIndex(
                commandParameter.PrimaryCursorSnapshot.ImmutableCursor);

            var wordTextSpan = commandParameter.Model.GetWordAt(positionIndex);

            if (wordTextSpan is null)
                return Task.CompletedTask;

            var definitionTextSpan = commandParameter.Model.CompilerService.Binder.GetDefinition(
                wordTextSpan);

            if (definitionTextSpan is null)
                return Task.CompletedTask;

            var definitionModel = commandParameter.TextEditorService.Model.FindOrDefault(definitionTextSpan.ResourceUri);

            if (definitionModel is null)
            {
                if (commandParameter.RegisterModelAction is not null)
                {
                    commandParameter.RegisterModelAction.Invoke(definitionTextSpan.ResourceUri);
                    definitionModel = commandParameter.TextEditorService.Model.FindOrDefault(definitionTextSpan.ResourceUri);

                    if (definitionModel is null)
                        return Task.CompletedTask;
                }
                else
                {
                    return Task.CompletedTask;
                }
            }

            var definitionViewModels = commandParameter.TextEditorService.Model.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

            if (!definitionViewModels.Any())
            {
                if (commandParameter.RegisterViewModelAction is not null)
                {
                    commandParameter.RegisterViewModelAction.Invoke(definitionTextSpan.ResourceUri);
                    definitionViewModels = commandParameter.TextEditorService.Model.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

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

            firstDefinitionViewModel.PrimaryCursor.IndexCoordinates = (rowData.rowIndex, columnIndex);
            firstDefinitionViewModel.PrimaryCursor.PreferredColumnIndex = columnIndex;

            if (commandParameter.ShowViewModelAction is not null)
                commandParameter.ShowViewModelAction.Invoke(firstDefinitionViewModel.ViewModelKey);

            return Task.CompletedTask;
        },
        true,
        "GoToDefinition",
        "defaults_go-to-definition");

    public static readonly CommandTextEditor ShowFindDialog = new(
        interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;
            commandParameter.TextEditorService.Options.ShowFindDialog();
            return Task.CompletedTask;
        },
        false,
        "OpenFindDialog",
        "defaults_open-find-dialog");

    /// <summary>
    /// <see cref="ShowTooltipByCursorPosition"/> is to fire the @onmouseover event
    /// so to speak. Such that a tooltip appears if one were to have moused over a symbol or etc...
    /// </summary>
    public static readonly CommandTextEditor ShowTooltipByCursorPosition = new(
        async interfaceCommandParameter =>
        {
            var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

            if (commandParameter.JsRuntime is null || 
                commandParameter.HandleMouseStoppedMovingEventAsyncFunc is null)
            {
                return;
            }

            var elementPositionInPixels = await commandParameter.JsRuntime.InvokeAsync<ElementPositionInPixels>(
                "luthetusTextEditor.getBoundingClientRect",
                commandParameter.ViewModel.PrimaryCursorContentId);

            elementPositionInPixels = elementPositionInPixels with
            {
                Top = elementPositionInPixels.Top + 
                    (commandParameter.ViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels *
                    .9)
            };

            await commandParameter.HandleMouseStoppedMovingEventAsyncFunc.Invoke(new MouseEventArgs
            {
                ClientX = elementPositionInPixels.Left,
                ClientY = elementPositionInPixels.Top
            });
        },
        true,
        "ShowTooltipByCursorPosition",
        "defaults_show-tooltip-by-cursor-position");
}