using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static class Verbs
    {
        public static TextEditorCommand DeleteLineCommand = new(
            "Vim::Delete(Line)", "Vim::Delete(Line)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.Post(
                    nameof(DeleteLineCommand),
                    TextEditorCommandDefaultFunctions.CutFactory(
                        commandArgs.ModelResourceUri,
                        commandArgs.ViewModelKey,
                        commandArgs));

                return Task.CompletedTask;
            });

        public static readonly TextEditorCommand ChangeLineCommand = new(
            "Vim::Change(Line)", "Vim::Change(Line)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                commandArgs.TextEditorService.Post(nameof(ChangeLineCommand), ChangeLineFactory(commandArgs));
                return Task.CompletedTask;
            });

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

        public static TextEditorCommand DeleteMotionCommandFactory(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Delete({innerTextEditorCommand.DisplayName})", $"Vim::Delete({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                
                commandArgs.TextEditorService.Post(
                    nameof(DeleteMotionCommandFactory),
                    DeleteMotionFactory(commandArgs));

                return Task.CompletedTask;
            });

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

        public static TextEditorCommand ChangeMotionCommandFactory(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Change({innerTextEditorCommand.DisplayName})", $"Vim::Change({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.Post(
                    nameof(ChangeMotionCommandFactory),
                    GetChangeMotionFactory(commandArgs));

                return Task.CompletedTask;
            });

        public static TextEditorEdit GetChangeMotionFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                var deleteMotion = DeleteMotionCommandFactory(commandArgs.InnerCommand);

                await deleteMotion.CommandFunc.Invoke(commandArgs);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            };
        }

        public static readonly TextEditorCommand ChangeSelectionCommand = new(
            "Vim::Change(Selection)", "Vim::Change(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                commandArgs.TextEditorService.Post(nameof(ChangeSelectionCommand), ChangeSelectionFactory(commandArgs));
                return Task.CompletedTask;
            });

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

        public static readonly TextEditorCommand YankCommand = new(
            "Vim::Change(Selection)", "Vim::Change(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                commandArgs.TextEditorService.Post(nameof(YankCommand), YankFactory(commandArgs));
                return Task.CompletedTask;
            });

        public static TextEditorEdit YankFactory(TextEditorCommandArgs commandArgs)
        {
            return async editContext =>
            {
                await TextEditorCommandDefaultFacts.Copy.CommandFunc.Invoke(commandArgs);
                await TextEditorCommandDefaultFacts.ClearTextSelection.CommandFunc.Invoke(commandArgs);
            };
        }

        public static readonly TextEditorCommand NewLineBelowCommand = new(
            "Vim::NewLineBelow()", "Vim::NewLineBelow()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                commandArgs.TextEditorService.Post(nameof(NewLineBelowCommand), NewLineBelowFactory(commandArgs));
                return Task.CompletedTask;
            });

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

        public static readonly TextEditorCommand NewLineAboveCommand = new(
            "Vim::NewLineAbove()", "Vim::NewLineAbove()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                commandArgs.TextEditorService.Post(nameof(NewLineAboveCommand), NewLineAboveFactory(commandArgs));
                return Task.CompletedTask;
            });

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