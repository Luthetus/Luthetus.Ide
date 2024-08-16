using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public class TextEditorRefactorFacts
{
	public static void GenerateConstructor(
		ITextEditorService textEditorService,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
    	textEditorService.PostUnique(nameof(GenerateConstructor), editContext =>
		{
			var modelModifier = editContext.GetModelModifier(resourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
			
			TextEditorRefactorFunctions.GenerateConstructor(
				editContext,
				modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        primaryCursorModifier);
		    return Task.CompletedTask;
		});
    }
}
