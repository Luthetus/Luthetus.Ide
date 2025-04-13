using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class VariableDeclarationNode : IExpressionNode
{
	public VariableDeclarationNode(
		TypeReference typeReference,
		SyntaxToken identifierToken,
		VariableKind variableKind,
		bool isInitialized)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.VariableDeclarationNode++;
		#endif
	
		TypeReference = typeReference;
		IdentifierToken = identifierToken;
		VariableKind = variableKind;
		IsInitialized = isInitialized;
	}

	// private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public TypeReference TypeReference { get; private set; }

	public SyntaxToken IdentifierToken { get; }
	/// <summary>
	/// TODO: Remove the 'set;' on this property
	/// </summary>
	public VariableKind VariableKind { get; set; }
	public bool IsInitialized { get; set; }
	/// <summary>
	/// TODO: Remove the 'set;' on this property
	/// </summary>
	public bool HasGetter { get; set; }
	/// <summary>
	/// TODO: Remove the 'set;' on this property
	/// </summary>
	public bool GetterIsAutoImplemented { get; set; }
	/// <summary>
	/// TODO: Remove the 'set;' on this property
	/// </summary>
	public bool HasSetter { get; set; }
	/// <summary>
	/// TODO: Remove the 'set;' on this property
	/// </summary>
	public bool SetterIsAutoImplemented { get; set; }

	TypeReference IExpressionNode.ResultTypeReference => TypeFacts.Pseudo.ToTypeReference();

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.VariableDeclarationNode;

	public VariableDeclarationNode SetTypeReference(TypeReference typeReference)
	{
		TypeReference = typeReference;
		return this;
	}

	/*public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = new ISyntax[]
		{
			IdentifierToken,
		};

		_childListIsDirty = false;
		return _childList;
	}*/
}
