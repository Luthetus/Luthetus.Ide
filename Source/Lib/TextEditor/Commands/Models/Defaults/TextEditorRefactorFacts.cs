using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public class TextEditorRefactorFacts
{
	/// <summary>
	/// The variableDeclarationNodeList is referring to the class members
	/// i.e.: fields, and properties, that one wants to initialize from the constructor.
	/// </summary>
	public static void GenerateConstructor(
		TypeDefinitionNode unsafeTypeDefinitionNode,
		IEnumerable<IVariableDeclarationNode> variableDeclarationNodeList,
		IServiceProvider serviceProvider,
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
				unsafeTypeDefinitionNode,
				variableDeclarationNodeList,
				serviceProvider,
				editContext,
				modelModifier,
		        viewModelModifier,
		        cursorModifierBag,
		        primaryCursorModifier);
		    return Task.CompletedTask;
		});
    }
}
