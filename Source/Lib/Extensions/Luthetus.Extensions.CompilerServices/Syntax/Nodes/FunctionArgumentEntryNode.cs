namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a function.
/// </summary>
public sealed class FunctionArgumentEntryNode : ISyntaxNode
{
	public FunctionArgumentEntryNode(
		VariableDeclarationNode variableDeclarationNode,
		SyntaxToken? optionalCompileTimeConstantToken,
		bool isOptional,
		bool hasParamsKeyword,
		bool hasOutKeyword,
		bool hasInKeyword,
		bool hasRefKeyword)
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.FunctionArgumentEntryNode++;
	
		VariableDeclarationNode = variableDeclarationNode;
		OptionalCompileTimeConstantToken = optionalCompileTimeConstantToken;
		IsOptional = isOptional;
		HasParamsKeyword = hasParamsKeyword;
		HasOutKeyword = hasOutKeyword;
		HasInKeyword = hasInKeyword;
		HasRefKeyword = hasRefKeyword;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public VariableDeclarationNode VariableDeclarationNode { get; }
	public SyntaxToken? OptionalCompileTimeConstantToken { get; }
	public bool IsOptional { get; }
	public bool HasParamsKeyword { get; }
	public bool HasOutKeyword { get; }
	public bool HasInKeyword { get; }
	public bool HasRefKeyword { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.FunctionArgumentEntryNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 1; // VariableDeclarationNode,
		if (OptionalCompileTimeConstantToken is not null)
			childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = VariableDeclarationNode;
		if (OptionalCompileTimeConstantToken is not null)
			childList[i++] = OptionalCompileTimeConstantToken;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}