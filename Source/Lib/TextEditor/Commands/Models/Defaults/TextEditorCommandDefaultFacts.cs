using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

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
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            return TextEditorCommandDefaultFunctions.CopyAsync(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs.ServiceProvider.GetRequiredService<IClipboardService>());
        });

    public static readonly TextEditorCommand Cut = new(
        "Cut", "defaults_cut", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            return TextEditorCommandDefaultFunctions.CutAsync(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs.ServiceProvider.GetRequiredService<IClipboardService>());
        });

    public static readonly TextEditorCommand PasteCommand = new(
        "Paste", "defaults_paste", false, true, TextEditKind.Other, "defaults_paste",
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            return TextEditorCommandDefaultFunctions.PasteAsync(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs.ServiceProvider.GetRequiredService<IClipboardService>());
        });

    public static readonly TextEditorCommand TriggerSave = new(
        "Save", "defaults_save", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.TriggerSave(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
	        return Task.CompletedTask;
        });

    public static readonly TextEditorCommand SelectAll = new(
        "Select All", "defaults_select-all", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.SelectAll(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Undo = new(
        "Undo", "defaults_undo", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.Undo(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Redo = new(
        "Redo", "defaults_redo", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.Redo(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Remeasure = new(
        "Remeasure", "defaults_remeasure", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.TriggerRemeasure(
            	commandArgs.EditContext,
		        viewModelModifier,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand RefreshSyntaxHighlighting = new(
        "Refresh Syntax Highlighting", "defaults_refresh_syntax_highlighting", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            modelModifier.CompilerService.ResourceWasModified(
				modelModifier.ResourceUri,
				ImmutableArray<TextEditorTextSpan>.Empty);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollLineDown = new(
        "Scroll Line Down", "defaults_scroll-line-down", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.ScrollLineDown(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollLineUp = new(
        "Scroll Line Up", "defaults_scroll-line-up", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.ScrollLineUp(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollPageDown = new(
        "Scroll Page Down", "defaults_scroll-page-down", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.ScrollPageDown(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ScrollPageUp = new(
        "Scroll Page Up", "defaults_scroll-page-up", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.ScrollPageUp(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand CursorMovePageBottom = new(
        "Move Cursor to Bottom of the Page", "defaults_cursor-move-page-bottom", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.CursorMovePageBottom(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand CursorMovePageTop = new(
        "Move Cursor to Top of the Page", "defaults_cursor-move-page-top", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.CursorMovePageTop(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand Duplicate = new(
        "Duplicate", "defaults_duplicate", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.Duplicate(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand IndentMore = new(
        "Indent More", "defaults_indent-more", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.IndentMore(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand IndentLess = new(
        "Indent Less", "defaults_indent-less", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.IndentLess(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ClearTextSelection = new(
        "ClearTextSelection", "defaults_clear-text-selection", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.ClearTextSelection(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand NewLineBelow = new(
        "NewLineBelow", "defaults_new-line-below", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.NewLineBelow(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand NewLineAbove = new(
        "NewLineAbove", "defaults_new-line-above", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.NewLineAbove(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });
    
    public static readonly TextEditorCommand MoveLineDown = new(
        "MoveLineDown", "defaults_move-line-down", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.MoveLineDown(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });
    
    public static readonly TextEditorCommand MoveLineUp = new(
        "MoveLineUp", "defaults_move-line-up", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.MoveLineUp(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static TextEditorCommand GoToMatchingCharacterFactory(bool shouldSelectText) => new(
        "GoToMatchingCharacter", "defaults_go-to-matching-character", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.ShouldSelectText = shouldSelectText;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.GoToMatchingCharacter(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand RelatedFilesQuickPick = new(
        "RelatedFilesQuickPick", "defaults_related-files-quick-pick", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
			
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            return TextEditorCommandDefaultFunctions.RelatedFilesQuickPick(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
        });
    
    public static readonly TextEditorCommand GoToDefinition = new(
        "GoToDefinition", "defaults_go-to-definition", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.GoToDefinition(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
		    return Task.CompletedTask;
        });

    public static readonly TextEditorCommand ShowFindAllDialog = new(
        "OpenFindDialog", "defaults_open-find-dialog", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            TextEditorCommandDefaultFunctions.ShowFindAllDialog(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
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
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
                
            return TextEditorCommandDefaultFunctions.ShowTooltipByCursorPositionAsync(
            	commandArgs.EditContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        commandArgs);
        });

    public static readonly TextEditorCommand ShowFindOverlay = new(
        "ShowFindOverlay", "defaults_show-find-overlay", false, true, TextEditKind.None, null,
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);

            if (viewModelModifier is null)
                return;

			// If the user has an active text selection,
			// then populate the find overlay with their selection.
			{
				var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
	            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
	            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);
	
	            if (modelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
	                return;
	
	            var selectedText = TextEditorSelectionHelper.GetSelectedText(primaryCursorModifier, modelModifier);

				if (selectedText is not null)
				{
					viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                    {
                        FindOverlayValue = selectedText,
                    };
				}
			}

            if (viewModelModifier.ViewModel.ShowFindOverlay)
            {
                await commandArgs.ServiceProvider.GetRequiredService<IJSRuntime>().GetLuthetusCommonApi()
                    .FocusHtmlElementById(viewModelModifier.ViewModel.FindOverlayId)
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
}