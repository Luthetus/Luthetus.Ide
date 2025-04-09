using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// TODO: Track the open and close braces for the function body.
/// </summary>
public sealed class FunctionDefinitionNode : ICodeBlockOwner, IFunctionDefinitionNode, IGenericParameterNode
{
	public FunctionDefinitionNode(
		AccessModifierKind accessModifierKind,
		TypeClauseNode returnTypeClauseNode,
		SyntaxToken functionIdentifierToken,
		GenericParameterListing genericParameterListing,
		FunctionArgumentListing functionArgumentListing,
		CodeBlockNode? codeBlockNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.FunctionDefinitionNode++;
		#endif
	
		AccessModifierKind = accessModifierKind;
		ReturnTypeClauseNode = returnTypeClauseNode;
		FunctionIdentifierToken = functionIdentifierToken;
		GenericParameterListing = genericParameterListing;
		FunctionArgumentListing = functionArgumentListing;
		CodeBlockNode = codeBlockNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public AccessModifierKind AccessModifierKind { get; }
	public TypeClauseNode ReturnTypeClauseNode { get; }
	public SyntaxToken FunctionIdentifierToken { get; }
	public GenericParameterListing GenericParameterListing { get; set; }
	public FunctionArgumentListing FunctionArgumentListing { get; private set; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; private set; }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.FunctionDefinitionNode;
	
	TypeClauseNode IExpressionNode.ResultTypeClauseNode => TypeFacts.Pseudo.ToTypeClause();
	
	public bool IsParsingGenericParameters { get; set; }
	
	public void SetFunctionArgumentListing(FunctionArgumentListing functionArgumentListing)
	{
		FunctionArgumentListing = functionArgumentListing;
		_childListIsDirty = true;
	}
	
	public void SetGenericParameterListing(GenericParameterListing genericParameterListing)
	{
		GenericParameterListing = genericParameterListing;
		_childListIsDirty = true;
	}
	
	public void SetGenericParameterListingCloseAngleBracketToken(SyntaxToken closeAngleBracketToken)
	{
		GenericParameterListing.SetCloseAngleBracketToken(closeAngleBracketToken);
		_childListIsDirty = true;
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

		var childCount = 2 +                                          // ReturnTypeClauseNode, FunctionIdentifierToken
			1 +                                                       // FunctionArgumentListing.OpenParenthesisToken
			FunctionArgumentListing.FunctionArgumentEntryList.Count + // FunctionArgumentListing.FunctionArgumentEntryList.Count
			1;                                                        // FunctionArgumentListing.CloseParenthesisToken
		if (GenericParameterListing.ConstructorWasInvoked)
		{
			childCount +=
				1 +                                                       // GenericParameterListing.OpenAngleBracketToken
				GenericParameterListing.GenericParameterEntryList.Count + // GenericParameterListing.GenericParameterEntryList.Count
				1;                                                        // GenericParameterListing.CloseAngleBracketToken
		}
		if (CodeBlockNode is not null)
			childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ReturnTypeClauseNode;
		childList[i++] = FunctionIdentifierToken;
		if (GenericParameterListing.ConstructorWasInvoked)
		{
			childList[i++] = GenericParameterListing.OpenAngleBracketToken;
			foreach (var entry in GenericParameterListing.GenericParameterEntryList)
			{
				childList[i++] = entry.TypeClauseNode;
			}
			childList[i++] = GenericParameterListing.CloseAngleBracketToken;
		}
		
		childList[i++] = FunctionArgumentListing.OpenParenthesisToken;
		foreach (var entry in FunctionArgumentListing.FunctionArgumentEntryList)
		{
			childList[i++] = entry.VariableDeclarationNode;
		}
		childList[i++] = FunctionArgumentListing.CloseParenthesisToken;
		
		if (CodeBlockNode is not null)
			childList[i++] = CodeBlockNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
