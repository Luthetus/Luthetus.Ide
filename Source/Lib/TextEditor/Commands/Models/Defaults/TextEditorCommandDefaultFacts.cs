using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public static class TextEditorCommandDefaultFacts
{
	/*
    public static readonly TextEditorCommand RefreshSyntaxHighlighting = new(
        "Refresh Syntax Highlighting", "defaults_refresh_syntax_highlighting", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            
            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
                return ValueTask.CompletedTask;
                
            modelModifier.CompilerService.ResourceWasModified(
				modelModifier.ResourceUri,
				Array.Empty<TextEditorTextSpan>());
		    return ValueTask.CompletedTask;
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
			
			var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
                return;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(primaryCursorModifier, modelModifier);
			if (selectedText is not null)
			{
				viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                {
                    FindOverlayValue = selectedText,
                    FindOverlayValueExternallyChangedMarker = !viewModelModifier.ViewModel.FindOverlayValueExternallyChangedMarker,
                };
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
        });
        
    public static readonly TextEditorCommand PopulateSearchFindAll = new(
        "PopulateSearchFindAll", "defaults_populate-search-find-all", false, true, TextEditKind.None, null,
        async interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);

            if (viewModelModifier is null)
                return;

			// If the user has an active text selection,
			// then populate the find overlay with their selection.
			
			var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
                return;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(primaryCursorModifier, modelModifier);
			if (selectedText is null)
				return;
			
			var findAllService = commandArgs.ServiceProvider.GetRequiredService<IFindAllService>();
			findAllService.SetSearchQuery(selectedText);
        });
    */
}
