using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// One usage of the <see cref="EmptyExpressionNode"/> is for a <see cref="ParenthesizedExpressionNode"/>
/// which has no <see cref="ParenthesizedExpressionNode.InnerExpression"/>
/// </summary>
public sealed class EmptyExpressionNode : IExpressionNode
{
	/// <summary>
	/// Luthetus.CompilerServices.CSharp.ParserCase.Internals.ParseOthers currently
	/// compares memory locations of nodes in 'public static IExpressionNode ParseExpression(CSharpParserModel model)'
	///
	/// So, if one uses this in conjunction with adding it to the IParserModel.ExpressionList then
	/// two it might result in discarding more entries in the list than was expected.
	///
	/// If this is used justs temporarily when instantiating a node, and never added to the
	/// IParserModel.ExpressionList then there should not be an issue.
	/// </summary>
	public static readonly EmptyExpressionNode Empty = new EmptyExpressionNode(TypeFacts.Empty.ToTypeClause());
	
	/// <inheritdoc cref="Empty"/>
	public static readonly EmptyExpressionNode EmptyFollowsMemberAccessToken = new EmptyExpressionNode(TypeFacts.Empty.ToTypeClause())
	{
		FollowsMemberAccessToken = true
	};

    public EmptyExpressionNode(TypeClauseNode typeClauseNode)
    {
        ResultTypeClauseNode = typeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode ResultTypeClauseNode { get; }
    public bool FollowsMemberAccessToken { get; init; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.EmptyExpressionNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 1; // ResultTypeClauseNode,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ResultTypeClauseNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}