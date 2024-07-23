using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static partial class Motions
    {
        public static void Word(
        	IEditContext editContext,
	        TextEditorModelModifier modelModifier,
	        TextEditorViewModelModifier viewModelModifier,
	        CursorModifierBagTextEditor cursorModifierBag,
        	TextEditorCommandArgs commandArgs)
        {
        	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
        
            void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
            {
                primaryCursorModifier.ColumnIndex = columnIndex;
                primaryCursorModifier.PreferredColumnIndex = columnIndex;
            }

            var lengthOfRow = modelModifier.GetLineLength(primaryCursorModifier.LineIndex);

            if (primaryCursorModifier.ColumnIndex == lengthOfRow &&
                primaryCursorModifier.LineIndex < modelModifier.LineCount - 1)
            {
                MutateIndexCoordinatesAndPreferredColumnIndex(0);
                primaryCursorModifier.LineIndex++;
            }
            else if (primaryCursorModifier.ColumnIndex != lengthOfRow)
            {
                var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                    primaryCursorModifier.LineIndex,
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
                    primaryCursorModifier.LineIndex,
                    primaryCursorModifier.ColumnIndex);
            }
        }

        public static void End(
        	IEditContext editContext,
	        TextEditorModelModifier modelModifier,
	        TextEditorViewModelModifier viewModelModifier,
	        CursorModifierBagTextEditor cursorModifierBag,
        	TextEditorCommandArgs commandArgs)
        {
            PerformEnd(
            	editContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
	        	commandArgs);
        }

        private static void PerformEnd(
            IEditContext editContext,
	        TextEditorModelModifier modelModifier,
	        TextEditorViewModelModifier viewModelModifier,
	        CursorModifierBagTextEditor cursorModifierBag,
        	TextEditorCommandArgs commandArgs,
        	bool isRecursiveCall = false)
        {
        	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
        
            void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
            {
                primaryCursorModifier.ColumnIndex = columnIndex;
                primaryCursorModifier.PreferredColumnIndex = columnIndex;
            }

            var lengthOfRow = modelModifier.GetLineLength(primaryCursorModifier.LineIndex);

            if (primaryCursorModifier.ColumnIndex == lengthOfRow &&
                primaryCursorModifier.LineIndex < modelModifier.LineCount - 1)
            {
                MutateIndexCoordinatesAndPreferredColumnIndex(0);
                primaryCursorModifier.LineIndex++;
            }
            else if (primaryCursorModifier.ColumnIndex != lengthOfRow)
            {
                var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                    primaryCursorModifier.LineIndex,
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

                            PerformEnd(
                        		editContext,
						        modelModifier,
						        viewModelModifier,
						        cursorModifierBag,
					        	commandArgs,
                                isRecursiveCall: true);

                            // Leave method early as all is finished.
                            return;
                        }
                    }
                }
            }

            if (TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier))
                primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
        }

        public static void Back(
        	IEditContext editContext,
	        TextEditorModelModifier modelModifier,
	        TextEditorViewModelModifier viewModelModifier,
	        CursorModifierBagTextEditor cursorModifierBag,
        	TextEditorCommandArgs commandArgs)
        {
        	var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
        
            void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
            {
                primaryCursorModifier.ColumnIndex = columnIndex;
                primaryCursorModifier.PreferredColumnIndex = columnIndex;
            }

            if (primaryCursorModifier.ColumnIndex == 0)
            {
                if (primaryCursorModifier.LineIndex != 0)
                {
                    primaryCursorModifier.LineIndex--;

                    var lengthOfRow = modelModifier.GetLineLength(primaryCursorModifier.LineIndex);

                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                }
            }
            else
            {
                var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                    primaryCursorModifier.LineIndex,
                    primaryCursorModifier.ColumnIndex,
                    true);

                if (columnIndexOfCharacterWithDifferingKind == -1)
                    MutateIndexCoordinatesAndPreferredColumnIndex(0);
                else
                    MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);
            }

            if (TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier))
                primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
        }

        public static async Task Visual(
        	IEditContext editContext,
	        TextEditorModelModifier modelModifier,
	        TextEditorViewModelModifier viewModelModifier,
	        CursorModifierBagTextEditor cursorModifierBag,
        	TextEditorCommandArgs commandArgs)
        {
            var activeKeymap = commandArgs.ComponentData.Options.Keymap ?? TextEditorKeymapFacts.DefaultKeymap;
            if (activeKeymap is not TextEditorKeymapVim keymapVim)
                return;
                
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            var previousAnchorPositionIndex = primaryCursorModifier.SelectionAnchorPositionIndex;
            var previousEndingPositionIndex = primaryCursorModifier.SelectionEndingPositionIndex;

            await commandArgs.InnerCommand.CommandFunc.Invoke(commandArgs).ConfigureAwait(false);

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
        }

        public static async Task VisualLine(
        	IEditContext editContext,
	        TextEditorModelModifier modelModifier,
	        TextEditorViewModelModifier viewModelModifier,
	        CursorModifierBagTextEditor cursorModifierBag,
        	TextEditorCommandArgs commandArgs)
        {
            var activeKeymap = commandArgs.ComponentData.Options.Keymap ?? TextEditorKeymapFacts.DefaultKeymap;
            if (activeKeymap is not TextEditorKeymapVim keymapVim)
                return;
                
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            var previousAnchorPositionIndex = primaryCursorModifier.SelectionAnchorPositionIndex;
            var previousEndingPositionIndex = primaryCursorModifier.SelectionEndingPositionIndex;

            await commandArgs.InnerCommand.CommandFunc.Invoke(commandArgs).ConfigureAwait(false);

            var nextEndingPositionIndex = primaryCursorModifier.SelectionEndingPositionIndex;

            if (nextEndingPositionIndex < primaryCursorModifier.SelectionAnchorPositionIndex)
            {
                if (previousAnchorPositionIndex < previousEndingPositionIndex)
                {
                    // Anchor went from being the lower bound to the upper bound.
                    var rowDataAnchorIsOn = modelModifier.GetLineInformationFromPositionIndex(
                        previousAnchorPositionIndex.Value);

                    primaryCursorModifier.SelectionAnchorPositionIndex = modelModifier.LineEndList[
                        rowDataAnchorIsOn.Index]
                        .EndPositionIndexExclusive;
                }

                var rowStartPositionInclusive = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex)
                    .StartPositionIndexInclusive;

                primaryCursorModifier.SelectionEndingPositionIndex = rowStartPositionInclusive;
            }
            else if (nextEndingPositionIndex >= primaryCursorModifier.SelectionAnchorPositionIndex)
            {
                if (previousAnchorPositionIndex > previousEndingPositionIndex)
                {
                    // Anchor went from being the upper bound to the lower bound.
                    var rowDataAnchorIsOn = modelModifier.GetLineInformationFromPositionIndex(
                        previousAnchorPositionIndex.Value);

                    primaryCursorModifier.SelectionAnchorPositionIndex = modelModifier
                        .GetLineInformation(rowDataAnchorIsOn.Index - 1)
                        .StartPositionIndexInclusive;
                }

                var endingPositionOfRow = modelModifier.LineEndList[primaryCursorModifier.LineIndex]
                    .EndPositionIndexExclusive;

                primaryCursorModifier.SelectionEndingPositionIndex = endingPositionOfRow;
            }
        }
    }
}