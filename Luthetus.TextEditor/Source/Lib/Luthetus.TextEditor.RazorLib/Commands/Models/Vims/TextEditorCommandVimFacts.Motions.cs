using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using static Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorService;

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

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    new List<TextEditorCursorModifier>());

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(Word),
                    refreshCursorsRequest,
                    () =>
                    {
                        var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                        var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                        var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                        if (viewModel is null || model is null || cursor is null)
                            return Task.CompletedTask;

                        void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                        {
                            cursor.ColumnIndex = columnIndex;
                            cursor.PreferredColumnIndex = columnIndex;
                        }

                        var lengthOfRow = model.GetLengthOfRow(cursor.RowIndex);

                        if (cursor.ColumnIndex == lengthOfRow &&
                            cursor.RowIndex < model.RowCount - 1)
                        {
                            MutateIndexCoordinatesAndPreferredColumnIndex(0);
                            cursor.RowIndex++;
                        }
                        else if (cursor.ColumnIndex != lengthOfRow)
                        {
                            var columnIndexOfCharacterWithDifferingKind = model.GetColumnIndexOfCharacterWithDifferingKind(
                                cursor.RowIndex,
                                cursor.ColumnIndex,
                                false);

                            if (columnIndexOfCharacterWithDifferingKind == -1)
                                MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                            else
                            {
                                MutateIndexCoordinatesAndPreferredColumnIndex(
                                    columnIndexOfCharacterWithDifferingKind);
                            }
                        }

                        if (TextEditorSelectionHelper.HasSelectedText(cursor))
                        {
                            cursor.SelectionEndingPositionIndex = model.GetPositionIndex(
                                cursor.RowIndex, cursor.ColumnIndex);
                        }

                        return Task.CompletedTask;
                    });

                return Task.CompletedTask;
            });

        public static readonly TextEditorCommand End = new(
            "Vim::End()", "Vim::End()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var refreshCursorsRequest = new RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    new List<TextEditorCursorModifier>());

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(End),
                    refreshCursorsRequest,
                    async () => await PerformEnd(commandArgs, refreshCursorsRequest));

                return Task.CompletedTask;
            });

        public static async Task PerformEnd(
            TextEditorCommandArgs commandArgs,
            RefreshCursorsRequest refreshCursorsRequest,
            bool isRecursiveCall = false)
        {
            var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
            var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
            var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

            if (viewModel is null || model is null || cursor is null)
                return;

            void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
            {
                cursor.ColumnIndex = columnIndex;
                cursor.PreferredColumnIndex = columnIndex;
            }

            var lengthOfRow = model.GetLengthOfRow(cursor.RowIndex);

            if (cursor.ColumnIndex == lengthOfRow &&
                cursor.RowIndex < model.RowCount - 1)
            {
                MutateIndexCoordinatesAndPreferredColumnIndex(0);
                cursor.RowIndex++;
            }
            else if (cursor.ColumnIndex != lengthOfRow)
            {
                var columnIndexOfCharacterWithDifferingKind = model.GetColumnIndexOfCharacterWithDifferingKind(
                    cursor.RowIndex,
                    cursor.ColumnIndex,
                    false);

                if (columnIndexOfCharacterWithDifferingKind == -1)
                {
                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                }
                else
                {
                    var columnsToMoveBy = columnIndexOfCharacterWithDifferingKind -
                        cursor.ColumnIndex;

                    MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);

                    if (columnsToMoveBy > 1)
                    {
                        MutateIndexCoordinatesAndPreferredColumnIndex(cursor.ColumnIndex - 1);
                    }
                    else if (columnsToMoveBy == 1 && !isRecursiveCall)
                    {
                        var positionIndex = model.GetCursorPositionIndex(cursor);
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

                            await PerformEnd(commandArgs, refreshCursorsRequest, isRecursiveCall: true);

                            // Leave method early as all is finished.
                            return;
                        }
                    }
                }
            }

            if (TextEditorSelectionHelper.HasSelectedText(cursor))
                cursor.SelectionEndingPositionIndex = model.GetCursorPositionIndex(cursor);
        }

        public static readonly TextEditorCommand Back = new(
            "Vim::Back()", "Vim::Back()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var refreshCursorsRequest = new RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    new List<TextEditorCursorModifier>());

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(Back),
                    refreshCursorsRequest,
                    () =>
                    {
                        var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                        var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                        var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                        if (viewModel is null || model is null || cursor is null)
                            return Task.CompletedTask;

                        void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                        {
                            cursor.ColumnIndex = columnIndex;
                            cursor.PreferredColumnIndex = columnIndex;
                        }

                        if (cursor.ColumnIndex == 0)
                        {
                            if (cursor.RowIndex != 0)
                            {
                                cursor.RowIndex--;

                                var lengthOfRow = model.GetLengthOfRow(cursor.RowIndex);

                                MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                            }
                        }
                        else
                        {
                            var columnIndexOfCharacterWithDifferingKind = model.GetColumnIndexOfCharacterWithDifferingKind(
                                cursor.RowIndex,
                                cursor.ColumnIndex,
                                true);

                            if (columnIndexOfCharacterWithDifferingKind == -1)
                                MutateIndexCoordinatesAndPreferredColumnIndex(0);
                            else
                            {
                                MutateIndexCoordinatesAndPreferredColumnIndex(
                                    columnIndexOfCharacterWithDifferingKind);
                            }
                        }

                        if (TextEditorSelectionHelper.HasSelectedText(cursor))
                        {
                            cursor.SelectionEndingPositionIndex =
                                model.GetCursorPositionIndex(cursor);
                        }

                        return Task.CompletedTask;
                    });

                return Task.CompletedTask;
            });

        public static TextEditorCommand GetVisual(
            TextEditorCommand textEditorCommandMotion,
            string displayName)
        {
            return new TextEditorCommand(
                $"Vim::GetVisual({displayName})", $"Vim::GetVisual({displayName})", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    var refreshCursorsRequest = new RefreshCursorsRequest(
                        commandArgs.ViewModelKey,
                        new List<TextEditorCursorModifier>());

                    commandArgs.TextEditorService.EnqueueModification(
                        nameof(GetVisual),
                        refreshCursorsRequest,
                        async () =>
                        {
                            var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                            var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                            var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                            if (viewModel is null || model is null || cursor is null)
                                return;

                            var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                                ?? TextEditorKeymapFacts.DefaultKeymap;

                            if (activeKeymap is not TextEditorKeymapVim keymapVim)
                                return;

                            var previousAnchorPositionIndex = cursor.SelectionAnchorPositionIndex;
                            var previousEndingPositionIndex = cursor.SelectionEndingPositionIndex;

                            await textEditorCommandMotion.DoAsyncFunc.Invoke(commandArgs);

                            var nextEndingPositionIndex = cursor.SelectionEndingPositionIndex;

                            if (nextEndingPositionIndex < cursor.SelectionAnchorPositionIndex)
                            {
                                if (previousAnchorPositionIndex < previousEndingPositionIndex)
                                {
                                    // Anchor went from being the lower bound to the upper bound.
                                    cursor.SelectionAnchorPositionIndex += 1;
                                }
                            }
                            else if (nextEndingPositionIndex >= cursor.SelectionAnchorPositionIndex)
                            {
                                if (previousAnchorPositionIndex > previousEndingPositionIndex)
                                {
                                    // Anchor went from being the upper bound to the lower bound.
                                    cursor.SelectionAnchorPositionIndex -= 1;
                                }

                                cursor.SelectionEndingPositionIndex += 1;
                            }
                        });

                    return Task.CompletedTask;
                });
        }

        public static TextEditorCommand GetVisualLine(
            TextEditorCommand textEditorCommandMotion,
            string displayName)
        {
            return new TextEditorCommand(
                $"Vim::GetVisual({displayName})", $"Vim::GetVisual({displayName})", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    var refreshCursorsRequest = new RefreshCursorsRequest(
                        commandArgs.ViewModelKey,
                        new List<TextEditorCursorModifier>());

                    commandArgs.TextEditorService.EnqueueModification(
                        nameof(GetVisualLine),
                        refreshCursorsRequest,
                        async () =>
                        {
                            var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                            var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                            var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                            if (viewModel is null || model is null || cursor is null)
                                return;

                            var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                                ?? TextEditorKeymapFacts.DefaultKeymap;

                            if (activeKeymap is not TextEditorKeymapVim keymapVim)
                                return;

                            var previousAnchorPositionIndex = cursor.SelectionAnchorPositionIndex;
                            var previousEndingPositionIndex = cursor.SelectionEndingPositionIndex;

                            await textEditorCommandMotion.DoAsyncFunc.Invoke(commandArgs);

                            var nextEndingPositionIndex = cursor.SelectionEndingPositionIndex;

                            if (nextEndingPositionIndex < cursor.SelectionAnchorPositionIndex)
                            {
                                if (previousAnchorPositionIndex < previousEndingPositionIndex)
                                {
                                    // Anchor went from being the lower bound to the upper bound.
                                    var rowDataAnchorIsOn = model.FindRowInformation(previousAnchorPositionIndex.Value);

                                    cursor.SelectionAnchorPositionIndex = model.RowEndingPositionsBag[
                                        rowDataAnchorIsOn.rowIndex].positionIndex;
                                }

                                var startingPositionOfRow = model.GetStartOfRowTuple(cursor.RowIndex)
                                    .positionIndex;

                                cursor.SelectionEndingPositionIndex = startingPositionOfRow;
                            }
                            else if (nextEndingPositionIndex >= cursor.SelectionAnchorPositionIndex)
                            {
                                if (previousAnchorPositionIndex > previousEndingPositionIndex)
                                {
                                    // Anchor went from being the upper bound to the lower bound.
                                    var rowDataAnchorIsOn = model.FindRowInformation(previousAnchorPositionIndex.Value);

                                    cursor.SelectionAnchorPositionIndex = model.GetStartOfRowTuple(rowDataAnchorIsOn.rowIndex - 1)
                                        .positionIndex;
                                }

                                var endingPositionOfRow = model.RowEndingPositionsBag[cursor.RowIndex]
                                    .positionIndex;

                                cursor.SelectionEndingPositionIndex = endingPositionOfRow;
                            }
                        });

                    return Task.CompletedTask;
                });
        }
    }
}