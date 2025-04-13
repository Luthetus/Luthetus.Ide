using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.Extensions.CompilerServices;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ConstructorDefinitionNode : ICodeBlockOwner, IFunctionDefinitionNode
{
	public ConstructorDefinitionNode(
		TypeReference returnTypeReference,
		SyntaxToken functionIdentifier,
		GenericParameterListing genericParameterListing,
		FunctionArgumentListing functionArgumentListing,
		CodeBlock codeBlock)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ConstructorDefinitionNode++;
		#endif
	
		ReturnTypeReference = returnTypeReference;
		FunctionIdentifier = functionIdentifier;
		GenericParameterListing = genericParameterListing;
		FunctionArgumentListing = functionArgumentListing;
		CodeBlock = codeBlock;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public TypeReference ReturnTypeReference { get; }
	public SyntaxToken FunctionIdentifier { get; }
	public GenericParameterListing GenericParameterListing { get; }
	public FunctionArgumentListing FunctionArgumentListing { get; private set; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan OpenCodeBlockTextSpan { get; set; }
	public CodeBlock CodeBlock { get; set; }
	public TextEditorTextSpan CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ConstructorDefinitionNode;
	
	TypeReference IExpressionNode.ResultTypeReference => TypeFacts.Pseudo.ToTypeReference();
	
	public void SetFunctionArgumentListing(FunctionArgumentListing functionArgumentListing)
	{
		FunctionArgumentListing = functionArgumentListing;
		
		_childListIsDirty = true;
	}

	#region ICodeBlockOwner_Methods
	public TypeReference GetReturnTypeReference()
	{
		return ReturnTypeReference;
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

		
		var childCount = 1 +                                          // FunctionIdentifier
			1 +                                                       // FunctionArgumentListing.OpenParenthesisToken
			FunctionArgumentListing.FunctionArgumentEntryList.Count + // FunctionArgumentListing.FunctionArgumentEntryList.Count
			1;                                                        // FunctionArgumentListing.CloseParenthesisToken
		if (GenericParameterListing.ConstructorWasInvoked)
		{
			childCount +=
				1 +                                                       // GenericParameterListing.OpenAngleBracketToken
				// GenericParameterListing.GenericParameterEntryList.Count + // GenericParameterListing.GenericParameterEntryList.Count
				1;                                                        // GenericParameterListing.CloseAngleBracketToken
		}
		// if (CodeBlockNode is not null)
		// 	childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = FunctionIdentifier;
		if (GenericParameterListing.ConstructorWasInvoked)
		{
			childList[i++] = GenericParameterListing.OpenAngleBracketToken;
			/*foreach (var entry in GenericParameterListing.GenericParameterEntryList)
			{
				childList[i++] = entry.TypeClauseNode;
			}*/
			childList[i++] = GenericParameterListing.CloseAngleBracketToken;
		}
		
		childList[i++] = FunctionArgumentListing.OpenParenthesisToken;
		foreach (var entry in FunctionArgumentListing.FunctionArgumentEntryList)
		{
			childList[i++] = entry.VariableDeclarationNode;
		}
		childList[i++] = FunctionArgumentListing.CloseParenthesisToken;
		
		// if (CodeBlockNode is not null)
		// 	childList[i++] = CodeBlockNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
