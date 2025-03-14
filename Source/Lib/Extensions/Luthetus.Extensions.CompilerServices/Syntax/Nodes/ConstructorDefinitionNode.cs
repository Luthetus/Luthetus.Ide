using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.Extensions.CompilerServices;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ConstructorDefinitionNode : ICodeBlockOwner
{
	public ConstructorDefinitionNode(
		TypeClauseNode returnTypeClauseNode,
		SyntaxToken functionIdentifier,
		GenericArgumentsListingNode? genericArgumentsListingNode,
		FunctionArgumentsListingNode functionArgumentsListingNode,
		CodeBlockNode? codeBlockNode)
	{
		// Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ConstructorDefinitionNode++;
	
		ReturnTypeClauseNode = returnTypeClauseNode;
		FunctionIdentifier = functionIdentifier;
		GenericArgumentsListingNode = genericArgumentsListingNode;
		FunctionArgumentsListingNode = functionArgumentsListingNode;
		CodeBlockNode = codeBlockNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public TypeClauseNode ReturnTypeClauseNode { get; }
	public SyntaxToken FunctionIdentifier { get; }
	public GenericArgumentsListingNode? GenericArgumentsListingNode { get; }
	public FunctionArgumentsListingNode FunctionArgumentsListingNode { get; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; private set; }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
	public int? ScopeIndexKey { get; set; }

	/// <summary>
	/// public MyConstructor(string firstName)
	/// 	: base(firstName)
	/// {
	/// }
	///
	/// This stores the indices of tokens that deliminate the parameters to the 'base()' invocation.
	/// The reason for this is the invocation needs to have 'string firstName' in scope.
	/// But, 'string firstName' doesn't come into scope until the '{' token.
	/// 
	/// So, remember where the parameters to the 'base()' invocation were,
	/// then later when 'string firstName' is in scope, parse the parameters.
	/// </summary>
	public (int OpenParenthesisIndex, int CloseParenthesisIndex)? OtherConstructorInvocation { get; set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ConstructorDefinitionNode;

	#region ICodeBlockOwner_Methods
	public TypeClauseNode? GetReturnTypeClauseNode()
	{
		return ReturnTypeClauseNode;
	}

	public ICodeBlockOwner SetOpenCodeBlockTextSpan(TextEditorTextSpan? openCodeBlockTextSpan, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker)
	{
		if (OpenCodeBlockTextSpan is not null)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticList, tokenWalker);

		OpenCodeBlockTextSpan = openCodeBlockTextSpan;

		_childListIsDirty = true;
		return this;
	}

	public ICodeBlockOwner SetCloseCodeBlockTextSpan(TextEditorTextSpan? closeCodeBlockTextSpan, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker)
	{
		if (CloseCodeBlockTextSpan is not null)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticList, tokenWalker);

		CloseCodeBlockTextSpan = closeCodeBlockTextSpan;

		_childListIsDirty = true;
		return this;
	}

	public ICodeBlockOwner SetCodeBlockNode(CodeBlockNode codeBlockNode, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker)
	{
		if (CodeBlockNode is not null)
			ICodeBlockOwner.ThrowAlreadyAssignedCodeBlockNodeException(diagnosticList, tokenWalker);

		CodeBlockNode = codeBlockNode;

		_childListIsDirty = true;
		return this;
	}
	#endregion

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		// ReturnTypeClauseNode, FunctionIdentifier, ...FunctionArgumentsListingNode,
		var childCount = 3;
		if (GenericArgumentsListingNode is not null)
			childCount++;
		if (CodeBlockNode is not null)
			childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ReturnTypeClauseNode;
		childList[i++] = FunctionIdentifier;
		if (GenericArgumentsListingNode is not null)
			childList[i++] = GenericArgumentsListingNode;
		childList[i++] = FunctionArgumentsListingNode;
		if (CodeBlockNode is not null)
			childList[i++] = CodeBlockNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
