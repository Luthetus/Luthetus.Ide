using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Fluxor;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public static class TextEditorCommandDefaultFacts
{
    public static readonly TextEditorCommand DoNothingDiscard = new(
        "DoNothingDiscard", "defaults_do-nothing-discard", false, false, TextEditKind.None, null,
        _ => Task.CompletedTask);

    public static readonly TextEditorCommand Copy = new(
        "Copy", "defaults_copy", false, false, TextEditKind.None, null,
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                async inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    var selectedText = TextEditorSelectionHelper.GetSelectedText(
                        cursorModifier,
                        commandArgs.Model);

                    selectedText ??= commandArgs.Model.GetLinesRange(
                        cursorModifier.RowIndex,
                        1);

                    await commandArgs.ClipboardService.SetClipboard(selectedText);
                    await commandArgs.ViewModel.FocusAsync();

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        });
                });
        });

    public static readonly TextEditorCommand Cut = new(
        "Cut", "defaults_cut", false, true, TextEditKind.None, null,
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                async inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    var selectedText = TextEditorSelectionHelper.GetSelectedText(
                        cursorModifier,
                        commandArgs.Model);

                    var cursorSnapshotsBag = commandArgs.CursorBag;

                    if (selectedText is null)
                    {
                        var cursor = TextEditorSelectionHelper.SelectLinesRange(
                            commandArgs.Model,
                            cursorModifier.RowIndex,
                            1);

                        selectedText = TextEditorSelectionHelper.GetSelectedText(
                            cursor.Selection,
                            commandArgs.Model);
                    }

                    if (selectedText is null)
                        return state => state; // Should never occur

                    await commandArgs.ClipboardService.SetClipboard(selectedText);
                    await commandArgs.ViewModel.FocusAsync();

                    commandArgs.TextEditorService.ModelApi.HandleKeyboardEvent(
                        new TextEditorModelState.KeyboardEventAction(
                            commandArgs.Model.ResourceUri,
                            cursorSnapshotsBag,
                            new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
                            CancellationToken.None));

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        });
                });
        });

    public static readonly TextEditorCommand Paste = new(
        "Paste", "defaults_paste", false, true, TextEditKind.Other, "defaults_paste",
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                async inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    var clipboard = await commandArgs.ClipboardService.ReadClipboard();

                    commandArgs.TextEditorService.ModelApi.InsertText(
                        new TextEditorModelState.InsertTextAction(
                            commandArgs.Model.ResourceUri,
                            new[] { cursorModifier.ToCursor() }.ToImmutableArray(),
                            clipboard,
                            CancellationToken.None));

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        });
                });
        });

    public static readonly TextEditorCommand Save = new(
        "Save",
        "defaults_save",
        false,
        false,
        TextEditKind.None,
        null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    var onSaveRequestedFunc = commandArgs.ViewModel.OnSaveRequested;

                    if (onSaveRequestedFunc is not null)
                    {
                        onSaveRequestedFunc.Invoke(commandArgs.Model);

                        commandArgs.TextEditorService.ViewModelApi.With(
                            commandArgs.ViewModel.ViewModelKey,
                            previousViewModel => previousViewModel with { }); // "with { }" is a Hack to re-render
                    }

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        }));
                });

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand SelectAll = new(
        "Select All", "defaults_select-all", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    cursorModifier.SelectionAnchorPositionIndex = 0;
                    cursorModifier.SelectionEndingPositionIndex = commandArgs.Model.DocumentLength;

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        }));
                });

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Undo = new(
        "Undo", "defaults_undo", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.TextEditorService.ModelApi.UndoEdit(commandArgs.Model.ResourceUri);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Redo = new(
        "Redo", "defaults_redo", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.TextEditorService.ModelApi.RedoEdit(commandArgs.Model.ResourceUri);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Remeasure = new(
        "Remeasure", "defaults_remeasure", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.TextEditorService.OptionsApi.SetRenderStateKey(Key<RenderState>.NewKey());
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollLineDown = new(
        "Scroll Line Down", "defaults_scroll-line-down", false, false, TextEditKind.None, null,
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            await commandArgs.ViewModel.MutateScrollVerticalPositionByLinesAsync(1);
        });

    public static readonly TextEditorCommand ScrollLineUp = new(
        "Scroll Line Up", "defaults_scroll-line-up", false, false, TextEditKind.None, null,
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            await commandArgs.ViewModel.MutateScrollVerticalPositionByLinesAsync(-1);
        });

    public static readonly TextEditorCommand ScrollPageDown = new(
        "Scroll Page Down", "defaults_scroll-page-down", false, false, TextEditKind.None, null,
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            await commandArgs.ViewModel.MutateScrollVerticalPositionByPagesAsync(1);
        });

    public static readonly TextEditorCommand ScrollPageUp = new(
        "Scroll Page Up", "defaults_scroll-page-up", false, false, TextEditKind.None, null,
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            await commandArgs.ViewModel.MutateScrollVerticalPositionByPagesAsync(-1);
        });

    public static readonly TextEditorCommand CursorMovePageBottom = new(
        "Move Cursor to Bottom of the Page", "defaults_cursor-move-page-bottom", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.CursorMovePageBottom(
                commandArgs.Model.ResourceUri,
                commandArgs.ViewModel.ViewModelKey,
                commandArgs.ViewModel.PrimaryCursor.Key);

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand CursorMovePageTop = new(
        "Move Cursor to Top of the Page", "defaults_cursor-move-page-top", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.CursorMovePageTop(
                commandArgs.Model.ResourceUri,
                commandArgs.ViewModel.ViewModelKey,
                commandArgs.ViewModel.PrimaryCursor.Key);

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Duplicate = new(
        "Duplicate", "defaults_duplicate", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    var selectedText = TextEditorSelectionHelper.GetSelectedText(
                        cursorModifier,
                        commandArgs.Model);

                    TextEditorCursor cursorForInsertion;

                    if (selectedText is null)
                    {
                        selectedText = commandArgs.Model.GetLinesRange(
                            cursorModifier.RowIndex,
                            1);

                        cursorForInsertion = TextEditorCursor.Empty with
                        {
                            RowIndex = cursorModifier.RowIndex,
                            ColumnIndex = 0,
                            PreferredColumnIndex = 0,
                            IsPrimaryCursor = cursorModifier.IsPrimaryCursor
                        };
                    }
                    else
                    {
                        // Clone the TextEditorCursor to remove the TextEditorSelection otherwise the
                        // selected text to duplicate would be overwritten by itself and do nothing
                        cursorForInsertion = cursorModifier.ToCursor() with
                        {
                            Selection = TextEditorSelection.Empty
                        };
                    }

                    var insertTextAction = new TextEditorModelState.InsertTextAction(
                        commandArgs.Model.ResourceUri,
                        new TextEditorCursor[] { cursorForInsertion }.ToImmutableArray(),
                        selectedText,
                        CancellationToken.None);

                    commandArgs.TextEditorService.ModelApi.InsertText(insertTextAction);

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        }));
                });

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand IndentMore = new(
        "Indent More", "defaults_indent-more", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
                        cursorModifier);

                    var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                        commandArgs.Model,
                        selectionBoundsInPositionIndexUnits);

                    for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
                         i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
                         i++)
                    {
                        var cursorForInsertion = TextEditorCursor.Empty with
                        {
                            RowIndex = i,
                            ColumnIndex = 0,
                            IsPrimaryCursor = true
                        };

                        var insertTextAction = new TextEditorModelState.InsertTextAction(
                            commandArgs.Model.ResourceUri,
                            new TextEditorCursor[] { cursorForInsertion }.ToImmutableArray(),
                            KeyboardKeyFacts.WhitespaceCharacters.TAB.ToString(),
                            CancellationToken.None);

                        commandArgs.TextEditorService.ModelApi.InsertText(insertTextAction);
                    }

                    var lowerBoundPositionIndexChange = 1;

                    var upperBoundPositionIndexChange = selectionBoundsInRowIndexUnits.upperRowIndexExclusive -
                        selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;

                    if (cursorModifier.SelectionAnchorPositionIndex <
                        cursorModifier.SelectionEndingPositionIndex)
                    {
                        cursorModifier.SelectionAnchorPositionIndex +=
                            lowerBoundPositionIndexChange;

                        cursorModifier.SelectionEndingPositionIndex +=
                            upperBoundPositionIndexChange;
                    }
                    else
                    {
                        cursorModifier.SelectionAnchorPositionIndex +=
                            upperBoundPositionIndexChange;

                        cursorModifier.SelectionEndingPositionIndex +=
                            lowerBoundPositionIndexChange;
                    }

                    var userCursorRowIndex = cursorModifier.RowIndex;
                    var userCursorColumnIndex = cursorModifier.ColumnIndex;

                    cursorModifier.RowIndex = userCursorRowIndex;
                    cursorModifier.ColumnIndex = userCursorColumnIndex + 1;

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        }));
                });

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand IndentLess = new(
        "Indent Less", "defaults_indent-less", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
                        cursorModifier);

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

                            var cursorForDeletion = TextEditorCursor.Empty with
                            {
                                RowIndex = i,
                                ColumnIndex = 0,
                                IsPrimaryCursor = true
                            };

                            var deleteTextAction = new TextEditorModelState.DeleteTextByRangeAction(
                                commandArgs.Model.ResourceUri,
                                new TextEditorCursor[] { cursorForDeletion }.ToImmutableArray(),
                                removeCharacterCount, // Delete a single "Tab" character
                                CancellationToken.None);

                            commandArgs.TextEditorService.ModelApi.DeleteTextByRange(deleteTextAction);
                        }
                        else if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.SPACE))
                        {
                            var cursorForDeletion = TextEditorCursor.Empty with
                            {
                                RowIndex = i,
                                ColumnIndex = 0,
                                IsPrimaryCursor = true
                            };

                            var contiguousSpaceCount = 0;

                            foreach (var character in readResult)
                            {
                                if (character == KeyboardKeyFacts.WhitespaceCharacters.SPACE)
                                    contiguousSpaceCount++;
                            }

                            removeCharacterCount = contiguousSpaceCount;

                            var deleteTextAction = new TextEditorModelState.DeleteTextByRangeAction(
                                commandArgs.Model.ResourceUri,
                                new TextEditorCursor[] { cursorForDeletion }.ToImmutableArray(),
                                removeCharacterCount,
                                CancellationToken.None);

                            commandArgs.TextEditorService.ModelApi.DeleteTextByRange(deleteTextAction);
                        }

                        // Modify the lower bound of user's text selection
                        if (isFirstLoop)
                        {
                            isFirstLoop = false;

                            if (cursorModifier.SelectionAnchorPositionIndex <
                                cursorModifier.SelectionEndingPositionIndex)
                            {
                                cursorModifier.SelectionAnchorPositionIndex -=
                                    removeCharacterCount;
                            }
                            else
                            {
                                cursorModifier.SelectionEndingPositionIndex -=
                                    removeCharacterCount;
                            }
                        }

                        // Modify the upper bound of user's text selection
                        if (cursorModifier.SelectionAnchorPositionIndex <
                            cursorModifier.SelectionEndingPositionIndex)
                        {
                            cursorModifier.SelectionEndingPositionIndex -=
                                removeCharacterCount;
                        }
                        else
                        {
                            cursorModifier.SelectionAnchorPositionIndex -=
                                removeCharacterCount;
                        }

                        // Modify the column index of user's cursor
                        if (i == cursorModifier.RowIndex)
                        {
                            var nextColumnIndex = cursorModifier.ColumnIndex -
                                removeCharacterCount;

                            var userCursorRowIndex = cursorModifier.RowIndex;
                            var userCursorColumnIndex = cursorModifier.ColumnIndex;

                            cursorModifier.RowIndex = userCursorRowIndex;
                            cursorModifier.ColumnIndex = Math.Max(0, nextColumnIndex);
                        }
                    }

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        }));
                });

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ClearTextSelection = new(
        "ClearTextSelection", "defaults_clear-text-selection", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    cursorModifier.SelectionAnchorPositionIndex = null;

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        }));
                });

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand NewLineBelow = new(
        "NewLineBelow", "defaults_new-line-below", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    cursorModifier.SelectionAnchorPositionIndex = null;

                    var lengthOfRow = commandArgs.Model.GetLengthOfRow(
                        cursorModifier.RowIndex);

                    var temporaryRowIndex = cursorModifier.RowIndex;
                    var temporaryColumnIndex = cursorModifier.ColumnIndex;

                    cursorModifier.RowIndex = temporaryRowIndex;
                    cursorModifier.ColumnIndex = lengthOfRow;

                    commandArgs.TextEditorService.ModelApi.InsertText(
                        new TextEditorModelState.InsertTextAction(
                            commandArgs.Model.ResourceUri,
                            new TextEditorCursor[] { cursorModifier.ToCursor() }.ToImmutableArray(),
                            "\n",
                            CancellationToken.None));

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        }));
                });

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand NewLineAbove = new(
        "NewLineBelow", "defaults_new-line-below", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
            commandArgs.ViewModel.ViewModelKey,
            inViewModel =>
            {
                var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                cursorModifier.SelectionAnchorPositionIndex = null;

                var temporaryRowIndex = cursorModifier.RowIndex;
                var temporaryColumnIndex = cursorModifier.ColumnIndex;

                cursorModifier.RowIndex = temporaryRowIndex;
                cursorModifier.ColumnIndex = 0;

                commandArgs.TextEditorService.ModelApi.InsertText(
                    new TextEditorModelState.InsertTextAction(
                        commandArgs.Model.ResourceUri,
                        new TextEditorCursor[] { cursorModifier.ToCursor() }.ToImmutableArray(),
                        "\n",
                        CancellationToken.None));

                temporaryRowIndex = cursorModifier.RowIndex;
                temporaryColumnIndex = cursorModifier.ColumnIndex;

                if (temporaryRowIndex > 1)
                {
                    cursorModifier.RowIndex = temporaryRowIndex - 1;
                    cursorModifier.ColumnIndex = 0;
                }

                var outCursor = cursorModifier.ToCursor();
                var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                    state => state with
                    {
                        CursorBag = outCursorBag
                    }));
            });

            return Task.CompletedTask;
        });

    public static TextEditorCommand GoToMatchingCharacterFactory(bool shouldSelectText) => new(
        "GoToMatchingCharacter",
        "defaults_go-to-matching-character",
        false,
        true,
        TextEditKind.None,
        null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    var cursorPositionIndex = commandArgs.Model.GetCursorPositionIndex(
                        cursorModifier);

                    if (shouldSelectText)
                    {
                        if (!TextEditorSelectionHelper.HasSelectedText(cursorModifier))
                            cursorModifier.SelectionAnchorPositionIndex = cursorPositionIndex;
                    }
                    else
                    {
                        cursorModifier.SelectionAnchorPositionIndex = null;
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
                        return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(state => state));

                    var directionToFindMatchingPunctuationCharacter =
                        KeyboardKeyFacts.DirectionToFindMatchingPunctuationCharacter(characterToMatch.Value);

                    if (directionToFindMatchingPunctuationCharacter is null)
                        return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(state => state));

                    var temporaryCursor = TextEditorCursor.Empty with
                    {
                        RowIndex = cursorModifier.RowIndex,
                        ColumnIndex = cursorModifier.ColumnIndex,
                        PreferredColumnIndex = cursorModifier.PreferredColumnIndex,
                        IsPrimaryCursor = cursorModifier.IsPrimaryCursor,
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

                        commandArgs.TextEditorService.ViewModelApi.MoveCursor(
                            keyboardEventArgs,
                            commandArgs.Model.ResourceUri,
                            commandArgs.ViewModel.ViewModelKey,
                            commandArgs.ViewModel.PrimaryCursor.Key);

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
                        cursorModifier.SelectionEndingPositionIndex =
                            commandArgs.Model.GetCursorPositionIndex(temporaryCursor);
                    }

                    cursorModifier.RowIndex = temporaryCursor.RowIndex;
                    cursorModifier.ColumnIndex = temporaryCursor.ColumnIndex;

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        }));
                });

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand GoToDefinition = new(
        "GoToDefinition", "defaults_go-to-definition", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            if (commandArgs.Model.CompilerService.Binder is null)
                return Task.CompletedTask;

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                    var positionIndex = commandArgs.Model.GetCursorPositionIndex(
                cursorModifier);

                    var wordTextSpan = commandArgs.Model.GetWordAt(positionIndex);

                    if (wordTextSpan is null)
                        return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(state => state));

                    var definitionTextSpan = commandArgs.Model.CompilerService.Binder.GetDefinition(
                        wordTextSpan);

                    if (definitionTextSpan is null)
                        return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(state => state));

                    var definitionModel = commandArgs.TextEditorService.ModelApi.FindOrDefault(definitionTextSpan.ResourceUri);

                    if (definitionModel is null)
                    {
                        if (commandArgs.RegisterModelAction is not null)
                        {
                            commandArgs.RegisterModelAction.Invoke(definitionTextSpan.ResourceUri);
                            definitionModel = commandArgs.TextEditorService.ModelApi.FindOrDefault(definitionTextSpan.ResourceUri);

                            if (definitionModel is null)
                                return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(state => state));
                        }
                        else
                        {
                            return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(state => state));
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
                                return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(state => state));
                        }
                        else
                        {
                            return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(state => state));
                        }
                    }

                    var firstDefinitionViewModel = definitionViewModels.First();
                    var rowData = definitionModel.FindRowInformation(definitionTextSpan.StartingIndexInclusive);
                    var columnIndex = definitionTextSpan.StartingIndexInclusive - rowData.rowStartPositionIndex;

                    var firstDefinitionViewModelCursorModifier = new TextEditorCursorModifier(firstDefinitionViewModel.PrimaryCursor);

                    firstDefinitionViewModelCursorModifier.RowIndex = rowData.rowIndex;
                    firstDefinitionViewModelCursorModifier.ColumnIndex = columnIndex;
                    firstDefinitionViewModelCursorModifier.PreferredColumnIndex = columnIndex;

                    commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                        commandArgs.ViewModel.ViewModelKey,
                        firstDefinitionInViewModel =>
                    {
                        var outCursor = firstDefinitionViewModelCursorModifier.ToCursor();
                        var outCursorBag = firstDefinitionInViewModel.CursorBag.Replace(firstDefinitionInViewModel.PrimaryCursor, outCursor);

                        return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                            state => state with
                            {
                                CursorBag = outCursorBag
                            }));
                    });

                    if (commandArgs.ShowViewModelAction is not null)
                        commandArgs.ShowViewModelAction.Invoke(firstDefinitionViewModel.ViewModelKey);

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        }));
                });

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ShowFindDialog = new(
        "OpenFindDialog", "defaults_open-find-dialog", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.TextEditorService.OptionsApi.ShowFindDialog();
            return Task.CompletedTask;
        });

    /// <summary>
    /// <see cref="ShowTooltipByCursorPosition"/> is to fire the @onmouseover event
    /// so to speak. Such that a tooltip appears if one were to have moused over a symbol or etc...
    /// </summary>
    public static readonly TextEditorCommand ShowTooltipByCursorPosition = new(
        "ShowTooltipByCursorPosition", "defaults_show-tooltip-by-cursor-position", false, true, TextEditKind.None, null,
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            if (commandArgs.JsRuntime is null ||
                commandArgs.HandleMouseStoppedMovingEventAsyncFunc is null)
            {
                return;
            }

            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                async inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

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

                    var outCursor = cursorModifier.ToCursor();
                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        });
                });
        });
}