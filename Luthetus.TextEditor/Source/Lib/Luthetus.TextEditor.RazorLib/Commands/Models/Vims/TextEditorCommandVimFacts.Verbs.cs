using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static class Verbs
    {
        public static readonly TextEditorCommand DeleteLine = new(
            async commandParameter =>
            {
                await TextEditorCommandDefaultFacts.Cut.DoAsyncFunc.Invoke(commandParameter);
            },
            true,
            "Vim::Delete(Line)",
            "Vim::Delete(Line)");

        public static readonly TextEditorCommand ChangeLine = new(
            async interfaceCommandParameter =>
            {
                var commandParameter = (TextEditorCommandArgs)interfaceCommandParameter;

                var activeKeymap = commandParameter.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim vimKeymap)
                    return;

                await DeleteLine.DoAsyncFunc.Invoke(commandParameter);
                vimKeymap.ActiveVimMode = VimMode.Insert;
            },
            true,
            "Vim::Change(Line)",
            "Vim::Change(Line)");

        public static TextEditorCommand GetDeleteMotion(TextEditorCommand innerTextEditorCommand) => new(
            async interfaceCommandParameter =>
            {
                var commandParameter = (TextEditorCommandArgs)interfaceCommandParameter;

                var textEditorCursorForMotion = new TextEditorCursor(
                    commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates,
                    true);

                var textEditorCommandParameterForMotion = new TextEditorCommandArgs(
                    commandParameter.Model,
                    TextEditorCursorSnapshot.TakeSnapshots(textEditorCursorForMotion),
                    commandParameter.HasTextSelection,
                    commandParameter.ClipboardService,
                    commandParameter.TextEditorService,
                    commandParameter.ViewModel,
                    commandParameter.HandleMouseStoppedMovingEventAsyncFunc,
                    commandParameter.JsRuntime,
                    commandParameter.RegisterModelAction,
                    commandParameter.RegisterViewModelAction,
                    commandParameter.ShowViewModelAction);

                var motionResult = await VimMotionResult.GetResultAsync(
                    commandParameter,
                    textEditorCursorForMotion,
                    async () => await innerTextEditorCommand.DoAsyncFunc
                        .Invoke(textEditorCommandParameterForMotion));

                var cursorForDeletion = new TextEditorCursor(
                    motionResult.LowerPositionIndexImmutableCursor,
                    true);

                var deleteTextTextEditorModelAction = new TextEditorModelState.DeleteTextByRangeAction(
                    commandParameter.Model.ResourceUri,
                    TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                    motionResult.PositionIndexDisplacement,
                    CancellationToken.None);

                commandParameter.TextEditorService.Model.DeleteTextByRange(deleteTextTextEditorModelAction);
            },
            true,
            $"Vim::Delete({innerTextEditorCommand.DisplayName})",
            $"Vim::Delete({innerTextEditorCommand.DisplayName})");

        public static TextEditorCommand GetChangeMotion(TextEditorCommand innerTextEditorCommand) => new(
            async interfaceCommandParameter =>
            {
                var commandParameter = (TextEditorCommandArgs)interfaceCommandParameter;

                var activeKeymap = commandParameter.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                var deleteMotion = GetDeleteMotion(innerTextEditorCommand);

                await deleteMotion.DoAsyncFunc.Invoke(commandParameter);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            },
            true,
            $"Vim::Change({innerTextEditorCommand.DisplayName})",
            $"Vim::Change({innerTextEditorCommand.DisplayName})");

        public static readonly TextEditorCommand ChangeSelection = new(
            async interfaceCommandParameter =>
            {
                var commandParameter = (TextEditorCommandArgs)interfaceCommandParameter;

                var activeKeymap = commandParameter.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                await TextEditorCommandDefaultFacts.Cut.DoAsyncFunc.Invoke(commandParameter);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            },
            true,
            "Vim::Change(Selection)",
            "Vim::Change(Selection)");

        public static readonly TextEditorCommand Yank = new(
            async commandParameter =>
            {
                await TextEditorCommandDefaultFacts.Copy.DoAsyncFunc.Invoke(commandParameter);
                await TextEditorCommandDefaultFacts.ClearTextSelection.DoAsyncFunc.Invoke(commandParameter);
            },
            true,
            "Vim::Change(Selection)",
            "Vim::Change(Selection)");

        public static readonly TextEditorCommand NewLineBelow = new(
            async interfaceCommandParameter =>
            {
                var commandParameter = (TextEditorCommandArgs)interfaceCommandParameter;

                var activeKeymap = commandParameter.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                await TextEditorCommandDefaultFacts.NewLineBelow.DoAsyncFunc.Invoke(commandParameter);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            },
            true,
            "Vim::NewLineBelow()",
            "Vim::NewLineBelow()");

        public static readonly TextEditorCommand NewLineAbove = new(
            async interfaceCommandParameter =>
            {
                var commandParameter = (TextEditorCommandArgs)interfaceCommandParameter;

                var activeKeymap = commandParameter.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                await TextEditorCommandDefaultFacts.NewLineAbove.DoAsyncFunc.Invoke(commandParameter);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            },
            true,
            "Vim::NewLineAbove()",
            "Vim::NewLineAbove()");
    }
}