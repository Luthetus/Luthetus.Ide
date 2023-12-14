using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static class Verbs
    {
        public static readonly TextEditorCommand DeleteLine = new(
            "Vim::Delete(Line)", "Vim::Delete(Line)", false, true, TextEditKind.None, null,
            commandArgs => TextEditorCommandDefaultFacts.Cut.DoAsyncFunc.Invoke(commandArgs));

        public static readonly TextEditorCommand ChangeLine = new(
            "Vim::Change(Line)", "Vim::Change(Line)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    new List<TextEditorCursorModifier>());

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(ChangeLine),
                    refreshCursorsRequest,
                    async () =>
                    {
                        var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                        var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                        var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                        if (viewModel is null || model is null || cursor is null)
                            return;

                        var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                            ?? TextEditorKeymapFacts.DefaultKeymap;

                        if (activeKeymap is not TextEditorKeymapVim vimKeymap)
                            return;

                        await DeleteLine.DoAsyncFunc.Invoke(commandArgs);
                        vimKeymap.ActiveVimMode = VimMode.Insert;
                    });

                return Task.CompletedTask;
            });

        public static TextEditorCommand GetDeleteMotion(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Delete({innerTextEditorCommand.DisplayName})", $"Vim::Delete({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            async interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    new List<TextEditorCursorModifier>());

                commandArgs.TextEditorService.EnqueueModification(
                    nameof(GetDeleteMotion),
                    refreshCursorsRequest,
                    () =>
                    {
                        var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                        var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                        var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                        if (viewModel is null || model is null || cursor is null)
                            return Task.CompletedTask;

                        var cursorForMotion = TextEditorCursor.Empty with
                        {
                            RowIndex = cursor.RowIndex,
                            ColumnIndex = cursor.ColumnIndex,
                            IsPrimaryCursor = true,
                        };

                        var textEditorCommandArgsForMotion = new TextEditorCommandArgs(
                            commandArgs.ModelResourceUri,
                            new TextEditorCursorModifier[] { new (cursorForMotion) }.ToList(),
                            commandArgs.HasTextSelection,
                            commandArgs.ClipboardService,
                            commandArgs.TextEditorService,
                            commandArgs.ViewModelKey,
                            commandArgs.HandleMouseStoppedMovingEventAsyncFunc,
                            commandArgs.JsRuntime,
                            commandArgs.RegisterModelAction,
                            commandArgs.RegisterViewModelAction,
                            commandArgs.ShowViewModelAction);

                        var motionResult = await VimMotionResult.GetResultAsync(
                            commandArgs,
                            cursorForMotion,
                            async () => await innerTextEditorCommand.DoAsyncFunc
                                .Invoke(textEditorCommandArgsForMotion));

                        var cursorForDeletion = TextEditorCursor.Empty with
                        {
                            RowIndex = motionResult.LowerPositionIndexCursor.RowIndex,
                            ColumnIndex = motionResult.LowerPositionIndexCursor.ColumnIndex,
                            IsPrimaryCursor = true,
                        };

                        var deleteTextTextEditorModelAction = new TextEditorModelState.DeleteTextByRangeAction(
                            commandArgs.ModelResourceUri.ResourceUri,
                            commandArgs.ViewModelKey.ViewModelKey,
                            new TextEditorCursorModifier[] { new TextEditorCursorModifier(cursorForDeletion) }.ToList(),
                            motionResult.PositionIndexDisplacement,
                            CancellationToken.None);

                        commandArgs.TextEditorService.ModelApi.DeleteTextByRange(deleteTextTextEditorModelAction);

                        return Task.CompletedTask;
                    });
            });

        public static TextEditorCommand GetChangeMotion(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Change({innerTextEditorCommand.DisplayName})", $"Vim::Change({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            async interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    new List<TextEditorCursorModifier>());

                commandArgs.TextEditorService.EnqueueModification(
                    /* name */,
                    refreshCursorsRequest,
                    () =>
                    {
                        var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                        var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                        var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                        if (viewModel is null || model is null || cursor is null)
                            return Task.CompletedTask; ;

                        /* code */

                        return Task.CompletedTask;
                    });

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

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    new List<TextEditorCursorModifier>());

                commandArgs.TextEditorService.EnqueueModification(
                    /* name */,
                    refreshCursorsRequest,
                    () =>
                    {
                        var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                        var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                        var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                        if (viewModel is null || model is null || cursor is null)
                            return Task.CompletedTask; ;

                        /* code */

                        return Task.CompletedTask;
                    });

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
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    new List<TextEditorCursorModifier>());

                commandArgs.TextEditorService.EnqueueModification(
                    /* name */,
                    refreshCursorsRequest,
                    () =>
                    {
                        var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                        var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                        var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                        if (viewModel is null || model is null || cursor is null)
                            return Task.CompletedTask; ;

                        /* code */

                        return Task.CompletedTask;
                    });

                await TextEditorCommandDefaultFacts.Copy.DoAsyncFunc.Invoke(commandArgs);
                await TextEditorCommandDefaultFacts.ClearTextSelection.DoAsyncFunc.Invoke(commandArgs);
            });

        public static readonly TextEditorCommand NewLineBelow = new(
            "Vim::NewLineBelow()", "Vim::NewLineBelow()", false, true, TextEditKind.None, null,
            async interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    new List<TextEditorCursorModifier>());

                commandArgs.TextEditorService.EnqueueModification(
                    /* name */,
                    refreshCursorsRequest,
                    () =>
                    {
                        var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                        var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                        var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                        if (viewModel is null || model is null || cursor is null)
                            return Task.CompletedTask; ;

                        /* code */

                        return Task.CompletedTask;
                    });

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

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    new List<TextEditorCursorModifier>());

                commandArgs.TextEditorService.EnqueueModification(
                    /* name */,
                    refreshCursorsRequest,
                    () =>
                    {
                        var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                        var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                        var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                        if (viewModel is null || model is null || cursor is null)
                            return Task.CompletedTask; ;

                        /* code */

                        return Task.CompletedTask;
                    });

                var activeKeymap = commandArgs.TextEditorService.OptionsStateWrap.Value.Options.Keymap
                    ?? TextEditorKeymapFacts.DefaultKeymap;

                if (activeKeymap is not TextEditorKeymapVim textEditorKeymapVim)
                    return;

                await TextEditorCommandDefaultFacts.NewLineAbove.DoAsyncFunc.Invoke(commandArgs);
                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
            });
    }
}