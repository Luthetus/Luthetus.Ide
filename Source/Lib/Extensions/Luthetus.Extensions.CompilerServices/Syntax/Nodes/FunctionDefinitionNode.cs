using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// TODO: Track the open and close braces for the function body.
/// </summary>
public sealed class FunctionDefinitionNode : ICodeBlockOwner
{
	public FunctionDefinitionNode(
		AccessModifierKind accessModifierKind,
		TypeClauseNode returnTypeClauseNode,
		SyntaxToken functionIdentifierToken,
		GenericParametersListingNode? genericArgumentsListingNode,
		FunctionArgumentsListingNode functionArgumentsListingNode,
		CodeBlockNode? codeBlockNode,
		ConstraintNode? constraintNode)
	{
		AccessModifierKind = accessModifierKind;
		ReturnTypeClauseNode = returnTypeClauseNode;
		FunctionIdentifierToken = functionIdentifierToken;
		GenericArgumentsListingNode = genericArgumentsListingNode;
		FunctionArgumentsListingNode = functionArgumentsListingNode;
		CodeBlockNode = codeBlockNode;
		ConstraintNode = constraintNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public AccessModifierKind AccessModifierKind { get; }
	public TypeClauseNode ReturnTypeClauseNode { get; }
	public SyntaxToken FunctionIdentifierToken { get; }
	public GenericParametersListingNode? GenericArgumentsListingNode { get; }
	public FunctionArgumentsListingNode FunctionArgumentsListingNode { get; }
	public ConstraintNode? ConstraintNode { get; private set; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; private set; }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
	public int? ScopeIndexKey { get; set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.FunctionDefinitionNode;

	public ICodeBlockOwner SetConstraintNode(ConstraintNode constraintNode)
	{
		ConstraintNode = constraintNode;

		_childListIsDirty = true;
		return this;
	}

	public ICodeBlockOwner SetExpressionBody(CodeBlockNode codeBlockNode)
	{
		CodeBlockNode = codeBlockNode;

		_childListIsDirty = true;
		return this;
	}

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

		var childCount = 3; // ReturnTypeClauseNode, FunctionIdentifierToken, ...FunctionArgumentsListingNode,
		if (GenericArgumentsListingNode is not null)
			childCount++;
		if (CodeBlockNode is not null)
			childCount++;
		if (ConstraintNode is not null)
			childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ReturnTypeClauseNode;
		childList[i++] = FunctionIdentifierToken;
		if (GenericArgumentsListingNode is not null)
			childList[i++] = GenericArgumentsListingNode;
		childList[i++] = FunctionArgumentsListingNode;
		if (CodeBlockNode is not null)
			childList[i++] = CodeBlockNode;
		if (ConstraintNode is not null)
			childList[i++] = ConstraintNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
