using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundFunctionParametersNode : ISyntaxNode
{
    public BoundFunctionParametersNode(
        OpenParenthesisToken openParenthesisToken,
        List<ISyntax> boundFunctionParameterListing,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        BoundFunctionParameterListing = boundFunctionParameterListing;
        CloseParenthesisToken = closeParenthesisToken;

        var children = new List<ISyntax>
        {
            OpenParenthesisToken
        };

        children.AddRange(BoundFunctionParameterListing);

        children.Add(CloseParenthesisToken);

        Children = children.ToImmutableArray();
    }

    public OpenParenthesisToken OpenParenthesisToken { get; init; }
    public List<ISyntax> BoundFunctionParameterListing { get; init; }
    public CloseParenthesisToken CloseParenthesisToken { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionParametersNode;
}
