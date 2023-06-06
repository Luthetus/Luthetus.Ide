using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundFunctionArgumentsNode : ISyntaxNode
{
    public BoundFunctionArgumentsNode(
        OpenParenthesisToken openParenthesisToken,
        List<ISyntax> boundFunctionArgumentListing,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        BoundFunctionArgumentListing = boundFunctionArgumentListing;
        CloseParenthesisToken = closeParenthesisToken;

        var children = new List<ISyntax>
        {
            OpenParenthesisToken
        };

        children.AddRange(BoundFunctionArgumentListing);

        children.Add(CloseParenthesisToken);

        Children = children.ToImmutableArray();
    }

    public OpenParenthesisToken OpenParenthesisToken { get; init; }
    public List<ISyntax> BoundFunctionArgumentListing { get; init; }
    public CloseParenthesisToken CloseParenthesisToken { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionArgumentsNode;
}
