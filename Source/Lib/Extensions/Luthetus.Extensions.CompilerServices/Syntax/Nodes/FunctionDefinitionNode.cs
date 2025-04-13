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
		TypeReference returnTypeReference,
		SyntaxToken functionIdentifierToken,
		GenericParameterListing genericParameterListing,
		FunctionArgumentListing functionArgumentListing,
		CodeBlock codeBlock)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.FunctionDefinitionNode++;
		#endif
	
		AccessModifierKind = accessModifierKind;
		ReturnTypeReference = returnTypeReference;
		FunctionIdentifierToken = functionIdentifierToken;
		GenericParameterListing = genericParameterListing;
		FunctionArgumentListing = functionArgumentListing;
		CodeBlock = codeBlock;
	}

	public AccessModifierKind AccessModifierKind { get; }
	public TypeReference ReturnTypeReference { get; }
	public SyntaxToken FunctionIdentifierToken { get; }
	public GenericParameterListing GenericParameterListing { get; set; }
	public FunctionArgumentListing FunctionArgumentListing { get; private set; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan OpenCodeBlockTextSpan { get; set; }
	public CodeBlock CodeBlock { get; set; }
	public TextEditorTextSpan CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.FunctionDefinitionNode;
	
	TypeReference IExpressionNode.ResultTypeReference => TypeFacts.Pseudo.ToTypeReference();
	
	public bool IsParsingGenericParameters { get; set; }
	
	public void SetFunctionArgumentListing(FunctionArgumentListing functionArgumentListing)
	{
		FunctionArgumentListing = functionArgumentListing;
	}
	
	public void SetGenericParameterListing(GenericParameterListing genericParameterListing)
	{
		GenericParameterListing = genericParameterListing;
	}
	
	public void SetGenericParameterListingCloseAngleBracketToken(SyntaxToken closeAngleBracketToken)
	{
		GenericParameterListing.SetCloseAngleBracketToken(closeAngleBracketToken);
	}

	public ICodeBlockOwner SetExpressionBody(CodeBlock codeBlock)
	{
		CodeBlock = codeBlock;
		return this;
	}

	#region ICodeBlockOwner_Methods
	public TypeReference GetReturnTypeReference()
	{
		return ReturnTypeReference;
	}
	#endregion
}
