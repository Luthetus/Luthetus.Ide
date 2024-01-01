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

                commandArgs.TextEditorService.Post(
                    nameof(Word),
                    async editContext =>
                    {
                        var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                        var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                        if (modelModifier is null || viewModelModifier is null)
                            return;

                        var cursorModifierBag = new TextEditorCursorModifierBag(
                            commandArgs.ViewModelKey,
                            viewModelModifier.ViewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                        await WordAsync(
                            commandArgs,
                            modelModifier,
                            viewModelModifier.ViewModel,
                            cursorModifierBag,
                            cursorModifierBag.CursorModifierBag.First(x => x.IsPrimaryCursor));
                    });
                return Task.CompletedTask;
            });

        public static Task WordAsync(
            TextEditorCommandArgs commandArgs,
            ITextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorCursorModifierBag refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor) 
        {
            void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
            {
                primaryCursor.ColumnIndex = columnIndex;
                primaryCursor.PreferredColumnIndex = columnIndex;
            }

            var lengthOfRow = model.GetLengthOfRow(primaryCursor.RowIndex);

            if (primaryCursor.ColumnIndex == lengthOfRow &&
                primaryCursor.RowIndex < model.RowCount - 1)
            {
                MutateIndexCoordinatesAndPreferredColumnIndex(0);
                primaryCursor.RowIndex++;
            }
            else if (primaryCursor.ColumnIndex != lengthOfRow)
            {
                var columnIndexOfCharacterWithDifferingKind = model.GetColumnIndexOfCharacterWithDifferingKind(
                    primaryCursor.RowIndex,
                    primaryCursor.ColumnIndex,
                    false);

                if (columnIndexOfCharacterWithDifferingKind == -1)
                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                else
                {
                    MutateIndexCoordinatesAndPreferredColumnIndex(
                        columnIndexOfCharacterWithDifferingKind);
                }
            }

            if (TextEditorSelectionHelper.HasSelectedText(primaryCursor))
            {
                primaryCursor.SelectionEndingPositionIndex = model.GetPositionIndex(
                    primaryCursor.RowIndex, primaryCursor.ColumnIndex);
            }

            return Task.CompletedTask;
        }

        public static readonly TextEditorCommand End = new(
            "Vim::End()", "Vim::End()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.Post(
                    nameof(End),
                    async editContext =>
                    {
                        var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                        var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                        if (modelModifier is null || viewModelModifier is null)
                            return;

                        var cursorModifierBag = new TextEditorCursorModifierBag(
                            commandArgs.ViewModelKey,
                            viewModelModifier.ViewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                        await EndAsync(
                            commandArgs,
                            modelModifier,
                            viewModelModifier.ViewModel,
                            cursorModifierBag,
                            cursorModifierBag.CursorModifierBag.First(x => x.IsPrimaryCursor));
                    });

                return Task.CompletedTask;
            });
        
        public static Task EndAsync(
            TextEditorCommandArgs commandArgs,
            ITextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorCursorModifierBag refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor)
        {
            return PerformEndAsync(
                commandArgs,
                model,
                viewModel,
                refreshCursorsRequest,
                primaryCursor);
        }

        private static async Task PerformEndAsync(
            TextEditorCommandArgs commandArgs,
            ITextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorCursorModifierBag refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor,
            bool isRecursiveCall = false)
        {
            void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
            {
                primaryCursor.ColumnIndex = columnIndex;
                primaryCursor.PreferredColumnIndex = columnIndex;
            }

            var lengthOfRow = model.GetLengthOfRow(primaryCursor.RowIndex);

            if (primaryCursor.ColumnIndex == lengthOfRow &&
                primaryCursor.RowIndex < model.RowCount - 1)
            {
                MutateIndexCoordinatesAndPreferredColumnIndex(0);
                primaryCursor.RowIndex++;
            }
            else if (primaryCursor.ColumnIndex != lengthOfRow)
            {
                var columnIndexOfCharacterWithDifferingKind = model.GetColumnIndexOfCharacterWithDifferingKind(
                    primaryCursor.RowIndex,
                    primaryCursor.ColumnIndex,
                    false);

                if (columnIndexOfCharacterWithDifferingKind == -1)
                {
                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                }
                else
                {
                    var columnsToMoveBy = columnIndexOfCharacterWithDifferingKind -
                        primaryCursor.ColumnIndex;

                    MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);

                    if (columnsToMoveBy > 1)
                    {
                        MutateIndexCoordinatesAndPreferredColumnIndex(primaryCursor.ColumnIndex - 1);
                    }
                    else if (columnsToMoveBy == 1 && !isRecursiveCall)
                    {
                        var positionIndex = model.GetCursorPositionIndex(primaryCursor);
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

                            await PerformEndAsync(
                                commandArgs,
                                model,
                                viewModel,
                                refreshCursorsRequest,
                                primaryCursor,
                                isRecursiveCall: true);

                            // Leave method early as all is finished.
                            return;
                        }
                    }
                }
            }

            if (TextEditorSelectionHelper.HasSelectedText(primaryCursor))
                primaryCursor.SelectionEndingPositionIndex = model.GetCursorPositionIndex(primaryCursor);
        }

        public static readonly TextEditorCommand Back = new(
            "Vim::Back()", "Vim::Back()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.Post(
                    nameof(Back),
                    async editContext =>
                    {
                        var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                        var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                        if (modelModifier is null || viewModelModifier is null)
                            return;

                        var cursorModifierBag = new TextEditorCursorModifierBag(
                            commandArgs.ViewModelKey,
                            viewModelModifier.ViewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                        await BackAsync(
                            commandArgs,
                            modelModifier,
                            viewModelModifier.ViewModel,
                            cursorModifierBag,
                            cursorModifierBag.CursorModifierBag.First(x => x.IsPrimaryCursor));
                    });
                return Task.CompletedTask;
            });
        
        public static Task BackAsync(
            TextEditorCommandArgs commandArgs,
            ITextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorCursorModifierBag refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor)
        {
            void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
            {
                primaryCursor.ColumnIndex = columnIndex;
                primaryCursor.PreferredColumnIndex = columnIndex;
            }

            if (primaryCursor.ColumnIndex == 0)
            {
                if (primaryCursor.RowIndex != 0)
                {
                    primaryCursor.RowIndex--;

                    var lengthOfRow = model.GetLengthOfRow(primaryCursor.RowIndex);

                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                }
            }
            else
            {
                var columnIndexOfCharacterWithDifferingKind = model.GetColumnIndexOfCharacterWithDifferingKind(
                    primaryCursor.RowIndex,
                    primaryCursor.ColumnIndex,
                    true);

                if (columnIndexOfCharacterWithDifferingKind == -1)
                    MutateIndexCoordinatesAndPreferredColumnIndex(0);
                else
                {
                    MutateIndexCoordinatesAndPreferredColumnIndex(
                        columnIndexOfCharacterWithDifferingKind);
                }
            }

            if (TextEditorSelectionHelper.HasSelectedText(primaryCursor))
            {
                primaryCursor.SelectionEndingPositionIndex =
                    model.GetCursorPositionIndex(primaryCursor);
            }

            return Task.CompletedTask;
        }

        public static TextEditorCommand GetVisualFactory(
            TextEditorCommand textEditorCommandMotion,
            string displayName)
        {
            return new TextEditorCommand(
                $"Vim::GetVisual({displayName})", $"Vim::GetVisual({displayName})", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(
                        nameof(GetVisualFactory),
                        async editContext =>
                        {
                            var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                            var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                            if (modelModifier is null || viewModelModifier is null)
                                return;

                            var cursorModifierBag = new TextEditorCursorModifierBag(
                                commandArgs.ViewModelKey,
                                viewModelModifier.ViewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                            await VisualAsync(
                                commandArgs,
                                modelModifier,
                                viewModelModifier.ViewModel,
                                cursorModifierBag,
                                cursorModifierBag.CursorModifierBag.First(x => x.IsPrimaryCursor));
                        });
                    return Task.CompletedTask;
                });
        }
        
        public static async Task VisualAsync(
            TextEditorCommandArgs commandArgs,
            ITextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorCursorModifierBag refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor)
        {
            var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim keymapVim)
                return;

            var previousAnchorPositionIndex = primaryCursor.SelectionAnchorPositionIndex;
            var previousEndingPositionIndex = primaryCursor.SelectionEndingPositionIndex;

            await commandArgs.InnerCommand.DoAsyncFunc.Invoke(commandArgs);

            var nextEndingPositionIndex = primaryCursor.SelectionEndingPositionIndex;

            if (nextEndingPositionIndex < primaryCursor.SelectionAnchorPositionIndex)
            {
                if (previousAnchorPositionIndex < previousEndingPositionIndex)
                {
                    // Anchor went from being the lower bound to the upper bound.
                    primaryCursor.SelectionAnchorPositionIndex += 1;
                }
            }
            else if (nextEndingPositionIndex >= primaryCursor.SelectionAnchorPositionIndex)
            {
                if (previousAnchorPositionIndex > previousEndingPositionIndex)
                {
                    // Anchor went from being the upper bound to the lower bound.
                    primaryCursor.SelectionAnchorPositionIndex -= 1;
                }

                primaryCursor.SelectionEndingPositionIndex += 1;
            }
        }

        public static TextEditorCommand GetVisualLineFactory(
            TextEditorCommand textEditorCommandMotion,
            string displayName)
        {
            return new TextEditorCommand(
                $"Vim::GetVisual({displayName})", $"Vim::GetVisual({displayName})", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(
                        nameof(GetVisualLineFactory),
                        async editContext =>
                        {
                            var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                            var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                            if (modelModifier is null || viewModelModifier is null)
                                return;

                            var cursorModifierBag = new TextEditorCursorModifierBag(
                                commandArgs.ViewModelKey,
                                viewModelModifier.ViewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                            await VisualLineAsync(
                                commandArgs,
                                modelModifier,
                                viewModelModifier.ViewModel,
                                cursorModifierBag,
                                cursorModifierBag.CursorModifierBag.First(x => x.IsPrimaryCursor));
                        });

                    return Task.CompletedTask;
                });
        }
        
        public static async Task VisualLineAsync(
            TextEditorCommandArgs commandArgs,
            ITextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorCursorModifierBag refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor)
        {
            var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim keymapVim)
                return;

            var previousAnchorPositionIndex = primaryCursor.SelectionAnchorPositionIndex;
            var previousEndingPositionIndex = primaryCursor.SelectionEndingPositionIndex;

            await commandArgs.InnerCommand.DoAsyncFunc.Invoke(commandArgs);

            var nextEndingPositionIndex = primaryCursor.SelectionEndingPositionIndex;

            if (nextEndingPositionIndex < primaryCursor.SelectionAnchorPositionIndex)
            {
                if (previousAnchorPositionIndex < previousEndingPositionIndex)
                {
                    // Anchor went from being the lower bound to the upper bound.
                    var rowDataAnchorIsOn = model.FindRowInformation(previousAnchorPositionIndex.Value);

                    primaryCursor.SelectionAnchorPositionIndex = model.RowEndingPositionsBag[
                        rowDataAnchorIsOn.rowIndex].EndPositionIndexExclusive;
                }

                var startingPositionOfRow = model.GetRowEndingThatCreatedRow(primaryCursor.RowIndex)
                    .EndPositionIndexExclusive;

                primaryCursor.SelectionEndingPositionIndex = startingPositionOfRow;
            }
            else if (nextEndingPositionIndex >= primaryCursor.SelectionAnchorPositionIndex)
            {
                if (previousAnchorPositionIndex > previousEndingPositionIndex)
                {
                    // Anchor went from being the upper bound to the lower bound.
                    var rowDataAnchorIsOn = model.FindRowInformation(previousAnchorPositionIndex.Value);

                    primaryCursor.SelectionAnchorPositionIndex = model.GetRowEndingThatCreatedRow(rowDataAnchorIsOn.rowIndex - 1)
                        .EndPositionIndexExclusive;
                }

                var endingPositionOfRow = model.RowEndingPositionsBag[primaryCursor.RowIndex]
                    .EndPositionIndexExclusive;

                primaryCursor.SelectionEndingPositionIndex = endingPositionOfRow;
            }
        }
    }
}