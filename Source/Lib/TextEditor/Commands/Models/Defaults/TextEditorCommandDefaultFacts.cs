using Luthetus.TextEditor.RazorLib.Edits.Models;
using Microsoft.JSInterop;

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

            commandArgs.TextEditorService.PostRedundant(
                $"{nameof(Copy)}_{commandArgs.ViewModelKey.Guid}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.CopyFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(Cut)}_{commandArgs.ViewModelKey.Guid}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.CutFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(PasteCommand)}_{commandArgs.ViewModelKey.Guid}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.PasteFactory(
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

            commandArgs.TextEditorService.PostRedundant(
                $"{nameof(Save)}_{commandArgs.ViewModelKey.Guid}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.SaveFactory(
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

            commandArgs.TextEditorService.PostRedundant(
                $"{nameof(SelectAll)}_{commandArgs.ViewModelKey.Guid}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.SelectAllFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(Undo)}_{commandArgs.ViewModelKey.Guid}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.UndoFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(Redo)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.RedoFactory(
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

            commandArgs.TextEditorService.PostRedundant(
                $"{nameof(Remeasure)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.RemeasureFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(ScrollLineDown)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.ScrollLineDownFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(ScrollLineUp)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.ScrollLineUpFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(ScrollPageDown)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.ScrollPageDownFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(ScrollPageUp)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.ScrollPageUpFactory(
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

            commandArgs.TextEditorService.PostRedundant(
                $"{nameof(CursorMovePageBottom)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.CursorMovePageBottomFactory(
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

            commandArgs.TextEditorService.PostRedundant(
                $"{nameof(CursorMovePageTop)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.CursorMovePageTopFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(Duplicate)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.DuplicateFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(IndentMore)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.IndentMoreFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(IndentLess)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.IndentLessFactory(
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

            commandArgs.TextEditorService.PostRedundant(
                $"{nameof(ClearTextSelection)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.ClearTextSelectionFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(NewLineBelow)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.NewLineBelowFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(NewLineAbove)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.NewLineAboveFactory(
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

            commandArgs.TextEditorService.PostIndependent(
                $"{nameof(GoToMatchingCharacterFactory)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.GoToMatchingCharacterFactory(
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

            commandArgs.TextEditorService.PostRedundant(
                $"{nameof(GoToDefinition)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.GoToDefinitionFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ShowFindAllDialog = new(
        "OpenFindDialog", "defaults_open-find-dialog", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.PostRedundant(
                $"{nameof(ShowFindAllDialog)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.ShowFindAllDialogFactory(
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

            commandArgs.TextEditorService.PostRedundant(
                $"{nameof(ShowTooltipByCursorPosition)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.ShowTooltipByCursorPositionFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));

            return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ShowFindOverlay = new(
        "ShowFindOverlay", "defaults_show-find-overlay", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            commandArgs.TextEditorService.PostRedundant(
                $"{nameof(ShowFindOverlay)}_{commandArgs.ViewModelKey}",
                commandArgs.Events,
                commandArgs.ViewModelKey,
                async editContext =>
                {
                    var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                    if (viewModelModifier is null)
                        return;

                    if (viewModelModifier.ViewModel.ShowFindOverlay &&
                        commandArgs.JsRuntime is not null)
                    {
                        _ = await commandArgs.JsRuntime.InvokeAsync<bool>(
                                "luthetusIde.tryFocusHtmlElementById",
                                viewModelModifier.ViewModel.FindOverlayId)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                        {
                            ShowFindOverlay = true,
                        };
                    }
                    
                    return;
                });

            return Task.CompletedTask;
        });
}