using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public static class TextEditorCommandDefaultFacts
{
    public static readonly TextEditorCommand DoNothingDiscard = new(
        "DoNothingDiscard", "defaults_do-nothing-discard", false, false, TextEditKind.None, null,
        interfaceCommandArgs => Task.CompletedTask);

    public static readonly TextEditorCommand Copy = new(
        "Copy", "defaults_copy", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .CopyAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Cut = new(
        "Cut", "defaults_cut", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .CutAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand PasteCommand = new(
        "Paste", "defaults_paste", false, true, TextEditKind.Other, "defaults_paste",
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .PasteAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Save = new(
        "Save", "defaults_save", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .SaveAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand SelectAll = new(
        "Select All", "defaults_select-all", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .SelectAllAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Undo = new(
        "Undo", "defaults_undo", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .UndoAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Redo = new(
        "Redo", "defaults_redo", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .RedoAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Remeasure = new(
        "Remeasure", "defaults_remeasure", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .RemeasureAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollLineDown = new(
        "Scroll Line Down", "defaults_scroll-line-down", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .ScrollLineDownAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollLineUp = new(
        "Scroll Line Up", "defaults_scroll-line-up", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .ScrollLineUpAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollPageDown = new(
        "Scroll Page Down", "defaults_scroll-page-down", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .ScrollPageDownAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollPageUp = new(
        "Scroll Page Up", "defaults_scroll-page-up", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .ScrollPageUpAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand CursorMovePageBottom = new(
        "Move Cursor to Bottom of the Page", "defaults_cursor-move-page-bottom", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .CursorMovePageBottomAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand CursorMovePageTop = new(
        "Move Cursor to Top of the Page", "defaults_cursor-move-page-top", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .CursorMovePageTopAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Duplicate = new(
        "Duplicate", "defaults_duplicate", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .DuplicateAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand IndentMore = new(
        "Indent More", "defaults_indent-more", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .IndentMoreAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand IndentLess = new(
        "Indent Less", "defaults_indent-less", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .IndentLessAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ClearTextSelection = new(
        "ClearTextSelection", "defaults_clear-text-selection", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .ClearTextSelectionAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand NewLineBelow = new(
        "NewLineBelow", "defaults_new-line-below", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .NewLineBelowAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand NewLineAbove = new(
        "NewLineBelow", "defaults_new-line-below", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .NewLineAboveAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static TextEditorCommand GoToMatchingCharacterFactory(bool shouldSelectText) => new(
        "GoToMatchingCharacter", "defaults_go-to-matching-character", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .GoToMatchingCharacterFactoryAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand GoToDefinition = new(
        "GoToDefinition", "defaults_go-to-definition", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .GoToDefinitionAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ShowFindDialog = new(
        "OpenFindDialog", "defaults_open-find-dialog", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .ShowFindDialogAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });

    /// <summary>
    /// <see cref="ShowTooltipByCursorPosition"/> is to fire the @onmouseover event
    /// so to speak. Such that a tooltip appears if one were to have moused over a symbol or etc...
    /// </summary>
    public static readonly TextEditorCommand ShowTooltipByCursorPosition = new(
        "ShowTooltipByCursorPosition", "defaults_show-tooltip-by-cursor-position", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.EnqueueEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions
                    .ShowTooltipByCursorPositionAsync(context)
                    .ExecuteAsync(context);
            });
            return Task.CompletedTask;
        });
}