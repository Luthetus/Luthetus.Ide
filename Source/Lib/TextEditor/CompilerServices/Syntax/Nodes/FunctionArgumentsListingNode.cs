using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a function.
/// </summary>
public sealed class FunctionArgumentsListingNode : ISyntaxNode
{
    public FunctionArgumentsListingNode(
        OpenParenthesisToken openParenthesisToken,
        ImmutableArray<FunctionArgumentEntryNode> functionArgumentEntryNodes,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        FunctionArgumentEntryNodeList = functionArgumentEntryNodes;
        CloseParenthesisToken = closeParenthesisToken;

        SetChildList();
    }

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public ImmutableArray<FunctionArgumentEntryNode> FunctionArgumentEntryNodeList { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionArgumentsListingNode;
    
    public void SetChildList()
    {
    	var children = new List<ISyntax>
        {
            OpenParenthesisToken
        };

        children.AddRange(FunctionArgumentEntryNodeList);

        children.Add(CloseParenthesisToken);

        ChildList = children.ToImmutableArray();
    	throw new NotImplementedException();
    }
}