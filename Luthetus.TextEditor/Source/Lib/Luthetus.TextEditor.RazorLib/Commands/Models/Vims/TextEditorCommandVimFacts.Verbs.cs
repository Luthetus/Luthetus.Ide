using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
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

                commandArgs.TextEditorService.EnqueueEdit(TextEditorCommandDefaultFunctions.CutAsync);
                return Task.CompletedTask;
            });

        public static readonly TextEditorCommand ChangeLineCommand = new(
            "Vim::Change(Line)", "Vim::Change(Line)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueEdit(async editContext =>
                {
                    var model = editContext.CommandArgs.TextEditorService.ModelApi.GetOrDefault(editContext.CommandArgs.ModelResourceUri);
                    var viewModel = editContext.CommandArgs.TextEditorService.ViewModelApi.GetOrDefault(editContext.CommandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        editContext.CommandArgs.ViewModelKey,
                        editContext.ViewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await ChangeLineAsync(editContext);
                });
                return Task.CompletedTask;
            });

        public static async Task ChangeLineAsync(ITextEditorEditContext editContext)
        {
            var activeKeymap = editContext.CommandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim vimKeymap)
                return;

            await TextEditorCommandDefaultFunctions.CutAsync.Invoke(editContext);
            vimKeymap.ActiveVimMode = VimMode.Insert;
        }

        public static TextEditorCommand DeleteMotionCommandFactory(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Delete({innerTextEditorCommand.DisplayName})", $"Vim::Delete({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueEdit(DeleteMotionAsync);
                return Task.CompletedTask;
            });

        public static async Task DeleteMotionAsync(ITextEditorEditContext editContext)
        {
            var cursorForMotion = new TextEditorCursor(
                editContext.PrimaryCursor.RowIndex,
                editContext.PrimaryCursor.ColumnIndex,
                true);

            var textEditorCommandArgsForMotion = new TextEditorCommandArgs(
                editContext.Model.ResourceUri,
                editContext.ViewModel.ViewModelKey,
                editContext.CommandArgs.HasTextSelection,
                editContext.CommandArgs.ClipboardService,
                editContext.CommandArgs.TextEditorService,
                editContext.CommandArgs.HandleMouseStoppedMovingEventAsyncFunc,
                editContext.CommandArgs.JsRuntime,
                editContext.CommandArgs.Dispatcher,
                editContext.CommandArgs.RegisterModelAction,
                editContext.CommandArgs.RegisterViewModelAction,
                editContext.CommandArgs.ShowViewModelAction);

            var motionResult = await VimMotionResult.GetResultAsync(
                editContext.Model,
                editContext.ViewModel,
                editContext.CommandArgs,
                cursorForMotion,
                async () => await editContext.CommandArgs.InnerCommand.DoAsyncFunc
                    .Invoke(textEditorCommandArgsForMotion));

            var cursorForDeletion = new TextEditorCursor(
                motionResult.LowerPositionIndexCursor.RowIndex,
                motionResult.LowerPositionIndexCursor.ColumnIndex,
                true);

            var deleteTextByRangeAction = new TextEditorModelState.DeleteTextByRangeAction(
                editContext.Model.ResourceUri,
                editContext.ViewModel.ViewModelKey,
                new TextEditorCursorModifier[] { new TextEditorCursorModifier(cursorForDeletion) }.ToList(),
                motionResult.PositionIndexDisplacement,
                CancellationToken.None);

            await editContext.CommandArgs.TextEditorService.ModelApi
                .DeleteTextByRange(deleteTextByRangeAction, editContext.RefreshCursorsRequest)
                .Invoke(editContext);
        }

        public static TextEditorCommand ChangeMotionCommandFactory(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Change({innerTextEditorCommand.DisplayName})", $"Vim::Change({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueEdit(async editContext =>
                {
                    var model = editContext.CommandArgs.TextEditorService.ModelApi.GetOrDefault(editContext.CommandArgs.ModelResourceUri);
                    var viewModel = editContext.CommandArgs.TextEditorService.ViewModelApi.GetOrDefault(editContext.CommandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        editContext.CommandArgs.ViewModelKey,
                        editContext.ViewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await GetChangeMotionAsync(editContext);
                });
                return Task.CompletedTask;
            });

        public static async Task GetChangeMotionAsync(ITextEditorEditContext editContext)
        {
            var activeKeymap = editContext.CommandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                return;

            var deleteMotion = DeleteMotionCommandFactory(editContext.CommandArgs.InnerCommand);

            await deleteMotion.DoAsyncFunc.Invoke(editContext.CommandArgs);
            textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
        }

        public static readonly TextEditorCommand ChangeSelectionCommand = new(
            "Vim::Change(Selection)", "Vim::Change(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueEdit(async editContext =>
                {
                    var model = editContext.CommandArgs.TextEditorService.ModelApi.GetOrDefault(editContext.CommandArgs.ModelResourceUri);
                    var viewModel = editContext.CommandArgs.TextEditorService.ViewModelApi.GetOrDefault(editContext.CommandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        editContext.CommandArgs.ViewModelKey,
                        editContext.ViewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await ChangeSelectionAsync(editContext);
                });
                return Task.CompletedTask;
            });

        public static async Task ChangeSelectionAsync(ITextEditorEditContext editContext)
        {
            var activeKeymap = editContext.CommandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                return;

            await TextEditorCommandDefaultFacts.Cut.DoAsyncFunc.Invoke(editContext.CommandArgs);
            textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
        }

        public static readonly TextEditorCommand YankCommand = new(
            "Vim::Change(Selection)", "Vim::Change(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueEdit(async editContext =>
                {
                    var model = editContext.CommandArgs.TextEditorService.ModelApi.GetOrDefault(editContext.CommandArgs.ModelResourceUri);
                    var viewModel = editContext.CommandArgs.TextEditorService.ViewModelApi.GetOrDefault(editContext.CommandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        editContext.CommandArgs.ViewModelKey,
                        editContext.ViewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await YankAsync(editContext);
                });
                return Task.CompletedTask;
            });

        public static async Task YankAsync(ITextEditorEditContext editContext)
        {
            await TextEditorCommandDefaultFacts.Copy.DoAsyncFunc.Invoke(editContext.CommandArgs);
            await TextEditorCommandDefaultFacts.ClearTextSelection.DoAsyncFunc.Invoke(editContext.CommandArgs);
        }

        public static readonly TextEditorCommand NewLineBelowCommand = new(
            "Vim::NewLineBelow()", "Vim::NewLineBelow()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueEdit(async editContext =>
                {
                    var model = editContext.CommandArgs.TextEditorService.ModelApi.GetOrDefault(editContext.CommandArgs.ModelResourceUri);
                    var viewModel = editContext.CommandArgs.TextEditorService.ViewModelApi.GetOrDefault(editContext.CommandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        editContext.CommandArgs.ViewModelKey,
                        editContext.ViewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await NewLineBelowAsync(editContext);
                });
                return Task.CompletedTask;
            });

        public static async Task NewLineBelowAsync(ITextEditorEditContext editContext)
        {
            var activeKeymap = editContext.CommandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                return;

            await TextEditorCommandDefaultFacts.NewLineBelow.DoAsyncFunc.Invoke(editContext.CommandArgs);
            textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
        }

        public static readonly TextEditorCommand NewLineAboveCommand = new(
            "Vim::NewLineAbove()", "Vim::NewLineAbove()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.EnqueueEdit(async editContext =>
                {
                    var model = editContext.CommandArgs.TextEditorService.ModelApi.GetOrDefault(editContext.CommandArgs.ModelResourceUri);
                    var viewModel = editContext.CommandArgs.TextEditorService.ViewModelApi.GetOrDefault(editContext.CommandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        editContext.CommandArgs.ViewModelKey,
                        editContext.ViewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await NewLineAboveAsync(editContext);
                });
                return Task.CompletedTask;
            });

        public static async Task NewLineAboveAsync(ITextEditorEditContext editContext)
        {
            var activeKeymap = editContext.CommandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

            if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                return;

            await TextEditorCommandDefaultFacts.NewLineAbove.DoAsyncFunc.Invoke(editContext.CommandArgs);
            textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
        }
    }
}