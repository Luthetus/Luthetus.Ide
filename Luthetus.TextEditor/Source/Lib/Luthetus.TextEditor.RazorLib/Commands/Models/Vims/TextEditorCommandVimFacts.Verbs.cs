using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static class Verbs
    {
        public static readonly TextEditorCommand DeleteLineCommand = new(
            "Vim::Delete(Line)", "Vim::Delete(Line)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(ChangeLineCommand),
                    commandArgs,
                    TextEditorCommandDefaultFunctions.CutAsync);

                return Task.CompletedTask;
            });

        public static readonly TextEditorCommand ChangeLineCommand = new(
            "Vim::Change(Line)", "Vim::Change(Line)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(ChangeLineCommand),
                    commandArgs,
                    ChangeLineAsync);

                return Task.CompletedTask;
            });

        public static async Task ChangeLineAsync(
            TextEditorCommandArgs commandArgs,
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorService.RefreshCursorsRequest refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor)
        {
            var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim vimKeymap)
                return;

            await TextEditorCommandDefaultFunctions.CutAsync(
                commandArgs, model, viewModel, refreshCursorsRequest, primaryCursor);

            vimKeymap.ActiveVimMode = VimMode.Insert;
        }

        public static TextEditorCommand DeleteMotionCommandFactory(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Delete({innerTextEditorCommand.DisplayName})", $"Vim::Delete({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.InnerCommand = innerTextEditorCommand;

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(ChangeLineCommand),
                    commandArgs,
                    DeleteMotionAsync);

                return Task.CompletedTask;
            });

        public static async Task DeleteMotionAsync(
            TextEditorCommandArgs commandArgs,
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorService.RefreshCursorsRequest refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor)
        {
            var cursorForMotion = TextEditorCursor.Empty with
            {
                RowIndex = primaryCursor.RowIndex,
                ColumnIndex = primaryCursor.ColumnIndex,
                IsPrimaryCursor = true,
            };

            var textEditorCommandArgsForMotion = new TextEditorCommandArgs(
                model.ResourceUri,
                viewModel.ViewModelKey,
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
                commandArgs,
                cursorForMotion,
                async () => await commandArgs.InnerCommand.DoAsyncFunc
                    .Invoke(textEditorCommandArgsForMotion));

            var cursorForDeletion = TextEditorCursor.Empty with
            {
                RowIndex = motionResult.LowerPositionIndexCursor.RowIndex,
                ColumnIndex = motionResult.LowerPositionIndexCursor.ColumnIndex,
                IsPrimaryCursor = true,
            };

            var deleteTextTextEditorModelAction = new TextEditorModelState.DeleteTextByRangeAction(
                model.ResourceUri,
                viewModel.ViewModelKey,
                new TextEditorCursorModifier[] { new TextEditorCursorModifier(cursorForDeletion) }.ToList(),
                motionResult.PositionIndexDisplacement,
                CancellationToken.None);

            commandArgs.TextEditorService.ModelApi.DeleteTextByRange(deleteTextTextEditorModelAction);
        }

        public static TextEditorCommand ChangeMotionCommandFactory(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Change({innerTextEditorCommand.DisplayName})", $"Vim::Change({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.InnerCommand = innerTextEditorCommand;

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(ChangeMotionCommandFactory),
                    commandArgs,
                    GetChangeMotionAsync);

                return Task.CompletedTask;
            });

        public static async Task GetChangeMotionAsync(
            TextEditorCommandArgs commandArgs,
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorService.RefreshCursorsRequest refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor)
        {
            var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                return;

            var deleteMotion = DeleteMotionCommandFactory(commandArgs.InnerCommand);

            await deleteMotion.DoAsyncFunc.Invoke(commandArgs);
            textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
        }

        public static readonly TextEditorCommand ChangeSelectionCommand = new(
            "Vim::Change(Selection)", "Vim::Change(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(ChangeSelectionCommand),
                    commandArgs,
                    ChangeSelectionAsync);

                return Task.CompletedTask;
            });

        public static async Task ChangeSelectionAsync(
            TextEditorCommandArgs commandArgs,
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorService.RefreshCursorsRequest refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor)
        {
            var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                return;

            await TextEditorCommandDefaultFacts.Cut.DoAsyncFunc.Invoke(commandArgs);
            textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
        }

        public static readonly TextEditorCommand YankCommand = new(
            "Vim::Change(Selection)", "Vim::Change(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(YankCommand),
                    commandArgs,
                    YankAsync);

                return Task.CompletedTask;
            });

        public static async Task YankAsync(
            TextEditorCommandArgs commandArgs,
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorService.RefreshCursorsRequest refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor)
        {
            await TextEditorCommandDefaultFacts.Copy.DoAsyncFunc.Invoke(commandArgs);
            await TextEditorCommandDefaultFacts.ClearTextSelection.DoAsyncFunc.Invoke(commandArgs);
        }

        public static readonly TextEditorCommand NewLineBelowCommand = new(
            "Vim::NewLineBelow()", "Vim::NewLineBelow()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(NewLineBelowCommand),
                    commandArgs,
                    NewLineBelowAsync);

                return Task.CompletedTask;
            });

        public static async Task NewLineBelowAsync(
            TextEditorCommandArgs commandArgs,
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorService.RefreshCursorsRequest refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor)
        {
            var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                return;

            await TextEditorCommandDefaultFacts.NewLineBelow.DoAsyncFunc.Invoke(commandArgs);
            textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
        }

        public static readonly TextEditorCommand NewLineAboveCommand = new(
            "Vim::NewLineAbove()", "Vim::NewLineAbove()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(NewLineAboveCommand),
                    commandArgs,
                    NewLineAboveAsync);

                return Task.CompletedTask;
            });

        public static async Task NewLineAboveAsync(
            TextEditorCommandArgs commandArgs,
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorService.RefreshCursorsRequest refreshCursorsRequest,
            TextEditorCursorModifier primaryCursor)
        {
            var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                return;

            await TextEditorCommandDefaultFacts.NewLineAbove.DoAsyncFunc.Invoke(commandArgs);
            textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
        }
    }
}