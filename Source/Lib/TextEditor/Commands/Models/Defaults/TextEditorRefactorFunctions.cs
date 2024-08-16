using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

/// <summary>
/// TODO: Decide on what folder this class belongs in...
///       ...I'm working on the Quick Actions / Refactor context menu (2024-08-16)
///       and the issue is that, the context menu is populated with menu options from
///       within the <see cref="ITextEditorEditContext"/>.
///       |
///       But by the time the user picks a context menu option, the text editor
///       may have been edited, thus invalidating any stored Func<Task> that is given
///       for a menu option's onclick.
///	   |
///       So, each menu option needs to make a separate Post to the <see cref="ITextEditorService"/>
///       upon onclick.
///       |
///       But, all of the variables that were created to construct the menu options,
///       during the old post, are still within scope when typing out code for the
///       menu option's onclick if written inline as a lambda function.
///       |
///       So, most importantly this class exists to avoid accidental usage of old text editor state
///       due to the variables being in scope.
///       |
///       But this also probably is nice for readability, and re-usability of the refactor itself.
///       |
///       I don't want a separate function within <see cref="TextEditorCommandDefaultFunctions"/> because
///       it is a nice separation of concern to keep compiler service refactors in a separate file.
/// </summary>
public class TextEditorRefactorFunctions
{
	/// <summary>
	/// How much concern should be given to the text editor changing between the time that
	/// the context menu option was generated, and this function is invoked?
	///
	/// If the { Ctrl + Period } keybind is hit when the cursor is at positionIndex 30.
	/// But then 100 characters of text was inserted at positionIndex 0, by the time
	/// this function gets access to the text editor, anything the menu option tells this
	/// function will be wrong by 100 position indices.
	///
	/// Preferably, long term, 100% concern should be given to the text editor changing between the time that
	/// the context menu option was generated, and this function is invoked.
	///
	/// But, in terms of progressively approaching that goal, how should this be handled?
	/// </summary>
	public static void GenerateConstructor(
		TypeDefinitionNode unsafeTypeDefinitionNode,
		IEnumerable<IVariableDeclarationNode> variableDeclarationNodeList,
		IServiceProvider serviceProvider,
		ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier primaryCursorModifier)
    {
		var compilerService = modelModifier.CompilerService;
	
		var compilerServiceResource = compilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri);
		
		if (compilerServiceResource is null)
		{
			NotificationHelper.DispatchError(
		        nameof(GenerateConstructor), "compilerServiceResource was null", serviceProvider.GetRequiredService<ICommonComponentRenderers>(), serviceProvider.GetRequiredService<IDispatcher>(), TimeSpan.FromSeconds(6));
			return;
		}
    
		// Try to match unsafeTypeDefinitionNode to the current text editor state
		// in the event that an edit to the document was made between the time
		// that the menu option was constructed, and the menu option was onclick'd.
		{
			var unsafeStartingIndexInclusive = unsafeTypeDefinitionNode.TypeIdentifierToken.TextSpan.StartingIndexInclusive;

			var syntaxNode = compilerService.Binder.GetSyntaxNode(unsafeStartingIndexInclusive, compilerServiceResource.CompilationUnit);
			
			if (syntaxNode is null)
			{
				NotificationHelper.DispatchError(
			        nameof(GenerateConstructor), "syntaxNode was null", serviceProvider.GetRequiredService<ICommonComponentRenderers>(), serviceProvider.GetRequiredService<IDispatcher>(), TimeSpan.FromSeconds(6));
				return;
			}
			
			if (syntaxNode is not TypeDefinitionNode typeDefinitionNode)
			{
				NotificationHelper.DispatchError(
			        nameof(GenerateConstructor), $"Node is not {nameof(SyntaxKind)}.{nameof(SyntaxKind.TypeDefinitionNode)} it is: {nameof(SyntaxKind)}.{syntaxNode.Parent.SyntaxKind}", serviceProvider.GetRequiredService<ICommonComponentRenderers>(), serviceProvider.GetRequiredService<IDispatcher>(), TimeSpan.FromSeconds(6));
			    return;
			}
			
			if (typeDefinitionNode.TypeBodyCodeBlockNode is null)
			{
				NotificationHelper.DispatchError(
			        nameof(GenerateConstructor), $"typeDefinitionNode.TypeBodyCodeBlockNode was null", serviceProvider.GetRequiredService<ICommonComponentRenderers>(), serviceProvider.GetRequiredService<IDispatcher>(), TimeSpan.FromSeconds(6));
			    return;
			}
			
			try
			{
				var insertionPointForText = 1 + typeDefinitionNode.OpenBraceToken.TextSpan.StartingIndexInclusive;
				
				var lineAndColumnIndices = modelModifier.GetLineAndColumnIndicesFromPositionIndex(insertionPointForText);
				
				var cursor = new TextEditorCursor(
					lineAndColumnIndices.lineIndex,
					lineAndColumnIndices.columnIndex,
					true);
			
				modelModifier.Insert(
					$"\n\tpublic {typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText()}()\n\t{{\n\t\t\n\t}}\n",
					new(Key<TextEditorViewModel>.Empty, new List<TextEditorCursorModifier> { new(cursor) }));
			}
			catch (Exception e)
			{
				NotificationHelper.DispatchError(
			        nameof(GenerateConstructor), e.ToString(), serviceProvider.GetRequiredService<ICommonComponentRenderers>(), serviceProvider.GetRequiredService<IDispatcher>(), TimeSpan.FromSeconds(6));
			}
			finally
			{
				NotificationHelper.DispatchInformative(
			        nameof(GenerateConstructor), "?", serviceProvider.GetRequiredService<ICommonComponentRenderers>(), serviceProvider.GetRequiredService<IDispatcher>(), TimeSpan.FromSeconds(6));
			}
		}
    
        return;
    }
}
