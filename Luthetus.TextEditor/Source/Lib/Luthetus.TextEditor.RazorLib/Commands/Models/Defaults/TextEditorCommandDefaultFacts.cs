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

            var edit = commandArgs.TextEditorService.CreateEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);
                
                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions.CopyAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Cut = new(
        "Cut", "defaults_cut", false, true, TextEditKind.None, null,
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

    public static readonly TextEditorCommand PasteCommand = new(
        "Paste", "defaults_paste", false, true, TextEditKind.Other, "defaults_paste",
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

                await TextEditorCommandDefaultFunctions.PasteAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Save = new(
        "Save", "defaults_save", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.SaveAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand SelectAll = new(
        "Select All", "defaults_select-all", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.SelectAllAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Undo = new(
        "Undo", "defaults_undo", false, true, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.UndoAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Redo = new(
        "Redo", "defaults_redo", false, true, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.RedoAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Remeasure = new(
        "Remeasure", "defaults_remeasure", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.RemeasureAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollLineDown = new(
        "Scroll Line Down", "defaults_scroll-line-down", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.ScrollLineDownAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollLineUp = new(
        "Scroll Line Up", "defaults_scroll-line-up", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.ScrollLineUpAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollPageDown = new(
        "Scroll Page Down", "defaults_scroll-page-down", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.ScrollPageDownAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollPageUp = new(
        "Scroll Page Up", "defaults_scroll-page-up", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.ScrollPageUpAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand CursorMovePageBottom = new(
        "Move Cursor to Bottom of the Page", "defaults_cursor-move-page-bottom", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.CursorMovePageBottomAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand CursorMovePageTop = new(
        "Move Cursor to Top of the Page", "defaults_cursor-move-page-top", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.CursorMovePageTopAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Duplicate = new(
        "Duplicate", "defaults_duplicate", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.DuplicateAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand IndentMore = new(
        "Indent More", "defaults_indent-more", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.IndentMoreAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand IndentLess = new(
        "Indent Less", "defaults_indent-less", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.IndentLessAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ClearTextSelection = new(
        "ClearTextSelection", "defaults_clear-text-selection", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.ClearTextSelectionAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand NewLineBelow = new(
        "NewLineBelow", "defaults_new-line-below", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.NewLineBelowAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand NewLineAbove = new(
        "NewLineBelow", "defaults_new-line-below", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.NewLineAboveAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static TextEditorCommand GoToMatchingCharacterFactory(bool shouldSelectText) => new(
        "GoToMatchingCharacter", "defaults_go-to-matching-character", false, true, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.GoToMatchingCharacterFactoryAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand GoToDefinition = new(
        "GoToDefinition", "defaults_go-to-definition", false, true, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.GoToDefinitionAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ShowFindDialog = new(
        "OpenFindDialog", "defaults_open-find-dialog", false, false, TextEditKind.None, null,
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

                await TextEditorCommandDefaultFunctions.ShowFindDialogAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
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

            var edit = commandArgs.TextEditorService.CreateEdit(async context =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                var refreshCursorsRequest = new TextEditorService.RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

                await TextEditorCommandDefaultFunctions.ShowTooltipByCursorPositionAsync(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    refreshCursorsRequest.CursorBag.First(x => x.IsPrimaryCursor));
            });

            commandArgs.TextEditorService.EnqueueEdit(edit);
            return Task.CompletedTask;
        });
}