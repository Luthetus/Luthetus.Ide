using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
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
                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim vimKeymap)
                    return;

                await TextEditorCommandDefaultFunctions
                    .CutFactory(commandArgs.ModelResourceUri, commandArgs.ViewModelKey, commandArgs)
                    .Invoke(editContext);

                vimKeymap.ActiveVimMode = VimMode.Insert;
            };
        }

        public static TextEditorEdit DeleteMotionFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                var cursorForMotion = new TextEditorCursor(
                    primaryCursorModifier.RowIndex,
                    primaryCursorModifier.ColumnIndex,
                    true);

                var textEditorCommandArgsForMotion = new TextEditorCommandArgs(
                    modelModifier.ResourceUri,
                    viewModelModifier.ViewModel.ViewModelKey,
                    commandArgs.HasTextSelection,
                    commandArgs.ClipboardService,
                    commandArgs.TextEditorService,
                    commandArgs.HandleMouseStoppedMovingEventAsyncFunc,
                    commandArgs.JsRuntime,
                    commandArgs.Dispatcher,
                    commandArgs.RegisterModelAction,
                    commandArgs.RegisterViewModelAction,
                    commandArgs.ShowViewModelAction);

                var motionResult = await VimMotionResult.GetResultAsync(
                    modelModifier,
                    viewModelModifier.ViewModel,
                    commandArgs,
                    cursorForMotion,
                    async () => await commandArgs.InnerCommand.CommandFunc
                        .Invoke(textEditorCommandArgsForMotion));

                var cursorForDeletion = new TextEditorCursor(
                    motionResult.LowerPositionIndexCursor.RowIndex,
                    motionResult.LowerPositionIndexCursor.ColumnIndex,
                    true);

                await editContext.TextEditorService.ModelApi.DeleteTextByRangeFactory(
                        modelModifier.ResourceUri,
                        viewModelModifier.ViewModel.ViewModelKey,
                        motionResult.PositionIndexDisplacement,
                        CancellationToken.None)
                    .Invoke(editContext);
            };
        }

        public static TextEditorEdit GetChangeMotionFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                var deleteMotion = DeleteMotionCommandConstructor(commandArgs.InnerCommand);

                await deleteMotion.CommandFunc.Invoke(commandArgs);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            };
        }

        public static TextEditorEdit ChangeSelectionFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                await TextEditorCommandDefaultFacts.Cut.CommandFunc.Invoke(commandArgs);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            };
        }

        public static TextEditorEdit YankFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                await TextEditorCommandDefaultFacts.Copy.CommandFunc.Invoke(commandArgs);
                await TextEditorCommandDefaultFacts.ClearTextSelection.CommandFunc.Invoke(commandArgs);
            };
        }

        public static TextEditorEdit NewLineBelowFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                await TextEditorCommandDefaultFacts.NewLineBelow.CommandFunc.Invoke(commandArgs);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            };
        }

        public static TextEditorEdit NewLineAboveFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                await TextEditorCommandDefaultFacts.NewLineAbove.CommandFunc.Invoke(commandArgs);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            };
        }
    }
}