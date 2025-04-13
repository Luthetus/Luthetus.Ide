using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class NamespaceStatementNode : ICodeBlockOwner
{
	public NamespaceStatementNode(
		SyntaxToken keywordToken,
		SyntaxToken identifierToken,
		CodeBlock codeBlock)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.NamespaceStatementNode++;
		#endif
	
		KeywordToken = keywordToken;
		IdentifierToken = identifierToken;
		CodeBlock = codeBlock;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken KeywordToken { get; }
	public SyntaxToken IdentifierToken { get; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Both;
	public TextEditorTextSpan OpenCodeBlockTextSpan { get; set; }
	public CodeBlock CodeBlock { get; set; }
	public TextEditorTextSpan CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.NamespaceStatementNode;

	/// <summary>
	/// <see cref="GetTopLevelTypeDefinitionNodes"/> provides a collection
	/// which contains all top level type definitions of the <see cref="NamespaceStatementNode"/>.
	/// </summary>
	public IEnumerable<TypeDefinitionNode> GetTopLevelTypeDefinitionNodes()
	{
		var localCodeBlockNode = CodeBlock;

		if (!localCodeBlockNode.ConstructorWasInvoked)
			return Array.Empty<TypeDefinitionNode>();

		return localCodeBlockNode.ChildList
			.Where(innerC => innerC.SyntaxKind == SyntaxKind.TypeDefinitionNode)
			.Select(td => (TypeDefinitionNode)td);
	}

	#region ICodeBlockOwner_Methods
	public TypeReference GetReturnTypeReference()
	{
		return TypeFacts.Empty.ToTypeReference();
	}

	public void SetChildListIsDirty(bool childListIsDirty)
	{
		_childListIsDirty = childListIsDirty;
	}
	#endregion

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 2; // KeywordToken, IdentifierToken,
		// if (CodeBlockNode is not null)
		// 	childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = KeywordToken;
		childList[i++] = IdentifierToken;
		// if (CodeBlockNode is not null)
		// 	childList[i++] = CodeBlockNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
