using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

/// <summary>
/// TODO: Change the 'public CodeBlockNode CodeBlockNode { get; }' property for...
/// 	  ...any type that implements this. Have it come from this interface.
/// </summary>
public interface ICodeBlockOwner : ISyntaxNode
{
	public ScopeDirectionKind ScopeDirectionKind { get; }
	public CodeBlockNode? CodeBlockNode { get; }
	public OpenBraceToken? OpenBraceToken { get; }
	
	public TypeClauseNode? GetReturnTypeClauseNode();
	
	/// <summary>
	/// TODO: Awkward 'With' naming yet it sets the property.
	/// TODO: Recreate the ChildList after setting the CodeBlockNode
	/// </summary>
	public ICodeBlockOwner WithCodeBlockNode(OpenBraceToken openBraceToken, CodeBlockNode codeBlockNode);
}
