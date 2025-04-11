using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// While parsing expression, it is necessary that there exists a node
/// that indicates a lambda expression is being parsed.
///
/// This type might be more along the lines of a "builder" type.
/// It is meant to be made when starting lambda expression,
/// then the primary expression can be equal to an instae of this type.
///
/// This then directs the parser accordingly until the lambda expression
/// is fully parsed.
///
/// At this point, it is planned that a FunctionDefinitionNode will be
/// made, and a 'MethodGroupExpressionNode' (this type does not yet exist) will be returned as the
/// primary expression.
/// </summary>
public sealed class LambdaExpressionNode : IExpressionNode, ICodeBlockOwner
{
	public LambdaExpressionNode(TypeClauseNode resultTypeClauseNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.LambdaExpressionNode++;
		#endif
	
		ResultTypeClauseNode = resultTypeClauseNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public TypeClauseNode ResultTypeClauseNode { get; }

	/// <summary>
	/// () => "Abc";
	///     Then this property is true;
	///
	/// () => { return "Abc" };
	///     Then this property is false;
	/// </summary>
	public bool CodeBlockNodeIsExpression { get; set; } = true;
	public bool HasReadParameters { get; set; }
	public List<VariableDeclarationNode> VariableDeclarationNodeList { get; } = new();

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.LambdaExpressionNode;

	public TypeClauseNode ReturnTypeClauseNode { get; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; private set; }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

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

	public void AddVariableDeclarationNode(VariableDeclarationNode variableDeclarationNode)
	{
		VariableDeclarationNodeList.Add(variableDeclarationNode);
		_childListIsDirty = true;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		// ResultTypeClauseNode, VariableDeclarationNodeList.Count
		var childCount =
			1 +                                // ResultTypeClauseNode
			VariableDeclarationNodeList.Count; // VariableDeclarationNodeList.Count

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ResultTypeClauseNode;
		foreach (var item in VariableDeclarationNodeList)
		{
			childList[i++] = item;
		}

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
