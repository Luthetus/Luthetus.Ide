using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// <see cref="CommaSeparatedExpressionNode"/>, <see cref="TupleExpressionNode"/>,
/// and <see cref="LambdaExpressionNode"/> are all related.
///
/// But, <see cref="CommaSeparatedExpressionNode"/> is an incomplete expression.
/// The <see cref="CommaSeparatedExpressionNode"/> goes on to become either a
/// <see cref="TupleExpressionNode"/>, or <see cref="LambdaExpressionNode"/>
/// (by checking the tokens that follow the close parenthesis token).
/// </summary>
public sealed class CommaSeparatedExpressionNode : IExpressionNode
{
    public CommaSeparatedExpressionNode()
    {
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode ResultTypeClauseNode { get; } = TypeFacts.Pseudo.ToTypeClause();
    
    public List<IExpressionNode> InnerExpressionList { get; } = new();
    public CloseParenthesisToken CloseParenthesisToken { get; set; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CommaSeparatedExpressionNode;
    
    public void AddInnerExpressionNode(IExpressionNode expressionNode)
    {
    	InnerExpressionList.Add(expressionNode);
    	_childListIsDirty = true;
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = InnerExpressionList.ToArray();
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
