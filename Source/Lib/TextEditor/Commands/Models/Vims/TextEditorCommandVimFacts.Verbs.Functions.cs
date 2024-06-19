using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static partial class Verbs
    {
        public static TextEditorEdit DeleteLineFactory(TextEditorCommandArgs commandArgs)
        {
            return TextEditorCommandDefaultFunctions.CutFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }

        public static TextEditorEdit ChangeLineFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var activeKeymap = commandArgs.ComponentData.Options.Keymap ?? TextEditorKeymapFacts.DefaultKeymap;
                if (activeKeymap is not TextEditorKeymapVim keymapVim)
                    return;

                await TextEditorCommandDefaultFunctions
                    .CutFactory(commandArgs.ModelResourceUri, commandArgs.ViewModelKey, commandArgs)
                    .Invoke(editContext)
					.ConfigureAwait(false);

                keymapVim.ActiveVimMode = VimMode.Insert;
            };
        }

        public static TextEditorEdit DeleteMotionFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                var textEditorCommandArgsForMotion = new TextEditorCommandArgs(
                    modelModifier.ResourceUri,
                    viewModelModifier.ViewModel.ViewModelKey,
					commandArgs.ComponentData,
                    commandArgs.TextEditorService,
                    commandArgs.ServiceProvider);

                var inCursor = primaryCursorModifier.ToCursor();

                var motionResult = await VimMotionResult.GetResultAsync(
                    modelModifier,
                    primaryCursorModifier,
                    async () => 
                    {
                        if (commandArgs.InnerCommand.TextEditorEditFactory is null)
                            return;

                        var textEditorEdit = commandArgs.InnerCommand.TextEditorEditFactory.Invoke(textEditorCommandArgsForMotion);
                        await textEditorEdit.Invoke(editContext).ConfigureAwait(false);
                    }).ConfigureAwait(false);

                primaryCursorModifier.LineIndex = inCursor.LineIndex;
                primaryCursorModifier.ColumnIndex = inCursor.ColumnIndex;
                primaryCursorModifier.PreferredColumnIndex = inCursor.ColumnIndex;

                var cursorForDeletion = new TextEditorCursor(
                    motionResult.LowerPositionIndexCursor.LineIndex,
                    motionResult.LowerPositionIndexCursor.ColumnIndex,
                    true);

                var cursorModifierBagForDeletion = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier> { new(cursorForDeletion) });

                await editContext.TextEditorService.ModelApi.DeleteTextByRangeUnsafeFactory(
                        modelModifier.ResourceUri,
                        cursorModifierBagForDeletion,
                        motionResult.PositionIndexDisplacement,
                        CancellationToken.None)
                    .Invoke(editContext)
					.ConfigureAwait(false);
            };
        }

        public static TextEditorEdit GetChangeMotionFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var activeKeymap = commandArgs.ComponentData.Options.Keymap ?? TextEditorKeymapFacts.DefaultKeymap;
                if (activeKeymap is not TextEditorKeymapVim keymapVim)
                    return;

                var deleteMotion = DeleteMotionCommandConstructor(commandArgs.InnerCommand);

                await deleteMotion.CommandFunc.Invoke(commandArgs).ConfigureAwait(false);
                keymapVim.ActiveVimMode = VimMode.Insert;
            };
        }

        public static TextEditorEdit ChangeSelectionFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var activeKeymap = commandArgs.ComponentData.Options.Keymap ?? TextEditorKeymapFacts.DefaultKeymap;
                if (activeKeymap is not TextEditorKeymapVim keymapVim)
                    return;

                await TextEditorCommandDefaultFacts.Cut.CommandFunc.Invoke(commandArgs).ConfigureAwait(false);
                keymapVim.ActiveVimMode = VimMode.Insert;
            };
        }

        public static TextEditorEdit YankFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                await TextEditorCommandDefaultFacts.Copy.CommandFunc.Invoke(commandArgs).ConfigureAwait(false);
                await TextEditorCommandDefaultFacts.ClearTextSelection.CommandFunc.Invoke(commandArgs).ConfigureAwait(false);
            };
        }

        public static TextEditorEdit NewLineBelowFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var activeKeymap = commandArgs.ComponentData.Options.Keymap ?? TextEditorKeymapFacts.DefaultKeymap;
                if (activeKeymap is not TextEditorKeymapVim keymapVim)
                    return;

                await TextEditorCommandDefaultFacts.NewLineBelow.CommandFunc.Invoke(commandArgs).ConfigureAwait(false);
                keymapVim.ActiveVimMode = VimMode.Insert;
            };
        }

        public static TextEditorEdit NewLineAboveFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var activeKeymap = commandArgs.ComponentData.Options.Keymap ?? TextEditorKeymapFacts.DefaultKeymap;
                if (activeKeymap is not TextEditorKeymapVim keymapVim)
                    return;

                await TextEditorCommandDefaultFacts.NewLineAbove.CommandFunc.Invoke(commandArgs).ConfigureAwait(false);
                keymapVim.ActiveVimMode = VimMode.Insert;
            };
        }
    }
}