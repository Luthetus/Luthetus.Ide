using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundGenericArgumentsNode : ISyntaxNode
{
    public BoundGenericArgumentsNode(
        OpenAngleBracketToken openAngleBracketToken,
        List<ISyntax> boundGenericArgumentListing,
        CloseAngleBracketToken closeAngleBracketToken)
    {
        OpenAngleBracketToken = openAngleBracketToken;
        BoundGenericArgumentListing = boundGenericArgumentListing;
        CloseAngleBracketToken = closeAngleBracketToken;

        var children = new List<ISyntax>
        {
            OpenAngleBracketToken
        };

        children.AddRange(BoundGenericArgumentListing);

        children.Add(CloseAngleBracketToken);

        Children = children.ToImmutableArray();
    }

    public OpenAngleBracketToken OpenAngleBracketToken { get; init; }
    public List<ISyntax> BoundGenericArgumentListing { get; init; }
    public CloseAngleBracketToken CloseAngleBracketToken { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundGenericArgumentsNode;
}
