using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static class Verbs
    {
        public static readonly TextEditorCommand DeleteLine = new(
            "Vim::Delete(Line)", "Vim::Delete(Line)", false, true, TextEditKind.None, null,
            async commandArgs =>
            {
                await TextEditorCommandDefaultFacts.Cut.DoAsyncFunc.Invoke(commandArgs);
            });

        public static readonly TextEditorCommand ChangeLine = new(
            "Vim::Change(Line)", "Vim::Change(Line)", false, true, TextEditKind.None, null,
            async interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim vimKeymap)
                    return;

                await DeleteLine.DoAsyncFunc.Invoke(commandArgs);
                vimKeymap.ActiveVimMode = VimMode.Insert;
            });

        public static TextEditorCommand GetDeleteMotion(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Delete({innerTextEditorCommand.DisplayName})", $"Vim::Delete({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            async interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var textEditorCursorForMotion = new TextEditorCursor(
                    commandArgs.PrimaryCursorSnapshot.UserCursor.IndexCoordinates,
                    true);

                var textEditorCommandArgsForMotion = new TextEditorCommandArgs(
                    commandArgs.Model,
                    TextEditorCursorSnapshot.TakeSnapshots(textEditorCursorForMotion),
                    commandArgs.HasTextSelection,
                    commandArgs.ClipboardService,
                    commandArgs.TextEditorService,
                    commandArgs.ViewModel,
                    commandArgs.HandleMouseStoppedMovingEventAsyncFunc,
                    commandArgs.JsRuntime,
                    commandArgs.RegisterModelAction,
                    commandArgs.RegisterViewModelAction,
                    commandArgs.ShowViewModelAction);

                var motionResult = await VimMotionResult.GetResultAsync(
                    commandArgs,
                    textEditorCursorForMotion,
                    async () => await innerTextEditorCommand.DoAsyncFunc
                        .Invoke(textEditorCommandArgsForMotion));

                var cursorForDeletion = new TextEditorCursor(
                    motionResult.LowerPositionIndexImmutableCursor,
                    true);

                var deleteTextTextEditorModelAction = new TextEditorModelState.DeleteTextByRangeAction(
                    commandArgs.Model.ResourceUri,
                    TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                    motionResult.PositionIndexDisplacement,
                    CancellationToken.None);

                commandArgs.TextEditorService.ModelApi.DeleteTextByRange(deleteTextTextEditorModelAction);
            });

        public static TextEditorCommand GetChangeMotion(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Change({innerTextEditorCommand.DisplayName})", $"Vim::Change({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            async interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                var deleteMotion = GetDeleteMotion(innerTextEditorCommand);

                await deleteMotion.DoAsyncFunc.Invoke(commandArgs);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            });

        public static readonly TextEditorCommand ChangeSelection = new(
            "Vim::Change(Selection)", "Vim::Change(Selection)", false, true, TextEditKind.None, null,
            async interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                await TextEditorCommandDefaultFacts.Cut.DoAsyncFunc.Invoke(commandArgs);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            });

        public static readonly TextEditorCommand Yank = new(
            "Vim::Change(Selection)",
            "Vim::Change(Selection)",
            false,
            true,
            TextEditKind.None,
            null,
            async commandArgs =>
            {
                await TextEditorCommandDefaultFacts.Copy.DoAsyncFunc.Invoke(commandArgs);
                await TextEditorCommandDefaultFacts.ClearTextSelection.DoAsyncFunc.Invoke(commandArgs);
            });

        public static readonly TextEditorCommand NewLineBelow = new(
            "Vim::NewLineBelow()", "Vim::NewLineBelow()", false, true, TextEditKind.None, null,
            async interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                await TextEditorCommandDefaultFacts.NewLineBelow.DoAsyncFunc.Invoke(commandArgs);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            });

        public static readonly TextEditorCommand NewLineAbove = new(
            "Vim::NewLineAbove()", "Vim::NewLineAbove()", false, true, TextEditKind.None, null,
            async interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                await TextEditorCommandDefaultFacts.NewLineAbove.DoAsyncFunc.Invoke(commandArgs);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            });
    }
}