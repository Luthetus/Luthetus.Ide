using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record FunctionArgumentsListingNode : ISyntaxNode
{
    public FunctionArgumentsListingNode(
        OpenParenthesisToken openParenthesisToken,
        ImmutableArray<FunctionArgumentEntryNode> functionArgumentEntryNodes,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        FunctionArgumentEntryNodeBag = functionArgumentEntryNodes;
        CloseParenthesisToken = closeParenthesisToken;

        var children = new List<ISyntax>
        {
            OpenParenthesisToken
        };

        children.AddRange(FunctionArgumentEntryNodeBag);

        children.Add(CloseParenthesisToken);

        ChildBag = children.ToImmutableArray();
    }

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public ImmutableArray<FunctionArgumentEntryNode> FunctionArgumentEntryNodeBag { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionArgumentsListingNode;
}