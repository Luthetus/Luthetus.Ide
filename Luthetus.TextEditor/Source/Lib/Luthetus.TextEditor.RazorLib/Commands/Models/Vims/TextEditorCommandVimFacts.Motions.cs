using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static class Motions
    {
        public static readonly TextEditorCommand Word = new(
            "Vim::Word()", "Vim::Word()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                    commandArgs.ViewModel.ViewModelKey,
                    inViewModel =>
                    {
                        var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);
                        var model = commandArgs.Model;

                        var localRowIndex = cursorModifier.RowIndex;
                        var localColumnIndex = cursorModifier.ColumnIndex;
                        var localPreferredColumnIndex = cursorModifier.PreferredColumnIndex;

                        void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                        {
                            localColumnIndex = columnIndex;
                            localPreferredColumnIndex = columnIndex;
                        }

                        var lengthOfRow = model.GetLengthOfRow(localRowIndex);

                        if (localColumnIndex == lengthOfRow &&
                            localRowIndex < model.RowCount - 1)
                        {
                            MutateIndexCoordinatesAndPreferredColumnIndex(0);
                            localRowIndex++;
                        }
                        else if (localColumnIndex != lengthOfRow)
                        {
                            var columnIndexOfCharacterWithDifferingKind = model.GetColumnIndexOfCharacterWithDifferingKind(
                                localRowIndex,
                                localColumnIndex,
                                false);

                            if (columnIndexOfCharacterWithDifferingKind == -1)
                                MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                            else
                            {
                                MutateIndexCoordinatesAndPreferredColumnIndex(
                                    columnIndexOfCharacterWithDifferingKind);
                            }
                        }

                        cursorModifier.RowIndex = localRowIndex;
                        cursorModifier.ColumnIndex = localColumnIndex;
                        cursorModifier.PreferredColumnIndex = localPreferredColumnIndex;

                        if (TextEditorSelectionHelper.HasSelectedText(cursorModifier))
                        {
                            cursorModifier.SelectionEndingPositionIndex = model.GetPositionIndex(
                                cursorModifier.RowIndex, cursorModifier.ColumnIndex);
                        }

                        cursorModifier.PreferredColumnIndex = localPreferredColumnIndex;

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

        public static readonly TextEditorCommand End = new(
            "Vim::End()", "Vim::End()", false, true, TextEditKind.None, null,
            async interfaceCommandArgs => await PerformEnd((TextEditorCommandArgs)interfaceCommandArgs));

        public static async Task PerformEnd(
            TextEditorCommandArgs commandArgs,
            bool isRecursiveCall = false)
        {
            commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                commandArgs.ViewModel.ViewModelKey,
                async inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);
                    var model = commandArgs.Model;

                    var localRowIndex = cursorModifier.RowIndex;
                    var localColumnIndex = cursorModifier.ColumnIndex;
                    var localPreferredColumnIndex = cursorModifier.PreferredColumnIndex;

                    void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                    {
                        localColumnIndex = columnIndex;
                        localPreferredColumnIndex = columnIndex;
                    }

                    var lengthOfRow = model.GetLengthOfRow(localRowIndex);

                    if (localColumnIndex == lengthOfRow &&
                        localRowIndex < model.RowCount - 1)
                    {
                        MutateIndexCoordinatesAndPreferredColumnIndex(0);
                        localRowIndex++;
                    }
                    else if (localColumnIndex != lengthOfRow)
                    {
                        var columnIndexOfCharacterWithDifferingKind = model.GetColumnIndexOfCharacterWithDifferingKind(
                            localRowIndex,
                            localColumnIndex,
                            false);

                        if (columnIndexOfCharacterWithDifferingKind == -1)
                        {
                            MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                        }
                        else
                        {
                            var columnsToMoveBy = columnIndexOfCharacterWithDifferingKind -
                                localColumnIndex;

                            MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);

                            if (columnsToMoveBy > 1)
                            {
                                MutateIndexCoordinatesAndPreferredColumnIndex(localColumnIndex - 1);
                            }
                            else if (columnsToMoveBy == 1 && !isRecursiveCall)
                            {
                                // Persist state of the first invocation
                                cursorModifier.RowIndex = localRowIndex;
                                cursorModifier.ColumnIndex = localColumnIndex;
                                cursorModifier.PreferredColumnIndex = localPreferredColumnIndex;

                                var positionIndex = model.GetCursorPositionIndex(cursorModifier);
                                var currentCharacterKind = model.GetCharacterKindAt(positionIndex);
                                var nextCharacterKind = model.GetCharacterKindAt(positionIndex + 1);

                                if (nextCharacterKind != CharacterKind.Bad &&
                                    currentCharacterKind == nextCharacterKind)
                                {
                                    /*
                                     * If the cursor is at the end of a word. Then the first End(...)
                                     * invocation will move the cursor to the next word.
                                     *
                                     * One must invoke the End(...) method a second time however because they
                                     * will erroneously be at the start of the next word otherwise.
                                     */

                                    await PerformEnd(commandArgs, isRecursiveCall: true);

                                    // Leave method early as all is finished.
                                    return state => state;
                                }
                            }
                        }
                    }

                    cursorModifier.RowIndex = localRowIndex;
                    cursorModifier.ColumnIndex = localColumnIndex;
                    cursorModifier.PreferredColumnIndex = localPreferredColumnIndex;

                    if (TextEditorSelectionHelper.HasSelectedText(cursorModifier))
                    {
                        cursorModifier.SelectionEndingPositionIndex = model.GetCursorPositionIndex(cursorModifier);
                    }

                    var outCursor = cursorModifier.ToCursor();

                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        });
                });
        }

        public static readonly TextEditorCommand Back = new(
            "Vim::Back()", "Vim::Back()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                    commandArgs.ViewModel.ViewModelKey,
                    async inViewModel =>
                {
                    var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);
                    var model = commandArgs.Model;

                    var localRowIndex = cursorModifier.RowIndex;
                    var localColumnIndex = cursorModifier.ColumnIndex;
                    var localPreferredColumnIndex = cursorModifier.PreferredColumnIndex;

                    void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                    {
                        localColumnIndex = columnIndex;
                        localPreferredColumnIndex = columnIndex;
                    }

                    if (localColumnIndex == 0)
                    {
                        if (localRowIndex != 0)
                        {
                            localRowIndex--;

                            var lengthOfRow = model.GetLengthOfRow(localRowIndex);

                            MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                        }
                    }
                    else
                    {
                        var columnIndexOfCharacterWithDifferingKind = model.GetColumnIndexOfCharacterWithDifferingKind(
                            localRowIndex,
                            localColumnIndex,
                            true);

                        if (columnIndexOfCharacterWithDifferingKind == -1)
                            MutateIndexCoordinatesAndPreferredColumnIndex(0);
                        else
                        {
                            MutateIndexCoordinatesAndPreferredColumnIndex(
                                columnIndexOfCharacterWithDifferingKind);
                        }
                    }

                    cursorModifier.RowIndex = localRowIndex;
                    cursorModifier.ColumnIndex = localColumnIndex;
                    cursorModifier.PreferredColumnIndex = localPreferredColumnIndex;

                    if (TextEditorSelectionHelper.HasSelectedText(cursorModifier))
                    {
                        cursorModifier.SelectionEndingPositionIndex =
                            model.GetCursorPositionIndex(cursorModifier);
                    }

                    var outCursor = cursorModifier.ToCursor();

                    var outCursorBag = inViewModel.CursorBag.Replace(inViewModel.PrimaryCursor, outCursor);

                    return new Func<TextEditorViewModel, TextEditorViewModel>(
                        state => state with
                        {
                            CursorBag = outCursorBag
                        });
                });

                return Task.CompletedTask;
            });

        public static TextEditorCommand GetVisual(
            TextEditorCommand textEditorCommandMotion,
            string displayName)
        {
            return new TextEditorCommand(
                $"Vim::GetVisual({displayName})", $"Vim::GetVisual({displayName})", false, true, TextEditKind.None, null,
                async interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    
                    commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                        commandArgs.ViewModel.ViewModelKey,
                        async inViewModel =>
                        {
                            var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                            var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                                ?? TextEditorKeymapFacts.DefaultKeymap;

                            if (activeKeymap is not TextEditorKeymapVim keymapVim)
                                return state => state;

                            var previousAnchorPositionIndex = commandArgs
                                .PrimaryCursor.Selection.AnchorPositionIndex;

                            var previousEndingPositionIndex = commandArgs
                                .PrimaryCursor.Selection.EndingPositionIndex;

                            await textEditorCommandMotion.DoAsyncFunc.Invoke(commandArgs);

                            var nextEndingPositionIndex = commandArgs
                                .PrimaryCursor.Selection.EndingPositionIndex;

                            if (nextEndingPositionIndex < cursorModifier.SelectionAnchorPositionIndex)
                            {
                                if (previousAnchorPositionIndex < previousEndingPositionIndex)
                                {
                                    // Anchor went from being the lower bound to the upper bound.
                                    cursorModifier.SelectionAnchorPositionIndex += 1;
                                }
                            }
                            else if (nextEndingPositionIndex >= cursorModifier.SelectionAnchorPositionIndex)
                            {
                                if (previousAnchorPositionIndex > previousEndingPositionIndex)
                                {
                                    // Anchor went from being the upper bound to the lower bound.
                                    cursorModifier.SelectionAnchorPositionIndex -= 1;
                                }

                                cursorModifier.SelectionEndingPositionIndex += 1;
                            }

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

        public static TextEditorCommand GetVisualLine(
            TextEditorCommand textEditorCommandMotion,
            string displayName)
        {
            return new TextEditorCommand(
                $"Vim::GetVisual({displayName})", $"Vim::GetVisual({displayName})", false, true, TextEditKind.None, null,
                async interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.ViewModelApi.SetViewModelWith(
                        commandArgs.ViewModel.ViewModelKey,
                        async inViewModel =>
                        {
                            var cursorModifier = new TextEditorCursorModifier(commandArgs.PrimaryCursor);

                            var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                                ?? TextEditorKeymapFacts.DefaultKeymap;

                            if (activeKeymap is not TextEditorKeymapVim keymapVim)
                                return state => state;

                            var previousAnchorPositionIndex = commandArgs
                                .PrimaryCursor.Selection.AnchorPositionIndex;

                            var previousEndingPositionIndex = commandArgs
                                .PrimaryCursor.Selection.EndingPositionIndex;

                            await textEditorCommandMotion.DoAsyncFunc.Invoke(commandArgs);

                            var nextEndingPositionIndex = commandArgs
                                .PrimaryCursor.Selection.EndingPositionIndex;

                            if (nextEndingPositionIndex < cursorModifier.SelectionAnchorPositionIndex)
                            {
                                if (previousAnchorPositionIndex < previousEndingPositionIndex)
                                {
                                    // Anchor went from being the lower bound to the upper bound.

                                    var rowDataAnchorIsOn = commandArgs.Model.FindRowInformation(previousAnchorPositionIndex.Value);

                                    cursorModifier.SelectionAnchorPositionIndex =
                                        commandArgs.Model.RowEndingPositionsBag[rowDataAnchorIsOn.rowIndex].positionIndex;
                                }

                                var startingPositionOfRow =
                                    commandArgs.Model.GetStartOfRowTuple(cursorModifier.RowIndex)
                                    .positionIndex;

                                cursorModifier.SelectionEndingPositionIndex =
                                    startingPositionOfRow;
                            }
                            else if (nextEndingPositionIndex >= cursorModifier.SelectionAnchorPositionIndex)
                            {
                                if (previousAnchorPositionIndex > previousEndingPositionIndex)
                                {
                                    // Anchor went from being the upper bound to the lower bound.

                                    var rowDataAnchorIsOn = commandArgs.Model.FindRowInformation(previousAnchorPositionIndex.Value);

                                    cursorModifier.SelectionAnchorPositionIndex =
                                        commandArgs.Model.GetStartOfRowTuple(rowDataAnchorIsOn.rowIndex - 1)
                                        .positionIndex;
                                }

                                var endingPositionOfRow = commandArgs.Model.RowEndingPositionsBag[
                                    cursorModifier.RowIndex]
                                    .positionIndex;

                                cursorModifier.SelectionEndingPositionIndex = endingPositionOfRow;
                            }

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
    }
}