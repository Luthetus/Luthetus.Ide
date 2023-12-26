using Luthetus.TextEditor.RazorLib.Edits.Models;

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

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.CopyFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Cut = new(
        "Cut", "defaults_cut", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.CutFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand PasteCommand = new(
        "Paste", "defaults_paste", false, true, TextEditKind.Other, "defaults_paste",
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.PasteFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Save = new(
        "Save", "defaults_save", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.SaveFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand SelectAll = new(
        "Select All", "defaults_select-all", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.SelectAllFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Undo = new(
        "Undo", "defaults_undo", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.UndoFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Redo = new(
        "Redo", "defaults_redo", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.RedoFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Remeasure = new(
        "Remeasure", "defaults_remeasure", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.RemeasureFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollLineDown = new(
        "Scroll Line Down", "defaults_scroll-line-down", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.ScrollLineDownFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollLineUp = new(
        "Scroll Line Up", "defaults_scroll-line-up", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.ScrollLineUpFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollPageDown = new(
        "Scroll Page Down", "defaults_scroll-page-down", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.ScrollPageDownFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollPageUp = new(
        "Scroll Page Up", "defaults_scroll-page-up", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.ScrollPageUpFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand CursorMovePageBottom = new(
        "Move Cursor to Bottom of the Page", "defaults_cursor-move-page-bottom", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.CursorMovePageBottomFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand CursorMovePageTop = new(
        "Move Cursor to Top of the Page", "defaults_cursor-move-page-top", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.CursorMovePageTopFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Duplicate = new(
        "Duplicate", "defaults_duplicate", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.DuplicateFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand IndentMore = new(
        "Indent More", "defaults_indent-more", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.IndentMoreFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand IndentLess = new(
        "Indent Less", "defaults_indent-less", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.IndentLessFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ClearTextSelection = new(
        "ClearTextSelection", "defaults_clear-text-selection", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.ClearTextSelectionFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand NewLineBelow = new(
        "NewLineBelow", "defaults_new-line-below", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.NewLineBelowFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand NewLineAbove = new(
        "NewLineBelow", "defaults_new-line-below", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.NewLineAboveFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static TextEditorCommand GoToMatchingCharacterFactory(bool shouldSelectText) => new(
        "GoToMatchingCharacter", "defaults_go-to-matching-character", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.ShouldSelectText = shouldSelectText;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.GoToMatchingCharacterFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand GoToDefinition = new(
        "GoToDefinition", "defaults_go-to-definition", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.GoToDefinitionFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ShowFindDialog = new(
        "OpenFindDialog", "defaults_open-find-dialog", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.ShowFindDialogFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

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

            commandArgs.TextEditorService.Post(TextEditorCommandDefaultFunctions.ShowTooltipByCursorPositionFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs));

            return Task.CompletedTask;
        });
}