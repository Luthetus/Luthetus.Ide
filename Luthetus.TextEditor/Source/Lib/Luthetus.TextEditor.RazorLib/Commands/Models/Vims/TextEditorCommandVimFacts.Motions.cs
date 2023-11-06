using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static class Motions
    {
        public static readonly TextEditorCommand Word = new(interfaceCommandParameter =>
            {
                var commandParameter = (TextEditorCommandArgs)interfaceCommandParameter;
                var textEditorCursor = commandParameter.PrimaryCursorSnapshot.UserCursor;
                var textEditorModel = commandParameter.Model;

                var localIndexCoordinates = textEditorCursor.IndexCoordinates;
                var localPreferredColumnIndex = textEditorCursor.PreferredColumnIndex;

                void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                {
                    localIndexCoordinates.columnIndex = columnIndex;
                    localPreferredColumnIndex = columnIndex;
                }

                var lengthOfRow = textEditorModel.GetLengthOfRow(localIndexCoordinates.rowIndex);

                if (localIndexCoordinates.columnIndex == lengthOfRow &&
                    localIndexCoordinates.rowIndex < textEditorModel.RowCount - 1)
                {
                    MutateIndexCoordinatesAndPreferredColumnIndex(0);
                    localIndexCoordinates.rowIndex++;
                }
                else if (localIndexCoordinates.columnIndex != lengthOfRow)
                {
                    var columnIndexOfCharacterWithDifferingKind = textEditorModel.GetColumnIndexOfCharacterWithDifferingKind(
                        localIndexCoordinates.rowIndex,
                        localIndexCoordinates.columnIndex,
                        false);

                    if (columnIndexOfCharacterWithDifferingKind == -1)
                        MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                    else
                    {
                        MutateIndexCoordinatesAndPreferredColumnIndex(
                            columnIndexOfCharacterWithDifferingKind);
                    }
                }

                textEditorCursor.IndexCoordinates = localIndexCoordinates;
                textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;

                if (TextEditorSelectionHelper.HasSelectedText(textEditorCursor.Selection))
                {
                    textEditorCursor.Selection.EndingPositionIndex =
                        textEditorModel.GetCursorPositionIndex(textEditorCursor);
                }

                textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;
                return Task.CompletedTask;
            },
            true,
            "Vim::Word()",
            "Vim::Word()");

        public static readonly TextEditorCommand End = new(
            async interfaceCommandParameter => await PerformEnd((TextEditorCommandArgs)interfaceCommandParameter),
            true,
            "Vim::End()",
            "Vim::End()");

        public static async Task PerformEnd(
            TextEditorCommandArgs commandParameter,
            bool isRecursiveCall = false)
        {
            var textEditorCursor = commandParameter.PrimaryCursorSnapshot.UserCursor;
            var textEditorModel = commandParameter.Model;

            var localIndexCoordinates = textEditorCursor.IndexCoordinates;
            var localPreferredColumnIndex = textEditorCursor.PreferredColumnIndex;

            void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
            {
                localIndexCoordinates.columnIndex = columnIndex;
                localPreferredColumnIndex = columnIndex;
            }

            var lengthOfRow = textEditorModel.GetLengthOfRow(localIndexCoordinates.rowIndex);

            if (localIndexCoordinates.columnIndex == lengthOfRow &&
                localIndexCoordinates.rowIndex < textEditorModel.RowCount - 1)
            {
                MutateIndexCoordinatesAndPreferredColumnIndex(0);
                localIndexCoordinates.rowIndex++;
            }
            else if (localIndexCoordinates.columnIndex != lengthOfRow)
            {
                var columnIndexOfCharacterWithDifferingKind = textEditorModel.GetColumnIndexOfCharacterWithDifferingKind(
                    localIndexCoordinates.rowIndex,
                    localIndexCoordinates.columnIndex,
                    false);

                if (columnIndexOfCharacterWithDifferingKind == -1)
                {
                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                }
                else
                {
                    var columnsToMoveBy = columnIndexOfCharacterWithDifferingKind -
                        localIndexCoordinates.columnIndex;

                    MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);

                    if (columnsToMoveBy > 1)
                    {
                        MutateIndexCoordinatesAndPreferredColumnIndex(localIndexCoordinates.columnIndex - 1);
                    }
                    else if (columnsToMoveBy == 1 && !isRecursiveCall)
                    {
                        // Persist state of the first invocation
                        textEditorCursor.IndexCoordinates = localIndexCoordinates;
                        textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;

                        var positionIndex = textEditorModel.GetCursorPositionIndex(textEditorCursor);
                        var currentCharacterKind = textEditorModel.GetCharacterKindAt(positionIndex);
                        var nextCharacterKind = textEditorModel.GetCharacterKindAt(positionIndex + 1);

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

                            await PerformEnd(commandParameter, isRecursiveCall: true);

                            // Leave method early as all is finished.
                            return;
                        }
                    }
                }
            }

            textEditorCursor.IndexCoordinates = localIndexCoordinates;
            textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;

            if (TextEditorSelectionHelper.HasSelectedText(textEditorCursor.Selection))
            {
                textEditorCursor.Selection.EndingPositionIndex =
                    textEditorModel.GetCursorPositionIndex(textEditorCursor);
            }
        }

        public static readonly TextEditorCommand Back = new(interfaceCommandParameter =>
            {
                var commandParameter = (TextEditorCommandArgs)interfaceCommandParameter;

                var textEditorCursor = commandParameter.PrimaryCursorSnapshot.UserCursor;
                var textEditorModel = commandParameter.Model;

                var localIndexCoordinates = textEditorCursor.IndexCoordinates;
                var localPreferredColumnIndex = textEditorCursor.PreferredColumnIndex;

                void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                {
                    localIndexCoordinates.columnIndex = columnIndex;
                    localPreferredColumnIndex = columnIndex;
                }

                if (localIndexCoordinates.columnIndex == 0)
                {
                    if (localIndexCoordinates.rowIndex != 0)
                    {
                        localIndexCoordinates.rowIndex--;

                        var lengthOfRow = textEditorModel.GetLengthOfRow(localIndexCoordinates.rowIndex);

                        MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                    }
                }
                else
                {
                    var columnIndexOfCharacterWithDifferingKind = textEditorModel.GetColumnIndexOfCharacterWithDifferingKind(
                        localIndexCoordinates.rowIndex,
                        localIndexCoordinates.columnIndex,
                        true);

                    if (columnIndexOfCharacterWithDifferingKind == -1)
                        MutateIndexCoordinatesAndPreferredColumnIndex(0);
                    else
                    {
                        MutateIndexCoordinatesAndPreferredColumnIndex(
                            columnIndexOfCharacterWithDifferingKind);
                    }
                }

                textEditorCursor.IndexCoordinates = localIndexCoordinates;
                textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;

                if (TextEditorSelectionHelper.HasSelectedText(textEditorCursor.Selection))
                {
                    textEditorCursor.Selection.EndingPositionIndex =
                        textEditorModel.GetCursorPositionIndex(textEditorCursor);
                }

                return Task.CompletedTask;
            },
            true,
            "Vim::Back()",
            "Vim::Back()");

        public static TextEditorCommand GetVisual(
            TextEditorCommand textEditorCommandMotion,
            string displayName)
        {
            return new TextEditorCommand(async interfaceCommandParameter =>
            {
                var commandParameter = (TextEditorCommandArgs)interfaceCommandParameter;

                var activeKeymap = commandParameter.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim keymapVim)
                    return;

                var previousAnchorPositionIndex = commandParameter
                    .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex;

                var previousEndingPositionIndex = commandParameter
                    .PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex;

                await textEditorCommandMotion.DoAsyncFunc.Invoke(commandParameter);

                var nextEndingPositionIndex = commandParameter
                    .PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex;

                if (nextEndingPositionIndex < commandParameter
                        .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex)
                {
                    if (previousAnchorPositionIndex < previousEndingPositionIndex)
                    {
                        // Anchor went from being the lower bound to the upper bound.

                        commandParameter
                            .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex += 1;
                    }
                }
                else if (nextEndingPositionIndex >= commandParameter
                                .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex)
                {
                    if (previousAnchorPositionIndex > previousEndingPositionIndex)
                    {
                        // Anchor went from being the upper bound to the lower bound.

                        commandParameter
                            .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex -= 1;
                    }

                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection
                        .EndingPositionIndex += 1;
                }
            },
            true,
            $"Vim::GetVisual({displayName})",
            $"Vim::GetVisual({displayName})");
        }

        public static TextEditorCommand GetVisualLine(
            TextEditorCommand textEditorCommandMotion,
            string displayName)
        {
            return new TextEditorCommand(async interfaceCommandParameter =>
            {
                var commandParameter = (TextEditorCommandArgs)interfaceCommandParameter;

                var activeKeymap = commandParameter.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim keymapVim)
                    return;

                var previousAnchorPositionIndex = commandParameter
                    .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex;

                var previousEndingPositionIndex = commandParameter
                    .PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex;

                await textEditorCommandMotion.DoAsyncFunc.Invoke(commandParameter);

                var nextEndingPositionIndex = commandParameter
                    .PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex;

                if (nextEndingPositionIndex < commandParameter
                        .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex)
                {
                    if (previousAnchorPositionIndex < previousEndingPositionIndex)
                    {
                        // Anchor went from being the lower bound to the upper bound.

                        var rowDataAnchorIsOn = commandParameter.Model.FindRowInformation(
                            previousAnchorPositionIndex.Value);

                        commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex =
                            commandParameter.Model.RowEndingPositionsBag[rowDataAnchorIsOn.rowIndex].positionIndex;
                    }

                    var startingPositionOfRow = commandParameter.Model.GetStartOfRowTuple(
                            commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.rowIndex)
                        .positionIndex;

                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex =
                        startingPositionOfRow;
                }
                else if (nextEndingPositionIndex >= commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex)
                {
                    if (previousAnchorPositionIndex > previousEndingPositionIndex)
                    {
                        // Anchor went from being the upper bound to the lower bound.

                        var rowDataAnchorIsOn = commandParameter.Model.FindRowInformation(
                            previousAnchorPositionIndex.Value);

                        commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex =
                            commandParameter.Model.GetStartOfRowTuple(rowDataAnchorIsOn.rowIndex - 1)
                                .positionIndex;
                    }

                    var endingPositionOfRow = commandParameter.Model.RowEndingPositionsBag[
                            commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.rowIndex]
                        .positionIndex;

                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex =
                        endingPositionOfRow;
                }
            },
            true,
            $"Vim::GetVisual({displayName})",
            $"Vim::GetVisual({displayName})");
        }
    }
}