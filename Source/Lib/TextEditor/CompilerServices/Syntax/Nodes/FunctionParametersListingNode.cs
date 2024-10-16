using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a function.
/// </summary>
public sealed class FunctionParametersListingNode : ISyntaxNode
{
    public FunctionParametersListingNode(
        OpenParenthesisToken openParenthesisToken,
        ImmutableArray<FunctionParameterEntryNode> functionParameterEntryNodes,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        FunctionParameterEntryNodeList = functionParameterEntryNodes;
        CloseParenthesisToken = closeParenthesisToken;

        SetChildList();
    }

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public ImmutableArray<FunctionParameterEntryNode> FunctionParameterEntryNodeList { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionParametersListingNode;
    
    public void SetChildList()
    {
    	var children = new List<ISyntax>
        {
            OpenParenthesisToken
        };

        children.AddRange(FunctionParameterEntryNodeList);

        children.Add(CloseParenthesisToken);

        ChildList = children.ToImmutableArray();
    	throw new NotImplementedException();
    }
}
