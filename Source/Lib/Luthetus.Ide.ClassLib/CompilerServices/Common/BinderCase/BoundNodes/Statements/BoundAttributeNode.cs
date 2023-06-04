using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundAttributeNode : ISyntaxNode
{
    public BoundAttributeNode(
        OpenSquareBracketToken openSquareBracketToken,
        CloseSquareBracketToken closeSquareBracketToken)
    {
        OpenSquareBracketToken = openSquareBracketToken;
        CloseSquareBracketToken = closeSquareBracketToken;
        
        Children = new ISyntax[]
        {
            OpenSquareBracketToken,
            CloseSquareBracketToken,
        }.ToImmutableArray();
    }

    public OpenSquareBracketToken OpenSquareBracketToken { get; init; }
    public CloseSquareBracketToken CloseSquareBracketToken { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundAttributeNode;
}
