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
	public SyntaxViewModel(
		ITextEditorSymbol? targetSymbol,
		ISyntaxNode? targetNode,
		ISyntaxNode? definitionNode,
		int depth)
	{
		TargetSymbol = targetSymbol;
		TargetNode = targetNode;
		DefinitionNode = definitionNode;
		Depth = depth;
	}
	
	/// <summary>
	/// The user interface gesture is to hover a symbol in the text editor.
	/// Other than a UI event, it is unlikely this property is non-null.
	/// </summary>
	public ITextEditorSymbol? TargetSymbol { get; }
	
	/// <summary>
	/// The user interface provided an 'ITextEditorSymbol',
	/// but there is little data available on a symbol relative
	/// to the an ISyntaxNode it represents.
	///
	/// Therefore, the symbol is mapped to its corresponding node,
	/// so that more information can be displayed on the UI.
	/// </summary>
	public ISyntaxNode? TargetNode { get; }
	
	/// <summary>
	/// The user interface goes from an ISymbol to an ISyntaxNode,
	/// but that ISyntaxNode might be a reference to a definition node, declaration node, or etc...
	///
	/// So, since there is other data that exists on the definition node, that the reference node
	/// does not have access to, then the definition node is retrieved
	/// in order to display more information on the UI.
	///
	/// As well, if there is a definition node, then when
	/// the text is hovered, it should show an underline and be clickable.
	/// This click will then 'goto definition' to that node.
	/// </summary>
	public ISyntaxNode? DefinitionNode { get; }
	
	/// <summary>
	/// The 'TypeSyntaxDisplay' is used recursively because a 'TypeClauseNode'
	/// can have a 'GenericParametersListingNode' which goes on to contains other 'TypeClauseNode'
	/// (which as well may or may not have their own 'GenericParametersListingNode').
	///
	/// But, it is only for a 'Depth == 0' that extra information about the
	/// node should be added to the UI. (is it public..., partial, class or struct..., etc...)
	/// </summary>
	public int Depth { get; }
	
	public bool ShowPrefixText { get; } = false;
	public bool ShouldTryResolveNullDefinitionNode { get; } = false;
	
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
