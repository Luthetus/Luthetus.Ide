namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class InheritanceStatementNode : ISyntaxNode
{
	public InheritanceStatementNode(TypeClauseNode parentTypeClauseNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.InheritanceStatementNode++;
		#endif
	
		ParentTypeClauseNode = parentTypeClauseNode;
	}

	public TypeClauseNode ParentTypeClauseNode { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.InheritanceStatementNode;

	#if DEBUG	
	~InheritanceStatementNode()
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.InheritanceStatementNode--;
	}
	#endif
}