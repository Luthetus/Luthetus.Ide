using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundGenericArgumentNode : ISyntaxNode
{
    public BoundGenericArgumentNode(
        OpenAngleBracketToken openAngleBracketToken,
        CloseAngleBracketToken closeAngleBracketToken)
    {
        OpenAngleBracketToken = openAngleBracketToken;
        CloseAngleBracketToken = closeAngleBracketToken;
        
        Children = new ISyntax[]
        {
            OpenAngleBracketToken,
            CloseAngleBracketToken,
        }.ToImmutableArray();
    }

    public OpenAngleBracketToken OpenAngleBracketToken { get; init; }
    public CloseAngleBracketToken CloseAngleBracketToken { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundGenericArgumentNode;
}
