using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class VariableReferenceNode : IExpressionNode
{
	public VariableReferenceNode(
		SyntaxToken variableIdentifierToken,
		VariableDeclarationNode variableDeclarationNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.VariableReferenceNode++;
		#endif
	
		VariableIdentifierToken = variableIdentifierToken;
		VariableDeclarationNode = variableDeclarationNode;
	}
	
	public VariableReferenceNode(VariableReference variableReference)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.VariableReferenceNode++;
		#endif
	
		VariableIdentifierToken = variableReference.VariableIdentifierToken;
		VariableDeclarationNode = variableReference.VariableDeclarationNode;
		IsFabricated = variableReference.IsFabricated;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;
	
	private bool _isFabricated;
	
	public bool IsBeingUsed { get; set; } = false;

	public SyntaxToken VariableIdentifierToken { get; private set; }
	/// <summary>
	/// The <see cref="VariableDeclarationNode"/> is null when the variable is undeclared
	/// </summary>
	public VariableDeclarationNode VariableDeclarationNode { get; private set; }
	public TypeReference ResultTypeReference
	{
		get
		{
			if (VariableDeclarationNode is null)
				return TypeFacts.Empty.ToTypeReference();
			
			return VariableDeclarationNode.TypeReference;
		}
	}

	public bool IsFabricated
	{
		get
		{
			return _isFabricated;
		}
		init
		{
			_isFabricated = value;
		}
	}
	public SyntaxKind SyntaxKind => SyntaxKind.VariableReferenceNode;
	
	public void SetSharedInstance(
		SyntaxToken variableIdentifierToken,
		VariableDeclarationNode variableDeclarationNode)
	{
		IsBeingUsed = true;
	
		_childList = Array.Empty<ISyntax>();
		_childListIsDirty = true;
	
		VariableIdentifierToken = variableIdentifierToken;
		VariableDeclarationNode = variableDeclarationNode;
		_isFabricated = false;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = new ISyntax[]
		{
			VariableIdentifierToken,
			VariableDeclarationNode,
		};

		_childListIsDirty = false;
		return _childList;
	}
}
