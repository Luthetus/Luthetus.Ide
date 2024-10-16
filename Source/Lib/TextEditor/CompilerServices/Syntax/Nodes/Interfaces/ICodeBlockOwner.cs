using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

/// <summary>
/// TODO: Change the 'public CodeBlockNode CodeBlockNode { get; }' property for...
/// 	  ...any type that implements this. Have it come from this interface.
/// </summary>
public interface ICodeBlockOwner : ISyntaxNode
{
	public ScopeDirectionKind ScopeDirectionKind { get; }
	public OpenBraceToken OpenBraceToken { get; }
	public CodeBlockNode? CodeBlockNode { get; }
	
	public TypeClauseNode? GetReturnTypeClauseNode();
	
	public ICodeBlockOwner SetCodeBlockNode(OpenBraceToken openBraceToken, CodeBlockNode codeBlockNode);
	
	/// <summary>
	/// Once the code block owner's scope has been constructed,
	/// this method gives them an opportunity to pull any variables
	/// into scope that ought to be there.
	/// As well, the current code block builder will have been updated.
	///
	/// (i.e.: a function definition's arguments)
	/// </summary>
	public void OnBoundScopeCreatedAndSetAsCurrent(IParserModel parserModel);
}
