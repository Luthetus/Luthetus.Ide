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

                var edit = commandArgs.TextEditorService.CreateEdit(async context =>
                {
                    var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                    var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        commandArgs.ViewModelKey,
                        viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await TextEditorCommandDefaultFunctions.CutAsync(
                        commandArgs,
                        model,
                        viewModel,
                        refreshCursorsRequest,
                        refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
                });

                commandArgs.TextEditorService.EnqueueEdit(edit);
                return Task.CompletedTask;
            });

        public static readonly TextEditorCommand ChangeLineCommand = new(
            "Vim::Change(Line)", "Vim::Change(Line)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var edit = commandArgs.TextEditorService.CreateEdit(async context =>
                {
                    var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                    var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        commandArgs.ViewModelKey,
                        viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await ChangeLineAsync(
                        commandArgs,
                        model,
                        viewModel,
                        refreshCursorsRequest,
                        refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
                });

                commandArgs.TextEditorService.EnqueueEdit(edit);
                return Task.CompletedTask;
            });

        public static async Task ChangeLineAsync(ITextEditorEditContext editContext)
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

                var edit = commandArgs.TextEditorService.CreateEdit(async context =>
                {
                    var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                    var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        commandArgs.ViewModelKey,
                        viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await DeleteMotionAsync(
                        commandArgs,
                        model,
                        viewModel,
                        refreshCursorsRequest,
                        refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
                });

                commandArgs.TextEditorService.EnqueueEdit(edit);
                return Task.CompletedTask;
            });

        public static async Task DeleteMotionAsync(ITextEditorEditContext editContext)
        {
            var cursorForMotion = new TextEditorCursor(
                primaryCursor.RowIndex,
                primaryCursor.ColumnIndex,
                true);

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
                model,
                viewModel,
                commandArgs,
                cursorForMotion,
                async () => await commandArgs.InnerCommand.DoAsyncFunc
                    .Invoke(textEditorCommandArgsForMotion));

            var cursorForDeletion = new TextEditorCursor(
                motionResult.LowerPositionIndexCursor.RowIndex,
                motionResult.LowerPositionIndexCursor.ColumnIndex,
                true);

            var deleteTextTextEditorModelAction = new TextEditorModelState.DeleteTextByRangeAction(
                model.ResourceUri,
                viewModel.ViewModelKey,
                new TextEditorCursorModifier[] { new TextEditorCursorModifier(cursorForDeletion) }.ToList(),
                motionResult.PositionIndexDisplacement,
                CancellationToken.None);

            await commandArgs.TextEditorService.ModelApi
                .GetDeleteTextByRangeTask(deleteTextTextEditorModelAction)
                .Invoke(commandArgs, model, viewModel, refreshCursorsRequest, primaryCursor);
        }

        public static TextEditorCommand ChangeMotionCommandFactory(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Change({innerTextEditorCommand.DisplayName})", $"Vim::Change({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var edit = commandArgs.TextEditorService.CreateEdit(async context =>
                {
                    var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                    var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        commandArgs.ViewModelKey,
                        viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await GetChangeMotionAsync(
                        commandArgs,
                        model,
                        viewModel,
                        refreshCursorsRequest,
                        refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
                });

                commandArgs.TextEditorService.EnqueueEdit(edit);
                return Task.CompletedTask;
            });

        public static async Task GetChangeMotionAsync(ITextEditorEditContext editContext)
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

                var edit = commandArgs.TextEditorService.CreateEdit(async context =>
                {
                    var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                    var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        commandArgs.ViewModelKey,
                        viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await ChangeSelectionAsync(
                        commandArgs,
                        model,
                        viewModel,
                        refreshCursorsRequest,
                        refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
                });

                commandArgs.TextEditorService.EnqueueEdit(edit);
                return Task.CompletedTask;
            });

        public static async Task ChangeSelectionAsync(ITextEditorEditContext editContext)
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

                var edit = commandArgs.TextEditorService.CreateEdit(async context =>
                {
                    var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                    var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        commandArgs.ViewModelKey,
                        viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await YankAsync(
                        commandArgs,
                        model,
                        viewModel,
                        refreshCursorsRequest,
                        refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
                });

                commandArgs.TextEditorService.EnqueueEdit(edit);
                return Task.CompletedTask;
            });

        public static async Task YankAsync(ITextEditorEditContext editContext)
        {
            await TextEditorCommandDefaultFacts.Copy.DoAsyncFunc.Invoke(commandArgs);
            await TextEditorCommandDefaultFacts.ClearTextSelection.DoAsyncFunc.Invoke(commandArgs);
        }

        public static readonly TextEditorCommand NewLineBelowCommand = new(
            "Vim::NewLineBelow()", "Vim::NewLineBelow()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var edit = commandArgs.TextEditorService.CreateEdit(async context =>
                {
                    var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                    var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        commandArgs.ViewModelKey,
                        viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await NewLineBelowAsync(
                        commandArgs,
                        model,
                        viewModel,
                        refreshCursorsRequest,
                        refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
                });

                commandArgs.TextEditorService.EnqueueEdit(edit);
                return Task.CompletedTask;
            });

        public static async Task NewLineBelowAsync(ITextEditorEditContext editContext)
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

                var edit = commandArgs.TextEditorService.CreateEdit(async context =>
                {
                    var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                    var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                    var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                        commandArgs.ViewModelKey,
                        viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                    await NewLineAboveAsync(
                        commandArgs,
                        model,
                        viewModel,
                        refreshCursorsRequest,
                        refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
                });

                commandArgs.TextEditorService.EnqueueEdit(edit);
                return Task.CompletedTask;
            });

        public static async Task NewLineAboveAsync(ITextEditorEditContext editContext)
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