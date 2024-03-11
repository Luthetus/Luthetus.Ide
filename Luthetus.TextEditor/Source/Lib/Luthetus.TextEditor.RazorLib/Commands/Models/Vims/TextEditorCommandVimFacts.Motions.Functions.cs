using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static partial class Motions
    {
        public static TextEditorEdit WordFactory(TextEditorCommandArgs commandArgs)
        {
            return (ITextEditorEditContext editContext) =>
            {
                var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                {
                    primaryCursorModifier.ColumnIndex = columnIndex;
                    primaryCursorModifier.PreferredColumnIndex = columnIndex;
                }

                var lengthOfRow = modelModifier.GetLengthOfRow(primaryCursorModifier.RowIndex);

                if (primaryCursorModifier.ColumnIndex == lengthOfRow &&
                    primaryCursorModifier.RowIndex < modelModifier.RowCount - 1)
                {
                    MutateIndexCoordinatesAndPreferredColumnIndex(0);
                    primaryCursorModifier.RowIndex++;
                }
                else if (primaryCursorModifier.ColumnIndex != lengthOfRow)
                {
                    var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                        primaryCursorModifier.RowIndex,
                        primaryCursorModifier.ColumnIndex,
                        false);

                    if (columnIndexOfCharacterWithDifferingKind == -1)
                        MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                    else
                        MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);
                }

                if (TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier))
                {
                    primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(
                        primaryCursorModifier.RowIndex,
                        primaryCursorModifier.ColumnIndex);
                }

                return Task.CompletedTask;
            };
        }

        public static TextEditorEdit EndFactory(TextEditorCommandArgs commandArgs)
        {
            return async (ITextEditorEditContext editContext) =>
            {
                await PerformEndAsync(commandArgs, editContext).ConfigureAwait(false);
                return;
            };
        }

        private static async Task PerformEndAsync(
            TextEditorCommandArgs commandArgs, ITextEditorEditContext editContext, bool isRecursiveCall = false)
        {
            var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return;

            void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
            {
                primaryCursorModifier.ColumnIndex = columnIndex;
                primaryCursorModifier.PreferredColumnIndex = columnIndex;
            }

            var lengthOfRow = modelModifier.GetLengthOfRow(primaryCursorModifier.RowIndex);

            if (primaryCursorModifier.ColumnIndex == lengthOfRow &&
                primaryCursorModifier.RowIndex < modelModifier.RowCount - 1)
            {
                MutateIndexCoordinatesAndPreferredColumnIndex(0);
                primaryCursorModifier.RowIndex++;
            }
            else if (primaryCursorModifier.ColumnIndex != lengthOfRow)
            {
                var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                    primaryCursorModifier.RowIndex,
                    primaryCursorModifier.ColumnIndex,
                    false);

                if (columnIndexOfCharacterWithDifferingKind == -1)
                {
                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                }
                else
                {
                    var columnsToMoveBy = columnIndexOfCharacterWithDifferingKind - primaryCursorModifier.ColumnIndex;
                    
                    MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);

                    if (columnsToMoveBy > 1)
                    {
                        MutateIndexCoordinatesAndPreferredColumnIndex(primaryCursorModifier.ColumnIndex - 1);
                    }
                    else if (columnsToMoveBy == 1 && !isRecursiveCall)
                    {
                        var positionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
                        var currentCharacterKind = modelModifier.GetCharacterKind(positionIndex);
                        var nextCharacterKind = modelModifier.GetCharacterKind(positionIndex + 1);

                        if (nextCharacterKind != CharacterKind.Bad && currentCharacterKind == nextCharacterKind)
                        {
                            /*
                             * If the cursor is at the end of a word. Then the first End(...)
                             * invocation will move the cursor to the next word.
                             *
                             * One must invoke the End(...) method a second time however because they
                             * will erroneously be at the start of the next word otherwise.
                             */

                            await PerformEndAsync(
                                    commandArgs,
                                    editContext,
                                    isRecursiveCall: true)
                                .ConfigureAwait(false);

                            // Leave method early as all is finished.
                            return;
                        }
                    }
                }
            }

            if (TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier))
                primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
        }

        public static TextEditorEdit BackFactory(TextEditorCommandArgs commandArgs)
        {
            return (ITextEditorEditContext editContext) =>
            {
                var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                {
                    primaryCursorModifier.ColumnIndex = columnIndex;
                    primaryCursorModifier.PreferredColumnIndex = columnIndex;
                }

                if (primaryCursorModifier.ColumnIndex == 0)
                {
                    if (primaryCursorModifier.RowIndex != 0)
                    {
                        primaryCursorModifier.RowIndex--;

                        var lengthOfRow = modelModifier.GetLengthOfRow(primaryCursorModifier.RowIndex);

                        MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                    }
                }
                else
                {
                    var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                        primaryCursorModifier.RowIndex,
                        primaryCursorModifier.ColumnIndex,
                        true);

                    if (columnIndexOfCharacterWithDifferingKind == -1)
                        MutateIndexCoordinatesAndPreferredColumnIndex(0);
                    else
                        MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);
                }

                if (TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier))
                    primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);

                return Task.CompletedTask;
            };
        }

        public static TextEditorEdit VisualFactory(TextEditorCommandArgs commandArgs)
        {
            return async (ITextEditorEditContext editContext) =>
            {
                var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim keymapVim)
                    return;

                var previousAnchorPositionIndex = primaryCursorModifier.SelectionAnchorPositionIndex;
                var previousEndingPositionIndex = primaryCursorModifier.SelectionEndingPositionIndex;

                if (commandArgs.InnerCommand.TextEditorEditFactory is null)
                    return;

                var textEditorEdit = commandArgs.InnerCommand.TextEditorEditFactory.Invoke(commandArgs);
                await textEditorEdit.Invoke(editContext).ConfigureAwait(false);

                var nextEndingPositionIndex = primaryCursorModifier.SelectionEndingPositionIndex;

                if (nextEndingPositionIndex < primaryCursorModifier.SelectionAnchorPositionIndex)
                {
                    if (previousAnchorPositionIndex < previousEndingPositionIndex)
                    {
                        // Anchor went from being the lower bound to the upper bound.
                        primaryCursorModifier.SelectionAnchorPositionIndex += 1;
                    }
                }
                else if (nextEndingPositionIndex >= primaryCursorModifier.SelectionAnchorPositionIndex)
                {
                    if (previousAnchorPositionIndex > previousEndingPositionIndex)
                    {
                        // Anchor went from being the upper bound to the lower bound.
                        primaryCursorModifier.SelectionAnchorPositionIndex -= 1;
                    }

                    primaryCursorModifier.SelectionEndingPositionIndex += 1;
                }
            };
        }

        public static TextEditorEdit VisualLineFactory(TextEditorCommandArgs commandArgs)
        {
            return async (ITextEditorEditContext editContext) =>
            {
                var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim keymapVim)
                    return;

                var previousAnchorPositionIndex = primaryCursorModifier.SelectionAnchorPositionIndex;
                var previousEndingPositionIndex = primaryCursorModifier.SelectionEndingPositionIndex;

                if (commandArgs.InnerCommand.TextEditorEditFactory is null)
                    return;

                var textEditorEdit = commandArgs.InnerCommand.TextEditorEditFactory.Invoke(commandArgs);
                await textEditorEdit.Invoke(editContext).ConfigureAwait(false);

                var nextEndingPositionIndex = primaryCursorModifier.SelectionEndingPositionIndex;

                if (nextEndingPositionIndex < primaryCursorModifier.SelectionAnchorPositionIndex)
                {
                    if (previousAnchorPositionIndex < previousEndingPositionIndex)
                    {
                        // Anchor went from being the lower bound to the upper bound.
                        var rowDataAnchorIsOn = modelModifier.GetRowInformationFromPositionIndex(previousAnchorPositionIndex.Value);

                        primaryCursorModifier.SelectionAnchorPositionIndex = modelModifier.RowEndingPositionsList[
                            rowDataAnchorIsOn.RowIndex].EndPositionIndexExclusive;
                    }

                    var startingPositionOfRow = modelModifier.GetRowEndingThatCreatedRow(primaryCursorModifier.RowIndex)
                        .EndPositionIndexExclusive;

                    primaryCursorModifier.SelectionEndingPositionIndex = startingPositionOfRow;
                }
                else if (nextEndingPositionIndex >= primaryCursorModifier.SelectionAnchorPositionIndex)
                {
                    if (previousAnchorPositionIndex > previousEndingPositionIndex)
                    {
                        // Anchor went from being the upper bound to the lower bound.
                        var rowDataAnchorIsOn = modelModifier.GetRowInformationFromPositionIndex(previousAnchorPositionIndex.Value);

                        primaryCursorModifier.SelectionAnchorPositionIndex = modelModifier.GetRowEndingThatCreatedRow(rowDataAnchorIsOn.RowIndex - 1)
                            .EndPositionIndexExclusive;
                    }

                    var endingPositionOfRow = modelModifier.RowEndingPositionsList[primaryCursorModifier.RowIndex]
                        .EndPositionIndexExclusive;

                    primaryCursorModifier.SelectionEndingPositionIndex = endingPositionOfRow;
                }
            };
        }
    }
}