using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// All the properties are null because the symbol UI is used recursively.
/// 
/// i.e.:
/// TypeDefinitionNode renders TypeDefinitionNodeDisplay
/// 	TypeSyntaxDisplay
/// 	GenericArgumentsListingNode
/// 		TypeSyntaxDisplay
/// 		...
///
/// So, the symbol tooltip starts with 'symbol' not being null.
///
/// But, if the UI that represents the symbol requires
/// multiple nodes to be shown, symbol goes on to be null for the inner UI components.
///
/// The 'SymbolDisplay' is expected to prevent itself from re-rendering unnecessarily.
/// So, the optimization of the tooltip is not as crucial because
/// of that, and furthermore because it is a throttled user gesture
/// so many many loops of rendering this within a short period of time are expected to never occur.
/// </summary>
public struct SyntaxViewModel
{
	public SyntaxViewModel(ITextEditorSymbol? symbol, ISyntaxNode? targetNode, ISyntaxNode? definitionNode)
	{
		Symbol = symbol;
		TargetNode = targetNode;
		DefinitionNode = definitionNode;
	}
	
	public ITextEditorSymbol? Symbol { get; }
	public ISyntaxNode? TargetNode { get; }
	public ISyntaxNode? DefinitionNode { get; }
	
	public Task HandleOnClick(ITextEditorService textEditorService, SyntaxKind syntaxKindExpected)
	{
		if (DefinitionNode is null ||
			DefinitionNode.SyntaxKind != syntaxKindExpected)
		{
			return Task.CompletedTask;
		}
		
		string? resourceUriValue = null;
		var indexInclusiveStart = -1;
		
		if (DefinitionNode.SyntaxKind == SyntaxKind.TypeDefinitionNode)
		{
			var typeDefinitionNode = (TypeDefinitionNode)DefinitionNode;
			resourceUriValue = typeDefinitionNode.TypeIdentifierToken.TextSpan.ResourceUri.Value;
			indexInclusiveStart = typeDefinitionNode.TypeIdentifierToken.TextSpan.StartingIndexInclusive;
		}
		
		if (resourceUriValue is null || indexInclusiveStart == -1)
			return Task.CompletedTask;
	
		return textEditorService.OpenInEditorAsync(
			resourceUriValue,
			true,
			indexInclusiveStart,
			new Category("main"),
			Key<TextEditorViewModel>.NewKey());
	}
}
