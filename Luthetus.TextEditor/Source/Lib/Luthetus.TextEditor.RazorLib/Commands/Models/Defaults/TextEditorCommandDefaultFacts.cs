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
    public static readonly TextEditorCommand DoNothingDiscard = new(
        _ => Task.CompletedTask,
        false,
        "DoNothingDiscard",
        "defaults_do-nothing-discard");

    public static readonly TextEditorCommand Copy = new(
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(
                commandArgs.PrimaryCursorSnapshot.ImmutableCursor.ImmutableSelection,
                commandArgs.Model);

            selectedText ??= commandArgs.Model.GetLinesRange(
                commandArgs.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                1);

            await commandArgs.ClipboardService.SetClipboard(selectedText);
            await commandArgs.ViewModel.FocusAsync();
        },
        false,
        "Copy",
        "defaults_copy");

    public static readonly TextEditorCommand Cut = new(
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(
                commandArgs.PrimaryCursorSnapshot.ImmutableCursor.ImmutableSelection,
                commandArgs.Model);

            var cursorSnapshotsBag = commandArgs.CursorSnapshotsBag;

            if (selectedText is null)
            {
                var cursor = TextEditorSelectionHelper.SelectLinesRange(
                    commandArgs.Model,
                    commandArgs.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                    1);

                cursorSnapshotsBag = TextEditorCursorSnapshot.TakeSnapshots(cursor);

                var primaryCursorSnapshot = cursorSnapshotsBag.FirstOrDefault();

                if (primaryCursorSnapshot is null)
                    return; // Should never occur

                selectedText = TextEditorSelectionHelper.GetSelectedText(
                    primaryCursorSnapshot.ImmutableCursor.ImmutableSelection,
                    commandArgs.Model);
            }

            if (selectedText is null)
                return; // Should never occur

            await commandArgs.ClipboardService.SetClipboard(selectedText);
            await commandArgs.ViewModel.FocusAsync();

            commandArgs.TextEditorService.Model.HandleKeyboardEvent(
                new TextEditorModelState.KeyboardEventAction(
                    commandArgs.Model.ResourceUri,
                    cursorSnapshotsBag,
                    new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
                    CancellationToken.None));
        },
        true,
        "Cut",
        "defaults_cut");

    public static readonly TextEditorCommand Paste = new(
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            var clipboard = await commandArgs.ClipboardService.ReadClipboard();

            commandArgs.TextEditorService.Model.InsertText(
                new TextEditorModelState.InsertTextAction(
                    commandArgs.Model.ResourceUri,
                    new[] { commandArgs.PrimaryCursorSnapshot }.ToImmutableArray(),
                    clipboard,
                    CancellationToken.None));
        },
        true,
        "Paste",
        "defaults_paste",
        TextEditKind.Other,
        "defaults_paste");

    public static readonly TextEditorCommand Save = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            var onSaveRequestedFunc = commandArgs.ViewModel.OnSaveRequested;

            if (onSaveRequestedFunc is not null)
            {
                onSaveRequestedFunc.Invoke(commandArgs.Model);

                commandArgs.TextEditorService.ViewModel.With(
                    commandArgs.ViewModel.ViewModelKey,
                    previousViewModel => previousViewModel with { }); // "with { }" is a Hack to re-render
            }

            return Task.CompletedTask;
        },
        false,
        "Save",
        "defaults_save");

    public static readonly TextEditorCommand SelectAll = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            var primaryCursor = commandArgs.PrimaryCursorSnapshot.UserCursor;
            
            primaryCursor.Selection.AnchorPositionIndex = 0;
            primaryCursor.Selection.EndingPositionIndex = commandArgs.Model.DocumentLength;

            commandArgs.TextEditorService.ViewModel.With(
                commandArgs.ViewModel.ViewModelKey,
                previousViewModel => previousViewModel with { }); // "with { }" is a Hack to re-render

            return Task.CompletedTask;
        },
        false,
        "Select All",
        "defaults_select-all");

    public static readonly TextEditorCommand Undo = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.TextEditorService.Model.UndoEdit(commandArgs.Model.ResourceUri);
            return Task.CompletedTask;
        },
        true,
        "Undo",
        "defaults_undo");

    public static readonly TextEditorCommand Redo = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.TextEditorService.Model.RedoEdit(commandArgs.Model.ResourceUri);
            return Task.CompletedTask;
        },
        true,
        "Redo",
        "defaults_redo");

    public static readonly TextEditorCommand Remeasure = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.TextEditorService.Options.SetRenderStateKey(Key<RenderState>.NewKey());
            return Task.CompletedTask;
        },
        false,
        "Remeasure",
        "defaults_remeasure");

    public static readonly TextEditorCommand ScrollLineDown = new(
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            await commandArgs.ViewModel.MutateScrollVerticalPositionByLinesAsync(1);
        },
        false,
        "Scroll Line Down",
        "defaults_scroll-line-down");

    public static readonly TextEditorCommand ScrollLineUp = new(
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            await commandArgs.ViewModel.MutateScrollVerticalPositionByLinesAsync(-1);
        },
        false,
        "Scroll Line Up",
        "defaults_scroll-line-up");

    public static readonly TextEditorCommand ScrollPageDown = new(
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            await commandArgs.ViewModel.MutateScrollVerticalPositionByPagesAsync(1);
        },
        false,
        "Scroll Page Down",
        "defaults_scroll-page-down");

    public static readonly TextEditorCommand ScrollPageUp = new(
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            await commandArgs.ViewModel.MutateScrollVerticalPositionByPagesAsync(-1);
        },
        false,
        "Scroll Page Up",
        "defaults_scroll-page-up");

    public static readonly TextEditorCommand CursorMovePageBottom = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.ViewModel.CursorMovePageBottom();
            return Task.CompletedTask;
        },
        false,
        "Move Cursor to Bottom of the Page",
        "defaults_cursor-move-page-bottom");

    public static readonly TextEditorCommand CursorMovePageTop = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.ViewModel.CursorMovePageTop();
            return Task.CompletedTask;
        },
        false,
        "Move Cursor to Top of the Page",
        "defaults_cursor-move-page-top");

    public static readonly TextEditorCommand Duplicate = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(
                commandArgs.PrimaryCursorSnapshot.ImmutableCursor.ImmutableSelection,
                commandArgs.Model);

            TextEditorCursor cursorForInsertion;

            if (selectedText is null)
            {
                selectedText = commandArgs.Model.GetLinesRange(
                    commandArgs.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                    1);

                cursorForInsertion = new TextEditorCursor(
                    (commandArgs.PrimaryCursorSnapshot.ImmutableCursor.RowIndex, 0),
                    commandArgs.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);
            }
            else
            {
                // Clone the TextEditorCursor to remove the TextEditorSelection otherwise the
                // selected text to duplicate would be overwritten by itself and do nothing
                cursorForInsertion = new TextEditorCursor(
                    commandArgs.PrimaryCursorSnapshot.ImmutableCursor,
                    commandArgs.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);
            }

            var insertTextAction = new TextEditorModelState.InsertTextAction(
                commandArgs.Model.ResourceUri,
                TextEditorCursorSnapshot.TakeSnapshots(cursorForInsertion),
                selectedText,
                CancellationToken.None);

            commandArgs.TextEditorService.Model.InsertText(insertTextAction);
            return Task.CompletedTask;
        },
        false,
        "Duplicate",
        "defaults_duplicate");

    public static readonly TextEditorCommand IndentMore = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
                commandArgs.PrimaryCursorSnapshot.ImmutableCursor.ImmutableSelection);

            var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                commandArgs.Model,
                selectionBoundsInPositionIndexUnits);

            for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
                 i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
                 i++)
            {
                var cursorForInsertion = new TextEditorCursor((i, 0), true);

                var insertTextAction = new TextEditorModelState.InsertTextAction(
                    commandArgs.Model.ResourceUri,
                    TextEditorCursorSnapshot.TakeSnapshots(cursorForInsertion),
                    KeyboardKeyFacts.WhitespaceCharacters.TAB.ToString(),
                    CancellationToken.None);

                commandArgs.TextEditorService.Model.InsertText(insertTextAction);
            }

            var lowerBoundPositionIndexChange = 1;

            var upperBoundPositionIndexChange = selectionBoundsInRowIndexUnits.upperRowIndexExclusive -
                selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;

            if (commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex <
                commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex)
            {
                commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex +=
                    lowerBoundPositionIndexChange;

                commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex +=
                    upperBoundPositionIndexChange;
            }
            else
            {
                commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex +=
                    upperBoundPositionIndexChange;

                commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex +=
                    lowerBoundPositionIndexChange;
            }

            var userCursorIndexCoordinates = commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

            commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (userCursorIndexCoordinates.rowIndex, userCursorIndexCoordinates.columnIndex + 1);

            return Task.CompletedTask;
        },
        false,
        "Indent More",
        "defaults_indent-more");

    public static readonly TextEditorCommand IndentLess = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
                commandArgs.PrimaryCursorSnapshot.ImmutableCursor.ImmutableSelection);

            var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                commandArgs.Model,
                selectionBoundsInPositionIndexUnits);

            bool isFirstLoop = true;

            for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
                 i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
                 i++)
            {
                var rowPositionIndex = commandArgs.Model.GetPositionIndex(i, 0);
                var characterReadCount = TextEditorModel.TAB_WIDTH;
                var lengthOfRow = commandArgs.Model.GetLengthOfRow(i);

                characterReadCount = Math.Min(lengthOfRow, characterReadCount);

                var readResult = commandArgs.Model.GetTextRange(rowPositionIndex, characterReadCount);
                var removeCharacterCount = 0;

                if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.TAB))
                {
                    removeCharacterCount = 1;

                    var cursorForDeletion = new TextEditorCursor((i, 0), true);

                    var deleteTextAction = new TextEditorModelState.DeleteTextByRangeAction(
                        commandArgs.Model.ResourceUri,
                        TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                        removeCharacterCount, // Delete a single "Tab" character
                        CancellationToken.None);

                    commandArgs.TextEditorService.Model.DeleteTextByRange(deleteTextAction);
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
                        commandArgs.Model.ResourceUri,
                        TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                        removeCharacterCount,
                        CancellationToken.None);

                    commandArgs.TextEditorService.Model.DeleteTextByRange(deleteTextAction);
                }

                // Modify the lower bound of user's text selection
                if (isFirstLoop)
                {
                    isFirstLoop = false;

                    if (commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex <
                        commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex)
                    {
                        commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex -=
                            removeCharacterCount;
                    }
                    else
                    {
                        commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex -=
                            removeCharacterCount;
                    }
                }

                // Modify the upper bound of user's text selection
                if (commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex <
                    commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex)
                {
                    commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex -=
                        removeCharacterCount;
                }
                else
                {
                    commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex -=
                        removeCharacterCount;
                }

                // Modify the column index of user's cursor
                if (i == commandArgs.PrimaryCursorSnapshot.ImmutableCursor.RowIndex)
                {
                    var nextColumnIndex = commandArgs.PrimaryCursorSnapshot.ImmutableCursor.ColumnIndex -
                        removeCharacterCount;

                    var userCursorIndexCoordinates = commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

                    commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                        (userCursorIndexCoordinates.rowIndex, Math.Max(0, nextColumnIndex));
                }
            }

            return Task.CompletedTask;
        },
        false,
        "Indent Less",
        "defaults_indent-less");

    public static readonly TextEditorCommand ClearTextSelection = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;
            return Task.CompletedTask;
        },
        false,
        "ClearTextSelection",
        "defaults_clear-text-selection");

    public static readonly TextEditorCommand NewLineBelow = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;

            var lengthOfRow = commandArgs.Model.GetLengthOfRow(
                commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.rowIndex);

            var temporaryIndexCoordinates = commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

            commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (temporaryIndexCoordinates.rowIndex, lengthOfRow);

            commandArgs.TextEditorService.Model.InsertText(
                new TextEditorModelState.InsertTextAction(
                    commandArgs.Model.ResourceUri,
                    TextEditorCursorSnapshot.TakeSnapshots(commandArgs.PrimaryCursorSnapshot.UserCursor),
                    "\n",
                    CancellationToken.None));

            return Task.CompletedTask;
        },
        false,
        "NewLineBelow",
        "defaults_new-line-below");

    public static readonly TextEditorCommand NewLineAbove = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;

            var temporaryIndexCoordinates = commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

            commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (temporaryIndexCoordinates.rowIndex, 0);

            commandArgs.TextEditorService.Model.InsertText(
                new TextEditorModelState.InsertTextAction(
                    commandArgs.Model.ResourceUri,
                    TextEditorCursorSnapshot.TakeSnapshots(commandArgs.PrimaryCursorSnapshot.UserCursor),
                    "\n",
                    CancellationToken.None));

            temporaryIndexCoordinates = commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

            if (temporaryIndexCoordinates.rowIndex > 1)
            {
                commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                    (temporaryIndexCoordinates.rowIndex - 1, 0);
            }

            return Task.CompletedTask;
        },
        false,
        "NewLineBelow",
        "defaults_new-line-below");

    public static TextEditorCommand GoToMatchingCharacterFactory(bool shouldSelectText) => new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var cursorPositionIndex = commandArgs.Model.GetCursorPositionIndex(
                commandArgs.PrimaryCursorSnapshot.UserCursor);

            if (shouldSelectText)
            {
                if (!TextEditorSelectionHelper.HasSelectedText(commandArgs.PrimaryCursorSnapshot.UserCursor.Selection))
                {
                    commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex =
                        cursorPositionIndex;
                }
            }
            else
            {
                commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;
            }

            var previousCharacter = commandArgs.Model.GetTextAt(cursorPositionIndex - 1);
            var currentCharacter = commandArgs.Model.GetTextAt(cursorPositionIndex);

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
                commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates,
                commandArgs.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);

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

                TextEditorCursor.MoveCursor(keyboardEventArgs, temporaryCursor, commandArgs.Model);

                var temporaryCursorPositionIndex = commandArgs.Model.GetCursorPositionIndex(
                    temporaryCursor);

                var characterAt = commandArgs.Model.GetTextAt(temporaryCursorPositionIndex);

                if (characterAt == match)
                    unmatchedCharacters--;
                else if (characterAt == characterToMatch)
                    unmatchedCharacters++;

                if (unmatchedCharacters == 0)
                    break;

                if (temporaryCursorPositionIndex <= 0 ||
                    temporaryCursorPositionIndex >= commandArgs.Model.DocumentLength)
                    break;
            }

            if (shouldSelectText)
            {
                commandArgs.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex =
                    commandArgs.Model.GetCursorPositionIndex(temporaryCursor);
            }

            commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates = temporaryCursor.IndexCoordinates;

            return Task.CompletedTask;
        },
        true,
        "GoToMatchingCharacter",
        "defaults_go-to-matching-character");

    public static readonly TextEditorCommand GoToDefinition = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            if (commandArgs.Model.CompilerService.Binder is null)
                return Task.CompletedTask;

            var positionIndex = commandArgs.Model.GetCursorPositionIndex(
                commandArgs.PrimaryCursorSnapshot.ImmutableCursor);

            var wordTextSpan = commandArgs.Model.GetWordAt(positionIndex);

            if (wordTextSpan is null)
                return Task.CompletedTask;

            var definitionTextSpan = commandArgs.Model.CompilerService.Binder.GetDefinition(
                wordTextSpan);

            if (definitionTextSpan is null)
                return Task.CompletedTask;

            var definitionModel = commandArgs.TextEditorService.Model.FindOrDefault(definitionTextSpan.ResourceUri);

            if (definitionModel is null)
            {
                if (commandArgs.RegisterModelAction is not null)
                {
                    commandArgs.RegisterModelAction.Invoke(definitionTextSpan.ResourceUri);
                    definitionModel = commandArgs.TextEditorService.Model.FindOrDefault(definitionTextSpan.ResourceUri);

                    if (definitionModel is null)
                        return Task.CompletedTask;
                }
                else
                {
                    return Task.CompletedTask;
                }
            }

            var definitionViewModels = commandArgs.TextEditorService.Model.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

            if (!definitionViewModels.Any())
            {
                if (commandArgs.RegisterViewModelAction is not null)
                {
                    commandArgs.RegisterViewModelAction.Invoke(definitionTextSpan.ResourceUri);
                    definitionViewModels = commandArgs.TextEditorService.Model.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

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

            if (commandArgs.ShowViewModelAction is not null)
                commandArgs.ShowViewModelAction.Invoke(firstDefinitionViewModel.ViewModelKey);

            return Task.CompletedTask;
        },
        true,
        "GoToDefinition",
        "defaults_go-to-definition");

    public static readonly TextEditorCommand ShowFindDialog = new(
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.TextEditorService.Options.ShowFindDialog();
            return Task.CompletedTask;
        },
        false,
        "OpenFindDialog",
        "defaults_open-find-dialog");

    /// <summary>
    /// <see cref="ShowTooltipByCursorPosition"/> is to fire the @onmouseover event
    /// so to speak. Such that a tooltip appears if one were to have moused over a symbol or etc...
    /// </summary>
    public static readonly TextEditorCommand ShowTooltipByCursorPosition = new(
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            if (commandArgs.JsRuntime is null || 
                commandArgs.HandleMouseStoppedMovingEventAsyncFunc is null)
            {
                return;
            }

            var elementPositionInPixels = await commandArgs.JsRuntime.InvokeAsync<ElementPositionInPixels>(
                "luthetusTextEditor.getBoundingClientRect",
                commandArgs.ViewModel.PrimaryCursorContentId);

            elementPositionInPixels = elementPositionInPixels with
            {
                Top = elementPositionInPixels.Top + 
                    (.9 * commandArgs.ViewModel.VirtualizationResult.CharAndRowMeasurements.RowHeight)
            };

            await commandArgs.HandleMouseStoppedMovingEventAsyncFunc.Invoke(new MouseEventArgs
            {
                ClientX = elementPositionInPixels.Left,
                ClientY = elementPositionInPixels.Top
            });
        },
        true,
        "ShowTooltipByCursorPosition",
        "defaults_show-tooltip-by-cursor-position");
}