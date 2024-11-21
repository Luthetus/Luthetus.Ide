using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// All the properties are null because the symbol UI is used recursively.
/// 
/// i.e.:
/// TypeDefinitionNode renders TypeDefinitionNodeDisplay
/// 	TypeClauseNodeDisplay
/// 	GenericArgumentsListingNode
/// 		TypeClauseNodeDisplay
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
public struct SymbolViewModel
{
	public SymbolViewModel(ITextEditorSymbol? symbol, ISyntaxNode? targetNode, ISyntaxNode? definitionNode)
	{
		Symbol = symbol;
		TargetNode = targetNode;
		DefinitionNode = definitionNode;
	}
	
	public ITextEditorSymbol? Symbol { get; }
	public ISyntaxNode? TargetNode { get; }
	public ISyntaxNode? DefinitionNode { get; }
}
